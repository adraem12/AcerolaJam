using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI moveSpeedText;
    public TextMeshProUGUI fireRateText;
    public RectTransform itemsParent;

    // Start is called before the first frame update
    void Start()
    {
        PlayerController.OnStatsChange += UpdateStats;
        UpdateStats(null, EventArgs.Empty);
    }

    void UpdateStats(object sender, EventArgs e)
    {
        healthText.text = "Health: " + GameManager.instance.playerController.Health + " / " + GameManager.instance.playerController.MaxHealth;
        moveSpeedText.text = "MoveSpeed: " + GameManager.instance.playerController.MovementSpeed.ToString("0.00");
        fireRateText.text = "MoveSpeed: " + GameManager.instance.playerController.AttackSpeed.ToString("0.00");
    }

    public void CreateItemImage(Texture2D image)
    {
        RawImage newItem = new GameObject().AddComponent<RawImage>();
        newItem.transform.SetParent(itemsParent);
        newItem.texture = image;
    }
}