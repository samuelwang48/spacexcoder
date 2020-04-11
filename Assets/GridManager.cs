using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;
using TMPro;
using SpaceXCoder;
using Coffee.UIExtensions;

public class Rover
{
    public int X;
    public int Y;
    public int dir = 0;
    public GameObject GameObject;

}

public class Rock
{
    public int X;
    public int Y;
    public GameObject GameObject;

}
public class Resource
{
    public int X;
    public int Y;
    public GameObject GameObject;

}

public class GridManager : MonoBehaviour
{
    public GameObject prefab;
    public GameObject RoverPrefab;
    public GameObject RockPrefab;
    public GameObject ResourcePrefab;
    public GameObject InventoryCellPrefab;
    public GameObject InstructionView;

    public GameObject GridScrollView;
    public GameObject BannerResourceContainer;
    public GameObject WinModal;
    public GameObject LoseModal;
    public GameObject GameInventoryOverlay;
    public GameObject PrefabItemTpl;

    public GameObject ObjTurnLeft;
    public GameObject ObjTurnRight;
    public GameObject ObjForward;
    public GameObject ObjUndo;
    public GameObject ObjSend;
    public GameObject ObjGameInventory;
    public GameObject ObjGameItemOverlay;

    public GameObject ObjTimeLeftText;
    public GameObject ObjProgressBg;
    public GameObject ObjProgressFg;
    public UnityEngine.Color ColorProgressBg100 = new UnityEngine.Color(1f, 0f, 0f, 1f);
    public UnityEngine.Color ColorProgressBg70 = new UnityEngine.Color(1f, 0f, 0f, 1f);
    public UnityEngine.Color ColorProgressBg50 = new UnityEngine.Color(1f, 0f, 0f, 1f);
    public UnityEngine.Color ColorProgressBg40 = new UnityEngine.Color(1f, 0f, 0f, 1f);


    public GameObject ObjWall;

    public GameObject ObjFinishContinue;
    public GameObject ObjForceExit;
    public GameObject ObjPlayAgain;
    public GameObject ObjGameOverExit;

    public GameObject ObjTimeLeftCountdown;
    public GameObject ObjWinnerStarContainer;

    public GameObject ObjFog;

    private const float ZEROF = 0f;
    private const int SCORE_PER_SEC = 30;
    private const int MAX_STAR_NUMBER = 3;

    public string LeaderboardID = "spacexcoder.uat.toptesters";
    public string Achievement_0 = "spacexcoder.uat.unlock10";

    public int GridWidth = 11;
    public int GridHeight = 17;
    public int RockQty = 5;
    public int ResourceQty = 3;
    // public float DistanceX = 1f;
    // public float DistanceY = 1f;
    // public float ObjPosZ = -1f;
    public float MinFogScale = 1f;
    private float BaseFogScale = 10f;
    public float FogGrowSpeed = 0.1f;
    private bool IsFogReady = false;

    private string[] DIR = { "Up", "Right", "Down", "Left" };

    private List<GameObject> GridList = new List<GameObject>();
    private Rover Rover;
    private List<Rock> RockList = new List<Rock>();
    private List<Resource> ResList = new List<Resource>();
    private List<GameObject> BannerResourceList = new List<GameObject>();
    private List<string> Instruction = new List<string>();
    private List<GameObject> InventoryGridList = new List<GameObject>();

    public UnityEngine.Color ColorRockNormal = new UnityEngine.Color(0.84f, 0.81f, 0f, 1f);
    public UnityEngine.Color ColorRockError = new UnityEngine.Color(1f, 0f, 0f, 1f);
    public UnityEngine.Color ColorRockRemoved = new UnityEngine.Color(1f, 1f, 1f, 0.2f);

    public UnityEngine.Color ColorBannerResourceDark = new UnityEngine.Color(0.27f, 0.27f, 0.27f, 1f);
    public UnityEngine.Color ColorBannerResourceBright = new UnityEngine.Color(1f, 1f, 1f, 1f);
    public UnityEngine.Color ColorBannerResourceHidden = new UnityEngine.Color(1f, 1f, 1f, 0f);

    public UnityEngine.Color ColorWinnerStarDark = new UnityEngine.Color(0f, 0f, 0f, 0.5f);
    public UnityEngine.Color ColorWinnerStarBright = new UnityEngine.Color(0f, 0.728f, 1f, 1f);
    public UnityEngine.Color ColorWinnerStarHidden = new UnityEngine.Color(1f, 1f, 1f, 0f);

    public UnityEngine.Color ColorCtrlBtnDark = new UnityEngine.Color(0.92f, 0.33f, 0.33f, 0.3f);
    public UnityEngine.Color ColorCtrlBtnBright = new UnityEngine.Color(0.92f, 0.33f, 0.33f, 1f);
    public UnityEngine.Color ColorCtrlBtnHidden = new UnityEngine.Color(1f, 1f, 1f, 0f);

    public UnityEngine.Color ColorCtrlBtnContentDark = new UnityEngine.Color(1f, 1f, 1f, 0.2f);
    public UnityEngine.Color ColorCtrlBtnContentBright = new UnityEngine.Color(1f, 1f, 1f, 1f);
    public UnityEngine.Color ColorCtrlBtnContentHidden = new UnityEngine.Color(1f, 1f, 1f, 0f);


    private float TimeLeft = 90f; //~0.0003f fog speed
    private float TimeSpent = 0f;
    private float ProgressBgWidth;
    private float ProgressBgHeight;
    private Vector2 GridSize;
    private Vector2 GridSpacing;
    private RectOffset GridPadding;
    private int EarnedResourceCount = 0;

    private bool IsGameFrozen = false;
    private bool IsInstExecuting = false;

    private int CurrentLevel;

    private int QtyToBeUsed = 1;

    private int CurrentGameItemIndex;
    private GameObject CurrentGameItemObj;

    public GameObject EffectShortRangeBomb;
    public GameObject EffectTeleport;

