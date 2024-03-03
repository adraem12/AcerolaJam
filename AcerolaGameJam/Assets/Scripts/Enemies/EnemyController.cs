using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Wander,
    Follow,
    Die
}

public class EnemyController : MonoBehaviour
{
    PlayerController player;
    public EnemyState currentState = EnemyState.Wander;
    public float range;
    public float speed;
    bool chooseDir = false;
    bool dead = false;
    Vector3 randomDir;

    void Start()
    {
        player = GameManager.instance.playerController;
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Wander:
                Wander();
                break;
            case EnemyState.Follow:
                Follow();
                break;
            case EnemyState.Die:
                Die();
                break;
        }
        if (IsPlayerInRange() && currentState != EnemyState.Die)
            currentState = EnemyState.Follow;
        else if (!IsPlayerInRange() && currentState != EnemyState.Die)
            currentState = EnemyState.Wander;
    }

    bool IsPlayerInRange()
    {
        return Vector3.Distance(transform.position, player.transform.position) <= range;
    }

    IEnumerator ChooseDirection()
    {
        chooseDir = true;
        yield return new WaitForSeconds(Random.Range(2f, 8f));
        randomDir = new Vector3(0, 0, Random.Range(0, 360));
        Quaternion nextRotation = Quaternion.Euler(randomDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, nextRotation, Random.Range(0.5f, 2.5f));
        chooseDir = false;
    }

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
        /*
        Vector3 target = player.transform.position -  transform.position;
        float angle = Mathf.Atan2(target.x, target.z) * Mathf.Rad2Deg;
        transform.SetPositionAndRotation(speed * Time.deltaTime * -transform.right, Quaternion.AngleAxis(angle - 180, Vector3.forward));
        */
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }

    void Die()
    {
        Destroy(gameObject);
    }
}