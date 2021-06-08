using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour {
    public AudioClip chopSound1;                //1 of 2 audio clips that play when the wall is attacked by the player.
    public AudioClip chopSound2;                //2 of 2 audio clips that play when the wall is attacked by the player.
    public Sprite dmgSprite;                    //Alternate sprite to display after Wall has been attacked by player.
    public int hp = 1;                          //hit points for the wall.
    public string costsToDamage = "none";


    private SpriteRenderer spriteRenderer;      //Store a component reference to the attached SpriteRenderer.
    public RectTransform destroySprite;
    public RectTransform hp1DamageSprite;
    public RectTransform hp2DamageSprite;
    public RectTransform hp3DamageSprite;
    public RectTransform hp4DamageSprite;
    public RectTransform hp5DamageSprite;

    public RectTransform powerupBanana;
    public RectTransform powerupIronToDirt;


    void Awake()
    {
        //Get a component reference to the SpriteRenderer.
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    //DamageWall is called when the player attacks a wall.
    public void DamageWall(int loss)
    {
        //Call the RandomizeSfx function of SoundManager to play one of two chop sounds.
        //SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);

        //Set spriteRenderer to the damaged wall sprite.
        //spriteRenderer.sprite = dmgSprite;
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        //Subtract loss from hit point total.
        hp -= loss;

        RectTransform damage = null;
        bool instantiate = false;
        if (hp == 1 && hp1DamageSprite != null) { damage = Instantiate(hp1DamageSprite); instantiate = true; }
        else if (hp == 2 && hp2DamageSprite != null) { damage = Instantiate(hp2DamageSprite); instantiate = true; }
        else if (hp == 3 && hp3DamageSprite != null) { damage = Instantiate(hp3DamageSprite); instantiate = true; }
        else if (hp == 4 && hp4DamageSprite != null) { damage = Instantiate(hp4DamageSprite); instantiate = true; }
        if (instantiate == true)
        {
            damage.transform.SetParent(transform);
            damage.anchoredPosition = new Vector3(0.5f, -0.5f, 100f);
        }


        //If hit points are less than or equal to zero:
        if (hp <= 0) {
            //Disable the gameObject.         
            RectTransform rt = (RectTransform)transform;
            RectTransform destroy = Instantiate(destroySprite);
            destroy.transform.SetParent(transform.parent);
            destroy.anchoredPosition = new Vector3(rt.anchoredPosition.x, rt.anchoredPosition.y, 100f);

            BlockProps blockProps = transform.parent.GetComponent<BlockProps>();
            if (blockProps.board == "top")
            {
                GameManager.instance.boardScript.topPanel.RemoveBlock(new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y));
            }
            else
            {
                GameManager.instance.boardScript.bottomPanel.RemoveBlock(new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y));
            }

            if (rt.tag == "Crate") {
                SpawnPowerup(rt);
            }
            
            gameObject.SetActive(false); 
        }
    }

    void SpawnPowerup(RectTransform crate) {

        RectTransform chosenPowerup = ChooseRandomPowerup();
        BlockProps blockProps = transform.parent.GetComponent<BlockProps>();
        if (blockProps.board == "top")
        {
            GameManager.instance.boardScript.topPanel.blocks.Add(new Vector2(crate.anchoredPosition.x, crate.anchoredPosition.y), chosenPowerup);
        }
        else
        {
            GameManager.instance.boardScript.bottomPanel.blocks.Add(new Vector2(crate.anchoredPosition.x, crate.anchoredPosition.y), chosenPowerup);
        }
        
        RectTransform powerup = Instantiate(chosenPowerup);
        powerup.transform.SetParent(transform.parent);
        powerup.anchoredPosition = new Vector3(crate.anchoredPosition.x, crate.anchoredPosition.y, 100f);
    }

    RectTransform ChooseRandomPowerup() {
        int rand = Random.Range(1, 4);
        switch (rand)
        {
            case 1:
                return powerupBanana;
            case 2:
                return powerupBanana;
            case 3:
                return powerupIronToDirt;
            default:
                return powerupBanana;
        }
    }
}
