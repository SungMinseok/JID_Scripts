using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    [Header("UI_Menu_Main")]
    public Color defaultBtnColor;
    public Color activatedBtnColor;
    public GameObject collectionPanel;
    public GameObject antCollectionPanel;
    public GameObject endingCollectionPanel;
    [Header("UI_Collection_Ending")]
    public Animator animator;
    public TextMeshProUGUI collectionSubText0;
    public TextMeshProUGUI collectionNameText;
    public Image[] collectionCardImages;
    public Sprite[] collectionCardSprites;
    public Button[] collectionScrollArrows;
    public float scrollTime;
    [Header("Debug────────────────────")]
    public int totalPage;
    public int curPage;
    public int[] tempCardNum = new int[5];

    void Awake(){
        instance = this;
    }
    void Start(){
        ResetCardOrder();
    }
    // void Update(){
    //     if(animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Collection_Scroll_Right") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime>=1f){
    //         RearrangeCardOrder();
    //         Debug.Log("33");
    //     }
    // }

    public void CollectionScrollRightBtn(){
        DeactivateBtns(collectionScrollArrows);
        if(curPage+1 == totalPage){
            curPage = 0;
        }
        else{
            curPage += 1;
        }
        RearrangeCardOrder();
        animator.SetTrigger("right");
    }
    
    public void CollectionScrollLeftBtn(){
        DeactivateBtns(collectionScrollArrows);
        if(curPage == 0){
            curPage = totalPage-1;
        }
        else{
            curPage -= 1;
        }
        RearrangeCardOrder();
        animator.SetTrigger("left");
    }
    public void ResetCardOrder(){
        curPage = 0;
        RearrangeCardOrder();
        RearrangeCardImages();
    }
    public void RearrangeCardOrder(){
        tempCardNum = new int[]{curPage-2,curPage-1,curPage,curPage+1,curPage+2};
        if(tempCardNum[0]==-2) tempCardNum[0]=totalPage-2;
        else if(tempCardNum[0]==-1) tempCardNum[0]=totalPage-1;
        if(tempCardNum[1]==-1) tempCardNum[1]=totalPage-1;
        if(tempCardNum[3]==totalPage) tempCardNum[3]=0;
        if(tempCardNum[4]==totalPage+1) tempCardNum[4]=1;
        else if(tempCardNum[4]==totalPage) tempCardNum[4]=0;
        Debug.Log(tempCardNum[0]+","+tempCardNum[1]+","+tempCardNum[2]+","+tempCardNum[3]+","+tempCardNum[4]);
    }
    public void RearrangeCardImages(){
        for(int i=0;i<5;i++){
            collectionCardImages[i].sprite = collectionCardSprites[tempCardNum[i]];
        }
        ActivateBtns(collectionScrollArrows);
    }
    public void ActivateBtns(Button[] btns){
        for(int i=0; i<btns.Length;i++){
            btns[i].interactable = true;
        }
    }
    public void DeactivateBtns(Button[] btns){
        for(int i=0; i<btns.Length;i++){
            btns[i].interactable = false;
        }
    }
}
