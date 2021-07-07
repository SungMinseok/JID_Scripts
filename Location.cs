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
    Patrol_NPC,
}
public class Location : MonoBehaviour
{
    [SerializeField]BoxCollider2D boxCollider2D;
    [SerializeField]
    LocationType type;
    public bool preserveTrigger;
    //[Header("Teleport")]
    [Header("Teleport & Order")]
    public int doorNum;
    public Transform desLoc;
    bool orderFlag;
    [Header("Dialogue")]
    public Dialogue[] dialogues;
    [Header("Trigger")]
    public int trigNum;
    public Transform[] poses;
    public Dialogue[] dialogues_T;
    [Header("Patrol_NPC")]
    public Transform desLoc_Patrol_NPC;
    public bool patrolFlag;
    public float patrolWaitTime;


    TriggerScript triggerScript;
    
    void Start(){
        triggerScript = TriggerScript.instance;
        boxCollider2D = GetComponent<BoxCollider2D>();
    }
    void OnTriggerStay2D(Collider2D other) {
            // if(other.CompareTag("Rader")){
                
            //     Physics2D.IgnoreCollision(other.gameObject.GetComponent<PolygonCollider2D>(), boxCollider2D, true);
            // }

    
            switch(type){
                case LocationType.Teleport :

                    if(other.CompareTag("Player")){
                        if(desLoc!=null){
                            PlayerManager.instance.transform.position = desLoc.position;
                            Debug.Log(desLoc.parent.transform.GetSiblingIndex());
                            MapManager.instance.SetConfiner(desLoc.parent.transform.parent.transform.GetSiblingIndex());
                        }
                        else{

                            DM("목적지 없음");
                        }
                    }
                    break;

                case LocationType.Order :

                    if(other.CompareTag("Player")){
                        if(desLoc!=null){
                            StartCoroutine(OrderCoroutine(PlayerManager.instance.transform,desLoc));
                        }
                        else{
                            DM("목적지 없음");
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

                    if(!DBManager.instance.CheckTrigOver(trigNum)){
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

                                            
                            if(!preserveTrigger){
                                DBManager.instance.TrigOver(trigNum);
                            }
                        }

                    }
                    break;

                case LocationType.Patrol_NPC :

                    if(other.CompareTag("NPC")){
                        if(desLoc_Patrol_NPC!=null){
                            if(!patrolFlag){
                                //DM("gogo");
                                StartCoroutine(NPCPatrolCoroutineToStart(other.GetComponent<NPCScript>()));
                            }   
                        }
                        else{
                            DM("목적지 없음");
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
    
    IEnumerator NPCPatrolCoroutineToStart(NPCScript npc){ 
        while(npc.onPatrol && !patrolFlag){

            patrolFlag = true;  
            //DM("도착지로 이동");         
            if(npc.desPos.position.x > npc.transform.position.x){
                npc.wSet = 1;
            }
            else{
                npc.wSet = -1;
            }
            npc.animator.SetBool("walk",true);
            yield return new WaitUntil(()=>npc.onTriggerCol == npc.desPos);
            
            //DM("대기");         
            npc.wSet = 0;
            npc.animator.SetBool("walk",false);
            yield return new WaitForSeconds(patrolWaitTime); 
            
            //DM("출발지로 이동");                 
            if(npc.startPos.position.x > npc.transform.position.x){
                npc.wSet = 1;
            }
            else{
                npc.wSet = -1;
            }
            npc.animator.SetBool("walk",true);
            yield return new WaitUntil(()=>npc.onTriggerCol == npc.startPos);
            //DM("대기");         
            npc.wSet = 0;
            npc.animator.SetBool("walk",false);
            yield return new WaitForSeconds(patrolWaitTime); 

            patrolFlag = false;
        }
    }
    public void SetTalk(){
        DialogueManager.instance.SetFullDialogue(dialogues);
    }
    public void DM(string msg) => DebugManager.instance.PrintDebug(msg);
    
































#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle();
        Vector3 namePos = Vector3.zero;
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
                
                style.fontSize = 20;
                style.fontStyle = FontStyle.Bold;
                style.normal.textColor = Color.red;
                namePos = transform.position;
                namePos.x -= 0.5f;
                namePos.y += 0.7f;
                Handles.Label(namePos, doorNum.ToString(),style);
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

                //GUIStyle style = new GUIStyle();
                style.fontSize = 20;
                style.fontStyle = FontStyle.Bold;
                style.normal.textColor = Color.cyan;
                namePos = transform.position;
                namePos.x -= 0.5f;
                namePos.y += 0.7f;
                Handles.Label(namePos, trigNum.ToString(),style);
                break;
            case LocationType.Patrol_NPC :
                Gizmos.color = Color.black;   
                Gizmos.DrawWireCube(transform.position,  transform.localScale);                
                if(desLoc_Patrol_NPC!=null){
                    Gizmos.color = Color.white;   
                    Gizmos.DrawWireCube(desLoc_Patrol_NPC.transform.position, Vector3.one);
                    Gizmos.color = Color.black;   
                    Gizmos.DrawLine(transform.position,desLoc_Patrol_NPC.transform.position);
                }
                break;
    //Handles.Label(transform.position, "Text");
 
        }


    }
#endif

}
