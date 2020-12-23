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

    public GameObject Hop_0;
    public GameObject Hop_1;
    public GameObject Hop_2;
    public GameObject Hop_3;
    public GameObject Hop_4;
    public GameObject Hop_5;
    public GameObject Hop_6;
    public GameObject Hop_7;
	public GameObject BtnLeaderboard;
    public GameObject BtnAchievements;
    public GameObject BtnInventory;
    public GameObject BtnReward;
    public GameObject BluryMask;

    public GameObject BonusUI;

    public GameObject PrefabItemTpl;


    // Inventory Grid
    public GameObject InventoryUI;
    public GameObject CellPrefab;
    public int GridWidth = 8;
    public int GridHeight = 7;
    private List<GameObject> InventoryGridList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {

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

        Button btnHop_0 = Hop_0.GetComponent<Button>();
		btnHop_0.onClick.AddListener(delegate { LoadStage(0); });

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

                int wi = 0;
                if (weekday == "Monday")
                {
                    wi = 1;
                }
                if (weekday == "Tuesday")
                {
                    wi = 2;
                }
                if (weekday == "Wednesday")
                {
                    wi = 3;
                }
                if (weekday == "Thursday")
                {
                    wi = 4;
                }
                if (weekday == "Friday")
                {
                    wi = 5;
                }
                if (weekday == "Saturday")
                {
                    wi = 6;
                }
                if (weekday == "Sunday")
                {
                    wi = 7;
                }

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
                    btn.onClick.AddListener(delegate { ReceiveDailyBonus(transform, weekday); });
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
            SpaceXCoder.Save save = GameSave.Load();

            if (weekday == "Monday")
            {
                save.ReceiveItem("FogLight", 10);
            }
            else if (weekday == "Tuesday")
            {
                save.ReceiveItem("StopClock", 10);
            }
            else if (weekday == "Wednesday")
            {
                save.ReceiveItem("BombShortRange", 10);
            }
            else if (weekday == "Thursday")
            {
                save.ReceiveItem("RocketBomb", 10);
            }
            else if (weekday == "Friday")
            {
                save.ReceiveItem("Teleport", 10);
            }
            else if (weekday == "Saturday")
            {
                save.ReceiveItem("ExtraStar", 10);
            }
            else if (weekday == "Sunday")
            {
                /*
                save.ReceiveItem("FogLight", 10);
                save.ReceiveItem("StopClock", 10);
                save.ReceiveItem("BombShortRange", 10);
                save.ReceiveItem("RocketBomb", 10);
                save.ReceiveItem("ExtraStar", 10);
                save.ReceiveItem("Teleport", 10);
                */
                save.ReceiveItem("PowerOverwhelming", 10);
            }

            GameSave.Write(save);

            received.SetActive(true);
        }
    }

    void ShowInventoryUI()
    {
        Debug.Log("Inventory Show");

        Dictionary<string, Dictionary<string, string>> itemInfoDict = SpaceXCoder.CONST.ITEM_INFO;

        // begin inventory
        SpaceXCoder.Save saved = GameSave.Load();

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

    void LoadStage(int stage)
    {
        Debug.Log("About to start game stage: " + stage);
        PlayerPrefs.SetInt("stage", stage);
        SceneManager.LoadScene("Stage_" + stage);
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
