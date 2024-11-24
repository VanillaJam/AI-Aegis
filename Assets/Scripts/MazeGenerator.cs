using UnityEngine;
using static CellStruct;

public class MazeGenerator : MonoBehaviour
{

    [SerializeField] public int width;   // Width of the maze
    [SerializeField] public int height;  // Height of the maze
    public GameObject wallPrefab;  // Prefab for walls
    public GameObject floorPrefab; // Prefab for floors

    public MazeCell[,] maze; // 2D array of MazeCell structs

    void Start()
    {
        InitializeMaze();    // Initialize the maze grid
        GenerateMaze();      // Generate the maze structure
        BuildMaze();         // Build the maze in Unity
    }

    // Step 1: Initialize the Maze Grid
    void InitializeMaze()
    {
        maze = new MazeCell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = new MazeCell
                {
                    isWall = false,
                    isVisited = false
                };
            }
        }
    }

    // Step 2: Generate the Maze Structure
    void GenerateMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Borders are walls
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    maze[x, y].isWall = true;
                }
                else
                {
                    maze[x, y].isWall = Random.Range(0, 2) == 0; // Randomly set some interior cells as walls
                }
            }
        }
    }

    // Step 3: Build the Maze in Unity
    void BuildMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 position = new Vector2(x, y); // Adjust position based on prefab

                if (maze[x, y].isWall)
                {
                    Instantiate(wallPrefab, position, Quaternion.identity, transform);
                }
                else
                {
                    Instantiate(floorPrefab, position, Quaternion.identity, transform);
                }
            }
        }
    }

}
