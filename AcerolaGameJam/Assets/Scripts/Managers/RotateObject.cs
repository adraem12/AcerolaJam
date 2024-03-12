using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float speed = 100f;

    void Update()
    {
        transform.Rotate(new Vector3(0, Time.deltaTime * speed, 0));
    }
}
