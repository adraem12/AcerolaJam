using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
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
    public GameObject mapTile;
    List<GameObject> mapTiles = new();
    GameObject playerPosition = null;
    float mapWidthOffset = 0;
    float mapHeightOffset = 0;

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
        healthText.text = "HLTH: " + GameManager.instance.playerController.Health + " / " + GameManager.instance.playerController.MaxHealth;
        damageText.text = "DMG: " + GameManager.instance.playerController.Damage.ToString("0.00");
        attackSpeedText.text = "ATK_SPD: " + GameManager.instance.playerController.AttackSpeed.ToString("0.00");
        rangeText.text = "RNG: " + GameManager.instance.playerController.Range.ToString("0.00");
        moveSpeedText.text = "MV_SPD: " + GameManager.instance.playerController.MovementSpeed.ToString("0.00");
    }

    public void CreateItemImage(GameObject model)
    {
        RectTransform item = Instantiate(model, itemsParent.transform).AddComponent<RectTransform>();
        item.localScale = Vector3.one * 100;
        item.rotation = Quaternion.Euler(0 , UnityEngine.Random.Range(0, 180), 0);
        item.GetComponentInChildren<SkinnedMeshRenderer>().gameObject.layer = LayerMask.NameToLayer("UI");
        item.gameObject.GetComponent<SphereCollider>().enabled = false;
    }

    public void DrawMap(List<RoomScript> rooms)
    {
        int xMax = rooms.Max(v => v.x);
        int xMin = rooms.Min(v => v.x);
        int zMax = rooms.Max(v => v.z);
        int zMin = rooms.Min(v => v.z);
        int mapWidth = (int)MathF.Abs(xMax) + (int)MathF.Abs(xMin) + 1;
        int mapHeight = (int)MathF.Abs(zMax) + (int)MathF.Abs(zMin) + 1;
        Vector2 tileSize = new(mapParent.sizeDelta.x / mapWidth, mapParent.sizeDelta.y / mapHeight);
        if(Mathf.Abs(xMax) > Mathf.Abs(xMin))
            mapWidthOffset = -(tileSize.x / 2) * (Mathf.Abs(xMax) - Mathf.Abs(xMin));
        else if (Mathf.Abs(xMax) < Mathf.Abs(xMin))
            mapWidthOffset = (tileSize.x / 2) * (Mathf.Abs(xMin) - Mathf.Abs(xMax));
        if (Mathf.Abs(zMax) > Mathf.Abs(zMin))
            mapHeightOffset = -(tileSize.y / 2) * (Mathf.Abs(zMax) - Mathf.Abs(zMin));
        else if(Mathf.Abs(zMax) < Mathf.Abs(zMin))
            mapHeightOffset = (tileSize.y / 2) * (Mathf.Abs(zMin) - Mathf.Abs(zMax));
        foreach (RoomScript room in rooms)
        {
            GameObject newMapTile = Instantiate(mapTile, mapParent);
            newMapTile.name = room.name;
            RawImage roomImage = newMapTile.GetComponent<RawImage>();
            roomImage.rectTransform.sizeDelta = tileSize;
            newMapTile.GetComponent<RectTransform>().SetLocalPositionAndRotation(new Vector3(room.x * roomImage.rectTransform.sizeDelta.x + mapWidthOffset, room.z * roomImage.rectTransform.sizeDelta.y + mapHeightOffset, 0), Quaternion.identity);
            if (room.name.Contains("End"))
                roomImage.color = Color.red;
            else if (room.name.Contains("Item"))
                roomImage.color = Color.green;
            if (room.x != 0 || room.z != 0)
                newMapTile.SetActive(false);
            mapTiles.Add(newMapTile);
        }
        playerPosition = Instantiate(mapTile, mapParent);
        playerPosition.name = "PlayerPosition";
        RawImage playerImage = playerPosition.GetComponent<RawImage>();
        playerImage.rectTransform.sizeDelta = tileSize;
        playerImage.color = Color.blue;
        playerPosition.GetComponent<RectTransform>().SetLocalPositionAndRotation(new Vector3(rooms.First().x * playerImage.rectTransform.sizeDelta.x + mapWidthOffset, rooms.First().z * playerImage.rectTransform.sizeDelta.y + mapHeightOffset, 0), Quaternion.identity);
    }

    public void ActivateTileMap(RoomScript room)
    {
        if(mapTiles.Count > 0)
        {
            GameObject currentMapTile = mapTiles.Where(obj => obj.name == room.name).SingleOrDefault();
            if (!currentMapTile.activeInHierarchy)
                currentMapTile.SetActive(true);
            playerPosition.GetComponent<RectTransform>().SetLocalPositionAndRotation(new Vector3(room.x * playerPosition.GetComponent<RawImage>().rectTransform.sizeDelta.x + mapWidthOffset, room.z * playerPosition.GetComponent<RawImage>().rectTransform.sizeDelta.y + mapHeightOffset, 0), Quaternion.identity);
        }
    }

    public IEnumerator UpdateColorText(string stat, Color color)
    {
        TextMeshProUGUI currentText = null;
        switch (stat)
        {
            case "Health":
                currentText = healthText;
                break;
            case "Damage":
                currentText = damageText;
                break;
            case "AttackSpeed":
                currentText = attackSpeedText;
                break;
            case "Range":
                currentText = rangeText;
                break;
            case "MoveSpeed":
                currentText = moveSpeedText;
                break;
        }
        if (currentText != null)
        {
            currentText.color = color;
            yield return new WaitForSeconds(0.25f);
            currentText.DOColor(Color.white, 0.5f);
        }
    }
}