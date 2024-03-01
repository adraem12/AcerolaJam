using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed;
    Rigidbody rb;
    Vector3 inputDirection;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GameManager.instance.controls.PlayMap.Movement.performed += ReadInput;
        GameManager.instance.controls.PlayMap.Movement.canceled += ReadInput;
    }

    void ReadInput(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<Vector2>();
        inputDirection.x = input.x;
        inputDirection.z = input.y;
    }

    private void Update()
    {
        rb.velocity = new Vector3(inputDirection.x * speed, 0, inputDirection.z * speed);
    }
}