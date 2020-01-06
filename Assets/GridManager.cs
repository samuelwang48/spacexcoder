using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using TMPro;

public class Rover {
    public int X;
    public int Y;
    public GameObject GameObject;

}

public class GridManager : MonoBehaviour
{
    public GameObject gridCtrl;
    public int GridWidth;
    public int GridHeight;
    [Range(0,5)]
    public int distanceX;
    [Range(0,5)]
    public int distanceY;
    public Transform spawnPoint;
    public GameObject myPrefab;
    public GameObject RoverObject;
    public List<GameObject> list = new List<GameObject>();


    void Start()
    {
        CreateGrid();
    }

    public List<int> InitGrid() {
        List<int> grid = new List<int>();
        Debug.Log(grid);
        return grid;
    }

    public void GenMap(float offsetLeft, float offsetBottom, int distanceX, int distanceY, GameObject gameObject)
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
        int randomX = random.Next(0, allCord.Count);
        Rover rover = new Rover();
        rover.GameObject = RoverObject;
        rover.X = allCord[randomX].X;
        rover.Y = allCord[randomX].Y;

        Debug.Log(rover.X);
        Debug.Log(rover.Y);
        Debug.Log(rover.GameObject);
        rover.GameObject.transform.parent = gameObject.transform;
        //rover.GameObject.transform.position = new Vector3(offsetLeft, offsetBottom, 2f);
    }

    public void CreateGrid()
    {
        GameObject newEmptyGameObject = new GameObject("Grid");
        // following line is probably not neccessary
        newEmptyGameObject.transform.position = Vector3.zero;

        // align to middle
        float offsetLeft = (-GridWidth / 2f) * distanceX + distanceX / 2f;
        float offsetBottom = -((-GridHeight / 2f) * distanceY + distanceY / 2f);
        // align to top left corner
        /*
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;
        float offsetLeft = (-width / 2f) * distanceX + distanceX / 2f;
        float offsetBottom = (-height / 2f) * distanceY + distanceY / 2f + (height - (GridHeight * distanceY));
        */

        // set it as first spawn position (z=1 because you had it in your script)
        Vector3 nextPosition = new Vector3(offsetLeft, offsetBottom, 1f);

        for (int y = 0; y < GridHeight; y++)
        {
            for (int x = 0; x < GridWidth; x++)
            {
                GameObject clone = Instantiate(myPrefab, nextPosition, Quaternion.identity) as GameObject;
                clone.transform.parent = newEmptyGameObject.transform;
                clone.transform.GetChild(0).GetComponent<TextMeshPro>().text = x.ToString() + ',' + y.ToString();
                //clone.transform.localScale += new Vector3(0f, 0f, 0f);
                // add to list
                list.Add(clone);

                // add x distance
                nextPosition.x += distanceX;
            }
            // reset x position and add y distance
            nextPosition.x = offsetLeft;
            nextPosition.y -= distanceY;
        }
        // move the whole grid to the spawnPoint, if there is one
        if (spawnPoint != null)
            newEmptyGameObject.transform.position = spawnPoint.position;


        GenMap(offsetLeft, offsetBottom, distanceX, distanceY, newEmptyGameObject);
    }
}