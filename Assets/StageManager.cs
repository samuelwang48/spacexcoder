using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class StageManager : MonoBehaviour
{

    public GameObject Hop_0;
    public GameObject Hop_1;
    public GameObject Hop_2;
    public GameObject Hop_3;
    public GameObject Hop_4;
    public GameObject Hop_5;
    public GameObject Hop_6;
    public GameObject Hop_7;
	public GameObject BtnLeaderboard;
	public GameObject BtnAchievements;

    // Start is called before the first frame update
    void Start()
    {
        Social.localUser.Authenticate(success => {
            if (success)
            {
                Debug.Log("Authentication successful");
                string userInfo = "Username: " + Social.localUser.userName +
                    "\nUser ID: " + Social.localUser.id +
                    "\nIsUnderage: " + Social.localUser.underage;
                Debug.Log(userInfo);
            }
            else
                Debug.Log("Authentication failed");
        });

        Button btnHop_0 = Hop_0.GetComponent<Button>();
		btnHop_0.onClick.AddListener(delegate { LoadStage(0); });

		Button btnLeaderboard = BtnLeaderboard.GetComponent<Button>();
		btnLeaderboard.onClick.AddListener(delegate { Social.ShowLeaderboardUI(); });

		Button btnAchievements = BtnAchievements.GetComponent<Button>();
		btnAchievements.onClick.AddListener(delegate { Social.ShowAchievementsUI(); });
	}

    void LoadStage(int stage)
    {
        Debug.Log("About to start game stage: " + stage);
        PlayerPrefs.SetInt("stage", stage);
        SceneManager.LoadScene("Stage_" + stage);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
