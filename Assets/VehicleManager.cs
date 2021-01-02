using SpaceXCoder;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
    public DashConfig[] myDashConfig;
    public Save save;


    void Awake()
    {
        save = GameService.LoadSave();
        myDashConfig = save.GetDashConfig();
        SkillSlot = GameObject.FindGameObjectsWithTag("skillSlot");

        for (int i = 0; i < myDashConfig.Length; i++)
        {
            DashConfig c = myDashConfig[i];
            if (c != null)
            {
                Debug.Log("Dash Config => " + i + ", " + c.gameItemIndex + ", " + c.type);
                if (c.type == "GameItem" && c.gameItemIndex > -1)
                {
                    Debug.Log("Dash Config GameItem => " + c.gameItemIndex);
                    GameObject newObj = SpaceXCoder.Inventory.PopulateItem(SkillSlot[i].transform.GetChild(0).transform, PrefabItemTpl, c.gameItemIndex);
                    newObj.AddComponent<LayoutElement>();
                }
                else if (c.type == "Skill" && c.skillName != "")
                {
                    GameObject obj = GameObject.Find(c.skillName);
                    Transform parent = SkillSlot[i].transform.GetChild(0).transform;
                    GameObject newObj = UnityEngine.Object.Instantiate(obj, parent) as GameObject;
                    newObj.transform.SetParent(parent);
                    newObj.transform.SetSiblingIndex(0);
                    newObj.AddComponent<LayoutElement>();
                }
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Button btnExitStage = ObjExitStage.GetComponent<Button>();
        btnExitStage.onClick.AddListener(delegate { ExitStage(); });

        SpaceXCoder.Inventory.InitGameItemList(
            PrefabItemTpl,
            SkillGridCellPrefab,
            SkillGridContainer,
            null
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
        SceneManager.LoadScene("Main Scene");
    }
    public void SelectionAdded(ReorderableListEventStruct item)
    {
        Debug.Log("Event Received SelectionAdded");
        Debug.Log("Event Received Hello World, is my item a clone? [" + item.IsAClone + "]");
        RemovalArea.SetActive(false);
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
        Debug.Log("Event Received " + myDashConfig.Length);

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
