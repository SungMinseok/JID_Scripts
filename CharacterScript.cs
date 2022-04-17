using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterScript : MonoBehaviour
{
    public void MoveTo(Transform destination){
        this.transform.position = destination.position;
    }
}
