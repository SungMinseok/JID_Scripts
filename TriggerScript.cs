using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.Rendering;
#if !DISABLESTEAMWORKS
using Steamworks;
#endif

public class TriggerScript : MonoBehaviour
{    
    public static TriggerScript instance;
    WaitForSeconds wait100ms = new WaitForSeconds(0.1f);
    WaitForSeconds wait500ms = new WaitForSeconds(0.5f);
    WaitForSeconds wait1000ms = new WaitForSeconds(1);
    WaitForSeconds wait2000ms = new WaitForSeconds(2);
    WaitForSeconds wait3000ms = new WaitForSeconds(3);
    WaitForSeconds wait5000ms = new WaitForSeconds(5);

    WaitUntil waitTalking = new WaitUntil(()=>!PlayerManager.instance.isTalking);
    WaitUntil waitSelecting = new WaitUntil(()=>!PlayerManager.instance.isSelecting);
    WaitUntil waitShopping = new WaitUntil(()=>!PlayerManager.instance.isShopping);
    WaitUntil waitMoving = new WaitUntil(()=>PlayerManager.instance.canMove);
    Coroutine iceGaugeCoroutine;
    
    void Awake()
    {
            instance = this;

    }
    void Start(){
        //SoundManager.instance.PlaySound("button0");
        SceneController.instance.centerViewPoint.localPosition = Vector3.zero;
    }
    public void PreAction(Location location){
        Transform[] objects;
        objects = location.poses;
        
        for(int i=0;i<objects.Length;i++){
            if(objects[i] == null) return;

        }

//트리거 이미 완료됐을 때,
        if(DBManager.instance.CheckTrigOver(location.trigNum)){
            switch(location.trigNum){
                case 4 :
                    objects[0].gameObject.SetActive(false);
                    break;
                case 12 :
                    if(DBManager.instance.CheckTrigOver(41)){
                        objects[0].gameObject.SetActive(false);
                    }

                    break;

                case 15 :
                    objects[4].gameObject.SetActive(false);
                    objects[3].gameObject.SetActive(true);
                    objects[2].gameObject.SetActive(false);
                    break;
                case 17 :
                    UIManager.instance.hud_sub_map.GetComponent<Button>().interactable = true;
                DBManager.instance.TrigOver(91);

                    objects[2].gameObject.SetActive(false);
                    objects[7].gameObject.SetActive(false);//표지판
                    //objects[2].GetComponent<NPCScript>().onJYD = false;
                    //objects[2].transform.position = objects[3].transform.position;


                    if(!DBManager.instance.CheckTrigOver(51)){//광장 노개미 완료 안했으면
                        objects[5].gameObject.SetActive(true);//광장 노개미
                    }
                    if(!DBManager.instance.CheckTrigOver(62)){//버섯농장 앞 수레개미 완료 안했으면
                        objects[6].gameObject.SetActive(true);//버섯농장 수레개미
                    }

                    break;
                case 18 :                
                    UIManager.instance.hud_sub_map.GetComponent<Button>().interactable = true;

                    objects[2].gameObject.SetActive(false);
                    objects[5].gameObject.SetActive(false);
                    objects[6].gameObject.SetActive(true);
                    
                    if(!DBManager.instance.CheckTrigOver(62)){//버섯농장 앞 수레개미 완료 안했으면
                        objects[3].gameObject.SetActive(true);//버섯농장 수레개미
                    }
                    // else{
                    //     objects[3].gameObject.SetActive(true);
                    // }
                    
                    if(!DBManager.instance.CheckTrigOver(51)){//광장 노개미 완료 안했으면
                        objects[7].gameObject.SetActive(true);//광장 노개미
                        //SetObjectActive(objects,7,true);

                    }
                    break;

                    
                case 22 :
                    //location.poses[10].gameObject.SetActive(false);
                        for(int i=0;i<14;i++){
                            location.poses[i].gameObject.SetActive(false);
                        }

                    break;       

                case 25 :
                    
                    objects[0].GetComponent<Location>().isLocked = false;
                    break;       
                case 27 :
                    //objects[1].gameObject.SetActive(false);
                    objects[1].GetComponent<NPCScript>().mainBody.GetComponent<Animator>().SetBool("sleep", true);

                    if(!InventoryManager.instance.CheckHaveItem(1)){
                        objects[0].gameObject.SetActive(true);
                    }
                    //if(!DBManager.instance.CheckTrigOver(location.trigNum)){
                    //    objects[0].gameObject.SetActive(true);
                    //} 
                    break;
                    
#if !demo
                case 9 :
#endif
                case 37 :

                    objects[0].gameObject.SetActive(true);
                    break;
                case 38 :
                    objects[0].GetComponent<Rigidbody2D>().mass = 30f;
                    break;
                case 29 :
                    objects[0].gameObject.SetActive(false);
                    break;
                case 43 :
                    objects[0].GetComponent<Location>().isLocked = false;
                    location.selectPhase = -1;
                    break;
                case 44:
                    // objects[0].gameObject.SetActive(false);//완성본 비활성화
                    // objects[1].gameObject.SetActive(true);//부서진거 활성화
                    break;
                case 46 :
                    if(DBManager.instance.CheckTrigOver(17)&&!DBManager.instance.CheckTrigOver(50)){
                        objects[0].gameObject.SetActive(true);

                    }
                    else{
                        objects[0].gameObject.SetActive(false);

                    }
                    break;
                case 47 :
                    objects[1].gameObject.SetActive(false);
                    objects[2].gameObject.SetActive(false);
                    DBManager.instance.TrigOver(52);
                    break;
                case 51 :
                    objects[0].gameObject.SetActive(false);
                    break;
                case 53 :
                    objects[0].gameObject.SetActive(false);
                    break;
                case 54 :
                    objects[0].gameObject.SetActive(true);
                    objects[1].gameObject.SetActive(false);
                    break;
                case 55 :
                    objects[0].gameObject.SetActive(false);
                    objects[1].gameObject.SetActive(true);
                    break;
                case 62 :
                    objects[0].GetComponent<Location>().isLocked = false;
                    objects[1].gameObject.SetActive(false);
                    break;           
                case 74 :
                    //objects = location.poses;
                    objects[17].GetComponent<Location>().isLocked = false;
                    for(int i=8;i<17;i++){
                        objects[i].gameObject.SetActive(false);
                    }
                    objects[19].gameObject.SetActive(false);
                    objects[20].gameObject.SetActive(false);

                    break;             
                case 76 :
                    objects[0].gameObject.SetActive(true);
                    objects[1].gameObject.SetActive(false);
                    break;  
                case 80 :
                    objects[1].gameObject.SetActive(false);
                    break; 
                case 87 :
                    // if(!InventoryManager.instance.CheckHaveItem(38)){
                    //     InventoryManager.instance.AddItem(38);
                    // }
                    objects[0].gameObject.SetActive(false);
                    objects[1].gameObject.SetActive(true);
                    break;   
 
                case 90 :
                    objects[0].transform.position = objects[1].transform.position;
                    break; 

                    
                default :
                    break;
            }
        }
        else{
             switch(location.trigNum){
                case 9 :
                case 37 :
                    objects[0].gameObject.SetActive(false);
                    break;

                case 17 ://미친수개미에서 노개미 선택
                    objects[5].gameObject.SetActive(false);//광장 노개미
                    break;
                //220901 제거
                // case 18 ://미친수개미에서 수레개미 선택
                //     objects[3].gameObject.SetActive(false);//버섯농장 수레개미
                //     break;
                    
                case 201 :
                    objects[0].gameObject.SetActive(false);
                    objects[1].gameObject.SetActive(false);
                    objects[2].gameObject.SetActive(false);
                    break;
#if demo
                case 45 :
                    objects[0].GetComponent<Location>().isLocked = true;
                    break;
#endif

                case 89 :                
                    objects[0].GetComponent<NPCScript>().mainBody.GetComponent<Animator>().SetBool("think", true);

                    break; 
                default :
                    break;
            }
        }
    }

   //public void Action(Location location, Dialogue[] dialogues = null, Select[] selects = null, Transform[] objects = null){
    public void Action(Location location, bool waitInTrigger = false){
        //Debug.Log("트리거 발동" + trigNum);
        // if(selects != null){
        //     Debug.Log(selects[0].answers[0]);
        // }
        // else{
        //     Debug.Log("선택지 없음");
        // }
//        Debug.Log(location.trigNum + " aa");
        if(waitInTrigger){

        }
        
//StartCoroutine(ActionCoroutine(location, dialogues, selects, objects));
        if(location != null)
            StartCoroutine(ActionCoroutine(location));
//        Debug.Log("44");

    }

