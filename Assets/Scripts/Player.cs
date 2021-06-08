using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
public class Player : MovingObject
{
    public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.
    public int pointsPerOil = 1;              //Number of points to add to player food points when picking up a food object.
    public int pointsPerDynamite = 1;              //Number of points to add to player food points when picking up a soda object.
    public int wallDamage = 1;                  //How much damage a player does to a wall when chopping it.
    public Text oilText;                       //UI Text to display current player food total.
    public Text dynamiteText;
    public Text levelText;
    public int level = 1;
    public GameObject blocks;
    public BoardManager boardManager;
    public KeyCode up = KeyCode.W;
    public KeyCode down = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public KeyCode setDynamite = KeyCode.LeftShift;
    public KeyCode throwDynamite = KeyCode.Z;
    public RectTransform Dynamite;
    public RectTransform Brick;
    public Sprite regularMonkey;
    public Sprite strongMonkey;
    public AudioClip moveSound1;                //1 of 2 Audio clips to play when player moves.
    public AudioClip moveSound2;                //2 of 2 Audio clips to play when player moves.
    public AudioClip eatSound1;                 //1 of 2 Audio clips to play when player collects a food object.
    public AudioClip eatSound2;                 //2 of 2 Audio clips to play when player collects a food object.
    public AudioClip drinkSound1;               //1 of 2 Audio clips to play when player collects a soda object.
    public AudioClip drinkSound2;               //2 of 2 Audio clips to play when player collects a soda object.
    public AudioClip gameOverSound;             //Audio clip to play when player dies.
    public double moveDelay = 2;

    private Animator animator;                  //Used to store a reference to the Player's animator component.
    private int oil;
    private int dynamite;
    private DateTime lastMove;
    public RectTransform rt;
    private double stunSeconds;
    private int horizontal = 0;     //Used to store the horizontal move direction.
    private int vertical = 0;       //Used to store the vertical move direction.
    private int setting = 0;
    private int throwing = 0;
    private SpriteRenderer sr;
    private int powerUpsCount = 0;
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        private Vector2 touchOrigin = -Vector2.one;	//Used to store location of screen touch origin for mobile controls.
#endif


    //Start overrides the Start function of MovingObject
    protected override void Start()
    {
        rt = (RectTransform)transform;
        lastMove = DateTime.Now;
        //Get a component reference to the Player's animator component
        animator = GetComponent<Animator>();

        //Get the current food point total stored in GameManager.instance between levels.
        oil = GameManager.instance.startingOil;
        dynamite = GameManager.instance.startingDynamite;
        stunSeconds = GameManager.instance.stunSeconds;
        sr = gameObject.GetComponent<SpriteRenderer>();
        //Set the foodText to reflect the current player food total.
        oilText.text = "Oil: " + oil;
        dynamiteText.text = "TNT: " + dynamite + "/3";
        levelText.text = "Level: " + level + "/" + GameManager.instance.screens;

        //Call the Start function of the MovingObject base class.
        base.Start();
    }

    protected override void UpdateLevel()
    {
        level++;
        levelText.text = "Level: " + level + "/" + GameManager.instance.screens;
        if (level == GameManager.instance.screens)
        {
            GameManager.instance.CreateFinalWalls(board);
        }
    }
    //This function is called when the behaviour becomes disabled or inactive.
    private void OnDisable()
    {
        //When Player object is disabled, store the current local food total in the GameManager so it can be re-loaded in next level.
       // GameManager.instance.playerFoodPoints = food;
    }


