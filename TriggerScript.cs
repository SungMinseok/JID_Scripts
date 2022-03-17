using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TriggerScript : MonoBehaviour
{    
    public static TriggerScript instance;
    WaitForSeconds wait100ms = new WaitForSeconds(0.1f);
    WaitForSeconds wait500ms = new WaitForSeconds(0.5f);
    WaitForSeconds wait1000ms = new WaitForSeconds(1);
    WaitForSeconds wait2000ms = new WaitForSeconds(2);
    WaitForSeconds wait3000ms = new WaitForSeconds(3);

    WaitUntil waitTalking = new WaitUntil(()=>!PlayerManager.instance.isTalking);
    WaitUntil waitSelecting = new WaitUntil(()=>!PlayerManager.instance.isSelecting);
    WaitUntil waitShopping = new WaitUntil(()=>!PlayerManager.instance.isShopping);
    WaitUntil waitMoving = new WaitUntil(()=>PlayerManager.instance.canMove);
    
    void Awake()
    {
            instance = this;
    }
    void Start(){
        //SoundManager.instance.PlaySound("button0");
    }
    public void PreAction(Location location){
        Transform[] objects;
        objects = location.poses;

        if(DBManager.instance.CheckTrigOver(location.trigNum)){
            switch(location.trigNum){
                case 22 :
                    //location.poses[10].gameObject.SetActive(false);
                        for(int i=0;i<14;i++){
                            location.poses[i].gameObject.SetActive(false);
                        }

                    break;                
                case 23 :
                    //objects = location.poses;
                    objects[17].GetComponent<Location>().isLocked = false;
                    for(int i=8;i<17;i++){
                        objects[i].gameObject.SetActive(false);
                    }
                    break;     
                case 27 :
                    objects[1].gameObject.SetActive(false);
                    //if(!DBManager.instance.CheckTrigOver(location.trigNum)){
                    //    objects[0].gameObject.SetActive(true);
                    //} 
                    break;
                case 9 :
                case 37 :
                    objects[0].gameObject.SetActive(true);
                    break;
                case 38 :
                    objects[0].GetComponent<Rigidbody2D>().mass = 30f;
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
                default :
                    break;
            }
        }
    }

   //public void Action(Location location, Dialogue[] dialogues = null, Select[] selects = null, Transform[] objects = null){
    public void Action(Location location){
        //Debug.Log("트리거 발동" + trigNum);
        // if(selects != null){
        //     Debug.Log(selects[0].answers[0]);
        // }
        // else{
        //     Debug.Log("선택지 없음");
        // }
        
//StartCoroutine(ActionCoroutine(location, dialogues, selects, objects));
        StartCoroutine(ActionCoroutine(location));
//        Debug.Log("44");

    }

    //IEnumerator ActionCoroutine(int trigNum, Dialogue[] dialogues, Select[] selects, Transform[] objects){
    IEnumerator ActionCoroutine(Location location){
        PlayerManager.instance.canMove =false;

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

//타겟 지정된 트리거(타겟이 움직임)일 경우 트리거 중 움직이지 않고, 바라보도록 함
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
            if(npc.lookPlayer){
                if(npc.mainBody.position.x > PlayerManager.instance.transform.position.x){
                    npc.Look("left");
                }
                else{
                    npc.Look("right");

                }
            }


            //if(npc.animator!=null) npc.animator.SetBool("talk", true);
        }
        else{
            if(location.transform.position.x > PlayerManager.instance.transform.position.x){
                PlayerManager.instance.Look("right");
            }
            else{
                PlayerManager.instance.Look("left");

            }
            PlayerManager.instance.SetTalkCanvasDirection();
        }
        if(!location.notZoom){

            if(GameManager.instance.mode_zoomWhenInteract){

                SceneController.instance.SetCameraDefaultZoomIn();
                SceneController.instance.SetSomeConfiner(SceneController.instance.mapZoomBounds[DBManager.instance.curData.curMapNum]);
            }
        }


        //일반 트리거 1~100일 경우, 메인 HUD 비활성화
        if(location.trigNum<300){
            UIManager.instance.SetHUD(false);
        }


        SceneController.instance.virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 0;
        SceneController.instance.virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 0;


        if(location.type == LocationType.Trigger){
           
            switch(location.trigNum){

//저장개미
#region 999
            case 999 :
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                SetSelect(selects[0]);
                yield return new WaitUntil(()=>!PlayerManager.instance.isSelecting);
                if(GetSelect()==0){
                    MenuManager.instance.savePanel.SetActive(true);
                    yield return new WaitUntil(()=>!MenuManager.instance.savePanel.activeSelf);
                }
                break;
#endregion

//상점 - 지렁이 
#region 101
            case 101 :

                ShopManager.instance.OpenShopUI(0,"지렁이 상점",new int[]{5,12});
                yield return waitShopping;

                if(ShopManager.instance.lastBuyItemIndex != -1){

                    SetDialogue(dialogues[0]);
                    yield return waitTalking;
                }



                break;
#endregion

//상점 - 귀뚜라미 
#region 102
            case 102 :


                ShopManager.instance.OpenShopUI(0,"귀뚜라미 상점",new int[]{19,16,3});//붉은 산딸기 / 보라색 열매 / 사과 조각
                yield return waitShopping;


                if(ShopManager.instance.lastBuyItemIndex != -1){

                    SetDialogue(dialogues[0]);
                    yield return waitTalking;
                }

                break;
#endregion
//상점 - 벼룩 
#region 103
            case 103 :

                ShopManager.instance.OpenShopUI(0,"벼룩 상점",new int[]{24,18,25});
                yield return waitShopping;

                if(ShopManager.instance.lastBuyItemIndex != -1){

                    SetDialogue(dialogues[0]);
                    yield return waitTalking;
                }


                break;
#endregion
//상점 - 땃쥐 
#region 104
            case 104 :

                SetDialogue(dialogues[0]);
                yield return waitTalking;

                ShopManager.instance.OpenShopUI(0,"땃쥐 상점",new int[]{23,20});
                yield return waitShopping;


                if(ShopManager.instance.lastBuyItemIndex != -1){

                    SetDialogue(dialogues[1]);
                    yield return waitTalking;
                }


                break;
#endregion
//상점 - 두더지 
#region 105
            case 105 :

                SetDialogue(dialogues[0]);
                yield return waitTalking;

                ShopManager.instance.OpenShopUI(0,"두더지 상점",new int[]{14,11});
                yield return waitShopping;

                if(ShopManager.instance.lastBuyItemIndex == 0){

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
                else if(ShopManager.instance.lastBuyItemIndex == 1){

                    SetDialogue(dialogues[1]);
                    yield return waitTalking;
                    SetDialogue(dialogues[2]);
                    yield return waitTalking;
                    SetDialogue(dialogues[6]);
                    yield return waitTalking;
                }

                break;
#endregion



//여긴 어디?
#region 1
            case 1 :
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                PlayerManager.instance.Look("right");
                SetDialogue(dialogues[1]);
                yield return waitTalking;
                PlayerManager.instance.Look("left");
                //MapManager.instance.virtualCamera.Follow = null;
                //ObjectController.instance.npcs[0].animator.SetTrigger("wakeUp");
                break;
#endregion

//먹이창고 탈출
#region 3
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
                //CameraView(objects[0]);
                yield return waitTalking;
                SetDialogue(dialogues[2]);
                nerd_ant.animator.SetTrigger("sweat");
                yield return waitTalking;
                SetDialogue(dialogues[3]);
                nerd_ant.animator.SetTrigger("turn");
                //CameraView(nerd_ant.transform);
                ShakeCamera(1,2);
                yield return new WaitForSeconds(1.2f);
                nerd_ant.wSet = -1;
                yield return waitTalking;
                yield return wait2000ms;
                FadeOut();
                yield return wait1000ms;
                PlayerManager.instance.vignette.SetActive(true);
                objects[1].gameObject.SetActive(false);
                nerd_ant.gameObject.SetActive(false);
                PlayerManager.instance.transform.position = objects[0].position;
                SceneController.instance.SetConfiner(objects[0].parent.transform.GetSiblingIndex());
                yield return new WaitForSeconds(0.1f);
                SetHUD(true);
                CameraView(PlayerManager.instance.transform);
                FadeIn();

                //MapManager.instance.virtualCamera.Follow = null;
                //ObjectController.instance.npcs[0].animator.SetTrigger("wakeUp");

                break;
#endregion

//수배지 확인
#region 4
            case 4 :
                ActivateEffect(1,3);
                yield return new WaitUntil(()=>!UIManager.instance.onEffect);
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                PlayerManager.instance.Look("left");
                break;
#endregion

//복도 경비병 만남
#region 5
            case 5 :

                FadeOut();
                yield return wait1000ms;
                SetHUD(false);
                PlayerManager.instance.vignette.SetActive(false);
                objects[1].gameObject.SetActive(true);
                
                SceneController.instance.SetConfiner(2);
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
                yield return wait3000ms;

                FadeOut();
                yield return wait1000ms;
                PlayerManager.instance.vignette.SetActive(true);
                objects[1].gameObject.SetActive(false);
                
                PlayerManager.instance.transform.position = objects[2].position;

                CameraView(PlayerManager.instance.transform);
                objects[0].GetComponent<NPCScript>().onPatrol = true;
                objects[3].GetComponent<NPCScript>().onPatrol = true;
                objects[3].GetComponent<NPCScript>().onRandomDialogue = true;

                ForceToPatrol(objects[0].GetComponent<NPCScript>());
                ForceToPatrol(objects[3].GetComponent<NPCScript>());

                SetHUD(true);
                CameraView(PlayerManager.instance.transform);
                FadeIn();
                break;
#endregion

//노개미에게 말을 건다.(선택지)
#region 6
            case 6 :

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
                    }
                }
                break;
#endregion

//화난 노개미에게 말을 다시 건다.
#region 7
            case 7 :
                
                dialogues[0].talker.GetComponent<NPCScript>().animator.SetBool("mad", true);
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                dialogues[0].talker.GetComponent<NPCScript>().animator.SetBool("mad", false);
                break;
#endregion

//[미니게임0 - 종이 오리기] 유치원 센세에게 말을 건다.
#region 8
            case 8 :
            
                CameraView(objects[0]);
                
                for(int k=0;k<10;k++){
                    CameraView(dialogues[k].talker);
                    SetDialogue(dialogues[k]);
                    yield return waitTalking;
                }

                //CheatManager.instance.InputCheat("minigame 0");
                MinigameManager.instance.StartMinigame(0);
                yield return new WaitUntil(()=>MinigameManager.instance.success);
                MinigameManager.instance.success = false;
                
                PlayerManager.instance.canMove =true;   
                SetDialogue(dialogues[10]);
                yield return waitTalking;
                SetDialogue(dialogues[11]);
                yield return waitTalking;


                CameraView(PlayerManager.instance.transform);

                // SetSelect(selects[0]);
                // yield return new WaitUntil(()=>!PlayerManager.instance.isSelecting);
                // if(GetSelect()==0){

                //     SetDialogue(dialogues[1]);
                //     yield return waitTalking;
                //     SetSelect(selects[1]);
                //     yield return new WaitUntil(()=>!PlayerManager.instance.isSelecting);
                //     if(GetSelect()==0){
                //     }
                //     else if(GetSelect()==1){
                        
                //         SetDialogue(dialogues[2]);
                //         yield return waitTalking;
                //         SetDialogue(dialogues[3]);
                //         yield return waitTalking;
                //         SetDialogue(dialogues[4]);
                //         yield return waitTalking;
                //     }
                // }
                // else if(GetSelect()==1){
                    
                // }






                break;
#endregion

//꽃핀 가졌을 때 유치원 센세 말걸기
#region 9
            case 9 :
                
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                SetDialogue(dialogues[1]);
                yield return waitTalking;
                SetSelect(selects[0]);
                yield return waitSelecting;
                if(GetSelect()==0){
                    location.selectPhase = -1;
                    InventoryManager.instance.RemoveItem(6);
                    SetDialogue(dialogues[2]);
                    yield return waitTalking;

                    objects[0].gameObject.SetActive(true);


                }
                else if(GetSelect()==1){
                    
                }






                break;
#endregion

//꽃핀을 가진 아이
#region 10
            case 10 :
                
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                InventoryManager.instance.AddItem(6);
                break;
#endregion

//수레개미에게 말을 건다.
#region 11
            case 11 :
                
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                InventoryManager.instance.AddItem(7);
                break;
#endregion

//[미니게임1 - 로메슈제 제조] 술먹는 개미에게 말을 건다.
#region 12
            case 12 :   
                
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                InventoryManager.instance.RemoveItem(8);
                SetDialogue(dialogues[1]);
                yield return waitTalking;
                SetDialogue(dialogues[2]);
                yield return waitTalking;
                SetDialogue(dialogues[3]);
                yield return waitTalking;

                
                MinigameManager.instance.StartMinigame(1);
                // CheatManager.instance.InputCheat("minigame 1");
                yield return new WaitUntil(()=>MinigameManager.instance.success);
                break;
#endregion

//"로메슈제" 미니게임 성공 후 취한 개미 ( 선행 : 12 )
#region 13
            case 13 :   
                
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                break;
#endregion

//중독 수개미에게 말을 건다.
#region 14
            case 14 :   
                
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                SetDialogue(dialogues[1]);
                yield return waitTalking;
                SetDialogue(dialogues[2]);
                yield return waitTalking;
                SetDialogue(dialogues[3]);
                yield return waitTalking;
                break;
#endregion

//[미니게임2 - 미친 수개미 피하기] 빛나는 양동이를 클릭한다.
#region 15
            case 15 :   
                objects[2].gameObject.SetActive(false);
                objects[3].gameObject.SetActive(true);
                yield return wait1000ms;
                PlayerManager.instance.Look("left");

                SetDialogue(dialogues[0]);
                yield return waitTalking;

                //LoadManager.instance.FadeOut();
                //yield return wait1000ms;
                //PlayerManager.instance.transform.position = objects[0].position;
                //SceneController.instance.SetSomeConfiner(objects[1].GetComponent<PolygonCollider2D>());
                
                
                //LoadManager.instance.FadeIn();
                //objects[4].gameObject.SetActive(false);

                MinigameManager.instance.StartMinigame(2);
                //PlayerManager.instance.canMove = true;
                //SceneController.instance.virtualCamera.Follow = null;
                //yield return new WaitUntil(()=>MinigameManager.instance.success);
                



                //성공 시 끝맵으로 이동.(FI/FO)
                
                break;
#endregion

//"도망" 미니게임 성공 , 끝 맵으로 이동
#region 16
            case 16 :   
                
                // UIManager.instance.SetFadeOut();
                // yield return wait1000ms;
                SceneController.instance.virtualCamera.Follow = PlayerManager.instance.transform;
                
                PlayerManager.instance.transform.position = objects[0].position;
                SceneController.instance.SetConfiner(7);

                //SceneController.instance.SetSomeConfiner(SceneController.instance.mapBounds[7]);
                //UIManager.instance.SetFadeIn();
                
                
                break;
#endregion

//"도망" 미니게임 성공 후, 노개미 만남
#region 17
            case 17 :   
                
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                SetDialogue(dialogues[1]);
                yield return waitTalking;
                SetDialogue(dialogues[2]);
                yield return waitTalking;
                //UIManager.instance.SetFadeOut();
                LoadManager.instance.FadeOut();
                yield return wait1000ms;

                
                PlayerManager.instance.transform.position = objects[0].position;
                SceneController.instance.SetConfiner(8);
                //UIManager.instance.SetFadeIn();
                LoadManager.instance.FadeIn();
                
                
                break;
#endregion

//"도망" 미니게임 성공 후, 수레개미 만남
#region 18
            case 18 :   
                
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                SetDialogue(dialogues[1]);
                yield return waitTalking;
                //UIManager.instance.SetFadeOut();
                
                LoadManager.instance.FadeOut();
                yield return wait1000ms;

                
                PlayerManager.instance.transform.position = objects[0].position;
                SceneController.instance.SetConfiner(8);
                //UIManager.instance.SetFadeIn();
                
                LoadManager.instance.FadeIn();
                
                
                break;
                
#endregion
//노개미방 책장
#region 19
            case 19 :   
                
                // SetDialogue(dialogues[0]);
                // yield return waitTalking;
                SetSelect(selects[0]);
                yield return waitSelecting;
                
                SetDialogue(dialogues[GetSelect()]);
                yield return waitTalking;
                
                    
                
                
                break;
#endregion
//요리사 로메슈제
#region 22
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
                        for(int i=0;i<14;i++){
                            objects[i].gameObject.SetActive(false);
                        }
                        
                        objects[14].GetChild(1).GetComponent<Animator>().SetBool("hang", false);
                        objects[14].gameObject.SetActive(true);
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

                    }
                    else if(GetSelect()==1){
                        
                    }
                }

                break;
