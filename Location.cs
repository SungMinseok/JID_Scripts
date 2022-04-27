using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public enum LocationType{
    Teleport,   //캐릭터 순간이동
    Order,   //캐릭터 걸어서 이동
    Dialogue,
    Trigger,
    Patrol_NPC,
    Comment,
    PlaySound,
}
public enum TargetType{
    Player,
    NPC,
}
[System.Serializable]
public class Location : MonoBehaviour
{
    BoxCollider2D boxCollider2D;
    
    WaitForSeconds wait1000ms = new WaitForSeconds(1);
    public LocationType type;
    public bool preserveTrigger;
    //[Header("Teleport")]
    [Header("Teleport & Order")]
    public int doorNum;
    public TargetType targetType;
    public Transform desLoc;
    public int desMapNum;
    public string orderDirection;
    public bool flipCheck;
    public bool isLocked;
    public bool isDoor;//입/출구가 같이 있을 경우
    public string doorSoundFileName;
    public bool useTrigger;//트리거 사용 시 체크
    public Location teleportTriggerlocation;
    [Header("Dialogue")]
    //public int dialogueNum;
    public bool stopCheck;
    public string direction;
    //public Dialogue[] dialogues;
    [Header("Trigger")]
    public int trigNum;
    public string trigComment;
    public Transform target;
    public bool activateTargetMark;//True: 플레이어가 근처에 있을 경우 + 해당 트리거가 완료되지 않았을 경우 = 느낌표 표시 출력
    public Transform targetMark;
    public Transform[] poses;
    public Dialogue[] dialogues_T;
    public Select[] selects_T;
    public bool waitKey;
    public int[] completedTriggerNums;
    public int[] incompletedTriggerNums;
    public int[] haveItemNums;
    public bool keepGo; //선행 트리거 실행된 것 확인되면 진행
    public int selectPhase; //선택지 갯수 만큼 단계 설정 , 선택지 통과 시 해당 선택지 체크 포인트 설정용 . (2개 선택지 있을 경우, 선택지 정답 시 +1, 오답 후 다시 대화 시, 해당 phase 부터시작)
    public bool notZoom; //카메라 줌 X
    public bool holdPlayer; //True 시, 트리거 종료 후 이동 제한 해제 X
    [Header("Patrol_NPC")]
    public Transform desLoc_Patrol_NPC;
    public bool patrolFlag;
    public float patrolWaitTime;
    [Header("Comment")]
    //Just 주석 (에디터 표시용)
    public string commentStr;

    [Header("PlaySound")]
    public string soundFileName;
    public int soundRandomCount;
    public float soundDelay;

    [HideInInspector] public bool locFlag;
    TriggerScript triggerScript;
    void Start(){
        triggerScript = TriggerScript.instance;
        boxCollider2D = GetComponent<BoxCollider2D>();

        if(CSVReader.instance!=null){
            //Debug.Log(TextLoader.instance.transform.parent.name);
            //LoadText();
        }

        if(type==LocationType.Trigger || type==LocationType.Dialogue){
            if(target!=null){
                this.transform.SetParent(target);
                this.transform.localPosition = Vector3.zero;

                if(target.GetComponent<NPCScript>() != null && target.GetComponent<NPCScript>().interactiveMark !=null){
                    if(!DBManager.instance.CheckTrigOver(trigNum) /*&& DBManager.instance.CheckCompletedTrigs(trigNum,completedTriggerNums)*/){
                        target.GetComponent<NPCScript>().interactiveMark.gameObject.SetActive(true);
                    }
                    // else{
                    //     target.GetComponent<NPCScript>().interactiveMark.gameObject.SetActive(false);
                    // }
                }
            }

            if(activateTargetMark){
                targetMark=this.transform.GetChild(0).GetChild(0);
                //targetMark.gameObject.SetActive(false);
            }

            triggerScript.PreAction(this);
        }

        
        if(type!=LocationType.Trigger && type!=LocationType.Teleport && type!=LocationType.Dialogue && type!=LocationType.PlaySound){
            waitKey = false;
        }
    }

