using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(CanvasGroup))]
public class InteractiveUI : MonoBehaviour
{
    Coroutine clockCoroutine;
    CanvasGroup canvasGroup;
    BoxCollider2D boxCollider2D;
    public float valueX;
    public bool isHided;
    WaitForSeconds wait10ms = new WaitForSeconds(0.01f);
    void Awake(){
        boxCollider2D = GetComponent<BoxCollider2D>();
        canvasGroup = GetComponent<CanvasGroup>();
        boxCollider2D.isTrigger = true;
    }
    // void Update(){
        
    //     if ( !hided && Mathf.Abs(PlayerManager.instance.transform.position.x - this.transform.position.x )< valueX){
    //         Cloak();
    //     }
    //     else{
    //         Decloak();
    //     }
    // }
    void OnTriggerEnter2D(Collider2D other){
        
            if(other.CompareTag("Player")){
                if(!isHided){
                    Cloak();
                }
            }
    }
    void OnTriggerExit2D(Collider2D other){
        
            if(other.CompareTag("Player")){
                if(isHided){
                    Decloak();
                }
            }
    }
    void Cloak(float _speed = 0.1f){
        isHided = true;
        //if(clockCoroutine) StopCoroutine(clockCoroutine);
        
        StopAllCoroutines();
        clockCoroutine = StartCoroutine(CloakCoroutine(_speed));
        //spriteRenderer.color = darkColor;
        //PlayerManager.instance.SetAlpha(0.5f);
    }
    IEnumerator CloakCoroutine(float _speed){
        while(canvasGroup.alpha >0.5f){
            canvasGroup.alpha -= _speed;
            
            yield return wait10ms;
        }
    }    
    void Decloak(float _speed = 0.1f){
        isHided = false;
        
        StopAllCoroutines();
        StartCoroutine(DecloakCoroutine(_speed));
        //spriteRenderer.color = defaultColor;
        //PlayerManager.instance.SetAlpha(1f);
    }
    IEnumerator DecloakCoroutine(float _speed){
        while(canvasGroup.alpha <1f){
            canvasGroup.alpha += _speed;
            yield return wait10ms;
        }
    }
}
