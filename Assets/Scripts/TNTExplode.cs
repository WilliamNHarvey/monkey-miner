using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNTExplode : MonoBehaviour {

    public int beatsToExplode;
    public float gameBeatDelay;
    public RectTransform explodeSprite;
	// Use this for initialization
	void Start () {
        InvokeRepeating("CountDown", 0.0f, gameBeatDelay);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void CountDown(){
        if (beatsToExplode == 0){
            ExplodeTNT();
            //as a safety:
            beatsToExplode--;
        }
        else{
            beatsToExplode--;
        }
    }

    void ExplodeTNT()
    {
        bool upEx = false;
        bool downEx = false;
        bool leftEx = false;
        bool rightEx = false;
        bool onEx = false;
        Vector3 pos = transform.position;
        RectTransform rt = (RectTransform)transform;
        float x = rt.anchoredPosition.x;
        float y = rt.anchoredPosition.y;

        BlockProps blockProps = transform.parent.GetComponent<BlockProps>();
        string board = blockProps.board;

        RectTransform on = GameManager.instance.boardScript.GetBlock(x, y, board);
        RectTransform up1 = GameManager.instance.boardScript.GetBlock(x, y - 1,board);
        RectTransform up2 = GameManager.instance.boardScript.GetBlock(x, y - 2, board);
        RectTransform down1 = GameManager.instance.boardScript.GetBlock(x, y + 1, board);
        RectTransform down2 = GameManager.instance.boardScript.GetBlock(x, y + 2, board);
        RectTransform left1 = GameManager.instance.boardScript.GetBlock(x - 1, y, board);
        RectTransform left2 = GameManager.instance.boardScript.GetBlock(x - 2, y, board);
        RectTransform right1 = GameManager.instance.boardScript.GetBlock(x + 1, y, board);
        RectTransform right2 = GameManager.instance.boardScript.GetBlock(x + 2, y, board);
        RectTransform ul = GameManager.instance.boardScript.GetBlock(x - 1, y - 1, board);
        RectTransform ur = GameManager.instance.boardScript.GetBlock(x + 1, y - 1, board);
        RectTransform dl = GameManager.instance.boardScript.GetBlock(x - 1, y + 1, board);
        RectTransform dr = GameManager.instance.boardScript.GetBlock(x + 1, y + 1, board);

        DestroyBlock((RectTransform)transform, x, y);
        
        if (on == null || CanExplode(on))
        {
            DestroyBlock(on, x, y);
            onEx = true;
        }

        if (onEx)
        {
            if (up1== null || CanExplode(up1)) {
                DestroyBlock(up1, x, y - 1);
                upEx = true;
            }
            if (down1 == null || CanExplode(down1))
            {
                DestroyBlock(down1, x, y + 1);
                downEx = true;
            }
            if (left1 == null || CanExplode(left1))
            {
                DestroyBlock(left1, x - 1, y);
                leftEx = true;
            }
            if (right1 == null || CanExplode(right1))
            {
                DestroyBlock(right1, x + 1, y);
                rightEx = true;
            }

            if (upEx && (up2 == null || CanExplode(up2))) {
                DestroyBlock(up2, x, y - 2);
            }
            if (downEx && (down2 == null || CanExplode(down2)))
            {
                DestroyBlock(down2, x, y + 2);
            }
            if (leftEx && (left2 == null || CanExplode(left2)))
            {
                DestroyBlock(left2, x - 2, y);
            }
            if (rightEx && (right2 == null || CanExplode(right2)))
            {
                DestroyBlock(right2, x + 2, y);
            }

            if ((upEx || leftEx) && (ul == null || CanExplode(ul))) {
                DestroyBlock(ul, x - 1, y - 1);
            }
            if ((upEx || rightEx) && (ur == null || CanExplode(ur)))
            {
                DestroyBlock(ur, x + 1, y - 1);
            }
            if ((downEx || leftEx) && (dl == null || CanExplode(dl)))
            {
                DestroyBlock(dl, x - 1, y + 1);
            }
            if ((downEx || rightEx) && (dr == null || CanExplode(dr)))
            {
                DestroyBlock(dr, x + 1, y + 1);
            }
        }
        

    }

    bool CanExplode(RectTransform block) {
        if (block.tag == "WallSwitch" || block.tag == "Brick" || block.tag == "Rock" || block.tag == "Dirt" || block.tag == "Oil" || block.tag == "Dynamite")
        { return true; }
        else { return false; }
    }

    void DestroyBlock(RectTransform block,float bx,float by)
    {
        RectTransform explode = Instantiate(explodeSprite);
        explode.transform.SetParent(transform.parent);
        if (block != null)
        {
            explode.anchoredPosition = new Vector2(bx, by);
            BlockProps blockProps = transform.parent.GetComponent<BlockProps>();
            string board = blockProps.board;
            if (board == "top")
            {
                GameManager.instance.boardScript.topPanel.RemoveBlock(explode.anchoredPosition);
            }
            else
            {
                GameManager.instance.boardScript.bottomPanel.RemoveBlock(explode.anchoredPosition);
            }
            block.gameObject.SetActive(false);
        }
        else
        {
            explode.anchoredPosition = new Vector3(bx, by, 100f);
        }
    }
}
