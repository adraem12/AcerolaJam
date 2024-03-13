using System;
using System.IO;
using System.Collections;
using UnityEngine;

[Serializable]
public class PlayerStats
{
    public int runsStarted = 0;
    public int runsEnded = 0;
    public int deaths = 0;
    public int enemiesKilled = 0;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Controls controls;
    public PlayerController playerController;
    public GameUIManager uiManager;
    public GameObject explosionPrefab;
    public PlayerStats playerStats;
    readonly IDataService dataService = new JsonDataService();
    readonly string path = "/player-stats.json";
    public GameObject[] item;

    private void Awake()
    {
        instance = this;
        controls = new Controls();
        if(File.Exists(Application.persistentDataPath + path))
            playerStats = dataService.LoadData<PlayerStats>(path);
        else
        {
            playerStats = new();
            dataService.SaveData(path, playerStats);
        }
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    public void SerializeJson()
    {
        dataService.SaveData(path, playerStats);
    }

    public IEnumerator CreateExplosion(Vector3 position)
    {
        GameObject explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        yield return new WaitForSeconds(2);
        Destroy(explosion);
    }
}