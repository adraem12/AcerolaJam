using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;
using static UnityEditor.Recorder.OutputPath;

public class RoomInfo
{
    public string name;
    public int x;
    public int z;
}

public class RoomController : MonoBehaviour
{
    public static RoomController instance;
    RoomInfo currentLoadRoomData;
    RoomScript currentRoom;
    Queue<RoomInfo> loadRoomQueue = new();
    List<RoomScript> loadedRooms = new();
    bool isLoadingRoom = false;
    bool spawnedBossRoom = false;
    bool spawnedExtraRooms = false;
    bool updatedRooms = false;
    public List<GameObject> startRoomPrefabs;
    public List<GameObject> emptyRoomPrefabs;
    public List<GameObject> itemRoomPrefabs;
    public List<GameObject> endRoomPrefabs;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        UpdateRoomQueue();
    }

    void UpdateRoomQueue()
    {
        if (isLoadingRoom)
            return;
        if(loadRoomQueue.Count == 0)
        {
            if (!spawnedBossRoom)
                StartCoroutine(SpawnExtraRooms());
            else if (spawnedExtraRooms && !updatedRooms)
            {
                foreach (RoomScript room in loadedRooms)
                    room.RemoveUnconnectedDoors();
                StartCoroutine(RoomCoroutine());
                updatedRooms = true;
                GameUIManager.instance.DrawMap(loadedRooms);
            }
            return;
        }
        currentLoadRoomData = loadRoomQueue.Dequeue();
        isLoadingRoom = true;
        GameObject newRoom = null;
        if (currentLoadRoomData.name == "Start")
            newRoom = startRoomPrefabs[Random.Range(0, startRoomPrefabs.Count - 1)];
        else if (currentLoadRoomData.name == "Empty")
            newRoom = emptyRoomPrefabs[Random.Range(0, emptyRoomPrefabs.Count - 1)];
        else if (currentLoadRoomData.name == "Item")
            newRoom = itemRoomPrefabs[Random.Range(0, itemRoomPrefabs.Count - 1)];
        else if (currentLoadRoomData.name == "End")
            newRoom = endRoomPrefabs[Random.Range(0, endRoomPrefabs.Count - 1)];
        Instantiate(newRoom);
    }

    IEnumerator SpawnExtraRooms()
    {
        spawnedBossRoom = true;
        yield return new WaitForSeconds(0.15f);
        if (loadRoomQueue.Count == 0)
        {
            RoomScript bossRoom = loadedRooms.Last();
            Vector2Int tempRoom = new(bossRoom.x, bossRoom.z);
            Destroy(bossRoom.gameObject);
            loadedRooms.Remove(loadedRooms.Single(r => r.x == tempRoom.x && r.z == tempRoom.y));
            LoadRoom("End", tempRoom.x, tempRoom.y);
            StartCoroutine(SpawnItemRoom());
        }
    }

    IEnumerator SpawnItemRoom()
    {
        spawnedExtraRooms = true;
        yield return new WaitForSeconds(0.15f);
        List<RoomScript> roomListByDistance = new(loadedRooms);
        roomListByDistance.RemoveAt(0);
        roomListByDistance.Sort((a, b) => Vector3.Distance(a.GetRoomCenter(), loadedRooms.Last().GetRoomCenter()).CompareTo(Vector3.Distance(b.GetRoomCenter(), loadedRooms.Last().GetRoomCenter())));
        RoomScript itemRoom = roomListByDistance.Last();
        Vector2Int tempRoom = new(itemRoom.x, itemRoom.z);
        Destroy(itemRoom.gameObject);
        loadedRooms.Remove(loadedRooms.Single(r => r.x == tempRoom.x && r.z == tempRoom.y));
        LoadRoom("Item", tempRoom.x, tempRoom.y);
    }

    public void LoadRoom(string name, int x, int z)
    {
        if (DoesRoomExist(x, z))
            return;
        RoomInfo newRoomData = new()
        {
            name = name,
            x = x,
            z = z
        };
        loadRoomQueue.Enqueue(newRoomData);
    }

    public void RegisterRoom(RoomScript room)
    {
        if(!DoesRoomExist(currentLoadRoomData.x, currentLoadRoomData.z))
        {
            room.transform.position = new Vector3(currentLoadRoomData.x * room.width, 0, currentLoadRoomData.z * room.length);
            room.x = currentLoadRoomData.x;
            room.z = currentLoadRoomData.z;
            room.name += "_" + room.x + "-" + room.z;
            isLoadingRoom = false;
            if (loadedRooms.Count == 0)
                CameraController.instance.currentRoom = room;
            loadedRooms.Add(room);
            room.StartCoroutine(room.Init());
        }
        else
        {
            Destroy(room.gameObject);
            isLoadingRoom = false;
        }
    }

    public bool DoesRoomExist(int x, int z)
    {
        return loadedRooms.Find(item => item.x == x && item.z == z) != null;
    }

    public RoomScript FindRoom(int x, int z)
    {
        return loadedRooms.Find(item => item.x == x && item.z == z);
    }

    public void OnPlayerEnterRoom(RoomScript room)
    {
        CameraController.instance.currentRoom = room;
        currentRoom = room;
        StartCoroutine(RoomCoroutine());
    }

    public IEnumerator RoomCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        UpdateRooms();
    }

    void UpdateRooms()
    {
        foreach (RoomScript room in loadedRooms)
            if (room != currentRoom)
            {
                EnemyController[] enemies = room.GetComponentsInChildren<EnemyController>();
                if (enemies != null)
                    foreach (EnemyController enemy in enemies)
                        enemy.notInRoom = true;
                foreach (DoorScript door in room.doors)
                    door.OpenDoor();
            }
            else
            {
                EnemyController[] enemies = room.GetComponentsInChildren<EnemyController>();
                if (enemies.Length > 0)
                {
                    foreach (EnemyController enemy in enemies)
                        enemy.notInRoom = false;
                    foreach (DoorScript door in room.doors)
                        door.CloseDoor();
                }
                else
                    foreach (DoorScript door in room.doors)
                        door.OpenDoor();
            }
    }
}