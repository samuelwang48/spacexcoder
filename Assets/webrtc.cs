using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using SocketIOClient;
using SocketIOClient.ConnectInterval;
using TMPro;
using UnityEngine;
using UnityToolbag;
using Unity.WebRTC;
using UnityEngine.UI;
using System.IO;

public class webrtc : MonoBehaviour {
    public GameObject btn;
    public GameObject container;
    public static TMP_InputField Output;
    public static string OutputText = "";

    //socket members
    public Uri uri = new Uri("https://server-dev.rukkou.com:5003");
    public static string MySocketId = "Unknown";
    public static SocketIO MySocket;

    //rtc members
    public static bool isAlreadyCalling = false;
    private RTCPeerConnection peerConnection;
    private RTCDataChannel dataChannel, receiveChannel;
    private RTCOfferOptions OfferOptions = new RTCOfferOptions {
        iceRestart = false,
        offerToReceiveAudio = false,
        offerToReceiveVideo = false
    };
    private RTCAnswerOptions AnswerOptions = new RTCAnswerOptions {
        iceRestart = false,
    };

    private delegate void SDPCallback(RTCSessionDescription desc, string socket_id);

    private void Awake() {
    }

    public static void DebugLog(string output) {
        Debug.Log(output);
        Dispatcher.Invoke(() => {
            Output.SetTextWithoutNotify(OutputText + output + "\n");
            OutputText = OutputText + output + "\n";
        });
    }

    void Start() {
        Output = GameObject.Find("Output").GetComponent<TMP_InputField>();
        DebugLog($"Listening...");

        // Initialize WebRTC
        DebugLog("WebRTC Initialize");
        WebRTC.Initialize();

        RTCConfiguration config = default;

        DebugLog($"RTCPeerConnection {config}");
        peerConnection = new RTCPeerConnection(ref config);
        peerConnection.OnIceCandidate = candidate => { Rtc_OnIceCandidate(peerConnection, candidate); };
        peerConnection.OnIceConnectionChange = state => { Rtc_OnIceConnectionChange(peerConnection, state); };
        peerConnection.OnIceGatheringStateChange = state => { DebugLog("peerConnection " + state); };

        peerConnection.OnDataChannel = channel => {
            receiveChannel = channel;
            receiveChannel.OnMessage = bytes => { DebugLog("Received!!! => " + System.Text.Encoding.UTF8.GetString(bytes)); };
        };
        dataChannel = peerConnection.CreateDataChannel("sendChannel", new RTCDataChannelInit());
        dataChannel.OnOpen = () => { };


        // Initialize WebSocket
        DebugLog("WebSocket Initialize");
        MySocket = new SocketIO(uri, new SocketIOOptions {
            Query = new Dictionary<string, string> { { "token", "v3" } },
            EIO = 4
        });

        MySocket.GetConnectInterval = () => new MyConnectInterval();
        MySocket.OnError += Socket_OnError;
        MySocket.OnConnected += Socket_OnConnected;
        MySocket.OnPing += Socket_OnPing;
        MySocket.OnPong += Socket_OnPong;
        MySocket.OnDisconnected += Socket_OnDisconnected;
        MySocket.OnReconnecting += Socket_OnReconnecting;


        GameObject.Find("Send").GetComponent<Button>().onClick.AddListener(delegate {
            dataChannel.Send("Hello from SpaceXCoder!");
        });

        int index = 0;
        MySocket.On("update-user-list", response => {
            List<string> obj = response.GetValue<List<string>>();
            DebugLog("update-user-list: ");
            obj.ForEach(socket_id => {
                try {
                    DebugLog("Peer Socket id (" + index + ") => " + socket_id);
                    index++;
                    Dispatcher.Invoke(() => {
                        GameObject.Find("MySocketId").GetComponent<TextMeshProUGUI>().SetText("MySocketId => " + MySocketId);
                        if (socket_id != MySocketId) {
                            GameObject newObj = Instantiate(btn, container.transform);
                            newObj.transform.SetParent(container.transform);
                            newObj.transform.Find("text").GetComponent<TextMeshProUGUI>().SetText("Call " + socket_id);
                            newObj.GetComponent<Button>().onClick.AddListener(delegate {
                                StartCoroutine(MakeCall(socket_id));
                            });
                        }
                    });
                } catch (Exception e) {
                    DebugLog("Exception caught." + e);
                }
            });
        });

        MySocket.On("answer-made", response => {
            ISocketIoWebRTCAnswer obj = response.GetValue<ISocketIoWebRTCAnswer>();
            Dispatcher.Invoke(() => {
                StartCoroutine(Rtc_SetRemoteSDP(obj.answer, obj.socket, (desc, socket_id) => {
                    DebugLog($"peerConnection SetRemoteDescription complete");
                    if (isAlreadyCalling == false) {
                        StartCoroutine(MakeCall(socket_id));
                        isAlreadyCalling = true;
                    }
                })); 
            });
            //var debug = JsonConvert.SerializeObject(obj.answer, Formatting.Indented); DebugLog(debug);
            //var answer = new RTCSessionDescription { type = RTCSdpType.Answer, sdp = obj.answer.sdp };
        });

        MySocket.On("call-made", response => {
            ISocketIoWebRTCOffer obj = response.GetValue<ISocketIoWebRTCOffer>();
            Dispatcher.Invoke(() => {
                StartCoroutine(Rtc_SetRemoteSDP(obj.offer, obj.socket, (desc, socket_id) => {
                    DebugLog($"peerConnection SetRemoteDescription complete");
                    StartCoroutine(MakeAnswer(socket_id, (answer, _socket_id) => {
                        StartCoroutine(Rtc_SetLocalSDP(answer, _socket_id, (_answer, to) => {
                            DebugLog($"peerConnection SetLocalDescription complete");
                            MySocket.EmitAsync("make-answer", new Dictionary<string, object>() {
                                { "answer", _answer },
                                { "to", to }
                            });
                        }));
                    }));
                }));
            });
        });

        MySocket.ConnectAsync();
    }