    private void Update()
    {
        horizontal = 0;     //Used to store the horizontal move direction.
        vertical = 0;       //Used to store the vertical move direction.
        setting = 0;
        throwing = 0;
        //Check if we are running either in the Unity editor or in a standalone build.
#if UNITY_STANDALONE || UNITY_WEBPLAYER
        if (Input.GetKey(setDynamite))
        {
            setting = 1;
        }
        else if (Input.GetKey(throwDynamite))
        {
            throwing = 1;
        }
        else if (Input.GetKey(up))
        {
            vertical = 1;
        }
        else if(Input.GetKey(down))
        {
            vertical = -1;
        }
        else if(Input.GetKey(right))
        {
            horizontal = 1;
        }
        else if(Input.GetKey(left))
        {
            horizontal = -1;
        }

        //Check if setting, don't throw
        if (setting != 0)
        {
            throwing = 0;
        }
        if (horizontal != 0)
        {
            vertical = 0;
        }
        //Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
			
			//Check if Input has registered more than zero touches
			if (Input.touchCount > 0)
			{
				//Store the first touch detected.
				Touch myTouch = Input.touches[0];
				
				//Check if the phase of that touch equals Began
				if (myTouch.phase == TouchPhase.Began)
				{
					//If so, set touchOrigin to the position of that touch
					touchOrigin = myTouch.position;
				}
				
				//If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
				else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
				{
					//Set touchEnd to equal the position of this touch
					Vector2 touchEnd = myTouch.position;
					
					//Calculate the difference between the beginning and end of the touch on the x axis.
					float x = touchEnd.x - touchOrigin.x;
					
					//Calculate the difference between the beginning and end of the touch on the y axis.
					float y = touchEnd.y - touchOrigin.y;
					
					//Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
					touchOrigin.x = -1;
					
					//Check if the difference along the x axis is greater than the difference along the y axis.
					if (Mathf.Abs(x) > Mathf.Abs(y))
						//If x is greater than zero, set horizontal to 1, otherwise set it to -1
						horizontal = x > 0 ? 1 : -1;
					else
						//If y is greater than zero, set horizontal to 1, otherwise set it to -1
						vertical = y > 0 ? 1 : -1;
				}
			}
			
#endif //End of mobile platform dependendent compilation section started above with #elif
        //Check if we have a non-zero value for horizontal or vertical
        double secondsSinceLastMove = (DateTime.Now - lastMove).TotalSeconds;
        if(secondsSinceLastMove >= moveDelay)
        {
            if (dynamite > 0 && setting == 1)
            {
                lastMove = DateTime.Now;
                PlaceDynamite();
            }
            else if(dynamite > 0 && throwing == 1)
            {
                lastMove = DateTime.Now;
                ThrowDynamite();
            }
            else if (horizontal != 0 || vertical != 0)
            {
                //Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
                //Pass in horizontal and vertical as parameters to specify the direction to move Player in.
                lastMove = DateTime.Now;
                AttemptMove<Breakable>(horizontal, vertical);

            }
        }
        
    }

    //AttemptMove overrides the AttemptMove function in the base class MovingObject
    //AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        //Every time player moves, subtract from food points total.
        //food--;

        //Update food text display to reflect current score.
        //foodText.text = "Food: " + food;

        //Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
        base.AttemptMove<T>(xDir, yDir);

        //Hit allows us to reference the result of the Linecast done in Move.
        

        //If Move returns true, meaning Player was able to move into an empty space.
        //if (Move(xDir, yDir, out hit))
        //{
            //Debug.Log(hit);
            //Call RandomizeSfx of SoundManager to play the move sound, passing in two audio clips to choose from.
            //SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        //}

        //Since the player has moved and lost food points, check if the game has ended.
        //CheckIfGameOver();

