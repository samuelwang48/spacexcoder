using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VehicleManager : MonoBehaviour
{
    public GameObject ObjExitStage;
    public float RotateSpeed = 1.1f;

    // Start is called before the first frame update
    void Start()
    {
        Button btnExitStage = ObjExitStage.GetComponent<Button>();
        btnExitStage.onClick.AddListener(delegate { ExitStage(); });

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
