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
    Rigidbody rb;
    Vector3 moveDirection;
    Vector3 lookDirection = Vector3.forward;
    public Vector3 LookDirection { get => lookDirection; }
    [SerializeField] PlayerWeapon playerWeapon;
    Transform weaponController;
    bool attacking = false;
    Cooldown cooldown;
    [SerializeField] GameObject[] tentacleControler;
    Vector3[] tentacleOriginalPos;
    [SerializeField] float frequency = 1;
    [SerializeField] float magnitude = 1;
    [SerializeField] AudioSource walkAudioSource;
    [SerializeField] AudioSource attackAudioSource;
    [SerializeField] AudioSource hitAudioSource;
    float maxSpeed;
    bool invencible = false;
    bool dead = false;

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
        if (GameManager.instance.controls.PlayMap.EndGame.ReadValue<float>() > 0.1f && !dead)
            Die();
        rb.velocity = new Vector3(moveDirection.x * movementSpeed, 0, moveDirection.z * movementSpeed);
        Quaternion rot = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 50f);
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
            GameUIManager.instance.StartCoroutine(GameUIManager.instance.UpdateColorText("Health", Color.red));
            OnStatsChange?.Invoke(this, EventArgs.Empty);
            if (health <= 0)
                Die();
        }
    }

    public void GetHeal(int heal)
    {
        health = Mathf.Min(maxHealth, health + heal);
        GameUIManager.instance.StartCoroutine(GameUIManager.instance.UpdateColorText("Health", Color.green));
        OnStatsChange?.Invoke(this, EventArgs.Empty);
    }

    void Attack()
    {
        playerWeapon.performingAttack = true;
        float attackSpeed1 = attackSpeed * 0.9f * 0.15f;
        float attackSpeed2 = attackSpeed * 0.9f * 0.85f;
        attackAudioSource.Play();
        weaponController.DOLocalMove(Vector3.forward * range, attackSpeed1).SetEase(Ease.InOutQuint).OnComplete(() => 
        {
            weaponController.DOLocalMove(Vector3.forward * 0.01f, attackSpeed2).SetEase(Ease.InOutCirc).OnComplete(() =>
            playerWeapon.performingAttack = false);
        });
        cooldown.StartCooldown();
    }

    IEnumerator DamageBlink()
    {
        hitAudioSource.Play();
        Material mat = GetComponentInChildren<MeshRenderer>().material;
        mat.SetFloat("_LightingCutoff", -1);
        yield return new WaitForSeconds(0.5f);
        invencible = false;
        mat.SetFloat("_LightingCutoff", 0.7f);
    }

    void MoveTentacles()
    {
        float currentSpeed = Mathf.InverseLerp(0, maxSpeed, rb.velocity.magnitude);
        walkAudioSource.volume = currentSpeed;
        for (int i = 0; i < tentacleControler.Length; i++)
            tentacleControler[i].transform.position = tentacleOriginalPos[i] + transform.position + (magnitude * Mathf.Sin(Time.time * frequency + i * 180) * tentacleControler[i].transform.forward + magnitude * Mathf.Sin(Time.time * frequency + i * 50) * tentacleControler[i].transform.right - (tentacleControler[i].transform.position - transform.position) * 0.2f) * currentSpeed;
    }

    void Die()
    {
        dead = true;
        GameManager.instance.playerStats.deaths++;
        GameManager.instance.SerializeJson();
        SceneHandler.instance.OpenMenuScene();
    }

    public void TakeObject(int healthChange, float damageChange, float attackSpeedChange, float rangeChange, float moveSpeedChange, GameObject model, ObjectController.ItemType type)
    {
        if (type == ObjectController.ItemType.Consumable)
            health += healthChange;
        else if (type == ObjectController.ItemType.Item)
        {
            maxHealth += healthChange;
            health += healthChange;
            GameManager.instance.uiManager.CreateItemImage(model);
        }
        damage += damageChange;
        attackSpeed -= attackSpeedChange;
        range += rangeChange;
        movementSpeed += moveSpeedChange;
        maxSpeed = new Vector3(movementSpeed, 0, movementSpeed).magnitude;
        if (healthChange > 0)
            GameUIManager.instance.StartCoroutine(GameUIManager.instance.UpdateColorText("Health", Color.green));
        else if (healthChange < 0)
            GameUIManager.instance.StartCoroutine(GameUIManager.instance.UpdateColorText("Health", Color.red));
        if (damageChange > 0)
            GameUIManager.instance.StartCoroutine(GameUIManager.instance.UpdateColorText("Damge", Color.green));
        else if (damageChange < 0)
            GameUIManager.instance.StartCoroutine(GameUIManager.instance.UpdateColorText("Damge", Color.red));
        if (attackSpeedChange > 0)
            GameUIManager.instance.StartCoroutine(GameUIManager.instance.UpdateColorText("AttackSpeed", Color.green));
        else if (attackSpeedChange < 0)
            GameUIManager.instance.StartCoroutine(GameUIManager.instance.UpdateColorText("AttackSpeed", Color.red));
        if (rangeChange > 0)
            GameUIManager.instance.StartCoroutine(GameUIManager.instance.UpdateColorText("Range", Color.green));
        else if (rangeChange < 0)
            GameUIManager.instance.StartCoroutine(GameUIManager.instance.UpdateColorText("Range", Color.red));
        if (moveSpeedChange > 0)
            GameUIManager.instance.StartCoroutine(GameUIManager.instance.UpdateColorText("MoveSpeed", Color.green));
        else if (moveSpeedChange < 0)
            GameUIManager.instance.StartCoroutine(GameUIManager.instance.UpdateColorText("MoveSpeed", Color.red));
        OnStatsChange?.Invoke(this, EventArgs.Empty);
    }
}