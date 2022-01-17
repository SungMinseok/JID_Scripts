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
    [Header("Honey ────────────────────")]
    public float amount_honey;
    [Header("Dirt ────────────────────")]
    public float amount_dirt;
    [Header("Item ────────────────────")]
    public float amount_item;
    public int itemID;
    //public string getItemDefaultDialogue = "4";
    [Header("Dialogue ────────────────────")]
    //을(를) 얻었다! 사용.
    public bool useDialogue = true;
    public bool isDefaultDialogue = true;
    public Dialogue getItemDialogue;
    //[Header("Dirt")]
    //public float dirtAmount;
    Animator animator;
    //public BoxCollider2D itemCol;
    //Vector2 itemVector;
    WaitUntil waitTalking = new WaitUntil(()=>!PlayerManager.instance.isTalking);
    bool getFlag;
    public string getItemDefaultDialogue = "4";
    [Space]
    public Transform itemObject;
    void Start(){
        if(type == ItemType.Dirt){
            itemObject = this.transform;
        }
        else{
            itemObject = transform.GetChild(0);
        }

        if(GetComponent<Animator>()!=null)
            animator= itemObject.GetComponent<Animator>();


        switch(type){
            case ItemType.Honey :
                isDefaultDialogue = false;
                itemObject.GetComponent<SpriteRenderer>().sprite = DBManager.instance.honeySprite;

                break;


            case ItemType.Item :
                
                itemObject.GetComponent<SpriteRenderer>().sprite = DBManager.instance.cache_ItemDataList[itemID].icon;

                if(isDefaultDialogue){


                    getItemDefaultDialogue = CSVReader.instance.GetIndexToString(int.Parse(getItemDefaultDialogue),"sysmsg");

                    if(DBManager.instance.language == "kr"){

                        getItemDialogue.sentences[0] = DBManager.instance.cache_ItemDataList[itemID].name + getItemDefaultDialogue;
                    }
                }

                break;
        }


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
//아이템 습득 시 대화
        if(useDialogue){
            PlayerManager.instance.LockPlayer();
            DialogueManager.instance.SetDialogue(getItemDialogue);
            yield return waitTalking;
            PlayerManager.instance.UnlockPlayer();
        }

        if(type == ItemType.Honey){
            StartCoroutine(GetItemAndRemoveCoroutine());
            //DM("꿀 충전 : "+amount_honey);
            DBManager.instance.curData.curHoneyAmount+=amount_honey;
        }
        else if(type == ItemType.Dirt){
            StartCoroutine(GetItemAndRemoveCoroutine());
            
            InventoryManager.instance.AddItem(5,3);

            //DM("흙 충전 : "+amount_dirt);
            
            // DBManager.instance.curData.curDirtAmount+=amount_dirt;
            // if(DBManager.instance.curData.curDirtAmount>DBManager.instance.maxDirtAmount){
            //     DBManager.instance.curData.curDirtAmount=DBManager.instance.maxDirtAmount;
            // }
        }
        else if(type == ItemType.Item){
            StartCoroutine(GetItemAndRemoveCoroutine());
            DM(itemID+"번 아이템 "+amount_item+"개 획득");
            
            InventoryManager.instance.AddItem(itemID);
        }


        // if(useDialogue){
        //     //WaitUntil waitTalking = new WaitUntil(()=>!PlayerManager.instance.isTalking);
        // }

    }

    IEnumerator GetItemAndRemoveCoroutine(){
        yield return null;
        // animator.SetTrigger("got");
        // while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.8)
        // {
        //     yield return null;
        // }
        itemObject.gameObject.SetActive(false);
    }
    public void DM(string msg) => DebugManager.instance.PrintDebug(msg);
}
