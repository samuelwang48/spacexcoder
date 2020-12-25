using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VehicleManager : MonoBehaviour
{
    public GameObject ObjExitStage;

    // Start is called before the first frame update
    void Start()
    {
        Button btnExitStage = ObjExitStage.GetComponent<Button>();
        btnExitStage.onClick.AddListener(delegate { ExitStage(); });

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ExitStage()
    {
        SceneManager.LoadScene("Main Scene");
    }
}
