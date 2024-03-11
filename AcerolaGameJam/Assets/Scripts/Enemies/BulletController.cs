using System.Collections;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float lifeTime;
    Vector3 lastPosition;
    Vector3 currentPosition;
    Vector3 playerPosition;

    void Start()
    {
        StartCoroutine(DeathDelay());
    }

    private void Update()
    {
        currentPosition = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, playerPosition, 4f * Time.deltaTime);
        if (currentPosition == lastPosition)
            Destroy(gameObject);
        lastPosition = currentPosition;
    }

    public void SetBullet(Transform playerTransform)
    {
        playerPosition = playerTransform.position;
    }

    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(lifeTime);
        GameManager.instance.StartCoroutine(GameManager.instance.CreateExplosion(transform.position));
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().GetDamage(1);
            Destroy(gameObject);
        }
    }
}