using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotkeyManager : MonoBehaviour
{
    InventoryManager theInven;
    SoundManager theSound;
    MenuManager theMenu;
    UIManager theUI;

    void Start(){
        theInven = InventoryManager.instance;
        theSound = SoundManager.instance;
        theMenu = MenuManager.instance;
        theUI = UIManager.instance;
    }
    void Update()
    {
        for(int i=1;i<12;i++){
            if(Input.GetKeyDown((KeyCode)(48+i))){
                theInven.PushItemBtn(i-1);
            }
        }
        if(Input.GetKeyDown(KeyCode.Minus)){
            theInven.PushItemBtn(10);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha0)){
            theInven.PushItemBtn(9);
        }

        if(Input.GetKey(KeyCode.LeftControl)&&Input.GetKeyDown(KeyCode.M)){
            theSound.SetVolumeBGM(0);
            //Debug.Log("#3434");
        }


        if(Input.GetButtonUp("Cancel")){
            if(!PlayerManager.instance.isActing && !UIManager.instance.waitTutorial){

                if(theMenu.popUpOnWork.activeSelf){
                    theMenu.popUpOnWork.SetActive(false);
                }
                // else if(theMenu.savePanel.activeSelf){
                //     theMenu.savePanel.SetActive(false);
                // }
                else if(theMenu.popUpPanel.activeSelf){
                    theMenu.popUpPanel.SetActive(false);
                }
                else if(theMenu.popUpPanel1.activeSelf){
                    theMenu.popUpPanel1.SetActive(false);
                }
                else if(theMenu.loadPanel.activeSelf){
                    theMenu.loadPanel.SetActive(false);
                }
                else if(theMenu.collectionPanel.activeSelf){
                    theMenu.collectionPanel.SetActive(false);
                }
                else if(theMenu.settingPanel.activeSelf){
                    theMenu.settingPanel.SetActive(false);
                }
                else{
                    //MenuManager.instance.menuPanel.SetActive(!MenuManager.instance.menuPanel.activeSelf);
                    MenuManager.instance.ToggleMenuPanel(!MenuManager.instance.menuPanel.activeSelf);
                }

            }
            else{
                if(theMenu.savePanel.activeSelf){
                    theMenu.savePanel.SetActive(false);
                }
                else if(theUI.ui_book.activeSelf){
                    theUI.ui_book.SetActive(false);
                }
            }
        }
        
        if(Input.GetKey(KeyCode.LeftAlt)&&Input.GetKeyDown(KeyCode.Return)){
            MenuManager.instance.SetWindowedMode(!DBManager.instance.localData.isWindowedMode);
            //Debug.Log("#3434");
        }
    }
}
