﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDOLScript : MonoBehaviour
{
    public static DDOLScript instance;
    void Awake(){
        //DontDestroyOnLoad(this.gameObject);
//        Debug.Log(Application.targetFrameRate);
        
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
