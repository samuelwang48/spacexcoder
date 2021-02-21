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


public class webrtc : MonoBehaviour
{
    public GameObject btn;
    public GameObject container;

    //socket members
    public Uri uri = new Uri("http://192.168.0.126:5003");
    public static string MySocketId;
    public static string PeerSocketId;
    public static SocketIO MySocket;

    //rtc members
    public static bool isAlreadyCalling = false;
    private RTCPeerConnection peerConnection;
    private RTCDataChannel dataChannel, receiveChannel;
    private RTCOfferOptions OfferOptions = new RTCOfferOptions
    {
        iceRestart = false,
        offerToReceiveAudio = false,
        offerToReceiveVideo = false
    };
    private RTCAnswerOptions AnswerOptions = new RTCAnswerOptions
    {
        iceRestart = false,
    };

    private void Awake() {
        // Initialize WebRTC
        WebRTC.Initialize();
        RTCConfiguration config = default;
        peerConnection = new RTCPeerConnection(ref config);
        peerConnection.OnIceCandidate = candidate => { Rtc_OnIceCandidate(peerConnection, candidate); };
        peerConnection.OnIceConnectionChange = state => { Rtc_OnIceConnectionChange(peerConnection, state); };
        peerConnection.OnIceGatheringStateChange = state => { Debug.Log("XXX " + state); };

        peerConnection.OnDataChannel = channel => {
            receiveChannel = channel;
            receiveChannel.OnMessage = bytes => { Debug.Log("Received!!! => " + System.Text.Encoding.UTF8.GetString(bytes)); };
        };
        dataChannel = peerConnection.CreateDataChannel("sendChannel", new RTCDataChannelInit());
        dataChannel.OnOpen = () => { };


        // Initialize WebSocket
        MySocket = new SocketIO(uri, new SocketIOOptions
        {
            Query = new Dictionary<string, string>{ {"token", "v3" } },
            EIO = 4
        });

        MySocket.GetConnectInterval = () => new MyConnectInterval();
        MySocket.OnConnected += Socket_OnConnected;
        MySocket.OnPing += Socket_OnPing;
        MySocket.OnPong += Socket_OnPong;
        MySocket.OnDisconnected += Socket_OnDisconnected;
        MySocket.OnReconnecting += Socket_OnReconnecting;
    }

    IEnumerator CallUser(string socket_id) {
        Debug.Log("$peerConnection createOffer start {socket_id}");
        var createOffer = peerConnection.CreateOffer(ref OfferOptions);
        yield return createOffer;

        if (!createOffer.IsError) {
            yield return StartCoroutine(Rtc_OnCreateOfferSuccess(createOffer.Desc, socket_id));
        } else {
            Debug.Log("$peerConnection createOffer error {createOffer.Error}");
        }
    }

    void Start() {
        GameObject.Find("Send").GetComponent<Button>().onClick.AddListener(delegate {
            dataChannel.Send("Hello from SpaceXCoder!");
        });

        int index = 0;
        MySocket.On("update-user-list", response => {
            List<string> obj = response.GetValue<List<string>>();
            Debug.Log("update-user-list: ");
            obj.ForEach(socket_id => {
                try {
                    Debug.Log("Peer Socket id (" + index + ") => " + socket_id);
                    index++;
                    Dispatcher.Invoke(() => {
                        GameObject.Find("MySocketId").GetComponent<TextMeshProUGUI>().SetText("MySocketId => " + MySocketId);
                        if (socket_id != MySocketId)
                        {
                            GameObject newObj = Instantiate(btn, container.transform);
                            newObj.transform.SetParent(container.transform);
                            newObj.transform.Find("text").GetComponent<TextMeshProUGUI>().SetText("Call " + socket_id);
                            newObj.GetComponent<Button>().onClick.AddListener(delegate {
                                StartCoroutine(CallUser(socket_id));
                            });
                        }
                    });
                } catch (Exception e) {
                    Debug.Log("Exception caught." + e);
                }
            });
        });

        MySocket.On("answer-made", response => {
            ISocketIoWebRTCAnswer obj = response.GetValue<ISocketIoWebRTCAnswer>();
            PeerSocketId = obj.socket;
            Dispatcher.Invoke(() => { StartCoroutine(Rtc_OnCreateAnswerSuccess(obj.answer)); });
            //var debug = JsonConvert.SerializeObject(obj.answer, Formatting.Indented); Debug.Log(debug);
            //var answer = new RTCSessionDescription { type = RTCSdpType.Answer, sdp = obj.answer.sdp };
        });

        MySocket.ConnectAsync();
    }
    IEnumerator Rtc_OnCreateAnswerSuccess(RTCSessionDescription desc) {
        Debug.Log($"peerConnection setRemoteDescription start {desc.sdp}");
        var setRemote = peerConnection.SetRemoteDescription(ref desc);
        yield return setRemote;

        if (!setRemote.IsError) {
            Debug.Log($"peerConnection SetRemoteDescription complete");
            if (isAlreadyCalling == false) {
                StartCoroutine(CallUser(PeerSocketId));
                isAlreadyCalling = true;
            }
        } else {
            Debug.Log($"peerConnection SetRemoteDescription error {setRemote.Error}");
        }
    }
    IEnumerator Rtc_OnCreateOfferSuccess(RTCSessionDescription desc, string socket_id) {
        Debug.Log($"peerConnection setLocalDescription start {desc.sdp} {socket_id}");
        var setLocal = peerConnection.SetLocalDescription(ref desc);
        yield return setLocal;

        if (!setLocal.IsError) {
            Debug.Log($"peerConnection SetLocalDescription complete");
            MySocket.EmitAsync("call-user", new Dictionary<string, object>() {
                { "offer", desc },
                { "to", socket_id }
            });
        } else {
            Debug.Log($"peerConnection SetLocalDescription error {setLocal.Error}");
        }
    }

