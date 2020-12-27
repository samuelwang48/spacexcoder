using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class VehicleManager : MonoBehaviour
{
    public GameObject ObjExitStage;
    public float RotateSpeed = 1.1f;
    public GameObject PrefabItemTpl;
    public GameObject InventoryCellPrefab;
    public GameObject GameInventoryOverlay;

    public object MouseDisable { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Button btnExitStage = ObjExitStage.GetComponent<Button>();
        btnExitStage.onClick.AddListener(delegate { ExitStage(); });

        SpaceXCoder.Inventory.InitGameInventoryOverlay(
            PrefabItemTpl,
            InventoryCellPrefab,
            GameInventoryOverlay,
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
                GameObject current = li[j];
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
}
