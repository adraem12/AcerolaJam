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

    public void OpenDoor(bool open)
    {

    }
}