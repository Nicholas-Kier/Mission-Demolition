﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public Text uitButton;
    public Text gameOver;
    public Vector3 castlePos;
    public GameObject[] castles;

    [Header("Set Dynamically")]

    public int level;
    public int levelMax;
    public int remainingShots;
    public GameObject castle;
    public GameMode mode = GameMode.idle;
    public string showing = "Show Slingshot";

    private void Start()
    {
        S = this;
        level = 0;
        levelMax = castles.Length;
        StartLevel();
    }
    private void StartLevel()
    {
        S.gameOver.text = "";

        if(castle != null)
        {
            Destroy(castle);
        }

        GameObject[] gos = GameObject.FindGameObjectsWithTag("Projectile");
        foreach(GameObject pTemp in gos)
        {
            Destroy(pTemp);
        }

        castle = Instantiate<GameObject>(castles[level]);
        castle.transform.position = castlePos;
        remainingShots = 3;

        SwitchView("Show Both");
        ProjectileLine.S.Clear();

        Goal.goalMet = false;

        UpdateGUI();

        mode = GameMode.playing;
    }
    void UpdateGUI()
    {
        uitLevel.text = "Level: " + (level + 1) + " of " + levelMax;
        uitShots.text = "Shots Taken: " + remainingShots;
        int highScore = -1;
    }

    void drawGUI()
    {
        Rect buttonRectangle = new Rect(Screen.width / 2 - 50, 10, 100, 24);

        switch(showing)
        {
            case "Slingshot":
                if(GUI.Button(buttonRectangle, "Show Castle"))
                {
                    SwitchView("Castle");
                    if(S.remainingShots == 0)
                    {
                        GameOver();
                        Invoke("StartLevel", 5f);
                    }
                }
                break;
            case "Castle":
                if(GUI.Button(buttonRectangle, "Show Both"))
                {
                    SwitchView("Both");
                    if(S.remainingShots == 0)
                    {
                        GameOver();
                        Invoke("StartLevel", 5f);
                    }
                }
                break;
            case "Both":
                if(GUI.Button(buttonRectangle, "Show Slingshot"))
                {
                    SwitchView("Slingshot");
                    if(S.remainingShots == 0)
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

        if((mode == GameMode.playing) && Goal.goalMet)
        {
            mode = GameMode.levelEnd;
            SwitchView("Show Both");
            Invoke("NextLevel", 2f);
        }
    }
    private void NextLevel()
    {
        ScoreChecker(remainingShots);
        level++;
        if(level == levelMax)
        {
            level = 0;
        }
        StartLevel();
    }
    public void SwitchView(string eView = "")
    {
        if(eView == "")
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
    public static void ShotFired()
    {
        S.remainingShots--;
    }
    public static void GameOver()
    {
        S.gameOver.text = "YOU DIED";
    }
    public static void ScoreChecker(int leftoverShots)
    {
        switch (S.level)
        {
            case 0:
                print("Remaining Shots: " + leftoverShots);
                if(leftoverShots > PlayerPrefs.GetInt("LevelOneHighScore"))
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
        }
    }
}