#endregion
            
//진딧물농장 미니게임4
#region 23
            case 23 :

                FadeOut();
                yield return wait1000ms;
                SetHUD(false);
                PlayerManager.instance.vignette.SetActive(false);
                
                SceneController.instance.SetConfiner(15);
                PlayerManager.instance.transform.position = objects[5].position;
                yield return wait100ms;

                CameraView(objects[0]);
                yield return wait500ms;
                FadeIn();

                for(int k=0;k<9;k++){
                    CameraView(dialogues[k].talker);
                    SetDialogue(dialogues[k]);
                    yield return waitTalking;
                }
                yield return wait500ms;


                CameraView(PlayerManager.instance.transform);

                SetSelect(selects[0]);
                yield return new WaitUntil(()=>!PlayerManager.instance.isSelecting);
                if(GetSelect()==0){
                }
                else if(GetSelect()==1){
                    
                }
                location.selectPhase = -1;

                yield return wait500ms;

                SceneController.instance.SetCameraDefaultZoomOut();

                MinigameManager.instance.StartMinigame(4);
                // CheatManager.instance.InputCheat("minigame 1");
                yield return new WaitUntil(()=>MinigameManager.instance.success || MinigameManager.instance.fail);

                yield return wait1000ms;
                PlayerManager.instance.vignette.SetActive(true);
                for(int i=8;i<17;i++){
                    objects[i].gameObject.SetActive(false);
                }
                PlayerManager.instance.transform.position = objects[6].position;
                PlayerManager.instance.Look("right");
                objects[7].gameObject.SetActive(true);
                SceneController.instance.SetCameraDefaultZoomIn();


                SetHUD(false);

                yield return wait500ms;
                if(GetSelect()==0){
                    SetDialogue(dialogues[9]);
                    yield return waitTalking;
                }
                else if(GetSelect()==1){
                    
                    SetDialogue(dialogues[10]);
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

                objects[17].GetComponent<Location>().isLocked = false;


                //FadeIn();
                break;
#endregion
            
//알번데기방 - 꼰대개미와 대화
#region 24
            case 24 :

                CameraView(dialogues[0].talker);
                SetDialogue(dialogues[0]);
                yield return waitTalking;

                SetSelect(selects[0]);
                yield return waitSelecting;
                
                CameraView(dialogues[1].talker);
                SetDialogue(dialogues[1]);
                yield return waitTalking;

                SetSelect(selects[1]);
                yield return waitSelecting;

                CameraView(dialogues[2].talker);
                SetDialogue(dialogues[2]);
                yield return waitTalking;

                
                PlayerManager.instance.Look("left");

                CameraView(dialogues[3].talker);
                SetDialogue(dialogues[3]);
                yield return waitTalking;

                PlayerManager.instance.Look("right");

                CameraView(dialogues[4].talker);
                SetDialogue(dialogues[4]);
                yield return waitTalking;

                SetSelect(selects[2]);
                yield return waitSelecting;
                
                CameraView(dialogues[5].talker);
                SetDialogue(dialogues[5]);
                yield return waitTalking;
                
                location.selectPhase = -1;

                break;
#endregion
            
//대왕일개미방 입구 
#region 25
            case 25 :

                if(!DBManager.instance.CheckTrigOver(24)){
                    SetDialogue(dialogues[0]);
                    yield return waitTalking;
                }
                else{
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
                        
                        CameraView(objects[4]);
                                
                        SceneController.instance.SetCameraDefaultZoomIn();
                        SceneController.instance.SetSomeConfiner(SceneController.instance.mapZoomBounds[DBManager.instance.curData.curMapNum]);
                        yield return new WaitUntil(()=>!PlayerManager.instance.isActing);
                        //PlayerManager.instance.wSet = -1;

                    }
                    location.preserveTrigger = false;
                }

                break;
