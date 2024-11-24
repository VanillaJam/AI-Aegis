using UnityEngine;

public class CellStruct : MonoBehaviour
{
    public struct MazeCell
    {
        public bool isWall;     // Is this cell a wall?
        public bool isVisited;  // Has this cell been visited? (useful for generation algorithms)
    }
}
