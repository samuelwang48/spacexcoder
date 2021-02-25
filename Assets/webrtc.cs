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
using System.Threading;
using System.Threading.Tasks;

public class MySocketIO : SocketIO {
    public MySocketIO(Uri uri, SocketIOOptions options) : base(uri, options) {
    }
}

public class webrtc : MonoBehaviour {
    //socket members
    public Uri uri = new Uri("http://server-dev.rukkou.com:5003");
    public static string MySocketId = "Unknown";
    public static SocketIO MySocket;

    private void Awake() {
    }

    public static void DebugLog(string output) {
        Debug.Log(output);
    }

    async void WebsocketConnect() {
        Debug.Log($"WebsocketConnect Thread {System.Threading.Thread.CurrentThread.ManagedThreadId}");
        await MySocket.ConnectAsync();
    }

    void Start() {
        GameObject.Find("WSConnect").GetComponent<Button>().onClick.AddListener(delegate {
            Debug.Log($"Start Thread {System.Threading.Thread.CurrentThread.ManagedThreadId}");
            // Initialize WebSocket
            DebugLog("WebSocket Initialize");
            MySocket = new MySocketIO(uri, new SocketIOOptions {
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

            Thread thread_1 = new Thread(WebsocketConnect);
            thread_1.Start();
        });
    }

    private static async void Socket_OnConnected(object sender, EventArgs e) {
        Debug.Log($"Socket_OnConnected Thread {System.Threading.Thread.CurrentThread.ManagedThreadId}");
        await MySocket.EmitAsync("hi", new Dictionary<string, string>() { { "hi", "from SpaceXCoder" } });
    }

    private static void Socket_OnError(object sender, string e) { DebugLog($"Socket Error: attempt = {e}"); }
    private static void Socket_OnReconnecting(object sender, int e) { DebugLog($"Socket Reconnecting: attempt = {e}"); }
    private static void Socket_OnDisconnected(object sender, string e) { DebugLog($"Socket Disconnect: {e}"); }
    private static void Socket_OnPing(object sender, EventArgs e) {
        Debug.Log($"Socket_OnPing Thread {System.Threading.Thread.CurrentThread.ManagedThreadId}");
        DebugLog($"Socket Ping => {e}");
    }
    private static void Socket_OnPong(object sender, TimeSpan e) {
        Debug.Log($"Socket_OnPong Thread {System.Threading.Thread.CurrentThread.ManagedThreadId}");
        DebugLog($"Socket Pong => {e.TotalMilliseconds}");
    }

    private void OnDestroy() {
        MySocket.DisconnectAsync();
    }

}


public class MyConnectInterval : IConnectInterval {
    public MyConnectInterval() { delay = 100; }

    double delay;
    public int GetDelay() { Debug.Log("GetDelay: " + delay); return (int)delay; }
    public double NextDealy() { Debug.Log("NextDealy: " + (delay + 100)); return delay += 100; }
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