using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    public int width;
    public int length;
    public int x;
    public int z;
    bool updatedDoors = false;
    public DoorScript topDoor;
    public DoorScript rightDoor;
    public DoorScript downDoor;
    public DoorScript leftDoor;
    public List<DoorScript> doors = new();

    void Start()
    {
        if (RoomController.instance == null)
        {
            Debug.Log("You pressed play in the wrong scene");
            return;
        }
        RoomController.instance.RegisterRoom(this);
    }

    private void Update()
    {
        if(name.Contains("End") && !updatedDoors)
        {
            RemoveUnconnectedDoors();
            updatedDoors = true;
        }
    }

    public void RemoveUnconnectedDoors()
    {
        foreach (DoorScript door in doors)
        {
            switch (door.type)
            {
                case DoorScript.DoorType.top:
                    if (!GetCloseRoom(0, 1))
                    {
                        door.doorObject.SetActive(false);
                        foreach (GameObject wallObject in door.wallObjects)
                            wallObject.SetActive(true);
                    }
                    break;
                case DoorScript.DoorType.right:
                    if (!GetCloseRoom(1, 0))
                    {
                        door.doorObject.SetActive(false);
                        foreach (GameObject wallObject in door.wallObjects)
                            wallObject.SetActive(true);
                    }
                    break;
                case DoorScript.DoorType.down:
                    if (!GetCloseRoom(0, -1))
                    {
                        door.doorObject.SetActive(false);
                        foreach (GameObject wallObject in door.wallObjects)
                            wallObject.SetActive(true);
                    }
                    break;
                case DoorScript.DoorType.left:
                    if (!GetCloseRoom(-1, 0))
                    {
                        door.doorObject.SetActive(false);
                        foreach (GameObject wallObject in door.wallObjects)
                            wallObject.SetActive(true);
                    }
                    break;
            }
        }
    }

    public RoomScript GetCloseRoom(int X, int Z)
    {
        if(RoomController.instance.DoesRoomExist(x + X, z + Z))
            return RoomController.instance.FindRoom(x + X, z + Z);
        return null;
    }

    public Vector3 GetRoomCenter()
    {
        return new Vector3 (x * width, 0, z * length);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            RoomController.instance.OnPlayerEnterRoom(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(width, 0, length));
    }
}