    //IEnumerator ActionCoroutine(int trigNum, Dialogue[] dialogues, Select[] selects, Transform[] objects){
    IEnumerator ActionCoroutine(Location location){

#region 액션 실행전
        Dialogue[] dialogues = null;
        Select[] selects = null;
        Transform[] objects = null;

        if(location.dialogues_T !=null){
            dialogues = location.dialogues_T;
        }
        else{
            dialogues = null;
        }
        if(location.selects_T!=null){
            selects = location.selects_T;
        }
        // else{
        //     selects = null;
        // }
        if(location.poses != null){
            objects = location.poses;
        }
        else{
            objects = null;
        }

        
        if(dialogues != null){

            PlayerManager.instance.canMove =false;

    //타겟 지정된 트리거(타겟이 움직임)일 경우 트리거 중 멈춤
            if(location.target != null){
                
                var npc = location.target.GetComponent<NPCScript>();
                npc.isPaused = true;
                npc.patrolInput = 0;

                if(npc.mainBody.position.x > PlayerManager.instance.transform.position.x){
                    PlayerManager.instance.Look("right");
                }
                else{
                    PlayerManager.instance.Look("left");

                }


                //if(npc.animator!=null) npc.animator.SetBool("talk", true);
            }
            else{

                if(!location.notLook){

                    if(location.transform.position.x > PlayerManager.instance.transform.position.x){
                        PlayerManager.instance.Look("right");
                    }
                    else{
                        PlayerManager.instance.Look("left");

                    }
                    PlayerManager.instance.SetTalkCanvasDirection();
                }

            }

            
            if(!location.notZoom){

                if(GameManager.instance.mode_zoomWhenInteract){

                    SceneController.instance.SetCameraDefaultZoomIn();
                    SceneController.instance.SetSomeConfiner(SceneController.instance.mapZoomBounds[DBManager.instance.curData.curMapNum]);
                }
            }



            // 메인 HUD 비활성화
            //if(location.trigNum<300){
                UIManager.instance.SetHUD(false);
            //}

            SceneController.instance.virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 0;
            SceneController.instance.virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 0;
            //트리거 시작시 배경음 감소
            SoundManager.instance.bgmPlayer.volume *= DBManager.instance.bgmFadeValueInTrigger;

        
        }


        if(location.type == LocationType.Trigger){
           
            switch(location.trigNum){
#endregion

#region @999 저장개미 첫만남
            case 999 :
                location.selectPhase = -1;
                DBManager.instance.TrigOver(999);
                DBManager.instance.AntCollectionOver(4);
            
                CameraView(dialogues[0].talker);
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                SetSelect(selects[0]);
                yield return new WaitUntil(()=>!PlayerManager.instance.isSelecting);
                if(GetSelect()==0){
                    SetDialogue(dialogues[1]);
                    yield return waitTalking;
                }
                else{
                    SetDialogue(dialogues[2]);
                    yield return waitTalking;
                }
                SetDialogue(dialogues[3]);
                yield return waitTalking;

                SetSelect(selects[1]);
                yield return new WaitUntil(()=>!PlayerManager.instance.isSelecting);
                if(GetSelect()==0){
                    MenuManager.instance.savePanel.SetActive(true);
                    yield return new WaitUntil(()=>!MenuManager.instance.savePanel.activeSelf);
                }

                SetDialogue(dialogues[4]);
                yield return waitTalking;

                break;
#endregion

#region @998 저장개미 두번째 만남 이후
            case 998 :
                //CameraView(dialogues[0].talker);
                SetDialogue(dialogues[Random.Range(0,3)]);
                yield return waitTalking;

                SetDialogue(dialogues[3]);
                yield return waitTalking;

                SetSelect(selects[0]);
                yield return new WaitUntil(()=>!PlayerManager.instance.isSelecting);
                
                if(GetSelect()==0){
                    MenuManager.instance.savePanel.SetActive(true);
                    yield return new WaitUntil(()=>!MenuManager.instance.savePanel.activeSelf);
                }

                SetDialogue(dialogues[Random.Range(4,9)]);
                yield return waitTalking;
                break;
#endregion

#region @101 상점 - 지렁이 
            case 101 :

                ShopManager.instance.OpenShopUI(0,shopName:CSVReader.instance.GetIndexToString(251,"sysmsg")
                ,/* new int[]{5,12} */new ShopSales[]{new ShopSales(5,10),new ShopSales(5,25),new ShopSales(5,50)});
                yield return waitShopping;

                if(ShopManager.instance.lastBuyItemIndex != -1){

                    SetDialogue(dialogues[0]);
                    yield return waitTalking;
                }



                break;
#endregion

#region @102 상점 - 귀뚜라미
            case 102 :

                ShopManager.instance.OpenShopUI(1,shopName:CSVReader.instance.GetIndexToString(252,"sysmsg")
                ,/* new int[]{19,16,3} */new ShopSales[]{new ShopSales(19),new ShopSales(16),new ShopSales(3)});//붉은 산딸기 / 보라색 열매 / 사과 조각
                yield return waitShopping;


                if(ShopManager.instance.lastBuyItemIndex != -1){

                    SetDialogue(dialogues[0]);
                    yield return waitTalking;
                }

                break;
#endregion

#region @103 상점 - 벼룩
            case 103 :
                //무지개개미옷,줄무늬개미옷
                ShopManager.instance.OpenShopUI(2,shopName:CSVReader.instance.GetIndexToString(253,"sysmsg")
                ,/* new int[]{24,18,25} */new ShopSales[]{new ShopSales(18),new ShopSales(25)});
                yield return waitShopping;

                if(ShopManager.instance.lastBuyItemIndex != -1){

                    //dialogues[0].talker.GetComponent<NPCScript>().mainBody.GetComponent<Animator>().SetBool("talk", false);
                    yield return new WaitUntil(()=>dialogues[0].talker.GetComponent<NPCScript>().
                    mainBody.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("flea_idle"));

                    SetDialogue(dialogues[0]);
                    yield return waitTalking;
                }


                break;
#endregion

#region @104 상점 - 땃쥐
            case 104 :

                SetDialogue(dialogues[0]);
                yield return waitTalking;
                ShopManager.instance.OpenShopUI(3,shopName:CSVReader.instance.GetIndexToString(254,"sysmsg")
                ,/* new int[]{23,20} */new ShopSales[]{new ShopSales(23),new ShopSales(20)});//23빨대, 20비밀쪽지
                // if(!DBManager.instance.CheckTrigOver(40)){
                //     ShopManager.instance.OpenShopUI(3,shopName:CSVReader.instance.GetIndexToString(254,"sysmsg")
                //     ,/* new int[]{23,20} */new ShopSales[]{new ShopSales(23),new ShopSales(20)});//23빨대, 20비밀쪽지
                // }
                // else{
                //     ShopManager.instance.OpenShopUI(3,shopName:CSVReader.instance.GetIndexToString(254,"sysmsg")
                //     ,new ShopSales[]{new ShopSales(20)});//20비밀쪽지
                // }
                yield return waitShopping;


                if(ShopManager.instance.lastBuyItemIndex != -1){

                    SetDialogue(dialogues[1]);
                    yield return waitTalking;
                }


                break;
#endregion

#region @105 상점 - 두더지 
            case 105 :
                //병원 방문 후
                if(DBManager.instance.curData.trigOverList.Contains(40)){
                    if(location.selectPhase ==0 ){
                        location.selectPhase = 1;
                        //CameraView(dialogues[8].talker);
                        SetDialogue(dialogues[8]);
                        yield return waitTalking;
                    }
                    else{
                        //CameraView(dialogues[9].talker);
                        SetDialogue(dialogues[9]);
                        yield return waitTalking;
                    }
                }
                else{

                    SetDialogue(dialogues[0]);
                    yield return waitTalking;

                    ShopManager.instance.OpenShopUI(4,shopName:CSVReader.instance.GetIndexToString(255,"sysmsg"),
                    //new ShopSales[]{new ShopSales(14),new ShopSales(11)}//11 : 콩젤리
                    new ShopSales[]{new ShopSales(14)}
                    );
                    yield return waitShopping;

                    if(ShopManager.instance.lastBuyItemIndex == 14){

                        SetDialogue(dialogues[1]);
                        yield return waitTalking;
                        SetDialogue(dialogues[2]);
                        yield return waitTalking;
                        SetDialogue(dialogues[3]);
                        yield return waitTalking;
                        SetDialogue(dialogues[4]);
                        yield return waitTalking;
                        SetDialogue(dialogues[5]);
                        yield return waitTalking;
                    }
                    else if(ShopManager.instance.lastBuyItemIndex == 11){

                        SetDialogue(dialogues[1]);
                        yield return waitTalking;
                        SetDialogue(dialogues[2]);
                        yield return waitTalking;
                        SetDialogue(dialogues[6]);
                        yield return waitTalking;
                    }
                }


                break;
#endregion


#region @1 여긴 어디?
            case 1 :
                SteamAchievement.instance.ApplyAchievements(0);
#if !alpha
                //첫 대화 시작 확인
                // SteamUserStats.GetStat("gs",out int gs);
                // SteamUserStats.SetStat("gs",gs + 1);
                // SteamUserStats.StoreStats();
                SteamAchievement.instance.GetAndSetSteamUserStat("gs");
#endif

                SetDialogue(dialogues[0]);
                yield return wait500ms;
                PlayerManager.instance.tutorialBox_Right.gameObject.SetActive(true);
                
                PlayerManager.instance.tutorialBox_Right.GetChild(1).GetComponent<Animator>().SetBool("activate",true);
                //UIManager.instance.ShowKeyTutorial(GameInputManager.ReadKey("Interact"),argumentIndex:"82",boxType:1);
                yield return waitTalking;

                PlayerManager.instance.Look("left");
                SetDialogue(dialogues[1]);
                yield return waitTalking;
                UIManager.instance.HideKeyTutorial();
                PlayerManager.instance.Look("right");
                //MapManager.instance.virtualCamera.Follow = null;
                //ObjectController.instance.npcs[0].animator.SetTrigger("wakeUp");
                break;
#endregion

#region @2 너드개미 만남
            case 2 :
                SetDialogue(dialogues[0]);
                yield return waitTalking;

                DBManager.instance.AntCollectionOver(1);
                break;
#endregion

#region @3 먹이창고 탈출
            case 3 :
                var nerd_ant = SceneController.instance.npcs[0];
                
                FadeOut();
                yield return wait1000ms;
                SetHUD(false);
                PlayerManager.instance.vignette.SetActive(false);
                objects[1].gameObject.SetActive(true);

                CameraView(nerd_ant.transform);
                yield return wait500ms;
                FadeIn();

                nerd_ant.animator.SetTrigger("wakeUp");
                yield return wait2000ms;
                //MapManager.instance.virtualCamera.Follow = ObjectController.instance.npcs[0].transform;
                nerd_ant.animator.SetTrigger("standUp");
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                //yield return wait2000ms;
                yield return waitTalking;
                SetDialogue(dialogues[1]);
                nerd_ant.animator.SetTrigger("count");
                ////CameraView(objects[0]);
                yield return waitTalking;
                SetDialogue(dialogues[2]);
                nerd_ant.animator.SetTrigger("sweat");
                yield return waitTalking;
                SetDialogue(dialogues[3]);
                nerd_ant.animator.SetTrigger("turn");
                ////CameraView(nerd_ant.transform);
                ShakeCamera(2,3);
                yield return new WaitForSeconds(1.2f);
                nerd_ant.wSet = 1;
                nerd_ant.spriteRenderer.flipX = true;
                yield return waitTalking;
                yield return wait2000ms;
                FadeOut();
                yield return wait1000ms;
                PlayerManager.instance.vignette.SetActive(true);
                objects[1].gameObject.SetActive(false);
                nerd_ant.gameObject.SetActive(false);
                PlayerManager.instance.transform.position = objects[0].position;
                SceneController.instance.SetConfiner(1,true);
                yield return new WaitForSeconds(0.1f);
                SetHUD(true);
                UIManager.instance.hud_state.GetComponent<InteractiveUI>().Cloak();
                //CameraView(PlayerManager.instance.transform);
                FadeIn();

                //MapManager.instance.virtualCamera.Follow = null;
                //ObjectController.instance.npcs[0].animator.SetTrigger("wakeUp");

                break;
#endregion

#region @4 수배지 확인
            case 4 :
            
                objects[0].gameObject.SetActive(false);
                ActivateEffect(1,3);
                SoundManager.instance.PlaySound("wanted_paper");
                ShakeCamera();
                yield return new WaitUntil(()=>!UIManager.instance.onEffect);
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                PlayerManager.instance.Look("left");
                break;
#endregion

#region @5 복도 경비병 만남
            case 5 :

                PlayerManager.instance.wSet = -1;
                yield return wait1000ms;
                PlayerManager.instance.wSet = 0;
                FadeOut();
                yield return wait1000ms;
                SetHUD(false);
                PlayerManager.instance.vignette.SetActive(false);
                objects[1].gameObject.SetActive(true);
                
                SceneController.instance.SetConfiner(2,true);
                yield return wait100ms;

                CameraView(objects[0]);
                yield return wait500ms;
                FadeIn();

                

                objects[0].gameObject.SetActive(true);
                objects[3].gameObject.SetActive(true);
                
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                //SceneController.instance.npcs[1].gameObject.SetActive(true);
                //SceneController.instance.npcs[2].gameObject.SetActive(true);
                yield return wait1000ms;
                yield return wait500ms;

                FadeOut();
                yield return wait1000ms;
                PlayerManager.instance.vignette.SetActive(true);
                objects[1].gameObject.SetActive(false);
                
                PlayerManager.instance.transform.position = objects[2].position;


                //SetHUD(true);
                //CameraView(PlayerManager.instance.transform);
                FadeIn();

                        
                CameraView(PlayerManager.instance.transform);
                MinigameManager.instance.OpenGuide(151,152);
                yield return new WaitUntil(()=>!MinigameManager.instance.waitGuidePass);
                
                objects[0].GetComponent<NPCScript>().onPatrol = true;
                objects[3].GetComponent<NPCScript>().onPatrol = true;
                objects[3].GetComponent<NPCScript>().onRandomDialogue = true;

                objects[4].gameObject.SetActive(true);
                objects[5].gameObject.SetActive(true);
                ForceToPatrol(objects[0].GetComponent<NPCScript>());
                ForceToPatrol(objects[3].GetComponent<NPCScript>());
                break;
#endregion

#region @6 노개미에게 말을 건다.(선택지)
            case 6 :

                DBManager.instance.AntCollectionOver(2);
                if(location.selectPhase == 0){
                    location.selectPhase = 1;
                    //npc.SetBool("talk",true);
                    SetDialogue(dialogues[0]);
                    yield return waitTalking;
                    SetDialogue(dialogues[6]);
                    yield return waitTalking;
                    SetDialogue(dialogues[7]);
                    yield return waitTalking;
                    SetSelect(selects[0]);
                    yield return new WaitUntil(()=>!PlayerManager.instance.isSelecting);
                    if(GetSelect()==0){
                        location.selectPhase = 2;
                        SetDialogue(dialogues[1]);
                        yield return waitTalking;
                        SetSelect(selects[1]);
                        yield return new WaitUntil(()=>!PlayerManager.instance.isSelecting);
                        if(GetSelect()==0){
                            SetDialogue(dialogues[9]);
                            yield return waitTalking;
                        }
                        else if(GetSelect()==1){
                            //선택지 완료 시 -1
                            location.selectPhase = -1;
                            SetDialogue(dialogues[2]);
                            yield return waitTalking;
                            SetDialogue(dialogues[3]);
                            yield return waitTalking;
                            SetDialogue(dialogues[4]);
                            yield return waitTalking;
                            InventoryManager.instance.AddItem(2);
                            SetDialogue(dialogues[8]);
                            yield return waitTalking;
                        }
                    }
                    else if(GetSelect()==1){
                        
                    }
                }
                else if(location.selectPhase == 1){                    
                    SetDialogue(dialogues[7]);
                    yield return waitTalking;
                    SetSelect(selects[0]);
                    yield return new WaitUntil(()=>!PlayerManager.instance.isSelecting);
                    if(GetSelect()==0){
                        location.selectPhase = 2;
                        SetDialogue(dialogues[1]);
                        yield return waitTalking;
                        SetSelect(selects[1]);
                        yield return new WaitUntil(()=>!PlayerManager.instance.isSelecting);
                        if(GetSelect()==0){
                            SetDialogue(dialogues[9]);
                            yield return waitTalking;
                        }
                        else if(GetSelect()==1){
                            //선택지 완료 시 -1
                            location.selectPhase = -1;
                            SetDialogue(dialogues[2]);
                            yield return waitTalking;
                            SetDialogue(dialogues[3]);
                            yield return waitTalking;
                            SetDialogue(dialogues[4]);
                            yield return waitTalking;
                            InventoryManager.instance.AddItem(2);
                            SetDialogue(dialogues[8]);
                            yield return waitTalking;
                        }
                    }
                    else if(GetSelect()==1){
                        
                    }
                }
                else if(location.selectPhase == 2){
                    SetDialogue(dialogues[1]);
                    yield return waitTalking;
                    SetSelect(selects[1]);
                    yield return new WaitUntil(()=>!PlayerManager.instance.isSelecting);
                    if(GetSelect()==0){
                        SetDialogue(dialogues[9]);
                        yield return waitTalking;
                    }
                    else if(GetSelect()==1){
                        //선택지 완료 시 -1
                        location.selectPhase = -1;
                        SetDialogue(dialogues[2]);
                        yield return waitTalking;
                        SetDialogue(dialogues[3]);
                        yield return waitTalking;
                        SetDialogue(dialogues[4]);
                        yield return waitTalking;
                        InventoryManager.instance.AddItem(2);
                            SetDialogue(dialogues[8]);
                            yield return waitTalking;
                    }
                }
                break;
#endregion

#region @7 화난 노개미에게 말을 다시 건다.
            case 7 :
                
                dialogues[0].talker.GetComponent<NPCScript>().animator.SetBool("mad", true);
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                dialogues[0].talker.GetComponent<NPCScript>().animator.SetBool("mad", false);
                break;
#endregion

#region @8 [미니게임0 - 종이 오리기] 유치원 센세에게 말을 건다.
            case 8 :
            
                DBManager.instance.AntCollectionOver(6);
                AutoSave();
                //CameraView(objects[0]);
#if !UNITY_EDITOR
                for(int k=0;k<10;k++){
                    PlayerLookObject(dialogues[k].talker);
                    //CameraView(dialogues[k].talker);
                    SetDialogue(dialogues[k]);
                    yield return waitTalking;
                }
#endif
                //CheatManager.instance.InputCheat("minigame 0");
                MinigameManager.instance.StartMinigame(0);
                yield return new WaitUntil(()=>MinigameManager.instance.success);
                //MinigameManager.instance.success = false;
                
                if(MinigameManager.instance.success){
                        dialogues[0].talker.GetComponent<NPCScript>().animator.SetBool("clap", true);
                    yield return wait2000ms;

                    for(int k=10;k<11;k++){
                        PlayerLookObject(dialogues[k].talker);
                        //CameraView(dialogues[k].talker);
                        SetDialogue(dialogues[k]);
                        yield return waitTalking;
                    }

                    PlayerLookObject(dialogues[13].talker);
                    SetDialogue(dialogues[13]);
                    yield return waitTalking;
                        dialogues[0].talker.GetComponent<NPCScript>().animator.SetBool("clap", false);

                    InventoryManager.instance.AddHoney(100,true);
                }

                else{
                    MinigameManager.instance.FailMinigame(3);

                }

                ////CameraView(PlayerManager.instance.transform);


                break;
#endregion

#region @9 꽃핀 가졌을 때 유치원 센세 말걸기
            case 9 :
                
                SetDialogue(dialogues[0]);
                // yield return waitTalking;
                // SetDialogue(dialogues[1]);
                // yield return waitTalking;
                // SetSelect(selects[0]);
                // yield return waitSelecting;
                // if(GetSelect()==0){
                    location.selectPhase = -1;
                    InventoryManager.instance.RemoveItem(6);
                    SetDialogue(dialogues[2]);
                    yield return waitTalking;

#if !demo
                    objects[0].gameObject.SetActive(true);
#endif

                // }
                // else if(GetSelect()==1){
                    
                // }






                break;
#endregion

#region @10 꽃핀을 가진 아이
            case 10 :
                
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                InventoryManager.instance.AddItem(6,activateDialogue:true);
                break;
#endregion

#region @11 수개미방 직전 수레개미 말 걸기
            case 11 :
                
                DBManager.instance.AntCollectionOver(5);
                //CameraView(dialogues[0].talker);
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                InventoryManager.instance.AddItem(7);
                break;
#endregion

#region @12 [미니게임1 - 로메슈제 제조] 술먹는 개미에게 말을 건다.
            case 12 :   
                
                InventoryManager.instance.RemoveItem(7);

                for(int i=0;i<4;i++){
                    //CameraView(dialogues[i].talker);
                    PlayerLookObject(dialogues[i].talker);
                    SetDialogue(dialogues[i]);
                    yield return waitTalking;
                }

                
                MinigameManager.instance.StartMinigame(1);
                yield return new WaitUntil(()=>MinigameManager.instance.success || MinigameManager.instance.fail);

                yield return wait2000ms;
                if(MinigameManager.instance.success){
                    yield return wait2000ms;
                    objects[0].gameObject.SetActive(false);
                    DBManager.instance.TrigOver(41);
                    //CameraView(dialogues[4].talker);
                    PlayerLookObject(dialogues[4].talker);
                    SetDialogue(dialogues[4]);
                    yield return waitTalking;
                    //CameraView(dialogues[5].talker);
                    PlayerLookObject(dialogues[5].talker);
                    SetDialogue(dialogues[5]);
                    yield return waitTalking;
                    InventoryManager.instance.AddItem(10,activateDialogue:true);
                }
                else{

                    //CameraView(dialogues[6].talker);
                    SetDialogue(dialogues[6]);
                    yield return waitTalking;
                }


                break;
#endregion

#region @13 "로메슈제" 미니게임 성공 후 취한 개미 ( 선행 : 12 )
            case 13 :   
                
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                break;
#endregion

#region @14 중독 수개미에게 말을 건다.
            case 14 :   
                
                for(int i=0;i<4;i++){
                    //CameraView(dialogues[i].talker);
                    SetDialogue(dialogues[i]);
                    yield return waitTalking;
                }
                break;
#endregion

#region @15 [미니게임2 - 미친 수개미 피하기] 빛나는 양동이를 클릭한다.
            case 15 :   
            
                DBManager.instance.AntCollectionOver(9);
                AutoSave();

                objects[2].gameObject.SetActive(false);
                objects[3].gameObject.SetActive(true);
                
                PlaySound("curtain_open");
                yield return wait1000ms;
                PlayerManager.instance.Look("left");
                ShakeCamera(intensity: 1.5f, duration: 2f);
                SetDialogue(dialogues[0]);
                yield return waitTalking;


                MinigameManager.instance.StartMinigame(2);
                ////////////yield return new WaitUntil(()=>PlayerManager.instance.isPlayingMinigame);
                // SceneController.instance.SetCameraDefaultZoomOut();
                // SceneController.instance.SetSomeConfiner(SceneController.instance.mapBounds[DBManager.instance.curData.curMapNum]);
                // yield return new WaitUntil(()=>MinigameManager.instance.success || MinigameManager.instance.fail);
                
                
                break;
#endregion

#region @16 "도망" 미니게임 성공 , 끝 맵으로 이동
            case 16 :   
                
                // UIManager.instance.SetFadeOut();
                // yield return wait1000ms;
                objects[2].gameObject.SetActive(false);
                SceneController.instance.virtualCamera.Follow = PlayerManager.instance.transform;
                
                PlayerManager.instance.transform.position = objects[0].position;
                SceneController.instance.SetConfiner(7);

                //SceneController.instance.SetSomeConfiner(SceneController.instance.mapBounds[7]);
                //UIManager.instance.SetFadeIn();
                
                
                break;
#endregion

#region @17 "도망" 미니게임 성공 후, 노개미 만남
            case 17 :   

                DBManager.instance.TrigOver(91);
                objects[2].gameObject.SetActive(false);
                //objects[2].GetComponent<NPCScript>().onJYD = false;
                //objects[2].transform.position = objects[3].transform.position;
                
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                SetDialogue(dialogues[1]);
                yield return waitTalking;
                //UIManager.instance.SetFadeOut();
                LoadManager.instance.FadeOut();
                yield return wait1000ms;



//#if demo

                PlayerManager.instance.transform.position = objects[1].position;
                SceneController.instance.SetConfiner(4,true);
                PlayerManager.instance.Look("left");
                PlayerManager.instance.SetTalkCanvasDirection("right");
                objects[4].gameObject.SetActive(true);
// #else

//                 PlayerManager.instance.transform.position = objects[0].position;
//                 SceneController.instance.SetConfiner(8);
// #endif
                //UIManager.instance.SetFadeIn();
                LoadManager.instance.FadeIn();
        
                
                for(int i=2;i<10;i++){

                    SetDialogue(dialogues[i]);
                    yield return waitTalking;
                }
                
                InventoryManager.instance.AddItem(12,activateDialogue:true,delayTime:1.5f,tutorialID:5);
                //UIManager.instance.hud_sub_map.GetComponent<Button>().interactable = true;
                
                // SetDialogue(dialogues[10]);
                // yield return waitTalking;

                
                FadeOut();
                yield return wait1000ms;
                objects[4].gameObject.SetActive(false);//노개미방의 노개미
                objects[7].gameObject.SetActive(false);//표지판
                FadeIn();
                objects[5].gameObject.SetActive(true);
                objects[6].gameObject.SetActive(true);

                break;
#endregion

#region @18 "도망" 미니게임 성공 후, 수레개미 만남
            case 18 :   
                
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                SetDialogue(dialogues[1]);
                yield return waitTalking;
                //UIManager.instance.SetFadeOut();
                
                LoadManager.instance.FadeOut();
                yield return wait1000ms;

                objects[2].gameObject.SetActive(false);
                objects[3].gameObject.SetActive(true);
                objects[5].gameObject.SetActive(false);
                objects[6].gameObject.SetActive(true);
                objects[4].gameObject.SetActive(true);
                objects[7].gameObject.SetActive(true);

#if demo

                PlayerManager.instance.transform.position = objects[1].position;
                SceneController.instance.SetConfiner(4);
#else

                PlayerManager.instance.transform.position = objects[1].position;
                SceneController.instance.SetConfiner(4, true);
                PlayerManager.instance.Look("left");
                PlayerManager.instance.SetTalkCanvasDirection("right");
                FadeIn();
                
                for(int i=2;i<3;i++){

                    SetDialogue(dialogues[i]);
                    yield return waitTalking;
                }
                
                InventoryManager.instance.AddItem(12,activateDialogue:true,delayTime:1.5f,tutorialID:5);
                //UIManager.instance.hud_sub_map.GetComponent<Button>().interactable = true;
                

                
                FadeOut();
                yield return wait1000ms;
                objects[4].gameObject.SetActive(false);
                DBManager.instance.TrigOver(11);
#endif
                FadeIn();
                
                //220801
                //DBManager.instance.TrigOver(17);
                break;
                
#endregion

#region @19 노개미방 책장
            case 19 :   
                var theUI = UIManager.instance;
                
                // SetDialogue(dialogues[0]);
                // yield return waitTalking;

                
                while(true){

                    SetSelect(selects[0]);
                    yield return waitSelecting;

                    if(GetSelect()==3) break;
                    PlaySound("book_open");
                    switch(GetSelect()){
                        case 0 :
                        case 1 :
                        case 2 :
                        //case 4 :
                            theUI.OpenBookUI(0,GetSelect());
                            break;
                        //case 3 :
                        //    theUI.OpenBookUI(1,GetSelect());
                        //    break;
                    }
                    yield return new WaitUntil(()=>!UIManager.instance.ui_book.activeSelf);
                    PlaySound("book_close");
                
                }
                    
                
                
                break;
#endregion

#region @21 요리사개미
            case 21 :
                
                DBManager.instance.AntCollectionOver(8);
                
                    SetDialogue(dialogues[0]);
                    yield return waitTalking;

                    CameraView(objects[0]);
                    yield return wait2000ms;

                    SetDialogue(dialogues[2]);
                    yield return waitTalking;

                    dialogues[0].talker.GetComponent<NPCScript>().NpcLookObject(objects[0]);
                    SetDialogue(dialogues[3]);
                    yield return waitTalking;

                    dialogues[0].talker.GetComponent<NPCScript>().NpcLookObject(PlayerManager.instance.transform);
                    SetDialogue(dialogues[1]);
                    yield return waitTalking;
                
                break;
#endregion

#region @22 요리사 로메슈제 (노랑젤리 구출)
            case 22 :   
                
                
                //로메슈제 보유시
                //if(location.selectPhase == 0){
                    //location.selectPhase = 1;

                SetDialogue(dialogues[0]);
                yield return waitTalking;

                if(InventoryManager.instance.CheckHaveItem(10)){

                    SetDialogue(dialogues[1]);
                    yield return waitTalking;
                    SetSelect(selects[0]);
                    yield return new WaitUntil(()=>!PlayerManager.instance.isSelecting);
                    if(GetSelect()==0){

                        location.selectPhase = -1;

                        InventoryManager.instance.RemoveItem(10);

                        FadeOut();
                        yield return wait1000ms;
                        for(int i=9;i<14;i++){
                            objects[i].gameObject.SetActive(false);
                        }
                        objects[15].gameObject.SetActive(false);
                        objects[16].gameObject.SetActive(false);
                        for(int i=1;i<9;i++){
                            objects[i].GetComponent<Animator>().SetBool("drunken", true);
                        }
                        objects[0].GetComponent<NPCScript>().mainBody.GetComponent<Animator>().SetBool("drunken", true);
                        objects[0].GetComponent<NPCScript>().mainBody.GetComponent<SortingGroup>().sortingOrder = 1;
                        objects[0].GetComponent<NPCScript>().mainBody.localPosition = new Vector2(0,1.61f);
                        
                        //요리사 y : 0 > 1.61, sortingGroup orderLayer : 0 > 1
                        
                        objects[14].GetChild(1).GetComponent<Animator>().SetBool("hang", false);
                        objects[14].gameObject.SetActive(true);
                        SoundManager.instance.PlaySound("water_pour");
                        yield return wait2000ms;
                        FadeIn();

                        for(int i=0;i<11;i++){
                            SetDialogue(dialogues[i+2]);
                            yield return waitTalking;
                        }

                        FadeOut();
                        yield return wait1000ms;
                        objects[14].gameObject.SetActive(false);
                        yield return wait1000ms;
                        FadeIn();

                        SteamAchievement.instance.ApplyAchievements(7);

                    }
                    else if(GetSelect()==1){
                        
                    }
                }
                else{
                    
                    for(int i=13;i<15;i++){
                        SetDialogue(dialogues[i]);
                        yield return waitTalking;
                    }
                }

                break;
#endregion
           
#region @23 진딧물농장 미니게임4
            case 23 :

                FadeOut();
                yield return wait1000ms;
                SetHUD(false);
                PlayerManager.instance.vignette.SetActive(false);
                PlayerManager.instance.redVignette.SetActive(false);
                
                SceneController.instance.SetConfiner(15,true);
                PlayerManager.instance.transform.position = objects[5].position;
                yield return wait100ms;

                //CameraView(objects[0]);
                yield return wait500ms;
                FadeIn();

#if !UNITY_EDITOR
                for(int k=0;k<9;k++){
                    //CameraView(dialogues[k].talker);
                    
                    SceneController.instance.virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 0;
                    SceneController.instance.virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 0;
                    SetDialogue(dialogues[k]);
                    yield return waitTalking;
                    
                }
                yield return wait500ms;
#endif

                SetSelect(selects[0]);
                yield return new WaitUntil(()=>!PlayerManager.instance.isSelecting);
                if(GetSelect()==2){
                    break;
                }
                location.selectPhase = -1;


                SceneController.instance.SetCameraDefaultZoomOut();

                MinigameManager.instance.StartMinigame(4);

                yield return new WaitUntil(()=>MinigameManager.instance.success || MinigameManager.instance.fail);
                        

                objects[17].GetComponent<Location>().isLocked = false;
                PlayerManager.instance.transform.position = objects[6].position;
                PlayerManager.instance.Look("right");

                PlayerManager.instance.vignette.SetActive(true);
                PlayerManager.instance.redVignette.SetActive(true);
                
                //게임 성공 시
                if(MinigameManager.instance.success){
DBManager.instance.AntCollectionOver(12);
                    for(int i=8;i<17;i++){
                        objects[i].gameObject.SetActive(false);
                    }
                    yield return wait1000ms;

                    CameraView(dialogues[9].talker);

                    objects[7].gameObject.SetActive(true);
                    SceneController.instance.SetCameraDefaultZoomIn();

                    yield return wait1000ms;
                    yield return wait1000ms;
                    if(GetSelect()==0){
                        CameraView(dialogues[9].talker);
                        
                        SetDialogue(dialogues[9],onCameraCenter:true);
                        SetHUD(false);
                        yield return waitTalking;
                    }
                    else if(GetSelect()==1){
                        SetHUD(false);
                        CameraView(dialogues[10].talker);
                        SetDialogue(dialogues[10],onCameraCenter:true);
                        yield return waitTalking;
                    }

                    FadeOut();

                    if(GetSelect()==0){
                        InventoryManager.instance.AddItem(17);
                    }
                    else if(GetSelect()==1){
                        InventoryManager.instance.AddItem(0);
                    }
                    yield return wait2000ms;
                    objects[7].gameObject.SetActive(false);

                    FadeIn();

                    //지키기 : 여왕지지자
                    if(GetSelect()==0){
                        SteamAchievement.instance.ApplyAchievements(9);

                    }
                    //훔치는거 도와주기 : 공주지지자
                    else if(GetSelect()==1){
                        SteamAchievement.instance.ApplyAchievements(8);

                    }

                    DBManager.instance.TrigOver(74);
                //FadeIn();
                
                    objects[19].gameObject.SetActive(false);
                    objects[20].gameObject.SetActive(false);

                }
                else{
                    yield return wait3000ms;

                }

                break;
#endregion
            
#region @24 알번데기방 - 꼰대개미와 대화
            case 24 :
DBManager.instance.AntCollectionOver(14);

                //CameraView(dialogues[0].talker);
                SetDialogue(dialogues[0]);
                yield return waitTalking;

                SetSelect(selects[0]);
                yield return waitSelecting;
                
                //CameraView(dialogues[1].talker);
                SetDialogue(dialogues[1]);
                yield return waitTalking;

                SetSelect(selects[1]);
                yield return waitSelecting;

                //CameraView(dialogues[2].talker);
                SetDialogue(dialogues[2]);
                yield return waitTalking;

                
                PlayerManager.instance.Look("left");

                //CameraView(dialogues[3].talker);
                SetDialogue(dialogues[3]);
                yield return waitTalking;

                PlayerManager.instance.Look("right");

                //CameraView(dialogues[4].talker);
                SetDialogue(dialogues[4]);
                yield return waitTalking;

                SetSelect(selects[2]);
                yield return waitSelecting;
                
                //CameraView(dialogues[5].talker);
                SetDialogue(dialogues[5]);
                yield return waitTalking;
                
                location.selectPhase = -1;

                InventoryManager.instance.AddItem(37);

                break;
#endregion
            
#region @25 대왕일개미방 입구
            case 25 :

                if(!DBManager.instance.CheckTrigOver(24)){
                    SoundManager.instance.PlaySound("locked_door");

                    SetDialogue(dialogues[0]);
                    yield return waitTalking;
                }
                else{
                    
                    InventoryManager.instance.RemoveItem(37);

                    SetDialogue(dialogues[1]);
                    yield return waitTalking;

                    SetSelect(selects[0]);
                    yield return waitSelecting;
                    if(GetSelect()==0){
                        location.selectPhase = -1;
                        PlayerManager.instance.transform.position = objects[1].transform.position;
                        SceneController.instance.SetConfiner(11);
                        objects[0].GetComponent<Location>().isLocked = false;
                        
                        Action(objects[3].GetComponent<Location>());
                        SoundManager.instance.SetBgmByMapNum(11);

                        DBManager.instance.MapOver(11);
                        
                                
            // SceneController.instance.virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 0;
            // SceneController.instance.virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 0;
                         //SceneController.instance.SetCameraDefaultZoomIn();
                         SceneController.instance.SetSomeConfiner(SceneController.instance.mapZoomBounds[DBManager.instance.curData.curMapNum],true);
                         yield return new WaitUntil(()=>!PlayerManager.instance.isActing);
                        //PlayerManager.instance.wSet = -1;

                    }
                    location.preserveTrigger = false;
                }

                break;
#endregion        

#region @26 대왕일개미방 내부 
            case 26 :

                if(PlayerManager.instance.equipments_id[1] == -1){

                    SetDialogue(dialogues[17]);
                    yield return waitTalking;

                    UIManager.instance.SetGameOverUI(19);
                    objects[3].GetComponent<Animator>().SetBool("kill",true);
                    yield return wait500ms;                    
                    PlayerManager.instance.playerBody.localScale = Vector2.zero;


                    SceneController.instance.virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 0;
                    SceneController.instance.virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 0;
            
                }
                else{
                        location.selectPhase = -1;
DBManager.instance.AntCollectionOver(16);
                    CameraView(dialogues[0].talker);
                    SetDialogue(dialogues[0]);
                    yield return waitTalking;
                    SceneController.instance.virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 0;
                    SceneController.instance.virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 0;
            
                    SetSelect(selects[0]);
                    yield return waitSelecting;
                    
                    if(GetSelect()==0){
                        for(int i=1;i<6;i++){
                    CameraView(dialogues[i].talker);
                            SetDialogue(dialogues[i]);
                            yield return waitTalking;
                        }
                        if(InventoryManager.instance.CheckHaveItem(3)
                        ||InventoryManager.instance.CheckHaveItem(16)
                        ||InventoryManager.instance.CheckHaveItem(19)
                        ){
                                    
                    CameraView(dialogues[6].talker);
                            SetDialogue(dialogues[6]);
                            yield return waitTalking;
                    CameraView(dialogues[7].talker);
                            SetDialogue(dialogues[7]);
                            yield return waitTalking;



                            //보유한 과일에 따른 선택지 노출 220729
                            int[] tempIntArr26 = new int[3]{3,16,19};

                            List<string> tempStringList26 = new List<string>(){};//선택지 배열 전환용
                            List<string> tempStringList26_1 = new List<string>();//아이템 ID 스트링 배열 전환용
                            List<int> tempIntList = new List<int>();//아이템 ID
                            for(int i=0;i<3;i++){
                                if(InventoryManager.instance.CheckHaveItem(tempIntArr26[i])){
                                    tempStringList26_1.Add(DBManager.instance.cache_ItemDataList[tempIntArr26[i]].name.ToString());
                                    tempIntList.Add(tempIntArr26[i]);
                                    tempStringList26.Add("17");
                                }
                            }
                            tempStringList26_1.Add("44");//배열 길이 초과 떄문에 넣음(의미X)
                            tempStringList26.Add("44");

                            string [] tempArg26 = tempStringList26_1.ToArray();

                            Select tempSelect26 = new Select();
                            tempSelect26.answers = tempStringList26.ToArray();

                            SetSelect(tempSelect26, tempArg26);
                            yield return waitSelecting;     
    

                            // SetSelect(selects[1]);
                            // yield return waitSelecting;
                            if(GetSelect()==tempSelect26.answers.Length-1){//주지 않는다.
                                
                    CameraView(dialogues[15].talker);
                                SetDialogue(dialogues[15]);
                                yield return waitTalking;
                                FadeOut();
                                yield return wait1000ms;
                                SceneController.instance.SetConfiner(20);
                                PlayerManager.instance.transform.position = objects[0].transform.position;
                                FadeIn();
                                
                    CameraView(dialogues[16].talker);
                                SetDialogue(dialogues[16]);
                                yield return waitTalking;

                                //방나가짐
                            }
                            else{//과일을 준다.
                                //선택한 과일에 맞는 아이템 ID 삭제
                                InventoryManager.instance.RemoveItem(tempIntList[GetSelect()]);
                                //objects[3].GetComponent<Animator>().SetTrigger("eat");

                                for(int i=8;i<14;i++){
                                    if(i==11){
                                objects[3].GetComponent<Animator>().SetTrigger("eat");

                                    }
                    CameraView(dialogues[i].talker);
                                    SetDialogue(dialogues[i]);
                                    yield return waitTalking;
                                }
                                DBManager.instance.TrigOver(81);
                                location.selectPhase = -1;
                            }

                        }
                        else{
                            //방나가짐
                                FadeOut();
                                yield return wait1000ms;
                                SceneController.instance.SetConfiner(20);
                                PlayerManager.instance.transform.position = objects[0].transform.position;
                                FadeIn();

                                
                    CameraView(dialogues[16].talker);
                                SetDialogue(dialogues[16]);
                                yield return waitTalking;
                        }
                    }
                    else{
                    CameraView(dialogues[14].talker);
                        SetDialogue(dialogues[14]);
                        yield return waitTalking;

                        //죽음
                        UIManager.instance.SetGameOverUI(19);
                    objects[3].GetComponent<Animator>().SetBool("kill",true);
                    yield return wait500ms;
                    PlayerManager.instance.playerBody.localScale = Vector2.zero;
                    }


                }

                break;
#endregion

#region @27 과학자개미와 대화
            case 27 :

                DBManager.instance.AntCollectionOver(11);

                    PlayerManager.instance.SetTalkCanvasDirection();

                    var animator = objects[1].GetComponent<NPCScript>().mainBody.GetComponent<Animator>();

                    if(location.selectPhase == 0){
                        location.selectPhase = 1;

                        for(int i=8;i<=14;i++){
                            if(i==12) animator.SetBool("stand",true);
                            
                            SetDialogue(dialogues[i]);
                            yield return waitTalking;

                        }
                        
                        //CameraView(dialogues[0].talker);
                        //yield return new WaitUntil(()=>animator.GetCurrentAnimatorStateInfo(0).IsName("Ant_Scientist_idle"));
                        SetDialogue(dialogues[0]);
                        yield return waitTalking;
                    }

                    animator.SetBool("stand",true);
                    SetDialogue(dialogues[6]);
                    yield return waitTalking;
                    
                    if(InventoryManager.instance.CheckHaveItem(10)){
                        //PlayerManager.instance.SetTalkCanvasDirection("left");
                        //CameraView(dialogues[1].talker);
                        //yield return new WaitUntil(()=>animator.GetCurrentAnimatorStateInfo(0).IsName("Ant_Scientist_idle"));
                        SetDialogue(dialogues[1]);
                        yield return waitTalking;
                        SetSelect(selects[0]);
                        yield return waitSelecting;
                        if(GetSelect()==0){
                            location.selectPhase = -1;
                            InventoryManager.instance.RemoveItem(10);
                            //CameraView(dialogues[2].talker);
                            SetDialogue(dialogues[2]);
                            yield return waitTalking;
                            //CameraView(dialogues[3].talker);
                            SetDialogue(dialogues[3]);
                            yield return waitTalking;
                            animator.SetBool("stand",false);
                            animator.SetBool("mess", true);
                            SoundManager.instance.PlaySound("makingsound");
                            objects[2].GetComponent<ParticleSystem>().Play();

                            yield return new WaitForSeconds(5.3f);

                            SoundManager.instance.PlaySound("minigame_complete");
                            objects[2].GetComponent<ParticleSystem>().Stop();
                            animator.SetBool("success", true);
                            animator.SetBool("mess", false);

                            yield return wait100ms;
                            objects[0].gameObject.SetActive(true);
                            //CameraView(dialogues[4].talker);
                            SetDialogue(dialogues[4]);
                            yield return waitTalking;
                            animator.SetBool("stand",true);
                            animator.SetBool("success", false);
                            
                            SetDialogue(dialogues[5]);
                            yield return waitTalking;
                            FadeOut();
                            yield return wait1000ms;
                            //objects[0].gameObject.SetActive(true);
                            animator.SetBool("stand",false);
                            animator.SetBool("sleep", true);
                            //objects[1].gameObject.SetActive(false);
                            yield return wait1000ms;
                            FadeIn();
                        }
                        //내가 웃기게 생겼다니.. 제발 달라고 해도 안줄래.
                        else{
                            
                            for(int i=18;i<=19;i++){
                                
                                SetDialogue(dialogues[i]);
                                yield return waitTalking;
                            }
                            animator.SetBool("stand",false);
                        }
                    }
                    else{

                        //로메슈제 미소지시 첫번째 상호작용>
                        if(location.selectPhase == 1){
                            location.selectPhase = 2;
                            for(int i=15;i<=17;i++){
                                
                                SetDialogue(dialogues[i]);
                                yield return waitTalking;
                            }
                            animator.SetBool("stand",false);
                        }
                        else{
                            for(int i=18;i<=19;i++){
                                
                                SetDialogue(dialogues[i]);
                                yield return waitTalking;
                            }
                            animator.SetBool("stand",false);
                        }
                    }

                //}

                break;
#endregion

#region @28 과학자/박사개미의 완성품 획득
            case 28 :
                if(location.selectPhase == 0){
                    location.selectPhase = 1;
                    SetDialogue(dialogues[0]);
                    yield return waitTalking;
                }
                SetSelect(selects[0]);
                yield return waitSelecting;
                if(GetSelect()==0){
                    location.selectPhase = -1;
                    objects[0].gameObject.SetActive(false);
                    InventoryManager.instance.AddItem(1);
                }
                else{
                    SteamAchievement.instance.ApplyAchievements(15);

                }

                //location.selectPhase = -1;

                break;
#endregion
//병원에 누워있는 병사 개미
#region 29
            case 29 :
DBManager.instance.AntCollectionOver(15);


                if(location.selectPhase == 0){
                    location.selectPhase = 1;
                    SetDialogue(dialogues[0]);
                    yield return waitTalking;
                }
                SetDialogue(dialogues[1]);
                yield return waitTalking;

                if(InventoryManager.instance.CheckHaveItem(8)){
                    SetSelect(selects[0]);
                    yield return waitSelecting;
                    if(GetSelect()==0){
                        location.selectPhase = -1;
                        InventoryManager.instance.RemoveItem(8);
                                
                        SetDialogue(dialogues[2]);
                        yield return waitTalking;
                        SetDialogue(dialogues[3]);
                        yield return waitTalking;
                        FadeOut();
                        yield return wait1000ms;
                        SoundManager.instance.PlaySound("drinking_01");
                        yield return wait500ms;
                        yield return wait100ms;
                        SoundManager.instance.PlaySound("drinking_02");
                        FadeIn();
                        dialogues[0].talker.GetComponent<NPCScript>().mainBody.GetComponent<Animator>().SetBool("healed",true);
                        SoundManager.instance.PlaySound("healing_bubble");
                        //yield return wait500ms;
                        yield return wait1000ms;

                        
                        SetDialogue(dialogues[8]);
                        yield return waitTalking;
                        SetDialogue(dialogues[4]);
                        yield return waitTalking;
                        SetDialogue(dialogues[5]);
                        yield return waitTalking;
                        SetDialogue(dialogues[6]);
                        yield return waitTalking;
                        InventoryManager.instance.AddItem(22);


                        FadeOut();
                        yield return wait2000ms;
                        objects[0].gameObject.SetActive(false);
                        FadeIn();


                    }
                    else{
                        SetDialogue(dialogues[7]);
                        yield return waitTalking;
                    }

                }


                break;
#endregion

#region 30  병원 입장여부 체크
            case 30 :

                if(objects[0].GetComponent<Location>().isLocked ){

                    if(location.selectPhase == 0){

                        //CameraView(dialogues[0].talker);
                        SetDialogue(dialogues[0]);
                        yield return waitTalking;
                        location.selectPhase = 1;
                    }

                    //히든월드 입장했을 경우
                    if(DBManager.instance.curData.trigOverList.Contains(39)){
                        
                        //CameraView(dialogues[1].talker);
                        SetDialogue(dialogues[1]);
                        yield return waitTalking;
                    }
                    else{

                        //CameraView(dialogues[2].talker);
                        SetDialogue(dialogues[2]);
                        yield return waitTalking;
                        objects[0].GetComponent<Location>().isLocked = false;
                        location.GetComponent<BoxCollider2D>().enabled = false;
                    }
                }
                
                
                
                break;
#endregion 

#region @31 룰렛 ( 미니게임 5 )
            case 31 :
                PlayerManager.instance.Look("left");

                Animator tempAnimator = dialogues[0].talker.GetComponent<NPCScript>().mainBody.GetComponent<Animator>();

                tempAnimator.SetBool("up", true);
                yield return new WaitUntil(()=>tempAnimator.GetCurrentAnimatorStateInfo(0).IsName("worm_idle"));


                if(location.selectPhase == 0){
                    location.selectPhase = 1;
                    SetDialogue(dialogues[0]);
                    yield return waitTalking;
                }
                else{
                    SetDialogue(dialogues[3]);
                    yield return waitTalking;
                }

                SetSelect(selects[0]);
                yield return waitSelecting;

                if(GetSelect()==0){
                    
                    SetDialogue(dialogues[1]);
                    yield return waitTalking;
                    
                    MinigameManager.instance.StartMinigame(5);
                    //MinigameManager.instance.minigameScriptTransforms[5].gameObject.SetActive(true);
                    //yield return waitMoving;
                    yield return new WaitUntil(()=>MinigameManager.instance.success);

                    if(DBManager.instance.curData.roulettePlayCount >= 20 && !InventoryManager.instance.CheckHaveItem(52)){
                            
                        SetDialogue(dialogues[4]);
                        yield return waitTalking;

                        InventoryManager.instance.AddItem(52,activateDialogue:true,delayTime:1f);
                    }
                
                }

                else{
                    
                    SetDialogue(dialogues[2]);
                    yield return waitTalking;
                }
                tempAnimator.SetBool("up", false);

                break;
#endregion 
             
//물약제조 ( 미니게임 3 )
#region @32 물약제조 ( 미니게임 3 )
            case 32 :
                if(location.selectPhase ==0 ){

                    MinigameManager.instance.StartMinigame(3);
                    //MinigameManager.instance.minigameScriptTransforms[3].gameObject.SetActive(true);
                    yield return new WaitUntil(()=>MinigameManager.instance.success);

                }
                else{
                    SetDialogue(dialogues[0]);
                    yield return waitTalking;
                    location.preserveTrigger = false;
                }

                
                break;
#endregion 
              
//지네상점, 쥐며느리상점
#region @33, @34
            case 33 :
            case 34 :
                //CameraView(dialogues[0].talker);
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                break;
#endregion 

#region @35 자판기
            case 35 :

                if(location.selectPhase == 0){
                    location.selectPhase ++;
                    SetDialogue(dialogues[0]);
                    yield return waitTalking;
                }
                else if(location.selectPhase == 1){
                    location.selectPhase ++;
                    SetDialogue(dialogues[1]);
                    yield return waitTalking;
                    PlaySound("kick_vending_1");
                    PlayerManager.instance.animator.SetTrigger("kick");
                    SetDialogue(dialogues[2]);
                    yield return waitTalking;
                }
                else if(location.selectPhase == 2){
                    location.selectPhase ++;
                    SetDialogue(dialogues[1]);
                    yield return waitTalking;
                    PlaySound("kick_vending_1");
                    PlayerManager.instance.animator.SetTrigger("kick");
                    SetDialogue(dialogues[2]);
                    yield return waitTalking;
                }
                else if(location.selectPhase == 3){
                    location.selectPhase = -1;
                    DBManager.instance.TrigOver(35);
                    SetDialogue(dialogues[3]);
                    yield return waitTalking;
                    PlaySound("kick_vending_1");
                    PlayerManager.instance.animator.SetTrigger("kick");
                    SetDialogue(dialogues[4]);
                    yield return waitTalking;
                    PlaySound("vending_price");
                    objects[0].gameObject.SetActive(true);

                    objects[0].GetComponent<Rigidbody2D>().AddForce(new Vector2(0,1) * (7), ForceMode2D.Impulse);



                    SetDialogue(dialogues[5]);
                    yield return waitTalking;
                    objects[0].GetComponent<ItemScript>().isAvailable = true;
                }
                //     SetDialogue(dialogues[1]);
                //     yield return waitTalking;
                    
                //     SetSelect(selects[0]);
                //     yield return waitSelecting;
                //     if(GetSelect()==0){
                //         InventoryManager.instance.AddDirt(20);
                //         SoundManager.instance.PlaySound("dirt_charge");


                //     }
                // }

                break;
#endregion 
#region @36 거미상점
            case 36 :

                //if(location.selectPhase == 0){
                //    location.selectPhase = 1;
                    SetDialogue(dialogues[0]);
                    yield return waitTalking;
                    InventoryManager.instance.AddDirt(20);
                    SoundManager.instance.PlaySound("dirt_charge");
                //}
                // else if(location.selectPhase == 1){
                //     SetDialogue(dialogues[1]);
                //     yield return waitTalking;
                    
                //     SetSelect(selects[0]);
                //     yield return waitSelecting;
                //     if(GetSelect()==0){
                    location.selectPhase = -1;
                //         InventoryManager.instance.AddDirt(20);
                //         SoundManager.instance.PlaySound("dirt_charge");


                //     }
                // }
                break;
#endregion 

//연못앞
#region 37
            case 37 :
                
                //CameraView(dialogues[0].talker);
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                if(InventoryManager.instance.CheckHaveItem(23)){

                    SetDialogue(dialogues[1]);
                    yield return waitTalking;
                
                    SetSelect(selects[0]);
                    yield return waitSelecting;
                    if(GetSelect()==0){
                        location.selectPhase = -1;
                        InventoryManager.instance.RemoveItem(23);

                        FadeOut();
                        yield return wait2000ms;
                        objects[0].gameObject.SetActive(true);
                        FadeIn();

                        SetDialogue(dialogues[2]);
                        yield return waitTalking;
                    }
                }

                break;
#endregion 


//연못앞
#region 38
            case 38 :
                
                //CameraView(dialogues[0].talker);
                SetDialogue(dialogues[0]);
                yield return waitTalking;

                objects[0].GetComponent<Rigidbody2D>().mass = 30f;

                break;
#endregion 
//히든월드 입장
#region @39
            case 39 :
                objects[0].GetComponent<BoxCollider2D>().enabled = true;
                objects[1].GetComponent<Location>().isLocked = true;
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                
                //0,1,2,7 맵 제외 모두 방문 시 업적 달성
                if(DBManager.instance.curData.mapOverList.Count >= 21/* CSVReader.instance.data_map.Count - 3 */
            
                ){
                    SteamAchievement.instance.ApplyAchievements(11);

                }
                break;
#endregion 

#region @41 로메슈제 미니게임1 재시작
            case 41 :
                if(!InventoryManager.instance.CheckHaveItem(7)){
                    //CameraView(dialogues[0].talker);
                    SetDialogue(dialogues[0]);
                    yield return waitTalking;
                }
                else{
                    int temp41 = Random.Range(1,4);
                    //CameraView(dialogues[1].talker);
                    SetDialogue(dialogues[temp41]);
                    yield return waitTalking;
                    InventoryManager.instance.RemoveItem(7);
                    MinigameManager.instance.StartMinigame(1);
                    yield return new WaitUntil(()=>MinigameManager.instance.success || MinigameManager.instance.fail);

                    yield return wait2000ms;
                    if(MinigameManager.instance.success){

                    yield return wait2000ms;
                        objects[0].gameObject.SetActive(false);
                        //CameraView(dialogues[4].talker);
                        SetDialogue(dialogues[4]);
                        yield return waitTalking;
                        //CameraView(dialogues[5].talker);
                        SetDialogue(dialogues[5]);
                        yield return waitTalking;
                        InventoryManager.instance.AddItem(10);
                        location.preserveTrigger = false;
                    }
                    else{

                        //CameraView(dialogues[6].talker);
                        SetDialogue(dialogues[6]);
                        yield return waitTalking;
                    }
                }
                break;
#endregion 

#region @42 수레개미 꿀방울 재획득
            case 42 :

                if(!DBManager.instance.CheckTrigOver(18)){

                    if(InventoryManager.instance.CheckHaveItem(7)){
                        //CameraView(dialogues[0].talker);
                        SetDialogue(dialogues[0]);
                        yield return waitTalking;
                    }
                    else{
                        //CameraView(dialogues[1].talker);
                        SetDialogue(dialogues[1]);
                        yield return waitTalking;
                            InventoryManager.instance.AddItem(7);
                        
                    }
                }
                else{
                    
                    if(!InventoryManager.instance.CheckHaveItem(7)){
                        SetDialogue(dialogues[2]);
                        yield return waitTalking;
                        InventoryManager.instance.AddItem(7,activateDialogue:true);
                        
                    }
                }
                break;
#endregion 

#region @43 여왕개미방 입장여부 체크 // 최초 통과 후 더이상 진행 X ,
            case 43 :
                if(objects[0].GetComponent<Location>().isLocked ){

                    //if(location.selectPhase == 0){

                        //CameraView(dialogues[0].talker);
                        SetDialogue(dialogues[0]);
                        yield return waitTalking;
                        
                        //location.selectPhase = 1;
                    //}

                    SetSelect(selects[0]);
                    yield return waitSelecting;
                    if(GetSelect()==0){
                        //CameraView(dialogues[1].talker);
                        SetDialogue(dialogues[1]);
                        yield return waitTalking;
                    }
                    else{
                        
                        if(InventoryManager.instance.CheckHaveItem(17)){
                            
                            //CameraView(dialogues[2].talker);
                            SetDialogue(dialogues[2]);
                            yield return waitTalking;


                            objects[0].GetComponent<Location>().isLocked = false;
                            location.selectPhase = -1;
                        }
                        else{
                            
                            //CameraView(dialogues[3].talker);
                            SetDialogue(dialogues[3]);
                            yield return waitTalking;
                        }
                    }

                    
                }
                break;
#endregion 

#region @44 제작대 상호작용
            case 44 :
                if(location.selectPhase == 0){
                    location.selectPhase = 1;
                    //CameraView(dialogues[0].talker);
                    SetDialogue(dialogues[0]);
                    yield return waitTalking;
                }

                SetDialogue(dialogues[1]);
                yield return waitTalking;

                string[] tempArg = new string[4]{
                    DBManager.instance.cache_ItemDataList[9].name.ToString(),
                    DBManager.instance.cache_ItemDataList[33].name.ToString(),
                    DBManager.instance.cache_ItemDataList[34].name.ToString(),
                    ""
                };

                //Debug.Log(DBManager.instance.cache_ItemDataList[9].name.ToString());

                SetSelect(selects[0], tempArg);
                yield return waitSelecting;

                //List<int> tempMaterials = new List<int>();
                bool metrialCheck = false;
                int resultItemID = -1;
                int materialItemID0 = -1;
                int materialItemID1 = -1;
                int materialItemAmount = 1;
                InventoryManager theInven = InventoryManager.instance;

                //얼음 덩이, 장전 권총, 장전 소총
                if(GetSelect()==0){
                    if(theInven.CheckHaveItem(31,5)){
                        metrialCheck = true;
                        materialItemID0 = 31;
                        materialItemAmount = 5;
                        resultItemID = 9;
                    }
                }
                else if(GetSelect()==1){
                    if(theInven.CheckHaveItem(27)&&theInven.CheckHaveItem(30)){
                        metrialCheck = true;
                        materialItemID0 = 27;
                        materialItemID1 = 30;
                        resultItemID = 33;
                    }
                }
                else if(GetSelect()==2){
                    if(theInven.CheckHaveItem(28)&&theInven.CheckHaveItem(30)){
                        metrialCheck = true;
                        materialItemID0 = 28;
                        materialItemID1 = 30;
                        resultItemID = 34;
                    }
                }

                if(GetSelect()!=3){

                    //재료 부족해
                    if(!metrialCheck){
                        SetDialogue(dialogues[2]);
                        yield return waitTalking;
                    }
                    //얼음덩어리/장전된 권총/장전된 소총을 만들까?
                    else{
                        SetDialogue(dialogues[3], DBManager.instance.cache_ItemDataList[resultItemID].name.ToString());
                        yield return waitTalking;

                        SetSelect(selects[1]);
                        yield return waitSelecting;

                        //만들자
                        if(GetSelect()==0){

                            
                            FadeOut();
                            yield return wait1000ms;
                            SoundManager.instance.PlaySound("makingsound");
                            yield return wait5000ms;
                            FadeIn();

                            SetDialogue(dialogues[4], DBManager.instance.cache_ItemDataList[resultItemID].name.ToString());
                            yield return waitTalking;

                            // objects[0].gameObject.SetActive(false);//완성본 비활성화
                            // objects[1].gameObject.SetActive(true);//부서진거 활성화
                            // SoundManager.instance.PlaySound("table_broken");


                            // SetDialogue(dialogues[5]);
                            // yield return waitTalking;

                            InventoryManager.instance.RemoveItem(materialItemID0, materialItemAmount);
                            InventoryManager.instance.RemoveItem(materialItemID1, materialItemAmount);
                            InventoryManager.instance.AddItem(resultItemID);
                            //location.selectPhase = -1;
                        }
                        //다음에 만들자
                        else if(GetSelect()==1){
                            SetDialogue(dialogues[6]);
                            yield return waitTalking;
                        }

                    }
                }
                break;
#endregion 

#if demo

#region @45 광장 출입 불가(데모)
            case 45 :
                MenuManager.instance.waitPopUpClosed = true;
                MenuManager.instance.popUpOnWork.SetActive(true);
                yield return new WaitUntil(()=>!MenuManager.instance.waitPopUpClosed );

                break;
#endregion 

#endif

#region @46 노개미방 출구 체크
            case 46 :

                objects[0].gameObject.SetActive(false);

                break;
#endregion 

#region @47 노개미방 총
            case 47 :

                if(objects[0].gameObject.activeSelf){
                    //dialogues[0].talker = objects[0];
                    objects[0].GetComponent<NPCScript>().StopMove();
                    objects[0].GetComponent<NPCScript>().SetNpcAnimatorBool("mad",true);
                    objects[0].GetComponent<NPCScript>().NpcLookObject(PlayerManager.instance.transform);
                    //CameraView(dialogues[0].talker);
                    SetDialogue(dialogues[0]);
                    yield return waitTalking;
                    objects[0].GetComponent<NPCScript>().SetNpcAnimatorBool("mad",false);
                }
                // else if(objects[1].gameObject.activeSelf){
                //     dialogues[0].talker = objects[1];                    
                //     objects[1].GetComponent<NPCScript>().StopMove();

                //     //CameraView(dialogues[0].talker);
                //     SetDialogue(dialogues[0]);
                //     yield return waitTalking;
                // }
                else{

                    // if(location.selectPhase == 2){

                    //     SetDialogue(dialogues[6]);
                    //     yield return waitTalking;

                        
                    //     location.selectPhase = -1;
                    // }
                    // else{


                        if(location.selectPhase == 0){
                            location.selectPhase = 1;
                            SetDialogue(dialogues[1]);
                            yield return waitTalking;
                        }
                    //else if(location.selectPhase == 0){

                        SetDialogue(dialogues[2]);
                        yield return waitTalking;

                        if(objects[1].gameObject.activeSelf && objects[2].gameObject.activeSelf){
                            SetSelect(selects[0]);
                            yield return waitSelecting;
                        }

                        if(GetSelect()==0){
                            
                            SetDialogue(dialogues[3]);
                            yield return waitTalking;
                        }
                        else if(GetSelect()==1){
                            if(objects[1].gameObject.activeSelf){

                                SetDialogue(dialogues[4]);
                                yield return waitTalking;
                                
                                objects[1].gameObject.SetActive(false);
                                InventoryManager.instance.AddItem(27);

                                
                                location.selectPhase = -1;
                            }
                        }
                        else if(GetSelect()==2){
                            
                            SetDialogue(dialogues[5]);
                            yield return waitTalking;
                            
                            objects[2].gameObject.SetActive(false);
                            InventoryManager.instance.AddItem(28);

                            location.selectPhase = -1;
                        }

                    //}

                    // if(!objects[1].gameObject.activeSelf && !objects[2].gameObject.activeSelf){
                        
                    //     location.preserveTrigger = false;
                    // }
                    //}
                    // else{
                        
                    //     SetDialogue(dialogues[6]);
                    //     yield return waitTalking;
                    // }

                }


                break;
#endregion 

#region @48 미니게임0 최초 실행 후 유치원 나감 체크
            case 48 :
                break;
#endregion 

#region @49 [미니게임0 - 종이 오리기] 재시작
            case 49 :

                AutoSave();

                int curSelectNumber;

                if(!InventoryManager.instance.CheckHaveItem(6)){
                    curSelectNumber = 0;
                    SetSelect(selects[0]);
                    yield return waitSelecting;
                }
                else{
                    curSelectNumber = 1;
                    SetSelect(selects[1]);
                    yield return waitSelecting;
                }



                if((curSelectNumber == 0 && GetSelect()==0)||(curSelectNumber == 1 && GetSelect()==1)){

                    SetDialogue(dialogues[0]);
                    yield return waitTalking;

                    MinigameManager.instance.StartMinigame(0);
                    yield return new WaitUntil(()=>MinigameManager.instance.success || MinigameManager.instance.fail);

                    if(MinigameManager.instance.success){
                        dialogues[0].talker.GetComponent<NPCScript>().animator.SetBool("clap", true);

                    yield return wait2000ms;
                        SetDialogue(dialogues[1]);
                        yield return waitTalking;
                        dialogues[0].talker.GetComponent<NPCScript>().animator.SetBool("clap", false);

                        
                        InventoryManager.instance.AddHoney(100,true);
                    }       
                    else{

                        SetDialogue(dialogues[2]);
                        yield return waitTalking;
                        SetDialogue(dialogues[3]);
                        yield return waitTalking;


                        MinigameManager.instance.FailMinigame(3);
                    }
                }
                else if((curSelectNumber == 1 && GetSelect()==0)){
                    //Action(objects[0].GetComponent<Location>());
                    //yield return new WaitUntil(()=>!PlayerManager.instance.isActing);
                    InventoryManager.instance.RemoveItem(6);
                    SetDialogue(dialogues[4]);
                    yield return waitTalking;
                    SetDialogue(dialogues[5]);
                    yield return waitTalking;
                    SetDialogue(dialogues[6]);
                    yield return waitTalking;

                    DBManager.instance.TrigOver(9);
#if !demo
                    objects[1].gameObject.SetActive(true);
#endif
                }
                


                break;
#endregion

#region @50 노개미 재방문
            case 50 :
                location.selectPhase = -1;
                //미친수개미 > 노개미 선택
                if(DBManager.instance.CheckTrigOver(17)){

                    for(int i=0 ;i<3;i++){
                        
                        //CameraView(dialogues[i].talker);
                        SetDialogue(dialogues[i]);
                        yield return waitTalking;
                    }
                }

                //if(location.selectPhase == 0){
                //}
            
                SetDialogue(dialogues[3]);
                yield return waitTalking;


                if(InventoryManager.instance.CheckHaveItem(20)){
                    
                    SetSelect(selects[0]);
                    yield return waitSelecting;

                    //쪽지를 줌
                    if(GetSelect()==0){
                        
                        //location.selectPhase = -1;
                        for(int i=4 ;i<9;i++){
                            if(i==6) yield return wait1000ms;
                            

                            SetDialogue(dialogues[i]);
                            yield return waitTalking;
                        }

                        dialogues[8].talker.GetComponent<NPCScript>().wSet = -1;
                        yield return wait2000ms;
                        dialogues[8].talker.GetComponent<NPCScript>().wSet = 0;
                        dialogues[8].talker.GetComponent<NPCScript>().NpcLookObject(PlayerManager.instance.transform);
                        PlayerLookObject(dialogues[8].talker);
                        ////CameraView(dialogues[9].talker);
                        SetDialogue(dialogues[9]);
                        yield return waitTalking;
                        ////CameraView(dialogues[10].talker);
                        SetDialogue(dialogues[10]);
                        yield return waitTalking;
                        dialogues[8].talker.GetComponent<NPCScript>().wSet = -1;
                        dialogues[8].talker.GetComponent<NPCScript>().Look("left");
                        FadeOut();
                        yield return wait2000ms;
                        objects[0].gameObject.SetActive(false);
                        FadeIn();
                        yield return wait1000ms;
                        ////CameraView(dialogues[11].talker);
                        SetDialogue(dialogues[11]);
                        yield return waitTalking;
                        ShakeCamera();
                        ////CameraView(dialogues[12].talker);
                        SetDialogue(dialogues[12]);
                        yield return waitTalking;

                        InventoryManager.instance.RemoveItem(20,1);
                        DBManager.instance.TrigOver(51);

                        //location.selectPhase = -1;
                    }
                    else if(GetSelect()==1){

                        //location.selectPhase = -1;
                        for(int i=13 ;i<21;i++){
                            SetDialogue(dialogues[i]);
                            yield return waitTalking;
                        }
                    }
                    else{
                    
                        SetDialogue(dialogues[24]);
                        yield return waitTalking;
                    }
                }
                else{
                    SetSelect(selects[1]);
                    yield return waitSelecting;
                    
                    if(GetSelect()==0){

                        //location.selectPhase = -1;
                        for(int i=13 ;i<21;i++){
                            SetDialogue(dialogues[i]);
                            yield return waitTalking;
                        }
                    }
                    else{
                    
                        SetDialogue(dialogues[24]);
                        yield return waitTalking;
                    }
                }
                

                break;
#endregion

#region @51 노개미 재방문2(반복용)
            case 51 :

                    if(InventoryManager.instance.CheckHaveItem(20)){

                        for(int i=22 ;i<24;i++){
                            SetDialogue(dialogues[i]);
                            yield return waitTalking;
                        }
                        
                        for(int i=4 ;i<9;i++){
                            if(i==6) yield return wait1000ms;
                            SetDialogue(dialogues[i]);
                            yield return waitTalking;
                        }

                        dialogues[8].talker.GetComponent<NPCScript>().wSet = -1;
                        yield return wait2000ms;
                        dialogues[8].talker.GetComponent<NPCScript>().wSet = 0;
                        dialogues[8].talker.GetComponent<NPCScript>().NpcLookObject(PlayerManager.instance.transform);
                        PlayerLookObject(dialogues[8].talker);
                        //CameraView(dialogues[9].talker);
                        SetDialogue(dialogues[9]);
                        yield return waitTalking;
                        //CameraView(dialogues[10].talker);
                        SetDialogue(dialogues[10]);
                        yield return waitTalking;
                        dialogues[8].talker.GetComponent<NPCScript>().wSet = -1;
                        dialogues[8].talker.GetComponent<NPCScript>().Look("left");
                        FadeOut();
                        yield return wait2000ms;
                        objects[0].gameObject.SetActive(false);
                        FadeIn();
                        yield return wait1000ms;
                        //CameraView(dialogues[11].talker);
                        SetDialogue(dialogues[11]);
                        yield return waitTalking;
                        ShakeCamera();
                        //CameraView(dialogues[12].talker);
                        SetDialogue(dialogues[12]);
                        yield return waitTalking;

                        InventoryManager.instance.RemoveItem(20,1);
                        location.selectPhase = -1;
                        
                    }
                    else{
                        dialogues[0].talker.GetComponent<NPCScript>().animator.SetBool("mad", true);
                        SetDialogue(dialogues[21]);
                        yield return waitTalking;
                        dialogues[0].talker.GetComponent<NPCScript>().animator.SetBool("mad", false);
                    }
                

                


                break;
#endregion

#region @53 풀사다리 습득
            case 53 :

                objects[0].gameObject.SetActive(false);
                objects[1].gameObject.SetActive(true);
                InventoryManager.instance.AddItem(36,activateDialogue:true);


                break;
#endregion

#region @54 풀사다리 습득
            case 54 :
                
                objects[0].gameObject.SetActive(true);
                objects[1].gameObject.SetActive(false);
                InventoryManager.instance.RemoveItem(36);
                PlaySound("rope1s");
                yield return wait1000ms;


                break;
#endregion

#region @55 알번데기방 상자
            case 55 :
                UIManager.instance.OpenScreen(0);
                yield return new WaitUntil(()=>!UIManager.instance.screenOn);
                
                if(InventoryManager.instance.CheckHaveItem(38)){
                    location.preserveTrigger= false;

                    objects[0].gameObject.SetActive(false);
                    objects[1].gameObject.SetActive(true);
                    
                }
                else{

                }
                
                break;
#endregion

#region @56 세갈래길 자동저장
            case 56 :
#if !alpha
                //튜토리얼 패스 확인
                // SteamUserStats.GetStat("pt",out int pt);
                // SteamUserStats.SetStat("pt",pt + 1);
                // SteamUserStats.StoreStats();
                SteamAchievement.instance.GetAndSetSteamUserStat("pt");
#endif
                AutoSave();
                break;
#endregion

#region @57 경비개미방 입장직전 자동저장
            case 57 :
                AutoSave();
                break;
#endregion

#region @58,t59 진딧물게임 패배 후
//             case 58 :
//             case 59 :
//                 if(location.selectPhase==0){
//                     location.selectPhase = 1;

//                     SetDialogue(dialogues[0]);
//                     yield return waitTalking;
//                 }
//                 else{

//                     SetDialogue(dialogues[1]);
//                     yield return waitTalking;
//                 }
//                 break;
// #endregion

// #region @23 진딧물농장 미니게임4
            case 58 :

                SetSelect(selects[0]);
                yield return new WaitUntil(()=>!PlayerManager.instance.isSelecting);
                if(GetSelect()==2){
                    break;
                }
                location.selectPhase = -1;


                SceneController.instance.SetCameraDefaultZoomOut();

                MinigameManager.instance.StartMinigame(4);

                yield return new WaitUntil(()=>MinigameManager.instance.success || MinigameManager.instance.fail);
                        

                objects[17].GetComponent<Location>().isLocked = false;

                PlayerManager.instance.vignette.SetActive(true);
                PlayerManager.instance.redVignette.SetActive(true);
                
                //게임 성공 시
                if(MinigameManager.instance.success){
                    location.preserveTrigger = false;
DBManager.instance.AntCollectionOver(12);

                PlayerManager.instance.transform.position = objects[6].position;
                PlayerManager.instance.Look("right");
                    for(int i=8;i<17;i++){
                        objects[i].gameObject.SetActive(false);
                    }
                    yield return wait1000ms;

                    CameraView(dialogues[9].talker);

                    objects[7].gameObject.SetActive(true);
                    SceneController.instance.SetCameraDefaultZoomIn();

                    yield return wait1000ms;
                    yield return wait1000ms;
                    if(GetSelect()==0){
                        CameraView(dialogues[9].talker);
                        
                        SetDialogue(dialogues[9],onCameraCenter:true);
                        SetHUD(false);
                        yield return waitTalking;
                    }
                    else if(GetSelect()==1){
                        SetHUD(false);
                        CameraView(dialogues[10].talker);
                        SetDialogue(dialogues[10],onCameraCenter:true);
                        yield return waitTalking;
                    }

                    FadeOut();

                    if(GetSelect()==0){
                        InventoryManager.instance.AddItem(17);
                    }
                    else if(GetSelect()==1){
                        InventoryManager.instance.AddItem(0);
                    }
                    yield return wait2000ms;
                    objects[7].gameObject.SetActive(false);

                    FadeIn();

                    //지키기 : 여왕지지자
                    if(GetSelect()==0){
                        SteamAchievement.instance.ApplyAchievements(9);

                    }
                    //훔치는거 도와주기 : 공주지지자
                    else if(GetSelect()==1){
                        SteamAchievement.instance.ApplyAchievements(8);

                    }

                }
                else{
                    yield return wait3000ms;

                }

                DBManager.instance.TrigOver(74);

                break;
#endregion

#region @61 버섯농장앞 표지판
            case 61 :
        
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                break;
#endregion

#region @62 버섯농장앞 수레개미
            case 62 :
            
                for(int i=0;i<dialogues.Length;i++){
                    PlayerLookObject(dialogues[i].talker);
                    SetDialogue(dialogues[i]);
                    yield return waitTalking;
                }

                FadeOut();
                yield return wait2000ms;
                objects[1].gameObject.SetActive(false);
                FadeIn();



                objects[0].GetComponent<Location>().isLocked = false;
                break;
#endregion

#region @63 버섯농장앞 표지판
            case 63 :
                DBManager.instance.AntCollectionOver(3);
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                break;
#endregion
#region @69 냉동굴 게이지 ON
            case 69 :
            
                if(!UIManager.instance.iceGaugeFlag){
                    UIManager.instance.iceGaugeFlag = true;
                    iceGaugeCoroutine = StartCoroutine(UIManager.instance.FillIceGaugeCoroutine());
                }
                break;
#endregion

#region @70 냉동굴 게이지 OFF
            case 70 :
            
                if(iceGaugeCoroutine!=null && UIManager.instance.iceGaugeFlag){
                    UIManager.instance.iceGaugeFlag = false;
                    StopCoroutine(iceGaugeCoroutine);
                    UIManager.instance.ResetIceGauge();
                }
                break;
#endregion

#region @71 거미상점 흙충전 무제한
            case 71 :

                SetDialogue(dialogues[0]);
                yield return waitTalking;
                
                SetSelect(selects[0]);
                yield return waitSelecting;
                if(GetSelect()==0){
                    var curHoneyAmount = DBManager.instance.curData.curHoneyAmount;
                    if(curHoneyAmount>=10){
                        DBManager.instance.curData.curHoneyAmount -= 10;
                        InventoryManager.instance.AddDirt(20);
                        SoundManager.instance.PlaySound("dirt_charge");
                    }
                    else{
                        
                        SetDialogue(dialogues[1]);
                        yield return waitTalking;
                    }
                }
                break;
#endregion 

#region @72 흙파기(키헬퍼ON)
            case 72 :
               
                UIManager.instance.OpenScreen(1);
                yield return new WaitUntil(()=>!UIManager.instance.screenOn);
                break;
#endregion
#region @73 미친수개미 종료후 선택
            case 73 :
               
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                SetSelect(selects[0]);
                yield return waitSelecting;

                FadeOut();
                yield return wait1000ms;
                FadeIn();



                if(GetSelect()==0){
                    PlayerManager.instance.transform.position = objects[0].transform.position;
                }
                else{
                    PlayerManager.instance.transform.position = objects[1].transform.position;

                }
                break;
#endregion


#region @76 광장 갑옷 앞 글 읽기
            case 76 :

                SetDialogue(dialogues[0]);
                yield return waitTalking;

                objects[0].gameObject.SetActive(true);
                objects[1].gameObject.SetActive(false);

                InventoryManager.instance.AddItem(24,activateDialogue:true);

                break;
#endregion

#region @80 유모 개미 유도대사
            case 80 :
                PlayerManager.instance.Look("right");
                dialogues[0].talker.GetComponent<NPCScript>().mainBody.localScale *= new Vector2(-1,1);
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                dialogues[0].talker.GetComponent<NPCScript>().wSet = -1;
                dialogues[0].talker.GetComponent<NPCScript>().mainBody.GetComponent<Animator>().SetBool("run", true);

                yield return new WaitUntil(()=>dialogues[0].talker.localPosition.x < objects[0].localPosition.x);
                PlaySound("open door");
                dialogues[0].talker.gameObject.SetActive(false);

            SceneController.instance.virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 0;
            SceneController.instance.virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 0;
            
                yield return wait500ms;

                SetDialogue(dialogues[1]);
                yield return waitTalking;


                break;
#endregion

#region @81 대왕일개미 re
            case 81 :

                    // SetDialogue(dialogues[0]);
                    // yield return waitTalking;
                    // SceneController.instance.virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 0;
                    // SceneController.instance.virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 0;
            
                    // SetSelect(selects[0]);
                    // yield return waitSelecting;
                    
                    // if(GetSelect()==0){
                    //     for(int i=1;i<6;i++){
                    //         SetDialogue(dialogues[i]);
                    //         yield return waitTalking;
                    //     }
                        if(InventoryManager.instance.CheckHaveItem(3)
                        ||InventoryManager.instance.CheckHaveItem(16)
                        ||InventoryManager.instance.CheckHaveItem(19)
                        ){
                                    
                            SetDialogue(dialogues[6]);
                            yield return waitTalking;
                            SetDialogue(dialogues[7]);
                            yield return waitTalking;
                            // SetSelect(selects[1]);
                            // yield return waitSelecting;
                            // if(GetSelect()==0){
                            //     for(int i=8;i<14;i++){
                            //         SetDialogue(dialogues[i]);
                            //         yield return waitTalking;
                            //     }
                            //     location.selectPhase = -1;
                            // }
                            // else{
                            //     SetDialogue(dialogues[15]);
                            //     yield return waitTalking;
                            //     FadeOut();
                            //     yield return wait1000ms;
                            //     SceneController.instance.SetConfiner(20);
                            //     SoundManager.instance.SetBgmByMapNum(20);
                            //     PlayerManager.instance.transform.position = objects[0].transform.position;
                            //     FadeIn();
                                
                            //     SetDialogue(dialogues[16]);
                            //     yield return waitTalking;

                            //     //방나가짐
                            // }

                            //보유한 과일에 따른 선택지 노출 220729
                            int[] tempIntArr26 = new int[3]{3,16,19};

                            List<string> tempStringList26 = new List<string>(){};//선택지 배열 전환용
                            List<string> tempStringList26_1 = new List<string>();//아이템 ID 스트링 배열 전환용
                            List<int> tempIntList = new List<int>();//아이템 ID
                            for(int i=0;i<3;i++){
                                if(InventoryManager.instance.CheckHaveItem(tempIntArr26[i])){
                                    tempStringList26_1.Add(DBManager.instance.cache_ItemDataList[tempIntArr26[i]].name.ToString());
                                    tempIntList.Add(tempIntArr26[i]);
                                    tempStringList26.Add("17");
                                }
                            }
                            tempStringList26_1.Add("44");//배열 길이 초과 떄문에 넣음(의미X)
                            tempStringList26.Add("44");

                            string [] tempArg26 = tempStringList26_1.ToArray();

                            Select tempSelect26 = new Select();
                            tempSelect26.answers = tempStringList26.ToArray();

                            SetSelect(tempSelect26, tempArg26);
                            yield return waitSelecting;     
    

                            // SetSelect(selects[1]);
                            // yield return waitSelecting;
                            if(GetSelect()==tempSelect26.answers.Length-1){//주지 않는다.
                                
                    CameraView(dialogues[15].talker);
                                SetDialogue(dialogues[15]);
                                yield return waitTalking;
                                FadeOut();
                                yield return wait1000ms;
                                SceneController.instance.SetConfiner(20);
                                PlayerManager.instance.transform.position = objects[0].transform.position;
                                FadeIn();
                                
                    CameraView(dialogues[16].talker);
                                SetDialogue(dialogues[16]);
                                yield return waitTalking;

                                //방나가짐
                            }
                            else{//과일을 준다.
                                //선택한 과일에 맞는 아이템 ID 삭제
                                InventoryManager.instance.RemoveItem(tempIntList[GetSelect()]);

                                for(int i=8;i<14;i++){
                                    if(i==11){
                                objects[3].GetComponent<Animator>().SetTrigger("eat");
                                        
                                    }
                    CameraView(dialogues[i].talker);
                                    SetDialogue(dialogues[i]);
                                    yield return waitTalking;
                                }
                                location.selectPhase = -1;
                            }
                        }
                        else{
                            
                                SetDialogue(dialogues[15]);
                                yield return waitTalking;
                                FadeOut();
                                yield return wait1000ms;
                                SceneController.instance.SetConfiner(20);
                                SoundManager.instance.SetBgmByMapNum(20);
                                PlayerManager.instance.transform.position = objects[0].transform.position;
                                FadeIn();
                                
                                SetDialogue(dialogues[16]);
                                yield return waitTalking;
                        }
                        // else{
                        //     //방나가짐
                        //         FadeOut();
                        //         yield return wait1000ms;
                        //         SceneController.instance.SetConfiner(20);
                        //         PlayerManager.instance.transform.position = objects[0].transform.position;
                        //         FadeIn();
                        // }
                    // }
                    // else{
                    //     SetDialogue(dialogues[14]);
                    //     yield return waitTalking;

                    //     //죽음
                    //     UIManager.instance.SetGameOverUI(4);
                    // }

                break;
#endregion

#region @82 대왕일개미 막기
            case 82 :

                SetDialogue(dialogues[15]);
                yield return waitTalking;
                FadeOut();
                yield return wait1000ms;
                SceneController.instance.SetConfiner(20);
                SoundManager.instance.SetBgmByMapNum(20);
                PlayerManager.instance.transform.position = objects[0].transform.position;
                FadeIn();
                
                SetDialogue(dialogues[16]);
                yield return waitTalking;


                break;
#endregion

#region @83 대왕일개미 막기
            case 83 :

                int ranNum83 = Random.Range(0,2) * 2;

                SetDialogue(dialogues[ranNum83]);
                yield return waitTalking;
                SetDialogue(dialogues[ranNum83 + 1]);
                yield return waitTalking;


                break;
#endregion

#region @84 유치원 형제개미 발견
            case 84 :

                DBManager.instance.AntCollectionOver(7);
                break;
#endregion

#region @85 수개미방 헬창수개미 발견
            case 85 :

                DBManager.instance.AntCollectionOver(10);
                break;
#endregion
#region @86 유모개미 발견
            case 86 :

                DBManager.instance.AntCollectionOver(13);
                break;
#endregion
#region @n88 반딧불이 발견
            case 88 :
                PlayerManager.instance.LockPlayer();
                PlayerManager.instance.Look("left");
                PlayerManager.instance.SetTalkCanvasDirection();
                
                for(int i=0;i<dialogues.Length;i++){
                //PlayerManager.instance.LockPlayer();
                    CameraView(PlayerManager.instance.transform);
                    SetDialogue(dialogues[i],onCameraCenter:false);
                    yield return waitTalking;
                }
                PlayerManager.instance.UnlockPlayer();

                break;
#endregion
#region @n89 공주개미 엿듣기
            case 89 :
                SetSelect(selects[0]);
                yield return waitSelecting;     

                if(GetSelect()==0){

                    for(int i=0;i<dialogues.Length;i++){
                        SetDialogue(dialogues[i]);
                        yield return waitTalking;
                    }
                    location.selectPhase = -1;
                }
                
                break;
#endregion
#region @90 공주개미 재배치
            case 90 :
                objects[0].transform.position = objects[1].transform.position;
                objects[0].transform.GetComponent<NPCScript>().animator.SetBool("think", false);
            
                //PlayerManager.instance.UnlockPlayer();

                break;
#endregion
#region @91 광장 노개미 (수레개미 선택 후)
            case 91 :
                
                for(int i=25 ;i<29;i++){
                    
                    //CameraView(dialogues[i].talker);
                    SetDialogue(dialogues[i]);
                    yield return waitTalking;
                }
                break;
#endregion
#region @93 제작대 레시피
            case 93 :
                
                UIManager.instance.OpenScreen(3);
                yield return new WaitUntil(()=>!UIManager.instance.screenOn);
                break;
#endregion


#region @899 자동저장
            case 899 :
                AutoSave();
                break;
#endregion

#region @201 [엔딩1 : 여왕의 방 - 전설의 젤리(젤할라)]
            case 201 :
#if !alpha
            //메인엔딩 달성 
            // SteamUserStats.GetStat("fe",out int fe);
            // SteamUserStats.SetStat("fe",fe + 1);
            // SteamUserStats.StoreStats();
            SteamAchievement.instance.GetAndSetSteamUserStat("fe");
#endif
                UIManager.instance.SetMovieEffectUI(true);
DBManager.instance.AntCollectionOver(18);

                FadeOut();
                yield return wait2000ms;
                PlayerManager.instance.Look("right");
                PlayerManager.instance.SetTalkCanvasDirection();
                yield return wait1000ms;
                FadeIn();

                //노개미 등장
                objects[1].gameObject.SetActive(true);
                //CameraView(dialogues[0].talker);
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                //CameraView(dialogues[1].talker);
                SetDialogue(dialogues[1]);
                yield return waitTalking;
        
                FadeOut();
                yield return wait1000ms;
                yield return wait500ms;
                objects[0].gameObject.SetActive(true);
                FadeIn();
                //퀸 등장
                SetDialogue(dialogues[2]);
                yield return waitTalking;
                FadeOut();
                yield return wait1000ms;
                PlaySound("boosruck");            
                objects[2].gameObject.SetActive(true);
                objects[3].gameObject.SetActive(true);
                objects[0].gameObject.SetActive(false);    
                yield return wait1000ms;
                FadeIn();
                ShakeCamera();
                yield return wait2000ms;


#if !UNITY_EDITOR
                for(int k=3;k<14;k++){
                    //CameraView(dialogues[k].talker);
                    SetDialogue(dialogues[k]);
                    yield return waitTalking;
                }

                ShakeCamera(intensity:2);
                PlaySound("knock_hard_01");
                CameraView(dialogues[14].talker);
                PlayerManager.instance.Look("left");
                yield return wait2000ms;
                //CameraView(dialogues[14].talker);
                SetDialogue(dialogues[14]);
                yield return waitTalking;
                PlayerManager.instance.Look("right");

                for(int k=15;k<dialogues.Length;k++){
                    //CameraView(dialogues[k].talker);
                    ShakeCamera(intensity:2);
                    PlaySound("knock_hard_01");
                    SetDialogue(dialogues[k]);
                    yield return waitTalking;
                }
#endif
                
                CameraView(objects[4]);
                PlayerManager.instance.GetComponent<PlayerManager>().wSet = 1;
                yield return wait2000ms;
                //yield return wait500ms;
                objects[1].GetComponent<NPCScript>().wSet = 1;
                objects[2].GetComponent<NPCScript>().wSet = 1;
                yield return wait2000ms;
                UIManager.instance.SetGameEndUI(1);

                //yield return wait1000ms;
                yield return wait1000ms;
                objects[1].GetComponent<NPCScript>().wSet = 0;
                objects[2].GetComponent<NPCScript>().wSet = 0;
                PlayerManager.instance.GetComponent<PlayerManager>().wSet = 0;
                break;
#endregion 

#region @202 [엔딩2 : 여왕의 방 - 사랑의 도피]
            case 202 :
                UIManager.instance.SetMovieEffectUI(true);

                NPCScript oldAnt202 = objects[1].GetComponent<NPCScript>();
                NPCScript queenJelly202 = objects[2].GetComponent<NPCScript>();


DBManager.instance.AntCollectionOver(18);
                FadeOut();
                yield return wait1000ms;
                PlayerManager.instance.Look("right");
                PlayerManager.instance.SetTalkCanvasDirection();
                yield return wait1000ms;
                FadeIn();
        
                //노개미 등장
                objects[1].gameObject.SetActive(true);
                //CameraView(dialogues[0].talker);
                        //oldAnt202.Look("right");
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                //CameraView(dialogues[1].talker);
                SetDialogue(dialogues[1]);
                yield return waitTalking;
        
                FadeOut();
                yield return wait1000ms;
                yield return wait500ms;
                objects[0].gameObject.SetActive(true);
                FadeIn();
                //퀸 등장
                SetDialogue(dialogues[2]);
                yield return waitTalking;
                FadeOut();
                yield return wait1000ms;
                PlaySound("boosruck");            
                objects[2].gameObject.SetActive(true);
                objects[3].gameObject.SetActive(true);
                objects[0].gameObject.SetActive(false);    
                yield return wait1000ms;
                FadeIn();
                ShakeCamera();
                yield return wait2000ms;
                for(int k=3;k<11;k++){
                    //CameraView(dialogues[k].talker);
                    SetDialogue(dialogues[k]);
                    yield return waitTalking;
                }

                ShakeCamera(intensity:2);
                PlaySound("knock_hard_01");
                CameraView(dialogues[11].talker);
                PlayerManager.instance.Look("left");
                yield return wait2000ms;
                SetDialogue(dialogues[11]);
                yield return waitTalking;
                PlayerManager.instance.Look("right");

                for(int k=12;k<15;k++){
                    //CameraView(dialogues[k].talker);
                    ShakeCamera(intensity:2);
                    PlaySound("knock_hard_01");
                    SetDialogue(dialogues[k]);
                    yield return waitTalking;
                }

                //220902 추가대사
                for(int k=30;k<=40;k++){
                    //CameraView(dialogues[k].talker);
                    if(k==32){
                        oldAnt202.Look("right");
                    }
                    if(k==36){
                        oldAnt202.Look("left");
                        PlayerManager.instance.animator.SetBool("bottle",true);
                        yield return wait1000ms;
                    }

                    SetDialogue(dialogues[k]);
                    yield return waitTalking; 
                }

                for(int k=16;k<=17;k++){
                    SetDialogue(dialogues[k]);
                    yield return waitTalking;
                }
                UIManager.instance.SetGameEndUI(2);
                yield return wait1000ms;
                break;
#endregion 
//[엔딩3 : 개미굴에서 젤리난다.]
#region @n203
            case 203 :
                UIManager.instance.SetMovieEffectUI(true);
DBManager.instance.AntCollectionOver(17);
                FadeOut();
                yield return wait1000ms;
                TeleportPlayer(objects[1]);
                PlayerManager.instance.Look("right");
                PlayerManager.instance.SetTalkCanvasDirection();
                yield return wait1000ms;
                FadeIn();
        
                for(int k=0;k<dialogues.Length;k++){
                    //CameraView(dialogues[k].talker);
                    SetDialogue(dialogues[k]);
                    yield return waitTalking;
                }

                UIManager.instance.SetGameEndUI(3);
                yield return wait1000ms;
                break;
#endregion 

#region @n204 [엔딩4 : 살육의밤]
            case 204 :
                UIManager.instance.SetMovieEffectUI(true);
DBManager.instance.AntCollectionOver(17);
                FadeOut();
                yield return wait1000ms;
                TeleportPlayer(objects[1]);
                PlayerManager.instance.Look("right");
                PlayerManager.instance.SetTalkCanvasDirection();
                dialogues[0].talker.GetComponent<NPCScript>().animator.SetBool("mad", true);
                yield return wait1000ms;
                FadeIn();
        
                for(int k=0;k<dialogues.Length;k++){
                    //CameraView(dialogues[k].talker);
                    SetDialogue(dialogues[k]);
                    yield return waitTalking;
                }

                FadeOut();
                yield return wait1000ms;
                PlaySound("lucky_head_down");
                PlayerManager.instance.animator.SetBool("dead0", true);
                yield return wait1000ms;
                FadeIn();
                ShakeCamera(2,2);
                SoundManager.instance.BgmOff();
                yield return wait2000ms;


                UIManager.instance.SetGameEndUI(4);
                yield return wait1000ms;
                break;
#endregion 
//
#region @n205 [엔딩5 : 꼭두각시]
            case 205 :
                UIManager.instance.SetMovieEffectUI(true);
DBManager.instance.AntCollectionOver(17);
                FadeOut();
                yield return wait1000ms;
                TeleportPlayer(objects[3]);
                PlayerManager.instance.Look("right");
                PlayerManager.instance.SetTalkCanvasDirection();
                yield return wait1000ms;
                FadeIn();
        
                //CameraView(dialogues[0].talker);
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                //CameraView(dialogues[1].talker);
                SetDialogue(dialogues[1]);
                yield return waitTalking;

                SetSelect(selects[0]);
                yield return waitSelecting;
                
                if(GetSelect()==1){

                    for(int k=2;k<dialogues.Length;k++){
                        //CameraView(dialogues[k].talker);
                        SetDialogue(dialogues[k]);
                        yield return waitTalking;
                    }

                    
                    SetSelect(selects[1]);
                    yield return waitSelecting;

                    location.selectPhase = -1;
                    UIManager.instance.SetGameEndUI(5);
                    yield return wait1000ms;
                }
                else{
                    FadeOut();
                    yield return wait1000ms;
                    objects[2].GetComponent<Location>().LocationScript(PlayerManager.instance.bodyCollider2D);
                    //PlayerManager.instance.transform.position = objects[1].transform.position;
                    yield return wait100ms;
                    FadeIn();

                    UIManager.instance.SetMovieEffectUI(false);

                }

                break;
#endregion 


#region @206 [엔딩6-친구와 함께라면]
            case 206 :
                UIManager.instance.SetMovieEffectUI(true);
        
                FadeOut();
                yield return wait1000ms;
                PlayerManager.instance.Look("left");
                PlayerManager.instance.SetTalkCanvasDirection();
                objects[0].gameObject.SetActive(true);
                //CameraView(objects[3]);
                yield return wait1000ms;
                FadeIn();

                for(int k=0;k<3;k++){
                    ////CameraView(dialogues[k].talker);
                    SetDialogue(dialogues[k]);
                    yield return waitTalking;
                }

                SetSelect(selects[0]);
                yield return waitSelecting;

                if(GetSelect()==0){
                    location.selectPhase = -1;
                    InventoryManager.instance.RemoveItem(0);
                    InventoryManager.instance.RemoveItem(14);

                    //뿌얘지는 효과
                    FadeOut();
                    yield return wait1000ms;
                    PlaySound("match");
                    objects[1].gameObject.SetActive(true);
                    yield return wait1000ms;
                    FadeIn();
                    yield return wait1000ms;
                    for(int k=4;k<6;k++){
                        ////CameraView(dialogues[k].talker);
                        SetDialogue(dialogues[k]);
                        yield return waitTalking;
                    }

                    //럭키 문 나가기 & 노란젤리 문으로 걸어가기

                    UIManager.instance.SetGameEndUI(6);
                    yield return wait1000ms;

                }
                UIManager.instance.SetMovieEffectUI(false);

                break;
#endregion 

#region @207 [엔딩7 - 여행가]
            case 207 :
        
                UIManager.instance.SetMovieEffectUI(true);
                FadeOut();
                yield return wait1000ms;
                PlayerManager.instance.Look("left");
                PlayerManager.instance.SetTalkCanvasDirection();
                yield return wait1000ms;
                FadeIn();

                SetDialogue(dialogues[0]);
                yield return waitTalking;

                SetSelect(selects[0]);
                yield return waitSelecting;

                if(GetSelect()==0){
                    location.selectPhase = -1;
                    InventoryManager.instance.RemoveItem(0);
                    InventoryManager.instance.RemoveItem(14);

                    //뿌얘지는 효과
                    FadeOut();
                    yield return wait1000ms;
                    objects[0].gameObject.SetActive(true);
                    PlaySound("match");
                    yield return wait1000ms;
                    FadeIn();
                    yield return wait1000ms;

                    SetDialogue(dialogues[1]);
                    yield return waitTalking;
                    

                    //럭키 문 나가기 & 노란젤리 문으로 걸어가기

                    UIManager.instance.SetGameEndUI(7);
                    yield return wait1000ms;

                }
                UIManager.instance.SetMovieEffectUI(false);

                break;
#endregion 

#region @208 [엔딩8 - 산제물]
            case 208 :
                UIManager.instance.SetMovieEffectUI(true);
                PlayerManager.instance.Look("right");
                PlayerManager.instance.SetTalkCanvasDirection();

                if(location.selectPhase==0){
                    location.selectPhase = 1;
                    SetDialogue(dialogues[0]);
                    yield return waitTalking;
                }
                else if(location.selectPhase==1){
                    SetDialogue(dialogues[4]);
                    yield return waitTalking;
                }
                if(DBManager.instance.curData.curHoneyAmount >= 1000){

                    SetDialogue(dialogues[1]);
                    yield return waitTalking;
                    SetSelect(selects[0]);
                    yield return waitSelecting;
                    if(GetSelect()==0){
                        location.selectPhase = -1;  
                        DBManager.instance.curData.curHoneyAmount -= 1000;
                            
                        for(int k=2;k<4;k++){
                            SetDialogue(dialogues[k]); 
                            yield return waitTalking;
                            
                        }
                        
                        FadeOut();
                        yield return wait1000ms;
                        objects[0].gameObject.SetActive(false);
                        yield return wait1000ms;
                        FadeIn();
                        //오른쪽으로 이동
                        SceneController.instance.SetConfiner(16);
                        //PlayerManager.instance.speed *= 0.7f;
                        PlayerManager.instance.wSet = 1;
                        objects[1].gameObject.SetActive(false);
                        //objects[1].GetComponent<Location>().isLocked = false;
                        yield return wait2000ms;
                        objects[2].gameObject.SetActive(true);


                        TeleportPlayer(objects[3]);
                        SceneController.instance.SetSomeConfiner(objects[4].GetComponent<PolygonCollider2D>());                        
                        //SceneController.instance.virtualCamera.Follow = null;
                        yield return wait2000ms;
                        UIManager.instance.SetGameEndUI(8);
                        yield return wait1000ms;

                        PlayerManager.instance.wSet = 0;
                        yield return wait1000ms;
                    }
                    else{
                    }

                }
                UIManager.instance.SetMovieEffectUI(false);


                break;
#endregion

#region @209 거대물약제조
            case 209 :

                SetDialogue(dialogues[0]);
                yield return waitTalking;
                SetSelect(selects[0]);
                yield return waitSelecting;
                if(GetSelect()==1){

                    InventoryManager.instance.RemoveItem(38);
                    InventoryManager.instance.AddItem(39,activateDialogue: true);
                }
                break;
#endregion      

#region @997 역행불가
            case 997 :
                int ranNum = Random.Range(0,2);
                //CameraView(dialogues[ranNum].talker);
                SetDialogue(dialogues[ranNum]);
                yield return waitTalking;
                break;
#endregion 
            
            default : 
                for(int i=0;i<dialogues.Length;i++){

                    SetDialogue(dialogues[i]);
                    yield return waitTalking;
                }


                break;
        }
        
        }

        else if(location.type == LocationType.Dialogue){
            
            for(int i=0;i<dialogues.Length;i++){
                SetDialogue(dialogues[i]);
                yield return waitTalking;
            }
            //DialogueManager.instance.SetFullDialogue(dialogues);
        }


        //yield return null;    

        UIManager.instance.SetHUD(true);

        //CameraView(PlayerManager.instance.transform);
        SceneController.instance.virtualCamera.Follow = PlayerManager.instance.transform;
        
//트리거 종료 후 배경음 볼륨 복구
        SoundManager.instance.bgmPlayer.volume = DBManager.instance.localData.bgmVolume;//MenuManager.instance.slider_bgm.value;


//타겟 지정된 트리거(타겟이 움직임)일 경우 트리거 종료 후 다시 이동
        if(location.target != null){
            var npc = location.target.GetComponent<NPCScript>();
            npc.isPaused = false;
            //if(npc.animator!=null) npc.animator.SetBool("talk", false);
        }

        if(!location.notZoom){
            if(GameManager.instance.mode_zoomWhenInteract){
                SceneController.instance.SetCameraDefaultZoomOut();
                SceneController.instance.SetSomeConfiner(SceneController.instance.mapBounds[DBManager.instance.curData.curMapNum]);
            }
        }
        //특정 트리거 종료 후, 튜토리얼 발생.
        if(location.trigNum == 6 && location.selectPhase == -1){

            UIManager.instance.OpenTutorialUI(4);
            yield return new WaitUntil(()=>!UIManager.instance.waitTutorial);
        }
        else if(location.trigNum == 999){

            UIManager.instance.OpenTutorialUI(7);
            yield return new WaitUntil(()=>!UIManager.instance.waitTutorial);
            UIManager.instance.OpenTutorialUI(6);
            yield return new WaitUntil(()=>!UIManager.instance.waitTutorial);

        }

        

        //아이템 획득 대화 있을 경우, 트리거 진행중 상태 유지
        //찐엔딩 시 상태 유지 추가 220801
        if(!location.holdPlayer && !InventoryManager.instance.waitGetItemDialogue && !PlayerManager.instance.watchingGameEnding){
            //PlayerManager.instance.canMove =true;  
            PlayerManager.instance.UnlockPlayer(); 
            //PlayerManager.instance.isActing =false;   
            //상호작용 키 연타로 인한 트리거 즉시 재시작 방지
            PlayerManager.instance.ActivateWaitInteract(PlayerManager.instance.delayTime_WaitingInteract);

        } 

        if(location.waitKey){
            location.locFlag = false; 

        }

        
        if(!location.preserveTrigger){
            if(location.selects_T.Length==0 || location.selectPhase == -1){
                DBManager.instance.TrigOver(location.trigNum);
                Debug.Log(location.trigNum +"번 트리거 실행 완료");
            }
            else{
                Debug.Log(location.trigNum +"번 트리거 실행 완료하였으나, 선택지가 종료되지 않아 다시 실행 가능");
            }
        }

        

        SceneController.instance.centerViewPoint.localPosition = Vector3.zero;

        SceneController.instance.virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 2;
        SceneController.instance.virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 2;
        //트리거 완료 혹은 재시작 가능 시 느낌표 재출력을 위해 로케이션 레이더 재활성화
        PlayerManager.instance.locationRader.ResetLocationRader();
        //PlayerManager.instance.ResetRigidbody();
    }
    public void TrigIsDone(Location location){

        Transform[] objects = null;

        if(location.poses != null){
            objects = location.poses;
        }
        else{
            objects = null;
        }

        switch(location.trigNum){
            case 15 :
                objects[2].gameObject.SetActive(false);
                objects[3].gameObject.SetActive(true);
                objects[4].gameObject.SetActive(false);

                break;

            default :
                break;
        }


        SceneController.instance.virtualCamera.Follow = PlayerManager.instance.transform;
        SceneController.instance.virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 2;
        SceneController.instance.virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 2;


        Debug.Log(location.trigNum + "(" + location.trigComment + ") 실행 처리됨");


    }
    

