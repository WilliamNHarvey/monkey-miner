using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public int player = 1;
    public KeyCode up = KeyCode.W;
    public KeyCode down = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;

    private Rigidbody2D rb2d;       //Store a reference to the Rigidbody2D component required to use 2D Physics.
    private RectTransform rt;

    // Use this for initialization
    void Start()
    {
        //Get and store a reference to the Rigidbody2D component so that we can access it.
        //rb2d = GetComponent<Rigidbody2D>();
        rt = (RectTransform)transform;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(up))
        {
            MovePlayer("up");
        }
        else if(Input.GetKeyDown(down))
        {
            MovePlayer("down");
        }
        else if (Input.GetKeyDown(left))
        {
            MovePlayer("left");
        }
        else if (Input.GetKeyDown(right))
        {
            MovePlayer("right");
        }
    }

    void MovePlayer(string direction) {
        switch (direction)
        {
            case "up":
                transform.Translate(Vector3.up * rt.localScale.y);
                break;
            case "down":
                transform.Translate(Vector3.down * rt.localScale.y);
                break;
            case "left":
                transform.Translate(Vector3.left * rt.localScale.x);
                break;
            case "right":
                transform.Translate(Vector3.right * rt.localScale.x);
                break;
            default:
                Debug.Log("WHat are you doing?");
                break;
        }
    }
}
