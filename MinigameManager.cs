using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{   
    public static MinigameManager instance;
    //public bool minigameFlag;

    public bool success, fail;

    void Awake(){
        instance = this;
    }
    void Start()
    {
        
    }

    public void SuccessMinigame(){
        success = true;
    }
}
