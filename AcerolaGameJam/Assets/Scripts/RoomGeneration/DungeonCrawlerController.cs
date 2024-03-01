using System.Collections.Generic;
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
        int iterations = Random.Range(dungeonData.iterationMin, dungeonData.iterationMax);
        for (int i = 0; i < iterations; ++i)
            foreach (DungeonCrawlerScript dungeonCrawler in dungeonCrawlers)
            {
                Vector2Int newPosition = dungeonCrawler.Move(directionMovementMap);
                positionsVisited.Add(newPosition);
            }
        return positionsVisited;
    }
}