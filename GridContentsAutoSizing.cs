using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridContentsAutoSizing : MonoBehaviour
{
    RectTransform rectTransform;
    GridLayoutGroup gridLayoutGroup;
    int childCount;
    void Start(){

        rectTransform = GetComponent<RectTransform>();
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        //childCount = transform.childCount;
    }
    public void AutoHeightSize(int multiple){
        Debug.Log("multiple : "+multiple);
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x
        ,multiple * (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y));
    }
}
