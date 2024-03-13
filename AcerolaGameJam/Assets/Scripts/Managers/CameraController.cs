using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    public RoomScript currentRoom;
    [SerializeField] float changeRoomSpeed;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        UpdatePosition();
    }

    void UpdatePosition()
    {
        if (currentRoom == null)
            return;
        transform.position = Vector3.MoveTowards(transform.position, GetCameraTargetPosition(), Time.deltaTime * changeRoomSpeed);
    }

    Vector3 GetCameraTargetPosition()
    {
        if(currentRoom == null)
            return Vector3.zero;
        Vector3 targetPosition = currentRoom.GetRoomCenter();
        targetPosition.y = transform.position.y;
        return targetPosition;
    }

    public bool IsSwitchingScene()
    {
        return transform.position.Equals(GetCameraTargetPosition()) == false;
    }
}