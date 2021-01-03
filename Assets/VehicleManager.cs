using SpaceXCoder;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using static UnityEngine.UI.Extensions.ReorderableList;

public class VehicleManager : MonoBehaviour
{
    public GameObject ObjExitStage;
    public float RotateSpeed = 1.1f;
    public GameObject PrefabItemTpl;
    public GameObject SkillGridContainer;
    public GameObject RemovalArea;
    public GameObject[] SkillSlot;
    public GameObject ResetModal;
    public Save save;


    void Awake()
    {
        save = GameService.LoadSave();
        SkillSlot = GameObject.FindGameObjectsWithTag("skillSlot").ToList<GameObject>().OrderBy(x => int.Parse(x.name.Replace("Slot_", ""))).ToArray();
        SpaceXCoder.Inventory.InitDash(SkillSlot, PrefabItemTpl, null);

    }
    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindGameObjectsWithTag("draggableBg").ToList<GameObject>().ForEach(g => {
            Debug.Log("g.name => " + g.name);
            g.GetComponent<ReorderableListElement>().enabled = false;
        });

        Button btnExitStage = ObjExitStage.GetComponent<Button>();
        btnExitStage.onClick.AddListener(delegate {
            ExitStage();
        });

        SpaceXCoder.Inventory.InitGameItemList(
            PrefabItemTpl,
            SkillGridContainer
        );

        GameObject[] list = GameObject.FindGameObjectsWithTag("ship");
        foreach (GameObject s in list)
        {
            s.SetActive(false);
        }

        GameObject prev = list[0];
        prev.SetActive(true);