#endregion        

//대왕일개미방 내부 
#region 26
            case 26 :


                SetDialogue(dialogues[0]);
                yield return waitTalking;
                SetSelect(selects[0]);
                yield return waitSelecting;
                
                if(GetSelect()==0){
                    for(int i=1;i<6;i++){
                        SetDialogue(dialogues[i]);
                        yield return waitTalking;
                    }
                    if(InventoryManager.instance.CheckHaveItem(3)
                    ||InventoryManager.instance.CheckHaveItem(16)
                    ||InventoryManager.instance.CheckHaveItem(19)
                    ){
                                
                        SetDialogue(dialogues[6]);
                        yield return waitTalking;
                        SetDialogue(dialogues[7]);
                        yield return waitTalking;
                        SetSelect(selects[1]);
                        yield return waitSelecting;
                        if(GetSelect()==0){
                            for(int i=8;i<14;i++){
                                SetDialogue(dialogues[i]);
                                yield return waitTalking;
                            }
                        }
                        else{
                            SetDialogue(dialogues[15]);
                            yield return waitTalking;
                            FadeOut();
                            yield return wait1000ms;
                            SceneController.instance.SetConfiner(25);
                            PlayerManager.instance.transform.position = objects[0].transform.position;
                            FadeIn();
                            
                            SetDialogue(dialogues[16]);
                            yield return waitTalking;

                            //방나가짐
                        }

                    }
                    else{
                        //방나가짐
                            FadeOut();
                            yield return wait1000ms;
                            SceneController.instance.SetConfiner(25);
                            PlayerManager.instance.transform.position = objects[0].transform.position;
                            FadeIn();
                    }
                }
                else{
                    SetDialogue(dialogues[14]);
                    yield return waitTalking;

                    //죽음
                    UIManager.instance.SetGameOverUI(4);
                }

                location.selectPhase = -1;



                break;
