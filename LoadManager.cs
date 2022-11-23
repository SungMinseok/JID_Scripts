using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UB.Simple2dWeatherEffects.Standard;
using TMPro;
#if !DISABLESTEAMWORKS
using Steamworks;
#endif

#region 앱 실행/종료 관련  처리
#endregion
public class LoadManager : MonoBehaviour
{
    public static LoadManager instance;
    public Image loadFader;
    public Slider slider;
    public InputField inputText;
    public D2FogsNoiseTexPE fogsNoiseTexPE;
    public GameObject[] mainBtns;
    public bool loadFlag;
    public bool reloadScene;  //로드 페이드 아웃 직후 ~ 로드 페이드 인 직전 (NPC 코루틴 종료용)
    //public GameObject ddol;
    [Header("UI_Save&Load")]
    public Transform saveSlotGrid;
    public SaveLoadSlot[] saveSlots;
    public Transform loadSlotGrid;
    public SaveLoadSlot[] loadSlots;
    [System.Serializable]
    public class SaveLoadSlot{
        public TextMeshProUGUI saveNameText;
        public TextMeshProUGUI saveDateText;
    }

    [Header("───────Debug───────")]
    //[Tooltip("자동저장 체크용 (True : 마지막 저장지점 불러오기 시 자동저장 파일 불러옴. // false : lastLoadFileNum 파일 불러옴")]
    //public bool isAutoSaved;
    [Tooltip("앱 최초 실행 시 체크 (타이틀 영상 풀 재생 여부 확인용)")]
    public bool checkFirstRun;
    [Tooltip("인게임에서 로드 시 (맵설정/플레이어위치설정)")]
    public bool isLoadingInGame;
    [Tooltip("인게임에서 게임 오버 후 마지막 저장 파일 로드")]
    public bool isLoadingInGameToLastPoint;
    public int lastLoadFileNum; // 최초 시작 : -1, 그 외 : 0 ~

    [Tooltip("흙고갈로 사망 시 흙 일부 채워주기용")]
    public bool isDeadByDepletingDirt;
    public string mainSceneName;

