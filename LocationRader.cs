using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationRader : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Location")){
            Debug.Log(other.GetComponent<Location>().trigNum);
        }
    }
}

