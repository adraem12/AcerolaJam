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
    }

    public IEnumerator Init()
    {
        gridController = GetComponentInChildren<GridController>();
        yield return new WaitForSeconds(0.2f);
        gridController.GenerateGrid();
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

    public Vector3 GetRoomCorner()
    {
        return GetRoomCenter() - new Vector3(gridController.grid.columns / 2f, 0, gridController.grid.rows / 2f) + Vector3.one * 0.5f;
    }

    void SpawnEnemies()
    {
        foreach (EnemyList enemy in enemies)
            for (int i = 0; i < enemy.quantity; i++)
            {
                Vector3 currentPoint = gridController.availablePoints[Random.Range(0, gridController.availablePoints.Count - 1)];
                GameObject newEnemy = Instantiate(enemy.enemyPrefab, GetRoomCorner() + currentPoint, Quaternion.identity, transform);
                newEnemy.transform.position = new Vector3(newEnemy.transform.position.x, 1, newEnemy.transform.position.z);
                gridController.availablePoints.Remove(currentPoint);
            }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            RoomController.instance.OnPlayerEnterRoom(this);
    }
}