using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetScript : MonoBehaviour
{
    enum PetState{
        idle,
        move,
    }
    [SerializeField]
    float speed = 0.1f;
    [SerializeField]
    PetState petState;

    bool isLeft;



    [SerializeField]
    Vector2 leftPos, rightPos;
    Transform thePlayer;
    SpriteRenderer sr;
    void Start(){
        thePlayer = PlayerManager.instance.playerBody;
        sr = GetComponent<SpriteRenderer>();
        leftPos = GetComponent<RectTransform>().localPosition;
        rightPos = new Vector2(-leftPos.x, leftPos.y);
    }
    void Update(){
        //Debug.Log(Vector2.Distance(gameObject.transform.position,temp0.position));
        
        //오른쪽 볼 때, 왼쪽에 위치
        if(thePlayer.transform.localScale.x >= 0){
            isLeft = true;
        }
        else{
            isLeft = false;
        }

    }
    private void FixedUpdate() {
        
        if(isLeft){
            Move("left");
            //Vector2.MoveTowards(transform.position, leftPos, Time.deltaTime*speed);
        }
        else{
            //Vector2.MoveTowards(transform.position, rightPos, Time.deltaTime*speed);
            Move("right");
            
        }
    }
    void Move(string direction){
        if(direction == "left"){

            if(transform.localPosition.x>=leftPos.x){
                transform.Translate(Vector2.left * speed);
                sr.flipX = false;
                //transform.localScale = new Vector2();
            }
        }
        else{

            if(transform.localPosition.x<=rightPos.x){
                transform.Translate(Vector2.right * speed);
                sr.flipX = true;
                //transform.localScale = Vector2.left;
            }
        }
    }
}