		/// <summary>
		/// <para> 대사 설정용</para>
		/// </summary>
    public void SetDialogue(Dialogue dialogue, string argument = null, bool onCameraCenter = true){
        if(dialogue==null){
            Debug.Log("대사 없음");
            return;
        }
        if(onCameraCenter){
            CameraView(dialogue.talker);
        }
        DialogueManager.instance.SetDialogue(dialogue , argument);
    }
    public void SetSelect(Select select, string[] arguments = null){
        if(select!=null){
            SelectManager.instance.SetSelect(select,arguments);

        }
        else{
            DM("Error : no selection");
        }
    }
    public int GetSelect(){
        return SelectManager.instance.GetSelect();
    }
    public void CameraView(Transform target, float speed=2){
        if(target==null){
            target = PlayerManager.instance.transform;
            //첫 대사가 럭키 대사일 경우
            if(SceneController.instance.centerViewPoint.localPosition == Vector3.zero){

                SceneController.instance.virtualCamera.Follow = target;
            }
            //첫 대사가 럭키 대사가 아닐 경우
            else{
                SceneController.instance.virtualCamera.Follow = SceneController.instance.centerViewPoint;
                
            }
        }
        else{
            Vector2 offset = target.position - PlayerManager.instance.transform.position;
            Vector2 centerPos = (target.position + PlayerManager.instance.transform.position) / 2;
            float closeDistance = 1.6f;

            var distance = Vector2.SqrMagnitude(offset);

            if(distance < closeDistance * closeDistance){
                SceneController.instance.centerViewPoint.position = centerPos;
                SceneController.instance.virtualCamera.Follow = SceneController.instance.centerViewPoint;
                
               //Debug.Log("가까이 : " + distance + "1 : "+ target.position + "2 : " + PlayerManager.instance.transform.position);
            }
            else{
//                   Debug.Log("멀리 : " + distance);
                    SceneController.instance.virtualCamera.Follow = target;
            }
        }

        // if(target!=null){
        //     SceneController.instance.virtualCamera.Follow = target;//ObjectController.instance.npcs[0].transform;
        // }
        // else{
        //     SceneController.instance.virtualCamera.Follow = PlayerManager.instance.transform;//ObjectController.instance.npcs[0].transform;
        // }
    }
    public void ActivateEffect(int num,float timer,bool bgOn = true){
        UIManager.instance.ActivateEffect(num,timer,bgOn);
    }
    public void SetHUD(bool active){
        UIManager.instance.SetHUD(active);
    }
    public void FadeOut(){
        LoadManager.instance.FadeOut();
    }
    public void FadeIn(){
        LoadManager.instance.FadeIn();
    }
    public IEnumerator OrderCoroutine(Location location,Transform objCol,Transform desCol){    
        
            PlayerManager.instance.canMove = false;
        
        if(desCol.position.x > objCol.position.x){
            PlayerManager.instance.wSet = 1;
        }
        else{
            PlayerManager.instance.wSet = -1;
        }
        yield return new WaitUntil(()=>PlayerManager.instance.orderDestinationCol == desCol);
        
        if(!location.stopCheck) PlayerManager.instance.canMove = true;
        else{
//            Debug.Log("3333333333");
            PlayerManager.instance.wSet = 0;
        }

        if(location.preserveTrigger) location.locFlag = false;
    }
    public IEnumerator OrderCoroutine_NPC(Location location,NPCScript objCol,Transform desCol){    
        
        if(desCol.position.x > objCol.transform.position.x){
            objCol.wSet = 1;
            
            //if(location.flipCheck) objCol.GetComponent<SpriteRenderer>().flipX = !objCol.GetComponent<SpriteRenderer>().flipX ;
        }
        else{
            objCol.wSet = -1;
            //if(location.flipCheck) objCol.GetComponent<SpriteRenderer>().flipX = !objCol.GetComponent<SpriteRenderer>().flipX ;
        }

        yield return wait1000ms;

        
        if(location.preserveTrigger) location.locFlag = false;
        //yield return new WaitUntil(()=>PlayerManager.instance.onTriggerCol == desCol);
        
        //if(location.preserveTrigger) location.locFlag = false;
    }
    public void ForceToPatrol(NPCScript npc){
        StartCoroutine(NPCPatrolCoroutineToStart(npc));
    }
    