#endregion

//박사개미에게 말 
#region 27
            case 27 :
                PlayerManager.instance.SetTalkCanvasDirection();

                var animator = objects[1].GetComponent<NPCScript>().mainBody.GetComponent<Animator>();

                if(location.selectPhase == 0){
                    location.selectPhase = 1;
                    
                    CameraView(dialogues[0].talker);
                    //yield return new WaitUntil(()=>animator.GetCurrentAnimatorStateInfo(0).IsName("Ant_Scientist_idle"));
                    SetDialogue(dialogues[0]);
                    yield return waitTalking;
                }

                
                if(InventoryManager.instance.CheckHaveItem(10)){
                    //PlayerManager.instance.SetTalkCanvasDirection("left");
                    CameraView(dialogues[1].talker);
                    //yield return new WaitUntil(()=>animator.GetCurrentAnimatorStateInfo(0).IsName("Ant_Scientist_idle"));
                    SetDialogue(dialogues[1]);
                    yield return waitTalking;
                    SetSelect(selects[0]);
                    yield return waitSelecting;
                    if(GetSelect()==0){
                        location.selectPhase = -1;
                        InventoryManager.instance.RemoveItem(10);
                        CameraView(dialogues[2].talker);
                        SetDialogue(dialogues[2]);
                        yield return waitTalking;
                        CameraView(dialogues[3].talker);
                        SetDialogue(dialogues[3]);
                        yield return waitTalking;
                        animator.SetBool("mess", true);
                        SoundManager.instance.PlaySound("makingsound");

                        yield return new WaitForSeconds(5.3f);

                        animator.SetBool("success", true);

                        yield return wait100ms;
                        objects[0].gameObject.SetActive(true);
                        CameraView(dialogues[4].talker);
                        SetDialogue(dialogues[4]);
                        yield return waitTalking;
                        animator.SetBool("success", false);
                        
                        SetDialogue(dialogues[5]);
                        yield return waitTalking;
                        FadeOut();
                        yield return wait1000ms;
                        //objects[0].gameObject.SetActive(true);
                        objects[1].gameObject.SetActive(false);
                        yield return wait1000ms;
                        FadeIn();
                    }
                    else{
                    }
                }
                else{
                    
                }


                break;
