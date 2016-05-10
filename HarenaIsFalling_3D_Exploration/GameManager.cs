using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Author: Mark DiVelbiss
/// The GameManager is attached to a unity object with a canvas that persists through scenes.
/// There can only be one of this class, as it is a Singleton, which is created at on Awake() and is accessible statically through Instance.
/// The GameManager is used to switch scenes and has the pause menu, notifier, transition screen, and death screen built in.
/// The GameManager also sets up or controls notifications, scene music, and letter data.
/// Most operations are controlled with semaphores to prevent multiple access to key values.
/// </summary>
public class GameManager : MonoBehaviour {
    public GameObject PauseController;
    public GameObject Notifier;
    public GameObject CoverScreen;
    public const string menuScene = "Menu";

    private static GameManager instance = null;
    private static object padlock = new object();

    private State state;
    private AudioSource aud;
    private Image notifierImage;
    private PauseButtons.PauseState pauseState;
    private string currentInfoLetter;
    private bool saveTerrain;

    private const float notificationShowTime = 10;
    private const float notificationFadeTime = 1;

    /// <summary>
    /// The only instance of the GameManager allowed is accessible here.
    /// </summary>
    public static GameManager Instance { get { return instance; } }

    /// <summary>
    /// On Awake, set this instance up and ensure it is the only instance and that this gameobject persists through scenes.
    /// Then instantiate all values.
    /// </summary>
    void Awake()
    {
        lock(padlock)
        {
            if (instance == null) instance = this;
            else { Destroy(gameObject); return; }
            DontDestroyOnLoad(this);
            state = State.Load;
            aud = GetComponent<AudioSource>();
            pauseState = PauseButtons.PauseState.Pause;
            ShowCursor = true;
            currentInfoLetter = "";
            PauseController.SetActive(false);
            notifierImage = Notifier.GetComponent<Image>();
            notifierImage.color = new Color(1, 1, 1, 0);
            CoverScreen.SetActive(false);
            saveTerrain = true;
        }
    }

    /// <summary>
    /// Returns if the game is currently paused.
    /// </summary>
    public bool IsPaused { get { return state == State.Pause; } }

    /// <summary>
    /// Controls and returns the current visibility of the mouse cursor, which is not visible during standard gameplay.
    /// Only the GameManager can change this setting.
    /// </summary>
    private bool ShowCursor {
        get { return Cursor.visible; }
        set
        {
            Cursor.visible = value;
            Cursor.lockState = CursorLockMode.None;
            Cursor.lockState = value ? CursorLockMode.Confined : CursorLockMode.Locked;
        }
    }

    /// <summary>
    /// Public class for changing or reseting scenes.
    /// Also handles state changes based on the scene selected.
    /// Do not use to Pause or Unpause the game.
    /// </summary>
    /// <param name="scene">String name of the scene.</param>
    /// <param name="playerHasDied">Whether or not this is a scene reset due to player death. Default is false.</param>
    public void PlayScene(string scene, bool playerHasDied = false)
    {
        lock (padlock)
        {
            if (state == State.Pause) Unpause();
            if (scene == menuScene) { state = State.Menu; ShowCursor = true; }
            else { state = State.Play; ShowCursor = false; }
            Time.timeScale = 1;
            notifierImage.sprite = null;
            notifierImage.color = new Color(1, 1, 1, 0);
            saveTerrain = !playerHasDied;
            StartCoroutine(SceneLoader(scene));
        }
    }

    /// <summary>
    /// Asynchronous process to handle scene loading by covering the screen with a cover until the scene is ready.
    /// The cover has a simple gear animation to show the loading process.
    /// </summary>
    /// <param name="scene">The name of the scene being waited on.</param>
    /// <returns>null</returns>
    private IEnumerator SceneLoader(string scene)
    {
        CoverScreen.SetActive(true);
        AsyncOperation async = SceneManager.LoadSceneAsync(scene);
        while (!async.isDone) { yield return null; }
        CoverScreen.SetActive(false);
    }

