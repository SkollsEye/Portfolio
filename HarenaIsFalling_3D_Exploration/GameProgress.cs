using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Author: Mark DiVelbiss
/// The GameProgress script controls the overall progress of the player through the game and is attached to the same object as the GameManager.
/// A single GameProgress object is akin to a save file that allows all other scripts in the game to know where the player should spawn and what functionality to have.
/// Currently, the game is short enough that only one instance of GameProgress is allowed at a time, making it a Singleton, which is created on Awake() when the game starts.
/// This class also comes with skip functionality to skip to other parts of the game instantly.
/// </summary>
public class GameProgress : MonoBehaviour {
    private static GameProgress instance = null;
    private static object padlock = new object();

    // Values for the Toy Block Puzzle
    string toyBlockDials;
    const string toyBlockDialsStart = "0000";
    const string toyBlockDialsAnswer = "4180";

    /// <summary>
    /// Allows all scripts to retrieve the current progress for the game.
    /// </summary>
    public static GameProgress Instance { get { return instance; } }

    /// <summary>
    /// Awake instantiates the instance and ensures it is not destroyed between scenes.
    /// Then default values are set.
    /// </summary>
    void Awake()
    {
        lock(padlock)
        {
            if (instance == null) instance = this;
            else if (instance != this) Destroy(this);
            DontDestroyOnLoad(this);
            ResetProgress();
        }
    }

    /// <summary>
    /// Resets all progress values and properties to default.
    /// </summary>
    public void ResetProgress()
    {
        PlayerHasWindCannon = false;
        toyBlockDials = toyBlockDialsStart;
        KeyCardsOwned = 0;
        LastSeen = Location.Lab;
        IsLabFinished = false;
        IsEstatesFinished = false;
        IsGreenhouseFinished = false;
        IsGreenhousePuzzleCorrect = false;
    }

    /// <summary>
    /// Sets all values to be as though the player has beaten the Welcome Center and entered the Research Lab.
    /// </summary>
    public void SkipToLab()
    {
        PlayerHasWindCannon = true;
        toyBlockDials = toyBlockDialsAnswer;
        KeyCardsOwned = 0;
        LastSeen = Location.Lab;
        IsLabFinished = false;
        IsEstatesFinished = false;
        IsGreenhouseFinished = false;
        IsGreenhousePuzzleCorrect = false;
    }

    /// <summary>
    /// Sets all values to be as though the player has beaten the initial Research Lab and entered the Canyon.
    /// </summary>
    public void SkipToCanyon()
    {
        PlayerHasWindCannon = true;
        toyBlockDials = toyBlockDialsAnswer;
        KeyCardsOwned = 1;
        LastSeen = Location.Estates;
        IsLabFinished = true;
        IsEstatesFinished = false;
        IsGreenhouseFinished = false;
        IsGreenhousePuzzleCorrect = false;
    }

    /// <summary>
    /// Sets all values to be as though the player has beaten the Canyon and entered the Eremite Estates.
    /// </summary>
    public void SkipToEstates()
    {
        PlayerHasWindCannon = true;
        toyBlockDials = toyBlockDialsAnswer;
        KeyCardsOwned = 1;
        LastSeen = Location.Estates;
        IsLabFinished = true;
        IsEstatesFinished = false;
        IsGreenhouseFinished = false;
        IsGreenhousePuzzleCorrect = false;
    }

    /// <summary>
    /// Sets all values to be as though the player has beaten the Research Lab and traveled to the Greenhouse.
    /// </summary>
    public void SkipToGreenhouse()
    {
        PlayerHasWindCannon = true;
        toyBlockDials = toyBlockDialsAnswer;
        KeyCardsOwned = 1;
        LastSeen = Location.Greenhouse;
        IsLabFinished = true;
        IsEstatesFinished = false;
        IsGreenhouseFinished = false;
        IsGreenhousePuzzleCorrect = false;
    }

    /// <summary>
    /// Sets all values to be as though the player has beaten the Eremite Estates and Greenhouse and traveled to the Research Lab.
    /// </summary>
    public void SkipToFinal()
    {
        PlayerHasWindCannon = true;
        toyBlockDials = toyBlockDialsAnswer;
        KeyCardsOwned = 3;
        LastSeen = Location.Lab;
        IsLabFinished = true;
        IsEstatesFinished = true;
        IsGreenhouseFinished = true;
        IsGreenhousePuzzleCorrect = true;
    }

    /// <summary>
    /// Updates when the dials on the toy chest turn to match the displayed values.
    /// Will stop updating if the dials are all correct.
    /// </summary>
    /// <param name="dial">Which dial is changing, corresponding to the string order.</param>
    /// <param name="digit">What the specified is changing to.</param>
    public void UpdateToyBlockPuzzle(int dial, int digit)
    {
        if (IsToyBlockDialsCorrect || dial < 1 || dial > 4) return;
        toyBlockDials = toyBlockDials.Substring(0, dial - 1) + digit + toyBlockDials.Substring(dial);
    }

    /// <summary>
    /// Compares the answer to the current state of the dials and returns if they are correct.
    /// </summary>
    public bool IsToyBlockDialsCorrect { get { return toyBlockDialsAnswer.Equals(toyBlockDials); } }

    /// <summary>
    /// Returns if the Research Lab has been completed.
    /// </summary>
    public bool IsLabFinished { get; set; }

    /// <summary>
    /// Returns if the Eremite Estates has been completed.
    /// </summary>
    public bool IsEstatesFinished { get; set; }

    /// <summary>
    /// Returns if the Greenhouse has been completed.
    /// </summary>
    public bool IsGreenhouseFinished { get; set; }

    /// <summary>
    /// Returns if the Greenhouse Puzzle has been completed.
    /// </summary>
    public bool IsGreenhousePuzzleCorrect { get; set; }

    /// <summary>
    /// Returns if the player has the Wind Cannon.
    /// </summary>
    public bool PlayerHasWindCannon { get; set; }

    /// <summary>
    /// Returns the total number of Key Cards the player has retrieved.
    /// </summary>
    public int KeyCardsOwned { get; set; }

    /// <summary>
    /// Returns the players last notable location within the Central Region.
    /// This allows us to set waypoints in case the player falls or when they come back from the estates.
    /// </summary>
    public Location LastSeen { get; set; }

    public enum Location { Lab, Estates, IslandsMain, IslandsFar, Greenhouse }
}
