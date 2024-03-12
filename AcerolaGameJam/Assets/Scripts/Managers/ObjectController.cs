using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    //public string objectName;
    //public string description;
    public int healthChange;
    public float damageChange;
    public float attackSpeedChange;
    public float rangeChange;
    public float moveSpeedChange;
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
            other.GetComponent<PlayerController>().TakeObject(healthChange, damageChange, attackSpeedChange, rangeChange, moveSpeedChange, gameObject, type);
            Destroy(gameObject);
        }
    }
}