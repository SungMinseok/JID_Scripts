using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DirtScript : MonoBehaviour
{
    public bool isBundle;
    public GameObject[] dirtPhases;
    public Transform[] dirtPieces;
    public float maxHp = 6;
    public float curHp;
    public float amountPerPiece;


    void Start(){
        curHp = maxHp;

        for(int i=0;i<dirtPieces.Length;i++){
            Physics2D.IgnoreCollision(PlayerManager.instance.boxCollider2D, dirtPieces[i].GetChild(0).GetComponent<PolygonCollider2D>(), true);
            Physics2D.IgnoreCollision(PlayerManager.instance.circleCollider2D, dirtPieces[i].GetChild(0).GetComponent<PolygonCollider2D>(), true);

            if(i>=1){
                Physics2D.IgnoreCollision(dirtPieces[i].GetChild(0).GetComponent<PolygonCollider2D>(), dirtPieces[i-1].GetChild(0).GetComponent<PolygonCollider2D>(), true);
            }
            if(i==dirtPieces.Length-1){
//                Debug.Log(i);
                Physics2D.IgnoreCollision(dirtPieces[i].GetChild(0).GetComponent<PolygonCollider2D>(), dirtPieces[0].GetChild(0).GetComponent<PolygonCollider2D>(), true);

            }
        }
    }

    public void GetDug(){
        Debug.Log(curHp/maxHp);
        if(curHp>0){
            curHp--;
            if(curHp==0){
                dirtPhases[2].SetActive(false);
                CreatePiece(2);

            }
            else if(curHp==2){
                dirtPhases[1].SetActive(false);
                CreatePiece(1);

            }
            else if(curHp==4){
                dirtPhases[0].SetActive(false);
                CreatePiece(0);
            }
        }
    }
    public void CreatePiece(int num){
        dirtPieces[num].gameObject.SetActive(true);
        dirtPieces[num].GetChild(0).GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-2f,2f),1) * (1), ForceMode2D.Impulse);
    }

}