#endregion
//박사개미의 완성품 획득 
#region 28
            case 28 :

                SetDialogue(dialogues[0]);
                yield return waitTalking;
                SetSelect(selects[0]);
                yield return waitSelecting;
                if(GetSelect()==0){
                    location.selectPhase = -1;
                    objects[0].gameObject.SetActive(false);
                    InventoryManager.instance.AddItem(1);
                }
                else{
                }

                //location.selectPhase = -1;

                break;
#endregion
//병원에 누워있는 병사 개미
#region 29
            case 29 :

                if(location.selectPhase == 0){
                    location.selectPhase = 1;
                    SetDialogue(dialogues[0]);
                    yield return waitTalking;
                }
                SetDialogue(dialogues[1]);
                yield return waitTalking;

                if(InventoryManager.instance.CheckHaveItem(8)){
                    InventoryManager.instance.RemoveItem(8);
                    SetSelect(selects[0]);
                    yield return waitSelecting;
                    if(GetSelect()==0){
                        location.selectPhase = -1;
                            
                        for(int k=2;k<7;k++){
                            CameraView(dialogues[k].talker);
                            SetDialogue(dialogues[k]);
                            yield return waitTalking;
                        }
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
//룰렛 ( 미니게임 5 )
#region 31
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
                    MinigameManager.instance.minigameScriptTransforms[5].gameObject.SetActive(true);
                    yield return waitMoving;
                }
                else{
                    
                    SetDialogue(dialogues[2]);
                    yield return waitTalking;
                }
                tempAnimator.SetBool("up", false);

                break;
#endregion 
             
//물약제조 ( 미니게임 3 )
#region 32
            case 32 :
                MinigameManager.instance.minigameScriptTransforms[3].gameObject.SetActive(true);
                yield return waitMoving;
                
                break;
#endregion 
              
//지네상점, 쥐며느리상점
#region 33, 34
            case 33 :
            case 34 :
                CameraView(dialogues[0].talker);
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                break;
#endregion 
//거미상점
#region 36
            case 36 :

                if(location.selectPhase == 0){
                    location.selectPhase = 1;
                    SetDialogue(dialogues[0]);
                    yield return waitTalking;
                    InventoryManager.instance.AddDirt(20);
                }
                else if(location.selectPhase == 1){
                    SetDialogue(dialogues[1]);
                    yield return waitTalking;
                    
                    SetSelect(selects[0]);
                    yield return waitSelecting;
                    if(GetSelect()==0){
                        location.selectPhase = -1;
                        InventoryManager.instance.AddDirt(20);


                    }
                }
                break;
#endregion 

//연못앞
#region 37
            case 37 :
                
                CameraView(dialogues[0].talker);
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

                    }
                }

                break;
