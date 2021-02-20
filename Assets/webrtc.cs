using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SocketIOClient;
using SocketIOClient.ConnectInterval;
using TMPro;
using UnityEngine;
using UnityToolbag;
using Unity.WebRTC;
using System.Collections;
using UnityEngine.UI;

public class ISocketId
{
    public KeyValuePair<string, string> sid { get; set; }
}

public class ISocketIoWebRTCAnswer
{
    public string socket { get; set; }
    public RTCSessionDescription answer { get; set; }
}

public class webrtc : MonoBehaviour
{
    private RTCPeerConnection pc1, pc2;
    private Coroutine sdpCheck;
    private DelegateOnIceConnectionChange pc1OnIceConnectionChange;
    //private DelegateOnIceConnectionChange pc2OnIceConnectionChange;
    private DelegateOnIceCandidate pc1OnIceCandidate;
    //private DelegateOnIceCandidate pc2OnIceCandidate;
    private RTCDataChannel dataChannel, receiveChannel;
    private DelegateOnMessage onDataChannelMessage;
    private DelegateOnOpen onDataChannelOpen;

    public GameObject btn;
    public GameObject container;
    public static string MySocketId;
    public static string PeerSocketId;
    public static SocketIO MySocket;
    public static bool isAlreadyCalling = false;


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

    private void Awake()
    {
        // Initialize WebRTC
        WebRTC.Initialize();
        //callButton.onClick.AddListener(() => { StartCoroutine(Call()); });
        //hangupButton.onClick.AddListener(() => { Hangup(); });
    }
    private void OnDestroy()
    {
        if (pc1 != null)
            pc1.Close();
        //if (pc1 != null)
        //    pc2.Close();
        WebRTC.Dispose();

    }

    void SendText()
    {
        dataChannel.Send("Hello from SpaceXCoder!");
    }

