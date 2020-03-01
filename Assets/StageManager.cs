using System.Collections.Generic;
using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SpaceXCoder;
using System.Linq;
using TMPro;

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

        // begin inventory
        SpaceXCoder.Save save = GameSave.Load();

        // Receive 1 item for free
        save.ReceiveItem("FogLight", 2);
        save.ReceiveItem("StopClock", 1);
        GameSave.Write(save);

        Dictionary<string, Sprite> itemSprite = new Dictionary<string, Sprite>();
        itemSprite.Add("FogLight", Resources.Load<Sprite>("EngineeringCraftIcons/bg/addons/engeniring_06_b"));
        itemSprite.Add("StopClock", Resources.Load<Sprite>("EngineeringCraftIcons/bg/addons/engeniring_33_b"));


        // Render inventory items
        Dictionary<string, int> dict = save.ListItemDict();
        for (int index = 0; index < dict.Count; index++)
        {
            var kv = dict.ElementAt(index);
            Debug.Log("key value pair: " + kv.Key + "=>" + kv.Value);
            Transform InventoryCell = InventoryGridList[index].transform;

            if (kv.Value > 0)
            {
                GameObject newObj = Instantiate(PrefabItemTpl, InventoryCell) as GameObject;
                newObj.transform.SetParent(InventoryCell);
                newObj.transform.Find("Image").GetComponent<Image>().sprite = itemSprite[kv.Key];
                newObj.transform.Find("Qty").GetComponent<TextMeshProUGUI>().SetText(kv.Value.ToString());
            }
        }
        // End inventory

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


    }

    void ShowInventoryUI()
    {
        Debug.Log("Inventory Show");
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
