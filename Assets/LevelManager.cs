using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using SpaceXCoder;

public class LevelManager : MonoBehaviour
{
    public GameObject ObjExitStage;

    public UnityEngine.Color ColorWinnerStarDark = new UnityEngine.Color(.24f, .26f, .38f, 1f);
    public UnityEngine.Color ColorWinnerStarBright = new UnityEngine.Color(0f, 0.728f, 1f, 1f);
    public UnityEngine.Color ColorWinnerStarHidden = new UnityEngine.Color(1f, 1f, 1f, 0f);
    public const int MAX_LEVEL_PER_STAGE = 20;

    // Start is called before the first frame update
    void Start()
    {
        InitStage();
    }

    void InitStage()
    {
        Button btnExitStage = ObjExitStage.GetComponent<Button>();
        btnExitStage.onClick.AddListener(delegate { ExitStage(); });

        SpaceXCoder.Save save = GameService.LoadSave();
        // by default it is zero
        int unlocked = save.unlocked;
        Debug.Log("Unlocked level: " + unlocked);
        Debug.Log("lvRecords: " + save.lvRecords);

        for (int i = 0; i < save.lvRecords.Length; i++)
        {
            LvRecord lvRecord = save.lvRecords[i];
            Debug.Log("Lv: " + i + ", " + lvRecord.score + ", " + lvRecord.timeLeft);
        }

        //LvButton = GameObject.FindGameObjectsWithTag("lvbtn");

        UnityEngine.Color colorActive = new UnityEngine.Color();
        UnityEngine.ColorUtility.TryParseHtmlString("#fff", out colorActive);


        List<Dictionary<string, object>> stage = GameService.ReadLevelsByStage("Mars");
        int levels_per_stage = stage.Count;
        Debug.Log("Mars levels_per_stage => " + levels_per_stage);
        for (int i = 0; i < MAX_LEVEL_PER_STAGE; i++)
        {
            GameObject lvbtn = GameObject.Find("Lv" + i);
            Debug.Log("Mars processing level => " + "Lv" + i);
            if (i >= levels_per_stage)
            {
                lvbtn.SetActive(false);
            }
            
            LvRecord lvRecord = save.lvRecords[i];
            int score = lvRecord.score;

            if (i <= unlocked)
            {
                lvbtn.GetComponent<Image>().color = colorActive;
                int level = i;
                lvbtn.GetComponent<Button>().onClick.AddListener(delegate { LoadLevel(level); });
            }

            Transform bsc = lvbtn.transform.GetChild(1).transform;
            for (int bsi = 0; bsi < bsc.childCount; bsi++)
            {
                GameObject bs = bsc.GetChild(bsi).gameObject;
                if (bsi < score)
                {
                    bs.GetComponent<Image>().color = ColorWinnerStarBright;
                }
                else
                {
                    bs.GetComponent<Image>().color = ColorWinnerStarDark;
                }
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
