using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Rover {
    public int X;
    public int Y;
    public GameObject GameObject;

}

public class Rock
{
    public int X;
    public int Y;
    public GameObject GameObject;

}

public class GridManager : MonoBehaviour
{
    public GameObject GridCtrl;
    public GameObject MyPrefab;
    public GameObject RoverPrefab;
    public GameObject RockPrefab;
    public GameObject ObjTurnLeft;
    public GameObject ObjTurnRight;

    public int GridWidth;
    public int GridHeight;
    public float ObjPosZ = 199f;
    public float GridPosZ = 200f;

    public float Scale = 0.5f;
    public float DistanceX = 1f;
    public float DistanceY = 1f;
    public int RockQty = 5;

    public Transform SpawnPoint;

    public List<GameObject> GridList = new List<GameObject>();
    public Rover Rover;
    public List<Rock> RockList = new List<Rock>();

    void Start()
    {
        Button btnTurnLeft = ObjTurnLeft.GetComponent<Button>();
        Button btnTurnRight = ObjTurnRight.GetComponent<Button>();

        CreateGrid();

        btnTurnLeft.onClick.AddListener(delegate { ActionButtonClicked("TurnLeft"); });
        btnTurnRight.onClick.AddListener(delegate { ActionButtonClicked("TurnRight"); });
    }

    public void ActionButtonClicked(string action)
    {
        Debug.Log(action);
        if (action == "TurnLeft")
        {
            Rover.GameObject.transform.Rotate(0, 0, 90, Space.World);
        }
        else if (action == "TurnRight")
        {
            Rover.GameObject.transform.Rotate(0, 0, -90, Space.World);
        }
    }

    public List<int> InitGrid() {
        List<int> grid = new List<int>();
        Debug.Log(grid);
        return grid;
    }

    public void GenMap(float offsetX, float offsetY, float DistanceX, float DistanceY, GameObject gameObject)
    {
        List<int> allX = Enumerable.Range(0, GridWidth).ToList();
        List<int> allY = Enumerable.Range(0, GridHeight).ToList();
        List<Point> allCord = new List<Point>();
        List<List<int>> grid = new List<List<int>>();


        Debug.Log("[" + string.Join(", ", allX) + "]");
        Debug.Log("[" + string.Join(", ", allY) + "]");

        allY.ForEach(y =>
        {
            List<int> row = new List<int>();
            allX.ForEach(x =>
            {
                allCord.Add(new Point(x, y));
                row.Add(0);
            });
            Debug.Log(string.Format("row: {0} [{1}]", y, string.Join(", ", row)));
            grid.Add(row);
        });

        Debug.Log(string.Join(", ", allCord));

        System.Random random = new System.Random();
        int randomIndex = random.Next(0, allCord.Count);
        Rover rover = new Rover();
        rover.GameObject = RoverPrefab;
        rover.X = allCord[randomIndex].X;
        rover.Y = allCord[randomIndex].Y;
        Debug.Log(rover.X);
        Debug.Log(rover.Y);
        Debug.Log(rover.GameObject);
        rover.GameObject.transform.parent = gameObject.transform;
        rover.GameObject.transform.position = new Vector3(offsetX + rover.X, offsetY + rover.Y, ObjPosZ);

        Rover = rover;
        Debug.Log("allCord.count=>" + allCord.Count);
        allCord.RemoveAt(randomIndex);
        Debug.Log("allCord.count=>" + allCord.Count);

        List<int> rockList = Enumerable.Range(0, RockQty).ToList();
        rockList.ForEach(i =>
        {
            Debug.Log(i);
            randomIndex = random.Next(0, allCord.Count);
            Rock rock = new Rock();
            rock.X = allCord[randomIndex].X;
            rock.Y = allCord[randomIndex].Y;
            rock.GameObject = Instantiate(RockPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            rock.GameObject.transform.parent = gameObject.transform;
            rock.GameObject.transform.position = new Vector3(offsetX + rock.X, offsetY + rock.Y, ObjPosZ);
            rock.GameObject.transform.Rotate(-93f, -8.5f, 9f, Space.World);
            RockList.Add(rock);
            allCord.RemoveAt(randomIndex);
        });
    }

    public void CreateGrid()
    {
        GameObject newEmptyGameObject = new GameObject("Grid");
        // following line is probably not neccessary
        newEmptyGameObject.transform.position = Vector3.zero;

        // align to middle
        float offsetX = -(GridWidth - 1) * DistanceX / 2f;
        float offsetY = -(GridHeight - 1) * DistanceY / 2f + 4f;

        // align to top left corner
        /*
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;
        float offsetX = (-width / 2f) * DistanceX + DistanceX / 2f;
        float offsetY = (-height / 2f) * DistanceY + DistanceY / 2f + (height - (GridHeight * DistanceY));
        */

        // set it as first spawn position (z=1 because you had it in your script)
        Vector3 nextPosition = new Vector3(offsetX, offsetY, GridPosZ);

        for (int y = 0; y < GridHeight; y++)
        {
            for (int x = 0; x < GridWidth; x++)
            {
                GameObject clone = Instantiate(MyPrefab, nextPosition, Quaternion.identity) as GameObject;
                clone.transform.parent = newEmptyGameObject.transform;
                clone.transform.GetChild(0).GetComponent<TextMeshPro>().text = x.ToString() + ',' + y.ToString();
                //clone.transform.localScale += new Vector3(0f, 0f, 0f);
                // add to list
                GridList.Add(clone);

                // add x distance
                nextPosition.x += DistanceX;
            }
            // reset x position and add y distance
            nextPosition.x = offsetX;
            nextPosition.y += DistanceY;
        }
        // move the whole grid to the SpawnPoint, if there is one
        if (SpawnPoint != null)
            newEmptyGameObject.transform.position = SpawnPoint.position;


        GenMap(offsetX, offsetY, DistanceX, DistanceY, newEmptyGameObject);


        Vector3 scale = transform.localScale;
        scale.Set(Scale, Scale, 1);
        newEmptyGameObject.transform.localScale = scale;
    }
}