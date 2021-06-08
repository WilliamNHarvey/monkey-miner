using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(SmoothMovement());
    }

    protected IEnumerator SmoothMovement()
    {
        for (int i = 0; i < 20; i++)
        {
            transform.Rotate(0, 0, i);
            yield return null;
        }
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
