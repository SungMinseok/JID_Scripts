using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class DirtBundleInfo{
    public int objectID;
    public int curHp;
    public DirtBundleInfo(int _objectID, int _curHp){
        objectID = _objectID;
        curHp = _curHp;
    }
}
public class DirtScript : MonoBehaviour
{
    public enum BundleType{
        Dirt,
        Icicle,
    }
    public DirtBundleInfo dirtBundleInfo;
    //public int objectID;
    //public bool isBundle;
    public BundleType bundleType;
    public bool phaseOn;
    public GameObject[] dirtPhases;
    //public bool pieceOn;
    public Transform[] dirtPieces;
    public int innerItemIndex = -1;
    public int innerHoneyAmount = 0;
    public int maxHp = 6;
    public int remainPieceCount;
    WaitForSeconds wait180ms = new WaitForSeconds(0.18f);
    [Space]
    [Header("[Debug]━━━━━━━━━━━━━━━━━━━━━━━")]
    //public float curHp;
    public float amountPerPiece;


    void Start(){
        dirtBundleInfo.curHp = maxHp;
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
        if(dirtBundleInfo.curHp>0){
            dirtBundleInfo.curHp--;

            if(!DBManager.instance.curData.getDirtBundleOverList.Exists(x=> x.objectID == dirtBundleInfo.objectID)){
                DBManager.instance.curData.getDirtBundleOverList.Add(new DirtBundleInfo(dirtBundleInfo.objectID,dirtBundleInfo.curHp));
            }
            else{
                int index = DBManager.instance.curData.getDirtBundleOverList.FindIndex(x=>x.objectID == dirtBundleInfo.objectID);
                DBManager.instance.curData.getDirtBundleOverList[index].curHp = dirtBundleInfo.curHp;
            }

            if(dirtBundleInfo.curHp==0){
                dirtPhases[0].SetActive(false);
                CreatePiece(0);

                if(bundleType == BundleType.Icicle){
                    if(innerItemIndex != -1){
                        InventoryManager.instance.AddItem(innerItemIndex);
                    }
                    if(innerHoneyAmount != 0){
                        DBManager.instance.curData.curHoneyAmount += innerHoneyAmount;
                    }
                }

            }
            else if(dirtBundleInfo.curHp/maxHp<0.334f && remainPieceCount==2){
                //if(phaseOn) dirtPhases[1].SetActive(false);
                if(bundleType==BundleType.Icicle) CreatePiece(1);

            }
            else if(dirtBundleInfo.curHp/maxHp<0.667f && remainPieceCount==3){
                //if(phaseOn) dirtPhases[2].SetActive(false);
                if(bundleType==BundleType.Icicle) CreatePiece(2);
            }

            if(phaseOn){
                SetSprite();
            }
        }

    }
    public void SetSprite(){
        int temp = maxHp / dirtPhases.Length;

        if(dirtBundleInfo.curHp%temp==0){
            CreatePiece(dirtBundleInfo.curHp/temp);


            for(int i=dirtBundleInfo.curHp/temp;i<dirtPhases.Length;i++){
                dirtPhases[i].SetActive(false);
            }
        }
    }
    public void ResetSprite(){
    
        int temp = maxHp / dirtPhases.Length;

        for(int i=dirtBundleInfo.curHp/temp;i<dirtPhases.Length;i++){
            if(i+1<dirtPhases.Length) dirtPhases[i+1].SetActive(false);
        }
    }
    public void CreatePiece(int num){
        if(dirtPieces.Length < num) return;
        remainPieceCount -- ;
        dirtPieces[num].gameObject.SetActive(true);
        if(bundleType == BundleType.Dirt) dirtPieces[num].GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-2f,2f),1) * (1), ForceMode2D.Impulse);
        else if(bundleType == BundleType.Icicle){
            if(dirtBundleInfo.curHp == 0){
                SoundManager.instance.PlaySound("ice_broken_0"+Random.Range(1,3));

            }
        }
        if(dirtBundleInfo.curHp <=0){
            GetComponent<BoxCollider2D>().enabled =false;
        }

    }

}
