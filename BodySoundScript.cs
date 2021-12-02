using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodySoundScript : MonoBehaviour
{
    public enum BodyType{
        Default,
        Pushable,
    }
    public BodyType bodyType;
    Rigidbody2D rb;
    bool soundFlag;
    WaitForSeconds wait1550ms = new WaitForSeconds(1.55f);
    void Start(){
        
        if(bodyType == BodyType.Pushable){
            rb = GetComponent<Rigidbody2D>();
        }
    }
    public void PlayWalkSound(){
        SoundManager.instance.PlaySound("LuckyWalk"+Random.Range(0,4));
    }
    void Update(){
        if(bodyType == BodyType.Pushable){
            if(rb.velocity.x >0 && !soundFlag){
                soundFlag = true;
                StartCoroutine(PlaySoundCoroutine());
            }
        }
    }
    IEnumerator PlaySoundCoroutine(){
        SoundManager.instance.PlaySound("CandyRoll"+Random.Range(0,2));
        yield return wait1550ms;
        soundFlag = false;
    }
}
