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
    [Header("Honey")]
    public float honeyAmount;
    [Header("Dirt")]
    public float dirtAmount;
    Animator animator;

    void Start(){
        animator= transform.GetChild(0).GetComponent<Animator>();
    }

    void OnTriggerStay2D(Collider2D other) {
            
            if(other.CompareTag("Player")){
                if(type == ItemType.Honey){
                    
                    StartCoroutine(GetItem());
                    DM("꿀 충전 : "+honeyAmount);
                }
            }

    }

    IEnumerator GetItem(){
        animator.SetTrigger("got");
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.8)
        {
            yield return null;
        }
        gameObject.SetActive(false);
    }
    public void DM(string msg) => DebugManager.instance.PrintDebug(msg);
}
