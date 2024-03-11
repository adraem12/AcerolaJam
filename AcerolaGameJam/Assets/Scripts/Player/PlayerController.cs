using System;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Linq;
using UnityEngine.Rendering;
using System.Collections;

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
    public Vector3 LookDirection { get => lookDirection; }
    public PlayerWeapon playerWeapon;
    Transform weaponController;
    bool attacking = false;
    Cooldown cooldown;
    [SerializeField] GameObject[] tentacleControler;
    Vector3[] tentacleOriginalPos;
    [SerializeField] float frequency = 1;
    [SerializeField] float magnitude = 1;
    float maxSpeed;
    bool invencible = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        weaponController = playerWeapon.weaponController;
        cooldown = new Cooldown();
        cooldown.SetCooldown(attackSpeed);
        health = maxHealth;
        maxSpeed = new Vector3(movementSpeed, 0, movementSpeed).magnitude;
        OnStatsChange?.Invoke(this, EventArgs.Empty);
        GameManager.instance.controls.PlayMap.Movement.performed += ReadMovementInput;
        GameManager.instance.controls.PlayMap.Movement.canceled += ReadMovementInput;
        GameManager.instance.controls.PlayMap.Look.performed += ReadLookInput;
        GameManager.instance.controls.PlayMap.Attack.performed += ReadAttackInput;
        GameManager.instance.controls.PlayMap.Attack.canceled += ReadAttackInput;
        tentacleOriginalPos = new Vector3[tentacleControler.Length];
        for (int i = 0; i < tentacleControler.Length; i++)
            tentacleOriginalPos[i] = tentacleControler[i].transform.localPosition;
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
        MoveTentacles();
        if (!cooldown.IsCoolingDown && attacking)
            Attack();
    }

    public void GetDamage(int damage)
    {
        if (!invencible)
        {
            invencible = true;
            StartCoroutine(DamageBlink());
            health -= damage;
            OnStatsChange?.Invoke(this, EventArgs.Empty);
            if (health <= 0)
                Die();
        }
    }

    public void GetHeal(int heal)
    {
        health = Mathf.Min(maxHealth, health + heal);
        OnStatsChange?.Invoke(this, EventArgs.Empty);
    }

    void Attack()
    {
        playerWeapon.performingAttack = true;
        float attackSpeed1 = attackSpeed * 0.9f * 0.15f;
        float attackSpeed2 = attackSpeed * 0.9f * 0.85f;
        weaponController.DOLocalMove(Vector3.forward * range, attackSpeed1).SetEase(Ease.InOutQuint).OnComplete(() => 
        {
            weaponController.DOLocalMove(Vector3.forward * 0.01f, attackSpeed2).SetEase(Ease.InOutCirc).OnComplete(() =>
            playerWeapon.performingAttack = false);
        });
        cooldown.StartCooldown();
    }

    IEnumerator DamageBlink()
    {  
        Material mat = GetComponentInChildren<MeshRenderer>().material;
        mat.SetFloat("_LightingCutoff", -1);
        yield return new WaitForSeconds(0.5f);
        invencible = false;
        mat.SetFloat("_LightingCutoff", 0.7f);
    }

    void MoveTentacles()
    {
        for (int i = 0; i < tentacleControler.Length; i++)
            tentacleControler[i].transform.position = tentacleOriginalPos[i] + transform.position + (magnitude * Mathf.Sin(Time.time * frequency + i * 180) * tentacleControler[i].transform.forward + magnitude * Mathf.Sin(Time.time * frequency + i * 50) * tentacleControler[i].transform.right - (tentacleControler[i].transform.position - transform.position) * 0.2f) * Mathf.InverseLerp(0, maxSpeed, rb.velocity.magnitude);
    }

    void Die()
    {

    }

    public void TakeObject(int healthChange, float moveSpeedChange, float damageChange, float attackSpeedChange, GameObject model, ObjectController.ItemType type)
    {
        if (type == ObjectController.ItemType.Consumable)
            health += healthChange;
        else if (type == ObjectController.ItemType.Item)
        {
            maxHealth += healthChange;
            health += healthChange;
            GameManager.instance.uiManager.CreateItemImage(model);
        }
        movementSpeed += moveSpeedChange;
        maxSpeed = new Vector3(movementSpeed, 0, movementSpeed).magnitude;
        damage += damageChange;
        attackSpeed -= attackSpeedChange;
        transform.position = new Vector3(transform.position.x, transform.localScale.y, transform.position.z);
        OnStatsChange?.Invoke(this, EventArgs.Empty);
    }
}