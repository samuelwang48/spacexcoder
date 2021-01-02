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
    public GameObject SkillGridCellPrefab;
    public GameObject SkillGridContainer;
    public GameObject RemovalArea;
    public GameObject[] SkillSlot;
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
        Button btnExitStage = ObjExitStage.GetComponent<Button>();
        btnExitStage.onClick.AddListener(delegate {
            ExitStage();
        });

        SpaceXCoder.Inventory.InitGameItemList(
            PrefabItemTpl,
            SkillGridCellPrefab,
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
    public void SelectionAdded(ReorderableListEventStruct item)
    {
        int fromIndex = System.Array.IndexOf(SkillSlot, item.FromList.gameObject);
        int toIndex = System.Array.IndexOf(SkillSlot, item.ToList.gameObject);
        Regex rgxGameItem = new Regex(@"^\d+$");
        string skillName = Regex.Replace(item.SourceObject.name, "^(.*)\\(.*$", "$1");
        string itemTag = item.SourceObject.tag;
        string objectName = item.SourceObject.name;
        string type;

        Debug.Log("Event Received SelectionAdded");
        Debug.Log("Event Received Hello World, is my item a clone? [" + item.IsAClone + "]");
        Debug.Log("Event Received item.FromList.gameObject " + item.FromList.gameObject.name);
        Debug.Log("Event Received item.ToList.gameObject " + item.ToList.gameObject.name);
        Debug.Log("Event Received toIndex " + toIndex);
        Debug.Log("Event Received itemTag " + itemTag);

        if (itemTag == "skill")
        {
            type = "Skill";
            Debug.Log("Event Received skillName => " + skillName);
            save.UpdateDashConfig(fromIndex, -1, "", "");
            save.UpdateDashConfig(toIndex, -1, skillName, type);
        }
        else if (rgxGameItem.IsMatch(objectName))
        {
            type = "GameItem";
            int gameItemIndex = int.Parse(objectName);
            save.UpdateDashConfig(fromIndex, -1, "", "");
            save.UpdateDashConfig(toIndex, gameItemIndex, "", type);
        }
        else
        {
            type = "Unknown";
        }
        GameService.Write(save);
        RemovalArea.SetActive(false);
    }

    public void SkillGrabbed(ReorderableListEventStruct item)
    {
        Transform t = item.DroppedObject.transform.GetChild(0).transform;
        Debug.Log("Event Received SkillGrabbed2 => " + t.name);
        int gameItemIndex = int.Parse(t.name);
        Transform qty = t.GetChild(1);

        if (qty != null)
        {
            Debug.Log("Event Received SkillGrabbed3 => " + qty.name);
            Dictionary<string, int> dict = GameService.LoadSave().ListItemDict();
            KeyValuePair<string, int> kv = dict.ElementAt(gameItemIndex);
            qty.GetComponent<TextMeshProUGUI>().SetText(kv.Value.ToString());
        }
    }
    public void SkillAdded(ReorderableListEventStruct item)
    {
        Regex rgxSkill = new Regex(@"^Skill.*");
        Regex rgxGameItem = new Regex(@"^InventoryCell.*");

        int index = System.Array.IndexOf(SkillSlot, item.ToList.gameObject);
        string objectName = item.DroppedObject.name;
        string type;

        Debug.Log("Event Received SkillAdded");
        Debug.Log("Event Received Hello World, is my item a clone? [" + item.IsAClone + "]");
        Debug.Log("Event Received " + item.ToList.name);
        Debug.Log("Event Received " + index);

        if (rgxSkill.IsMatch(objectName))
        {
            type = "Skill";
            string skillName = item.DroppedObject.transform.GetChild(0).name;
            save.UpdateDashConfig(index, -1, skillName, type);
        }
        else if (rgxGameItem.IsMatch(objectName))
        {
            type = "GameItem";
            int gameItemIndex = int.Parse(item.DroppedObject.transform.GetChild(0).name);
            save.UpdateDashConfig(index, gameItemIndex, "", type);
        }
        else
        {
            type = "Unknown";
        }
        GameService.Write(save);
    }

    public void SelectionGrabbed(ReorderableListEventStruct item)
    {
        Debug.Log("Event Received SelectionGrabbed");
        Debug.Log("Event Received Hello World, is my item a clone? [" + item.IsAClone + "]");
        RemovalArea.SetActive(true);
    }

    public void RemovalAdded(ReorderableListEventStruct item)
    {
        Debug.Log("Event Received RemovalAdded");

        Debug.Log("Event Received RemovalAdded => " + (item.DroppedObject == item.SourceObject) + " FromIndex " + item.FromIndex + " FromList " + item.FromList);
        int index = System.Array.IndexOf(SkillSlot, item.FromList.gameObject);
        save.UpdateDashConfig(index, -1, "", "");
        GameService.Write(save);

        Destroy(item.DroppedObject);
        RemovalArea.SetActive(false);
    }
}
