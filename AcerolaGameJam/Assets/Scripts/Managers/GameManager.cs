using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using static UnityEditor.Progress;

[Serializable]
public class PlayerStats
{
    public int enemiesKilled = 0;
    /*
    public string Name = "Llama Lover";
    public float BaseAttackSpeed = 1;
    public int Level = 1;
    public bool HasUnlockedSomething;
    public bool HasUnlockedSomethingElse;

    public Dictionary<Slot, Equipment> Equipment = new Dictionary<Slot, Equipment>()
    {
        { Slot.Head, new Armor() {
            Name = "Tiara",
            EquippedSlot = Slot.Head,
            Rarity = Rarity.Common,
            Value = 5,
            Defense = 1
        }},
        { Slot.Chest, new Armor() {
            Name = "Rags",
            EquippedSlot = Slot.Chest,
            Rarity = Rarity.Common,
            Value = 5,
            Defense = 1
        } },
        { Slot.Legs, new Armor() {
            Name = "Rags",
            EquippedSlot = Slot.Legs,
            Rarity = Rarity.Common,
            Value = 5,
            Defense = 1
        } },
        { Slot.Feet, null },
        { Slot.Hands, null },
        { Slot.Neck, null },
        { Slot.Ammo, null },
        { Slot.WeaponLeft, null },
        { Slot.Shield, null },
        { Slot.TwoHandedWeapon, null },
        { Slot.Wrist, null }
    };

    public List<Item> Inventory = new List<Item>() {
        new Item() {
            Value = 1,
            IsConsumable = false,
            Name = "Gold Pieces",
            Rarity = Rarity.Common,
            Quantity = 10
        }
    };
    */
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Controls controls;
    public PlayerController playerController;
    public GameUIManager uiManager;
    public GameObject explosionPrefab;
    public PlayerStats playerStats;
    readonly IDataService dataService = new JsonDataService();
    readonly string path = "/player-stats.json";

    private void Awake()
    {
        instance = this;
        controls = new Controls();
        /*
        if(File.Exists(Application.persistentDataPath + path))
            playerStats = dataService.LoadData<PlayerStats>(path);
        else
        {
            playerStats = new();
            dataService.SaveData(path, playerStats);
        }
        */
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
        dataService.SaveData(path, playerStats);
    }

    public IEnumerator CreateExplosion(Vector3 position)
    {
        GameObject explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        yield return new WaitForSeconds(2);
        Destroy(explosion);
    }
}