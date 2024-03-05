using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    RoomScript room;
    [System.Serializable]
    public struct Grid 
    {
        public int columns, rows;
        public float widthOffset, lengthOffset;
    }
    public Grid grid;
    public GameObject gridTile;
    public List<Vector3> availablePoints = new();

    private void Awake()
    {
        room = GetComponentInParent<RoomScript>();
        grid.columns = room.width - 2;
        grid.rows = room.length - 2;
        GenerateGrid();
    }

    void GenerateGrid()
    {
        grid.lengthOffset += room.transform.localPosition.x;
        grid.widthOffset += room.transform.localPosition.z;
        for (int z = 0; z < grid.rows; z++)
            for (int x = 0; x < grid.columns; x++)
                availablePoints.Add(new Vector3(x, 0, z));
    }
}