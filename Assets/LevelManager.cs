using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public GameObject ObjExitStage;

    // Start is called before the first frame update
    void Start()
    {
        Button btnExitStage = ObjExitStage.GetComponent<Button>();
        btnExitStage.onClick.AddListener(delegate { ExitStage(); });


        // by default it is zero
        int unlocked = PlayerPrefs.GetInt("unlocked");
        Debug.Log("Unlocked level: " + unlocked);

        //LvButton = GameObject.FindGameObjectsWithTag("lvbtn");

        UnityEngine.Color colorActive = new UnityEngine.Color();
        UnityEngine.ColorUtility.TryParseHtmlString("#fff", out colorActive);


        for (int i = 0; i < 20; i++)
        {
            GameObject lvbtn = GameObject.Find("Lv" + i);
            if (i <= unlocked)
            {
                lvbtn.GetComponent<Image>().color = colorActive;
                int level = i;
                lvbtn.GetComponent<Button>().onClick.AddListener(delegate { LoadLevel(level); });
            }
        }
    }

    void LoadLevel(int level)
    {
        Debug.Log("About to start game level: " + level);
        PlayerPrefs.SetInt("level", level);
        SceneManager.LoadScene("Game Scene");
    }

    void ExitStage()
    {
        SceneManager.LoadScene("Main Scene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
