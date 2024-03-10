using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUIManager : MonoBehaviour
{
    public GameObject startMenuObject;
    public GameObject statsMenuObject;
    public GameObject itemsMenuObject;

    public void PlayButton()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void StatsButton()
    {
        startMenuObject.SetActive(false);
        statsMenuObject.SetActive(true);
    }

    public void ItemsButton()
    {
        startMenuObject.SetActive(false);
        itemsMenuObject.SetActive(true);
    }

    public void ReturnButton()
    {
        startMenuObject.SetActive(true);
        statsMenuObject.SetActive(false);
        itemsMenuObject.SetActive(false);
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}