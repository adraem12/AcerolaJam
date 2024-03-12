using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;

public class SceneHandler : MonoBehaviour
{
    public static SceneHandler instance;
    [SerializeField] RectTransform fader;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
            StartCoroutine(RetardLoadScene());
        else
        {
            fader.gameObject.SetActive(true);
            fader.transform.DOScale(new Vector3(1, 1, 1), 0);
            fader.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutQuad).OnComplete(() => { fader.gameObject.SetActive(false); });
        }
    }

    public void OpenMenuScene()
    {
        fader.gameObject.SetActive(true);
        fader.transform.DOScale(Vector3.zero, 0f);
        fader.transform.DOScale(new Vector3(1, 1, 1), 0.5f).SetEase(Ease.InOutQuad).OnComplete(() => { SceneManager.LoadScene(0, LoadSceneMode.Single); });
    }

    public void OpenGameScene()
    {
        GameManager.instance.playerStats.runsStarted++;
        GameManager.instance.SerializeJson();
        fader.gameObject.SetActive(true);
        fader.transform.DOScale(Vector3.zero, 0f);
        fader.transform.DOScale(new Vector3(1, 1, 1), 0.5f).SetEase(Ease.InOutQuad).OnComplete(() => { SceneManager.LoadScene(1, LoadSceneMode.Single); });
    }

    IEnumerator RetardLoadScene()
    {
        fader.gameObject.SetActive(true);
        fader.transform.DOScale(new Vector3(1, 1, 1), 0);
        yield return new WaitForSeconds(1.5f);
        fader.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutQuad).OnComplete(() => { fader.gameObject.SetActive(false); });
    }
}