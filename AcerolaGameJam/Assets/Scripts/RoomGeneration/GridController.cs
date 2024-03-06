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
    public List<Vector3> availablePoints = new();

    private void Start()
    {
        room = GetComponentInParent<RoomScript>();
        grid.columns = room.width - 2;
        grid.rows = room.length - 2;
    }

    public void GenerateGrid()
    {
        grid.lengthOffset += room.transform.localPosition.x;
        grid.widthOffset += room.transform.localPosition.z;
        for (int z = 0; z < grid.rows; z++)
            for (int x = 0; x < grid.columns; x++)
            {
                Vector3 gridPoint = new(x, 0, z);
                RaycastHit[] hit = new RaycastHit[1];
                Physics.RaycastNonAlloc(room.GetRoomCorner() + gridPoint + Vector3.up * 3f, Vector3.down, hit, 10f, LayerMask.GetMask("Scenery"));
                if (hit[0].collider != null && hit[0].collider.name == "Floor")
                    availablePoints.Add(gridPoint);
            }
    }
}