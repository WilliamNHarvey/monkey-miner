using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;					//Allows us to use UI.

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;                      //Time to wait before starting level, in seconds.
    public float turnDelay = 0.1f;                          //Delay between each Player turn.
    public int startingOil = 20;                      //Starting value for Player food points.
    public int startingDynamite = 0;
    public double stunSeconds = 5;
    public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    public int screens = 4;


    private Text levelText;                                 //Text to display current level number.
    private GameObject levelImage;                          //Image to block out level as levels are being set up, background for levelText.
    public BoardManager boardScript;                       //Store a reference to our BoardManager which will set up the level.
    public BeatGenerator beatScript;
    //private int level = 1;                                  //Current level number, expressed in game as "Day 1".=
    private bool doingSetup = true;                         //Boolean to check if we're setting up board, prevent Player from moving during setup.

    public RectTransform wallBlock;
    public RectTransform topFinalWallParent;
    public RectTransform bottomFinalWallParent;

    public RectTransform victoryFade;
    public RectTransform redWinText;
    public RectTransform blueWinText;


    
    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        //Get a component reference to the attached BoardManager script
        boardScript = GetComponent<BoardManager>();
        beatScript = GetComponent<BeatGenerator>();

        //Call the InitGame function to initialize the first level 
        InitGame();
    }

    //this is called only once, and the paramter tell it to be called only after the scene was loaded
    //(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        //register the callback to be called everytime the scene is loaded
        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //This is called each time a scene is loaded.
    //static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    //{
    //    instance.level++;
    //    instance.InitGame();
    //}


    //Initializes the game for each level.
    void InitGame()
    {
        victoryFade.gameObject.SetActive(false);
        redWinText.gameObject.SetActive(false);
        blueWinText.gameObject.SetActive(false);
        //While doingSetup is true the player can't move, prevent player from moving while title card is up.
        /*doingSetup = true;

        //Get a reference to our image LevelImage by finding it by name.
        levelImage = GameObject.Find("LevelImage");

        //Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
        levelText = GameObject.Find("LevelText").GetComponent<Text>();

        //Set the text of levelText to the string "Day" and append the current level number.
        levelText.text = "Day " + level;

        //Set levelImage to active blocking player's view of the game board during setup.
        levelImage.SetActive(true);

        //Call the HideLevelImage function with a delay in seconds of levelStartDelay.
        Invoke("HideLevelImage", levelStartDelay);*/



        //Call the SetupScene function of the BoardManager script, pass it current level number.
        boardScript.Setup(4);

    }


    //Hides black image used between levels
    void HideLevelImage()
    {
        //Disable the levelImage gameObject.
        levelImage.SetActive(false);

        //Set doingSetup to false allowing player to move again.
        doingSetup = false;
    }

    //Update is called every frame.
    void Update()
    {
        //Check that playersTurn or enemiesMoving or doingSetup are not currently true.
        if (doingSetup)

            //If any of these are true, return and do not start MoveEnemies.
            return;
    }

    //player should be "top" or "bottom"
    public void ShiftBoard(string board)
    {
        boardScript.ShiftPanel(board);
    }

    public void CreateFinalWalls(string board)
    {
        RectTransform parent;
        if (board.Equals("top"))
        {
            parent = topFinalWallParent;
        }
        else
        {
            parent = bottomFinalWallParent;
        }
        for (float y = 0.5f; y < 8.5f; y++) //Lucas coords
        {
            Vector3 pos = new Vector3(32.5f, y, 0);
            RectTransform block = Instantiate(wallBlock);
            block.transform.SetParent(parent);
            block.anchoredPosition = pos;
        }
    }

    //board is "top" or "bottom"
    public void GameOver(string board)
    {
        victoryFade.gameObject.SetActive(true);
        if(board.Equals("top"))
        {
            redWinText.gameObject.SetActive(true);
        }
        else
        {
            blueWinText.gameObject.SetActive(true);
        }

        boardScript.topPlayer.enabled = false;
        boardScript.bottomPlayer.enabled = false;

        enabled = false;
    }

}
