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
    GridController gridController;
    [System.Serializable]
    public struct EnemyList
    {
        public GameObject enemyPrefab;
        public int quantity;
    }
    public EnemyList[] enemies;

    void Start()
    {
        if (RoomController.instance == null)
        {
            Debug.Log("You pressed play in the wrong scene");
            return;
        }
        RoomController.instance.RegisterRoom(this);
        gridController = GetComponentInChildren<GridController>();
        SpawnEnemies();
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

    void SpawnEnemies()
    {
        Vector3 roomSpawnPoint = GetRoomCenter() - new Vector3 (gridController.grid.columns / 2f, 0, gridController.grid.rows / 2f);
        foreach (EnemyList enemy in enemies)
            for (int i = 0; i < enemy.quantity; i++)
            {
                Vector3 currentPoint = gridController.availablePoints[Random.Range(0, gridController.availablePoints.Count - 1)];
                Instantiate(enemy.enemyPrefab, roomSpawnPoint + currentPoint, Quaternion.identity, transform);
                gridController.availablePoints.Remove(currentPoint);
            }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("a");
        if (other.CompareTag("Player"))
            RoomController.instance.OnPlayerEnterRoom(this);
    }
}