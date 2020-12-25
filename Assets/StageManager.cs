using System.Collections.Generic;
using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SpaceXCoder;
using System.Linq;
using TMPro;
using System;

public class StageManager : MonoBehaviour
{
    public GameObject BtnLeaderboard;
    public GameObject BtnAchievements;
    public GameObject BtnInventory;
    public GameObject BtnReward;
    public GameObject BtnVehicles;
    public GameObject BluryMask;

    public GameObject BonusUI;

    public GameObject PrefabItemTpl;


    // Inventory Grid
    public GameObject InventoryUI;
    public GameObject CellPrefab;
    public int GridWidth = 8;
    public int GridHeight = 7;
    private List<GameObject> InventoryGridList = new List<GameObject>();
    public Dictionary<string, Dictionary<string, object>> DailyBonus = SpaceXCoder.CONST.DAILY_BONUS;

    // Start is called before the first frame update
    void Start()
    {
        SpaceXCoder.Save save = GameService.LoadSave();
        GameService.Write(save);

        int unlocked = save.Unlocked();

        PopulateGrid();

        BluryMask.SetActive(false);
        InventoryUI.SetActive(false);
        BonusUI.SetActive(false);

        Social.localUser.Authenticate(success => {
            if (success)
            {
                Debug.Log("Authentication successful");
                string userInfo = "Username: " + Social.localUser.userName +
                    "\nUser ID: " + Social.localUser.id +
                    "\nIsUnderage: " + Social.localUser.underage;
                Debug.Log(userInfo);
            }
            else
                Debug.Log("Authentication failed");
        });

        List<string> stages = GameService.ReadStagesByChapter("Solar");

        Debug.Log("Solar Stages => " + string.Join(", ", stages));

        for (int i = 0; i < stages.Count; i++)
        {
            GameObject stageObj = GameObject.Find("T_" + i);
            Debug.Log("stageObj => " + stageObj);
            Button stageBtn = stageObj.GetComponent<Button>();
            Debug.Log("stageBtn => " + stageBtn);

            int startFrom = (int)GameService.ReadLevelsByStage(stages[i])[0]["Level"];

            Debug.Log("startFrom => " + startFrom);
            if (startFrom <= unlocked)
            {
                int j = i;
                stageBtn.onClick.AddListener(delegate { LoadStage(stages[j]); });
                GameObject.Find("Lock_" + i).SetActive(false);
            }
            else
            {
                stageObj.SetActive(false);
            }
        }

        Button btnLeaderboard = BtnLeaderboard.GetComponent<Button>();
        btnLeaderboard.onClick.AddListener(delegate { Social.ShowLeaderboardUI(); });

        Button btnAchievements = BtnAchievements.GetComponent<Button>();
        btnAchievements.onClick.AddListener(delegate { Social.ShowAchievementsUI(); });

        Button btnInventory = BtnInventory.GetComponent<Button>();
        btnInventory.onClick.AddListener(delegate { ShowInventoryUI(); });

        Button btnInventoryClose = InventoryUI.transform.Find("BtnClose").GetComponent<Button>();
        btnInventoryClose.onClick.AddListener(delegate { HideInventoryUI(); });

        Button btnReward = BtnReward.GetComponent<Button>();
        btnReward.onClick.AddListener(delegate { ShowBonusUI(); });

        Button btnVehicles = BtnVehicles.GetComponent<Button>();
        btnVehicles.onClick.AddListener(delegate { SceneManager.LoadScene("Vehicles"); });
        


        Button btnRewardClose = BonusUI.transform.Find("BtnClose").GetComponent<Button>();
        btnRewardClose.onClick.AddListener(delegate { HideBonusUI(); });


        BonusUI.transform.GetComponentsInChildren<Transform>().Where(t => t.name == "Weekday").ToList().ForEach(transform =>
        {
            Button btn = transform.GetComponent<Button>();
            string weekday = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text;
            
            if (weekday == "Monday" || weekday == "Tuesday" || weekday == "Wednesday" || weekday == "Thursday" || weekday == "Friday" || weekday == "Saturday" || weekday == "Sunday")
            {

                UIEffect effect = btn.transform.Find("Reward").GetComponent<UIEffect>();

                DateTime today = System.DateTime.Now;
                Debug.Log("Today => " + today);
                Debug.Log("Today => " + (int)today.DayOfWeek);
                Debug.Log("Today => " + today.DayOfWeek);

                int wi = (int)DailyBonus[weekday]["weekIndex"];

                int ti = 0;
                if ((int)today.DayOfWeek == 0)
                {
                    ti = 7;
                } else
                {
                    ti = (int)today.DayOfWeek;
                }

                if (wi <= ti)
                {
                    effect.effectFactor = 0f;
                    GameObject received = transform.Find("Received").gameObject;
                    string itemType = (string)DailyBonus[weekday]["itemType"];
                    int itemAmount = (int)DailyBonus[weekday]["itemAmount"];

                    if (save.HasClockedIn(today, itemType, itemAmount) == true)
                    {
                        Debug.Log("HasClockedIn => true");
                        received.SetActive(true);
                    }
                    else
                    {
                        Debug.Log("HasClockedIn => false");
                        btn.onClick.AddListener(delegate { ReceiveDailyBonus(transform, weekday); });
                    }
                } else if (wi > ti)
                {
                    effect.effectFactor = 1f;
                }
            }
        });
    }