    void InitLevel()
    {
        GridLayoutGroup glg = gameObject.GetComponent<GridLayoutGroup>();
        CurrentLevel = PlayerPrefs.GetInt("level");
        //CurrentLevel = 19;
        Debug.Log("Current level is: " + CurrentLevel);

        switch (CurrentLevel)
        {
            case 0:
                GridWidth = 3;
                GridHeight = 3;
                RockQty = 0;
                ResourceQty = 1;
                FogGrowSpeed = 0.0003f;
                break;
            case 1:
                GridWidth = 3;
                GridHeight = 3;
                RockQty = 3;
                ResourceQty = 2;
                FogGrowSpeed = 0.0003f;
                break;
            case 2:
                GridWidth = 5;
                GridHeight = 5;
                RockQty = 4;
                ResourceQty = 3;
                FogGrowSpeed = 0.0003f;
                break;
            case 3:
                GridWidth = 5;
                GridHeight = 5;
                RockQty = 5;
                ResourceQty = 4;
                FogGrowSpeed = 0.0003f;
                break;
            case 4:
                GridWidth = 5;
                GridHeight = 5;
                RockQty = 6;
                ResourceQty = 5;
                FogGrowSpeed = 0.0003f; //90s
                break;
            case 5:
                GridWidth = 6;
                GridHeight = 6;
                RockQty = 8;
                ResourceQty = 5;
                FogGrowSpeed = 0.0006f;
                break;
            case 6:
                GridWidth = 7;
                GridHeight = 7;
                RockQty = 11;
                ResourceQty = 5;
                FogGrowSpeed = 0.0007f;
                break;
            case 7:
                GridWidth = 8;
                GridHeight = 8;
                RockQty = 15;
                ResourceQty = 5;
                FogGrowSpeed = 0.0008f;
                break;
            case 8:
                GridWidth = 9;
                GridHeight = 9;
                RockQty = 19;
                ResourceQty = 5;
                FogGrowSpeed = 0.0009f;
                break;
            case 9:
                GridWidth = 10;
                GridHeight = 10;
                RockQty = 24;
                ResourceQty = 5;
                FogGrowSpeed = 0.0010f;
                break;
            case 10:
                GridWidth = 11;
                GridHeight = 11;
                RockQty = 30;
                ResourceQty = 5;
                FogGrowSpeed = 0.0011f;
                break;
            case 11:
                GridWidth = 11;
                GridHeight = 12;
                RockQty = 32;
                ResourceQty = 5;
                FogGrowSpeed = 0.0012f;
                break;
            case 12:
                GridWidth = 11;
                GridHeight = 13;
                RockQty = 35;
                ResourceQty = 5;
                FogGrowSpeed = 0.0013f;
                break;
            case 13:
                GridWidth = 11;
                GridHeight = 14;
                RockQty = 37;
                ResourceQty = 5;
                FogGrowSpeed = 0.0016f;
                break;
            case 14:
                GridWidth = 11;
                GridHeight = 15;
                RockQty = 40;
                ResourceQty = 5;
                FogGrowSpeed = 0.0019f;
                break;
            case 15:
                GridWidth = 11;
                GridHeight = 16;
                RockQty = 43;
                ResourceQty = 5;
                FogGrowSpeed = 0.0022f;
                break;
            case 16:
                GridWidth = 11;
                GridHeight = 17;
                RockQty = 45;
                ResourceQty = 5;
                FogGrowSpeed = 0.0025f;
                break;
            case 17:
                GridWidth = 11;
                GridHeight = 17;
                RockQty = 45;
                ResourceQty = 5;
                FogGrowSpeed = 0.0028f;
                break;
            case 18:
                GridWidth = 11;
                GridHeight = 17;
                RockQty = 50;
                ResourceQty = 5;
                FogGrowSpeed = 0.003f;
                break;
            case 19:
                GridWidth = 12;
                GridHeight = 17;
                RockQty = 65;
                ResourceQty = 5;
                FogGrowSpeed = 0.0035f;
                break;
            default:
                break;
        }

        if (GridHeight >= 17)
        {
            glg.cellSize = new Vector2(87f, 87f);
        }
    }

    void Start()
    {
        Time.timeScale = 1f;

        InitLevel();

        InitWinModal();
        InitLoseModal();
        InitGameInventoryOverlay();
        InitGameItemOverlay();

        InitBanner();

        CenterGrid();

        PopulateGrid();

        GenMap();

        StartCoroutine(PositionFog());

        InitButtons();

        InitEarnedStar();
    }

    void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void InitWinModal()
    {
        WinModal.SetActive(false);
    }

    void InitLoseModal()
    {
        LoseModal.SetActive(false);
    }

    void InitGameInventoryOverlay()
    {

        int gridWidth = 2;
        int gridHeight = 5;
        int numberToCreate = gridWidth * gridHeight;
        int cellIndex = 0;
        bool isDefaultClicked = false;

        Dictionary<string, Dictionary<string, string>> itemInfo = SpaceXCoder.CONST.ITEM_INFO;
        Dictionary<string, int> dict = GameSave.Load().ListItemDict();
        Transform containerTransform = GameInventoryOverlay.transform.Find("ItemGrid");
        GameObject newCell;

        /*
        GridLayoutGroup glg = containerTransform.gameObject.GetComponent<GridLayoutGroup>();
        glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        glg.constraintCount = gridWidth;
        */

        // GameInventoryOverlay.SetActive(false);

        for (int i = 0; i < numberToCreate; i++)
        {
            newCell = (GameObject)Instantiate(InventoryCellPrefab, containerTransform);
            InventoryGridList.Add(newCell);
        }

        for (int index = 0; index < dict.Count; index++)
        {
            var kv = dict.ElementAt(index);
            Debug.Log("key value pair: " + kv.Key + "=>" + kv.Value);
            Transform InventoryCell = InventoryGridList[cellIndex].transform;

            //if (kv.Value > 0)
            //{
            GameObject newObj = Instantiate(PrefabItemTpl, InventoryCell) as GameObject;
            newObj.transform.SetParent(InventoryCell);
            newObj.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>(itemInfo[kv.Key]["Sprite"]);
            newObj.transform.Find("Qty").GetComponent<TextMeshProUGUI>().SetText(kv.Value.ToString());
            Button itemBtn = newObj.GetComponent<Button>();
            int i = index;
            itemBtn.onClick.AddListener(delegate {
                GameItemClicked(newObj, i);
                UseGameItem();
            });
            cellIndex++;

            if (isDefaultClicked == false)
            {
                GameItemClicked(newObj, index);
                isDefaultClicked = true;
            }
            //}
        }
    }

