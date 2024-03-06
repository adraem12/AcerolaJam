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
    public GameObject[] wallObjects;
    float doorOffset = 3f;

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
        doorObject.GetComponent<Collider>().isTrigger = true;
    }

    public void CloseDoor()
    {
        doorObject.GetComponent<Collider>().isTrigger = false;
    }
}