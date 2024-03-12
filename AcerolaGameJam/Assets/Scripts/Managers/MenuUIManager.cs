using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUIManager : MonoBehaviour
{
    public GameObject startMenuObject;
    public GameObject statsMenuObject;
    public TextMeshProUGUI runsStartedText;
    public TextMeshProUGUI runsEndedText;
    public TextMeshProUGUI deathsText;
    public TextMeshProUGUI enemiesKilledText;

    public void PlayButton()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void StatsButton()
    {
        runsStartedText.text = "RUNS STARTED: " + GameManager.instance.playerStats.runsStarted;
        runsEndedText.text = "RUNS ENDED: " + GameManager.instance.playerStats.runsEnded;
        deathsText.text = "DEATHS: " + GameManager.instance.playerStats.deaths;
        enemiesKilledText.text = "ENEMIES KILLED: " + GameManager.instance.playerStats.enemiesKilled;
        startMenuObject.SetActive(false);
        statsMenuObject.SetActive(true);
    }

    public void ReturnButton()
    {
        startMenuObject.SetActive(true);
        statsMenuObject.SetActive(false);
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}