    void GameItemClicked(GameObject gameItem, int index)
    {
        Dictionary<string, Dictionary<string, string>> itemInfoDict = SpaceXCoder.CONST.ITEM_INFO;
        Dictionary<string, int> dict = GameSave.Load().ListItemDict();

        KeyValuePair<string, int> kv = dict.ElementAt(index);
        Dictionary<string, string> itemInfo = itemInfoDict[kv.Key];

        Transform t = ObjGameItemOverlay.transform;
        Button btnMoreItem = t.Find("Qty/BtnMoreItem").GetComponent<Button>();
        Button btnLessItem = t.Find("Qty/BtnLessItem").GetComponent<Button>();
        Button btnUse = t.Find("Use").GetComponent<Button>();
        TextMeshProUGUI itemQty = t.Find("Qty/GameItemQty/Text").GetComponent<TextMeshProUGUI>();

        CurrentGameItemObj = gameItem;
        CurrentGameItemIndex = index;

        Debug.Log("Game Item index: " + index);
        Debug.Log("Game Item Clicked: " + kv.Key + "=>" + kv.Value);

        t.Find("ItemTpl/Image").GetComponent<Image>().sprite = Resources.Load<Sprite>(itemInfo["Sprite"]);
        t.Find("ItemTpl/Qty").GetComponent<TextMeshProUGUI>().SetText(kv.Value.ToString());
        t.Find("ItemName").GetComponent<TextMeshProUGUI>().SetText(itemInfo["Name"]);

        if (kv.Value <= 0) {
            btnMoreItem.interactable = false;
            btnLessItem.interactable = false;
            btnUse.interactable = false;
            QtyToBeUsed = 0;
        }
        else
        {
            btnMoreItem.interactable = itemInfo["Stackable"] == "false" ? false : true;
            btnLessItem.interactable = itemInfo["Stackable"] == "false" ? false : true;
            btnUse.interactable = true;
            QtyToBeUsed = 1;
        }

        itemQty.SetText(QtyToBeUsed.ToString());
    }

    void InitGameItemOverlay()
    {
        ObjGameItemOverlay.SetActive(false);

        Transform t = ObjGameItemOverlay.transform;
        Button btnMoreItem = t.Find("Qty/BtnMoreItem").GetComponent<Button>();
        Button btnLessItem = t.Find("Qty/BtnLessItem").GetComponent<Button>();
        Button btnUse = t.Find("Use").GetComponent<Button>();

        btnMoreItem.onClick.AddListener(delegate { IncreaseGameItemQtyToBeUsed(); });
        btnLessItem.onClick.AddListener(delegate { DecreaseGameItemQtyToBeUsed(); });
        btnUse.onClick.AddListener(delegate { UseGameItem(); });
    }