#endregion 


//연못앞
#region 38
            case 38 :
                
                CameraView(dialogues[0].talker);
                SetDialogue(dialogues[0]);
                yield return waitTalking;

                objects[0].GetComponent<Rigidbody2D>().mass = 30f;

                break;
#endregion 


//[엔딩1 : 여왕의 방 - 전설의 젤리(젤할라)]
#region 201
            case 201 :
                FadeOut();
                yield return wait2000ms;
                FadeIn();
                PlayerManager.instance.Look("right");
        
                for(int k=0;k<14;k++){
                    CameraView(dialogues[k].talker);
                    SetDialogue(dialogues[k]);
                    yield return waitTalking;
                }

                ShakeCamera();
                CameraView(dialogues[14].talker);
                SetDialogue(dialogues[14]);
                yield return waitTalking;

                for(int k=15;k<dialogues.Length;k++){
                    CameraView(dialogues[k].talker);
                    SetDialogue(dialogues[k]);
                    yield return waitTalking;
                }

                UIManager.instance.SetGameEndUI(1);
                break;
#endregion 

//[엔딩2 : 여왕의 방 - 사랑의 도피]
#region 202
            case 202 :
                FadeOut();
                yield return wait1000ms;
                PlayerManager.instance.Look("right");
                PlayerManager.instance.SetTalkCanvasDirection();
                yield return wait1000ms;
                FadeIn();
        
                for(int k=0;k<11;k++){
                    CameraView(dialogues[k].talker);
                    SetDialogue(dialogues[k]);
                    yield return waitTalking;
                }

                ShakeCamera();
                CameraView(dialogues[11].talker);
                SetDialogue(dialogues[11]);
                yield return waitTalking;

                for(int k=12;k<dialogues.Length;k++){
                    CameraView(dialogues[k].talker);
                    SetDialogue(dialogues[k]);
                    yield return waitTalking;
                }

                UIManager.instance.SetGameEndUI(2);
                break;