    IEnumerator MakeAnswer(string socket_id, SDPCallback sdpCallback) {
        DebugLog("$peerConnection createAnswer start {socket_id}");
        var createAnswer = peerConnection.CreateAnswer(ref AnswerOptions);
        yield return createAnswer;

        if (!createAnswer.IsError) {
            sdpCallback(createAnswer.Desc, socket_id);
        } else {
            DebugLog("$peerConnection createOffer error {createAnswer.Error}");
        }
    }
    IEnumerator MakeCall(string socket_id) {
        DebugLog("$peerConnection createOffer start {socket_id}");
        var createOffer = peerConnection.CreateOffer(ref OfferOptions);
        yield return createOffer;

        if (!createOffer.IsError) {
            yield return StartCoroutine(Rtc_SetLocalSDP(createOffer.Desc, socket_id, (offer, to) => {
                DebugLog($"peerConnection SetLocalDescription complete");
                MySocket.EmitAsync("make-call", new Dictionary<string, object>() {
                    { "offer", offer },
                    { "to", to }
                });
            }));
        } else {
            DebugLog("$peerConnection createOffer error {createOffer.Error}");
        }
    }
    IEnumerator Rtc_SetRemoteSDP(RTCSessionDescription desc, string socket_id, SDPCallback sdpCallback) {
        DebugLog($"peerConnection setRemoteDescription start {desc.sdp}");
        var setRemote = peerConnection.SetRemoteDescription(ref desc);
        yield return setRemote;

        if (!setRemote.IsError) {
            sdpCallback(desc, socket_id);
        } else {
            DebugLog($"peerConnection SetRemoteDescription error {setRemote.Error}");
        }
    }

    IEnumerator Rtc_SetLocalSDP(RTCSessionDescription desc, string socket_id, SDPCallback sdpCallback) {
        DebugLog($"peerConnection setLocalDescription start {desc.sdp} {socket_id}");
        var setLocal = peerConnection.SetLocalDescription(ref desc);
        yield return setLocal;

        if (!setLocal.IsError) {
            sdpCallback(desc, socket_id);
        } else {
            DebugLog($"peerConnection SetLocalDescription error {setLocal.Error}");
        }
    }


    void Rtc_OnIceConnectionChange(RTCPeerConnection peerConnection, RTCIceConnectionState state) {
        switch (state) {
            case RTCIceConnectionState.New:
                DebugLog($"peerConnection IceConnectionState: New");
                break;
            case RTCIceConnectionState.Checking:
                DebugLog($"peerConnection IceConnectionState: Checking");
                break;
            case RTCIceConnectionState.Closed:
                DebugLog($"peerConnection IceConnectionState: Closed");
                break;
            case RTCIceConnectionState.Completed:
                DebugLog($"peerConnection IceConnectionState: Completed");
                break;
            case RTCIceConnectionState.Connected:
                DebugLog($"peerConnection IceConnectionState: Connected");
                break;
            case RTCIceConnectionState.Disconnected:
                DebugLog($"peerConnection IceConnectionState: Disconnected");
                break;
            case RTCIceConnectionState.Failed:
                DebugLog($"peerConnection IceConnectionState: Failed");
                break;
            case RTCIceConnectionState.Max:
                DebugLog($"peerConnection IceConnectionState: Max");
                break;
            default:
                break;
        }
    }

    void Rtc_OnIceCandidate(RTCPeerConnection peerConnection, RTCIceCandidate candidate) {
        peerConnection.AddIceCandidate(candidate);
        DebugLog($"Add peerConnection ICE candidate:\n {candidate.Candidate}");
    }

    private static async void Socket_OnConnected(object sender, EventArgs e) {
        SocketIO socket = sender as SocketIO;
        Dictionary<string, string> socket_dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(socket.Id);
        MySocketId = socket_dict["sid"];
        DebugLog($"Socket Connected MySocketId => {MySocketId} | Socket Event => {e}");
        await MySocket.EmitAsync("hi", new Dictionary<string, string>() { { "hi", "from SpaceXCoder" } });
    }

    private static void Socket_OnError(object sender, string e) { DebugLog($"Socket Error: attempt = {e}"); }
    private static void Socket_OnReconnecting(object sender, int e) { DebugLog($"Socket Reconnecting: attempt = {e}"); }
    private static void Socket_OnDisconnected(object sender, string e) { DebugLog($"Socket Disconnect: {e}"); }
    private static void Socket_OnPing(object sender, EventArgs e) { DebugLog($"Socket Ping => {e}"); }
    private static void Socket_OnPong(object sender, TimeSpan e) { DebugLog($"Socket Pong => {e.TotalMilliseconds}"); }

    private void OnDestroy() {
        if (peerConnection != null) { peerConnection.Close(); }
        WebRTC.Dispose();
        MySocket.DisconnectAsync();
    }

}




public class MyConnectInterval : IConnectInterval {
    public MyConnectInterval() { delay = 1000; }

    double delay;
    public int GetDelay() { Debug.Log("GetDelay: " + delay); return (int)delay; }
    public double NextDealy() { Debug.Log("NextDealy: " + (delay + 1000)); return delay += 1000; }
}









public class ISocketId {
    public KeyValuePair<string, string> sid { get; set; }
}
public class ISocketIoWebRTCAnswer {
    public string socket { get; set; }
    public RTCSessionDescription answer { get; set; }
}
public class ISocketIoWebRTCOffer {
    public string socket { get; set; }
    public RTCSessionDescription offer { get; set; }
}