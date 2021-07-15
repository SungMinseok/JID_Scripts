using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
//scene 별로 존재 ( 해당 씬의 NPC 등 미리 할당 )
public class ObjectController : MonoBehaviour
{
    public static ObjectController instance;

    void Awake(){
        instance = this;
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