    public IEnumerator NPCPatrolCoroutineToStart(NPCScript npc){ 
        WaitForSeconds _patrolInterval = new WaitForSeconds(npc.patrolInterval);
        while(npc.onPatrol && !npc.patrolFlag){
//            Debug.Log("NPCPatrolCoroutineToStart");
            npc.patrolFlag = true;  
            //DM("도착지로 이동");         
            if(npc.desPos.position.x > npc.transform.position.x){
                npc.wSet = 1;
            }
            else{
                npc.wSet = -1;
            }
            npc.animator.SetBool("walk",true);
            yield return new WaitUntil(()=>npc.onTriggerCol == npc.desPos);
            
            //DM("대기");         
            npc.wSet = 0;
            npc.animator.SetBool("walk",false);
            yield return _patrolInterval; 
            
            //DM("출발지로 이동");                 
            if(npc.startPos.position.x > npc.transform.position.x){
                npc.wSet = 1;
            }
            else{
                npc.wSet = -1;
            }
            npc.animator.SetBool("walk",true);
            yield return new WaitUntil(()=>npc.onTriggerCol == npc.startPos);
            //DM("대기");         
            npc.wSet = 0;
            npc.animator.SetBool("walk",false);
            yield return _patrolInterval; 

            npc.patrolFlag = false;
        }
    }

    public void DM(string msg) => DebugManager.instance.PrintDebug(msg);

    public void ShakeCamera(float intensity = 1, float duration = 1) => SceneController.instance.SetCameraNoised(intensity,duration);

    public void PlayerLookObject(Transform target) => PlayerManager.instance.PlayerLookObject(target);

    public void PlaySound(string soundFileName, float volume = 1f) => SoundManager.instance.PlaySound(soundFileName);

    public void AutoSave(){
        DBManager.instance.CallSave(99);
        LoadManager.instance.lastLoadFileNum = 99;
    }
    public void GetItemDelay(float delayTime){
        //InventoryManager.instance.AddItem
    }
    public void TeleportPlayer(Transform destination){
        
        PlayerManager.instance.transform.position = destination.position;
    }
    public void SetObjectActive(Transform[] objects, int index ,bool active){
        if(objects.Length <= index ) return;

        objects[index].gameObject.SetActive(active);
    }
}