        for (int i = 0; i < 5; i++)
        {
            GameObject[] li = list;
            int j = i;
            GameObject b = GameObject.Find("R" + i.ToString());
                
            b.GetComponent<Button>().onClick.AddListener(() => {
                GameObject current = null;
                for (int k = 0; k < 5; k++)
                {
                    if (li[k].name == "S" + j.ToString())
                    {
                        Debug.Log("current name => " + li[k].name);
                        current = li[k];
                    }
                }
                prev.SetActive(false);
                current.SetActive(true);
                prev = current;
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * RotateSpeed);
    }

    void ExitStage()
    {
        SceneManager.LoadScene(PlayerPrefs.GetString("VehicleExitToScene"));
    }

    public string ReplaceCloneInName(string name)
    {
        return Regex.Replace(name, "\\(.*$", "");
    }

    public void SelectionAdded(ReorderableListEventStruct item)
    {
        RemovalArea.SetActive(false);
        Debug.Log("next sibling => " + item.FromList.gameObject.name + ", " + item.ToList.gameObject.name);
        Debug.Log("next sibling => " + item.SourceObject.gameObject.name + ", " + item.DroppedObject.gameObject.name);
        if (item.FromList.gameObject.name == item.ToList.gameObject.name)
        {
            return;
        }

        item.SourceObject.name = ReplaceCloneInName(item.SourceObject.name);
        item.DroppedObject.name = ReplaceCloneInName(item.DroppedObject.name);

        int fromIndex = System.Array.IndexOf(SkillSlot, item.FromList.gameObject);
        int toIndex = System.Array.IndexOf(SkillSlot, item.ToList.gameObject);
        string sourceItemTag = item.SourceObject.tag;
        string sourceObjectName = item.SourceObject.name;
        string sourceType = "Unknown";
        sourceType = sourceItemTag == "skill" ? "Skill" : sourceType;
        sourceType = sourceItemTag == "gameItem" ? "GameItem" : sourceType;


        GameObject targetObj = item.ToList.transform.GetChild(0).transform.GetChild(2).gameObject;
        string targetItemTag = targetObj.tag; // either "skill" or "gameItem"

        /*
        Debug.Log("Event Received SelectionAdded");
        Debug.Log("Event Received item.FromList.gameObject " + item.FromList.gameObject.name);
        Debug.Log("Event Received item.ToList.gameObject " + item.ToList.gameObject.name);
        
        Debug.Log("Event Received sourceItemTag => " + sourceItemTag);
        Debug.Log("Event Received sourceObjectName => " + sourceObjectName);
        Debug.Log("Event Received sourceType => " + sourceType);
        */
        if (sourceType == "Skill")
        {
            string sourceSkillName = ReplaceCloneInName(sourceObjectName);
            Debug.Log("Event Received skillName => " + sourceSkillName);
            
            save.UpdateDashConfig(toIndex, -1, sourceSkillName, sourceType);
        }
        else if (sourceType == "GameItem")
        {
            int gameItemIndex = int.Parse(ReplaceCloneInName(sourceObjectName));
            Debug.Log("Event Received gameItemIndex => " + gameItemIndex);
            save.UpdateDashConfig(toIndex, gameItemIndex, "", sourceType);
        }

        if (targetItemTag != "draggableBg")
        {
            string targetObjectName = targetObj.name;
            string targetType = "Unknown";
            targetType = targetItemTag == "skill" ? "Skill" : targetType;
            targetType = targetItemTag == "gameItem" ? "GameItem" : targetType;

            Debug.Log("Event Received targetObjectName => " + targetObjectName);
            Debug.Log("Event Received targetItemTag => " + targetItemTag);
            Debug.Log("Event Received targetType => " + targetType);

            if (targetType == "Skill")
            {
                string targetSkillName = ReplaceCloneInName(targetObjectName);
                Debug.Log("Event Received targetSkillName => " + targetSkillName);
                save.UpdateDashConfig(fromIndex, -1, targetSkillName, targetType);
            }
            else if (targetType == "GameItem")
            {
                int targetGameItemIndex = int.Parse(ReplaceCloneInName(targetObjectName));
                Debug.Log("Event Received targetGameItemIndex => " + targetGameItemIndex);
                save.UpdateDashConfig(fromIndex, targetGameItemIndex, "", targetType);
            }

            int i = item.SourceObject.transform.GetSiblingIndex();
            GameObject next = item.SourceObject.transform.parent.GetChild(i + 2).gameObject;
            Debug.Log("next sibling => " + next.name);
            next.name = ReplaceCloneInName(next.name);
            next.transform.SetParent(item.FromList.transform.GetChild(0));
            next.transform.SetSiblingIndex(0);
        }
        else
        {
            save.UpdateDashConfig(fromIndex, -1, "", "");
        }

        GameService.Write(save);
    }

    public void SkillGrabbed(ReorderableListEventStruct item)
    {
        Transform t = item.DroppedObject.transform;
        string objectName = ReplaceCloneInName(t.name);
        Debug.Log("Event Received SkillGrabbed2 t.name => " + t.name);
        Debug.Log("Event Received SkillGrabbed2 item.SourceObject.tag => " + item.SourceObject.tag);
        if (item.SourceObject.tag == "gameItem")
        {
            Transform qty = t.GetChild(1);
            int gameItemIndex = int.Parse(objectName);
            Debug.Log("Event Received SkillGrabbed3 => " + qty.name);
            Dictionary<string, int> dict = GameService.LoadSave().ListItemDict();
            KeyValuePair<string, int> kv = dict.ElementAt(gameItemIndex);
            qty.GetComponent<TextMeshProUGUI>().SetText(kv.Value.ToString());
        }
    }
    public void SkillAdded(ReorderableListEventStruct item)
    {
        item.SourceObject.name = ReplaceCloneInName(item.SourceObject.name);
        item.DroppedObject.name = ReplaceCloneInName(item.DroppedObject.name);

        int index = System.Array.IndexOf(SkillSlot, item.ToList.gameObject);
        string itemTag = item.DroppedObject.tag;
        string type = "Unknown";
        type = itemTag == "skill" ? "Skill" : type;
        type = itemTag == "gameItem" ? "GameItem" : type;

        GameObject targetObj = item.ToList.transform.GetChild(0).transform.GetChild(2).gameObject;
        string targetItemTag = targetObj.tag; // either "skill" or "gameItem"

        Debug.Log("Event Received SkillAdded");
        Debug.Log("Event Received Hello World, is my item a clone? [" + item.IsAClone + "]");
        Debug.Log("Event Received " + item.ToList.name);
        Debug.Log("Event Received item.SourceObject.name " + item.SourceObject.name);
        Debug.Log("Event Received targetItemTag " + targetItemTag);

        if (type == "Skill")
        {
            string skillName = ReplaceCloneInName(item.SourceObject.name);
            save.UpdateDashConfig(index, -1, skillName, type);
        }
        else if (type == "GameItem")
        {
            int gameItemIndex = int.Parse(ReplaceCloneInName(item.SourceObject.name));
            save.UpdateDashConfig(index, gameItemIndex, "", type);
        }

        if (targetItemTag != "draggableBg")
        {
            int i = item.DroppedObject.transform.GetSiblingIndex();
            GameObject next = item.DroppedObject.transform.parent.GetChild(i + 2).gameObject;
            Debug.Log("next sibling => " + next.name);
            Destroy(next);
        }

        GameService.Write(save);
    }

    public void SelectionGrabbed(ReorderableListEventStruct item)
    {
        RemovalArea.SetActive(true);
        Debug.Log("Event Received SelectionGrabbed");
        Debug.Log("Event Received Hello World, is my item a clone? [" + item.IsAClone + "]");
    }

    public void RemovalAdded(ReorderableListEventStruct item)
    {
        RemovalArea.SetActive(false);

        Debug.Log("Event Received RemovalAdded");
        Debug.Log("Event Received RemovalAdded => " + (item.DroppedObject == item.SourceObject) + " FromIndex " + item.FromIndex + " FromList " + item.FromList);

        int index = System.Array.IndexOf(SkillSlot, item.FromList.gameObject);
        save.UpdateDashConfig(index, -1, "", "");

        Destroy(item.DroppedObject);

        GameService.Write(save);
    }
}
