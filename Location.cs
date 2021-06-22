using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum LocationType{
    Teleport,   //캐릭터 순간이동
    Order   //캐릭터 걸어서 이동
}
public class Location : MonoBehaviour
{
    [SerializeField]
    LocationType type;
    public Transform desLoc;
    //[Header("Teleport")]
    [Header("Order")]
    bool orderFlag;

    void OnTriggerEnter2D(Collider2D other) {

        switch(type){
            case LocationType.Teleport :

                if(other.CompareTag("Player")){
                    if(desLoc!=null){
                        other.transform.position = desLoc.position;
                        MapManager.instance.SetConfiner(desLoc.parent.transform.GetSiblingIndex());
                    }
                    else{

                        DebugManager.instance.PrintDebug("목적지 없음");
                    }
                }
                break;

            case LocationType.Order :

                if(other.CompareTag("Player")){
                    if(desLoc!=null){
                        StartCoroutine(OrderCoroutine(other.transform,desLoc));
                    }
                    else{
                        DebugManager.instance.PrintDebug("목적지 없음");
                    }
                }
                break;

            default :
                DebugManager.instance.PrintDebug("로케이션 오류");
                break;
        }


    }

    IEnumerator OrderCoroutine(Transform objCol,Transform desCol){                        
        PlayerManager.instance.canMove = false;
        PlayerManager.instance.wInput = 0;
        PlayerManager.instance.hInput = 0;
        if(desCol.position.x > objCol.position.x){
            PlayerManager.instance.wInput = 1;
        }
        else{
            PlayerManager.instance.wInput = -1;
        }
        yield return new WaitUntil(()=>PlayerManager.instance.onTriggerCol == desCol);
        PlayerManager.instance.canMove = true;

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;   
        Gizmos.DrawCube(transform.position, Vector3.one);
        if(desLoc!=null){

            Gizmos.color = Color.blue;   
            Gizmos.DrawCube(desLoc.transform.position, Vector3.one);
            Gizmos.color = Color.black;   
            Gizmos.DrawLine(transform.position,desLoc.transform.position);
        }
    }


}
