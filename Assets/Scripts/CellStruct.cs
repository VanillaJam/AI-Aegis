using UnityEngine;

public class CellStruct : MonoBehaviour
{
    public struct MazeCell
    {
        public bool isWall;     // Is this cell a wall?
        public bool isVisited;  // Has this cell been visited? (useful for generation algorithms)

        public bool isGreenFloor;
        public bool isRedFloor;
        public bool isBlueFloor;

        public bool isGreenSwitch;
        public bool isRedSwitch;
        public bool isBlueSwitch;

        public bool isGreenDoor;
        public bool isRedDoor;
        public bool isBlueDoor;

        public bool isGoal;
        public bool isStartingPoint;

    }
}