    void UseGameItem()
    {
        Dictionary<string, int> dict = GameSave.Load().ListItemDict();
        KeyValuePair<string, int> kv = dict.ElementAt(CurrentGameItemIndex);

        Debug.Log("Use game item: " + kv.Key);
        Debug.Log("Use game item to be used: " + QtyToBeUsed);
        Debug.Log("Use game item stock: " + kv.Value);

        if (kv.Value > 0)
        {
            int itemQtyLeft = kv.Value - QtyToBeUsed;

            Debug.Log("Use game item left: " + itemQtyLeft);

            SpaceXCoder.Save save = GameSave.Load();
            save.ConsumeItem(kv.Key, QtyToBeUsed);
            GameSave.Write(save);

            CurrentGameItemObj.transform.Find("Qty").GetComponent<TextMeshProUGUI>().SetText(itemQtyLeft.ToString());

            GameItemClicked(CurrentGameItemObj, CurrentGameItemIndex);

            if (kv.Key == "FogLight")
            {
                Debug.Log("Fog Light being used");
                ScaleFog(BaseFogScale * 2f);
            }
            else if (kv.Key == "StopClock")
            {
                Debug.Log("Stop Clock being used");
                TimeLeft += 3f;
            }
            else if (kv.Key == "BombShortRange")
            {
                Debug.Log("BombShortRange being used");
                Debug.Log("Rover.X: " + Rover.X + ", Rover.Y: " + Rover.Y);
                // Eight possible rocks starting from the TopLeft to Rover

                Rock tl = RockList.Find(rock => rock.X == Rover.X - 1 && rock.Y == Rover.Y - 1);
                Rock t = RockList.Find(rock => rock.X == Rover.X && rock.Y == Rover.Y - 1);
                Rock tr = RockList.Find(rock => rock.X == Rover.X + 1 && rock.Y == Rover.Y - 1);
                Rock r = RockList.Find(rock => rock.X == Rover.X + 1 && rock.Y == Rover.Y);
                Rock br = RockList.Find(rock => rock.X == Rover.X + 1 && rock.Y == Rover.Y + 1);
                Rock b = RockList.Find(rock => rock.X == Rover.X && rock.Y == Rover.Y + 1);
                Rock bl = RockList.Find(rock => rock.X == Rover.X - 1 && rock.Y == Rover.Y + 1);
                Rock l = RockList.Find(rock => rock.X == Rover.X - 1 && rock.Y == Rover.Y);

                List<Rock> rockList = new List<Rock>() { tl, t, tr, r, br, b, bl, l };

                rockList.ForEach(rock =>
                {
                    if (rock != null)
                    {
                        rock.GameObject.GetComponent<Image>().color = ColorRockRemoved;
                        RockList.Remove(rock);

                        GameObject effect = (GameObject)Instantiate(EffectShortRangeBomb, rock.GameObject.transform);
                        effect.transform.SetParent(rock.GameObject.transform.parent.transform);
                    }
                });
            }
            else if (kv.Key == "RocketBomb")
            {
                Debug.Log("RocketBomb being used");
                Debug.Log("Rover.X: " + Rover.X + ", Rover.Y: " + Rover.Y + ", Rover.dir: " + Rover.dir);

                List<Rock> rockList = null;

                
                if (Rover.dir == 0)
                {
                    rockList = RockList.Where(rock => rock.X == Rover.X && rock.Y < Rover.Y).ToList();
                }
                else if (Rover.dir == 2)
                {
                    rockList = RockList.Where(rock => rock.X == Rover.X && rock.Y > Rover.Y).ToList();
                }
                else if (Rover.dir == 1)
                {
                    rockList = RockList.Where(rock => rock.Y == Rover.Y && rock.X > Rover.X).ToList();
                }
                else if (Rover.dir == 3)
                {
                    rockList = RockList.Where(rock => rock.Y == Rover.Y && rock.X < Rover.X).ToList();
                }

                rockList.ForEach(rock =>
                {
                    if (rock != null)
                    {
                        rock.GameObject.GetComponent<Image>().color = ColorRockRemoved;
                        RockList.Remove(rock);

                        GameObject effect = (GameObject)Instantiate(EffectShortRangeBomb, rock.GameObject.transform);
                        effect.transform.SetParent(rock.GameObject.transform.parent.transform);
                    }
                });
            }
            else if (kv.Key == "ExtraStar")
            {
                CurrentGameItemObj.transform.Find("Image").GetComponent<UIEffect>().enabled = true;
            }
            else if (kv.Key == "Teleport")
            {
                Debug.Log("Teleport being used");

                //EffectTeleport

                Resource farthest = null;
                Resource nearest = null;

                ResList.ForEach(res => {
                    if (farthest == null) farthest = res;
                    if (nearest == null) nearest = res;

                    double adjacent = Math.Abs(Rover.X - res.X);
                    double opposite = Math.Abs(Rover.Y - res.Y);
                    double hypotenuse = Math.Sqrt(Math.Pow(adjacent, 2) + Math.Pow(opposite, 2));

                    double far_adj = Math.Abs(Rover.X - farthest.X);
                    double far_opp = Math.Abs(Rover.Y - farthest.Y);
                    double far_hyp = Math.Sqrt(Math.Pow(far_adj, 2) + Math.Pow(far_opp, 2));

                    double near_adj = Math.Abs(Rover.X - nearest.X);
                    double near_opp = Math.Abs(Rover.Y - nearest.Y);
                    double near_hyp = Math.Sqrt(Math.Pow(near_adj, 2) + Math.Pow(near_opp, 2));

                    Debug.Log("Distance of resource: " + hypotenuse);

                    if (hypotenuse > far_hyp)
                    {
                        farthest = res;
                    }
                    if (hypotenuse < near_hyp)
                    {
                        nearest = res;
                    }
                });

                Debug.Log("Farthest Resource: X: " + farthest.X + ", Y: " + farthest.Y);
                Debug.Log("Nearest Resource: X: " + nearest.X + ", Y: " + nearest.Y);

                Debug.Log("Resource Left: " + ResList.Count);

                ResList.ForEach(res =>
                {
                    Debug.Log("res: X: " + res.X + ", Y: " + res.Y);
                });

                Resource target = ResList.Find(res => res.X == nearest.X && res.Y == nearest.Y);

                if (target != null)
                {
                    Debug.Log("Resource Target Found");
                    Debug.Log("Target Resource: X: " + target.X + ", Y: " + target.Y);

                    GameObject effect = (GameObject)Instantiate(EffectTeleport, target.GameObject.transform);
                    effect.transform.SetParent(target.GameObject.transform.parent.transform);
                    Rover.GameObject.transform.SetParent(target.GameObject.transform.parent.transform);
                    Rover.GameObject.transform.localPosition = Vector3.zero;

                    target.GameObject.SetActive(false);

                    ResList.Remove(target);

                    if (ResList.Count == 0)
                    {
                        CurrentGameItemObj.transform.Find("Image").GetComponent<UIEffect>().enabled = true;
                        GameWin();
                    }
                }
            }
        }
    }

    void IncreaseGameItemQtyToBeUsed()
    {
        Dictionary<string, int> dict = GameSave.Load().ListItemDict();
        KeyValuePair<string, int> kv = dict.ElementAt(CurrentGameItemIndex);
        Transform t = ObjGameItemOverlay.transform;
        TextMeshProUGUI qtyText = t.Find("Qty/GameItemQty/Text").GetComponent<TextMeshProUGUI>();

        QtyToBeUsed++;
        if (QtyToBeUsed > kv.Value)
        {
            QtyToBeUsed = kv.Value;
        }
        Debug.Log("QTY " + QtyToBeUsed);
        qtyText.SetText(QtyToBeUsed.ToString());
    }

    void DecreaseGameItemQtyToBeUsed()
    {
        Transform t = ObjGameItemOverlay.transform;
        TextMeshProUGUI qtyText = t.Find("Qty/GameItemQty/Text").GetComponent<TextMeshProUGUI>();

        QtyToBeUsed--;
        if (QtyToBeUsed < 1)
        {
            QtyToBeUsed = 1;
        }
        Debug.Log("QTY " + QtyToBeUsed);
        qtyText.SetText(QtyToBeUsed.ToString());
    }

    /*
    void ToggleGameInventoryOverlay()
    {
        bool isActive = !GameInventoryOverlay.activeSelf;
        GameInventoryOverlay.SetActive(isActive);
        ObjGameItemOverlay.SetActive(isActive);
    }
    */

    void InitEarnedStar()
    {
        Transform bsc = ObjWinnerStarContainer.transform;
        for (int bsi = 0; bsi < bsc.childCount; bsi++)
        {
            GameObject bs = bsc.GetChild(bsi).gameObject;
            bs.GetComponent<Image>().color = ColorWinnerStarDark;
        }
    }