    private IEnumerator Loop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            var desc = pc1.LocalDescription;
            Debug.Log($"pc1 sdp: {desc.sdp}");
        }
    }

    RTCConfiguration GetSelectedSdpSemantics()
    {
        RTCConfiguration config = default;
        /* Internet not supported
        config.iceServers = new RTCIceServer[]
        {
            new RTCIceServer { urls = new string[] { "stun:stun.l.google.com:19302" } }
        };
        */

        return config;
    }




    void OnIceConnectionChange(RTCPeerConnection pc, RTCIceConnectionState state)
    {
        switch (state)
        {
            case RTCIceConnectionState.New:
                Debug.Log($"{GetName(pc)} IceConnectionState: New");
                break;
            case RTCIceConnectionState.Checking:
                Debug.Log($"{GetName(pc)} IceConnectionState: Checking");
                break;
            case RTCIceConnectionState.Closed:
                Debug.Log($"{GetName(pc)} IceConnectionState: Closed");
                break;
            case RTCIceConnectionState.Completed:
                Debug.Log($"{GetName(pc)} IceConnectionState: Completed");
                break;
            case RTCIceConnectionState.Connected:
                Debug.Log($"{GetName(pc)} IceConnectionState: Connected");
                break;
            case RTCIceConnectionState.Disconnected:
                Debug.Log($"{GetName(pc)} IceConnectionState: Disconnected");
                break;
            case RTCIceConnectionState.Failed:
                Debug.Log($"{GetName(pc)} IceConnectionState: Failed");
                break;
            case RTCIceConnectionState.Max:
                Debug.Log($"{GetName(pc)} IceConnectionState: Max");
                break;
            default:
                break;
        }
    }

    void OnIceCandidate(RTCPeerConnection pc, RTCIceCandidate candidate)
    {
        //GetOtherPc(pc).AddIceCandidate(candidate);

        try
        {
            pc.AddIceCandidate(candidate);
            Debug.Log($"Add {GetName(pc)} ICE candidate:\n {candidate.Candidate}");
        }
        catch (Exception e)
        {
            Debug.Log("Exception caught." + e);
        }
    }
    string GetName(RTCPeerConnection pc)
    {
        return "pc1";
    }

    RTCPeerConnection GetOtherPc(RTCPeerConnection pc)
    {
        return (pc == pc1) ? pc2 : pc1;
    }


    void OnSetLocalSuccess(RTCPeerConnection pc)
    {
        Debug.Log($"{GetName(pc)} SetLocalDescription complete");
    }

    void OnSetSessionDescriptionError(ref RTCError error) { }

    void OnSetRemoteSuccess(RTCPeerConnection pc)
    {
        Debug.Log($"{GetName(pc)} SetRemoteDescription complete");
    }


    void OnCreateSessionDescriptionError(RTCError e)
    {

    }
    void Hangup()
    {
        Debug.Log("Ending call");
        StopCoroutine(sdpCheck);
        pc1.Close();
        pc2.Close();
        pc1 = null;
        pc2 = null;
        //hangupButton.interactable = false;
        //callButton.interactable = true;
    }
    IEnumerator _CallUser(string socket_id)
    {
        yield return MySocket.EmitAsync("hi", new Dictionary<string, string>() { { "making", "a call to " + socket_id } });
    }

    IEnumerator CallUser(string socket_id)
    {
        Debug.Log("pc1 createOffer start");

        var op = pc1.CreateOffer(ref OfferOptions);

        Debug.Log(op.Desc.type);

        yield return op;

        if (!op.IsError)
        {
            yield return StartCoroutine(OnCreateOfferSuccess(op.Desc, socket_id));
        }
        else
        {
            OnCreateSessionDescriptionError(op.Error);
        }
        //sdpCheck = StartCoroutine(Loop());
    }
    IEnumerator OnCreateOfferSuccess(RTCSessionDescription desc, string socket_id)
    {
        Debug.Log($"Offer from pc1\n{desc.sdp}");
        Debug.Log("pc1 setLocalDescription start");
        var op = pc1.SetLocalDescription(ref desc);
        yield return op;

        if (!op.IsError)
        {
            OnSetLocalSuccess(pc1);
            MySocket.EmitAsync("call-user", new Dictionary<string, object>() {
                { "offer", desc },
                { "to", socket_id }
            });
        }
        else
        {
            var error = op.Error;
            OnSetSessionDescriptionError(ref error);
        }
    }

    void Start()
    {

        GameObject.Find("Send").GetComponent<Button>().onClick.AddListener(delegate {
            SendText();
        });

        //callButton.interactable = true;
        pc1OnIceConnectionChange = state => { OnIceConnectionChange(pc1, state); };
        pc1OnIceCandidate = candidate => { OnIceCandidate(pc1, candidate); };

        var uri = new Uri("http://192.168.0.126:5003");

        var socket = new SocketIOClient.SocketIO(uri, new SocketIOClient.SocketIOOptions
        {
            Query = new Dictionary<string, string>
                {
                    //{"token", "io" }
                    {"token", "v3" }
                },
            EIO = 4
        });

        socket.GetConnectInterval = () => new MyConnectInterval();

        socket.OnConnected += Socket_OnConnected;
        socket.OnPing += Socket_OnPing;
        socket.OnPong += Socket_OnPong;
        socket.OnDisconnected += Socket_OnDisconnected;
        socket.OnReconnecting += Socket_OnReconnecting;

        int index = 0;
        socket.On("update-user-list", response =>
        {
            List<string> obj = response.GetValue<List<string>>();
            Debug.Log("update-user-list: ");
            obj.ForEach(socket_id =>
            {
                try
                {
                    Debug.Log("Peer Socket id (" + index + ") => " + socket_id);
                    index++;
                    Dispatcher.Invoke(() =>
                    {
                        GameObject.Find("MySocketId").GetComponent<TextMeshProUGUI>().SetText("MySocketId => " + MySocketId);
                        if (socket_id != MySocketId)
                        {
                            GameObject newObj = Instantiate(btn, container.transform);
                            newObj.transform.SetParent(container.transform);
                            newObj.transform.Find("text").GetComponent<TextMeshProUGUI>().SetText("Call " + socket_id);
                            newObj.GetComponent<Button>().onClick.AddListener(delegate {

                                Debug.Log("GetSelectedSdpSemantics");
                                var configuration = GetSelectedSdpSemantics();
                                pc1 = new RTCPeerConnection(ref configuration);
                                Debug.Log("Created local peer connection object pc1");
                                pc1.OnIceCandidate = pc1OnIceCandidate;
                                pc1.OnIceConnectionChange = pc1OnIceConnectionChange;
                                pc1.OnIceGatheringStateChange = state => { Debug.Log("XXX " + state); };
                                pc1.OnDataChannel = channel => { 
                                    receiveChannel = channel;
                                    receiveChannel.OnMessage = bytes => { Debug.Log("Received!!! => " + System.Text.Encoding.UTF8.GetString(bytes)); };
                                };
                                RTCDataChannelInit conf = new RTCDataChannelInit();
                                dataChannel = pc1.CreateDataChannel("sendChannel", conf);
                                dataChannel.OnOpen = onDataChannelOpen;


                                StartCoroutine(CallUser(socket_id));
                            });
                        }
                    });
                }
                catch (Exception e)
                {
                    Debug.Log("Exception caught." + e);
                }
            });
        });

        socket.On("answer-made", response =>
        {
            ISocketIoWebRTCAnswer obj = response.GetValue<ISocketIoWebRTCAnswer>();

            Dispatcher.Invoke(() =>
            {
                Debug.Log("Answer Made !!! ");
                Debug.Log(obj.socket);
                Debug.Log(obj.answer);
                PeerSocketId = obj.socket;
                var debug = JsonConvert.SerializeObject(obj.answer, Formatting.Indented);
                Debug.Log(debug);
                //var answer = new RTCSessionDescription { type = RTCSdpType.Answer, sdp = obj.answer.sdp };
                //debug = JsonConvert.SerializeObject(answer);
                //Debug.Log(debug);
                StartCoroutine(OnCreateAnswerSuccess(obj.answer));
            });
        });


        socket.ConnectAsync();

        MySocket = socket;
    }


    IEnumerator OnCreateAnswerSuccess(RTCSessionDescription desc)
    {
        Debug.Log("pc1 setRemoteDescription start");

        var op2 = pc1.SetRemoteDescription(ref desc);
        yield return op2;
        if (!op2.IsError)
        {
            if (isAlreadyCalling == false)
            {
                StartCoroutine(CallUser(PeerSocketId));
                isAlreadyCalling = true;
            }
            OnSetRemoteSuccess(pc1);
        }
        else
        {
            var error = op2.Error;
            OnSetSessionDescriptionError(ref error);
        }
    }

    private static void Socket_OnReconnecting(object sender, int e)
    {
        Debug.Log($"Reconnecting: attempt = {e}");
    }

    private static void Socket_OnDisconnected(object sender, string e)
    {
        Debug.Log("disconnect: " + e);
    }

    private static async void Socket_OnConnected(object sender, EventArgs e)
    {
        Debug.Log("Socket_OnConnected");
        var socket = sender as SocketIO;
        
        Dictionary<string, string> socket_id = JsonConvert.DeserializeObject<Dictionary<string, string>>(socket.Id);
        Debug.Log("My Socket Id 1 => " + socket_id["sid"]);
        MySocketId = socket_id["sid"];
        //ISocketId obj = JsonConvert.DeserializeObject<ISocketId>(socket.Id);
        //Debug.Log("My Socket Id 4 => " + obj.sid);

        await socket.EmitAsync("hi", new Dictionary<string, string>() {{ "foo", "bar" }});

        //await socket.EmitAsync("ack", response =>
        //{
        //    Debug.Log(response.ToString());
        //}, "SocketIOClient.Sample");

        //await socket.EmitAsync("bytes", "c#", new
        //{
        //    source = "client007",
        //    bytes = Encoding.UTF8.GetBytes("dot net")
        //});

        //socket.On("client binary callback", async response =>
        //{
        //    await response.CallbackAsync();
        //});

        //await socket.EmitAsync("client binary callback", Encoding.UTF8.GetBytes("SocketIOClient.Sample"));

        //socket.On("client message callback", async response =>
        //{
        //    await response.CallbackAsync(Encoding.UTF8.GetBytes("CallbackAsync();"));
        //});
        //await socket.EmitAsync("client message callback", "SocketIOClient.Sample");
    }

    private static void Socket_OnPing(object sender, EventArgs e)
    {
        Debug.Log("Ping");
    }

    private static void Socket_OnPong(object sender, TimeSpan e)
    {
        Debug.Log("Pong: " + e.TotalMilliseconds);
    }
}

public class MyConnectInterval : IConnectInterval
{
    public MyConnectInterval()
    {
        delay = 1000;
    }

    double delay;

    public int GetDelay()
    {
        Debug.Log("GetDelay: " + delay);
        return (int)delay;
    }

    public double NextDealy()
    {
        Debug.Log("NextDealy: " + (delay + 1000));
        return delay += 1000;
    }
}


class ByteResponse
{
    public string ClientSource { get; set; }

    public string Source { get; set; }

    [JsonProperty("bytes")]
    public byte[] Buffer { get; set; }
}

class ClientCallbackResponse
{
    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("bytes")]
    public byte[] Bytes { get; set; }
}