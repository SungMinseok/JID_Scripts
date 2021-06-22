using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour
{
    //public int desMapNum;
    public Transform desLoc;

    void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            if(desLoc!=null){
                other.transform.position = desLoc.position;
                MapManager.instance.SetConfiner(desLoc.parent.transform.GetSiblingIndex());
            }
            else{

                DebugManager.instance.PrintDebug("목적지 없음");
            }
        }
        
    }
}
