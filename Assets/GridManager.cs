﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    public GameObject ObjTurnLeft;
    public GameObject ObjTurnRight;
    public GameObject ObjForward;
    public GameObject ObjUndo;
    public GameObject ObjSend;


    public int GridWidth;
    public int GridHeight;
    public float DistanceX = 1f;
    public float DistanceY = 1f;
    public float ObjPosZ = -1f;
    public int RockQty = 5;
    public int StarQty = 3;

    public List<GameObject> GridList = new List<GameObject>();
    public Rover Rover;
    public List<Rock> RockList = new List<Rock>();
    public List<Star> StarList = new List<Star>();
    public List<string> Instruction = new List<string>();


    void Start()
    {
        InstructionManager InstMgr = InstructionView.GetComponent<InstructionManager>();
        InstMgr.Ring();

        GridLayoutGroup glg = gameObject.GetComponent<GridLayoutGroup>();
        glg.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        glg.constraintCount = GridHeight;
        Populate();

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

    public void SendInstruction(string action)
    {
        //Debug.Log("[" + string.Join(", ", Instruction) + "]");
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
        Rover rover = new Rover();
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
        roverScale.Set(0.4f, 0.4f, 1f);

        rover.GameObject = Instantiate(RoverPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        rover.GameObject.transform.SetParent(cell.gameObject.transform);
        rover.GameObject.transform.localPosition = Vector3.zero;
        rover.GameObject.transform.localScale = roverScale;

        Rover = rover;
        //Debug.Log("allCord.count=>" + allCord.Count);
        allCord.RemoveAt(randomIndex);
        //Debug.Log("allCord.count=>" + allCord.Count);

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
            scale.Set(150f, 150f, 150f);
            //scale.Set(0.3f, 0.3f, 1);

            rock.GameObject = Instantiate(RockPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            rock.GameObject.transform.SetParent(cell.gameObject.transform);
            rock.GameObject.transform.localPosition = Vector3.zero;
            rock.GameObject.transform.localScale = scale;
            rock.GameObject.transform.Rotate(-93f, -8.5f, 9f, Space.World);

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
            scale.Set(0.3f, 0.3f, 1);

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

            //newObj.GetComponent<Image>().color = Random.ColorHSV();
        }
        //GenMap(offsetX, offsetY, DistanceX, DistanceY, GridContainer);
        //StartCoroutine(GenMap());
        GenMap();
    }
}