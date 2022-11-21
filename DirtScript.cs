using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class DirtBundleInfo{
    public int objectID;
    public int curHp;
    //public bool isRecreatable = true;
    public float curRecreateCoolTime;
    public DirtBundleInfo(int _objectID, int _curHp, float _recreateCoolTime){
        objectID = _objectID;
        curHp = _curHp;
        //isRecreatable = _isRecreatable;
        curRecreateCoolTime = _recreateCoolTime;
    }
}
public class DirtScript : MonoBehaviour
{
    public enum BundleType{
        Dirt,
        Icicle,
    }
    [Space]
    [Header("[Settings]━━━━━━━━━━━━━━━━━━━━━━━")]
    public BundleType bundleType;
    public int maxHp = 6;
    //public int objectID;
    //public bool isBundle;
    public int innerItemIndex = -1;
    public int innerHoneyAmount = 0;
    public bool isRecreatable;
    public float recreateCoolTime;
    public bool phaseOn;//스프라이트 변경될 경우 트루
    [Header("[Objects]━━━━━━━━━━━━━━━━━━━━━━━")]
    public GameObject[] dirtPhases;
    //public bool pieceOn;
    public Transform[] dirtPieces;
    public int remainPieceCount;
    WaitForSeconds waitRecreateCoolTime;
    //public GameObject keyTutorial;
    WaitForSeconds wait180ms = new WaitForSeconds(0.18f);
    [Space]
    [Header("[Debug]━━━━━━━━━━━━━━━━━━━━━━━")]
    //public float curHp;
    public float amountPerPiece;
    public DirtBundleInfo dirtBundleInfo;


    void Start(){
        dirtBundleInfo.curHp = maxHp;
        remainPieceCount = dirtPieces.Length;
        waitRecreateCoolTime = new WaitForSeconds(recreateCoolTime);
        IgnoreCollision();
    }
    void IgnoreCollision(){

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

                dirtPieces[i].localPosition = Vector2.zero;
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
            if(bundleType == BundleType.Icicle){
                CreatePiece(dirtBundleInfo.curHp);
            }


            if(dirtBundleInfo.curHp==0){
                
                UIManager.instance.HideKeyTutorial();
                dirtPhases[0].SetActive(false);
                CreatePiece(0);

                if(bundleType == BundleType.Icicle){
                    if(innerItemIndex != -1){
                        InventoryManager.instance.AddItem(innerItemIndex,activateDialogue:true);
                    }
                    if(innerHoneyAmount != 0){
                        InventoryManager.instance.AddHoney(innerHoneyAmount,activateDialogue:true);

                        //DBManager.instance.curData.curHoneyAmount += innerHoneyAmount;
                    }
                }
                else if(bundleType == BundleType.Dirt){
                    UIManager.instance.CompleteQuest(11);
                }

                if(isRecreatable){
                    StartCoroutine(RecreateCoroutine());
                }

            }
            // else if(dirtBundleInfo.curHp/maxHp<0.334f && remainPieceCount==2){
            //     //if(phaseOn) dirtPhases[1].SetActive(false);
            //     if(bundleType==BundleType.Icicle) CreatePiece(1);

            // }
            // else if(dirtBundleInfo.curHp/maxHp<0.667f && remainPieceCount==3){
            //     //if(phaseOn) dirtPhases[2].SetActive(false);
            //     if(bundleType==BundleType.Icicle) CreatePiece(2);
            // }

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
    
        int temp0 = maxHp / dirtPhases.Length; // 6 / 2
        int temp1 = dirtBundleInfo.curHp / temp0;


        for(int i=0;i<temp1; i++){
            dirtPhases[i].SetActive(true);
        }
        for(int i=temp1;i<dirtPhases.Length;i++){
            dirtPhases[i].SetActive(false);
        }

        GetComponent<BoxCollider2D>().enabled =true;

        // for(int i=dirtBundleInfo.curHp/temp;i<dirtPhases.Length;i++){
        //     if(i+1<dirtPhases.Length) dirtPhases[i+1].SetActive(false);
        // }
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
    void OnTriggerEnter2D(Collider2D other){
        if(other.CompareTag("Player"))
            UIManager.instance.ShowKeyTutorial(GameInputManager.ReadKey("Interact"));
    }
    void OnTriggerExit2D(Collider2D other){
        if(other.CompareTag("Player"))
            UIManager.instance.HideKeyTutorial();
    }

    IEnumerator RecreateCoroutine(float coolTime = 0){
        if(coolTime == 0){
            yield return waitRecreateCoolTime;
        }
        else{
            yield return new WaitForSeconds(coolTime);
        }

        dirtBundleInfo.curHp = maxHp;
        dirtBundleInfo.curRecreateCoolTime = recreateCoolTime;

        ResetSprite(); IgnoreCollision();
    }
    void OnDisable(){
        StopAllCoroutines();
    }
}
