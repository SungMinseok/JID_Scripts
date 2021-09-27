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
}
public enum TargetType{
    Player,
    NPC,
}
[System.Serializable]
public class Location : MonoBehaviour
{
    BoxCollider2D boxCollider2D;
    
    public LocationType type;
    public bool preserveTrigger;
    //[Header("Teleport")]
    [Header("Teleport & Order")]
    public int doorNum;
    public TargetType targetType;
    public Transform desLoc;
    public string orderDirection;
    public bool flipCheck;
    public bool isLocked;
    [Header("Dialogue")]
    //public int dialogueNum;
    public bool stopCheck;
    public string direction;
    public Dialogue[] dialogues;
    [Header("Trigger")]
    public int trigNum;
    public string trigComment;
    public Transform target;
    public bool activateTargetMark;//True: 플레이어가 근처에 있을 경우 + 해당 트리거가 완료되지 않았을 경우 = 느낌표 표시 출력
    public Transform[] poses;
    public Dialogue[] dialogues_T;
    public Select[] selects_T;
    public bool waitKey;
    public int[] completedTriggerNums;
    public bool keepGo; //선행 트리거 실행된 것 확인되면 진행
    public int selectPhase; //선택지 갯수 만큼 단계 설정 , 선택지 통과 시 해당 선택지 체크 포인트 설정용 . (2개 선택지 있을 경우, 선택지 정답 시 +1, 오답 후 다시 대화 시, 해당 phase 부터시작)
    [Header("Patrol_NPC")]
     public Transform desLoc_Patrol_NPC;
    public bool patrolFlag;
    public float patrolWaitTime;
    [HideInInspector] public bool locFlag;
    TriggerScript triggerScript;
    void Start(){
        triggerScript = TriggerScript.instance;
        boxCollider2D = GetComponent<BoxCollider2D>();

        if(CSVReader.instance!=null){
            //Debug.Log(TextLoader.instance.transform.parent.name);
            LoadText();
        }

        if(type==LocationType.Trigger){
            if(target!=null){
                this.transform.SetParent(target);
                this.transform.localPosition = Vector3.zero;

                if(target.GetComponent<NPCScript>() != null && target.GetComponent<NPCScript>().interactiveMark !=null && !DBManager.instance.CheckTrigOver(trigNum)){
                    target.GetComponent<NPCScript>().interactiveMark.gameObject.SetActive(true);
                }
            }
        }

        
        if(type!=LocationType.Trigger){
            waitKey = false;
        }
    }
    void OnTriggerStay2D(Collider2D other) {
        if(other.CompareTag("Player")){
            if(waitKey&&!locFlag&&!PlayerManager.instance.isWaitingInteract&&!PlayerManager.instance.isActing){
                if(Input.GetButton("Interact_OnlyKey")){
                    locFlag = true;
                    Debug.Log(trigNum +"번 트리거 실행 시도");
                    //PlayerManager.instance.ActivateWaitInteract(1);
                    LocationScript(other);
                }
            }
        }
    }
    void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            if(!waitKey && !locFlag&&!PlayerManager.instance.isActing){
                locFlag = true;
                Debug.Log(trigNum +"번 트리거 실행 시도");
                //PlayerManager.instance.ActivateWaitInteract(1);
                LocationScript(other);
                //Debug.Log(gameObject.name +" : " + type +"트리거 실행 시도");
                
            }
        }
    }
    void LocationScript(Collider2D other){
        
                    //Debug.Log("3");
            switch(type){
                case LocationType.Teleport :
                    if(!isLocked){

                        if(other.CompareTag("Player")){
                            if(desLoc!=null){
                                PlayerManager.instance.transform.position = desLoc.position;
                                PlayerManager.instance.hInput = 1;
                                PlayerManager.instance.hInput = 0;
    //                            Debug.Log(desLoc.parent.transform.GetSiblingIndex());
                                //SceneController.instance.SetConfiner(desLoc.parent.transform.parent.transform.GetSiblingIndex());
                                SceneController.instance.SetSomeConfiner(desLoc.parent.parent.GetChild(0).GetComponent<Collider2D>());
                            }
                            else{

                                DM("목적지 없음");
                            }
                        }

                        if(preserveTrigger) locFlag = false;
                    }
                    else{
                        locFlag = false;
                    }
                    

                    break;

                case LocationType.Order :   
                    if(!isLocked){

                        if(targetType == TargetType.Player){
                            if(other.CompareTag("Player")){

                                //Debug.Log("1");
                                if(desLoc!=null){
                                    StartCoroutine(TriggerScript.instance.OrderCoroutine(this,PlayerManager.instance.transform,desLoc));
                                }
                                else{
                                    DM("목적지 없음");
                                }
                            }
                        }
                        else if(targetType == TargetType.NPC){
                        // Debug.Log("2");
                            if(other.CompareTag("NPC")){
                                Debug.Log("2");
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
                        if (other.CompareTag("Player"))
                        {
                            if (dialogues != null)
                            {
                                if (!PlayerManager.instance.isTalking)
                                {
                                    PlayerManager.instance.isTalking = true;
                                    if (direction != "")
                                    {
                                        PlayerManager.instance.Look(direction);
                                    }
                                    SetTalk();
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
                    break;
                case LocationType.Trigger :
                    if(!DBManager.instance.CheckTrigOver(trigNum)){

                        //선행 트리거 실행 여부 확인
                        if(completedTriggerNums.Length>0){
                            for(int i=0;i<completedTriggerNums.Length;i++){

                                // for(int j=0;j<DBManager.instance.data.trigOverList.Count;j++){
                                //     Debug.Log(completedTriggerNums[i] + "번 트리거 실행 여부 확인");
                                    
                                // }
                                if(!DBManager.instance.CheckTrigOver(completedTriggerNums[i])){
                                    Debug.Log(trigNum +"번 트리거 실행 실패 : " + completedTriggerNums[i] + "번 트리거 완료되지 않음");
                                    locFlag = false;
                                    return;
                                }
                            }
                            //Debug.Log(trigNum +"번 트리거 실행 실패 : 선행 트리거 실행 안됨");
                            //locFlag = false;
                            //return;
                        }

                        Debug.Log(trigNum +"번 트리거 실행 성공");

                        if(other.CompareTag("Player")){
                            if(trigNum>=0){
                                if(!PlayerManager.instance.isActing){
                                    PlayerManager.instance.isActing = true;
                                    triggerScript.Action(this);
                                    // if(dialogues_T!=null){
                                    //     if(poses!=null){
                                        
                                    //         triggerScript.Action(trigNum, dialogues_T, poses);
                                    //     }
                                    //     else{
                                    //         triggerScript.Action(trigNum, dialogues_T);

                                    //     }
                                    // }
                                    // else{
                                    //     triggerScript.Action(trigNum);
                                    // }
                                }
                                    
                            }
                            else{
                                DebugManager.instance.PrintDebug("트리거 설정 안됨");
                            }

                                            
                            // if(!preserveTrigger){
                            //     DBManager.instance.TrigOver(trigNum);
                            // }
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
                default :
                    DebugManager.instance.PrintDebug("로케이션 오류");
                    break;
            

        }
        
    }

    public void SetTalk(){
        DialogueManager.instance.SetFullDialogue(dialogues);
    }
    public void DM(string msg) => DebugManager.instance.PrintDebug(msg);
    

    public void LoadText(){
        
        if(dialogues!=null){
            for(int i=0; i<dialogues.Length;i++){
                for(int j=0; j<dialogues[i].sentences.Length;j++){
                    int temp = int.Parse(dialogues[i].sentences[j]);
                    //dialogues[i].sentences[j] = TextLoader.instance.ApplyText(temp);
                    dialogues[i].sentences[j] = CSVReader.instance.GetIndexToString(temp,"dialogue");
                }
            }
        }
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
                Gizmos.DrawWireCube(transform.position, transform.localScale);

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
            EditorGUILayout.Space();
            //EditorGUILayout.LabelField("대화 시 방향 설정");
            
            //EditorGUIUtility.labelWidth = 0;
           //EditorGUILayout.LabelField("대화 시 방향 설정" );
            //GUILayout.Label( "대화 시 방향 설정"  );
            //GUILayout.FlexibleSpace();
            //EditorGUILayout.PropertyField(serializedObject.FindProperty("direction"),GUIContent.none, true);
            selected.direction = EditorGUILayout.TextField("대화 시 방향 설정", selected.direction);
            //GUILayout.EndHorizontal();

            EditorGUILayout.LabelField("대화");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dialogues"),GUIContent.none, true);

            
            selected.stopCheck = EditorGUILayout.ToggleLeft("대화 중 이동 불가", selected.stopCheck);
            EditorGUILayout.Space();
            selected.preserveTrigger = EditorGUILayout.ToggleLeft("반복 사용", selected.preserveTrigger);

        }
        else if (selected.type == LocationType.Trigger)
        {
            selected.trigNum = EditorGUILayout.IntField("트리거 번호", selected.trigNum,EditorStyles.toolbarTextField);
            selected.trigComment =  EditorGUILayout.TextField("주석",selected.trigComment);
            EditorGUILayout.LabelField("선행 트리거 번호");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("completedTriggerNums"),GUIContent.none, true);
            selected.target = EditorGUILayout.ObjectField("트리거 부착", selected.target, typeof(Transform), true) as Transform;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("오브젝트");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("poses"),GUIContent.none, true);
            EditorGUILayout.LabelField("대화");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dialogues_T"),GUIContent.none, true);
            EditorGUILayout.LabelField("선택");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("selects_T"),GUIContent.none, true);
            selected.waitKey = EditorGUILayout.ToggleLeft("상호작용 시 발동", selected.waitKey);
            selected.activateTargetMark = EditorGUILayout.ToggleLeft("느낌표 표시", selected.activateTargetMark);
            EditorGUILayout.Space();
            selected.preserveTrigger = EditorGUILayout.ToggleLeft("반복 사용", selected.preserveTrigger);

        }
        else if (selected.type == LocationType.Patrol_NPC)
        {
            EditorGUILayout.LabelField("이 로케이션 위에 NPC가 놓여야 함.");
            selected.desLoc_Patrol_NPC = EditorGUILayout.ObjectField("도착지", selected.desLoc_Patrol_NPC, typeof(Transform), true) as Transform;
            selected.patrolWaitTime = EditorGUILayout.FloatField("이동 대기시간", selected.patrolWaitTime);
        }
        //EditorGUILayout.EndHorizontal();



        serializedObject.ApplyModifiedProperties();
    }    
    
}


#endif

}
