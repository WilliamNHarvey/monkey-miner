using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LayWalls : MonoBehaviour
{

    public RectTransform wallBlock;

    void Start()
    {
        if (transform.childCount == 0)
        {
            for (int y = -1; y < 19; y++)
            {
                for (int x = -1; x < 32; x++)
                {
                    if (y == -1 || y == 18 || x == -1)
                    {
                        Vector3 pos = new Vector3(x + 0.5f, -0.5f - y, 0);
                        RectTransform block = Instantiate(wallBlock);
                        block.transform.SetParent(transform);
                        block.anchoredPosition = pos;
                    }
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
