using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    void Start()
    {
        GameObject newItem = Instantiate(GameManager.instance.item[Random.Range(0, GameManager.instance.item.Length - 1)], transform);
        newItem.transform.localPosition = Vector3.zero + Vector3.up * 3;
        newItem.transform.localScale = Vector3.one * 3f;
    }
}