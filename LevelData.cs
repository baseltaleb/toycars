using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections;
//TODO find references to objects on other scripts and move them here when it makes sense.
public class LevelData : MonoBehaviour
{
    [Header("Level Data")]
    public GameObject[] MenuTracks;
    public int[] NumberOfTracks;
    [Space]
    [SerializeField]
    private int numberOfCars;
    public int NumberOfCars { get; private set; }

    [Header("Car Data")]
    public GameObject CarPrefab;
    public Transform CarSpawnTransform;

    [Header("Scene Objects References")]
    public Camera Cam;
    public GameObject GridPrefab;

    [NonSerialized]
    public bool IsEditMode = true;
    public GameObject Grid { get; private set; }


    public void Awake()
    {
        if (World.LevelGroups == null)
            World.InitWorld();
        World.CurrentLevel = this;
        NumberOfCars = numberOfCars; // Get number of cars for this level from inspector.
        if (!Tools.CheckIfSceneLoaded("LevelTemplate"))
        {
            SceneManager.LoadScene("LevelTemplate", LoadSceneMode.Additive);
        }

    }
    IEnumerator LoadLevelTemplate()
    {
        SceneManager.LoadScene("LevelTemplate", LoadSceneMode.Additive);
        yield return 0;
    }
    public void Start()
    {
        InitLevel();
        InitManagers();
        Scene scene = SceneManager.GetActiveScene();
        EventManager.OnLevelStart(scene.buildIndex);
        EnterEditMode();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();
    }

    private void InitLevel()
    {
        Grid = Instantiate(GridPrefab);
        if (UIManager.UIReferences.TracksChoiceMenu)
            UIManager.UIReferences.TracksChoiceMenu.GetComponent<TrackChoiceMenu>().InitTracksMenu();
    }

    private void InitManagers()
    {
        CameraManager.Instance.Initialize();
        TracksManager.Instance.Initialize();
        UIManager.Instance.Initialize();
        CarManager.instance.Initialize();
    }

    public void OnDisable()
    {
        World.CurrentLevel = null;
    }

    #region Game edit/play modes
    public void EnterPlayMode()
    {
        if (StatsManager.Instance.LevelCarsCount > 0)
        {
            IsEditMode = false;
            EventManager.OnEnterPlayMode();
        }
        else
        {
            EventManager.OnNoMoreCars();
        }
    }

    public void EnterEditMode()
    {
        IsEditMode = true;
        EventManager.OnEnterEditMode();
    }

    public void SwitchMode()
    {
        if (IsEditMode)
        {
            if (TracksManager.Instance.TrackIsPlaceable)//Check if selected track is not intersecting with anything.
                EnterPlayMode();
        }
        else
        {
            EnterEditMode();
        }
    }
    #endregion

    public void WinLevel()
    {
        World.LoadNextLevel();
    }
}
