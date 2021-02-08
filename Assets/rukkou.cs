using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rukkou : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       var webView = gameObject.AddComponent<UniWebView>();
       webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
       
       // Load a URL.
       webView.Load("http://192.168.0.126:8000/demos/code/index.html");
       
       // Show it.
       webView.Show();       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