#endregion 
//[엔딩3 : 개미굴에서 젤리난다.]
#region 203
            case 203 :
                FadeOut();
                yield return wait1000ms;
                PlayerManager.instance.Look("right");
                PlayerManager.instance.SetTalkCanvasDirection();
                yield return wait1000ms;
                FadeIn();
        
                for(int k=0;k<dialogues.Length;k++){
                    CameraView(dialogues[k].talker);
                    SetDialogue(dialogues[k]);
                    yield return waitTalking;
                }

                UIManager.instance.SetGameEndUI(3);
                break;
#endregion 
//[엔딩4 : 살육의밤]
#region 204
            case 204 :
                FadeOut();
                yield return wait1000ms;
                PlayerManager.instance.Look("right");
                PlayerManager.instance.SetTalkCanvasDirection();
                yield return wait1000ms;
                FadeIn();
        
                for(int k=0;k<dialogues.Length;k++){
                    CameraView(dialogues[k].talker);
                    SetDialogue(dialogues[k]);
                    yield return waitTalking;
                }

                UIManager.instance.SetGameEndUI(4);
                break;
#endregion 
//[엔딩5 : 꼭두각시]
#region 205
            case 205 :
                FadeOut();
                yield return wait1000ms;
                PlayerManager.instance.Look("right");
                PlayerManager.instance.SetTalkCanvasDirection();
                yield return wait1000ms;
                FadeIn();
        
                CameraView(dialogues[0].talker);
                SetDialogue(dialogues[0]);
                yield return waitTalking;
                CameraView(dialogues[1].talker);
                SetDialogue(dialogues[1]);
                yield return waitTalking;

                SetSelect(selects[0]);
                yield return waitSelecting;
                
                if(GetSelect()==1){

                    for(int k=2;k<dialogues.Length;k++){
                        CameraView(dialogues[k].talker);
                        SetDialogue(dialogues[k]);
                        yield return waitTalking;
                    }

                    
                    SetSelect(selects[1]);
                    yield return waitSelecting;

                    location.selectPhase = -1;
                    UIManager.instance.SetGameEndUI(5);
                }
                else{
                    FadeOut();
                    yield return wait1000ms;
                    objects[2].GetComponent<Location>().LocationScript(PlayerManager.instance.bodyCollider2D);
                    //PlayerManager.instance.transform.position = objects[1].transform.position;
                    yield return wait100ms;
                    FadeIn();


                }

                break;
