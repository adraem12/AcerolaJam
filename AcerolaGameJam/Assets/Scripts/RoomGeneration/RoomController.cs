using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomInfo
{
    public string name;
    public int x;
    public int z;
}

public class RoomController : MonoBehaviour
{
    public static RoomController instance;
    string currentWorldName = "Test";
    RoomInfo currentLoadRoomData;
    Queue<RoomInfo> loadRoomQueue = new();
    public List<RoomScript> loadedRooms = new();
    bool isLoading = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LoadRoom("Start", 0, 0);
        LoadRoom("Empty", 1, 0);
        LoadRoom("Empty", -1, 0);
        LoadRoom("Empty", 0, 1);
        LoadRoom("Empty", 0, -1);
    }

    private void Update()
    {
        UpdateRoomQueue();
    }

    void UpdateRoomQueue()
    {
        if (isLoading || loadRoomQueue.Count == 0)
            return;
        currentLoadRoomData = loadRoomQueue.Dequeue();
        isLoading = true;
        StartCoroutine(LoadRoomRutine(currentLoadRoomData));
    }

    public void LoadRoom(string name, int x, int z)
    {
        if (DoesRoomExist(x, z))
            return;
        RoomInfo newRoomData = new();
        newRoomData.name = name;
        newRoomData.x = x;
        newRoomData.z = z;
        loadRoomQueue.Enqueue(newRoomData);
    }

    IEnumerator LoadRoomRutine(RoomInfo info)
    {
        string roomName = currentWorldName + info.name;
        AsyncOperation loadRoom = SceneManager.LoadSceneAsync(roomName, LoadSceneMode.Additive);
        while (loadRoom.isDone == false)
            yield return null;
    }

    public void RegisterRoom(RoomScript room)
    {
        room.transform.position = new Vector3(currentLoadRoomData.x * room.width, 0, currentLoadRoomData.z * room.z);
        room.x = currentLoadRoomData.x;
        room.z = currentLoadRoomData.z;
        room.name = currentWorldName + "-" + currentLoadRoomData.name + " " + room.x + ", " + room.z;
        room.transform.parent = transform;
        isLoading = false;
        loadedRooms.Add(room);
    }

    public bool DoesRoomExist(int x, int z)
    {
        return loadedRooms.Find(item => item.x == x && item.z == z) != null;
    }
}