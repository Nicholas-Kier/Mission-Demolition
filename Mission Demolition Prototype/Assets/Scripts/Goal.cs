using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private readonly int level;
    static public bool goalMet = false;

    public AudioSource objectiveComplete;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("playerMax"))
        {
            LoadPlayerLevel();
        }
        SavePlayerLevel(level);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            Goal.goalMet = true;
            Material mat = GetComponent<Renderer>().material;
            mat.SetColor("_Color", Color.green);

            SavePlayerLevel(level + 1);

            objectiveComplete.Play();
        }
    }

    int LoadPlayerLevel()
    {
        return PlayerPrefs.GetInt("playerMaxLevel");
    }

    void SavePlayerLevel(int newScore)
    {
        if (LoadPlayerLevel() < newScore)
        {
            PlayerPrefs.SetInt("playerMaxLevel", newScore);
            PlayerPrefs.Save();
        }
    }
}
