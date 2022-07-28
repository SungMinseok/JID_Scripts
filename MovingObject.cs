using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MovingObject : MonoBehaviour
{
    

    [Header("[필수]기본 방향이 왼쪽이면 체크")]
    public bool isLookingLeft;
    
    [Header("Status")]
    [SerializeField][Range(1f,10f)] public float speed = 2f;
    public int wSet;



    
    [Header("AutoSet━━━━━━━━━━━━━━━━━━━━━━━━━━━━━")]
    public Rigidbody2D rb;
    [Header("바디가 스프라이트면 체크(not bones)")]
    public bool isSpriteRenderer;
    void Start(){
        if(GetComponent<SpriteRenderer>()!=null){
            isSpriteRenderer = true;
        }

        rb = GetComponentInParent<Rigidbody2D>();
    }
    void FixedUpdate(){
        if(wSet!=0 && rb!=null){
            
            rb.velocity = new Vector2(speed * wSet  , rb.velocity.y);

        }
    }
    
}
