using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomGameScript : MonoBehaviour
{
    public static MushroomGameScript instance;
    public string correctOrder;
    public Animator[] animators_piano;
    public Animator animator_main;
    public string curOrder;

    void Awake(){
        instance = this;
    }
    void Start(){
        ResetPiano();
    }
    public void PushPiano(int num){
        if(!animators_piano[num].GetBool("down")){
            SoundManager.instance.PlaySound("mushroom_push_0"+Random.Range(1,4));
            animators_piano[num].SetBool("down",true);
            curOrder += num.ToString();
        }
    }
    public void PushMain(){
        if(!animator_main.GetBool("down")){

            animator_main.SetBool("down",true);

            if(curOrder == correctOrder){
                SoundManager.instance.PlaySound("mushroom_success2");
                InventoryManager.instance.AddItem(13);
            }
            
            StartCoroutine(PushMainCoroutine());
        }
    }
    IEnumerator PushMainCoroutine(){
        yield return new WaitForSeconds(1f);
        ResetPiano();

        
        animator_main.SetBool("down",false);
    }
    void ResetPiano(){
        
        curOrder = "";
        for(int i=0; i<4;i++){
            animators_piano[i].SetBool("down",false);
            if(DBManager.instance.curData.curMapNum == 14)
                SoundManager.instance.PlaySound("mushroom_popup_0"+Random.Range(1,6));

        }
    }
}
