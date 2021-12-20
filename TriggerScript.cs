﻿using System.Collections;
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
    
    void Awake()
    {
            instance = this;
    }
    void Start(){
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
        if(!location.notZoom){

            if(GameManager.instance.mode_zoomWhenInteract){

                SceneController.instance.SetLensOrthoSize(4f,0.075f);
                SceneController.instance.SetSomeConfiner(SceneController.instance.mapZoomBounds[DBManager.instance.curData.curMapNum]);
            }
        }

        if(location.type == LocationType.Trigger){
           
            switch(location.trigNum){



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
                
                SceneController.instance.npcs[1].gameObject.SetActive(true);
                SceneController.instance.npcs[2].gameObject.SetActive(true);
                yield return wait3000ms;

                FadeOut();
                yield return wait1000ms;
                PlayerManager.instance.vignette.SetActive(true);
                objects[1].gameObject.SetActive(false);
                
                PlayerManager.instance.transform.position = objects[2].position;

                CameraView(PlayerManager.instance.transform);
                SceneController.instance.npcs[1].onPatrol = true;
                SceneController.instance.npcs[2].onPatrol = true;
                SceneController.instance.npcs[1].onRandomDialogue = true;

                ForceToPatrol(SceneController.instance.npcs[1]);
                ForceToPatrol(SceneController.instance.npcs[2]);

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
                
                SetDialogue(dialogues[0]);
                yield return waitTalking;
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
                SetDialogue(dialogues[6]);
                yield return waitTalking;
                SetDialogue(dialogues[7]);
                yield return waitTalking;
                SetDialogue(dialogues[8]);
                yield return waitTalking;
                SetDialogue(dialogues[9]);
                yield return waitTalking;

                //CheatManager.instance.InputCheat("minigame 0");
                MinigameManager.instance.StartMinigame(0);
                yield return new WaitUntil(()=>MinigameManager.instance.success);
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
                if(location.selectPhase == 0){
                    location.selectPhase = 1;

                    SetDialogue(dialogues[0]);
                    yield return waitTalking;
                    SetDialogue(dialogues[1]);
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
                SceneController.instance.SetLensOrthoSize(5.3f,0.1f);
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

        Debug.Log(location.trigNum + "(" + location.trigComment + ") 실행 처리됨");


    }
    

    public void SetDialogue(Dialogue dialogue){
        if(dialogue==null){
            Debug.Log("대사 없음");
        }
        DialogueManager.instance.SetDialogue(dialogue);
    }
    public void SetSelect(Select select){
        if(select!=null){
            SelectManager.instance.SetSelect(select);

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
            DM("Error : no pos");
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
        yield return new WaitUntil(()=>PlayerManager.instance.onTriggerCol == desCol);
        
        if(!location.stopCheck) PlayerManager.instance.canMove = true;

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


}
