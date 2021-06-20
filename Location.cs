using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour
{
    public Transform destination;

    void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            if(destination!=null){
                other.transform.position = destination.position;
            }
            else{

                DebugManager.instance.PrintDebug("목적지 없음");
            }
        }
        
    }
}
