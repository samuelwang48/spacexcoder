using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{

    public GameObject Hop_0;
    public GameObject Hop_1;
    public GameObject Hop_2;
    public GameObject Hop_3;
    public GameObject Hop_4;
    public GameObject Hop_5;
    public GameObject Hop_6;
    public GameObject Hop_7;


    // Start is called before the first frame update
    void Start()
    {

        Button btnHop_0 = Hop_0.GetComponent<Button>();
        btnHop_0.onClick.AddListener(delegate { LoadGame(0); });

    }

    void LoadGame(int difficulty)
    {
        Debug.Log("About to start game difficulty: " + difficulty);
        PlayerPrefs.SetInt("difficulty", difficulty);
        SceneManager.LoadScene("Main Scene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
