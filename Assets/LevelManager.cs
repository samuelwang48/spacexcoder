using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public GameObject[] LvButton;

    // Start is called before the first frame update
    void Start()
    {
        // by default it is zero
        int unlocked = PlayerPrefs.GetInt("unlocked");
        Debug.Log("Unlocked level: " + unlocked);

        LvButton = GameObject.FindGameObjectsWithTag("lvbtn");

        UnityEngine.Color colorActive = new UnityEngine.Color();
        UnityEngine.ColorUtility.TryParseHtmlString("#fff", out colorActive);

        for (int i = 0; i < LvButton.Length; i++)
        {
            GameObject lvbtn = LvButton[i];
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
        SceneManager.LoadScene("Main Scene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
