using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject newItem = Instantiate(GameManager.instance.item[Random.Range(0, GameManager.instance.item.Length - 1)], transform);
        newItem.transform.localPosition = Vector3.zero + Vector3.up * 3;
    }
}