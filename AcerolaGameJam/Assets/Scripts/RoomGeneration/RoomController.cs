using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    bool updatedRooms = false;
    public List<GameObject> startRoomPrefabs;
    public List<GameObject> emptyRoomPrefabs;
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
                StartCoroutine(SpawnBossRoom());
            else if (spawnedBossRoom && !updatedRooms)
            {
                foreach (RoomScript room in loadedRooms)
                    room.RemoveUnconnectedDoors();
                updatedRooms = true;
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
        else if (currentLoadRoomData.name == "End")
            newRoom = endRoomPrefabs[Random.Range(0, endRoomPrefabs.Count - 1)];
        Instantiate(newRoom);
    }

    IEnumerator SpawnBossRoom()
    {
        spawnedBossRoom=true;
        yield return new WaitForSeconds(0.5f);
        if (loadRoomQueue.Count == 0)
        {
            RoomScript bossRoom = loadedRooms.Last();
            Vector2Int tempRoom = new(bossRoom.x, bossRoom.z);
            Destroy(bossRoom.gameObject);
            loadedRooms.Remove(loadedRooms.Single(r => r.x == tempRoom.x && r.z == tempRoom.y));
            LoadRoom("End", tempRoom.x, tempRoom.y);
        }
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
            room.transform.parent = transform;
            isLoadingRoom = false;
            if (loadedRooms.Count == 0)
                CameraController.instance.currentRoom = room;
            loadedRooms.Add(room);
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
    }
}