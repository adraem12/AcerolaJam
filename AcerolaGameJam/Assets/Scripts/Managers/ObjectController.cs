using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public string objectName;
    public string description;
    public int healthChange;
    public float moveSpeedChange;
    public float damageChange;
    public float attackRateChange;
    public float attackSpeedChange;
    public enum ItemType
    {
        Consumable,
        Item
    }
    public ItemType type;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().TakeObject(healthChange, moveSpeedChange, damageChange, attackSpeedChange, gameObject, type);
            Destroy(gameObject);
        }
    }
}