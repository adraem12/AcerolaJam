using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Direction
{
    up = 0, right = 1, down = 2, left = 3,
}

public class DungeonCrawlerController : MonoBehaviour
{
    public static List<Vector2Int> positionsVisited = new();

    static readonly Dictionary<Direction, Vector2Int> directionMovementMap = new()
    {
        { Direction.up, Vector2Int.up },
        { Direction.right, Vector2Int.right },
        { Direction.down, Vector2Int.down },
        { Direction.left, Vector2Int.left }
    };

    public static List<Vector2Int> GenerateDungeon(DungeonGenerationData dungeonData)
    {
        List<DungeonCrawlerScript> dungeonCrawlers = new();
        for (int i = 0; i < dungeonData.numberOfCrawlers; ++i)
            dungeonCrawlers.Add(new DungeonCrawlerScript(Vector2Int.zero));
        for (int i = 0; i < dungeonData.iterations; ++i)
            foreach (DungeonCrawlerScript dungeonCrawler in dungeonCrawlers)
            {
                Vector2Int newPosition = dungeonCrawler.Move(directionMovementMap);
                positionsVisited.Add(newPosition);
            }
        positionsVisited = positionsVisited.Distinct().ToList();
        if (positionsVisited.Count < dungeonData.totalRooms)
            for (int i = 0; i < dungeonData.iterations; ++i)
                foreach (DungeonCrawlerScript dungeonCrawler in dungeonCrawlers)
                {
                    Vector2Int newPosition = dungeonCrawler.Move(directionMovementMap);
                    positionsVisited.Add(newPosition);
                }
        positionsVisited = positionsVisited.Distinct().ToList();
        if(positionsVisited.Count > dungeonData.totalRooms)
            positionsVisited.RemoveRange(14, positionsVisited.Count - 14);
        return positionsVisited;
    }
}