    void Rtc_OnIceConnectionChange(RTCPeerConnection peerConnection, RTCIceConnectionState state)
    {
        switch (state)
        {
            case RTCIceConnectionState.New:
                Debug.Log($"peerConnection IceConnectionState: New");
                break;
            case RTCIceConnectionState.Checking:
                Debug.Log($"peerConnection IceConnectionState: Checking");
                break;
            case RTCIceConnectionState.Closed:
                Debug.Log($"peerConnection IceConnectionState: Closed");
                break;
            case RTCIceConnectionState.Completed:
                Debug.Log($"peerConnection IceConnectionState: Completed");
                break;
            case RTCIceConnectionState.Connected:
                Debug.Log($"peerConnection IceConnectionState: Connected");
                break;
            case RTCIceConnectionState.Disconnected:
                Debug.Log($"peerConnection IceConnectionState: Disconnected");
                break;
            case RTCIceConnectionState.Failed:
                Debug.Log($"peerConnection IceConnectionState: Failed");
                break;
            case RTCIceConnectionState.Max:
                Debug.Log($"peerConnection IceConnectionState: Max");
                break;
            default:
                break;
        }
    }

    void Rtc_OnIceCandidate(RTCPeerConnection peerConnection, RTCIceCandidate candidate)
    {
        peerConnection.AddIceCandidate(candidate);
        Debug.Log($"Add peerConnection ICE candidate:\n {candidate.Candidate}");
    }

    private static async void Socket_OnConnected(object sender, EventArgs e)
    {
        SocketIO socket = sender as SocketIO;
        Dictionary<string, string> socket_dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(socket.Id);
        MySocketId = socket_dict["sid"];
        Debug.Log($"Socket Connected MySocketId => {MySocketId} | Socket Event => {e}");
        await MySocket.EmitAsync("hi", new Dictionary<string, string>() { { "hi", "from SpaceXCoder" } });
    }
    private static void Socket_OnReconnecting(object sender, int e) { Debug.Log($"Socket Reconnecting: attempt = {e}"); }
    private static void Socket_OnDisconnected(object sender, string e) { Debug.Log($"Socket Disconnect: {e}"); }
    private static void Socket_OnPing(object sender, EventArgs e) { Debug.Log($"Socket Ping => {e}"); }
    private static void Socket_OnPong(object sender, TimeSpan e) { Debug.Log($"Socket Pong => {e.TotalMilliseconds}"); }

    private void OnDestroy()
    {
        if (peerConnection != null) { peerConnection.Close(); }
        WebRTC.Dispose();
    }
}

public class MyConnectInterval : IConnectInterval
{
    public MyConnectInterval() { delay = 1000; }

    double delay;
    public int GetDelay() { Debug.Log("GetDelay: " + delay); return (int)delay; }
    public double NextDealy() { Debug.Log("NextDealy: " + (delay + 1000)); return delay += 1000; }
}













public class ISocketId
{
    public KeyValuePair<string, string> sid { get; set; }
}

public class ISocketIoWebRTCAnswer
{
    public string socket { get; set; }
    public RTCSessionDescription answer { get; set; }
}