    void InitBanner()
    {
        Rect progressBgRect = ObjProgressBg.gameObject.GetComponent<RectTransform>().rect;
        ProgressBgWidth = progressBgRect.width;
        ProgressBgHeight = progressBgRect.height;
        Transform bsc = BannerResourceContainer.transform;

        //Debug.Log("Progressbar Bg Width: " + ProgressBgWidth);

        for (int bsi = 0; bsi < bsc.childCount; bsi++)
        {
            GameObject bs = bsc.GetChild(bsi).gameObject;
            if (bsi >= ResourceQty)
            {
                bs.GetComponent<Image>().color = ColorBannerResourceHidden;
            }
            else
            {
                bs.GetComponent<Image>().color = ColorBannerResourceDark;
            }
        }
    }

    void InitButtons()
    {
        Button btnTurnLeft = ObjTurnLeft.GetComponent<Button>();
        Button btnTurnRight = ObjTurnRight.GetComponent<Button>();
        Button btnFoward = ObjForward.GetComponent<Button>();
        Button btnUndo = ObjUndo.GetComponent<Button>();
        Button btnSend = ObjSend.GetComponent<Button>();
        Button btnFinishContinue = ObjFinishContinue.GetComponent<Button>();
        Button btnForceExit = ObjForceExit.GetComponent<Button>();
        Button btnPlayAgain = ObjPlayAgain.GetComponent<Button>();
        Button btnGameOverExit = ObjGameOverExit.GetComponent<Button>();
        //Button btnGameInventory = ObjGameInventory.GetComponent<Button>();

        UpdateCtrlBtnStatus(true);

        btnTurnLeft.onClick.AddListener(delegate { AppendInstruction("TurnLeft"); });
        btnTurnRight.onClick.AddListener(delegate { AppendInstruction("TurnRight"); });
        btnFoward.onClick.AddListener(delegate { AppendInstruction("Forward"); });
        btnUndo.onClick.AddListener(delegate { UndoInstruction("Undo"); });
        btnSend.onClick.AddListener(delegate { SendInstruction("Send"); });
        btnFinishContinue.onClick.AddListener(delegate { FinishContinue(); });
        btnForceExit.onClick.AddListener(delegate { ForceExit(); });

        btnPlayAgain.onClick.AddListener(delegate { PlayAgain(); });
        btnGameOverExit.onClick.AddListener(delegate { ForceExit(); });

        //btnGameInventory.onClick.AddListener(delegate { ToggleGameInventoryOverlay(); });
    }

    void CenterGrid()
    {
        // Hide walls
        ObjWall.SetActive(false);

        GridLayoutGroup glg = gameObject.GetComponent<GridLayoutGroup>();
        Rect gsvRect = GridScrollView.gameObject.GetComponent<RectTransform>().rect;
        RectTransform wallObject = ObjWall.GetComponent<RectTransform>();

        GridSize = glg.cellSize;
        GridSpacing = glg.spacing;
        GridPadding = glg.padding;

        glg.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        glg.constraintCount = GridHeight;

        float wallWidth = (GridSize.x + GridSpacing.x) * GridWidth;
        float wallHeight = (GridSize.y + GridSpacing.y) * GridHeight;

        int paddingLeft = (int)(gsvRect.width - wallWidth) / 2;
        int paddingTop = (int)(gsvRect.height - wallHeight) / 2;

        //Debug.Log("Grid Container Width: " + gsvRect.width + " Height: " + gsvRect.height);
        //Debug.Log("wallWidth: " + wallWidth + " wallHeight: " + wallHeight);

        // Center game map
        glg.padding.left = paddingLeft;
        glg.padding.top = paddingTop;

        // Set correct wall size
        wallObject.sizeDelta = new Vector2(wallWidth, wallHeight);

        // Set correct wall position to match game map
        wallObject.transform.localPosition = new Vector2((float)paddingLeft, -(float)paddingTop);
    }

    void AppendInstruction(string action)
    {
        //Debug.Log(action);
        Instruction.Add(action);

        InstructionManager InstMgr = InstructionView.GetComponent<InstructionManager>();
        InstMgr.PopulateOne(action);

        UpdateCtrlBtnStatus(true);
    }

    void UndoInstruction(string action)
    {
        //Debug.Log(action);

        if (Instruction.Count > 0)
        {
            Instruction.RemoveAt(Instruction.Count - 1);

            InstructionManager InstMgr = InstructionView.GetComponent<InstructionManager>();
            InstMgr.Undo();

            UpdateCtrlBtnStatus(true);
        }
    }

    IEnumerator ExecuteInstruction(Action action, int i)
    {
        yield return new WaitForSeconds(i * .2f);
        action();

        InstructionManager InstMgr = InstructionView.GetComponent<InstructionManager>();
        InstMgr.ClearFirst();

        if (i == Instruction.Count - 1)
        {
            Instruction.Clear();
            UpdateCtrlBtnStatus(true);
        }
    }

    void TestExecutionIndex(int index, int count)
    {

        Debug.Log("Executing: " + (index + 1) + "/" + count);
        if (index == count - 1)
        {
            IsInstExecuting = false;
            ActivateAllCtrlBtn();
        }
    }

