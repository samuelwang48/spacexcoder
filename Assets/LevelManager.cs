using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public GameObject Lv_0;

    // Start is called before the first frame update
    void Start()
    {

        Button btnLv_0 = Lv_0.GetComponent<Button>();
        btnLv_0.onClick.AddListener(delegate { LoadLevel(0); });

    }

    void LoadLevel(int level)
    {
        Debug.Log("About to start game level: " + level);
        PlayerPrefs.SetInt("level", level);
        SceneManager.LoadScene("Main Scene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
