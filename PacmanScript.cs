using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PacmanScript : MonoBehaviour
{
    public Transform temp0;




    void Update(){
        Debug.Log(Vector2.Distance(gameObject.transform.position,temp0.position));
    }
}
