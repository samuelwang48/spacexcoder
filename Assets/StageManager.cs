using System.Collections;
using System.Collections.Generic;
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


    // Start is called before the first frame update
    void Start()
    {
        KTGameCenter.SharedCenter().Authenticate();

        Button btnHop_0 = Hop_0.GetComponent<Button>();
        btnHop_0.onClick.AddListener(delegate { LoadStage(0); });

    }

    void LoadStage(int stage)
    {
        Debug.Log("About to start game stage: " + stage);
        PlayerPrefs.SetInt("stage", stage);
        SceneManager.LoadScene("Stage_0");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void OnEnable()
	{
		StartCoroutine(RegisterForGameCenter());
	}
	void OnDisable()
	{
		KTGameCenter.SharedCenter().GCUserAuthenticated -= GCAuthentication;
		KTGameCenter.SharedCenter().GCScoreSubmitted -= ScoreSubmitted;
		KTGameCenter.SharedCenter().GCAchievementSubmitted -= AchievementSubmitted;
		KTGameCenter.SharedCenter().GCAchievementsReset -= AchivementsReset;
		KTGameCenter.SharedCenter().GCMyScoreFetched -= MyScoreFetched;
	}

	IEnumerator RegisterForGameCenter()
	{
		yield return new WaitForSeconds(0.5f);
		KTGameCenter.SharedCenter().GCUserAuthenticated += GCAuthentication;
		KTGameCenter.SharedCenter().GCScoreSubmitted += ScoreSubmitted;
		KTGameCenter.SharedCenter().GCAchievementSubmitted += AchievementSubmitted;
		KTGameCenter.SharedCenter().GCAchievementsReset += AchivementsReset;
		KTGameCenter.SharedCenter().GCMyScoreFetched += MyScoreFetched;
	}

	void OnGUI()
	{
		if (!KTGameCenter.SharedCenter().IsGameCenterAuthenticated())
		{
			GUI.skin.label.fontSize = 20;
			GUI.Label(new Rect(10, 150, 200, 50), "Authenticating!");
			Debug.Log("Authenticating...");
		}
		else
		{
			Debug.Log("Authenticated!");
			GUI.skin.button.fontSize = 20;
			if (GUI.Button(new Rect(10, 150, 300, 60), "Show Leaderboards"))
			{
				KTGameCenter.SharedCenter().ShowLeaderboard();
			}
			if (GUI.Button(new Rect(10, 250, 250, 60), "Submit Score"))
			{
				KTGameCenter.SharedCenter().SubmitScore(110, "spacexcoder.uat.toptesters");
			}
			if (GUI.Button(new Rect(300, 250, 250, 60), "Submit Achievement"))
			{
				KTGameCenter.SharedCenter().SubmitAchievement(10, "spacexcoder.uat.unlock10", true);
			}
			if (GUI.Button(new Rect(10, 350, 300, 60), "Reset Achievement"))
			{
				KTGameCenter.SharedCenter().ResetAchievements();
			}
			if (GUI.Button(new Rect(330, 350, 250, 60), "Fetch my Score"))
			{
				KTGameCenter.SharedCenter().FetchMyScore("spacexcoder.uat.toptesters");
			}
			/*
			if (GUI.Button(new Rect(330, 350, 250, 60), "Submit Float Score"))
			{
				KTGameCenter.SharedCenter().SubmitFloatScore(110.123f, 3, "spacexcoder.uat.toptesters.float");
			}
			if (GUI.Button(new Rect(10, 450, 250, 60), "Submit Time"))
			{
				KTGameCenter.SharedCenter().SubmitFloatScore(2459.3f, 2, "spacexcoder.uat.toptesters.time");
			}
            */
		}
	}

	void GCAuthentication(string status)
	{
		print("delegate call back status= " + status);
		StartCoroutine(CheckAttributes());
	}
	void ScoreSubmitted(string leaderboardId, string error)
	{
		print("score submitted with id " + leaderboardId + " and error= " + error);
	}
	void AchievementSubmitted(string achId, string error)
	{
		print("achievement submitted with id " + achId + " and error= " + error);
	}
	void AchivementsReset(string error)
	{
		print("Achievment reset with error= " + error);
	}

	void MyScoreFetched(string leaderboardId, int score, string error)
	{
		print("My score for leaderboardId= " + leaderboardId + " is " + score + " with error= " + error);
	}

	IEnumerator CheckAttributes()
	{
		yield return new WaitForSeconds(1.0f);
		print(" alias= " + KTGameCenter.SharedCenter().PlayerAlias + " name= " +
			   KTGameCenter.SharedCenter().PlayerName + " id= " + KTGameCenter.SharedCenter().PlayerId);
	}
}
