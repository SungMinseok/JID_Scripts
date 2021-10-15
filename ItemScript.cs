using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType{
    Honey,
    Dirt,
    Item,
}
public class ItemScript : MonoBehaviour
{
    public ItemType type;
    //[Header("Honey")]
    public float amount;
    public int itemID;
    public Dialogue getItemDialogue;
    //[Header("Dirt")]
    //public float dirtAmount;
    Animator animator;
    //public BoxCollider2D itemCol;
    //Vector2 itemVector;
    WaitUntil waitTalking = new WaitUntil(()=>!PlayerManager.instance.isTalking);
    bool getFlag;

    void Start(){
        if(GetComponent<Animator>()!=null)
            animator= GetComponent<Animator>();
        //itemCol = GetComponent<BoxCollider2D>();
        //itemVector = transform.GetChild(0).localPosition;
    }

    void OnTriggerStay2D(Collider2D other) {
        
        if(other.CompareTag("Player")){
            if(!getFlag) {
                getFlag = true;

                StartCoroutine(GetItemCoroutine());

            }
        }

    }
    IEnumerator GetItemCoroutine(){

        if(getItemDialogue != null){

            DialogueManager.instance.SetDialogue(getItemDialogue);
            yield return waitTalking;
        }


        if(type == ItemType.Honey){
            StartCoroutine(GetItemAndRemoveCoroutine());
            DM("꿀 충전 : "+amount);
            PlayerManager.instance.curHoneyAmount+=amount;
        }
        else if(type == ItemType.Dirt){
            StartCoroutine(GetItemAndRemoveCoroutine());
            DM("흙 충전 : "+amount);
            
            PlayerManager.instance.curDirtAmount+=amount;
            if(PlayerManager.instance.curDirtAmount>PlayerManager.instance.maxDirtAmount){
                PlayerManager.instance.curDirtAmount=PlayerManager.instance.maxDirtAmount;
            }
        }
        else if(type == ItemType.Item){
            StartCoroutine(GetItemAndRemoveCoroutine());
            DM(itemID+"번 아이템 "+amount+"개 획득");
            
            InventoryManager.instance.AddItem(itemID);
        }




    }

    IEnumerator GetItemAndRemoveCoroutine(){
        yield return null;
        // animator.SetTrigger("got");
        // while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.8)
        // {
        //     yield return null;
        // }
        gameObject.SetActive(false);
    }
    public void DM(string msg) => DebugManager.instance.PrintDebug(msg);
}
