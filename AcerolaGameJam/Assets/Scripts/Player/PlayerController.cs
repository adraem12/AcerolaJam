using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    static int health;
    static int maxHealth = 4;
    static float damage = 1;
    static float attackRate = 0.5f;
    static float attackSpeed = 0.5f;
    static float range = 1;
    static float movementSpeed = 4f;
    public int Health { get => health; set => health = value; }
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float Damage { get => damage; set => damage = value; }
    public float AttackRate { get => attackRate; set => attackRate = value; }
    public float AttackSpeed { get => attackSpeed; set => attackSpeed = value; }
    public float Range { get => range; set => range = value; }
    public float MovementSpeed { get => movementSpeed; set => movementSpeed = value; }
    public static event EventHandler OnStatsChange;
    float lookSpeed = 50;
    Rigidbody rb;
    Vector3 moveDirection;
    Vector3 lookDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        health = maxHealth;
        OnStatsChange?.Invoke(this, EventArgs.Empty);
        GameManager.instance.controls.PlayMap.Movement.performed += ReadMovementInput;
        GameManager.instance.controls.PlayMap.Movement.canceled += ReadMovementInput;
        GameManager.instance.controls.PlayMap.Look.performed += ReadLookInput;
    }

    void ReadMovementInput(InputAction.CallbackContext context)
    {
        moveDirection.x = context.ReadValue<Vector2>().x;
        moveDirection.z = context.ReadValue<Vector2>().y;
    }

    void ReadLookInput(InputAction.CallbackContext context)
    {
        lookDirection.x = context.ReadValue<Vector2>().x;
        lookDirection.z = context.ReadValue<Vector2>().y;
    }

    private void Update()
    {
        rb.velocity = new Vector3(moveDirection.x * movementSpeed, 0, moveDirection.z * movementSpeed);
        Quaternion rot = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * lookSpeed);
        rot.x = 0;
        rot.z = 0;
        rb.rotation = rot;
    }

    public void GetDamage(int damage)
    {
        health -= damage;
        OnStatsChange?.Invoke(this, EventArgs.Empty);
        if (health <= 0)
            Die();
    }

    public void GetHeal(int heal)
    {
        health = Mathf.Min(maxHealth, health + heal);
        OnStatsChange?.Invoke(this, EventArgs.Empty);
    }

    void Die()
    {

    }

    public void TakeObject(int healthChange, float moveSpeedChange, float damageChange, float attackRateChange, float attackSpeedChange, float sizeChange, Texture2D image, ObjectController.ItemType type)
    {
        if (type == ObjectController.ItemType.Consumable)
            health += healthChange;
        else if (type == ObjectController.ItemType.Item)
        {
            maxHealth += healthChange;
            health += healthChange;
            GameManager.instance.uiManager.CreateItemImage(image);
        }
        movementSpeed += moveSpeedChange;
        damage += damage;
        attackRate += attackRateChange;
        attackSpeed += attackSpeedChange;
        transform.localScale += Vector3.one * sizeChange;
        OnStatsChange?.Invoke(this, EventArgs.Empty);
    }
}