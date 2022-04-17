using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonActScript : MonoBehaviour,IPointerEnterHandler
{
    public enum ButtonHoverSound{
        UI_button_01,
    }
    public enum ButtonClickSound{
        button0,
    }
    public ButtonHoverSound buttonHoverSound;
    public ButtonClickSound buttonClickSound;
    void Start(){
        this.GetComponent<Button>().onClick.AddListener(()=>PlayDefaultBtnSound());
    }
    void PlayDefaultBtnSound(){
//        Debug.Log("@222");
        switch(buttonClickSound){
            case 0 :
                SoundManager.instance.PlaySound(SoundManager.instance.defaultBtnSoundName);
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!GetComponent<Button>().interactable){
            return;
        }
        switch(buttonClickSound){
            case 0 :
                SoundManager.instance.PlaySound("UI_button_01");
                break;
        }
    }
}
