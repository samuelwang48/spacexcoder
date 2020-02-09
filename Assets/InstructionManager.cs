using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionManager : MonoBehaviour
{
    public GameObject prefab;
    public GameObject pfTurnLeft;
    public GameObject pfTurnRight;
    public GameObject pfForward;

    public List<GameObject> InstructionList = new List<GameObject>();

    public int numberToCreate; // number of objects to create. Exposed in inspector

    void Start()
    {
        Populate();
    }

    void Update()
    {

    }

    public void Ring()
    {
        Debug.Log("Ring!");
    }

    public void PopulateOne(string type)
    {
        GameObject newObj; // Create GameObject instance

        if (type == "TurnLeft")
        {
            newObj = (GameObject)Instantiate(pfTurnLeft, transform);
            InstructionList.Add(newObj);
        }
        else if (type == "TurnRight")
        {
            newObj = (GameObject)Instantiate(pfTurnRight, transform);
            InstructionList.Add(newObj);
        }
        else if (type == "Forward")
        {
            newObj = (GameObject)Instantiate(pfForward, transform);
            InstructionList.Add(newObj);
        }

        //newObj.GetComponent<Image>().color = Random.ColorHSV();

    }

    public void ClearAll()
    {
        InstructionList.ForEach(inst =>
        {
            Destroy(inst.gameObject);
        });
        InstructionList.Clear();
    }

    public void ClearFirst()
    {
        Destroy(InstructionList[0].gameObject);
        InstructionList.RemoveAt(0);
    }

    public void Undo()
    {
        if (InstructionList.Count > 0) {
            Destroy(InstructionList[InstructionList.Count - 1].gameObject);
            InstructionList.RemoveAt(InstructionList.Count - 1);
        }
    }

    void Populate()
    {
        GameObject newObj; // Create GameObject instance

        for (int i = 0; i < numberToCreate; i++)
        {
            // Create new instances of our prefab until we've created as many as we specified
            newObj = (GameObject)Instantiate(prefab, transform);

            // Randomize the color of our image
            // newObj.GetComponent<Image>().color = Random.ColorHSV();
        }

    }
}