using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType{
    Honey,
    Dirt,
}
public class ItemScript : MonoBehaviour
{
    public ItemType type;
    //[Header("Honey")]
    public float amount;
    //[Header("Dirt")]
    //public float dirtAmount;
    Animator animator;
    public BoxCollider2D itemCol;
    //Vector2 itemVector;
    bool getFlag;

    void Start(){
        if(GetComponent<Animator>()!=null)
            animator= GetComponent<Animator>();
        itemCol = GetComponent<BoxCollider2D>();
        //itemVector = transform.GetChild(0).localPosition;
    }

    void OnTriggerStay2D(Collider2D other) {
            
                    //Debug.Log("1");
            if(other.CompareTag("Player")){
                if(!getFlag) {
                    getFlag = true;

                // Debug.Log("2");
                    if(type == ItemType.Honey){
                    // Debug.Log("33");
                        
                        StartCoroutine(GetItem());
                        DM("꿀 충전 : "+amount);
                    }
                    else if(type == ItemType.Dirt){
                    // Debug.Log("33");
                        
                        StartCoroutine(GetItem());
                        DM("흙 충전 : "+amount);
                    }
                }
            }

    }


    IEnumerator GetItem(){
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