    /// <summary>
    /// Public method for pausing the game. The pause screen is not a separate scene, but is apart of the Game Manager.
    /// Done by setting the time scaling to zero.
    /// </summary>
    /// <param name="newPauseState">The type of pause being initiated (Pause, Options, Transition, Info, Death).</param>
    /// <param name="newInfoLetter">If this pause is of type Info, this is the name of the letter being displayed. Default is an empty string.</param>
    public void Pause(PauseButtons.PauseState newPauseState = PauseButtons.PauseState.Pause, string newInfoLetter = "")
    {
        lock (padlock)
        {
            if (state == State.Pause) return;
            pauseState = newPauseState;
            Time.timeScale = 0;
            state = State.Pause;
            ShowCursor = true;
            currentInfoLetter = newInfoLetter;
            aud.Pause();
            PauseController.SetActive(true);
        }
    }

    /// <summary>
    /// Public method for unpausing the game and putting the game back in play.
    /// </summary>
    public void Unpause()
    {
        lock (padlock)
        {
            if (state != State.Pause) return;
            Time.timeScale = 1;
            state = State.Play;
            pauseState = PauseButtons.PauseState.Pause;
            ShowCursor = false;
            aud.UnPause();
            PauseController.SetActive(false);
        }
    }

    /// <summary>
    /// Returns the pause state, so other classes know the type of pause involved if the game is paused.
    /// </summary>
    public PauseButtons.PauseState PauseState { get { return pauseState; } }
    /// <summary>
    /// Returns the letter name if the game is in the Info pause screen.
    /// </summary>
    public string CurrentInfoLetter { get { return currentInfoLetter; } }
    /// <summary>
    /// Returns whether or not the current terrain should be saved for later.
    /// Currently this funcationlity is untested and not in use.
    /// </summary>
    public bool SaveTerrain { get { return saveTerrain; } }

    /// <summary>
    /// Public method for closing the game.
    /// This method works in the unity editor as well.
    /// </summary>
    public void ExitGame()
    {
        lock (padlock)
        {
            state = State.Exit;
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

    /// <summary>
    /// Public method for playing scene music.
    /// </summary>
    /// <param name="clip">Audio clip to be played on loop.</param>
    public void PlayMusic(AudioClip clip)
    {
        lock (padlock)
        {
            if (aud.clip != null && aud.clip.Equals(clip)) return;
            aud.Stop();
            aud.clip = clip;
            aud.loop = true;
            aud.Play();
        }
    }

    /// <summary>
    /// Stops the current scene music and removes the clip stored.
    /// </summary>
    public void StopMusic()
    {
        if (aud.clip != null)
        {
            aud.Stop();
            aud.clip = null;
        }
    }

    /// <summary>
    /// Displays a notification for a certain time in the bottom left corner of the screen.
    /// </summary>
    /// <param name="sprite">Image to be displayed as a notification.</param>
    public void ShowNotification(Sprite sprite)
    {
        notifierImage.sprite = sprite;
        StopAllCoroutines();
        StartCoroutine(FadeNotifier());
    }

    /// <summary>
    /// Coroutine for handling image fading of the Notifier.
    /// </summary>
    /// <returns>null</returns>
    private IEnumerator FadeNotifier()
    {
        float startTime = Time.time;
        notifierImage.color = new Color(1, 1, 1, 1);
        while (Time.time - startTime < notificationShowTime)
        {
            if (Time.time - startTime > notificationShowTime - notificationFadeTime)
                notifierImage.color = new Color(1, 1, 1, 1 - (Time.time - startTime - notificationShowTime + notificationFadeTime));
            yield return null;
        }
        notifierImage.color = new Color(1, 1, 1, 0);
    }

    private enum State { Load, Menu, Pause, Play, Exit }
}