    //키 입력시 발동
    void OnTriggerStay2D(Collider2D other) {
        if(type != LocationType.Comment){

            if(other.CompareTag("Player")){
//                    Debug.Log(locFlag);
                if(waitKey&&!locFlag&&!PlayerManager.instance.isWaitingInteract&&!PlayerManager.instance.isActing){
                    //Debug.Log("AAA");
                    //if(Input.GetButton("Interact_OnlyKey")){
                    if(PlayerManager.instance.interactInput){
                        locFlag = true;
                        if(type == LocationType.Trigger)
                            //Debug.Log(trigNum +"번 트리거 (" + trigComment + ") 실행 시도");
                            Debug.Log(string.Format("TrigNum.{0} - [{1}] 실행 시도",trigNum,trigComment));
                        //PlayerManager.instance.ActivateWaitInteract(1);
                        LocationScript(other);
                    }
                }
            }
        }

    }
    //키 안입력해도 발동
    void OnTriggerEnter2D(Collider2D other) {
        if(type != LocationType.Comment){
            if(other.CompareTag("Player")){
                if(!waitKey && !locFlag&&!PlayerManager.instance.isActing ){

                    if(PlayerManager.instance.ladderDelay && PlayerManager.instance.jumpDelay){
                        return;
                    }


                    locFlag = true;
                    if(type == LocationType.Trigger)
                        Debug.Log(string.Format("TrigNum.{0} - [{1}] 실행 시도",trigNum,trigComment));
                    //PlayerManager.instance.ActivateWaitInteract(1);
                    LocationScript(other);
                    //Debug.Log(gameObject.name +" : " + type +"트리거 실행 시도");
                    
                }
            }        
            else if(other.CompareTag("NPC")){
                if(type == LocationType.Order && targetType == TargetType.NPC){
                    LocationScript(other);

                }

            }
        }

    }
    //키 안입력해도 발동
    void OnTriggerExit2D(Collider2D other) {
        if(type != LocationType.Comment){
            if(other.CompareTag("Player")){
                if(!waitKey && locFlag){
                    locFlag = false;
                }
            }       
        }

    }
    public void LocationScript(Collider2D other){
        
                    //Debug.Log("3");
            switch(type){
                case LocationType.Teleport :
                    if(!isLocked){

                        if(other.CompareTag("Player")){
                            if(desLoc!=null && !PlayerManager.instance.transferDelay){

                                PlayerManager.instance.transform.position = desLoc.position;
                                SceneController.instance.SetConfiner(desMapNum,true);
                                SoundManager.instance.SetBgmByMapNum(desMapNum);

                                if(isDoor){
                                    if(doorSoundFileName == ""){
                                        SoundManager.instance.PlaySound(SoundManager.instance.defaultDoorSoundName);

                                    }
                                    else{
                                        SoundManager.instance.PlaySound(doorSoundFileName);

                                    }
                                    PlayerManager.instance.transferDelay = true;
                                    Invoke("TeleportDelay",1f);

                                }
                                
                            }
                            else{

                                DM("목적지 없음");
                            }
                        }

                        if(preserveTrigger) locFlag = false;
                    }
                    else{

                        // if(useTrigger){
                        //     triggerScript.Action(teleportTriggerlocation);
                        // }
                        // else{

                        //     locFlag = false;
                        // }

                            locFlag = false;



                    }
                    

                    break;

                case LocationType.Order :   
                    if(!isLocked){

                        if(targetType == TargetType.Player){
                            if(other.CompareTag("Player")){

//                                Debug.Log("1");
                                if(desLoc!=null){
                                    StartCoroutine(TriggerScript.instance.OrderCoroutine(this,PlayerManager.instance.transform,desLoc));
                                }
                                else{
                                    DM("목적지 없음");
                                }
                            }
                        }
//                         else 
//                     if(other.CompareTag("NPC")){
//                         if(desLoc_Patrol_NPC!=null){
//                             if(!other.GetComponent<NPCScript>().patrolFlag){
// //                                DM("gogo");
// //                                Debug.Log("1");
//                                 StartCoroutine(TriggerScript.instance.NPCPatrolCoroutineToStart(other.GetComponent<NPCScript>()));
//                             }   
//                         }
//                         else{
//                             DM("목적지 없음");
//                         }
//                     }
                        else if(targetType == TargetType.NPC){
//                         Debug.Log("2");
                            if(other.CompareTag("NPC")){
//                                Debug.Log("3");
                                if(desLoc!=null){
                                    StartCoroutine(TriggerScript.instance.OrderCoroutine_NPC(this,other.transform.GetComponent<NPCScript>(),desLoc));
                                }
                                else{
                                    other.GetComponent<NPCScript>().wSet = orderDirection == "R" ? 1 : -1;
                                    //DM("목적지 없음");
                                }
                            }

                        }
                    }
                    else{
                        locFlag = false;
                    }
                    
                    
                    break;
                case LocationType.Dialogue :

                    if(!DBManager.instance.CheckTrigOver(trigNum))
                    {
                        //선행 트리거 실행 여부 확인
                        if(completedTriggerNums.Length>0){
                            if(!DBManager.instance.CheckCompletedTrigs(trigNum,completedTriggerNums)){
                                StartCoroutine(ResetFlagDelayCoroutine());
                                return;
                            }
                        }
                        if (other.CompareTag("Player"))
                        {
                            if (dialogues_T != null)
                            {
                                if (!PlayerManager.instance.isActing)
                                {
                                    PlayerManager.instance.isActing = true;
                                    if (direction != "")
                                    {
                                        PlayerManager.instance.Look(direction);
                                    }
                                    if(activateTargetMark) targetMark.GetComponent<Animator>().SetBool("activate",false);
                                    Debug.Log(trigNum +"번 트리거 실행 성공");
                                    //SetTalk();
                                    triggerScript.Action(this);
                                }

                            }
                            else
                            {
                                DebugManager.instance.PrintDebug("대화 설정 안됨");
                            }
                        }

                        if (!preserveTrigger)
                        {
                            DBManager.instance.TrigOver(trigNum);
                        }
                    }
                    
                    //트리거 이미 실행됨.
                    else{
                        if(poses!=null){
                            
                            triggerScript.TrigIsDone(this);
                        }
                    }
                
                    break;
                case LocationType.Trigger :
                    if(!DBManager.instance.CheckTrigOver(trigNum)){

                        //선행 트리거 실행 여부 확인
                        if(completedTriggerNums.Length>0){
                            if(!DBManager.instance.CheckCompletedTrigs(trigNum,completedTriggerNums)){
                                StartCoroutine(ResetFlagDelayCoroutine());
                                return;
                            }
                        }
                        //선행 트리거 미실행 여부 확인
                        if(incompletedTriggerNums.Length>0){
                            if(!DBManager.instance.CheckIncompletedTrigs(trigNum,incompletedTriggerNums)){
                                StartCoroutine(ResetFlagDelayCoroutine());
                                return;
                            }
                        }
                        //아이템 보유 여부 확인
                        if(haveItemNums.Length>0){
                            if(!DBManager.instance.CheckHaveItems(trigNum,haveItemNums)){
                                StartCoroutine(ResetFlagDelayCoroutine());
                                return;
                            }
                            // for(int i=0;i<haveItemNums.Length;i++){
                            //     if(!InventoryManager.instance.CheckHaveItem(haveItemNums[i])){
                            //         DM(trigNum +"번 트리거 실행 실패 : " + haveItemNums[i] + "번 아이템 미보유.");
                            //         StartCoroutine(ResetFlagDelayCoroutine());
                            //         return;
                            //     }
                            // }
                        }

                        if(other.CompareTag("Player")){
                            if(trigNum>=0){
                                if(!PlayerManager.instance.isActing){
                                    PlayerManager.instance.isActing = true;
                                    //triggerScript.Action(this);
                                    if(activateTargetMark) targetMark.GetComponent<Animator>().SetBool("activate",false);
                                    Debug.Log(trigNum +"번 트리거 실행 성공");

                                    triggerScript.Action(this);

                                }
                                    
                            }
                            else{
                                DebugManager.instance.PrintDebug("트리거 설정 안됨");
                            }

                        }

                    }

                    //트리거 이미 실행됨.
                    else{
                        if(poses!=null){
                            
                            triggerScript.TrigIsDone(this);
                        }
                    }
                
                    break;

                case LocationType.Patrol_NPC :

                    if(other.CompareTag("NPC")){
                        if(desLoc_Patrol_NPC!=null){
                            if(!other.GetComponent<NPCScript>().patrolFlag){
//                                DM("gogo");
//                                Debug.Log("1");
                                StartCoroutine(TriggerScript.instance.NPCPatrolCoroutineToStart(other.GetComponent<NPCScript>()));
                            }   
                        }
                        else{
                            DM("목적지 없음");
                        }
                    }
                    break;
                    
                case LocationType.PlaySound :
                    Invoke("SoundDelay",soundDelay);
                    if(soundRandomCount == 0){
                        SoundManager.instance.PlaySound(soundFileName);
                    }
                    else{
                        SoundManager.instance.PlaySound(soundFileName + Random.Range(0,soundRandomCount));
                    }
                    break;
                default :
                    DebugManager.instance.PrintDebug("로케이션 오류");
                    break;
            

        }
        
    }

