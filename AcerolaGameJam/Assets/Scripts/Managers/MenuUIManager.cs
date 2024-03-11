using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUIManager : MonoBehaviour
{
    public GameObject startMenuObject;
    public GameObject statsMenuObject;

    public void PlayButton()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void StatsButton()
    {
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