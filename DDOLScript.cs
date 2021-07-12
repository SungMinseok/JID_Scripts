using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDOLScript : MonoBehaviour
{
    
    void Awake(){
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(this.gameObject);
    }
}