    WaitForSeconds wait1s = new WaitForSeconds(1);
    WaitForSeconds wait500ms = new WaitForSeconds(0.5f);
    WaitForSeconds wait1ms = new WaitForSeconds(0.001f);
    WaitUntil waitPlayer = new WaitUntil(()=>PlayerManager.instance);
    void Awake(){
        
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {
        //Application.targetFrameRate = 60;
        
        //ddol = DDOLScript.instance.gameObject;

        mainSceneName = 
        "Level"
        //+ "_"
        //+ DBManager.instance.buildVersion
        //+ "_"
        //+ DBManager.instance.buildNum
        // + "_" 
        // + DBManager.instance.buildDate
        ;

    }
    /// <summary>
    /// [Main 씬에서 Level 씬으로 진입]
    /// </summary>
    /// 
    public void MainToGame()
    {
        StartCoroutine(MainToGameCoroutine());
    }
    IEnumerator MainToGameCoroutine(){
        lastLoadFileNum = -1;
        FadeOut();
        yield return wait1s;
        VideoManager.instance.StopVideo();
        StartCoroutine(LoadNextScene(mainSceneName));
    }
    public void LoadGame(){
        if(PlayerManager.instance!=null){
            PlayerManager.instance.canMove = false;
        }
//실행중인 코루틴 모두 중지 (다 파괴하기 전에)
        if(SceneManager.GetActiveScene().name == "Main"){

        }
        else{
            TriggerScript.instance.StopAllCoroutines();
            PlayerManager.instance.StopAllCoroutines();

        }

        StartCoroutine(LoadCoroutine(mainSceneName));
    }
    public void LoadMain(){
        
        if(TriggerScript.instance!=null) TriggerScript.instance.StopAllCoroutines();
        if(PlayerManager.instance!=null) PlayerManager.instance.StopAllCoroutines();
        SoundManager.instance.SoundOff();

        StartCoroutine(LoadCoroutine("Main"));


    }
    IEnumerator LoadCoroutine(string sceneName){
        Debug.Log("LoadCoroutine : " + sceneName);

        if(loadFader.color.a != 1){
            FadeOut();
            yield return wait1s;
        }
        StartCoroutine(LoadNextScene(sceneName));
        if(VideoManager.instance.isPlayingVideo){
            VideoManager.instance.StopVideo();
        }

        if(MenuManager.instance.menuPanel.activeSelf){
            MenuManager.instance.menuPanel.SetActive(false);
        }
    }
    
    IEnumerator LoadNextScene(string nextScene)
    {
        reloadScene = true;
        SceneManager.LoadScene("Loading");
        yield return null;

#region [ 인게임(Level) > 파일 선택해서 불러오기, 처 ]

        if(isLoadingInGame){
            if(lastLoadFileNum == -1){
                DBManager.instance.LoadDefaultData();
                Debug.Log("기본 데이터 불러오기 성공");
            }
            else{
                DBManager.instance.CallLoad(lastLoadFileNum);
                Debug.Log(lastLoadFileNum + "번 파일 데이터 불러오기 성공");

            }

            if(DDOLScript.instance!=null) Destroy(DDOLScript.instance.gameObject);
            if(PlayerManager.instance!=null) Destroy(PlayerManager.instance.gameObject);
            
        }

#endregion

#region [ 인게임(Level) > 마지막 저장지점 불러오기 ]
        else if(isLoadingInGameToLastPoint){
            if(lastLoadFileNum == -1){
                //DBManager.instance.curData = DBManager.instance.emptyData.DeepCopy();
                DBManager.instance.LoadDefaultData();
                Debug.Log("기본 데이터 불러오기 성공");

            }
            else{
                DBManager.instance.CallLoad(lastLoadFileNum);
                Debug.Log(lastLoadFileNum + "번 파일 데이터 불러오기 성공");

            }
            if(DDOLScript.instance!=null) Destroy(DDOLScript.instance.gameObject);
            if(PlayerManager.instance!=null) Destroy(PlayerManager.instance.gameObject);
        }
        
#endregion

#region [ 인게임(Level) > 메인씬(Main) ]
        else if(nextScene == "Main"){
                
            if(DDOLScript.instance!=null) Destroy(DDOLScript.instance.gameObject);
            if(PlayerManager.instance!=null) Destroy(PlayerManager.instance.gameObject);
        }
        else{
            DBManager.instance.LoadDefaultData();
            //FadeIn();
            //DBManager.instance.curData = DBManager.instance.emptyData.DeepCopy();
        }

#endregion

#region [ 데이터 로드 완료 후 ]

        AsyncOperation asyncScene = SceneManager.LoadSceneAsync(nextScene);
        //SceneManager.LoadSceneAsync("Menu",LoadSceneMode.Additive);
        asyncScene.allowSceneActivation = false;
        while (!asyncScene.isDone)
        {
            yield return null;
            if (asyncScene.progress >= 0.9f)
            {
                asyncScene.allowSceneActivation = true;
                if(!loadFlag){
                    loadFlag = true;
                    StartCoroutine(LoadSceneFadeIn(nextScene));
                } 
            }
        }
#endregion

    }
    IEnumerator LoadSceneFadeIn(string nextScene){
        yield return null;
        ResetFader(1);

//프롤로그 체크 221110
        var tempData = DBManager.instance.curData;
        
        if(tempData.version_minor == 0 && tempData.version_build <= 45){
            DBManager.instance.curData.passPrologue = true;
            DBManager.instance.localData.canSkipPrologue = true;
            Debug.Log("1.0.45버전 이하 저장파일은 스킵가능");
        }
        else if(DBManager.instance.CheckTrigOver(110)){
            
            DBManager.instance.curData.passPrologue = true;
            DBManager.instance.localData.canSkipPrologue = true;
            Debug.Log("110번 트리거 완료하여 프롤로그 패스처리");
        }


#region [ 인게임 로드 시 ]

        if(isLoadingInGame){
            isLoadingInGame = false;
            
            yield return wait1s;
            if(lastLoadFileNum != -1) SceneController.instance.CameraView(PlayerManager.instance.transform);
            if(lastLoadFileNum != -1) SceneController.instance.SetPlayerPosition();//Load(-1)로 로드 시, 먹창으로 이동하는 것 방지
            SceneController.instance.SetPlayerEquipments();
            InventoryManager.instance.ResetInventory();
            SceneController.instance.SetConfiner(tempData.curMapNum,isDirect:true);
            SoundManager.instance.SoundOff();
            Debug.Log(lastLoadFileNum + "번 파일 로드 완료");

        }
        else if(isLoadingInGameToLastPoint){
            isLoadingInGameToLastPoint = false;
            
            yield return wait1s;
            if(lastLoadFileNum == -1){

                SceneController.instance.CameraView(PlayerManager.instance.transform);
                SceneController.instance.SetPlayerPosition();
                SceneController.instance.SetPlayerEquipments();
                yield return new WaitUntil(()=>InventoryManager.instance);
                InventoryManager.instance.ResetInventory();
                SceneController.instance.SetConfiner(tempData.curMapNum);
                SoundManager.instance.SoundOff();
                Debug.Log("빈 파일 로드 완료");
            }
            else{
                    
                SceneController.instance.CameraView(PlayerManager.instance.transform);
                SceneController.instance.SetPlayerPosition();
                SceneController.instance.SetPlayerEquipments();
                InventoryManager.instance.ResetInventory();
                SceneController.instance.SetConfiner(tempData.curMapNum);
            SoundManager.instance.SoundOff();
                Debug.Log(lastLoadFileNum + "번 파일 로드 완료");
                Debug.Log("맵번호 : " + tempData.curMapNum);
            }

        }
        else{
            yield return wait1s;            

            SceneController.instance.SetFirstLoad();
        }


#endregion

        //yield return wait1s;
            //Debug.Log("A");
        if(!string.Equals(nextScene, "Main")){
        VideoManager.instance.videoRenderer.gameObject.SetActive(false);

        }
        FadeIn();

        loadFlag = false;
        reloadScene = false;

        //흙고갈로 사망시 흙 일정하게 설정
        if(isDeadByDepletingDirt){
            isDeadByDepletingDirt = false;
            DBManager.instance.curData.curDirtAmount = DBManager.instance.minimumDirtAmount;
        }
        //if(nextScene != "Main") SoundManager.instance.SetBgmByMapNum(DBManager.instance.curData.curMapNum);
        
        yield return wait500ms;


        
        if(lastLoadFileNum == -1){
            if(InventoryManager.instance!=null){
                InventoryManager.instance.GiveReward();

            }

        }

            SoundManager.instance.GetAudioSources();


        if(PlayerManager.instance!=null && !PlayerManager.instance.isActing){
            PlayerManager.instance.canMove = true;
        }
    }
    public void SetDefault(){
        SceneController.instance.CameraView(PlayerManager.instance.transform);
        SceneController.instance.SetPlayerPosition();
        SceneController.instance.SetPlayerEquipments();
        InventoryManager.instance.ResetInventory();
        SceneController.instance.SetConfiner(DBManager.instance.curData.curMapNum,isDirect:true);
        SoundManager.instance.SoundOff();
    }

    public void ResetFader(float value){
        var defaultColor = loadFader.color;
        loadFader.color = new Color(defaultColor.r,defaultColor.g,defaultColor.b,value);
    }
//화면 검게하기
    public void FadeOut(){
        if(loadFader.color.a == 1){
            return;
        }

        loadFader.gameObject.SetActive(true);
        Debug.Log("LoadManager.FadeOut()");
        //ResetFader(0f);
        loadFader.GetComponent<Animator>().SetTrigger("fadeOut");

        // while (fogsNoiseTexPE.Density <= 4)
        // {
        //     fogsNoiseTexPE.Density += 0.01f;
        //     yield return null;
        // }
    }
    public void FadeIn(){        
        Debug.Log("LoadManager.FadeIn()");

        //ResetFader(1);
        //loadFader.gameObject.SetActive(true);
        loadFader.GetComponent<Animator>().SetTrigger("fadeIn");

        // while (fogsNoiseTexPE.Density >= 0)
        // {
        //     fogsNoiseTexPE.Density -= 0.04f;
        //     yield return null;
        // }
    }

    public IEnumerator ReloadGame(){
        //SceneController.instance.objects[0].GetComponent<Animator>().SetTrigger("go");
        
        UIManager.instance.SetFadeOut();
        yield return new WaitForSeconds(1f);
        loadFader.GetComponent<Animator>().SetTrigger("fadeOut");
        yield return new WaitForSeconds(1f);
        StartCoroutine(LoadNextScene(mainSceneName));
    }

    
#region Save&Load 
    public void ResetSaveSlots(){
        for(int i=0; i<saveSlotGrid.childCount; i++){
            if(DBManager.instance.CheckSaveFile(i)){
                saveSlots[i].saveNameText.text = CSVReader.instance.GetIndexToString(DBManager.instance.GetData(i).curMapNum,"map");
                saveSlots[i].saveDateText.text = DBManager.instance.GetData(i).curPlayDate;
            }
            else{
                saveSlots[i].saveNameText.text = "빈 슬롯";
                saveSlots[i].saveDateText.text = "-";
            }
        }
    }
    public void ResetLoadSlots(){
        for(int i=0; i<loadSlotGrid.childCount; i++){
            if(DBManager.instance.CheckSaveFile(i)){
                loadSlots[i].saveNameText.text = CSVReader.instance.GetIndexToString(DBManager.instance.GetData(i).curMapNum,"map");
                loadSlots[i].saveDateText.text = DBManager.instance.GetData(i).curPlayDate;
            }
            else{
                loadSlots[i].saveNameText.text = "빈 슬롯";
                loadSlots[i].saveDateText.text = "-";
            }
        }
    }
    public void Save(int curSaveNum){
        DBManager.instance.CallSave(curSaveNum);
        saveSlots[curSaveNum].saveNameText.text = CSVReader.instance.GetIndexToString(DBManager.instance.curData.curMapNum,"map");//DBManager.instance.curData.curMapName;
        saveSlots[curSaveNum].saveDateText.text = DBManager.instance.curData.curPlayDate;
    }
    public void Load(int curLoadNum){
        LoadManager.instance.isLoadingInGame = true;
        LoadManager.instance.lastLoadFileNum = curLoadNum;
        LoadManager.instance.LoadGame();
        Debug.Log(curLoadNum + "번 파일 로드 시도");
    }
    public void LoadLast(){
        LoadManager.instance.isLoadingInGameToLastPoint = true;
        LoadManager.instance.LoadGame();
        Debug.Log(LoadManager.instance.lastLoadFileNum + "번 파일 로드 시도 (마지막 저장)");

    }
#endregion

    void OnApplicationQuit(){
        DBManager.instance.CallLocalDataSave();
    }

    public void LoadScene(string sceneName){
        
        SceneManager.LoadScene(sceneName);
        
    }
}