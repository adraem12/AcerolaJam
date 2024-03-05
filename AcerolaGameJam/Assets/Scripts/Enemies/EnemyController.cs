using System.Collections;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Wander,
    Follow,
    Attack,
    Die
}

public enum EnemyType
{
    Contact,
    Melee,
    Ranged
}

public class EnemyController : MonoBehaviour
{
    [HideInInspector] public PlayerController player;
    EnemyState currentState = EnemyState.Idle;
    public EnemyType enemyType;
    public float health;
    public float followRange;
    public float attackRange;
    public float speed;
    public float attackCoolDown;
    bool chooseDir = false;
    public bool notInRoom = false;
    [HideInInspector] public bool coolingDown = false;
    Vector3 randomDir;
    public GameObject bulletPrefab;

    void Start()
    {
        player = GameManager.instance.playerController;
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Wander:
                Wander();
                break;
            case EnemyState.Follow:
                Follow();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Die:
                Die();
                break;
        }
        if (!notInRoom)
        {
            if (IsPlayerInRange() && currentState != EnemyState.Die)
                currentState = EnemyState.Follow;
            else if (!IsPlayerInRange() && currentState != EnemyState.Die)
                currentState = EnemyState.Wander;
            if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
                currentState = EnemyState.Attack;
            if (health <= 0)
                currentState = EnemyState.Die;
        }
        else
            currentState = EnemyState.Idle;
    }

    bool IsPlayerInRange()
    {
        return Vector3.Distance(transform.position, player.transform.position) <= followRange;
    }

    IEnumerator ChooseDirection()
    {
        chooseDir = true;
        yield return new WaitForSeconds(Random.Range(2f, 8f));
        randomDir = new Vector3(0, 0, Random.Range(0, 360));
        Quaternion nextRotation = Quaternion.Euler(randomDir);
        nextRotation.x = 0;
        nextRotation.z = 0;
        transform.rotation = Quaternion.Lerp(transform.rotation, nextRotation, Random.Range(0.5f, 2.5f));
        chooseDir = false;
    }


    void Idle() { }

    void Wander()
    {
        if (!chooseDir)
            StartCoroutine(ChooseDirection());
        transform.position += speed * Time.deltaTime * -transform.right;
        if (IsPlayerInRange())
            currentState = EnemyState.Follow;
    }

    void Follow()
    {
        LookAtPlayer();
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }

    void Attack() 
    {
        LookAtPlayer();
        if (!coolingDown)
            switch (enemyType)
            {
                case EnemyType.Contact:
                    player.GetDamage(1);
                    StartCoroutine(CoolDown());
                    break;
                case EnemyType.Melee:
                    break;
                case EnemyType.Ranged:
                    GameObject bullet = Instantiate(bulletPrefab, transform.position + transform.forward * 1.1f, Quaternion.identity);
                    bullet.GetComponent<BulletController>().SetBullet(player.transform);
                    StartCoroutine(CoolDown());
                    break;
            }
    }

    void LookAtPlayer()
    {
        Quaternion rot = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(player.transform.position - transform.position), Time.deltaTime * 100f);
        rot.x = 0;
        rot.z = 0;
        transform.rotation = rot;
    }

    IEnumerator CoolDown()
    {
        coolingDown = true;
        yield return new WaitForSeconds(attackCoolDown);
        coolingDown = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
    }

    void Die()
    {
        RoomController.instance.StartCoroutine(RoomController.instance.RoomCoroutine());
        Destroy(gameObject);
    }
}