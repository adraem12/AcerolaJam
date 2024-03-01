using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Recorder.OutputPath;

public class DungeonGenerator : MonoBehaviour
{
    public DungeonGenerationData dungeonGenerationData;
    List<Vector2Int> dungeonRooms;

    private void Start()
    {
        dungeonRooms = DungeonCrawlerController.GenerateDungeon(dungeonGenerationData);
        SpawnRooms(dungeonRooms);
    }

    void SpawnRooms(IEnumerable<Vector2Int> rooms)
    {
        RoomController.instance.LoadRoom("Start", 0, 0);
        foreach (Vector2Int roomLocation in rooms)
            RoomController.instance.LoadRoom("Empty", roomLocation.x, roomLocation.y);
    }
}