using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LayUI : MonoBehaviour {

    public RectTransform UIT;
    public RectTransform UIB;
    public RectTransform UITR;
    public RectTransform UIBR;
    public RectTransform UITL;
    public RectTransform UIBL;

    void Start()
    {
        if (transform.childCount == 0)
        {
            InitUIBackground(0);
            InitUIBackground(-2);
        }
    }

    void InitUIBackground(float y)
    {
        RectTransform bUITR = Instantiate(UITR);
        bUITR.transform.SetParent(transform);
        bUITR.anchoredPosition = new Vector3(31.5f, y - 0.5f, 0);
        RectTransform bUIBR = Instantiate(UIBR);
        bUIBR.transform.SetParent(transform);
        bUIBR.anchoredPosition = new Vector3(31.5f, y - 1.5f, 0);
        RectTransform bUITL = Instantiate(UITL);
        bUITL.transform.SetParent(transform);
        bUITL.anchoredPosition = new Vector3(0.5f, y - 0.5f, 0);
        RectTransform bUIBL = Instantiate(UIBL);
        bUIBL.transform.SetParent(transform);
        bUIBL.anchoredPosition = new Vector3(0.5f, y - 1.5f, 0);
        for (int x = 1; x < 31; x++)
        {
            RectTransform bUIT = Instantiate(UIT);
            bUIT.transform.SetParent(transform);
            bUIT.anchoredPosition = new Vector3(0.5f + x, y - 0.5f, 0);
            RectTransform bUIB = Instantiate(UIB);
            bUIB.transform.SetParent(transform);
            bUIB.anchoredPosition = new Vector3(0.5f + x, y - 1.5f, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
