using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonActScript : MonoBehaviour,IPointerEnterHandler
{
    void Start(){
        this.GetComponent<Button>().onClick.AddListener(()=>PlayDefaultBtnSound());
    }
    void PlayDefaultBtnSound(){
//        Debug.Log("@222");
        SoundManager.instance.PlaySound(SoundManager.instance.defaultBtnSoundName);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.instance.PlaySound("UI_button_01");
    }
}
