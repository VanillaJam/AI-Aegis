using System;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int width = 10; // Maze width
    public int height = 10; // Maze height
    public GameObject wallPrefab; // Prefab for walls
    public GameObject floorPrefab; // Prefab for the floor
    public GameObject goalPrefab; // Prefab for the goal marker
    //public GameObject lockedCellBluePrefab; // Prefab for the blue door
    //public GameObject blueKeyPrefab; // Prefab for the blue splash
    //public GameObject lockedCellRedPrefab; // Prefab for the red door
    //public GameObject redKeyPrefab;// Prefab for the red splash
    //public GameObject lockedCellGreenPrefab; // Prefab for the red door
    //public GameObject greenKeyPrefab; // Prefab for the green splash


    public GameObject lockedCellPrefab;
    public GameObject keyPrefab;
    public GameObject blockedCellPrefab;
    public GameObject secondaryKeyPrefab;


    public Cell[,] maze; // 2D array representing the maze
    public Vector2Int start; // Start position (bottom-left corner)
    public Vector2Int goal; // Goal position
    public Vector2Int keyCell; // Position of the key
    public Vector2Int lockedCell; // Position of the locked cell
    public Vector2Int secondaryKeyCell;

    //public Vector2Int blueDoorCell; // Position of the blue door
    //public Vector2Int redDoorCell; // Position of the red door
    //public Vector2Int greenDoorCell; // Position of the green door
    //public Vector2Int blueSpashCell;
    //public Vector2Int redSpashCell;
    //public Vector2Int greenSpashCell;



    bool IsInBounds(Vector2Int position)
    {
        return position.x >= 0 && position.x < width && position.y >= 0 && position.y < height;
    }


    private bool keyCollected = false; // Tracks if the player has collected the key

    void Start()
    {
        InitializeMaze();
        GenerateMaze();
        EnsurePathToGoal();
        BuildMaze();
        //PlaceKeyAndLockedCell();
        PlaceGoal();
        PlaceKeyAndLockedCells();
    }

    // Represents a single cell in the maze
    public class Cell
    {
        public bool isWall = true; // Default all cells are walls
        public bool isLocked = false; // Indicates if the cell is locked
    }

    // Initializes the maze
    void InitializeMaze()
    {
        maze = new Cell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = new Cell();
            }
        }

        // Set start to bottom-left corner
        start = new Vector2Int(0, 0);

        // Set default goal to top-right corner
        goal = new Vector2Int(width - 1, height - 1);

        // Ensure the start cell is walkable
        maze[start.x, start.y].isWall = false;
    }

    // Generates a random maze (simple random walls example)
    void GenerateMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y].isWall = UnityEngine.Random.value > 0.7f; // 70% chance to be a wall
            }
        }

        // Ensure the start and goal cells are walkable
        maze[start.x, start.y].isWall = false;
        maze[goal.x, goal.y].isWall = false;
    }

    // Ensures there is a path from start to goal
    void EnsurePathToGoal()
    {
        if (!IsPathToGoal(start, goal))
        {
            Debug.Log("No path to goal. Creating one...");
            CreatePathToGoal();
        }
    }
    void CreatePathToGoal()
    {
        // Start carving a path from the start position
        Vector2Int current = start;

        while (current != goal)
        {
            // Make the current cell walkable
            maze[current.x, current.y].isWall = false;

            // Move closer to the goal
            if (current.x < goal.x)
                current.x++;
            else if (current.y < goal.y)
                current.y++;
            else if (current.x > goal.x)
                current.x--;
            else if (current.y > goal.y)
                current.y--;
        }

        // Ensure the goal cell is walkable
        maze[goal.x, goal.y].isWall = false;
    }

    void PlaceKeyAndLockedCells()
    {


        // Step 2: Surround the goal with locked cells on all 4 sides (up, down, left, right)
        Vector2Int[] goalSurroundingCells = new Vector2Int[]
        {
        new Vector2Int(goal.x - 1, goal.y), // Left
        new Vector2Int(goal.x + 1, goal.y), // Right
        new Vector2Int(goal.x, goal.y - 1), // Down
        new Vector2Int(goal.x, goal.y + 1)  // Up
        };

        foreach (var cell in goalSurroundingCells)
        {
            if (IsInBounds(cell))
            {
                maze[cell.x, cell.y].isLocked = true;
                maze[cell.x, cell.y].isWall = true; // Initially locked and unwalkable
                Instantiate(lockedCellPrefab, new Vector2(cell.x, cell.y), Quaternion.identity);
            }
        }

        // Step 3: Place the first key (Primary Key)
        do
        {
            keyCell = new Vector2Int(UnityEngine.Random.Range(0, width), UnityEngine.Random.Range(0, height));
        } while (maze[keyCell.x, keyCell.y].isWall || keyCell == start || keyCell == goal || Array.Exists(goalSurroundingCells, cell => cell == keyCell));

        // Ensure the key cell is walkable
        maze[keyCell.x, keyCell.y].isWall = false;
        Instantiate(keyPrefab, new Vector2(keyCell.x, keyCell.y), Quaternion.identity);

        // Step 4: Place the second key (Secondary Key) to unlock key-blocking cells
        // Surround the key cell with blocked cells
        Vector2Int[] keySurroundingCells = new Vector2Int[]
        {
        new Vector2Int(keyCell.x - 1, keyCell.y), // Left
        new Vector2Int(keyCell.x + 1, keyCell.y), // Right
        new Vector2Int(keyCell.x, keyCell.y - 1), // Down
        new Vector2Int(keyCell.x, keyCell.y + 1)  // Up
        };

        // Mark the cells surrounding the key as blocked
        foreach (var cell in keySurroundingCells)
        {
            if (IsInBounds(cell))
            {
                maze[cell.x, cell.y].isWall = true; // Block the surrounding cells of the key
                maze[cell.x, cell.y].isLocked = true; // Lock them as well
                Instantiate(blockedCellPrefab, new Vector2(cell.x, cell.y), Quaternion.identity); // Create block visuals
            }
        }

        // Place the secondary key somewhere in the maze, ensuring it doesn't conflict with start, goal, or locked cells
        do
        {
            secondaryKeyCell = new Vector2Int(UnityEngine.Random.Range(0, width), UnityEngine.Random.Range(0, height));
        } while (maze[secondaryKeyCell.x, secondaryKeyCell.y].isWall || secondaryKeyCell == start || secondaryKeyCell == keyCell || secondaryKeyCell == goal || Array.Exists(keySurroundingCells, cell => cell == secondaryKeyCell));

        // Ensure secondary key cell is walkable
        maze[secondaryKeyCell.x, secondaryKeyCell.y].isWall = false;
        Instantiate(secondaryKeyPrefab, new Vector2(secondaryKeyCell.x, secondaryKeyCell.y), Quaternion.identity);
    }

    public void CollectSecondaryKey()
    {
        Debug.Log("Secondary key collected! Unlocking blocked cells around the primary key...");

        // Unlock all surrounding blocked cells around the primary key cell
        Vector2Int[] keySurroundingCells = new Vector2Int[]
        {
        new Vector2Int(keyCell.x - 1, keyCell.y), // Left
        new Vector2Int(keyCell.x + 1, keyCell.y), // Right
        new Vector2Int(keyCell.x, keyCell.y - 1), // Down
        new Vector2Int(keyCell.x, keyCell.y + 1)  // Up
        };

        foreach (var cell in keySurroundingCells)
        {
            if (IsInBounds(cell))
            {
                maze[cell.x, cell.y].isLocked = false; // Unlock the surrounding blocked cells
                maze[cell.x, cell.y].isWall = false;   // Make it walkable
                Instantiate(floorPrefab, new Vector2(cell.x, cell.y), Quaternion.identity); // Replace with walkable floor
            }
        }
    }



    //void PlaceKeyAndLockedCell()
    //{
    //    // Place the goal cell in a random position, ensuring it is not on the edges
    //    do
    //    {
    //        goal = new Vector2Int(UnityEngine.Random.Range(1, width - 1), UnityEngine.Random.Range(1, height - 1));
    //    } while (maze[goal.x, goal.y].isWall); // Ensure the goal is placed on a walkable area
    //    maze[goal.x, goal.y].isWall = false; // Ensure goal is walkable

    //    // Surround the goal with locked cells on all 4 sides (up, down, left, right)
    //    Vector2Int[] surroundingCells = new Vector2Int[]
    //    {
    //    new Vector2Int(goal.x - 1, goal.y), // Left
    //    new Vector2Int(goal.x + 1, goal.y), // Right
    //    new Vector2Int(goal.x, goal.y - 1), // Down
    //    new Vector2Int(goal.x, goal.y + 1)  // Up
    //    };

    //    foreach (var cell in surroundingCells)
    //    {
    //        if (IsInBounds(cell))
    //        {
    //            maze[cell.x, cell.y].isLocked = true;
    //            maze[cell.x, cell.y].isWall = true; // Initially locked and unwalkable
    //            Instantiate(lockedCellBluePrefab, new Vector2(cell.x, cell.y), Quaternion.identity);
    //        }
    //    }

    //    // Place the key in a random walkable location, ensuring it's not the goal or locked cells
    //    do
    //    {
    //        keyCell = new Vector2Int(UnityEngine.Random.Range(0, width), UnityEngine.Random.Range(0, height));
    //    } while (maze[keyCell.x, keyCell.y].isWall || keyCell == start || keyCell == goal || Array.Exists(surroundingCells, cell => cell == keyCell));

    //    // Ensure the key cell is walkable
    //    maze[keyCell.x, keyCell.y].isWall = false;
    //    Instantiate(blueKeyPrefab, new Vector2(keyCell.x, keyCell.y), Quaternion.identity);
    //}




    // A* Pathfinding algorithm to check if a path exists
    bool IsPathToGoal(Vector2Int start, Vector2Int goal)
    {
        HashSet<Vector2Int> openSet = new HashSet<Vector2Int>();
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
        Dictionary<Vector2Int, int> gCosts = new Dictionary<Vector2Int, int>();
        Dictionary<Vector2Int, int> fCosts = new Dictionary<Vector2Int, int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        openSet.Add(start);
        gCosts[start] = 0;
        fCosts[start] = Heuristic(start, goal);

        while (openSet.Count > 0)
        {
            // Get node with lowest fCost
            Vector2Int current = GetLowestFCostNode(openSet, fCosts);

            if (current == goal) return true; // Path found

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Vector2Int neighbor in GetNeighbors(current))
            {
                if (closedSet.Contains(neighbor) || maze[neighbor.x, neighbor.y].isWall || maze[neighbor.x, neighbor.y].isLocked) continue;

                int tentativeGCost = gCosts[current] + 1;

                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
                else if (tentativeGCost >= gCosts[neighbor])
                {
                    continue;
                }

                // Update costs and path
                cameFrom[neighbor] = current;
                gCosts[neighbor] = tentativeGCost;
                fCosts[neighbor] = gCosts[neighbor] + Heuristic(neighbor, goal);
            }
        }

        return false; // No path found
    }

    // Heuristic for A* (Manhattan distance)
    int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    // Gets valid neighbors of a cell
    List<Vector2Int> GetNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        if (cell.x > 0) neighbors.Add(new Vector2Int(cell.x - 1, cell.y));
        if (cell.x < width - 1) neighbors.Add(new Vector2Int(cell.x + 1, cell.y));
        if (cell.y > 0) neighbors.Add(new Vector2Int(cell.x, cell.y - 1));
        if (cell.y < height - 1) neighbors.Add(new Vector2Int(cell.x, cell.y + 1));

        return neighbors;
    }

    // Finds the node with the lowest fCost
    Vector2Int GetLowestFCostNode(HashSet<Vector2Int> openSet, Dictionary<Vector2Int, int> fCosts)
    {
        Vector2Int lowest = Vector2Int.zero;
        int minCost = int.MaxValue;

        foreach (var node in openSet)
        {
            int cost = fCosts.ContainsKey(node) ? fCosts[node] : int.MaxValue;
            if (cost < minCost)
            {
                minCost = cost;
                lowest = node;
            }
        }

        return lowest;
    }

    // Builds the maze in the Unity scene
    void BuildMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (maze[x, y].isWall)
                {
                    Instantiate(wallPrefab, new Vector2(x, y), Quaternion.identity);
                }
                else
                {
                    Instantiate(floorPrefab, new Vector2(x, y), Quaternion.identity);
                }
            }
        }
    }

    // Places the goal object at the goal position
    void PlaceGoal()
    {
        // Step 1: Place the goal cell in a random position, ensuring it's not on the edges
        do
        {
            goal = new Vector2Int(UnityEngine.Random.Range(1, width - 1), UnityEngine.Random.Range(1, height - 1));
        } while (maze[goal.x, goal.y].isWall); // Ensure the goal is placed on a walkable area
        maze[goal.x, goal.y].isWall = false; // Ensure goal is walkable

        Instantiate(goalPrefab, new Vector2(goal.x, goal.y), Quaternion.identity);
    }

    // Unlocks the locked cell when the key is collected
    //public void CollectKey()
    //{
    //    Debug.Log("Key collected! Unlocking locked cell...");
    //    keyCollected = true;
    //    maze[lockedCell.x, lockedCell.y].isLocked = false; // Unlock the cell
    //    maze[lockedCell.x, lockedCell.y].isWall = false;   // Make it walkable
    //}

    public void CollectKey()
    {
        Debug.Log("Key collected! Unlocking locked cells...");
        keyCollected = true;

        // Unlock all surrounding locked cells
        Vector2Int[] surroundingCells = new Vector2Int[]
        {
        new Vector2Int(goal.x - 1, goal.y), // Left
        new Vector2Int(goal.x + 1, goal.y), // Right
        new Vector2Int(goal.x, goal.y - 1), // Down
        new Vector2Int(goal.x, goal.y + 1)  // Up
        };

        foreach (var cell in surroundingCells)
        {
            if (IsInBounds(cell))
            {
                maze[cell.x, cell.y].isLocked = false; // Unlock the cell
                maze[cell.x, cell.y].isWall = false;   // Make it walkable
                Instantiate(floorPrefab, new Vector2(cell.x, cell.y), Quaternion.identity); // Replace locked cell with walkable floor
            }
        }
    }


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
