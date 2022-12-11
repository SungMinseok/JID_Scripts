using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using Cinemachine;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    [Header("세팅값")]
    public float defaultZoomInSize = 4f;
    public float defaultZoomOutSize = 5.3f;
    [Header("맵")]
    public PolygonCollider2D[] mapBounds;
    public PolygonCollider2D[] mapZoomBounds;
    public CinemachineVirtualCamera virtualCamera;
    public CinemachineConfiner2D confiner2D;
    [Header("AudioSource")]
    public List<AudioSource> audioSources;
    [Header("NPC")]
    public List<NPCScript> npcs;
    [Header("오브젝트")]
    public Transform[] objects;
    // Start is called before the first frame update
    public Collider2D temp;
    public TranslateText[] translateTexts;
    public GameObject[] textObjs;
    public Transform centerViewPoint;
    public Transform tempObj;

    
    [Header("Set Demo")]
    public Location[] locations;

    Coroutine setDownLensOrthoSizeCoroutine;
    Coroutine setUpLensOrthoSizeCoroutine;
    void Awake()
    {
        instance = this;

    }
    void Start(){   
        /* if(SceneManager.GetActiveScene().name == "Main"){
            SoundManager.instance.PlayBGM("jelly in the dark");
        }
        else  */if(SceneManager.GetActiveScene().name.Substring(0,3) == "Lev"){
            //SoundManager.instance.ChangeBgm("juicy drug");
            SceneController.instance.virtualCamera.Follow = PlayerManager.instance.transform;

            
#if demo
        locations[0].isLocked = true;
#endif

        }

        // Debug.Log("11");     
        // //GameObject[] a = GameObject.FindGameObjectsWithTag("TranslateText");
        // textObjs = GameObject.FindObjectsOfTypeAll("TranslateText");
        // for(int i=0;i<textObjs.Length;i++){
        // Debug.Log("33");     
        //     translateTexts[i] = textObjs[i].GetComponent<TranslateText>();
        // }
        // Debug.Log("22");     

        //GetAudioSources();
    }
    void OnDisable(){
        StopAllCoroutines();
    }

    public void SetConfiner(int mapNum, bool isDirect = false){
        if(isDirect){
            virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 0;
            virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 0;
            confiner2D.m_BoundingShape2D = mapBounds[mapNum];
            Debug.Log("A : "+DBManager.instance.curData.curMapNum);
            SetCurrentMapName(mapNum);
            Debug.Log("B : "+DBManager.instance.curData.curMapNum);
            Invoke("RecoverConfinerDamping",1f);

        }
        else{
            
            virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 2;
            virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 2;
            
            confiner2D.m_BoundingShape2D = mapBounds[mapNum];
            SetCurrentMapName(mapNum);
        }
        //virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 2;
        //virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 2;

        // Debug.Log(num + " : 맵번호");

        SetQuestState(mapNum);
        AcceptQuestByMap(mapNum);



    }
    public void RecoverConfinerDamping(){
        
        virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 2;
        virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 2;
    }
    public void SetSomeConfiner(PolygonCollider2D boundCollider = null, bool isDirect = false){
        if(isDirect){
            virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 0;
            virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 0;

        }
        else{
            
            virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 2;
            virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 2;
        }
        if(boundCollider == null) temp = confiner2D.m_BoundingShape2D;
        confiner2D.m_BoundingShape2D = boundCollider;
        //virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 2;
        //virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 2;
    }
    public void SetCurrentMapName(int mapNum){
        //Debug.Log(DBManager.instance);
        //DBManager.instance.curData.curMapName = CSVReader.instance.GetIndexToString(mapNum, "map");
        DBManager.instance.curData.curMapNum = mapNum;
    }
    public void SetPlayerPosition(){
        PlayerManager.instance.transform.position = new Vector2(DBManager.instance.curData.playerX,DBManager.instance.curData.playerY);
    }
    public void SetPlayerEquipments(){
        if(DBManager.instance.curData.curEquipmentsID!=null){

            PlayerManager.instance.equipments_id = DBManager.instance.curData.curEquipmentsID;
        }
        PlayerManager.instance.ApplyEquipments(0);
        PlayerManager.instance.ApplyEquipments(1);
        PlayerManager.instance.ApplyEquipments(2);

    }
    public void SetFirstLoad(){
        
        SceneController.instance.SetCurrentMapName(0);

        // if(InventoryManager.instance!=null)
        //         InventoryManager.instance.AddItem(DBManager.instance.localData.usedCouponRewardItemID);
        //DBManager.instance.curData.playerX = PlayerManager.instance.transform.position.x;
        //DBManager.instance.curData.playerY = PlayerManager.instance.transform.position.y;
    }
    public void CameraView(Transform target, float speed=2){
        if(target!=null){
            SceneController.instance.virtualCamera.Follow = target;//ObjectController.instance.npcs[0].transform;
        }
        else{
            //DM("Error : no pos");
        }
    }
    
    public void SetLensOrthoSize(float value, float speed = 0.1f){
        if(virtualCamera.m_Lens.OrthographicSize > value){
            if(setUpLensOrthoSizeCoroutine!=null) StopCoroutine(setUpLensOrthoSizeCoroutine);
            setDownLensOrthoSizeCoroutine = StartCoroutine(SetDownLensOrthoSizeCoroutine(value, speed));
        }
        else{
            if(setDownLensOrthoSizeCoroutine!=null) StopCoroutine(setDownLensOrthoSizeCoroutine);
            setUpLensOrthoSizeCoroutine = StartCoroutine(SetUpLensOrthoSizeCoroutine(value, speed));
        }
    }
    IEnumerator SetDownLensOrthoSizeCoroutine(float value, float speed){  // size = 3.5, y= 4 
        while (virtualCamera.m_Lens.OrthographicSize > value)
        {
            virtualCamera.m_Lens.OrthographicSize -= speed;
            yield return null;
        }
    }
    IEnumerator SetUpLensOrthoSizeCoroutine(float value, float speed){  // size = 5.3, y= 6 
        while (virtualCamera.m_Lens.OrthographicSize < value)
        {
            virtualCamera.m_Lens.OrthographicSize += speed;
            yield return null;
        }
    }
    public void SetCameraDefaultZoomIn(){

        SetLensOrthoSize(defaultZoomInSize,0.075f);
    }
    public void SetCameraDefaultZoomOut(){

        SetLensOrthoSize(defaultZoomOutSize,0.1f);
    }
    public void SetCameraNoised(float intensity, float duration){
        SoundManager.instance.PlaySound("shock_effect_"+Random.Range(0,2));
        StartCoroutine(SetCameraNoisedCoroutine(intensity, duration));
    }
    IEnumerator SetCameraNoisedCoroutine(float intensity, float duration){
        virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = intensity;
        yield return new WaitForSeconds(duration);
        virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
    }
    

    /// <summary>
    /// data_quest 내 majorType 2 인 경우,
    /// 맵 이동 시 퀘스트 상태 업데이트
    /// </summary>
    /// <param name="mapNum"> 맵 번호 </param>
    public void SetQuestState(int mapNum){

        var tempQuestIDList = //이동한 맵으로 이동시 달성 가능한 QuestInfo의 리스트에서 questID만 뽑은 리스트
        DBManager.instance.cache_questList.FindAll(x=> x.majorType == 2 && x.objectives0.Contains(mapNum)).Select(x=>x.ID).ToList();

        //Debug.Log("tempQuestIDList.Count : " + tempQuestIDList.Count);

        foreach(QuestState questState in DBManager.instance.curData.questStateList){//현재 QuestStateList에서
            
            if(tempQuestIDList.Contains(questState.questID)){//위에서 뽑은 아이디중에 해당되는게 있다면

                if(questState.isCompleted) break;//예외) 완료상태면 그냥 패스함

                var tempQuestInfo = //뭐가필요할지 몰라서 일단 정보 다가져옴
                DBManager.instance.cache_questList.Find(x=>x.ID==questState.questID);
        
                if(tempQuestInfo.targetVal == 1){
                    UIManager.instance.CompleteQuest(questState.questID);
                }
                else if(tempQuestInfo.targetVal > 1){

                    if(questState.progressList.Contains(mapNum)) break;

                    questState.progress ++;
                    questState.progressList.Add(mapNum);

                    var slotIndex = UIManager.instance.curQuestIdList.IndexOf(questState.questID);
                    UIManager.instance.SetQuestSlotGrid(slotIndex);//그 슬롯만 진행도 업데이트 (n/N)
                    
                    if(questState.progress >= tempQuestInfo.targetVal){
                        UIManager.instance.CompleteQuest(questState.questID);
                    }
                }

            }
        }
    }

    public void AcceptQuestByMap(int mapNum){
        var mapOverList = DBManager.instance.curData.mapOverList;
        if(mapOverList == null || mapOverList.Count == 0) return;

        switch(mapNum){
            case 4 :
                if(mapOverList[mapOverList.Count-1] == 5 
                && !DBManager.instance.CheckMapOver(6)
                && !DBManager.instance.CheckTrigOver(11)
                ){
                    UIManager.instance.AcceptQuest(4);
                }
                break;

            case 8 :
                if(mapOverList[mapOverList.Count-1] == 8){
                    UIManager.instance.AcceptQuest(6);
                }


                break;

        }
    }
    public void GetAudioSources(){
        audioSources = new List<AudioSource>();
        audioSources.Clear();
        var temp = FindObjectsOfType<AudioSource>();
        foreach(AudioSource a in temp){
            if(a == SoundManager.instance.sfxPlayer || a == SoundManager.instance.bgmPlayer) continue;
            audioSources.Add(a);
        }

    }
}
