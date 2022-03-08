using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonActScript : MonoBehaviour
{
    void Start(){
        this.GetComponent<Button>().onClick.AddListener(()=>PlayDefaultBtnSound());
    }
    void PlayDefaultBtnSound(){
//        Debug.Log("@222");
        SoundManager.instance.PlaySound(SoundManager.instance.defaultBtnSoundName);
    }
}