    // public void SetTalk(){
    //     DialogueManager.instance.SetFullDialogue(dialogues_T);
    // }
    public void DM(string msg) => DebugManager.instance.PrintDebug(msg);
    

    public void LoadText(){
        
        // if(dialogues!=null){
        //     for(int i=0; i<dialogues.Length;i++){
        //         for(int j=0; j<dialogues[i].sentences.Length;j++){
        //             int temp = int.Parse(dialogues[i].sentences[j]);
        //             //dialogues[i].sentences[j] = TextLoader.instance.ApplyText(temp);
        //             dialogues[i].sentences[j] = CSVReader.instance.GetIndexToString(temp,"dialogue");
        //         }
        //     }
        // }
        if(dialogues_T!=null){
            for(int i=0; i<dialogues_T.Length;i++){
                for(int j=0; j<dialogues_T[i].sentences.Length;j++){
                    int temp = int.Parse(dialogues_T[i].sentences[j]);
                    //dialogues_T[i].sentences[j] = TextLoader.instance.ApplyText(temp);
                    dialogues_T[i].sentences[j] = CSVReader.instance.GetIndexToString(temp,"dialogue");
                }
            }
        }
        if(selects_T!=null){
            for(int i=0; i<selects_T.Length;i++){
                for(int j=0; j<selects_T[i].answers.Length;j++){
                    int temp = int.Parse(selects_T[i].answers[j]);
                    //selects_T[i].answers[j] = TextLoader.instance.ApplyText(temp);
                    selects_T[i].answers[j] = CSVReader.instance.GetIndexToString(temp,"select");
                }
            }
        }

    }
    IEnumerator ResetFlagDelayCoroutine(){
        yield return wait1000ms;
        locFlag= false;
    }
    void SoundDelay(){
        locFlag = false;
        Debug.Log("33");
    }
    void TeleportDelay(){
        PlayerManager.instance.transferDelay = false;
    }




























#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle();
        Vector3 namePos = Vector3.zero;
        switch(type){
            
            
            case LocationType.Teleport :
                //Gizmos.color = new Color(Color.red.r,Color.red.g,Color.red.b,0.3f); 

                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(transform.position, transform.localScale);
                if(desLoc!=null){
                    Gizmos.color = Color.blue;   
                    Gizmos.DrawWireCube(desLoc.transform.position, Vector3.one);
                    Gizmos.color = Color.black;   
                    Gizmos.DrawLine(transform.position,desLoc.transform.position);
                }
                
                style.fontSize = 20;
                style.fontStyle = FontStyle.Bold;
                style.normal.textColor = Color.red;
                namePos = transform.position;
                namePos.x -= 0.5f;
                namePos.y += 0.7f;
                Handles.Label(namePos, doorNum.ToString(),style);
                break;
            case LocationType.Order :
                //Gizmos.color = new Color(Color.red.r,Color.red.g,Color.red.b,0.3f);
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(transform.position, transform.localScale);
                if(desLoc!=null){
                    Gizmos.color = Color.blue;   
                    Gizmos.DrawWireCube(desLoc.transform.position, Vector3.one);
                    Gizmos.color = Color.white;   
                    Gizmos.DrawLine(transform.position,desLoc.transform.position);
                }
                break;
            case LocationType.Dialogue :
                Gizmos.color = Color.cyan;   
                Gizmos.DrawWireCube(transform.position,  transform.localScale);
                style.fontSize = 20;
                style.fontStyle = FontStyle.Bold;
                style.normal.textColor = Color.yellow;
                namePos = transform.position;
                namePos.x -= 0.5f;
                namePos.y += 0.7f;
                Handles.Label(namePos, trigNum.ToString(),style);
                break;
            case LocationType.Trigger :
                Gizmos.color = Color.cyan;   
                Gizmos.DrawWireCube(transform.position, transform.GetComponent<BoxCollider2D>().size);

                //GUIStyle style = new GUIStyle();
                style.fontSize = 20;
                style.fontStyle = FontStyle.Bold;
                style.normal.textColor = Color.cyan;
                namePos = transform.position;
                namePos.x -= 0.5f;
                namePos.y += 0.7f;
                Handles.Label(namePos, trigNum.ToString(),style);
                break;
            case LocationType.Patrol_NPC :
                Gizmos.color = Color.black;   
                Gizmos.DrawWireCube(transform.position,  transform.localScale);                
                if(desLoc_Patrol_NPC!=null){
                    Gizmos.color = Color.white;   
                    Gizmos.DrawWireCube(desLoc_Patrol_NPC.transform.position, Vector3.one);
                    Gizmos.color = Color.black;   
                    Gizmos.DrawLine(transform.position,desLoc_Patrol_NPC.transform.position);
                }
                
                break;
            case LocationType.Comment :
                Gizmos.color = Color.cyan;   
                //Gizmos.DrawWireCube(transform.position, transform.localScale);

                //GUIStyle style = new GUIStyle();
                style.fontSize = 20;
                style.fontStyle = FontStyle.Bold;
                style.normal.textColor = Color.white;
                namePos = transform.position;
                namePos.x -= 0.5f;
                namePos.y += 0.7f;
                Handles.Label(namePos, commentStr.ToString(),style);
                break;
    //Handles.Label(transform.position, "Text");
 
        }


    }
