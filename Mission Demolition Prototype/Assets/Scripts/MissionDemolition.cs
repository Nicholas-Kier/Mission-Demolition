using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameMode
{
    idle,
    playing,
    levelEnd
}

public class MissionDemolition : MonoBehaviour
{
    static public MissionDemolition S;

    [Header("Set in Inspector")]
    public Text uitLevel;

    public Text uitShots;
    public Text uitHighScore;
    public Text uitButton;
    public Text gameOver;
    public Vector3 castlePos;
    public GameObject[] castles;

    [Header("Set Dynamically")]
    public static int level;

    public int levelMax;
    public int shotsTaken;
    public int remainingShots;
    public int maxShots = 5;

    public GameObject castle;

    public GameMode mode = GameMode.idle;
    public string showing = "Show Slingshot";

    public void Start()
    {
        S = this;
        level = 1;
        levelMax = castles.Length;
        StartLevel();
        remainingShots = maxShots;
    }

    private void StartLevel()
    {
        if (castle != null)
        {
            Destroy(castle);
        }

        GameObject[] gos = GameObject.FindGameObjectsWithTag("Projectile");

        foreach (GameObject pTemp in gos)
        {
            Destroy(pTemp);
        }

        castle = Instantiate<GameObject>(castles[(level - 1)]);
        castle.transform.position = castlePos;
        shotsTaken = 0;

        SwitchView("Show Both");

        ProjectileLine.S.Clear();

        Goal.goalMet = false;

        UpdateGUI();

        mode = GameMode.playing;
    }

    private void UpdateGUI()
    {
        uitLevel.text = "Level: " + (level) + " of " + levelMax;
        uitShots.text = "Shots Taken: " + shotsTaken + " of 5";
        uitHighScore.text = "Highest Score: " + level;
    }

    private void DrawGUI()
    {
        Rect buttonRectangle = new Rect(Screen.width / 2 - 50, 10, 100, 24);

        switch (showing)
        {
            case "Slingshot":
                if (GUI.Button(buttonRectangle, "Show Castle"))
                {
                    SwitchView("Castle");
                    if (S.shotsTaken == 0)
                    {
                        GameOver();
                        Invoke("StartLevel", 5f);
                    }
                }
                break;

            case "Castle":
                if (GUI.Button(buttonRectangle, "Show Both"))
                {
                    SwitchView("Both");
                    if (S.shotsTaken == 0)
                    {
                        GameOver();
                        Invoke("StartLevel", 5f);
                    }
                }
                break;

            case "Both":
                if (GUI.Button(buttonRectangle, "Show Slingshot"))
                {
                    SwitchView("Slingshot");
                    if (S.shotsTaken == 0)
                    {
                        GameOver();
                        Invoke("StartLevel", 5f);
                    }
                }
                break;
        }
    }

    private void Update()
    {
        UpdateGUI();

        if ((mode == GameMode.playing) && Goal.goalMet)
        {
            mode = GameMode.levelEnd;
            SwitchView("Show Both");
            Invoke("NextLevel", 2f);
        }

        if (mode == GameMode.playing)
        {
            if (shotsTaken >= 5)
            {
                SceneManager.LoadScene("GameOver");
            }
        }
    }

    private void NextLevel()
    {
        ScoreChecker(remainingShots);
        level++;
        if (level > 4)
        {
            SceneManager.LoadScene("Victory");
        }
        StartLevel();
    }

    public void SwitchView(string eView = "")
    {
        if (eView == "")
        {
            eView = uitButton.text;
        }
        showing = eView;

        switch (showing)
        {
            case "Show Slingshot":
                FollowCam.POI = null;
                uitButton.text = "Show Castle";
                break;

            case "Show Castle":
                FollowCam.POI = S.castle;
                uitButton.text = "Show Both";
                break;

            case "Show Both":
                FollowCam.POI = GameObject.Find("ViewBoth");
                uitButton.text = "Show Slingshot";
                break;
        }
    }

    public static void GameOver()
    {

    }

    public static void ShotFired()
    {
        S.shotsTaken++;
    }

    public static void ScoreChecker(int leftoverShots)
    {
        switch (S.levelMax)
        {
            case 0:
                print("Remaining Shots: " + leftoverShots);
                if (leftoverShots > PlayerPrefs.GetInt("LevelOneHighScore"))
                {
                    PlayerPrefs.SetInt("LevelOneHighScore", 3 - leftoverShots);
                }
                break;

            case 1:
                print("Remaining Shots: " + leftoverShots);
                if (leftoverShots > PlayerPrefs.GetInt("LevelTwoHighScore"))
                {
                    PlayerPrefs.SetInt("LevelTwoHighScore", 3 - leftoverShots);
                }
                break;

            case 2:
                print("Remaining Shots: " + leftoverShots);
                if (leftoverShots > PlayerPrefs.GetInt("LevelThreeHighScore"))
                {
                    PlayerPrefs.SetInt("LevelThreeHighScore", 3 - leftoverShots);
                }
                break;

            default:
                break;
        }
    }
}