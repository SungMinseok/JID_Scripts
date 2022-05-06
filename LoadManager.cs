using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UB.Simple2dWeatherEffects.Standard;
using TMPro;

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

    public string mainSceneName;

    WaitForSeconds wait1s = new WaitForSeconds(1);
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
        + "_"
        + DBManager.instance.buildVersion
        + "_"
        + DBManager.instance.buildNum
        + "_" 
        + DBManager.instance.buildDate
        ;
    }

    public void MainToGame()
    {
        StartCoroutine(MainToGameCoroutine());
//        Debug.Log("33");
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
        
        TriggerScript.instance.StopAllCoroutines();
        PlayerManager.instance.StopAllCoroutines();
        SoundManager.instance.SoundOff();

        StartCoroutine(LoadCoroutine("Main"));


    }
    IEnumerator LoadCoroutine(string sceneName){
        FadeOut();
        yield return wait1s;
        StartCoroutine(LoadNextScene(sceneName));
        if(VideoManager.instance.isPlayingVideo){
            VideoManager.instance.StopVideo();
        }
    }
    
    IEnumerator LoadNextScene(string nextScene)
    {
        reloadScene = true;
        SceneManager.LoadScene("Loading");
        yield return null;

#region [ 인게임 > 파일 선택해서 불러오기, 처 ]

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

#region [ 인게임 > 마지막 저장지점 불러오기 ]
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

#region [ 인게임 > 메인씬 ]
        else if(nextScene == "Main"){
                
            if(DDOLScript.instance!=null) Destroy(DDOLScript.instance.gameObject);
            if(PlayerManager.instance!=null) Destroy(PlayerManager.instance.gameObject);
        }
        else{
            DBManager.instance.LoadDefaultData();
            //DBManager.instance.curData = DBManager.instance.emptyData.DeepCopy();
        }

#endregion

#region [ 데이터 로드 완료 후 ]

        AsyncOperation asyncScene = SceneManager.LoadSceneAsync(nextScene);
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

//게임 첫 시작시
        // if(!isLoadingInGame){
        //     switch(nextScene){
        //         case "Level2" :
        //             DBManager.instance.curData = DBManager.instance.emptyData;

        //             yield return wait1s;
        //             SceneController.instance.SetFirstLoad();
        //             break;
        //     }

        // }
//인게임 로드 시
        if(isLoadingInGame){
            isLoadingInGame = false;
            
            var tempData = DBManager.instance.curData;
            yield return wait1s;

            //SceneController.instance.SetConfiner(tempData.curMapNum);
            //StartCoroutine(SetCameraPos(tempData.curMapNum));
            SceneController.instance.CameraView(PlayerManager.instance.transform);
            SceneController.instance.SetPlayerPosition();
            SceneController.instance.SetPlayerEquipments();
            InventoryManager.instance.ResetInventory();
            SceneController.instance.SetConfiner(tempData.curMapNum);
            SoundManager.instance.SoundOff();
            SoundManager.instance.SetBgmByMapNum(tempData.curMapNum);
            Debug.Log(lastLoadFileNum + "번 파일 로드 완료");

        }
        else if(isLoadingInGameToLastPoint){
            isLoadingInGameToLastPoint = false;
            
            var tempData = DBManager.instance.curData;
            yield return wait1s;

            //SceneController.instance.SetConfiner(tempData.curMapNum);
            //StartCoroutine(SetCameraPos(tempData.curMapNum));
            if(lastLoadFileNum == -1){

                //SceneController.instance.SetFirstLoad();
                SceneController.instance.CameraView(PlayerManager.instance.transform);
                SceneController.instance.SetPlayerPosition();
                SceneController.instance.SetPlayerEquipments();
                InventoryManager.instance.ResetInventory();
                SceneController.instance.SetConfiner(tempData.curMapNum);
            SoundManager.instance.SoundOff();
                SoundManager.instance.SetBgmByMapNum(tempData.curMapNum);
                Debug.Log("빈 파일 로드 완료");
            }
            else{
                    
                SceneController.instance.CameraView(PlayerManager.instance.transform);
                SceneController.instance.SetPlayerPosition();
                SceneController.instance.SetPlayerEquipments();
                InventoryManager.instance.ResetInventory();
                SceneController.instance.SetConfiner(tempData.curMapNum);
            SoundManager.instance.SoundOff();
                SoundManager.instance.SetBgmByMapNum(tempData.curMapNum);
                Debug.Log(lastLoadFileNum + "번 파일 로드 완료");
            }

        }
        else{
    
            //DBManager.instance.curData = DBManager.instance.emptyData;

            yield return wait1s;
            SceneController.instance.SetFirstLoad();
        }


        // if(PlayerManager.instance!=null){
        //     if(PlayerManager.instance.isGameOver){
        //         PlayerManager.instance.RevivePlayer();
        //     }
        // }



        yield return wait1s;
            //Debug.Log("A");
        FadeIn();

        loadFlag = false;
        reloadScene = false;
        
        yield return wait1s;
        // yield return wait1s;
        // yield return wait1s;
        if(PlayerManager.instance!=null){
            Debug.Log("33344");
            PlayerManager.instance.canMove = true;
        }
        //loadFader.GetComponent<Animator>().SetTrigger("fadeIn");
    }
    // IEnumerator SetCameraDefault(int num){
    //     yield return new WaitForSeconds(1f);
    //     SceneController.instance.SetConfiner(num);

    // }

    public void ResetFader(float value){
        var defaultColor = loadFader.color;
        loadFader.color = new Color(defaultColor.r,defaultColor.g,defaultColor.b,value);
    }

    public void FadeOut(){
        loadFader.gameObject.SetActive(true);
        loadFader.GetComponent<Animator>().SetTrigger("fadeOut");

        // while (fogsNoiseTexPE.Density <= 4)
        // {
        //     fogsNoiseTexPE.Density += 0.01f;
        //     yield return null;
        // }
    }
    public void FadeIn(){
        //Debug.Log("fadeIn");
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
}