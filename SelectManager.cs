using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class Select
{
    public string comment;
    public string[] answers;
    
}
public class SelectManager : MonoBehaviour
{
    public static SelectManager instance;
    public int curSelectedNum;
    public int maxSelectedNum;
    public int lastSelectedNum;


    bool revealTextFlag;
    bool revealTextFlag_NPC;
    bool dialogueFlag;
    public bool canSkip;
    public bool canSkip2;
    public bool goSkip;
    
    WaitForSeconds wait250ms = new WaitForSeconds(0.25f);
    WaitForSeconds wait100ms = new WaitForSeconds(0.1f);
    
    void Awake()
    {
        instance = this;
    }
    void Start(){
        
    }
    public void ResetSelectUI(){
        for(int i=0;i<UIManager.instance.ui_select_grid.childCount;i++){
            UIManager.instance.ui_select_grid.GetChild(i).gameObject.SetActive(false);
            UIManager.instance.ui_select_grid.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        }
    }

    public void SetSelect(Select select){

        PlayerManager.LockPlayer();

        ResetSelectUI();

        for(int i=0;i<select.answers.Length;i++){
            UIManager.instance.ui_select_grid.GetChild(i).gameObject.SetActive(true);
            UIManager.instance.ui_select_grid.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = select.answers[i];
        }

        maxSelectedNum = select.answers.Length;

        HighlightSelectedAnswer(0);

        UIManager.instance.ui_select.SetActive(true);
        
        StartCoroutine(SelectSlotAnimationCoroutine(select));

        PlayerManager.instance.isSelecting = true;

    }
    IEnumerator SelectSlotAnimationCoroutine(Select select){
        for(int i=0;i<select.answers.Length;i++){
            UIManager.instance.ui_select_grid.GetChild(i).GetComponent<Animator>().SetTrigger("on");
            yield return wait100ms;
        }
    }

    public void HighlightSelectedAnswer(int selectedNum){

        for(int i=0;i<UIManager.instance.ui_select_grid.childCount;i++){
            UIManager.instance.ui_select_grid.GetChild(i).GetChild(1).GetComponent<Image>().sprite = UIManager.instance.non_selected_sprite;
            UIManager.instance.ui_select_grid.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().color = UIManager.instance.non_selected_color;
        }

        UIManager.instance.ui_select_grid.GetChild(selectedNum).GetChild(1).GetComponent<Image>().sprite = UIManager.instance.selected_sprite;
        UIManager.instance.ui_select_grid.GetChild(selectedNum).GetChild(0).GetComponent<TextMeshProUGUI>().color = UIManager.instance.selected_color;
    }
    public void ExitSelect(){
        PlayerManager.instance.isSelecting = false;

        lastSelectedNum = curSelectedNum;
        
        UIManager.instance.ui_select.SetActive(false);
        
        PlayerManager.UnlockPlayer();
    }

    void Update(){
        if(PlayerManager.instance.isSelecting){

            if(Input.GetButtonDown("Vertical")){
                if(Input.GetAxisRaw("Vertical")<0){
                    if(curSelectedNum < maxSelectedNum - 1) curSelectedNum ++;
                    else curSelectedNum = 0;
                    
                    HighlightSelectedAnswer(curSelectedNum);
                }
                else if(Input.GetAxisRaw("Vertical")>0){
                    if(curSelectedNum != 0) curSelectedNum --;
                    else curSelectedNum = maxSelectedNum - 1;

                    HighlightSelectedAnswer(curSelectedNum);
                }
            }


            if(Input.GetButtonDown("Interact")){
                ExitSelect();
            }




        }

    }
    public int GetSelect(){
        return lastSelectedNum;
    }
}   