    void UpdateCtrlBtnStatus(bool activated)
    {
        Button btnTurnLeft = ObjTurnLeft.GetComponent<Button>();
        Button btnTurnRight = ObjTurnRight.GetComponent<Button>();
        Button btnFoward = ObjForward.GetComponent<Button>();
        Button btnUndo = ObjUndo.GetComponent<Button>();
        Button btnSend = ObjSend.GetComponent<Button>();

        if (activated == true)
        {
            btnFoward.GetComponent<Image>().color = ColorCtrlBtnBright;
            btnTurnLeft.GetComponent<Image>().color = ColorCtrlBtnBright;
            btnTurnRight.GetComponent<Image>().color = ColorCtrlBtnBright;

            btnFoward.transform.GetChild(0).GetComponent<Image>().color = ColorCtrlBtnContentBright;
            btnTurnLeft.transform.GetChild(0).GetComponent<Image>().color = ColorCtrlBtnContentBright;
            btnTurnRight.transform.GetChild(0).GetComponent<Image>().color = ColorCtrlBtnContentBright;

            if (Instruction.Count == 0)
            {
                btnUndo.GetComponent<Image>().color = ColorCtrlBtnDark;
                btnSend.GetComponent<Image>().color = ColorCtrlBtnDark;
                btnUndo.transform.GetChild(0).GetComponent<Image>().color = ColorCtrlBtnContentDark;
                btnSend.transform.GetChild(0).GetComponent<Image>().color = ColorCtrlBtnContentDark;
            }
            else
            {
                if (Instruction.Count > 0)
                {
                    btnUndo.GetComponent<Image>().color = ColorCtrlBtnBright;
                    btnSend.GetComponent<Image>().color = ColorCtrlBtnBright;
                    btnUndo.transform.GetChild(0).GetComponent<Image>().color = ColorCtrlBtnContentBright;
                    btnSend.transform.GetChild(0).GetComponent<Image>().color = ColorCtrlBtnContentBright;
                }
            }
        }
        else
        {
            btnFoward.GetComponent<Image>().color = ColorCtrlBtnDark;
            btnTurnLeft.GetComponent<Image>().color = ColorCtrlBtnDark;
            btnTurnRight.GetComponent<Image>().color = ColorCtrlBtnDark;
            btnUndo.GetComponent<Image>().color = ColorCtrlBtnDark;
            btnSend.GetComponent<Image>().color = ColorCtrlBtnDark;

            btnFoward.transform.GetChild(0).GetComponent<Image>().color = ColorCtrlBtnContentDark;
            btnTurnLeft.transform.GetChild(0).GetComponent<Image>().color = ColorCtrlBtnContentDark;
            btnTurnRight.transform.GetChild(0).GetComponent<Image>().color = ColorCtrlBtnContentDark;
            btnUndo.transform.GetChild(0).GetComponent<Image>().color = ColorCtrlBtnContentDark;
            btnSend.transform.GetChild(0).GetComponent<Image>().color = ColorCtrlBtnContentDark;
        }
    }

    void DeactivateAllCtrlBtn()
    {
        UpdateCtrlBtnStatus(false);
    }

    void ActivateAllCtrlBtn()
    {
        UpdateCtrlBtnStatus(false);
    }

    void SendInstruction(string action)
    {
        //Debug.Log(action);
        if (IsInstExecuting == true)
        {
            return;
        }

        int inst_count = Instruction.Count;

        if (inst_count > 0) {
            IsInstExecuting = true;
            DeactivateAllCtrlBtn();
        }

        int dl = DIR.Length;
        for (int i = 0; i < inst_count; i++)
        {
            string inst = Instruction[i];
            Debug.Log(inst);
            int index = i;
            if (inst == "TurnLeft")
            {
                StartCoroutine(ExecuteInstruction(() =>
                {
                    Rover.GameObject.transform.Rotate(0, 0, 90, Space.World);
                    Rover.dir = (Rover.dir + dl - 1) % dl;
                    TestExecutionIndex(index, inst_count);
                }, i));
                
            }
            else if (inst == "TurnRight")
            {
                StartCoroutine(ExecuteInstruction(() =>
                {
                    Rover.GameObject.transform.Rotate(0, 0, -90, Space.World);
                    Rover.dir = (Rover.dir + 1) % dl;
                    TestExecutionIndex(index, inst_count);
                }, i));
            }
            else if (inst == "Forward")
            {
                StartCoroutine(ExecuteInstruction(() =>
                {
                    Debug.Log("Rover Pos x: " + Rover.X + ", y: " + Rover.Y + " dir: " + Rover.dir);
                    string direction = DIR[Rover.dir];
                    int dy = Rover.Y;
                    int dx = Rover.X;
                    if (direction == "Up")
                    {
                        dy = dy - 1;
                    }
                    else if (direction == "Right")
                    {
                        dx = dx + 1;
                    }
                    else if (direction == "Down")
                    {
                        dy = dy + 1;
                    }
                    else if (direction == "Left")
                    {
                        dx = dx - 1;
                    }

                    Rock foundRock = RockList.Find(rock => rock.X == dx && rock.Y == dy);

                    if (foundRock != null)
                    {
                        Debug.Log("Found a Rock conflict: " + foundRock);
                        foundRock.GameObject.GetComponent<Image>().color = ColorRockError;
                    }
                    else if (dy < 0 || dy >= GridHeight || dx < 0 || dx >= GridWidth)
                    {
                        Debug.Log("Hit the wall");
                        ObjWall.SetActive(true);
                    }
                    else
                    {
                        Resource foundResource = ResList.Find(res => res.X == dx && res.Y == dy);

                        if (foundResource != null)
                        {
                            Debug.Log("Congrats! Found a resource!");
                            foundResource.GameObject.SetActive(false);
                            ResList.RemoveAt(ResList.IndexOf(foundResource));

                            Transform bsc = BannerResourceContainer.transform;
                            bsc.GetChild(EarnedResourceCount++).gameObject.GetComponent<Image>().color = ColorBannerResourceBright;
                        }

                        GameObject cell = GridList[(dy * GridWidth) + dx];
                        Rover.GameObject.transform.SetParent(cell.gameObject.transform);
                        Rover.GameObject.transform.localPosition = Vector3.zero;
                        Rover.X = dx;
                        Rover.Y = dy;

                        StartCoroutine(PositionFog());
                    }

                    TestExecutionIndex(index, inst_count);

                    if (ResList.Count == 0)
                    {
                        GameWin();
                    }

                }, i));
            }
        }
    }

    void GameOver()
    {
        Debug.Log("Game Over!");
        FreezeGame();
        LoseModal.SetActive(true);
    }

