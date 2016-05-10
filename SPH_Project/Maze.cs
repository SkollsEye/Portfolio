using UnityEngine;
using System.Collections;

/// <summary>
/// Author: Mark DiVelbiss
/// The Maze class is a singleton that stores one square maze generated through a randomized Prim's Algorithm.
/// This maze is atypical in that it is made up of blocks rather than of walls.
/// </summary>
public class Maze {
    private static Maze instance = null;
    private static object padlock = new object();

    /// <summary>
    /// Creates a maze in the singleton if one does not already exist.
    /// </summary>
    /// <param name="size">Side length of the maze to be generated in blocks.</param>
    public static void CreateMaze(int size = minSize)
    {
        lock(padlock) { if (instance == null) instance = new Maze(size); }
    }
    /// <summary>
    /// Returns the current maze singleton, or creates one if none exists.
    /// </summary>
    public static Maze Instance
    {
        get { if (instance == null) CreateMaze(); return instance; }
    }

    byte[] grid;
    int _size;

    public const int minSize = 3, minBlocks = 0, maxBlocks = 5, pathHeight = 1, wallHeight = 2, boundaryHeight = 3;

    /// <summary>
    /// Private constructor for the maze singleton class.
    /// Generates the maze immediately using the maze size.
    /// </summary>
    /// <param name="size">Side length in blocks for the maze. Must be at least minSize.</param>
    private Maze(int size)
    {
        if (size < minSize) size = minSize;
        _size = size;
        GenerateMaze();
    }

    /// <summary>
    /// Runs the Randomized Prim's Algorithm to generate the maze.
    /// The maze is surrounded with boundary blocks that only has one exit.
    /// Inside the boundary, a section of the grid is either a path or a wall.
    /// </summary>
    void GenerateMaze()
    {
        // Initialize
        grid = new byte[_size * _size];
        ArrayList frontier = new ArrayList();
        int exitPath = 0;

        // Create Boundaries
        for(int i=0; i< _size; i++)
        {
            grid[i] = boundaryHeight;
            grid[i * _size] = boundaryHeight;
            grid[i + _size * (_size - 1)] = boundaryHeight;
            grid[i * _size + (_size - 1)] = boundaryHeight;
        }

        // Starting Cell is 1x1, add it to frontier
        frontier.Add(_size + 1);

        // Modified Prim's Algorithm
        while (frontier.Count > 0)
        {
            // Pick random cell from frontier
            int rand = Random.Range(0, frontier.Count);
            int index = (int)frontier[rand];
            // If cell is pathable, make cell a path and add adjacent unvisited cells to frontier
            if (isCellPath(index))
            {
                grid[index] = pathHeight;
                if (grid[index - 1] == 0 && !frontier.Contains(index - 1)) frontier.Add(index - 1);
                if (grid[index + 1] == 0 && !frontier.Contains(index + 1)) frontier.Add(index + 1);
                if (grid[index - _size] == 0 && !frontier.Contains(index - _size)) frontier.Add(index - _size);
                if (grid[index + _size] == 0 && !frontier.Contains(index + _size)) frontier.Add(index + _size);

                // Store value if cell is next to a boundary as a potential exit point
                if (isCellBoundary(index)) exitPath = index;
            }
            else // Otherwise, the cell is now a wall
            {
                grid[index] = wallHeight;
            }
            // Remove cell from frontier
            frontier.RemoveAt(rand);
        }

        // Create Exit
        if (isBoundary(exitPath)) grid[exitPath] = pathHeight;
        else if (isBoundary(exitPath - 1)) grid[exitPath - 1] = pathHeight;
        else if (isBoundary(exitPath + 1)) grid[exitPath + 1] = pathHeight;
        else if (isBoundary(exitPath - _size)) grid[exitPath - _size] = pathHeight;
        else grid[exitPath + _size] = pathHeight;

        // Ensure no zero height cells exist
        for (int i = 0; i < _size * _size; i++)
            if (grid[i] == 0) grid[i] = wallHeight;
    }

    /// <summary>
    /// Check for determining if a particular index on the grid is a cell next to the boundary.
    /// </summary>
    /// <param name="index">Index of a cell on the grid.</param>
    /// <returns>Whether or not the indicated cell is beside the boundary and is not itself a boundary cell.</returns>
    bool isCellBoundary(int index)
    {
        if (isBoundary(index)) return false;
        return (isBoundary(index - 1) ||
                isBoundary(index + 1) ||
                isBoundary(index - _size) ||
                isBoundary(index + _size));
    }

    /// <summary>
    /// Check for determining if a cell is on the boundary.
    /// </summary>
    /// <param name="index">Index of a cell on the grid.</param>
    /// <returns>Whether or not the indicated cell is a boundary cell.</returns>
    bool isBoundary(int index)
    {
        return (index / _size == 0 ||
                index / _size == _size - 1 ||
                index % _size == 0 ||
                index % _size == _size - 1);
    }
    
    /// <summary>
    /// Check for determining if a cell on the grid has the potential to be made into a path.
    /// A cell can be a path cell so long as it is not a boundary cell AND is adjacent to at most one other path cell.
    /// </summary>
    /// <param name="index">The index of a cell on the grid</param>
    /// <returns>Whether or not the indicated cell is a viable path cell.</returns>
    bool isCellPath(int index)
    {
        if (isBoundary(index)) return false;
        int count = 0;
        count += (grid[index - 1] == pathHeight ? 1 : 0);
        count += (grid[index + 1] == pathHeight ? 1 : 0);
        count += (grid[index - _size] == pathHeight ? 1 : 0);
        count += (grid[index + _size] == pathHeight ? 1 : 0);
        return (count <= 1);
    }

    /// <summary>
    /// Single indexing for array access to the maze grid.
    /// Not recommended.
    /// </summary>
    /// <param name="i">Index of cell on the grid.</param>
    /// <returns>The height value for the indicated cell.</returns>
    public int this[int i]
    {
        get { return grid[i]; }
        set { grid[i] = (byte)(value >= minBlocks && value <= maxBlocks ? value : 0); }
    }

    /// <summary>
    /// Double indexing for array access to the maze grid.
    /// Uses the standard format of [row, column]
    /// </summary>
    /// <param name="row">The row index of a cell on the grid.</param>
    /// <param name="column">The column index of a cell on the grid.</param>
    /// <returns>The height value for the indicated cell.</returns>
    public int this[int row, int column]
    {
        get { return grid[row * _size + column]; }
        set {
            if(value <= minBlocks) { grid[row * _size + column] = 0;  }
            else if(value > maxBlocks) { grid[row * _size + column] = 5; } 
            else { grid[row * _size + column] = (byte)value; }
        }
    }

    /// <summary>
    /// Returns the side length of the current maze.
    /// </summary>
    int Size { get { return _size; } }
}
