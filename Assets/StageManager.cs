using System.Collections.Generic;
using Coffee.UIExtensions;
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
	public GameObject BtnLeaderboard;
    public GameObject BtnAchievements;
    public GameObject BtnInventory;
    public GameObject BluryMask;

    // Inventory Grid
    public GameObject InventoryUI;
    public GameObject CellPrefab;
    public int GridWidth = 8;
    public int GridHeight = 7;
    private List<GameObject> GridList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        BluryMask.SetActive(false);
        InventoryUI.SetActive(false);

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

        PopulateGrid();
    }

    void ShowInventoryUI()
    {
        Debug.Log("Inventory Show");
        BluryMask.GetComponentInChildren<UIEffectCapturedImage>().Capture();
        BluryMask.SetActive(true);
        InventoryUI.SetActive(true);
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
            GridList.Add(newObj);
        }
    }
}
