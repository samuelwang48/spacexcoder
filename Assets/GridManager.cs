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
    public GameObject GrownRockPrefab;
    public GameObject ResourcePrefab;
    public GameObject SkillGridCellPrefab;
    public GameObject SkillGridContainer;
    public GameObject InstructionView;

    public GameObject GridScrollView;
    public GameObject BannerResourceContainer;
    public GameObject WinModal;
    public GameObject LoseModal;
    public GameObject PrefabItemTpl;

    public GameObject ObjTurnLeft;
    public GameObject ObjTurnRight;
    public GameObject ObjForward;
    public GameObject ObjUndo;
    public GameObject ObjSend;
    public GameObject ObjGameItemOverlay;
    public GameObject ObjGameSplashRover;

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
    private float BaseFogScale = 20f;
    public float FogGrowSpeed = 0.1f;
    private bool IsFogReady = false;
    public int RockGrowNumber = 0;
    public int RockGrowTime = 0;
    public int BlackholeNumber = 0;
    public int BlackholeTimeMin = 0;
    public int BlackholeTimeMax = 0;
    public int BlackholeLifeMin = 0;
    public int BlackholeLifeMax = 0;
    public int BlackholeSizeMin = 0;
    public int BlackholeSizeMax = 0;

    private string[] DIR = { "Up", "Right", "Down", "Left" };

    private List<GameObject> GridList = new List<GameObject>();
    private Rover Rover;
    private List<Rock> RockList = new List<Rock>();
    private List<Resource> ResList = new List<Resource>();
    private List<GameObject> BannerResourceList = new List<GameObject>();
    private List<string> Instruction = new List<string>();

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

    private UnityEngine.Color ColorWhite = new UnityEngine.Color(1f, 1f, 1f, 1f);
    private UnityEngine.Color ColorBlack = new UnityEngine.Color(0f, 0f, 0f, 1f);


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
    private bool IsPowerOverwhelming = false;

    private int CurrentLevel;

    private int QtyToBeUsed = 1;

    private int CurrentGameItemIndex;
    private GameObject CurrentGameItemObj;
    private bool ExtraStarUsed = false;

    public GameObject EffectShortRangeBomb;
    public GameObject EffectTeleport;
    public GameObject EffectBlackhole;
    public GameObject EffectPowerOverwhelming;


    void InitLevel()
    {
        GridLayoutGroup glg = gameObject.GetComponent<GridLayoutGroup>();
        CurrentLevel = PlayerPrefs.GetInt("level");
        //CurrentLevel = 19;
        Debug.Log("Current level is => " + CurrentLevel);

        Dictionary<string, object> cfg = GameService.ReadLevelConfig(CurrentLevel);
        Debug.Log("Level Config1 => " + cfg["GridWidth"] + ", " + cfg["GridHeight"] + ", " + cfg["RockQty"] + ", " + cfg["ResourceQty"] + ", " + cfg["FogGrowSpeed"] + ", " + cfg["RockGrowNumber"] + ", " + cfg["RockGrowTime"]);
        Debug.Log("Level Config2 => " + cfg["BlackholeNumber"] + ", " + cfg["BlackholeTimeMin"] + ", " + cfg["BlackholeTimeMax"] + ", " + cfg["BlackholeLifeMin"] + ", " + cfg["BlackholeLifeMax"] + ", " + cfg["BlackholeSizeMin"] + ", " + cfg["BlackholeSizeMax"]);

        GridWidth = (int)cfg["GridWidth"];
        GridHeight = (int)cfg["GridHeight"];
        RockQty = (int)cfg["RockQty"];
        ResourceQty = (int)cfg["ResourceQty"];
        FogGrowSpeed = (float)cfg["FogGrowSpeed"];
        RockGrowNumber = (int)cfg["RockGrowNumber"];
        RockGrowTime = (int)cfg["RockGrowTime"];
        BlackholeNumber = (int)cfg["BlackholeNumber"];
        BlackholeTimeMin = (int)cfg["BlackholeTimeMin"];
        BlackholeTimeMax = (int)cfg["BlackholeTimeMax"];
        BlackholeLifeMin = (int)cfg["BlackholeLifeMin"];
        BlackholeLifeMax = (int)cfg["BlackholeLifeMax"];
        BlackholeSizeMin = (int)cfg["BlackholeSizeMin"];
        BlackholeSizeMax = (int)cfg["BlackholeSizeMax"];


        if (GridHeight >= 17)
        {
            glg.cellSize = new Vector2(87f, 87f);
        }
    }

    void Start()
    {
        Time.timeScale = 1f;
        TimeLeft += 1f;

        InitLevel();

        InitWinModal();
        InitLoseModal();
        InitSkillList();
        InitGameItemOverlay();

        InitBanner();

        CenterGrid();

        PopulateGrid();

        GenMap();

        StartCoroutine(PositionFog());

        InitButtons();

        InitEarnedStar();

        HideGameSplashRover();

        InitGameItemObjects();
    }

    void InitGameItemObjects()
    {
        //Using an instance object in the scene, there is no need to clone it.
        //GameObject effect = (GameObject)Instantiate(EffectPowerOverwhelming, Rover.GameObject.transform);
        EffectPowerOverwhelming.gameObject.transform.SetParent(Rover.GameObject.transform);
        EffectPowerOverwhelming.gameObject.transform.localPosition = Vector3.zero;
        EffectPowerOverwhelming.gameObject.SetActive(false);
    }

    void HideGameSplashRover()
    {
        ObjGameSplashRover.gameObject.SetActive(false);
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

    void InitSkillList()
    {
        SpaceXCoder.Inventory.InitGameItemList(
            PrefabItemTpl,
            SkillGridCellPrefab,
            SkillGridContainer,
            GameItemClick
        );
    }

    void GameItemClick(GameObject newObj, int i)
    {
        GameItemClicked(newObj, i);
        UseGameItem();
    }

    void GameItemClicked(GameObject gameItem, int index)
    {

        Dictionary<string, Dictionary<string, string>> itemInfoDict = SpaceXCoder.CONST.ITEM_INFO;
        Dictionary<string, int> dict = GameService.LoadSave().ListItemDict();

        KeyValuePair<string, int> kv = dict.ElementAt(index);
        Dictionary<string, string> itemInfo = itemInfoDict[kv.Key];

        Transform t = ObjGameItemOverlay.transform;
        Button btnMoreItem = t.Find("Qty/BtnMoreItem").GetComponent<Button>();
        Button btnLessItem = t.Find("Qty/BtnLessItem").GetComponent<Button>();
        Button btnUse = t.Find("Use").GetComponent<Button>();
        TextMeshProUGUI itemQty = t.Find("Qty/GameItemQty/Text").GetComponent<TextMeshProUGUI>();

        CurrentGameItemObj = gameItem;
        CurrentGameItemIndex = index;

        // Item is only allowed to be used once each round
        if (CurrentGameItemObj.transform.Find("Image").GetComponent<UIEffect>().enabled == true)
        {
            btnMoreItem.interactable = false;
            btnLessItem.interactable = false;
            btnUse.interactable = false;
            QtyToBeUsed = 0;
            return;
        }

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

    IEnumerator CooldownAnimation(GameObject cd, float time)
    {
        float i = 0;
        float rate = 1 / time;

        while (i <= 1)
        {
            int progress = (int)(100f - 100f * i);
            if (i > 0.9f) progress = 0;
            cd.GetComponent<UnityEngine.UI.Extensions.UICircle>().FillPercent = progress;
            i += Time.deltaTime * rate;
            yield return 0;
        }

        if (i > 1)
        {
            Debug.Log("CD Time is over");
            yield return 0;
        }
    }

    IEnumerator LifeAnimation(GameObject life, float time, string key)
    {
        float i = 0;
        float rate = 1 / time;

        while (i <= 1)
        {
            int progress = (int)(100f - 100f * i);
            if (i > 0.9f) progress = 0;
            life.GetComponent<UnityEngine.UI.Extensions.UICircle>().FillPercent = progress;
            i += Time.deltaTime * rate;
            yield return 0;
        }

        if (i > 1)
        {
            Debug.Log("Item effectiveness life is over");
            Debug.Log("key => " + key);
            if (key == "PowerOverwhelming") {
                Rover.GameObject.GetComponent<Image>().color = ColorWhite;
                EffectPowerOverwhelming.gameObject.SetActive(false);
                IsPowerOverwhelming = false;
            }
            yield return 0;
        }
    }

    void UseGameItem()
    {
        Dictionary<string, Dictionary<string, string>> itemInfoDict = SpaceXCoder.CONST.ITEM_INFO;
        Dictionary<string, int> dict = GameService.LoadSave().ListItemDict();
        KeyValuePair<string, int> kv = dict.ElementAt(CurrentGameItemIndex);

        Debug.Log("Use game item: " + kv.Key);
        Debug.Log("Use game item to be used: " + QtyToBeUsed);
        Debug.Log("Use game item stock: " + kv.Value);

        if (kv.Value > 0)
        {
            int itemQtyLeft = kv.Value - QtyToBeUsed;

            if (itemInfoDict[kv.Key].ContainsKey("CD") == true)
            {
                string cooldown_str = itemInfoDict[kv.Key]["CD"];
                float cooldown_time = float.Parse(cooldown_str);
                if (cooldown_time > 0)
                {
                    GameObject cd = CurrentGameItemObj.transform.Find("Image/CD").gameObject;
                    cd.GetComponent<UnityEngine.UI.Extensions.UICircle>().FillPercent = 100;
                    cd.SetActive(true);
                    StartCoroutine(CooldownAnimation(cd, cooldown_time));
                }
            }

            if (itemInfoDict[kv.Key].ContainsKey("Life") == true)
            {
                string life_str = itemInfoDict[kv.Key]["Life"];
                float life_time = float.Parse(life_str);
                if (life_time > 0)
                {
                    GameObject life = CurrentGameItemObj.transform.Find("Life").gameObject;
                    life.GetComponent<UnityEngine.UI.Extensions.UICircle>().FillPercent = 100;
                    life.SetActive(true);
                    StartCoroutine(LifeAnimation(life, life_time, kv.Key));
                }
            }

            Debug.Log("Use game item left: " + itemQtyLeft);

            SpaceXCoder.Save save = GameService.LoadSave();
            save.ConsumeItem(kv.Key, QtyToBeUsed);
            GameService.Write(save);

            CurrentGameItemObj.transform.Find("Qty").GetComponent<TextMeshProUGUI>().SetText(itemQtyLeft.ToString());

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
                ExtraStarUsed = true;
            }
            else if (kv.Key == "PowerOverwhelming")
            {
                Debug.Log("PowerOverwhelming being used");

                Rover.GameObject.GetComponent<Image>().color = ColorBlack;
                EffectPowerOverwhelming.gameObject.SetActive(true);
                IsPowerOverwhelming = true;
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
                    Rover.X = target.X;
                    Rover.Y = target.Y;

                    StartCoroutine(PositionFog());

                    target.GameObject.SetActive(false);

                    ResList.Remove(target);

                    Transform bsc = BannerResourceContainer.transform;
                    bsc.GetChild(EarnedResourceCount++).gameObject.GetComponent<Image>().color = ColorBannerResourceBright;

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
        Dictionary<string, int> dict = GameService.LoadSave().ListItemDict();
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

                    if (foundRock != null && IsPowerOverwhelming == false)
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

                        if (foundRock != null && IsPowerOverwhelming == true)
                        {
                            Destroy(foundRock.GameObject);
                            RockList.Remove(foundRock);
                        }

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

        SpaceXCoder.Save save = GameService.LoadSave();

        int nextLevel = CurrentLevel + 1;
        int unlocked = save.Unlocked();
        //int currentLvTimeLeft = save.lvRecords[CurrentLevel].timeLeft;
        int currentLvScore = save.GetLvRecord(CurrentLevel).score;
        int timeLeft = (int)Math.Floor(TimeLeft);

        if (nextLevel > unlocked)
        {
            save.Unlock(nextLevel);
        }

        LvRecord lvRecord = new LvRecord();
        int score = timeLeft / SCORE_PER_SEC + 1;
        if (score > MAX_STAR_NUMBER)
        {
            score = MAX_STAR_NUMBER;
        }
        Debug.Log("Calling extra star " + ExtraStarUsed + " " + (score < MAX_STAR_NUMBER));

        for (int i = 0; i <= timeLeft; i++)
        {
            StartCoroutine(CalcStar(i));
        }
        if (ExtraStarUsed == true && score < MAX_STAR_NUMBER)
        {
            score = score + 1;
            CalcExtraStar(score);
        }

        //if (timeLeft > currentLvTimeLeft)
        if (score > currentLvScore)
        {
            lvRecord.score = score;
            lvRecord.timeLeft = timeLeft;
            Debug.Log("++++++++lvRecord: " + lvRecord.score + ", " + lvRecord.timeLeft);
            save.SetLvRecord(CurrentLevel, lvRecord);
        }

        int totalScore = save.GetAllRecords().ToList().Select(r => r.score).Sum();
        Debug.Log("totalScore: " + totalScore);
        if (totalScore > 0)
        {
            ReportScore(totalScore);
        }

        Debug.Log("Win Time Left: " + TimeLeft);
        Debug.Log("Win Time Left Floored: " + timeLeft);


        GameService.Write(save);
        Debug.Log("Game Saved");
    }

    void CalcExtraStar(int i)
    {
        Transform bsc = ObjWinnerStarContainer.transform;
        GameObject bs = bsc.GetChild(i - 1).gameObject;
        bs.GetComponent<Image>().color = ColorWinnerStarBright;
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
        SceneManager.LoadScene("Stage_0");
    }

    void FreezeGame()
    {
        IsGameFrozen = true;
    }

    void UnfreezeGame()
    {
        IsGameFrozen = false;
    }

    private string LastMinuteSecond = "";
    private string PropagatedGrownRockMinuteSecond = "";
    private int GrownRockPropagationSeconds = 5;
    private List<Rock> PropagatingGrownRock = new List<Rock>();

    private List<System.Drawing.Point> PropagatedBlackholeNumber = new List<System.Drawing.Point>();
    private string PropagatedBlackholeMinuteSecond = "";
    private int BlackholePropagationSeconds = 5;
    private bool PropagatingBlackhole = false;

    private bool FindBlackholeArea(System.Drawing.Point c, int posX, int posY, int size)
    {
        return (c.X >= posX && c.X < posX + size) && (c.Y >= posY && c.Y < posY + size);
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
                float fogScale = BaseFogScale * (1f - FogGrowSpeed * 0.0000001f);
                if (fogScale > MinFogScale)
                {
                    ScaleFog(fogScale);
                }
            }

            // difficulties

            if (BlackholeNumber > 0 && PropagatedBlackholeNumber.Count < BlackholeNumber)
            {
                if (LastMinuteSecond != minutes.ToString() + ":" + seconds.ToString())
                {
                    if ((seconds % BlackholeTimeMin == 0 || seconds % BlackholeTimeMax == 0) && Mathf.FloorToInt(TimeSpent) != 0)
                    {
                        int size = new System.Random().Next(BlackholeSizeMin, BlackholeSizeMax);
                        Debug.Log("Available Blackhole coordinates_ size => " + size);
                        float baseScale = 60f * size;

                        List<System.Drawing.Point> availCoord = AllCoord.FindAll(c => c.X < GridWidth - size + 1 && c.Y < GridHeight - size + 1);
                        System.Random random = new System.Random();
                        Debug.Log("Available Blackhole coordinates0 => " + availCoord.Count);
                        int rand = random.Next(availCoord.Count);
                        int posX = availCoord[rand].X;
                        int posY = availCoord[rand].Y;
                        availCoord.RemoveAt(rand);
                        Debug.Log("Available Blackhole coordinates1 => " + AllCoord.Count);

                        while(availCoord.Count > 0)
                        {
                            List<System.Drawing.Point> area = AllCoord.FindAll(c => FindBlackholeArea(c, posX, posY, size));
                            Debug.Log("Available Blackhole coordinates2 => " + area.Count);

                            if (area.Count < size * size)
                            {
                                rand = random.Next(availCoord.Count);
                                posX = availCoord[rand].X;
                                posY = availCoord[rand].Y;
                                availCoord.RemoveAt(rand);
                            }
                            else
                            {
                                AllCoord.RemoveAll(c => FindBlackholeArea(c, posX, posY, size));
                                Debug.Log("Available Blackhole coordinates3 => " + AllCoord.Count);
                                Debug.Log("Blackhole coordinate => posX: " + posX + ", posY: " + posY);

                                Vector3 bscale = transform.localScale;
                                bscale.Set(baseScale, baseScale, baseScale);
                                GameObject cell = GridList[(posY * GridWidth) + posX];
                                GameObject blackhole = (GameObject)Instantiate(EffectBlackhole, Vector3.zero, Quaternion.identity) as GameObject;
                                blackhole.transform.SetParent(cell.gameObject.transform);
                                blackhole.transform.localPosition = new Vector3(50f * (size - 1), -50f * (size - 1), 0f);
                                blackhole.transform.localScale = bscale;

                                PropagatedBlackholeNumber.Add(new System.Drawing.Point(posX, posY));

                                break;
                            }
                        }

                    }
                }
            }

            //propagating rocks
            if (RockGrowNumber > 0 && RockGrowTime != 0)
            {
                if (PropagatedGrownRockMinuteSecond == minutes.ToString() + ":" + seconds.ToString() && PropagatingGrownRock.Count > 0)
                {
                    PropagatingGrownRock.ForEach(rock =>
                    {
                        if (Rover.X == rock.X && Rover.Y == rock.Y)
                        {
                            GameOver();
                        }
                        Animator rockAnim = rock.GameObject.GetComponent<Animator>();
                        if (rockAnim != null)
                        {
                            rockAnim.enabled = false;
                            Image rockImage = rock.GameObject.GetComponent<Image>();
                            rockImage.sprite = Resources.Load<Sprite>("rock");
                            rockImage.color = ColorRockNormal;
                        }
                        RockList.Add(rock);
                    });
                    PropagatingGrownRock.RemoveAll(rock => rock != null);
                    Debug.Log("Rock propagated");
                }
            }
            // rocks propagation warning
            if (RockGrowNumber > 0 && RockGrowTime != 0 && RemainingCoord.Count >= RockGrowNumber)
            {
                // Debug.Log(Mathf.FloorToInt(TimeSpent) + " Grow " + RockGrowNumber + " rocks over " + RockGrowTime + " seconds ");
                // We have to grow some rocks over some time
                // every RockGrowTime we need to grow RockGrowNumber
                // the newly grown rocks can be anywhere the existing rocks are not present

                if (LastMinuteSecond != minutes.ToString() + ":" + seconds.ToString())
                {
                    Debug.Log(Mathf.FloorToInt(TimeSpent) + " | Grow " + RockGrowNumber + " rocks over " + RockGrowTime + " seconds | remaining space " + RemainingCoord.Count);

                    if (seconds % RockGrowTime == 0 && Mathf.FloorToInt(TimeSpent) != 0)
                    {
                        List<int> grownRockList = Enumerable.Range(0, RockGrowNumber).ToList();

                        System.Random random = new System.Random();

                        grownRockList.ForEach(i =>
                        {
                            int randomIndex = random.Next(0, RemainingCoord.Count);
                            Rock rock = new Rock();
                            rock.X = RemainingCoord[randomIndex].X;
                            rock.Y = RemainingCoord[randomIndex].Y;

                            GameObject cell = GridList[(rock.Y * GridWidth) + rock.X];
                            //pos = cell.gameObject.GetComponent<RectTransform>().localPosition;
                            //localPosition = new Vector3(pos.x, pos.y, ObjPosZ);

                            Vector3 scale = transform.localScale;
                            //scale.Set(150f, 150f, 150f);
                            //scale.Set(0.3f, 0.3f, 1);

                            rock.GameObject = Instantiate(GrownRockPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                            rock.GameObject.transform.SetParent(cell.gameObject.transform);
                            rock.GameObject.transform.localPosition = Vector3.zero;
                            rock.GameObject.transform.localScale = scale;
                            rock.GameObject.GetComponent<Image>().color = ColorRockNormal;
                            //rock.GameObject.transform.Rotate(-93f, -8.5f, 9f, Space.World);

                            PropagatingGrownRock.Add(rock);
                            RemainingCoord.RemoveAt(randomIndex);
                        });

                        int next_minutes;
                        int next_seconds;
                        if (TimeLeft - GrownRockPropagationSeconds <= 60f)
                        {
                            next_minutes = 0;
                            next_seconds = Mathf.FloorToInt(TimeLeft - GrownRockPropagationSeconds);
                        }
                        else
                        {
                            next_minutes = minutes;
                            next_seconds = seconds - GrownRockPropagationSeconds;
                        }
                        PropagatedGrownRockMinuteSecond = next_minutes.ToString() + ":" + next_seconds.ToString();
                        Debug.Log("newly grown rock to be propagated at " + PropagatedGrownRockMinuteSecond);
                    }
                }
            }

            LastMinuteSecond = minutes.ToString() + ":" + seconds.ToString();

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

    private List<System.Drawing.Point> RemainingCoord = new List<System.Drawing.Point>();
    private List<System.Drawing.Point> AllCoord = new List<System.Drawing.Point>();
    //IEnumerator GenMap()
    void GenMap()
    {
        //yield return new WaitForEndOfFrame();

        List<int> allX = Enumerable.Range(0, GridWidth).ToList();
        List<int> allY = Enumerable.Range(0, GridHeight).ToList();
        List<System.Drawing.Point> allCoord = new List<System.Drawing.Point>();
        int[][] grid = new int[GridHeight][];

        //Debug.Log("[" + string.Join(", ", allX) + "]");
        //Debug.Log("[" + string.Join(", ", allY) + "]");

        allY.ForEach(y =>
        {
            grid[y] = new int[GridWidth];
            allX.ForEach(x =>
            {
                allCoord.Add(new System.Drawing.Point(x, y));
                AllCoord.Add(new System.Drawing.Point(x, y));
                grid[y][x] = 0;
            });
        });

        Debug.Log("all coordinates");
        Debug.Log(string.Join(", ", allCoord));

        System.Random random = new System.Random();
        int randomIndex = random.Next(0, allCoord.Count);
        int randomDir = 0;// random.Next(0, DIR.Length);

        Debug.Log("Rover Direction: " + randomDir);

        Rover rover = new Rover();
        rover.dir = randomDir;
        rover.X = allCoord[randomIndex].X;
        rover.Y = allCoord[randomIndex].Y;
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
        //Debug.Log("allCoord.count=>" + allCoord.Count);
        allCoord.RemoveAt(randomIndex);
        //Debug.Log("allCoord.count=>" + allCoord.Count);

        RoverRotate(randomDir);

        List<int> rockList = Enumerable.Range(0, RockQty).ToList();
        rockList.ForEach(i =>
        {
            //Debug.Log(i);
            randomIndex = random.Next(0, allCoord.Count);
            Rock rock = new Rock();
            rock.X = allCoord[randomIndex].X;
            rock.Y = allCoord[randomIndex].Y;
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
            allCoord.RemoveAt(randomIndex);
        });

        List<int> resList = Enumerable.Range(0, ResourceQty).ToList();
        resList.ForEach(i =>
        {
            //Debug.Log(i);
            randomIndex = random.Next(0, allCoord.Count);
            Resource res = new Resource();
            res.X = allCoord[randomIndex].X;
            res.Y = allCoord[randomIndex].Y;

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
            allCoord.RemoveAt(randomIndex);
        });

        RemainingCoord = allCoord;

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