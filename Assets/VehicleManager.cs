using SpaceXCoder;
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

    void Awake()
    {
        SkillSlot = GameObject.FindGameObjectsWithTag("skillSlot");
    }
    // Start is called before the first frame update
    void Start()
    {
        Button btnExitStage = ObjExitStage.GetComponent<Button>();
        btnExitStage.onClick.AddListener(delegate { ExitStage(); });

        SpaceXCoder.Inventory.InitSkillList(
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

        Transform slot = item.DroppedObject.transform.parent.transform.parent;
        int index = System.Array.IndexOf(SkillSlot, slot.gameObject);
        string name = item.DroppedObject.name;
        string type;
        if (rgxSkill.IsMatch(name))
        {
            type = "Skill";
        }
        else if (rgxGameItem.IsMatch(name))
        {
            type = "GameItem";
        }
        else
        {
            type = "Unknown";
        }

        Debug.Log("Event Received SkillAdded");
        Debug.Log("Event Received Hello World, is my item a clone? [" + item.IsAClone + "]");
        Debug.Log("Event Received " + slot.name);
        Debug.Log("Event Received " + index);
        Save save = GameService.LoadSave();
        DashConfig[] myDashConfig = save.GetDashConfig();
        Debug.Log("Event Received " + myDashConfig.Length);
        Debug.Log("Event Received " + name + " : " + type);

        //save.UpdateDashConfig(index, name, type);
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
        Debug.Log("Event Received Hello World, is my item a clone? [" + item.IsAClone + "]");
        Destroy(item.DroppedObject);
    }
}
