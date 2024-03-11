using System.Linq.Expressions;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public enum DoorType
    {
        top, right, down, left
    }
    public DoorType type;
    public GameObject doorObject;
    float doorOffset = 5f;
    Animator animator;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (type)
            {
                case DoorType.top:
                    GameManager.instance.playerController.transform.position = new Vector3(other.transform.position.x, other.transform.position.y, other.transform.position.z + doorOffset);
                    break;
                case DoorType.right:
                    GameManager.instance.playerController.transform.position = new Vector3(other.transform.position.x + doorOffset, other.transform.position.y, other.transform.position.z);
                    break;
                case DoorType.down:
                    GameManager.instance.playerController.transform.position = new Vector3(other.transform.position.x, other.transform.position.y, other.transform.position.z - doorOffset);
                    break;
                case DoorType.left:
                    GameManager.instance.playerController.transform.position = new Vector3(other.transform.position.x - doorOffset, other.transform.position.y, other.transform.position.z);
                    break;
            }
        }
    }

    public void OpenDoor()
    {
        animator.SetBool("openDoor", true);
        doorObject.GetComponent<Collider>().isTrigger = true;
    }

    public void CloseDoor()
    {
        animator.SetBool("openDoor", false);
        doorObject.GetComponent<Collider>().isTrigger = false;
    }
}