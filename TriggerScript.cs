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
    
    void Awake()
    {
            instance = this;
    }
    void Start(){
    }

    public void Action(int trigNum, Dialogue[] dialogues = null, Transform[] poses = null){
        //Debug.Log("a");
        
        StartCoroutine(ActionCoroutine(trigNum, dialogues, poses));

    }

    IEnumerator ActionCoroutine(int trigNum, Dialogue[] dialogues = null, Transform[] poses = null){
        PlayerManager.instance.canMove =false;

        switch(trigNum){
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
                yield return new WaitForSeconds(1f);
                nerd_ant.animator.SetTrigger("wakeUp");
                yield return new WaitForSeconds(2f);
                //MapManager.instance.virtualCamera.Follow = ObjectController.instance.npcs[0].transform;
                SetDialogue(dialogues[0]);
                yield return new WaitForSeconds(2f);
                nerd_ant.animator.SetTrigger("standUp");
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
                yield return new WaitForSeconds(1f);
                nerd_ant.gameObject.SetActive(false);
                PlayerManager.instance.transform.position = poses[1].position;
                SceneController.instance.SetConfiner(poses[1].parent.transform.GetSiblingIndex());
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
                SceneController.instance.npcs[1].onRanDlg = true;

                ForceToPatrol(SceneController.instance.npcs[1]);
                ForceToPatrol(SceneController.instance.npcs[2]);


                break;
#endregion
            default : 
                break;
        }
        
        yield return null;    
        
        PlayerManager.instance.isActing =false;    
        PlayerManager.instance.canMove =true;    
    }

    public void SetDialogue(Dialogue dialogue){
        
        DialogueManager.instance.SetDialogue(dialogue);
    }
    public void CameraView(Transform target, float speed=2){
        SceneController.instance.virtualCamera.Follow = target;//ObjectController.instance.npcs[0].transform;
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
        
        PlayerManager.instance.canMove = true;

        if(location.preserveTrigger) location.locFlag = false;
    }
    public void ForceToPatrol(NPCScript npc){
        StartCoroutine(NPCPatrolCoroutineToStart(npc));
    }
    
    public IEnumerator NPCPatrolCoroutineToStart(NPCScript npc){ 
        WaitForSeconds _patrolInterval = new WaitForSeconds(npc.patrolInterval);
        while(npc.onPatrol && !npc.patrolFlag){
            Debug.Log("NPCPatrolCoroutineToStart");
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

}
