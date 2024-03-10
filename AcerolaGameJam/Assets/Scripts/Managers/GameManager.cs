using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerStats
{

}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Controls controls;
    public PlayerController playerController;
    public GameUIManager uiManager;
    IDataService dataService = new JsonDataService();

    private void Awake()
    {
        instance = this;
        controls = new Controls();
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
        dataService.SaveData("/player-stats.json", /**/1/**/);
    }
}