        //Set the playersTurn boolean of GameManager to false now that players turn is over.
        //GameManager.instance.playersTurn = false;
    }


    //OnCantMove overrides the abstract function OnCantMove in MovingObject.
    //It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
    protected override void OnCantMove<T>(T component)
    {
        //Set hitWall to equal the component passed in as a parameter.
        Breakable hitWall = component as Breakable;

        //Call the DamageWall function of the Wall we are hitting.
        if(hitWall.costsToDamage == "none")
        {
            hitWall.DamageWall(wallDamage);
        }
        else if(hitWall.costsToDamage == "oil")
        {
            if(oil >= 1)
            {
                LoseOil(1);
                hitWall.DamageWall(wallDamage);
            }
        }



        //Set the attack trigger of the player's animation controller in order to play the player's attack animation.

        /* SET ANIMATION */
        int direction = 1;
        if(horizontal == 1)
        {
            direction = 2;
        }
        else if (vertical == -1)
        {
            direction = 3;
        }
        else if (horizontal == -1)
        {
            direction = 4;
        }
        //animator.SetInteger("direction", direction);
        //animator.SetTrigger("mine");
    }

    private void PlaceDynamite()
    {
        RectTransform tnt = Instantiate(Dynamite);
        tnt.transform.SetParent(blocks.transform);
        Vector2 pos;
        if (rt.anchoredPosition.y > -8)
        {
            pos = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y + 7);
        }
        else
        {
            pos = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y + 18);
        }
        tnt.anchoredPosition = pos;
        LoseDynamite(1);
    }

    private void ThrowDynamite()
    {
        RectTransform tnt = Instantiate(Dynamite);
        GameObject newParent;
        Vector2 start;
        if (board == "top")
        {
            newParent = GameManager.instance.boardScript.bottomPlayer.blocks;
            start = new Vector2(0, 9);
        }
        else
        {
            newParent = GameManager.instance.boardScript.topPlayer.blocks;
            start = new Vector2(0, -9);
        }

        tnt.transform.SetParent(newParent.transform);
        Vector2 pos;
        if (rt.anchoredPosition.y > -8)
        {
            pos = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y + 7);
        }
        else
        {
            pos = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y + 18);
        }
        tnt.anchoredPosition = pos + start;
        StartCoroutine(DynamiteMove(tnt, pos, tnt.anchoredPosition));
        
        LoseDynamite(1);
    }

    protected IEnumerator DynamiteMove(RectTransform rt, Vector2 end, Vector2 start)
    {
        //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
        //Square magnitude is used instead of magnitude because it's computationally cheaper
        float sqrRemainingDistance = (float)(Math.Pow(rt.anchoredPosition.x - end.x, 2.0) + Math.Pow(rt.anchoredPosition.y - end.y, 2.0));
        float totalDistance = (float)(Math.Pow(start.x - end.x, 2.0) + Math.Pow(start.y - end.y, 2.0));

        //While that distance is greater than a very small amount (Epsilon, almost zero):
        while (sqrRemainingDistance > float.Epsilon)
        {
            //Find a new position proportionally closer to the end, based on the moveTime
            Vector2 newPosition = Vector2.MoveTowards(rt.anchoredPosition, end, 1f / moveTime / 2f * Time.deltaTime);
            if(totalDistance / 4 > sqrRemainingDistance)
            {
                rt.localScale = new Vector3(rt.localScale.x - 0.05f, rt.localScale.y - 0.05f, rt.localScale.z);
            }
            else
            {
                rt.localScale = new Vector3(rt.localScale.x + 0.05f, rt.localScale.y + 0.05f, rt.localScale.z);
            }

            //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
            rt.anchoredPosition = newPosition;
            sqrRemainingDistance = (float)(Math.Pow(rt.anchoredPosition.x - end.x, 2.0) + Math.Pow(rt.anchoredPosition.y - end.y, 2.0));

            //Return and loop until sqrRemainingDistance is close enough to zero to end the function
            yield return null;
        }
    }

    //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Check if the tag of the trigger collided with is Exit.
        if (other.tag == "Goal")
        {
            GameManager.instance.GameOver(board);

            //Disable the player object since level is over. --Is handled in GameOver()
            //enabled = false;
        }

        else if (other.tag == "Oil")
        {
            oil = 20;
            oilText.text = "Oil: " + oil;
            //Call the RandomizeSfx function of SoundManager and pass in two eating sounds to choose between to play the eating sound effect.
            //SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);

            //Disable the food object the player collided with.
            Vector2 pos = other.transform.GetComponent<RectTransform>().anchoredPosition;
            if (board == "top")
            {
                GameManager.instance.boardScript.topPanel.RemoveBlock(pos);
            }
            else
            {
                GameManager.instance.boardScript.bottomPanel.RemoveBlock(pos);
            }
            other.gameObject.SetActive(false);
        }

        else if (other.tag == "Dynamite")
        {
            if (dynamite <= 2)
            {
                //Add pointsPerSoda to players food points total
                dynamite += pointsPerDynamite;

                //Update foodText to represent current total and notify player that they gained points
                dynamiteText.text = "TNT: " + dynamite + "/3";

                //Call the RandomizeSfx function of SoundManager and pass in two drinking sounds to choose between to play the drinking sound effect.
                //SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);

                //Disable the soda object the player collided with.
                Vector2 pos = other.transform.GetComponent<RectTransform>().anchoredPosition;
                if (board == "top")
                {
                    GameManager.instance.boardScript.topPanel.RemoveBlock(pos);
                }
                else
                {
                    GameManager.instance.boardScript.bottomPanel.RemoveBlock(pos);
                }
                other.gameObject.SetActive(false);
            }
        }

        else if (other.tag == "Explosion")
        {
            Stun(stunSeconds - moveDelay);
        }

        else if (other.tag == "WallSwitch")
        {
            Vector2 pos = other.transform.GetComponent<RectTransform>().anchoredPosition;
            PutUpOpponentWall(pos);
            if (board == "top")
            {
                GameManager.instance.boardScript.topPanel.RemoveBlock(pos);
            }
            else
            {
                GameManager.instance.boardScript.bottomPanel.RemoveBlock(pos);
            }
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Banana")
        {
            Vector2 pos = other.transform.GetComponent<RectTransform>().anchoredPosition;
            if (board == "top")
            {
                GameManager.instance.boardScript.topPanel.RemoveBlock(pos);
            }
            else
            {
                GameManager.instance.boardScript.bottomPanel.RemoveBlock(pos);
            }
            other.gameObject.SetActive(false);
            IncreaseDamage(5, 10);
        }
        else if (other.tag == "IronToDirt")
        {
            Vector2 pos = other.transform.GetComponent<RectTransform>().anchoredPosition;
            RectTransform[] topBlocks;
            RectTransform[] botBlocks;
            if (board == "top")
            {
                topBlocks = GameManager.instance.boardScript.topPanel.GetRandomOfType("Wall", 3);
                botBlocks = GameManager.instance.boardScript.bottomPanel.GetRandomOfType("Dirt", 3);
            }
            else
            {
                topBlocks = GameManager.instance.boardScript.topPanel.GetRandomOfType("Dirt", 3);
                botBlocks = GameManager.instance.boardScript.bottomPanel.GetRandomOfType("Wall", 3);
            }
            GameManager.instance.beatScript.SwapForPowerups(topBlocks, botBlocks);
            if (board == "top")
            {
                GameManager.instance.boardScript.topPanel.RemoveBlock(pos);
            }
            else
            {
                GameManager.instance.boardScript.bottomPanel.RemoveBlock(pos);
            }
            other.gameObject.SetActive(false);
        }
    }

    public Vector2 MonkeyToLucas(Vector2 pos)
    {
        if (pos.y > -8)
        {
            return new Vector2(pos.x, pos.y + 7);
        }
        else
        {
           return new Vector2(pos.x, pos.y + 18);
        }
    }

    public void IncreaseDamage(int power, double seconds)
    {
        powerUpsCount++;
        wallDamage = power;
        sr.sprite = strongMonkey;
        StartCoroutine(UnIncreaseDamage(power, seconds));
    }
    public IEnumerator UnIncreaseDamage(int power, double delay)
    {
        yield return new WaitForSeconds((float)delay);
        powerUpsCount--;
        if(powerUpsCount == 0)
        {
            wallDamage = 1;
            sr.sprite = regularMonkey;
        }
        yield return null;
    }


    private void PutUpOpponentWall(Vector2 pos)
    {
        if (board == "top")
        {
            GameManager.instance.boardScript.bottomPanel.PlaceWall(pos, 3, Brick);
        }
        else
        {
            Debug.Log("place");
            GameManager.instance.boardScript.topPanel.PlaceWall(pos, 3, Brick);
        }
    }
    //Restart reloads the scene when called.
    private void Restart()
    {
        //Load the last scene loaded, in this case Main, the only scene in the game. And we load it in "Single" mode so it replace the existing one
        //and not load all the scene object in the current scene.
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }


    //LoseFood is called when an enemy attacks the player.
    //It takes a parameter loss which specifies how many points to lose.
    public void LoseOil(int loss)
    {
        

        //Subtract lost food points from the players total.
        oil -= loss;

        //Update the food display with the new total.
        oilText.text = "Oil: " + oil;

        //Check to see if game has ended.
        //CheckIfGameOver();
    }

    public void LoseDynamite(int loss)
    {
       

        //Subtract lost food points from the players total.
        dynamite -= loss;

        //Update the food display with the new total.
        dynamiteText.text = "TNT: " + dynamite + "/3";

        //Check to see if game has ended.
        //CheckIfGameOver();
    }


    //CheckIfGameOver checks if the player is out of food points and if so, ends the game.
    private void CheckIfGameOver()
    {
        //Check if food point total is less than or equal to zero.
        if (oil <= 0)
        {
            //Call the PlaySingle function of SoundManager and pass it the gameOverSound as the audio clip to play.
            SoundManager.instance.PlaySingle(gameOverSound);

            //Stop the background music.
            SoundManager.instance.musicSource.Stop();

            //Call the GameOver function of GameManager.
            //GameManager.instance.GameOver();
        }
    }

    public void GainOil(int amount)
    {
        if(oil == 20)
        {
            return;
        }
        else if (oil + amount > 20)
        {
            oil = 20;
            oilText.color = Color.green;
        }
        else
        {
            oil += amount;
            oilText.color = Color.green;
        }
        oilText.text = "Oil: " + oil;
    }

    public void Stun(double seconds)
    {
        lastMove = DateTime.Now.AddSeconds(seconds);
        rt.localScale = new Vector3(rt.localScale.x, - rt.localScale.y, 1);
        StartCoroutine(Unstun(seconds));
    }
    public IEnumerator Unstun(double delay)
    {
        yield return new WaitForSeconds((float)delay);
        rt.localScale = new Vector3(rt.localScale.x, -rt.localScale.y, 1);
        yield return null;
    }
}