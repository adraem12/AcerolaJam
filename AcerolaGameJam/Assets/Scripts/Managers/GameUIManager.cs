using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager instance;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI attackSpeedText;
    public TextMeshProUGUI rangeText;
    public TextMeshProUGUI moveSpeedText;
    public RectTransform itemsParent;
    public RectTransform mapParent;

    private void Awake()
    {
        instance = this;
    }

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

    public void DrawMap(List<RoomScript> rooms)
    {
        foreach (RoomScript room in rooms)
        {
            GameObject mapTile = Instantiate(new GameObject(), mapParent);
            RawImage roomImage = mapTile.AddComponent<RawImage>();
            roomImage.rectTransform.sizeDelta = new Vector2(30, 30);
            mapTile.GetComponent<RectTransform>().SetLocalPositionAndRotation(new Vector3(room.x * roomImage.rectTransform.sizeDelta.x, room.z * roomImage.rectTransform.sizeDelta.y, 0), Quaternion.identity);
            roomImage.color = Color.red;
        }
        /*
        string TileName = "MapTile";
        if (R.RoomNumber == 1) TileName = "BossRoomTile";
        if (R.RoomNumber == 2) TileName = "ShopRoomTile";
        if (R.RoomNumber == 3) TileName = "ItemRoomTile";
        GameObject MapTile = new GameObject(TileName);
        Image RoomImage = MapTile.AddComponent<Image>();
        RoomImage.sprite = R.RoomSprite;
        R.RoomImage = RoomImage;
        RectTransform rectTransform = RoomImage.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(Level.Height, Level.Width) * Level.IconScale;
        rectTransform.position = R.Location * (Level.IconScale * Level.Height * Level.Scale + (Level.padding * Level.Height * Level.Scale));
        RoomImage.transform.SetParent(transform, false);

        Level.Rooms.Add(R);
        Debug.Log("Drawing Room:" + R.RoomNumber + " at location:" + R.Location);
        */
    }
}