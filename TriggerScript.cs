using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TriggerScript : MonoBehaviour
{    
    public static TriggerScript instance;

    WaitForSeconds wait1s = new WaitForSeconds(1);
    WaitForSeconds wait2s = new WaitForSeconds(2);
    WaitForSeconds wait3s = new WaitForSeconds(3);

    WaitUntil waitTalking = new WaitUntil(()=>!PlayerManager.instance.isTalking);
    
    void Awake()
    {
            instance = this;
    }
    void Start(){
    }

   //public void Action(Location location, Dialogue[] dialogues = null, Select[] selects = null, Transform[] poses = null){
    public void Action(Location location){
        //Debug.Log("트리거 발동" + trigNum);
        // if(selects != null){
        //     Debug.Log(selects[0].answers[0]);
        // }
        // else{
        //     Debug.Log("선택지 없음");
        // }
        
//StartCoroutine(ActionCoroutine(location, dialogues, selects, poses));
        StartCoroutine(ActionCoroutine(location));
//        Debug.Log("44");

    }

    //IEnumerator ActionCoroutine(int trigNum, Dialogue[] dialogues, Select[] selects, Transform[] poses){
    IEnumerator ActionCoroutine(Location location){
        PlayerManager.instance.canMove =false;

        Dialogue[] dialogues = null;
        Select[] selects = null;
        Transform[] poses = null;

        if(location.dialogues_T !=null){
            dialogues = location.dialogues_T;
        }
        else{
            dialogues = null;
        }
        if(location.selects_T!=null){
            selects = location.selects_T;
        }
        // else{
        //     selects = null;
        // }
        if(location.poses != null){
            poses = location.poses;
        }
        else{
            poses = null;
        }

        switch(location.trigNum){
#region 1
            case 1 :
                SetDialogue(dialogues[0]);
                yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                PlayerManager.instance.Look("right");
                SetDialogue(dialogues[1]);
                yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                PlayerManager.instance.Look("left");
                //MapManager.instance.virtualCamera.Follow = null;
                //ObjectController.instance.npcs[0].animator.SetTrigger("wakeUp");
                break;
#endregion

#region 3
            case 3 :
                var nerd_ant = SceneController.instance.npcs[0];

                CameraView(nerd_ant.transform);
                yield return wait1s;
                nerd_ant.animator.SetTrigger("wakeUp");
                yield return wait2s;
                //MapManager.instance.virtualCamera.Follow = ObjectController.instance.npcs[0].transform;
                nerd_ant.animator.SetTrigger("standUp");
                SetDialogue(dialogues[0]);
                yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                //yield return wait2s;
                yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                SetDialogue(dialogues[1]);
                nerd_ant.animator.SetTrigger("count");
                //CameraView(poses[0]);
                yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                SetDialogue(dialogues[2]);
                nerd_ant.animator.SetTrigger("sweat");
                yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                SetDialogue(dialogues[3]);
                nerd_ant.animator.SetTrigger("turn");
                //CameraView(nerd_ant.transform);
                yield return new WaitForSeconds(1.2f);
                nerd_ant.wSet = -1;
                yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                yield return wait3s;
                nerd_ant.gameObject.SetActive(false);
                PlayerManager.instance.transform.position = poses[0].position;
                SceneController.instance.SetConfiner(poses[0].parent.transform.GetSiblingIndex());
                yield return new WaitForSeconds(0.1f);
                CameraView(PlayerManager.instance.transform);
                //MapManager.instance.virtualCamera.Follow = null;
                //ObjectController.instance.npcs[0].animator.SetTrigger("wakeUp");

                break;
#endregion

#region 4
            case 4 :
                ActivateEffect(1,3);
                yield return new WaitUntil(()=>!UIManager.instance.onEffect);
                SetDialogue(dialogues[0]);
                yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                PlayerManager.instance.Look("left");
                break;
#endregion

#region 5
            case 5 :
            
                CameraView(poses[0]);
                SceneController.instance.npcs[1].gameObject.SetActive(true);
                SceneController.instance.npcs[2].gameObject.SetActive(true);
                yield return wait3s;
                CameraView(PlayerManager.instance.transform);
                SceneController.instance.npcs[1].onPatrol = true;
                SceneController.instance.npcs[2].onPatrol = true;
                SceneController.instance.npcs[1].onRandomDialogue = true;

                ForceToPatrol(SceneController.instance.npcs[1]);
                ForceToPatrol(SceneController.instance.npcs[2]);


                break;
#endregion

#region 6
            case 6 :
                
                SetDialogue(dialogues[0]);
                yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                SetDialogue(dialogues[6]);
                yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                SetDialogue(dialogues[7]);
                yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                SetSelect(selects[0]);
                yield return new WaitUntil(()=>!PlayerManager.instance.isSelecting);
                if(GetSelect()==0){

                    SetDialogue(dialogues[1]);
                    yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                    SetSelect(selects[1]);
                    yield return new WaitUntil(()=>!PlayerManager.instance.isSelecting);
                    if(GetSelect()==0){
                    }
                    else if(GetSelect()==1){
                        
                        SetDialogue(dialogues[2]);
                        yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                        SetDialogue(dialogues[3]);
                        yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                        SetDialogue(dialogues[4]);
                        yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                    }
                }
                else if(GetSelect()==1){
                    
                }






                break;
#endregion

#region 7
            case 7 :
                
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                SetDialogue(dialogues[1]);
                yield return waitTalking;
                SetDialogue(dialogues[2]);
                yield return waitTalking;
                SetDialogue(dialogues[3]);
                yield return waitTalking;
                SetDialogue(dialogues[4]);
                yield return waitTalking;
                SetDialogue(dialogues[5]);
                yield return waitTalking;
                SetDialogue(dialogues[6]);
                yield return waitTalking;
                SetDialogue(dialogues[7]);
                yield return waitTalking;
                SetDialogue(dialogues[8]);
                yield return waitTalking;
                SetDialogue(dialogues[9]);
                yield return waitTalking;

                CheatManager.instance.InputCheat("minigame 0");
                yield return new WaitUntil(()=>MinigameManager.instance.success);
                SetDialogue(dialogues[10]);
                yield return waitTalking;



                // SetSelect(selects[0]);
                // yield return new WaitUntil(()=>!PlayerManager.instance.isSelecting);
                // if(GetSelect()==0){

                //     SetDialogue(dialogues[1]);
                //     yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                //     SetSelect(selects[1]);
                //     yield return new WaitUntil(()=>!PlayerManager.instance.isSelecting);
                //     if(GetSelect()==0){
                //     }
                //     else if(GetSelect()==1){
                        
                //         SetDialogue(dialogues[2]);
                //         yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                //         SetDialogue(dialogues[3]);
                //         yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                //         SetDialogue(dialogues[4]);
                //         yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                //     }
                // }
                // else if(GetSelect()==1){
                    
                // }






                break;
#endregion
            default : 
                break;
        }
        
        yield return null;    

        location.locFlag = false;
        
        PlayerManager.instance.isActing =false;    
        PlayerManager.instance.canMove =true;    
    }

    public void SetDialogue(Dialogue dialogue){
        
        DialogueManager.instance.SetDialogue(dialogue);
    }
    public void SetSelect(Select select){
        if(select!=null){
            SelectManager.instance.SetSelect(select);

        }
        else{
            DM("Error : no selection");
        }
    }
    public int GetSelect(){
        return SelectManager.instance.GetSelect();
    }
    public void CameraView(Transform target, float speed=2){
        if(target!=null){
            SceneController.instance.virtualCamera.Follow = target;//ObjectController.instance.npcs[0].transform;
        }
        else{
            DM("Error : no pos");
        }
    }
    public void ActivateEffect(int num,float timer,bool bgOn = true){
        UIManager.instance.ActivateEffect(num,timer,bgOn);
    }

    
    public IEnumerator OrderCoroutine(Location location,Transform objCol,Transform desCol){    
        
            PlayerManager.instance.canMove = false;
        
        if(desCol.position.x > objCol.position.x){
            PlayerManager.instance.wSet = 1;
        }
        else{
            PlayerManager.instance.wSet = -1;
        }
        yield return new WaitUntil(()=>PlayerManager.instance.onTriggerCol == desCol);
        
        if(!location.stopCheck) PlayerManager.instance.canMove = true;

        if(location.preserveTrigger) location.locFlag = false;
    }
    public IEnumerator OrderCoroutine_NPC(Location location,NPCScript objCol,Transform desCol){    
        
        if(desCol.position.x > objCol.transform.position.x){
            objCol.wSet = 1;
            
            //if(location.flipCheck) objCol.GetComponent<SpriteRenderer>().flipX = !objCol.GetComponent<SpriteRenderer>().flipX ;
        }
        else{
            objCol.wSet = -1;
            //if(location.flipCheck) objCol.GetComponent<SpriteRenderer>().flipX = !objCol.GetComponent<SpriteRenderer>().flipX ;
        }

        yield return wait1s;

        
        if(location.preserveTrigger) location.locFlag = false;
        //yield return new WaitUntil(()=>PlayerManager.instance.onTriggerCol == desCol);
        
        //if(location.preserveTrigger) location.locFlag = false;
    }
    public void ForceToPatrol(NPCScript npc){
        StartCoroutine(NPCPatrolCoroutineToStart(npc));
    }
    
    public IEnumerator NPCPatrolCoroutineToStart(NPCScript npc){ 
        WaitForSeconds _patrolInterval = new WaitForSeconds(npc.patrolInterval);
        while(npc.onPatrol && !npc.patrolFlag){
//            Debug.Log("NPCPatrolCoroutineToStart");
            npc.patrolFlag = true;  
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
            yield return _patrolInterval; 
            
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
            yield return _patrolInterval; 

            npc.patrolFlag = false;
        }
    }

    public void DM(string msg) => DebugManager.instance.PrintDebug(msg);
}
