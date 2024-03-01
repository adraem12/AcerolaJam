using System.Collections.Generic;
using UnityEngine;

public class DungeonCrawlerScript
{
    public Vector2Int Position {  get; set; }

    public DungeonCrawlerScript(Vector2Int startPosition)
    {
        Position = startPosition;
    }

    public Vector2Int Move(Dictionary<Direction, Vector2Int> directionMovementMap)
    {
        Direction toMove = (Direction)Random.Range(0, directionMovementMap.Count);
        Position += directionMovementMap[toMove];
        return Position;
    }
}