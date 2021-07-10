using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TriggerScript : MonoBehaviour
{    
    public static TriggerScript instance;
    
    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
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
                var nerd_ant = ObjectController.instance.npcs[0];

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
                yield return new WaitForSeconds(1.5f);
                nerd_ant.wSet = -1;
                yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                yield return new WaitForSeconds(1f);
                nerd_ant.gameObject.SetActive(false);
                PlayerManager.instance.transform.position = poses[1].position;
                MapManager.instance.SetConfiner(poses[1].parent.transform.GetSiblingIndex());
                yield return new WaitForSeconds(0.1f);
                CameraView(PlayerManager.instance.transform);
                //MapManager.instance.virtualCamera.Follow = null;
                //ObjectController.instance.npcs[0].animator.SetTrigger("wakeUp");

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
        MapManager.instance.virtualCamera.Follow = target;//ObjectController.instance.npcs[0].transform;
    }

}
