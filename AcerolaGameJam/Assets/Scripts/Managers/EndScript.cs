using UnityEngine;

public class EndScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.playerStats.runsEnded++;
            GameManager.instance.SerializeJson();
            SceneHandler.instance.OpenMenuScene();
        }
    }
}