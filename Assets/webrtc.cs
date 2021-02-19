using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using SocketIOClient;
using SocketIOClient.ConnectInterval;
using TMPro;
using UnityEngine;
using UnityToolbag;

public class ISocketId
{
    public KeyValuePair<string, string> sid { get; set; }
}

public class webrtc : MonoBehaviour
{
    public GameObject btn;
    public GameObject container;
    public static string MySocketId;

    void Start()
    {

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
                        GameObject newObj = Instantiate(btn, container.transform);
                        newObj.transform.SetParent(container.transform);
                        newObj.transform.Find("text").GetComponent<TextMeshProUGUI>().SetText(socket_id);
                    });
                }
                catch (Exception e)
                {
                    Debug.Log("Exception caught." + e);
                }
            });
        });


        socket.ConnectAsync();
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