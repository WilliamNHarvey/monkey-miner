using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeFlash : MonoBehaviour {

    private SpriteRenderer sr = null; 

    // Use this for initialization
    void Start () {
        StartCoroutine(SmoothMovement());
        
    }

    protected IEnumerator SmoothMovement()
    {
        sr = GetComponent<SpriteRenderer>();
        for (int i = 20; i > 0; i--)
        {
            Color color = new Color(1, 1, 1, i * 0.05f);
            sr.color = color;
            yield return null;
        }
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
