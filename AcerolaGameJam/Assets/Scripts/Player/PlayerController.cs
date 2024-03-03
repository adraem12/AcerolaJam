using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed;
    public float lookSpeed;
    Rigidbody rb;
    Vector3 moveDirection;
    Vector3 lookDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GameManager.instance.controls.PlayMap.Movement.performed += ReadMovementInput;
        GameManager.instance.controls.PlayMap.Movement.canceled += ReadMovementInput;
        GameManager.instance.controls.PlayMap.Look.performed += ReadLookInput;
    }

    void ReadMovementInput(InputAction.CallbackContext context)
    {
        var moveInput = context.ReadValue<Vector2>();
        moveDirection.x = moveInput.x;
        moveDirection.z = moveInput.y;
    }

    void ReadLookInput(InputAction.CallbackContext context)
    {
        var lookInput = context.ReadValue<Vector2>();
        lookDirection.x = lookInput.x;
        lookDirection.z = lookInput.y;
    }

    private void Update()
    {
        rb.velocity = new Vector3(moveDirection.x * movementSpeed, 0, moveDirection.z * movementSpeed);
        Quaternion rot = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * lookSpeed);
        rot.x = 0;
        rot.z = 0;
        rb.rotation = rot;
    }
}