[CustomEditor(typeof(Location)), CanEditMultipleObjects]
public class LocationEditor : Editor
{
    // [MenuItem("Window/Location")]
    // public static void ShowWindow(){
    //     EditorWindow.GetWindow(typeof(LocationEditor));
    // }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //DrawDefaultInspector();
        Location selected = (Location)target;
        //EditorGUILayout.BeginHorizontal();
        selected.type = (LocationType)EditorGUILayout.EnumPopup("로케이션 타입 설정", selected.type);
        //selected.type = EditorGUILayout.Foldout(selected.type == LocationType.Teleport,"type");
        EditorGUILayout.Space();
        

        if (selected.type == LocationType.Teleport)
        {
            //EditorGUILayout.LabelField("문 번호", EditorStyles.boldLabel);
            selected.doorNum = EditorGUILayout.IntField("문 번호",selected.doorNum,EditorStyles.toolbarTextField);
            EditorGUILayout.Space();
            selected.desLoc = EditorGUILayout.ObjectField("도착지", selected.desLoc, typeof(Transform), true) as Transform;
            EditorGUILayout.Space();
            selected.desMapNum = EditorGUILayout.IntField("도착 맵 번호",selected.desMapNum,EditorStyles.toolbarTextField);
            EditorGUILayout.Space();
            selected.useTrigger = EditorGUILayout.ToggleLeft("트리거 사용", selected.useTrigger);
            selected.teleportTriggerlocation =  EditorGUILayout.ObjectField("도착지", selected.teleportTriggerlocation, typeof(Location), true) as Location;
            EditorGUILayout.Space();
            selected.waitKey = EditorGUILayout.ToggleLeft("상호작용 시 발동", selected.waitKey);
            EditorGUILayout.Space();
            selected.isDoor = EditorGUILayout.ToggleLeft("출입구가 같음", selected.isDoor);
            selected.doorSoundFileName = EditorGUILayout.TextField("출입구 소리", selected.doorSoundFileName);
            EditorGUILayout.Space();
            selected.isLocked = EditorGUILayout.ToggleLeft("비활성화(잠금)", selected.isLocked);
            EditorGUILayout.Space();
            selected.preserveTrigger = EditorGUILayout.ToggleLeft("반복 사용", selected.preserveTrigger);
        }
        else if (selected.type == LocationType.Order)
        {
            EditorGUILayout.LabelField("대상 선택");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("targetType"),GUIContent.none, true);
            selected.desLoc = EditorGUILayout.ObjectField("도착지", selected.desLoc, typeof(Transform), true) as Transform;
            selected.stopCheck = EditorGUILayout.ToggleLeft("이동 후 정지", selected.stopCheck);
            EditorGUILayout.Space();
            selected.direction = EditorGUILayout.TextField("L/R 방향이동", selected.direction);
            EditorGUILayout.Space();
            selected.flipCheck = EditorGUILayout.ToggleLeft("스프라이트 좌우 반전", selected.flipCheck);
            EditorGUILayout.Space();
            selected.isLocked = EditorGUILayout.ToggleLeft("비활성화(잠금)", selected.isLocked);
            EditorGUILayout.Space();
            selected.preserveTrigger = EditorGUILayout.ToggleLeft("반복 사용", selected.preserveTrigger);
        }
        else if (selected.type == LocationType.Dialogue)
        {
            //selected.dialogueNum = EditorGUILayout.IntField("대화 번호", selected.dialogueNum,EditorStyles.toolbarTextField);
            selected.trigNum = EditorGUILayout.IntField("트리거 번호", selected.trigNum,EditorStyles.toolbarTextField);
            selected.trigComment =  EditorGUILayout.TextField("주석",selected.trigComment);
            EditorGUILayout.LabelField("선행 트리거 번호");
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("대화");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dialogues_T"),GUIContent.none, true);

            
            EditorGUILayout.Space();
            selected.waitKey = EditorGUILayout.ToggleLeft("상호작용 시 발동", selected.waitKey);
            selected.activateTargetMark = EditorGUILayout.ToggleLeft("느낌표 표시", selected.activateTargetMark);
            selected.notZoom = EditorGUILayout.ToggleLeft("카메라 줌 사용 안함", selected.notZoom);
            selected.stopCheck = EditorGUILayout.ToggleLeft("대화 중 이동 불가", selected.stopCheck);
            selected.direction = EditorGUILayout.TextField("대화 시 방향 설정", selected.direction);
            EditorGUILayout.Space();
            selected.preserveTrigger = EditorGUILayout.ToggleLeft("반복 사용", selected.preserveTrigger);

        }
        else if (selected.type == LocationType.Trigger)
        {
            selected.trigNum = EditorGUILayout.IntField("트리거 번호", selected.trigNum,EditorStyles.toolbarTextField);
            selected.trigComment =  EditorGUILayout.TextField("주석",selected.trigComment);
            selected.target = EditorGUILayout.ObjectField("트리거 부착", selected.target, typeof(Transform), true) as Transform;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("[Conditions]",EditorStyles.boldLabel);
            EditorGUILayout.LabelField("선행 트리거 번호");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("completedTriggerNums"),GUIContent.none, true);
            EditorGUILayout.LabelField("미선행 트리거 번호");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("incompletedTriggerNums"),GUIContent.none, true);
            EditorGUILayout.LabelField("필요 아이템 번호");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("haveItemNums"),GUIContent.none, true);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("[Actions]",EditorStyles.boldLabel);
            EditorGUILayout.LabelField("오브젝트");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("poses"),GUIContent.none, true);
            EditorGUILayout.LabelField("대화");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dialogues_T"),GUIContent.none, true);
            EditorGUILayout.LabelField("선택");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("selects_T"),GUIContent.none, true);
            selected.waitKey = EditorGUILayout.ToggleLeft("상호작용 시 발동", selected.waitKey);
            selected.activateTargetMark = EditorGUILayout.ToggleLeft("느낌표 표시", selected.activateTargetMark);
            selected.notZoom = EditorGUILayout.ToggleLeft("카메라 줌 사용 안함", selected.notZoom);
            selected.holdPlayer = EditorGUILayout.ToggleLeft("종료 후 이동불가", selected.holdPlayer);
            EditorGUILayout.Space();
            selected.preserveTrigger = EditorGUILayout.ToggleLeft("반복 사용(선택지 있으면 해제)", selected.preserveTrigger);

        }
        else if (selected.type == LocationType.Patrol_NPC)
        {
            EditorGUILayout.LabelField("이 로케이션 위에 NPC가 놓여야 함.");
            selected.desLoc_Patrol_NPC = EditorGUILayout.ObjectField("도착지", selected.desLoc_Patrol_NPC, typeof(Transform), true) as Transform;
            selected.patrolWaitTime = EditorGUILayout.FloatField("이동 대기시간", selected.patrolWaitTime);
        }
        
        else if (selected.type == LocationType.Comment)
        {
            selected.commentStr =  EditorGUILayout.TextField("주석",selected.commentStr);
        }
        else if (selected.type == LocationType.PlaySound)
        {
            selected.soundFileName =  EditorGUILayout.TextField("사운드명",selected.soundFileName);
            selected.soundRandomCount = EditorGUILayout.IntField("사운드 랜덤 개수", selected.soundRandomCount);
            selected.soundDelay = EditorGUILayout.FloatField("사운드 딜레이", selected.soundDelay);
            selected.waitKey = EditorGUILayout.ToggleLeft("상호작용 시 발동", selected.waitKey);
        }
        //EditorGUILayout.EndHorizontal();



        serializedObject.ApplyModifiedProperties();
    }    
    
}


#endif

}
