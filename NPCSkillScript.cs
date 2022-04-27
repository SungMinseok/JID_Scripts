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
    public bool skillFlag;
    public int curSkillNum;
    WaitForSeconds wait100ms = new WaitForSeconds(0.1f);
    WaitForSeconds wait500ms = new WaitForSeconds(0.5f);
    WaitForSeconds wait1000ms = new WaitForSeconds(1);
    WaitForSeconds wait2000ms = new WaitForSeconds(2);
    WaitForSeconds wait3000ms = new WaitForSeconds(3);
    WaitForSeconds wait5000ms = new WaitForSeconds(5);
    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    void Start()
    {
        bulletsPos = bullets[0].transform.localPosition;
        
    }
    //FlyingMadAnt > madant_flying > ant_mad_fly_throw 의 애니메이션에 연결
    public void ThrowDownBullets(){
        if(skillCoroutine != null)  StopCoroutine(skillCoroutine);
        skillCoroutine = StartCoroutine(ThrowDownBulletsCoroutine());
    }
    IEnumerator ThrowDownBulletsCoroutine(){
        ResetBullets();

        for(int i=0;i<bullets.Length;i++){

            bullets[i].SetActive(true);
            bullets[i].GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                yield return waitForFixedUpdate;
            bullets[i].GetComponent<Rigidbody2D>().AddForce(new Vector2((i-2)*2,0) * (Minigame2Script.instance.flyingBottleSpeed), ForceMode2D.Impulse);
            SoundManager.instance.PlaySound("minigame_bottle_throw");
        }
        yield return wait1000ms;

        skillFlag = false;
        // yield return new WaitForSeconds(3f);
        // ResetBullets();
    }
    public void Throw3(){
        if(skillCoroutine != null)  StopCoroutine(skillCoroutine);
        skillCoroutine = StartCoroutine(Throw3Coroutine());
    }
    IEnumerator Throw3Coroutine(){
        ResetBullets();
        yield return wait100ms;

        for(int i=0;i<10;i+=2){

            //for(int j=0;j<2;j++){
            bullets[i].SetActive(true);
                yield return waitForFixedUpdate;
            bullets[i].GetComponent<Rigidbody2D>().AddForce(new Vector2((i+0.2f)* (Minigame2Script.instance.flyingBottleSpeed) / 1.5f,0) , ForceMode2D.Impulse);
            Debug.Log(new Vector2((i+0.2f),0) * (Minigame2Script.instance.flyingBottleSpeed) / 1.5f);
                yield return waitForFixedUpdate;
            bullets[i+1].SetActive(true);
            bullets[i+1].GetComponent<Rigidbody2D>().AddForce(new Vector2((i+0.2f)*-1f,0) * (Minigame2Script.instance.flyingBottleSpeed) / 1.5f, ForceMode2D.Impulse);
            SoundManager.instance.PlaySound("minigame_bottle_throw");
            //}
            if(i!=8){

                yield return wait500ms;
            }
        }
        GetComponent<Animator>().SetTrigger("done");
        yield return wait1000ms;

        skillFlag = false;
    }
    public void ThrowBullets(){
        switch(curSkillNum){
            case 0 :
                ThrowRightBullets();
                break;
            case 1 :
                ThrowDownBullets();
                break;
            case 2 :
                ThrowUpBullets();
                break;
            case 3 :
                Throw3();
                break;
        }
    }
    public void SetSkill(int skillNum){
        skillFlag = true;
        curSkillNum = skillNum;
        //Debug.Log(skillNum);
        this.transform.GetComponent<Animator>().SetTrigger("throw");
    }

    public void ThrowRightBullets(){
        if(skillCoroutine != null)  StopCoroutine(skillCoroutine);
        skillCoroutine = StartCoroutine(ThrowRightBulletsCoroutine());
    }
    IEnumerator ThrowRightBulletsCoroutine(){
        //ResetBullets();

        
        int randomDice = Random.Range(0,2);
        float randomNum = 0;
        if(randomDice == 0){
            randomNum = 0f;
        }
        else if(randomDice == 1){

            randomNum = 1.5f;
        }
        Debug.Log(randomNum);
        //for(int i=0;i<bullets.Length;i++){
        bullets[0].transform.localPosition = new Vector2(bulletsPos.x, bulletsPos.y - randomNum);
        bullets[0].SetActive(true);
                yield return waitForFixedUpdate;
        bullets[0].GetComponent<Rigidbody2D>().AddForce(new Vector2(1.2f,0) * (Minigame2Script.instance.throwingBottleSpeed), ForceMode2D.Impulse);
        SoundManager.instance.PlaySound("minigame_bottle_throw");
        //}
        yield return wait1000ms;
        yield return wait500ms;
        bullets[0].SetActive(false);
        bullets[0].GetComponent<Rigidbody2D>().velocity = Vector3.zero;

        //yield return new WaitUntil(()=>!bullets[0].activeSelf);
        skillFlag = false;

        //yield return null;
        //yield return new WaitForSeconds(3f);
        //ResetBullets();
    }
    
    public void ThrowUpBullets(){
        if(skillCoroutine != null)  StopCoroutine(skillCoroutine);
        skillCoroutine = StartCoroutine(ThrowUpBulletsCoroutine());
    }
    IEnumerator ThrowUpBulletsCoroutine(){

        int randomDice = Random.Range(0,2);
        int randomNum = 0;
        if(randomDice == 0){
            randomNum = 3;
        }
        else if(randomDice == 1){

            randomNum = 4;
        }

        for(int i=1;i<bullets.Length;i++){
            bullets[i].GetComponent<DoodadsScript>().curPos = new Vector2(randomNum + (i-1)*3.5f, 15);
            //bullets[i].SetActive(true);
        }


        //ResetBullets();
        for(int i=0;i<1;i++){
            bullets[0].transform.localPosition = new Vector2(bulletsPos.x,bulletsPos.y);
            bullets[0].SetActive(true);
                yield return waitForFixedUpdate;
            bullets[0].GetComponent<Rigidbody2D>().AddForce(new Vector2(1f,2f) * (Minigame2Script.instance.throwingBottleSpeed), ForceMode2D.Impulse);
            SoundManager.instance.PlaySound("minigame_bottle_throw");
            
            //yield return wait1000ms;
        }

            // bullets[1].transform.localPosition = bulletsPos;
            // bullets[1].SetActive(true);
            // bullets[1].GetComponent<Rigidbody2D>().AddForce(new Vector2(0.5f,1f) * (Minigame2Script.instance.throwingBottleSpeed), ForceMode2D.Impulse);
            // SoundManager.instance.PlaySound("minigame_bottle_throw");

            
            //yield return wait500ms;
        yield return wait1000ms;
        bullets[0].GetComponent<Rigidbody2D>().velocity = Vector3.zero;

        for(int i=1;i<bullets.Length;i++){
            bullets[i].transform.localPosition = bullets[i].GetComponent<DoodadsScript>().curPos;
            bullets[i].SetActive(true);
        }
        //}
        yield return wait2000ms;
        //yield return new WaitForSeconds(3f);
        //ResetBullets();
        skillFlag = false;
    }

    public void ResetBullets(){
        
        for(int i=0;i<bullets.Length;i++){
            bullets[i].GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            
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
