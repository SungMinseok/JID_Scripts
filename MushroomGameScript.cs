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
    public Rigidbody2D mushroomItem;
    public ParticleSystem mushroomSpore; 

    void Awake(){
        instance = this;
    }
    void Start(){
        ResetPiano();

        if(DBManager.instance.CheckTrigOver(60)){
            
            animator_main.SetBool("down",true);
            
            for(int i=0; i<4;i++){
                animators_piano[i].SetBool("down",true);
            }
        }
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
                

                StartCoroutine(CanGetMushroom());
                //Invoke("CanGetMushroom",1f);
                

                SoundManager.instance.PlaySound("mushroom_success2");
                //InventoryManager.instance.AddItem(13);
                DBManager.instance.TrigOver(60);

                
                return;//성공 시 리셋되지 않음.
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
    IEnumerator CanGetMushroom(){
        PlayerManager.instance.LockPlayer();
        UIManager.instance.SetHUD(false);

        mushroomSpore.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
                SoundManager.instance.PlaySound("mushroom_popup_0"+Random.Range(1,6));
        
        mushroomItem.gameObject.SetActive(true);
        mushroomItem.AddForce(new Vector2(0,1) * (7), ForceMode2D.Impulse);
        mushroomItem.GetComponent<ItemScript>().isAvailable = true;
        mushroomItem.GetComponent<BoxCollider2D>().enabled = true;
        yield return new WaitForSeconds(3f);
        mushroomSpore.Stop();

        PlayerManager.instance.UnlockPlayer();
        UIManager.instance.SetHUD(true);

        //yield return new WaitUntil(()=>InventoryManager.instance.CheckHaveItem(13));
        //mushroomSpore.Stop();
        
    }
}
