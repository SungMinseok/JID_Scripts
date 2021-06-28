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
        switch(trigNum){
            case 1 :


                DialogueManager.instance.SetDialogue(dialogues);
                MapManager.instance.virtualCamera.Follow = null;
                ObjectController.instance.npcs[0].animator.SetTrigger("wakeUp");

                break;


            default : 
                break;
        }
    }
}
