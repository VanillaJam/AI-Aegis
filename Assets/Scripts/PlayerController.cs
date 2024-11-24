using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;      // Speed of movement
    public MazeGenerator mazeGenerator; // Reference to the MazeGenerator script
    public int startX = 1;             // Custom starting X position
    public int startY = 1;             // Custom starting Y position
    private Vector2 targetPosition;   // The target cell position to move to
    private bool isMoving = false;    // Is the player currently moving?

    void Start()
    {
        PlacePlayer(); // Automatically place the player at the custom starting position
        targetPosition = transform.position;
    }

    void Update()
    {
        if ((Vector2)transform.position == targetPosition)
        {
            isMoving = false;
        }

        if (!isMoving)
        {
            HandleInput();
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    void PlacePlayer()
    {
        if (mazeGenerator == null)
        {
            Debug.LogError("MazeGenerator reference is missing! Ensure the PlayerController has a reference to the MazeGenerator.");
            return;
        }

        if (mazeGenerator.maze == null)
        {
            Debug.LogError("Maze data is not initialized! Ensure the MazeGenerator has completed maze generation before the player is placed.");
            return;
        }

        if (mazeGenerator == null || mazeGenerator.maze == null)
        {
            Debug.LogError("Maze data not found!");
            return;
        }

        // Validate the custom starting position
        if (startX >= 0 && startX < mazeGenerator.width &&
            startY >= 0 && startY < mazeGenerator.height &&
            !mazeGenerator.maze[startX, startY].isWall) // Ensure the starting cell is walkable
        {
            transform.position = new Vector2(startX, startY); // Place the player at the custom position
        }
        else
        {
            Debug.LogError($"Invalid starting position ({startX}, {startY}). Ensure it's within bounds and not a wall.");
        }
    }

    void HandleInput()
    {
        int x = Mathf.RoundToInt(transform.position.x);
        int y = Mathf.RoundToInt(transform.position.y);

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveToCell(x, y + 1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveToCell(x, y - 1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveToCell(x - 1, y);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveToCell(x + 1, y);
        }
    }

    void MoveToCell(int targetX, int targetY)
    {
        if (IsCellValid(targetX, targetY))
        {
            targetPosition = new Vector2(targetX, targetY);
            isMoving = true;
        }
    }

    bool IsCellValid(int x, int y)
    {
        if (mazeGenerator == null || mazeGenerator.maze == null)
        {
            Debug.LogError("Maze data not found!");
            return false;
        }

        if (x < 0 || x >= mazeGenerator.width || y < 0 || y >= mazeGenerator.height)
        {
            return false;
        }

        return !mazeGenerator.maze[x, y].isWall;
    }
}
