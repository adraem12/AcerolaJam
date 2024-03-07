using System;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    static int health;
    [Min(1)]
    static int maxHealth = 4;
    [Min(0.1f)]
    static float damage = 1;
    [Range(0.15f, 4f)]
    static float attackSpeed = 1.5f;
    [Range(1f, 15f)]
    static float range = 4;
    [Range(0f, 2f)]
    static float movementSpeed = 1f;
    public int Health { get => health; set => health = value; }
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float Damage { get => damage; set => damage = value; }
    public float AttackSpeed { get => attackSpeed; set => attackSpeed = value; }
    public float Range { get => range; set => range = value; }
    public float MovementSpeed { get => movementSpeed; set => movementSpeed = value; }
    public static event EventHandler OnStatsChange;
    float lookSpeed = 50;
    Rigidbody rb;
    Vector3 moveDirection;
    Vector3 lookDirection = Vector3.forward;
    public PlayerWeapon playerWeapon;
    Transform weaponController;
    bool attacking = false;
    Cooldown cooldown;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        weaponController = playerWeapon.weaponController;
        cooldown = new Cooldown();
        cooldown.SetCooldown(attackSpeed);
        health = maxHealth;
        OnStatsChange?.Invoke(this, EventArgs.Empty);
        GameManager.instance.controls.PlayMap.Movement.performed += ReadMovementInput;
        GameManager.instance.controls.PlayMap.Movement.canceled += ReadMovementInput;
        GameManager.instance.controls.PlayMap.Look.performed += ReadLookInput;
        GameManager.instance.controls.PlayMap.Attack.performed += ReadAttackInput;
        GameManager.instance.controls.PlayMap.Attack.canceled += ReadAttackInput;
    }

    void ReadMovementInput(InputAction.CallbackContext context)
    {
        moveDirection.x = context.ReadValue<Vector2>().x;
        moveDirection.z = context.ReadValue<Vector2>().y;
        moveDirection *= 4f;
    }

    void ReadLookInput(InputAction.CallbackContext context)
    {
        lookDirection.x = context.ReadValue<Vector2>().x;
        lookDirection.z = context.ReadValue<Vector2>().y;
    }

    void ReadAttackInput(InputAction.CallbackContext context)
    {
        attacking = context.ReadValueAsButton();
    }

    private void Update()
    {
        rb.velocity = new Vector3(moveDirection.x * movementSpeed, 0, moveDirection.z * movementSpeed);
        Quaternion rot = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * lookSpeed);
        rot.x = 0;
        rot.z = 0;
        rb.rotation = rot;
        if (!cooldown.IsCoolingDown && attacking)
            Attack();
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

    void Attack()
    {
        playerWeapon.performingAttack = true;
        playerWeapon.canDamage = true;
        float attackSpeed1 = attackSpeed * 0.9f * 0.15f;
        float attackSpeed2 = attackSpeed * 0.9f * 0.85f;
        weaponController.DOLocalMove(Vector3.right * range, attackSpeed1).SetEase(Ease.InOutQuint).OnComplete(() => 
        {
            weaponController.DOLocalMove(Vector3.zero, attackSpeed2).SetEase(Ease.InOutCirc).OnComplete(() =>
            playerWeapon.performingAttack = false);
        });
        cooldown.StartCooldown();
    }

    void Die()
    {

    }

    public void TakeObject(int healthChange, float moveSpeedChange, float damageChange, float attackSpeedChange, float sizeChange, Texture2D image, ObjectController.ItemType type)
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
        damage += damageChange;
        attackSpeed -= attackSpeedChange;
        transform.localScale += Vector3.one * sizeChange;
        transform.position = new Vector3(transform.position.x, transform.localScale.y, transform.position.z);
        OnStatsChange?.Invoke(this, EventArgs.Empty);
    }
}