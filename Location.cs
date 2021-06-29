using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
public enum LocationType{
    Teleport,   //캐릭터 순간이동
    Order,   //캐릭터 걸어서 이동
    Dialogue,
    Trigger,
}
public class Location : MonoBehaviour
{
    [SerializeField]BoxCollider2D boxCollider2D;
    [SerializeField]
    LocationType type;
    //[Header("Teleport")]
    [Header("Teleport & Order")]
    public Transform desLoc;
    bool orderFlag;
    [Header("Dialogue")]
    public Dialogue[] dialogues;
    [Header("Trigger")]
    public int trigNum;
    public Transform[] poses;
    public Dialogue[] dialogues_T;

    TriggerScript triggerScript;
    
    void Start(){
        triggerScript = TriggerScript.instance;
        //boxCollider2D = GetComponent<BoxCollider2D>();
    }
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
            case LocationType.Dialogue :

                if(other.CompareTag("Player")){
                    if(dialogues!=null){
                        if(!PlayerManager.instance.isTalking){
                            PlayerManager.instance.isTalking = true;
                            SetTalk();
                        }

                    }
                    else{
                        DebugManager.instance.PrintDebug("대화 설정 안됨");
                    }
                }
                break;
            case LocationType.Trigger :

                if(other.CompareTag("Player")){
                    if(trigNum>=0){
                        if(!PlayerManager.instance.isActing){
                            PlayerManager.instance.isActing = true;
                            if(dialogues_T!=null){
                                if(poses!=null){
                                
                                    triggerScript.Action(trigNum, dialogues_T, poses);
                                }
                                else{
                                    triggerScript.Action(trigNum, dialogues_T);

                                }
                            }
                            else{
                                triggerScript.Action(trigNum);
                            }
                        }
                            
                    }
                    else{
                        DebugManager.instance.PrintDebug("트리거 설정 안됨");
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
        // PlayerManager.instance.wInput = 0;
        // PlayerManager.instance.hInput = 0;
        if(desCol.position.x > objCol.position.x){
            PlayerManager.instance.wSet = 1;
        }
        else{
            PlayerManager.instance.wSet = -1;
        }
        yield return new WaitUntil(()=>PlayerManager.instance.onTriggerCol == desCol);
        PlayerManager.instance.canMove = true;

    }
    public void SetTalk(){
        DialogueManager.instance.SetFullDialogue(dialogues);
    }

    void OnDrawGizmos()
    {
        switch(type){
            case LocationType.Teleport :
                //Gizmos.color = new Color(Color.red.r,Color.red.g,Color.red.b,0.3f); 

                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(transform.position, transform.localScale);
                if(desLoc!=null){
                    Gizmos.color = Color.blue;   
                    Gizmos.DrawWireCube(desLoc.transform.position, Vector3.one);
                    Gizmos.color = Color.black;   
                    Gizmos.DrawLine(transform.position,desLoc.transform.position);
                }
                break;
            case LocationType.Order :
                //Gizmos.color = new Color(Color.red.r,Color.red.g,Color.red.b,0.3f);
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(transform.position, transform.localScale);
                if(desLoc!=null){
                    Gizmos.color = Color.blue;   
                    Gizmos.DrawWireCube(desLoc.transform.position, Vector3.one);
                    Gizmos.color = Color.white;   
                    Gizmos.DrawLine(transform.position,desLoc.transform.position);
                }
                break;
            case LocationType.Dialogue :
                Gizmos.color = Color.yellow;   
                Gizmos.DrawWireCube(transform.position,  transform.localScale);
                break;
            case LocationType.Trigger :
                Gizmos.color = Color.cyan;   
                Gizmos.DrawWireCube(transform.position, transform.localScale);

                GUIStyle style = new GUIStyle();
                style.fontSize = 20;
                style.fontStyle = FontStyle.Bold;
                style.normal.textColor = Color.cyan;
                Vector3 namePos = transform.position;
                namePos.x -= 0.5f;
                namePos.y += 0.7f;
                Handles.Label(namePos, trigNum.ToString(),style);
                break;
    //Handles.Label(transform.position, "Text");
 
        }


    }


}
