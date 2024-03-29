using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Minigame3Script : MonoBehaviour
{
    public static Minigame3Script instance;

    [Header("0 : 죽은척물약(4), 1 : 회복물약(8)")]
    //x : 7.5  y : 4
    [Header("[Game Settings]─────────────────")]
    public string[] recipes = new string[3];


    
    [Space]
    [Header("[Game Objects]─────────────────")]
    public GameObject[] valves;
    public GameObject[] materials;
    public GameObject emptyBottle;
    public GameObject failedBottle;
    public GameObject[] madePotions;
    public GameObject note;
    public GameObject notePage;
    public GameObject mushroom;
    public GameObject openedCapsule;
    public GameObject closedCapsule;
    public Animator madeEffect;
    public Location trigLocation;
    public GameObject makingBtnGlow;
    [Space]

    [Header("[Debug]─────────────────")]
    public string curValveOrder;
    public bool checkedMushroom;







    void Awake(){
        instance = this;
    }
    void Start(){
        //waitAphidCreationCycle = new WaitForSeconds(aphidCreationCycle);
    }
    void ResetGameSettings(){
        ResetValves();

        //notePage.SetActive(false);

        openedCapsule.SetActive(false);
        closedCapsule.SetActive(true);

        checkedMushroom = false;
    }
    public void ResetValves(){
        curValveOrder = "";
        emptyBottle.SetActive(true);
        failedBottle.SetActive(false);
        for(int i=0;i<madePotions.Length;i++){
            madePotions[i].SetActive(false);
        }

        for(int i=0;i<materials.Length;i++){
            materials[i].SetActive(true);
            valves[i].GetComponent<Animator>().SetBool("rotate",false);
            valves[i].GetComponent<Button>().enabled = true;
        }
    }
    public void OpenValve(int valveNum){
        SoundManager.instance.PlaySound("potion_valve");
        valves[valveNum].GetComponent<Button>().enabled = false;
        materials[valveNum].SetActive(false);
        valves[valveNum].GetComponent<Animator>().SetBool("rotate",true);
        curValveOrder += valveNum.ToString();
    }
    public void MakePotion(){
        if(checkedMushroom){
            checkedMushroom = false;
            SoundManager.instance.PlaySound("waterdrop");
            // InventoryManager.instance.RemoveItem(13);
            // InventoryManager.instance.RemoveItem(53);
            // InventoryManager.instance.RemoveItem(54);

            mushroom.SetActive(false);
            makingBtnGlow.SetActive(false);

            emptyBottle.SetActive(false);
            switch(curValveOrder){
                case var value when value == recipes[0]:
                    madeEffect.SetTrigger("success");
                    SoundManager.instance.PlaySound("potion_success");
                    madePotions[0].SetActive(true);
                    break;
                case var value when value == recipes[1]:
                    madeEffect.SetTrigger("success");
                    SoundManager.instance.PlaySound("potion_success");
                    madePotions[1].SetActive(true);
                    break;
                default :
                    madeEffect.SetTrigger("fail");
                    SoundManager.instance.PlaySound("potion_fail");
                    failedBottle.SetActive(true);

                    //스팀업적5 (실패물약 제조)
                    SteamAchievement.instance.ApplyAchievements(5);

                    break;
            }
        }
    }
    public void GetPotion(){
        
        SoundManager.instance.PlaySound("glass");
        emptyBottle.SetActive(true);
        for(int i=0;i<madePotions.Length;i++){
            madePotions[i].SetActive(false);
        }
        failedBottle.SetActive(false);

        
        switch(curValveOrder){
            case var value when value == recipes[0]:
                InventoryManager.instance.AddItem(4);
                break;
            case var value when value == recipes[1]:
                InventoryManager.instance.AddItem(8);
                break;
            default :
                InventoryManager.instance.AddItem(32);
                break;
        }
        
        InventoryManager.instance.RemoveItem(13);
        InventoryManager.instance.RemoveItem(53);
        InventoryManager.instance.RemoveItem(54);

        //trigLocation.selectPhase = 1;
    }
    public void OpenNote(){
        notePage.SetActive(true);

        // if(InventoryManager.instance.CheckHaveItem(13)){
        //     mushroom.SetActive(true);
        // }
        // else{
        //     mushroom.SetActive(false);
        // }
    }
    public void PutMushroom(){
        //notePage.SetActive(false);
        if(!InventoryManager.instance.CheckHaveItem(13)
        &&!InventoryManager.instance.CheckHaveItem(53)
        &&!InventoryManager.instance.CheckHaveItem(54)
        ){
            return;
        }

        mushroom.SetActive(true);
        openedCapsule.SetActive(true);
        closedCapsule.SetActive(false);

        checkedMushroom = true;
    }
    // void FixedUpdate(){

    //     if(!PlayerManager.instance.isGameOver &&PlayerManager.instance.isPlayingMinigame && !isPaused){
    //         if(curTimer>0){
    //             curTimer -= Time.fixedDeltaTime;
    //         }
    //         else{
    //             isPaused = true;
    //             // if(score_ant>score_lucky){
    //             //     setScore_ant ++;
    //             // }
    //             // else{

    //             // }




    //             // MinigameManager.instance.SuccessMinigame();
    //         }
    //         timerGauge.fillAmount = curTimer / maxTimerSet;
    //     }
    // }
    void GameSet(){
        
    }
    private void OnEnable() {
        PlayerManager.instance.LockPlayer();

        
        MinigameManager.instance.OpenGuide(161,162);
        //yield return new WaitUntil(()=>!MinigameManager.instance.waitGuidePass);
       // ResetValves();
        ResetGameSettings();

        openedCapsule.SetActive(false);
        closedCapsule.SetActive(true);

        checkedMushroom = false;
        //minigameCoroutine = StartCoroutine(MinigameCoroutine());

        //SceneController.instance.SetSomeConfiner(mapCollider,true);
        //SceneController.instance.virtualCamera.Follow = mapViewPoint;
        UIManager.instance.SetOnlyHudBlock(true);
        UIManager.instance.SetHUD(false);
    }
    void OnDisable(){
        MinigameManager.instance.ExitMinigame();
        //SceneController.instance.SetConfiner(DBManager.instance.curData.curMapNum);
        //SceneController.instance.virtualCamera.Follow = PlayerManager.instance.transform;
        PlayerManager.instance.UnlockPlayer();
        UIManager.instance.SetHUD(true);
        UIManager.instance.SetOnlyHudBlock(false);
        StopAllCoroutines();
    }
}
