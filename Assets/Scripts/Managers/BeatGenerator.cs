using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatGenerator : MonoBehaviour {

    public float gameBeatDelay;
    public AudioClip beatSFX;
    public AudioClip swapSFX;
    public int beatCounter = 0;
    public int swapBeatCounter = 0;
    public RectTransform flashPrefab;
    public Player player1;
    public Player player2;
    public RectTransform Dirt;
    public RectTransform Brick;
    public RectTransform Rock;
    public RectTransform Iron;
    public RectTransform Oil;
    public RectTransform Dynamite;

    private RectTransform topBlock;
    private RectTransform bottomBlock;
    private RectTransform[] topSwapBlocks;
    private RectTransform[] bottomSwapBlocks;

    // Use this for initialization
    void Start () {
        topBlock = GameManager.instance.boardScript.topPanel.GetRandomBlockOnScreen();
        bottomBlock = GameManager.instance.boardScript.bottomPanel.GetRandomBlockOnScreen();
        InvokeRepeating("playBeat",0.0f, gameBeatDelay);
    }

    public void SwapForPowerups(RectTransform[] top, RectTransform[] bot)
    {
        topSwapBlocks = top;
        bottomSwapBlocks = bot;
        InvokeRepeating("swapBeat", 0.0f, gameBeatDelay);
    }
	
	// Update is called once per frame
	void Update () {
	}

    void swapBeat()
    {
        if (swapBeatCounter < 3)
        {
            swapBeatCounter++;

            for (int i = 0; i < topSwapBlocks.Length; i++)
            {
                RectTransform top = topSwapBlocks[i];
                RectTransform bottom = bottomSwapBlocks[i];
                if (top && top.gameObject.activeSelf && bottom && bottom.gameObject.activeSelf)
                {
                    RectTransform flashTop = Instantiate(flashPrefab);
                    flashTop.transform.SetParent(top);
                    flashTop.anchoredPosition = new Vector3(0.5f, -0.5f, 100f);
                    RectTransform flashBottom = Instantiate(flashPrefab);
                    flashBottom.transform.SetParent(bottom);
                    flashBottom.anchoredPosition = new Vector3(0.5f, -0.5f, 100f);
                }
            }
            
        }
        else
        {
            swapBeatCounter = 0;

            for (int i = 0; i < topSwapBlocks.Length; i++)
            {
                RectTransform top = topSwapBlocks[i];
                RectTransform bottom = bottomSwapBlocks[i];
                if (top && top.gameObject.activeSelf && bottom && bottom.gameObject.activeSelf)
                {
                    RectTransform topPrefab = GetPrefab(top);
                    RectTransform bottomPrefab = GetPrefab(bottom);

                    GameManager.instance.boardScript.topPanel.SetBlockLocation(top.anchoredPosition, bottomPrefab);
                    GameManager.instance.boardScript.bottomPanel.SetBlockLocation(bottom.anchoredPosition, topPrefab);

                    top.gameObject.SetActive(false);
                    bottom.gameObject.SetActive(false);
                }
            }
            CancelInvoke("swapBeat");
        }
    }

    void playBeat() {
        if (beatCounter < 3)
        {
            SoundManager.instance.PlaySingle(beatSFX);
            beatCounter++;
            player1.oilText.color = Color.white;
            player2.oilText.color = Color.white;

            RectTransform underPlayer1 = GameManager.instance.boardScript.GetBlock(player1.rt.anchoredPosition.x, player1.rt.anchoredPosition.y);
            RectTransform underPlayer2 = GameManager.instance.boardScript.GetBlock(player2.rt.anchoredPosition.x, player2.rt.anchoredPosition.y);
            if (underPlayer1 == topBlock)
            {
                topBlock = null;
            }
            else if (underPlayer2 == bottomBlock)
            {
                bottomBlock = null;
            }

            if (topBlock && topBlock.gameObject.activeSelf && bottomBlock && bottomBlock.gameObject.activeSelf) {
                RectTransform flashTop = Instantiate(flashPrefab);
                flashTop.transform.SetParent(topBlock);
                flashTop.anchoredPosition = new Vector3(0.5f, -0.5f, 100f);
                RectTransform flashBottom = Instantiate(flashPrefab);
                flashBottom.transform.SetParent(bottomBlock);
                flashBottom.anchoredPosition = new Vector3(0.5f, -0.5f, 100f);
            }
        }
        else {
            SoundManager.instance.PlaySingle(swapSFX);
            beatCounter = 0;
            player1.GainOil(1);
            player2.GainOil(1);

            if(topBlock && topBlock.gameObject.activeSelf && bottomBlock && bottomBlock.gameObject.activeSelf)
            {
                RectTransform topPrefab = GetPrefab(topBlock);
                RectTransform bottomPrefab = GetPrefab(bottomBlock);

                GameManager.instance.boardScript.topPanel.SetBlockLocation(topBlock.anchoredPosition, bottomPrefab);
                GameManager.instance.boardScript.bottomPanel.SetBlockLocation(bottomBlock.anchoredPosition, topPrefab);

                topBlock.gameObject.SetActive(false);
                bottomBlock.gameObject.SetActive(false);
            }
            topBlock = GameManager.instance.boardScript.topPanel.GetRandomBlockOnScreen();
            bottomBlock = GameManager.instance.boardScript.bottomPanel.GetRandomBlockOnScreen();
        }
    }

    RectTransform GetPrefab(RectTransform block)
    {
        if(block.tag == "Dirt")
        {
            return Dirt;
        }
        else if (block.tag == "Rock")
        {
            return Rock;
        }
        else if (block.tag == "Brick")
        {
            return Brick;
        }
        else if (block.tag == "Wall")
        {
            return Iron;
        }
        else if (block.tag == "Oil")
        {
            return Oil;
        }
        else if (block.tag == "Dynamite")
        {
            return Dynamite;
        }
        else
        {
            return null;
        }
    }
}
