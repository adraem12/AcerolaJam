using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI attackSpeedText;
    public TextMeshProUGUI rangeText;
    public TextMeshProUGUI moveSpeedText;
    public RectTransform itemsParent;

    void Start()
    {
        PlayerController.OnStatsChange += UpdateStats;
        UpdateStats(null, EventArgs.Empty);
    }

    void UpdateStats(object sender, EventArgs e)
    {
        healthText.text = "Health: " + GameManager.instance.playerController.Health + " / " + GameManager.instance.playerController.MaxHealth;
        damageText.text = "Damage: " + GameManager.instance.playerController.Damage.ToString("0.00");
        attackSpeedText.text = "AttackSpeed: " + GameManager.instance.playerController.AttackSpeed.ToString("0.00");
        rangeText.text = "Range: " + GameManager.instance.playerController.Range.ToString("0.00");
        moveSpeedText.text = "MoveSpeed: " + GameManager.instance.playerController.MovementSpeed.ToString("0.00");
    }

    public void CreateItemImage(GameObject model)
    {
        RectTransform item = Instantiate(model, itemsParent.transform).AddComponent<RectTransform>();
        item.localScale = Vector3.one * 50;
        item.gameObject.layer = LayerMask.NameToLayer("UI");
    }
}