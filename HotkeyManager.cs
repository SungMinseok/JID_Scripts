using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotkeyManager : MonoBehaviour
{
    InventoryManager theInven;
    SoundManager theSound;
    MenuManager theMenu;
    UIManager theUI;
    ShopManager theShop;

    void Start(){
        theInven = InventoryManager.instance;
        theSound = SoundManager.instance;
        theMenu = MenuManager.instance;
        theUI = UIManager.instance;
        theShop = ShopManager.instance;
    }
    void Update()
    {
        if(!PlayerManager.instance.isActing
        &&!PlayerManager.instance.isGameOver
        ){

     
#if UNITY_EDITOR || alpha
        
        if(CheatManager.instance.cheatPanel.gameObject.activeSelf){
            return;
        }
            
#endif
       

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
            else if(Input.GetKeyDown(KeyCode.M)){
                if(DBManager.instance.CheckTrigOver(17)||DBManager.instance.CheckTrigOver(18)){
                    //theUI.ui_map.SetActive(!theUI.ui_map.activeSelf);
                    theUI.SetActiveMapUI(!theUI.ui_map.activeSelf);
                }
            }
            else if(Input.GetKeyDown(KeyCode.Tab)){
                theInven.BtnNextPage();
            }
        }

        if(Input.GetKey(KeyCode.LeftControl)&&Input.GetKeyDown(KeyCode.M)){
            theSound.SetVolumeBGM(0);
            //Debug.Log("#3434");
        }


        if(Input.GetButtonUp("Cancel")){
            Debug.Log("4542525");
            if(!PlayerManager.instance.isActing 
            && !UIManager.instance.waitTutorial 
            && !PlayerManager.instance.isPlayingMinigame
            && !PlayerManager.instance.isCaught //221019 추가
            && !PlayerManager.instance.isGameOver   //221019 추가
            ){
            Debug.Log("11");

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
                    //theMenu.collectionPanel.SetActive(false);
                    theMenu.SetCollectionPage(false);
                }
                else if(theMenu.settingPanel.activeSelf){
                    theMenu.settingPanel.SetActive(false);
                }
                else if(theUI.ui_endingGuide.activeSelf){
                    //theUI.ui_endingGuide.SetActive(false);
                    theUI.ToggleEndingGuide(false);
                }
                else if(theUI.ui_map.activeSelf){
                    //theUI.ui_map.SetActive(false);
                    theUI.SetActiveMapUI(false);
                }
                else if(theMenu.ui_coupon.activeSelf){
                    theMenu.ui_coupon.SetActive(false);
                }
                else{
                    //MenuManager.instance.menuPanel.SetActive(!MenuManager.instance.menuPanel.activeSelf);
                    MenuManager.instance.ToggleMenuPanel(!MenuManager.instance.menuPanel.activeSelf);
                }

            }
            //상점
            else if(PlayerManager.instance.isShopping){
                
                if(theMenu.popUpPanel1.activeSelf){
                    theMenu.popUpPanel1.SetActive(false);
                }
                else if(theShop.ui_shop_check.activeSelf){
                    theShop.ui_shop_check.SetActive(false);
                }
                else if(theShop.ui_shop.activeSelf){
                    theShop.CloseShopUI();
                }

            }
            //트리거 중 뜨는 UI
            else{
                if(theMenu.savePanel.activeSelf){
                    theMenu.savePanel.SetActive(false);
                }
                else if(theUI.ui_book.activeSelf){
                    theUI.ui_book.SetActive(false);
                }
                else if(theUI.screenOn){
                    theUI.CloseScreen();
                }
            }
        }
        
        if(Input.GetKey(KeyCode.LeftAlt)&&Input.GetKeyDown(KeyCode.Return)){
            MenuManager.instance.SetWindowedMode(!DBManager.instance.localData.isWindowedMode);
            //Debug.Log("#3434");
        }

#if UNITY_EDITOR || alpha
        if(Input.GetKeyDown(KeyCode.F12)){
            PlayerManager.instance.petHolder.SetActive(!PlayerManager.instance.petHolder.activeSelf);
        }
#endif


    }
}
