using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DirtScript : MonoBehaviour
{
    public bool isBundle;
    public GameObject[] dirtPhases;
    public Transform[] dirtPieces;
    public int hp = 6;


    void Start(){
        for(int i=0;i<dirtPieces.Length;i++){
            Physics2D.IgnoreCollision(PlayerManager.instance.boxCollider2D, dirtPieces[i].GetChild(0).GetComponent<PolygonCollider2D>(), true);
            Physics2D.IgnoreCollision(PlayerManager.instance.circleCollider2D, dirtPieces[i].GetChild(0).GetComponent<PolygonCollider2D>(), true);

            if(i>=1){
                Physics2D.IgnoreCollision(dirtPieces[i].GetChild(0).GetComponent<PolygonCollider2D>(), dirtPieces[i-1].GetChild(0).GetComponent<PolygonCollider2D>(), true);
            }
            if(i==dirtPieces.Length-1){
                Debug.Log(i);
                Physics2D.IgnoreCollision(dirtPieces[i].GetChild(0).GetComponent<PolygonCollider2D>(), dirtPieces[0].GetChild(0).GetComponent<PolygonCollider2D>(), true);

            }
        }
    }

    public void GetDug(){
        if(hp>0){
            hp--;
            if(hp==4){
                dirtPhases[0].SetActive(false);
                CreatePiece(0);
            }
            else if(hp==2){
                dirtPhases[1].SetActive(false);
                CreatePiece(1);

            }
            else if(hp==0){
                dirtPhases[2].SetActive(false);
                CreatePiece(2);

            }
        }
    }
    public void CreatePiece(int num){
        dirtPieces[num].gameObject.SetActive(true);
        dirtPieces[num].GetChild(0).GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-2f,2f),1) * (1), ForceMode2D.Impulse);
    }

}