    void GameWin()
    {
        Debug.Log("Game Win!");
        FreezeGame();
        WinModal.SetActive(true);

        SpaceXCoder.Save save = GameSave.Load();

        int nextLevel = CurrentLevel + 1;
        int unlocked = save.unlocked;
        int currentLvTimeLeft = save.lvRecords[CurrentLevel].timeLeft;
        int timeLeft = (int)Math.Floor(TimeLeft);

        if (nextLevel > unlocked)
        {
            save.unlocked = nextLevel;
        }

        if (timeLeft > currentLvTimeLeft)
        {
            LvRecord lvRecord = new LvRecord();
            int score = timeLeft / SCORE_PER_SEC + 1;
            if (score > MAX_STAR_NUMBER)
            {
                score = MAX_STAR_NUMBER;
            }
            lvRecord.score = score;
            lvRecord.timeLeft = timeLeft;
            Debug.Log("++++++++lvRecord: " + lvRecord.score + ", " + lvRecord.timeLeft);
            save.lvRecords[CurrentLevel] = lvRecord;
        }

        int totalScore = save.lvRecords.ToList().Select(r => r.score).Sum();
        Debug.Log("totalScore: " + totalScore);
        if (totalScore > 0)
        {
            ReportScore(totalScore);
        }

        Debug.Log("Win Time Left: " + TimeLeft);
        Debug.Log("Win Time Left Floored: " + timeLeft);

        for (int i = 0; i <= timeLeft; i++)
        {
            StartCoroutine(CalcStar(i));
        }

        GameSave.Write(save);
        Debug.Log("Game Saved");
    }

    IEnumerator CalcStar(int i)
    {
        yield return new WaitForSeconds(i*.02f);
        int earned = i;
        ObjTimeLeftCountdown.gameObject.GetComponent<TextMeshProUGUI>().SetText(earned + "s");

        int indexOfStar = i / SCORE_PER_SEC;
        if (i % SCORE_PER_SEC == 0 && indexOfStar < MAX_STAR_NUMBER)
        {
            Transform bsc = ObjWinnerStarContainer.transform;
            GameObject bs = bsc.GetChild(indexOfStar).gameObject;
            bs.GetComponent<Image>().color = ColorWinnerStarBright;
        }
    }

    void ReportScore(int score)
    {
        Debug.Log("Reporting score " + score + " on leaderboard " + LeaderboardID);
        Social.ReportScore(score, LeaderboardID, success => {
            Debug.Log(success ? "Reported score successfully" : "Failed to report score");
        });
    }

    void ReportAchievement_0()
    {
        Debug.Log("Reporting progress on achievement " + Achievement_0);
        Social.ReportProgress(Achievement_0, 100.0, success => {
            Debug.Log(success ? "Reported score successfully" : "Failed to report score");
        });
    }

    void FinishContinue()
    {
        ExitScene();
    }

    void ForceExit()
    {
        ExitScene();
    }

    void ExitScene()
    {
        int stage = PlayerPrefs.GetInt("stage");
        SceneManager.LoadScene("Stage_" + stage);
    }

    void FreezeGame()
    {
        IsGameFrozen = true;
    }

    void UnfreezeGame()
    {
        IsGameFrozen = false;
    }

    void Update()
    {
        if (IsGameFrozen == true)
        {
            return;
        }

        TimeLeft -= Time.deltaTime;
        TimeSpent += Time.deltaTime;
        float percentLeft = TimeLeft / (TimeLeft + TimeSpent);
        float width = ProgressBgWidth * percentLeft;
        float height = ProgressBgHeight;

        if (TimeLeft < 0)
        {
            //Game Over
            GameOver();
            ObjTimeLeftText.gameObject.GetComponent<TextMeshProUGUI>().SetText("00:00");
            ObjProgressFg.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(ZEROF, height);
        }
        else
        {
            int minutes = Mathf.FloorToInt(TimeLeft / 60f);
            int seconds = Mathf.FloorToInt(TimeLeft - minutes * 60);
            string niceTime = string.Format("{0:00}:{1:00}", minutes, seconds);

            //Debug.Log("Time Left: " + niceTime);

            ObjTimeLeftText.gameObject.GetComponent<TextMeshProUGUI>().SetText(niceTime);
            ObjProgressFg.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

            
            if (percentLeft > 0.7f && percentLeft <= 1.0f) {
                ObjProgressBg.gameObject.GetComponent<Image>().color = ColorProgressBg100;
            }
            else if (percentLeft > 0.5f && percentLeft <= 0.7f)
            {
                ObjProgressBg.gameObject.GetComponent<Image>().color = ColorProgressBg70;
            }
            else if (percentLeft > 0.4f && percentLeft <= 0.5f)
            {
                ObjProgressBg.gameObject.GetComponent<Image>().color = ColorProgressBg50;
            }
            else if (percentLeft <= 0.4f)
            {
                ObjProgressBg.gameObject.GetComponent<Image>().color = ColorProgressBg40;
            }


            if (IsFogReady == true)
            {
                float fogScale = BaseFogScale * (1 - FogGrowSpeed);
                if (fogScale > MinFogScale)
                {
                    ScaleFog(fogScale);
                }
            }
        }
    }

    void RoverRotate(int dir)
    {
        Rover.GameObject.transform.Rotate(0f, 0f, dir * -90f, Space.World);
    }

    IEnumerator PositionFog()
    {
        yield return new WaitForEndOfFrame();

        Vector3 cellPosition = Rover.GameObject.transform.parent.gameObject.transform.localPosition;

        //Debug.Log("Rover global position x");
        //Debug.Log(cellPosition.x);
        //Debug.Log(cellPosition.y);

        Vector3 position = new Vector3(cellPosition.x, cellPosition.y, 0);
        ObjFog.transform.localPosition = position;
        IsFogReady = true;
    }

    void ScaleFog(float fogScale)
    {
        Vector3 scale = transform.localScale;
        scale.Set(fogScale, fogScale, 1);

        ObjFog.transform.localScale = scale;
        BaseFogScale = fogScale;
    }


