using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DirtScript : MonoBehaviour
{
    public enum BundleType{
        Dirt,
        Icicle,
    }
    //public bool isBundle;
    public BundleType bundleType;
    public bool phaseOn;
    public GameObject[] dirtPhases;
    //public bool pieceOn;
    public Transform[] dirtPieces;
    public float maxHp = 6;
    public int remainPieceCount;
    WaitForSeconds wait180ms = new WaitForSeconds(0.18f);
    [Space]
    [Header("[Debug]━━━━━━━━━━━━━━━━━━━━━━━")]
    public float curHp;
    public float amountPerPiece;


    void Start(){
        curHp = maxHp;
        remainPieceCount = dirtPieces.Length;
        
        if(bundleType == BundleType.Dirt){

            for(int i=0;i<dirtPieces.Length;i++){
                Physics2D.IgnoreCollision(PlayerManager.instance.bodyCollider2D, dirtPieces[i].GetComponent<PolygonCollider2D>(), true);
                Physics2D.IgnoreCollision(PlayerManager.instance.circleCollider2D, dirtPieces[i].GetComponent<PolygonCollider2D>(), true);

                if(i>=1){
                    Physics2D.IgnoreCollision(dirtPieces[i].GetComponent<PolygonCollider2D>(), dirtPieces[i-1].GetComponent<PolygonCollider2D>(), true);
                }
                if(i==dirtPieces.Length-1){
    //                Debug.Log(i);
                    Physics2D.IgnoreCollision(dirtPieces[i].GetComponent<PolygonCollider2D>(), dirtPieces[0].GetComponent<PolygonCollider2D>(), true);

                }
            }
        }
    }

    public void GetDug(){
        StartCoroutine(GetDugCoroutine());
//        Debug.Log(curHp/maxHp);
    }
    IEnumerator GetDugCoroutine(){
        yield return wait180ms;
        if(curHp>0){
            curHp--;
            if(curHp==0 && remainPieceCount==1){
                dirtPhases[0].SetActive(false);
                CreatePiece(0);

                if(bundleType == BundleType.Icicle) InventoryManager.instance.AddItem(31);

            }
            else if(curHp/maxHp<0.334f && remainPieceCount==2){
                if(phaseOn) dirtPhases[1].SetActive(false);
                CreatePiece(1);

            }
            else if(curHp/maxHp<0.667f && remainPieceCount==3){
                if(phaseOn) dirtPhases[2].SetActive(false);
                CreatePiece(2);
            }
        }

    }
    public void CreatePiece(int num){
        if(dirtPieces.Length == 0) return;
        remainPieceCount -- ;
        dirtPieces[num].gameObject.SetActive(true);
        if(bundleType == BundleType.Dirt) dirtPieces[num].GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-2f,2f),1) * (1), ForceMode2D.Impulse);

        if(curHp <=0){
            GetComponent<BoxCollider2D>().enabled =false;
        }

    }

}
