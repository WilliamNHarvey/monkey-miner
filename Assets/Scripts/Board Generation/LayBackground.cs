using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LayBackground : MonoBehaviour {

    public RectTransform backgroundBlock;

    void Start()
    {
        if(transform.childCount == 0)
        {
            for (int y = 0; y < 7; y++)
            {
                LayRowFloor(y);
            }

            for (int y = 11; y < 18; y++)
            {
                LayRowFloor(y);
            }
        }
        
    }

    void LayRowFloor(int y)
    {
        for (int x = 0; x < 32; x++)
        {
            Vector3 pos = new Vector3(0.5f + x, -0.5f - y, 0);
            RectTransform block = Instantiate(backgroundBlock);
            block.transform.SetParent(transform);
            block.anchoredPosition = pos;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
