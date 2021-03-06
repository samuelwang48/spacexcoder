using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using SpaceXCoder;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public GameObject ObjExitStage;
    public GameObject BtnVehicles;

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

        Button btnVehicles = BtnVehicles.GetComponent<Button>();
        btnVehicles.onClick.AddListener(delegate {
            PlayerPrefs.SetString("VehicleExitToScene", "Stage_0");
            SceneManager.LoadScene("Vehicles");
        });

        SpaceXCoder.Save save = GameService.LoadSave();
        // by default it is zero
        int unlocked = save.Unlocked();
        Debug.Log("Unlocked level: " + unlocked);
        Debug.Log("lvRecords: " + save.lvRecords);

        for (int i = 0; i < save.lvRecords.Length; i++)
        {
            LvRecord lvRecord = save.GetLvRecord(i);
            Debug.Log("Lv: " + i + ", " + lvRecord.score + ", " + lvRecord.timeLeft);
        }

        UnityEngine.Color colorActive = new UnityEngine.Color();
        UnityEngine.ColorUtility.TryParseHtmlString("#fff", out colorActive);

        string stageName = PlayerPrefs.GetString("stage");
        Debug.Log("stageName => " + stageName);

        List<Dictionary<string, object>> stage = GameService.ReadLevelsByStage(stageName);
        int levels_per_stage = stage.Count;
        Debug.Log("Mars levels_per_stage => " + levels_per_stage);
        for (int i = 0; i < MAX_LEVEL_PER_STAGE; i++)
        {
            GameObject lvbtn = GameObject.Find("Lv" + i);
            if (i >= levels_per_stage)
            {
                lvbtn.SetActive(false);
            }
            else
            {
                int current = (int)stage[i]["Level"];
                Debug.Log("Mars processing level => " + "Lv" + current);
                lvbtn.transform.Find("LevelName").GetComponent<TextMeshProUGUI>().SetText("Lv " + current.ToString());

                LvRecord lvRecord = save.GetLvRecord(current);
                int score = lvRecord.score;

                if (current <= unlocked)
                {
                    lvbtn.GetComponent<Image>().color = colorActive;
                    lvbtn.GetComponent<Button>().onClick.AddListener(delegate { LoadLevel(current); });

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
