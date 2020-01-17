using UnityEngine;
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
public class Star
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
    public GameObject StarPrefab;
    public GameObject InstructionView;

    public GameObject GridScrollView;

    public GameObject ObjTurnLeft;
    public GameObject ObjTurnRight;
    public GameObject ObjForward;
    public GameObject ObjUndo;
    public GameObject ObjSend;

    public GameObject ObjTimeLeftText;
    public GameObject ObjProgressBg;
    public GameObject ObjProgressFg;

    public GameObject ObjWall;


    public int GridWidth = 8;
    public int GridHeight = 13;
    public float DistanceX = 1f;
    public float DistanceY = 1f;
    public float ObjPosZ = -1f;
    public int RockQty = 5;
    public int StarQty = 3;
    private string[] DIR = { "Up", "Right", "Down", "Left" };

    private List<GameObject> GridList = new List<GameObject>();
    private Rover Rover;
    private List<Rock> RockList = new List<Rock>();
    private List<Star> StarList = new List<Star>();
    private List<string> Instruction = new List<string>();

    public UnityEngine.Color ColorRockNormal = new UnityEngine.Color(0.84f, 0.81f, 0f, 1f);
    public UnityEngine.Color ColorRockError = new UnityEngine.Color(1f, 0f, 0f, 1f);

    public float TimeLeft = 180f;
    private float TimeSpent = 0f;
    private float ProgressBgWidth;
    private float ProgressBgHeight;
    private Vector2 GridSize;
    private Vector2 GridSpacing;
    private RectOffset GridPadding;

    void Start()
    {
        Time.timeScale = 1f;

        Rect progressBgRect = ObjProgressBg.gameObject.GetComponent<RectTransform>().rect;
        ProgressBgWidth = progressBgRect.width;
        ProgressBgHeight = progressBgRect.height;

        //Debug.Log("Progressbar Bg Width: " + ProgressBgWidth);

        CenterGrid();

        Populate();

        InitButtons();
    }

    private void InitButtons()
    {
        Button btnTurnLeft = ObjTurnLeft.GetComponent<Button>();
        Button btnTurnRight = ObjTurnRight.GetComponent<Button>();
        Button btnFoward = ObjForward.GetComponent<Button>();
        Button btnUndo = ObjUndo.GetComponent<Button>();
        Button btnSend = ObjSend.GetComponent<Button>();

        btnTurnLeft.onClick.AddListener(delegate { AppendInstruction("TurnLeft"); });
        btnTurnRight.onClick.AddListener(delegate { AppendInstruction("TurnRight"); });
        btnFoward.onClick.AddListener(delegate { AppendInstruction("Forward"); });
        btnUndo.onClick.AddListener(delegate { UndoInstruction("Undo"); });
        btnSend.onClick.AddListener(delegate { SendInstruction("Send"); });
    }

    private void CenterGrid()
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

    public void AppendInstruction(string action)
    {
        //Debug.Log(action);
        Instruction.Add(action);

        InstructionManager InstMgr = InstructionView.GetComponent<InstructionManager>();
        InstMgr.PopulateOne(action);
    }

    public void UndoInstruction(string action)
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


    public void SendInstruction(string action)
    {
        //Debug.Log(action);

        int dl = DIR.Length;
        for (int i = 0; i < Instruction.Count; i++)
        {
            string inst = Instruction[i];
            Debug.Log(inst);
            if (inst == "TurnLeft")
            {
                StartCoroutine(ExecuteInstruction(() =>
                {
                    Rover.GameObject.transform.Rotate(0, 0, 90, Space.World);
                    Rover.dir = (Rover.dir + dl - 1) % dl;
                }, i));
                
            }
            else if (inst == "TurnRight")
            {
                StartCoroutine(ExecuteInstruction(() =>
                {
                    Rover.GameObject.transform.Rotate(0, 0, -90, Space.World);
                    Rover.dir = (Rover.dir + 1) % dl;
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
                        GameObject cell = GridList[(dy * GridWidth) + dx];
                        Rover.GameObject.transform.SetParent(cell.gameObject.transform);
                        Rover.GameObject.transform.localPosition = Vector3.zero;
                        Rover.X = dx;
                        Rover.Y = dy;
                    }

                }, i));
            }
        }

        /*
        if (action == "TurnLeft")
        {
            // Rover.GameObject.transform.Rotate(0, 0, 90, Space.World);
        }
        else if (action == "TurnRight")
        {
            // Rover.GameObject.transform.Rotate(0, 0, -90, Space.World);
        }
        else if (action == "Foward")
        {

        }
        else if (action == "Undo")
        {

        }
        else if (action == "Send")
        {

        }
        */
    }

    void Update()
    {
        TimeLeft -= Time.deltaTime;
        TimeSpent += Time.deltaTime;
        float width = ProgressBgWidth * (TimeLeft / (TimeLeft + TimeSpent));
        float height = ProgressBgHeight;

        if (TimeLeft < 0)
        {
            //Do something useful or Load a new game scene depending on your use-case
            ObjProgressFg.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0, height);
        }
        else
        {
            int minutes = Mathf.FloorToInt(TimeLeft / 60f);
            int seconds = Mathf.FloorToInt(TimeLeft - minutes * 60);
            string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);

            //Debug.Log("Time Left: " + niceTime);

            ObjTimeLeftText.gameObject.GetComponent<TextMeshProUGUI>().SetText(niceTime);

            ObjProgressFg.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

        }

    }

    public void RoverRotate(int dir)
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

        List<int> starList = Enumerable.Range(0, StarQty).ToList();
        starList.ForEach(i =>
        {
            //Debug.Log(i);
            randomIndex = random.Next(0, allCord.Count);
            Star star = new Star();
            star.X = allCord[randomIndex].X;
            star.Y = allCord[randomIndex].Y;

            cell = GridList[(star.Y * GridWidth) + star.X];
            //pos = cell.gameObject.GetComponent<RectTransform>().localPosition;
            //localPosition = new Vector3(pos.x, pos.y, ObjPosZ);

            Vector3 scale = transform.localScale;
            scale.Set(0.8f, 0.8f, 1);

            star.GameObject = Instantiate(StarPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            star.GameObject.transform.SetParent(cell.gameObject.transform);
            star.GameObject.transform.localPosition = Vector3.zero;
            star.GameObject.transform.localScale = scale;

            StarList.Add(star);
            allCord.RemoveAt(randomIndex);
        });

        /*
        for (int i = 0; i < grid.Length; i++)
        {
            Debug.Log(string.Format("row: {0} [{1}]", i, string.Join(", ", grid[i])));
        }
        //*/

        Debug.Log("rover: " + rover.X + "," + rover.Y);
        Debug.Log("star: " + StarList[0].X + "," + StarList[0].Y);


        int[] startPos = { rover.X, rover.Y};

        for (int i = 0; i < StarList.Count; i++)
        {
            Star endStar = StarList[i];
            int[] endPos = { endStar.X, endStar.Y };
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
    }

    void Populate()
    {
        // align to middle
        //float offsetX = -(GridWidth - 1) * DistanceX / 2f;
        //float offsetY = -(GridHeight - 1) * DistanceY / 2f;
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
        //GenMap(offsetX, offsetY, DistanceX, DistanceY, GridContainer);
        //StartCoroutine(GenMap());
        GenMap();
    }
}