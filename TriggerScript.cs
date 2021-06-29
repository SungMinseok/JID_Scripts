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

    public void Action(int trigNum, Dialogue[] dialogues = null){
        Debug.Log("a");
        StartCoroutine(ActionCoroutine(trigNum, dialogues));

    }

    IEnumerator ActionCoroutine(int trigNum, Dialogue[] dialogues = null){
        PlayerManager.instance.canMove =false;

        switch(trigNum){
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
            case 2 :
                ObjectController.instance.npcs[0].animator.SetTrigger("wakeUp");
                yield return new WaitForSeconds(2f);
                SetDialogue(dialogues[0]);
                yield return new WaitForSeconds(2f);
                ObjectController.instance.npcs[0].animator.SetTrigger("standUp");
                yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                SetDialogue(dialogues[1]);
                ObjectController.instance.npcs[0].animator.SetTrigger("count");
                yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                SetDialogue(dialogues[2]);
                ObjectController.instance.npcs[0].animator.SetTrigger("sweat");
                yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                SetDialogue(dialogues[3]);
                ObjectController.instance.npcs[0].animator.SetTrigger("turn");
                yield return new WaitForSeconds(1.417f);
                ObjectController.instance.npcs[0].wSet = -1;
                yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
                //MapManager.instance.virtualCamera.Follow = null;
                //ObjectController.instance.npcs[0].animator.SetTrigger("wakeUp");

                break;


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
    public void Wait(float time = 0) => StartCoroutine(WaitCoroutine(time));
    IEnumerator WaitCoroutine(float time = 0){
        if(time == 0){
            Debug.Log("c");
            yield return new WaitForSeconds(2f);
            yield return new WaitUntil(()=>!PlayerManager.instance.isActing);
        }
        else{
            Debug.Log("d");
            yield return new WaitForSeconds(time);
        }
    }

}
