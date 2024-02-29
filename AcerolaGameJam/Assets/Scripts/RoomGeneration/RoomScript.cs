using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    public int width;
    public int length;
    public int x;
    public int z;

    void Start()
    {
        if (RoomController.instance == null)
        {
            Debug.Log("You pressed play in the wrong scene");
            return;
        }
        RoomController.instance.RegisterRoom(this);
    }

    public Vector3 GetRoomCenter()
    {
        return new Vector3 (x * width, z * length);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(width, 0, length));
    }
}