    //IEnumerator GenMap()
    void GenMap()
    {
        //yield return new WaitForEndOfFrame();

        List<int> allX = Enumerable.Range(0, GridWidth).ToList();
        List<int> allY = Enumerable.Range(0, GridHeight).ToList();
        List<System.Drawing.Point> allCord = new List<System.Drawing.Point>();
        int[][] grid = new int[GridHeight][];

        //Debug.Log("[" + string.Join(", ", allX) + "]");
        //Debug.Log("[" + string.Join(", ", allY) + "]");

        allY.ForEach(y =>
        {
            grid[y] = new int[GridWidth];
            allX.ForEach(x =>
            {
                allCord.Add(new System.Drawing.Point(x, y));
                grid[y][x] = 0;
            });
        });

        //Debug.Log(string.Join(", ", allCord));

        System.Random random = new System.Random();
        int randomIndex = random.Next(0, allCord.Count);
        int randomDir = 0;// random.Next(0, DIR.Length);

        Debug.Log("Rover Direction: " + randomDir);

        Rover rover = new Rover();
        rover.dir = randomDir;
        rover.X = allCord[randomIndex].X;
        rover.Y = allCord[randomIndex].Y;
        //Debug.Log(rover.X);
        //Debug.Log(rover.Y);
        //Debug.Log(((rover.Y * GridWidth) + rover.X + 1));
        //Debug.Log(GridList.Count);
        GameObject cell = GridList[(rover.Y * GridWidth) + rover.X];
        //Vector3 pos = cell.gameObject.GetComponent<RectTransform>().localPosition;
        //Vector3 localPosition = new Vector3(pos.x, pos.y, ObjPosZ);

        Vector3 roverScale = transform.localScale;
        roverScale.Set(1f, 1f, 1f);

        rover.GameObject = Instantiate(RoverPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        rover.GameObject.transform.SetParent(cell.gameObject.transform);
        rover.GameObject.transform.localPosition = Vector3.zero;
        rover.GameObject.transform.localScale = roverScale;

        Rover = rover;
        //Debug.Log("allCord.count=>" + allCord.Count);
        allCord.RemoveAt(randomIndex);
        //Debug.Log("allCord.count=>" + allCord.Count);

        RoverRotate(randomDir);

        List<int> rockList = Enumerable.Range(0, RockQty).ToList();
        rockList.ForEach(i =>
        {
            //Debug.Log(i);
            randomIndex = random.Next(0, allCord.Count);
            Rock rock = new Rock();
            rock.X = allCord[randomIndex].X;
            rock.Y = allCord[randomIndex].Y;
            grid[rock.Y][rock.X] = 1;

            cell = GridList[(rock.Y * GridWidth) + rock.X];
            //pos = cell.gameObject.GetComponent<RectTransform>().localPosition;
            //localPosition = new Vector3(pos.x, pos.y, ObjPosZ);

            Vector3 scale = transform.localScale;
            //scale.Set(150f, 150f, 150f);
            //scale.Set(0.3f, 0.3f, 1);

            rock.GameObject = Instantiate(RockPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            rock.GameObject.transform.SetParent(cell.gameObject.transform);
            rock.GameObject.transform.localPosition = Vector3.zero;
            rock.GameObject.transform.localScale = scale;
            rock.GameObject.GetComponent<Image>().color = ColorRockNormal;
            //rock.GameObject.transform.Rotate(-93f, -8.5f, 9f, Space.World);

            RockList.Add(rock);
            allCord.RemoveAt(randomIndex);
        });

        List<int> resList = Enumerable.Range(0, ResourceQty).ToList();
        resList.ForEach(i =>
        {
            //Debug.Log(i);
            randomIndex = random.Next(0, allCord.Count);
            Resource res = new Resource();
            res.X = allCord[randomIndex].X;
            res.Y = allCord[randomIndex].Y;

            cell = GridList[(res.Y * GridWidth) + res.X];
            //pos = cell.gameObject.GetComponent<RectTransform>().localPosition;
            //localPosition = new Vector3(pos.x, pos.y, ObjPosZ);

            Vector3 scale = transform.localScale;
            scale.Set(0.8f, 0.8f, 1);

            res.GameObject = Instantiate(ResourcePrefab, Vector3.zero, Quaternion.identity) as GameObject;
            res.GameObject.transform.SetParent(cell.gameObject.transform);
            res.GameObject.transform.localPosition = Vector3.zero;
            res.GameObject.transform.localScale = scale;

            ResList.Add(res);
            allCord.RemoveAt(randomIndex);
        });

        /*
        for (int i = 0; i < grid.Length; i++)
        {
            Debug.Log(string.Format("row: {0} [{1}]", i, string.Join(", ", grid[i])));
        }
        //*/

        Debug.Log("rover: " + rover.X + "," + rover.Y);
        Debug.Log("resource: " + ResList[0].X + "," + ResList[0].Y);


        int[] startPos = { rover.X, rover.Y};

        for (int i = 0; i < ResList.Count; i++)
        {
            Resource endRes = ResList[i];
            int[] endPos = { endRes.X, endRes.Y };
            //StartCoroutine(AStarFind(grid, startPos, endPos, i));
            AStarFind(grid, startPos, endPos, i);
        }
        Debug.Log(Time.realtimeSinceStartup);
    }

    //IEnumerator AStarFind(int[][] map, int[] start, int[] end, int i)
    void AStarFind(int[][] map, int[] start, int[] end, int i)
    {
        //yield return new WaitForEndOfFrame();
        //yield return new WaitForSeconds(i * .2f);
        List<Vector2> path = new Astar(map, start, end, "Diagonal").result;
        Debug.Log("resultPath: [" + string.Join(", ", path) + "]");
        if ( path.Count == 0 )
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void PopulateGrid()
    {
        int numberToCreate = GridWidth * GridHeight;

        GameObject newObj;

        for (int i = 0; i < numberToCreate; i++)
        {
            newObj = (GameObject)Instantiate(prefab, transform);
            GridList.Add(newObj);

            //UnityEngine.Color myColor = new UnityEngine.Color();
            //UnityEngine.ColorUtility.TryParseHtmlString("#FF0100", out myColor);
            //newObj.GetComponent<Image>().color = myColor;
            //newObj.GetComponent<Image>().color = Random.ColorHSV();
        }
    }
}