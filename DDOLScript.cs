using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDOLScript : MonoBehaviour
{
    public static DDOLScript instance;
    void Awake(){
        Application.targetFrameRate = 60;
        //DontDestroyOnLoad(this.gameObject);
        
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
}