#endregion 

//[엔딩6-친구와 함께라면]
#region 206
            case 206 :
        
                FadeOut();
                yield return wait1000ms;
                PlayerManager.instance.Look("left");
                PlayerManager.instance.SetTalkCanvasDirection();
                objects[0].gameObject.SetActive(true);
                CameraView(objects[3]);
                yield return wait1000ms;
                FadeIn();

                for(int k=0;k<3;k++){
                    //CameraView(dialogues[k].talker);
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
                    objects[1].gameObject.SetActive(true);
                    yield return wait1000ms;
                    FadeIn();
                    for(int k=4;k<6;k++){
                        //CameraView(dialogues[k].talker);
                        SetDialogue(dialogues[k]);
                        yield return waitTalking;
                    }

                    //럭키 문 나가기 & 노란젤리 문으로 걸어가기

                    UIManager.instance.SetGameEndUI(6);

                }

                break;
#endregion 

//[엔딩7 - 여행가]
#region 207
            case 207 :
        
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
                    yield return wait1000ms;
                    FadeIn();

                    SetDialogue(dialogues[1]);
                    yield return waitTalking;
                    

                    //럭키 문 나가기 & 노란젤리 문으로 걸어가기

                    UIManager.instance.SetGameEndUI(7);

                }

                break;
#endregion 
//[엔딩8 - 산제물]
#region 208
            case 208 :
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
                        PlayerManager.instance.speed *= 0.6f;
                        PlayerManager.instance.wSet = 1;
                        objects[1].gameObject.SetActive(false);
                        //objects[1].GetComponent<Location>().isLocked = false;
                        yield return wait3000ms;
                        PlayerManager.instance.wSet = 0;

                        UIManager.instance.SetGameEndUI(8);
                    }
                    else{
                    }

                }


                break;
#endregion
            default : 
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


        yield return null;    

        UIManager.instance.SetHUD(true);

        CameraView(PlayerManager.instance.transform);

//타겟 지정된 트리거(타겟이 움직임)일 경우 트리거 종료 후 다시 이동
        if(location.target != null){
            var npc = location.target.GetComponent<NPCScript>();
            npc.isPaused = false;
            //if(npc.animator!=null) npc.animator.SetBool("talk", false);
        }
        
        PlayerManager.instance.isActing =false;    
        PlayerManager.instance.canMove =true;   

        //yield return wait1000ms;
        PlayerManager.instance.ActivateWaitInteract(PlayerManager.instance.delayTime_WaitingInteract);
        location.locFlag = false; 

        
        if(!location.preserveTrigger){
            if(location.selects_T.Length==0 || location.selectPhase == -1){
                DBManager.instance.TrigOver(location.trigNum);
                Debug.Log(location.trigNum +"번 트리거 실행 완료");
            }
            else{
                Debug.Log(location.trigNum +"번 트리거 실행 완료하였으나, 선택지가 종료되지 않아 다시 실행 가능");
            }
        }

        if(!location.notZoom){
            if(GameManager.instance.mode_zoomWhenInteract){
                SceneController.instance.SetCameraDefaultZoomOut();
                SceneController.instance.SetSomeConfiner(SceneController.instance.mapBounds[DBManager.instance.curData.curMapNum]);
            }
        }
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


        SceneController.instance.virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 2;
        SceneController.instance.virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 2;


        Debug.Log(location.trigNum + "(" + location.trigComment + ") 실행 처리됨");


    }
    

    public void SetDialogue(Dialogue dialogue){
        if(dialogue==null){
            Debug.Log("대사 없음");
        }
        DialogueManager.instance.SetDialogue(dialogue);
    }
    public void SetSelect(Select select, string[] argument0 = null){
        if(select!=null){
            SelectManager.instance.SetSelect(select,argument0);

        }
        else{
            DM("Error : no selection");
        }
    }
    public int GetSelect(){
        return SelectManager.instance.GetSelect();
    }
    public void CameraView(Transform target, float speed=2){
        if(target!=null){
            SceneController.instance.virtualCamera.Follow = target;//ObjectController.instance.npcs[0].transform;
        }
        else{
            SceneController.instance.virtualCamera.Follow = PlayerManager.instance.transform;//ObjectController.instance.npcs[0].transform;

            //DM("Error : no pos");
        }
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
            Debug.Log("3333333333");
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

}