    void ReceiveDailyBonus(Transform transform, string weekday)
    {
        GameObject received = transform.Find("Received").gameObject;
        if (received.activeSelf == false)
        {
            SpaceXCoder.Save save = GameService.LoadSave();

            string itemType = (string)DailyBonus[weekday]["itemType"];
            int itemAmount = (int)DailyBonus[weekday]["itemAmount"];
            save.ReceiveItem(itemType, itemAmount);

            GameService.Write(save);

            received.SetActive(true);
        }
    }

    void ShowInventoryUI()
    {
        Debug.Log("Inventory Show");

        Dictionary<string, Dictionary<string, string>> itemInfoDict = SpaceXCoder.CONST.ITEM_INFO;

        // begin inventory
        SpaceXCoder.Save saved = GameService.LoadSave();

        // Render inventory items
        Dictionary<string, int> dict = saved.ListItemDict();

        int cellIndex = 0;
        for (int index = 0; index < dict.Count; index++)
        {
            var kv = dict.ElementAt(index);
            Debug.Log("key value pair: " + kv.Key + "=>" + kv.Value);
            Transform InventoryCell = InventoryGridList[cellIndex].transform;

            if (kv.Value > 0)
            {
                GameObject newObj = Instantiate(PrefabItemTpl, InventoryCell) as GameObject;
                newObj.transform.SetParent(InventoryCell);
                newObj.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>(itemInfoDict[kv.Key]["Sprite"]);
                newObj.transform.Find("Qty").GetComponent<TextMeshProUGUI>().SetText(kv.Value.ToString());
                newObj.transform.Find("Life").gameObject.SetActive(false);
                newObj.transform.Find("Image/CD").gameObject.SetActive(false);

                cellIndex++;
            }
        }
        // End inventory

        BluryMask.GetComponentInChildren<UIEffectCapturedImage>().Capture();
        BluryMask.SetActive(true);
        InventoryUI.SetActive(true);
    }

    void HideInventoryUI()
    {
        Debug.Log("Inventory Hide");
        BluryMask.SetActive(false);
        InventoryUI.SetActive(false);
    }

    void ShowBonusUI()
    {
        Debug.Log("Inventory Show");
        BluryMask.GetComponentInChildren<UIEffectCapturedImage>().Capture();
        BluryMask.SetActive(true);
        BonusUI.SetActive(true);

        /*
        DateTime dateValue;
        dateValue = new DateTime(2020, 12, 21);
        Debug.Log((int)dateValue.DayOfWeek);
        dateValue = new DateTime(2020, 12, 22);
        Debug.Log((int)dateValue.DayOfWeek);
        dateValue = new DateTime(2020, 12, 23);
        Debug.Log((int)dateValue.DayOfWeek);
        dateValue = new DateTime(2020, 12, 24);
        Debug.Log((int)dateValue.DayOfWeek);
        dateValue = new DateTime(2020, 12, 25);
        Debug.Log((int)dateValue.DayOfWeek);
        dateValue = new DateTime(2020, 12, 26);
        Debug.Log((int)dateValue.DayOfWeek);
        dateValue = new DateTime(2020, 12, 27);
        Debug.Log((int)dateValue.DayOfWeek);
        dateValue = new DateTime(2020, 12, 28);
        Debug.Log((int)dateValue.DayOfWeek);


        DateTime today = System.DateTime.Now;
        Debug.Log("Today => " + today);
        Debug.Log("Today => " + (int)today.DayOfWeek);
        */
    }

    void HideBonusUI()
    {
        Debug.Log("Inventory Hide");
        BluryMask.SetActive(false);
        BonusUI.SetActive(false);
    }

    void LoadStage(string stage)
    {
        Debug.Log("About to start game stage: " + stage);
        PlayerPrefs.SetString("stage", stage);
        SceneManager.LoadScene("Stage_0");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PopulateGrid()
    {
        int numberToCreate = GridWidth * GridHeight;
        Transform containerTransform = InventoryUI.transform.Find("GridContainer");
        GameObject newObj;

        GridLayoutGroup glg = containerTransform.gameObject.GetComponent<GridLayoutGroup>();
        glg.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        glg.constraintCount = GridHeight;

        for (int i = 0; i < numberToCreate; i++)
        {
            newObj = (GameObject)Instantiate(CellPrefab, containerTransform);
            InventoryGridList.Add(newObj);
        }
    }
}
