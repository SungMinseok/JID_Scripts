using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSkillScript : MonoBehaviour
{
    public GameObject[] bullets;
    public Vector2 bulletsPos;
    public Coroutine skillCoroutine;
    //public BoxCollider2D raderCollider;
    public bool raderFlag;
    void Start()
    {
        bulletsPos = bullets[0].transform.localPosition;
        
    }

    public void ThrowDownBullets(){
        if(skillCoroutine != null)  StopCoroutine(skillCoroutine);
        skillCoroutine = StartCoroutine(ThrowDownBulletsCoroutine());
    }
    IEnumerator ThrowDownBulletsCoroutine(){
        ResetBullets();

        for(int i=0;i<bullets.Length;i++){

            bullets[i].SetActive(true);
            bullets[i].GetComponent<Rigidbody2D>().AddForce(new Vector2(i-1,0) * (2), ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(3f);
        ResetBullets();
    }

    public void ThrowRightBullets(){
        if(skillCoroutine != null)  StopCoroutine(skillCoroutine);
        skillCoroutine = StartCoroutine(ThrowRightBulletsCoroutine());
    }
    IEnumerator ThrowRightBulletsCoroutine(){
        ResetBullets();
        for(int i=0;i<bullets.Length;i++){
            bullets[i].transform.localPosition = bulletsPos;
            bullets[i].SetActive(true);
            bullets[i].GetComponent<Rigidbody2D>().AddForce(new Vector2(1,0) * (10), ForceMode2D.Impulse);
        }
        yield return new WaitForSeconds(3f);
        ResetBullets();
    }

    public void ResetBullets(){
        
        for(int i=0;i<bullets.Length;i++){
            bullets[i].SetActive(false);
            bullets[i].transform.localPosition = bulletsPos;
        }
    }
    public void OnTriggerStay2D(Collider2D other){
        
        if(other.CompareTag("Player")){
            if(!raderFlag){

                raderFlag = true;
                Debug.Log("CCC");
            }
        }


    }
    public void OnTriggerExit2D(Collider2D other){
        
        if(other.CompareTag("Player")){
            raderFlag = false;
        }
    }
}
