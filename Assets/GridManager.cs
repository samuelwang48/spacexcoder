using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Collections;
using System;
using TMPro;

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
    public GameObject InstructionView;

    public GameObject GridScrollView;
    public GameObject BannerResourceContainer;
    public GameObject WinModal;
    public GameObject LoseModal;

    public GameObject ObjTurnLeft;
    public GameObject ObjTurnRight;
    public GameObject ObjForward;
    public GameObject ObjUndo;
    public GameObject ObjSend;

    public GameObject ObjTimeLeftText;
    public GameObject ObjProgressBg;
    public GameObject ObjProgressFg;

    public GameObject ObjWall;

    public GameObject ObjFinishContinue;
    public GameObject ObjForceExit;
    public GameObject ObjPlayAgain;
    public GameObject ObjGameOverExit;

    public GameObject ObjTimeLeftCountdown;
    public GameObject ObjWinnerStarContainer;

    private const float ZEROF = 0f;

    public int GridWidth = 11;
    public int GridHeight = 17;
    public int RockQty = 5;
    public int ResourceQty = 3;
    // public float DistanceX = 1f;
    // public float DistanceY = 1f;
    // public float ObjPosZ = -1f;


    private string[] DIR = { "Up", "Right", "Down", "Left" };

    private List<GameObject> GridList = new List<GameObject>();
    private Rover Rover;
    private List<Rock> RockList = new List<Rock>();
    private List<Resource> ResList = new List<Resource>();
    private List<GameObject> BannerResourceList = new List<GameObject>();
    private List<string> Instruction = new List<string>();

    public UnityEngine.Color ColorRockNormal = new UnityEngine.Color(0.84f, 0.81f, 0f, 1f);
    public UnityEngine.Color ColorRockError = new UnityEngine.Color(1f, 0f, 0f, 1f);

    public UnityEngine.Color ColorBannerResourceDark = new UnityEngine.Color(0.27f, 0.27f, 0.27f, 1f);
    public UnityEngine.Color ColorBannerResourceBright = new UnityEngine.Color(1f, 1f, 1f, 1f);
    public UnityEngine.Color ColorBannerResourceHidden = new UnityEngine.Color(1f, 1f, 1f, 0f);

    public UnityEngine.Color ColorWinnerStarDark = new UnityEngine.Color(0f, 0f, 0f, 0.5f);
    public UnityEngine.Color ColorWinnerStarBright = new UnityEngine.Color(0f, 0.728f, 1f, 1f);
    public UnityEngine.Color ColorWinnerStarHidden = new UnityEngine.Color(1f, 1f, 1f, 0f);

    private float TimeLeft = 90f;
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


    void InitLevel()
    {
        CurrentLevel = PlayerPrefs.GetInt("level");
        Debug.Log("Current level is: " + CurrentLevel);

        switch (CurrentLevel)
        {
            case 0:
                GridWidth = 3;
                GridHeight = 3;
                RockQty = 0;
                ResourceQty = 1;
                break;
            case 1:
                GridWidth = 3;
                GridHeight = 3;
                RockQty = 3;
                ResourceQty = 2;
                break;
            case 2:
                GridWidth = 5;
                GridHeight = 5;
                RockQty = 4;
                ResourceQty = 3;
                break;
            case 3:
                GridWidth = 5;
                GridHeight = 5;
                RockQty = 5;
                ResourceQty = 4;
                break;
            case 4:
                GridWidth = 5;
                GridHeight = 5;
                RockQty = 6;
                ResourceQty = 5;
                break;
            case 5:
                GridWidth = 6;
                GridHeight = 6;
                RockQty = 8;
                ResourceQty = 5;
                break;
            case 6:
                GridWidth = 7;
                GridHeight = 7;
                RockQty = 11;
                ResourceQty = 5;
                break;
            case 7:
                GridWidth = 8;
                GridHeight = 8;
                RockQty = 15;
                ResourceQty = 5;
                break;
            case 8:
                GridWidth = 9;
                GridHeight = 9;
                RockQty = 19;
                ResourceQty = 5;
                break;
            case 9:
                GridWidth = 10;
                GridHeight = 10;
                RockQty = 24;
                ResourceQty = 5;
                break;
            case 10:
                GridWidth = 11;
                GridHeight = 11;
                RockQty = 30;
                ResourceQty = 5;
                break;
            case 11:
                GridWidth = 11;
                GridHeight = 12;
                RockQty = 32;
                ResourceQty = 5;
                break;
            case 12:
                GridWidth = 11;
                GridHeight = 13;
                RockQty = 35;
                ResourceQty = 5;
                break;
            case 13:
                GridWidth = 11;
                GridHeight = 14;
                RockQty = 37;
                ResourceQty = 5;
                break;
            case 14:
                GridWidth = 11;
                GridHeight = 15;
                RockQty = 40;
                ResourceQty = 5;
                break;
            case 15:
                GridWidth = 11;
                GridHeight = 16;
                RockQty = 43;
                ResourceQty = 5;
                break;
            case 16:
                GridWidth = 11;
                GridHeight = 17;
                RockQty = 45;
                ResourceQty = 5;
                break;
            case 17:
                GridWidth = 11;
                GridHeight = 17;
                RockQty = 45;
                ResourceQty = 5;
                break;
            case 18:
                GridWidth = 11;
                GridHeight = 17;
                RockQty = 50;
                ResourceQty = 5;
                break;
            case 19:
                GridWidth = 11;
                GridHeight = 17;
                RockQty = 60;
                ResourceQty = 5;
                break;
            default:
                break;
        }
    }

    void Start()
    {
        Time.timeScale = 1f;

        InitLevel();

        InitWinModal();
        InitLoseModal();

        InitBanner();

        CenterGrid();

        PopulateGrid();

        GenMap();

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

        //Debug.Log("Progressbar Bg Width: " + ProgressBgWidth);

        Transform bsc = BannerResourceContainer.transform;
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
    }

    void UndoInstruction(string action)
    {
        //Debug.Log(action);

        Instruction.RemoveAt(Instruction.Count - 1);

        InstructionManager InstMgr = InstructionView.GetComponent<InstructionManager>();
        InstMgr.Undo();

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
        }
    }

    void TestExecutionIndex(int index, int count)
    {

        Debug.Log("Executing: " + (index + 1) + "/" + count);
        if (index == count - 1)
        {
            IsInstExecuting = false;
            ActivateSendButton();
        }
    }

    void DeactivateSendButton()
    {
        UnityEngine.Color colorExecuting = new UnityEngine.Color();
        UnityEngine.ColorUtility.TryParseHtmlString("#939393", out colorExecuting);
        Button btnSend = ObjSend.GetComponent<Button>();
        btnSend.GetComponent<Image>().color = colorExecuting;
    }

    void ActivateSendButton()
    {
        UnityEngine.Color colorExecuting = new UnityEngine.Color();
        UnityEngine.ColorUtility.TryParseHtmlString("#EC5353", out colorExecuting);
        Button btnSend = ObjSend.GetComponent<Button>();
        btnSend.GetComponent<Image>().color = colorExecuting;
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
            DeactivateSendButton();
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
        int nextLevel = CurrentLevel + 1;
        int unlocked = PlayerPrefs.GetInt("unlocked");
        if (nextLevel > unlocked)
        {
            PlayerPrefs.SetInt("unlocked", nextLevel);
        }
        WinModal.SetActive(true);

        Debug.Log("Win Time Left: " + TimeLeft);
        Debug.Log("Win Time Left Rounded: " + Math.Round(TimeLeft));
        for (int i = 0; i < Math.Round(TimeLeft); i++)
        {
            StartCoroutine(CalcStar(i));
        }
    }

    IEnumerator CalcStar(int i)
    {
        yield return new WaitForSeconds(i*.02f);

        int total = (int)Math.Round(TimeLeft);
        int earned = total - i - 1;
        ObjTimeLeftCountdown.gameObject.GetComponent<TextMeshProUGUI>().SetText(earned + "s");

        if (i % 30 == 0)
        {
            int indexOfStar = i / 30;
            Transform bsc = ObjWinnerStarContainer.transform;
            GameObject bs = bsc.GetChild(indexOfStar).gameObject;
            bs.GetComponent<Image>().color = ColorWinnerStarBright;
            Debug.Log("new Star! " + indexOfStar);
        }
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
        float width = ProgressBgWidth * (TimeLeft / (TimeLeft + TimeSpent));
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
        }

    }

    void RoverRotate(int dir)
    {
        Rover.GameObject.transform.Rotate(0f, 0f, dir * -90f, Space.World);
    }

    //IEnumerator GenMap()
    void GenMap()
    {
        //yield return new WaitForEndOfFrame();

        List<int> allX = Enumerable.Range(0, GridWidth).ToList();
        List<int> allY = Enumerable.Range(0, GridHeight).ToList();
        List<Point> allCord = new List<Point>();
        int[][] grid = new int[GridHeight][];

        //Debug.Log("[" + string.Join(", ", allX) + "]");
        //Debug.Log("[" + string.Join(", ", allY) + "]");

        allY.ForEach(y =>
        {
            grid[y] = new int[GridWidth];
            allX.ForEach(x =>
            {
                allCord.Add(new Point(x, y));
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