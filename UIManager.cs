using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    //Vector3 playerOriginPos;
    PlayerManager thePlayer;
    //public GameObject clearPanel;
    public GameObject bgBlack;
    [Header("UI_States")]
    public Image dirtGauge;
    public TextMeshProUGUI honeyText;
    public Transform iceGaugeMother;
    public float iceGaugeCoolTime = 20f;
    public GameObject dirtHolder;
    public Text dirtHolderCountText;
    public Text dirtHolderHotKeyText;
    [Header("UI_Select")]
    public GameObject ui_select;
    public Transform ui_select_grid;
    public Color non_selected_color;
    public Color selected_color;
    public Sprite non_selected_sprite;
    public Sprite selected_sprite;
    [Header("UI_Effects")]
    public Transform effects;
    public bool onEffect;
    [Header("UI_GameOver")]
    public GameObject ui_gameOver;
    public Image ui_gameOver_image;
    public GameObject gameOverBtns;
    public Sprite[] gameOverSprites;
    public Animator gameOverNewImage;

    [Header("UI_GameEnd")]
    public GameObject ui_gameEnd;
    public CanvasGroup gameEndImageCanvas, gameEndTextCanvas0, gameEndTextCanvas1;
    public Image gameEndImage;
    public TextMeshProUGUI gameEndText0;
    public TextMeshProUGUI gameEndText1_0;
    public TextMeshProUGUI gameEndText1_1;
    public GameObject gameEndSkipBtn;
    public GameObject gameEndNextBtn;
    public bool gameEndCanSkip;
    public float gameEndMainTextPos_center;
    public float gameEndMainTextPos_lower;
    [Header("UI_Fader")]
    public Animator ui_fader;
    [Header("UI_HUD")]
    public GameObject hud_state;
    public GameObject hud_inventory;
    public GameObject hud_sub_collection;
    public GameObject hud_sub_endingGuide;
    public GameObject hud_sub_map;
    public GameObject hud_block;
    public bool getNewEndingCollection;
    public bool getNewAntCollection;
    public bool getNewItemCollection;
    public GameObject hud_sub_collection_redDot;
    public GameObject hud_sub_endingGuide_redDot;
    public bool blockUseItem;
    public GameObject ui_block_only_hud;
    public Animator hud_alert_item;
    public TextMeshProUGUI hud_alert_item_text;
    public Animator hud_broadcast;//hud_alert_item과 동일하나 통일을 위해 추가(일괄 변경예정) 221011
    public TextMeshProUGUI hud_broadcast_text;
    Coroutine broadcastCoroutine;
    public GameObject hud_skip_btn;
    [Header("UI_Book")]
    public GameObject ui_book;
    public Image bookMainImage;
    public Image bookTextImage;
    public Sprite[] bookMainSprites;
    public Sprite[] bookTextSprites;
    [Header("UI_Tutorial")]
    public GameObject ui_tutorial;
    public Transform tutorialSet;
    public bool waitTutorial;
    public int curTutorialID;
    public bool canSkipTutorial;//스킵용 딜레이
    public int indexOfItemList;
    [Header("UI_Screen")]
    public GameObject ui_screen;
    public Transform screenMother;
    public bool screenOn;
    [Header("UI_Map")]
    public GameObject ui_map;
    public Transform mapTextMother;
    public Transform mapPointMother;
    [Header("UI_EndingGuide")]
    public GameObject ui_endingGuide;
    [Header("UI_MovieEffect")]
    public GameObject ui_movieEffect;
    [Header("UI_Quest")]
    public Transform ui_questSlotGrid;
    public QuestSlot[] questSlots;
    public List<int> curQuestIdList;//HUD 퀘스트 슬롯의 ID
    int curHudQuestSlotCount;

    // [Header("UI_Fog")]
    //[Header("ETC")]
    //public Sprite nullSprite;
    // public Canvas ui_fog_canvas;
    // public Transform ui_fog;
    // Vector3 offset = Vector3.zero;

    //WaitForSeconds waitTime = new WaitForSeconds(0.5f);
    WaitForSeconds wait10ms = new WaitForSeconds(0.01f);

    WaitForSeconds wait100ms = new WaitForSeconds(0.1f);
    WaitForSeconds wait500ms = new WaitForSeconds(0.5f);
    WaitForSeconds wait1000ms = new WaitForSeconds(1);
    WaitForSeconds wait2000ms = new WaitForSeconds(2);
    WaitForSeconds wait3000ms = new WaitForSeconds(3);
    WaitForSeconds waitIceGaugeCoolTime;

    int calculatedHoney;
    [Header("Debug")]
    public bool iceGaugeFlag;
    Coroutine gameEndCoroutine;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        thePlayer = PlayerManager.instance;
        calculatedHoney = 0;
        honeyText.text = "0";
        ResetGameOverUI();

        
        //전부 인식 완료됐으면 메인 레드닷 제거
        if(UIManager.instance.CheckCollectionOverListAllRecognized()){
            UIManager.instance.hud_sub_collection_redDot.SetActive(false);
        }
        else{
            UIManager.instance.hud_sub_collection_redDot.SetActive(true);
        }
        //playerOriginPos = thePlayer.transform.position;
        // offset = transform.position - worldToUISpace(ui_fog_canvas, PlayerManager.instance.transform.position);
    
//        UIManager.instance.SetDirtOnlyHUD(DBManager.instance.dirtOnlyHUD);
        waitIceGaugeCoolTime = new WaitForSeconds(iceGaugeCoolTime);

        for(int i=0;i<mapTextMother.childCount;i++){
            mapTextMother.GetChild(i).GetComponent<TranslateText>().key = i;
        }

        questSlots = new QuestSlot[ui_questSlotGrid.childCount];
        for(int i=0;i<ui_questSlotGrid.childCount;i++){
            int temp = i;
            questSlots[i] = ui_questSlotGrid.GetChild(i).GetComponent<QuestSlot>();
            questSlots[i].GetComponent<Button>().onClick.AddListener(()=>GetRewardCompletedQuest(temp));
            questSlots[i].gameObject.SetActive(false);
        }


        // DBManager.instance.curData.questStateList.OrderBy(x => x.questID);
        // List<int> tempList = new List<int>();
        // for(int i=0; i<DBManager.instance.curData.questStateList.Count; i++){
        //     if(!DBManager.instance.curData.questStateList[i].gotReward
        //     &&DBManager.instance.curData.questStateList[i].isCompleted){
        //         tempList.Add(DBManager.instance.curData.questStateList[i].questID);
        //     }
        // }
        // for(int i=0; i<DBManager.instance.curData.questStateList.Count; i++){
        //     if(!DBManager.instance.curData.questStateList[i].gotReward
        //     &&!DBManager.instance.curData.questStateList[i].isCompleted){
        //         tempList.Add(DBManager.instance.curData.questStateList[i].questID);
        //     }
        // }
        // curQuestIdList = tempList;

        SortQuestStateList();
        SetQuestSlotGrid();
    }
    void OnDisable()
    {
        StopAllCoroutines();
    }
    void Update()
    {
 
        // KeyCode code = (KeyCode)Enum.Parse(typeof(KeyCode), alphaValue.ToString());
        // if(Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), alphaValue.ToString());))

        if (dirtGauge.fillAmount != DBManager.instance.curData.curDirtAmount / DBManager.instance.maxDirtAmount)
        {
            dirtGauge.fillAmount = DBManager.instance.curData.curDirtAmount / DBManager.instance.maxDirtAmount;
        }

        // if(honeyText.text != DBManager.instance.curData.curHoneyAmount.ToString()){

        //     int temp = DBManager.instance.curData.curHoneyAmount - int.Parse(honeyText.text);
        //     //honeyText.text = DBManager.instance.curData.curHoneyAmount.ToString();
        //     honeyText.text = temp > 0 ?  += 1;
        // }

        if (calculatedHoney != DBManager.instance.curData.curHoneyAmount)
        {
            int temp = DBManager.instance.curData.curHoneyAmount - calculatedHoney;
            if (temp >= 10 || temp <= -10)
            {
                calculatedHoney = calculatedHoney + temp / 10;
            }
            else
            {
                calculatedHoney = temp > 0 ? calculatedHoney + 1 : calculatedHoney - 1;

            }
            honeyText.text = string.Format("{0:#,###0}", calculatedHoney);
        }

        if (waitTutorial && canSkipTutorial)
        {
            //if (Input.GetButtonDown("Interact_OnlyKey"))
            if ( PlayerManager.instance.interactInput )
            {
                CloseTutorial();
            }
        }

        if ( gameEndCanSkip ){
            
            if ( PlayerManager.instance.interactInput )
            {
                //gameEndCanSkip = false;
                PushNextBtn();
            }
        }
        // if(Input.GetKeyDown(KeyCode.Escape)){
        //     // if(screenOn){
        //     //     CloseScreen();
        //     // }
        // }

    }

    public void ActivateEffect(int num, float timer, bool bgOn = true)
    {
        onEffect = true;
        StartCoroutine(ActivateEffectCoroutine(num, timer, bgOn));
    }
    IEnumerator ActivateEffectCoroutine(int num, float timer, bool bgOn)
    {

        var canvasGroup = effects.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;



        if (bgOn)
        {
            effects.GetChild(0).gameObject.SetActive(true);
        }

        effects.GetChild(num).gameObject.SetActive(true);



        while (canvasGroup.alpha <= 0.99f)
        {
            canvasGroup.alpha += 0.1f;
            yield return null;
        }


        yield return new WaitForSeconds(timer);



        while (canvasGroup.alpha >= 0.11)
        {
            canvasGroup.alpha -= 0.1f;
            yield return null;
        }

        effects.GetChild(num).gameObject.SetActive(false);

        if (bgOn)
        {
            effects.GetChild(0).gameObject.SetActive(false);
        }

        canvasGroup.alpha = 1;

        onEffect = false;
    }

    public void SetFadeOut(float speed = 1f)
    {
        ui_fader.SetTrigger("fadeOut");
    }
    public void SetFadeIn(float speed = 1f)
    {
        ui_fader.SetTrigger("fadeIn");
    }
    public void ResetFader(float value)
    {
        var defaultColor = ui_fader.GetComponent<Image>().color;
        ui_fader.GetComponent<Image>().color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, value);
    }
    public void FirstSceneLoad()
    {
        ResetFader(1);
        ui_fader.SetTrigger("fadeIn");
    }
    #region GameOver
    public void SetGameOverUI(int collectionID)
    {
        //PlayerManager.instance.LockPlayer();
        PlayerManager.instance.isGameOver = true;
        SoundManager.instance.BgmOff();
        StartCoroutine(SetGameOverUICoroutine(collectionID));
        Debug.Log("setgameoverID : "+collectionID);
    }
    public void ResetGameOverUI()
    {
        gameOverBtns.SetActive(false);
        gameOverNewImage.gameObject.SetActive(false);
    }
    IEnumerator SetGameOverUICoroutine(int collectionID)
    {
        Debug.Log("4141414");
        bool endingAlreadyOver;

        if (DBManager.instance.GetClearedEndingCollectionID(collectionID) == -1)
        {
            DBManager.instance.EndingCollectionOver(collectionID);
            

            endingAlreadyOver = false;
        }
        else
        {
            endingAlreadyOver = true;
        }

        if (collectionID == 21)
        {
            yield return wait2000ms;
        }
        else if (collectionID == 1)
        {
            yield return wait500ms;
        }

        yield return wait1000ms;
        yield return wait500ms;
        //SetFadeOut();
        LoadManager.instance.FadeOut();
        yield return wait1000ms;
        yield return wait500ms;

        switch (collectionID)
        {
            case 1:
                SoundManager.instance.PlaySound("spear_death");
                yield return wait3000ms;
                yield return wait500ms;
                break;
                // case 3 : 
                //     SoundManager.instance.PlaySound("ending_minigamefail");
                //     break;
        }

        //일반 죽음 횟수 체크
        if(DBManager.instance.cache_EndingCollectionDataList.Find(x=>x.ID == collectionID).trueID == 0){
            DBManager.instance.localData.deathCount += 1;
            if(DBManager.instance.localData.deathCount>=10){
                SteamAchievement.instance.ApplyAchievements(12);
            }
        }


        //이미 획득한 경우, 바로 마지막 불러오기
        if (!endingAlreadyOver)
        {

            if (MinigameManager.instance.nowMinigameNum != -1) MinigameManager.instance.minigameScriptTransforms[MinigameManager.instance.nowMinigameNum].gameObject.SetActive(false);
            ui_gameOver_image.sprite = DBManager.instance.endingCollectionSprites[collectionID];
            //ui_gameOver_image.sprite = gameOverSprites[num]; DBManager.instance.endingCollectionSprites[tempCardNum[i]]
            ui_gameOver.SetActive(true);
            LoadManager.instance.FadeIn();

            SoundManager.instance.PlaySound("gameover" + UnityEngine.Random.Range(0, 3));
            yield return new WaitForSeconds(2f);
            gameOverBtns.gameObject.SetActive(true);

            UIManager.instance.gameOverNewImage.gameObject.SetActive(true);
        }
        else{
            MenuManager.instance.LoadLast();
        }
    }
    #endregion

    #region GameEnd
    public void SetGameEndUI(int num)
    {
        PlayerManager.instance.watchingGameEnding = true;
        PlayerManager.instance.LockPlayer();
        PlayerManager.instance.isActing =true;

        var index = DBManager.instance.cache_GameEndDataList.FindIndex(x=> x.endingNum == num);

        //PlayerManager.instance.isGameOver = true;
        //for (int i = 0; i < DBManager.instance.cache_GameEndDataList.Count; i++)
        //{
            //if (DBManager.instance.cache_GameEndDataList[i].endingNum == num)
            if(index != -1)
            {
                Debug.Log(num + "번 진엔딩 시작");
                gameEndCoroutine = StartCoroutine(SetGameEndUICoroutine(index));
                return;
            }
            else{
                Debug.Log(num + "번 진엔딩 없음");

            }
        //}

    }
    IEnumerator SetGameEndUICoroutine(int num)
    {
        GameEndList curGameEndList = DBManager.instance.cache_GameEndDataList[num];
        DBManager.instance.EndingCollectionOver(curGameEndList.endingCollectionNum);
        var reader0 = CSVReader.instance.data_collection;
        var reader1 = CSVReader.instance.data_story;
        string language = DBManager.instance.language;

        LoadManager.instance.FadeOut();
        yield return wait3000ms;


        gameEndImageCanvas.alpha = 0;
        gameEndTextCanvas0.alpha = 0;
        gameEndTextCanvas1.alpha = 0;
        gameEndNextBtn.SetActive(false);
        gameEndImageCanvas.gameObject.SetActive(false);

        //gameEndSkipBtn.SetActive(false);

        ui_gameEnd.SetActive(true);
        LoadManager.instance.FadeIn();

            

        if (!string.IsNullOrEmpty(curGameEndList.preSoundFileName))
        {
            SoundManager.instance.BgmOff();
            SoundManager.instance.PlaySound(curGameEndList.preSoundFileName);
            yield return new WaitForSeconds(SoundManager.instance.GetSoundLength(curGameEndList.preSoundFileName));
        }
        
        if (!string.IsNullOrEmpty(curGameEndList.bgm))
        {
            SoundManager.instance.ChangeBgm(curGameEndList.bgm);
            //yield return new WaitForSeconds(SoundManager.instance.GetSoundLength(curGameEndList.preSoundFileName));
        }
        //for(int i=0;i<curGameEndList.storySprites.Length;i++){

        //gameEndImage.sprite = curGameEndList.stories[0].sprite;
        if (curGameEndList.stories[0].sprite != null/*  && i != 0 */)
        {
            gameEndTextCanvas0.transform.GetChild(0).localPosition = new Vector2(0,gameEndMainTextPos_lower);

            gameEndImage.sprite = curGameEndList.stories[0].sprite;
            gameEndImageCanvas.gameObject.SetActive(true);
            gameEndImageCanvas.GetComponent<Animator>().SetBool("fadeIn", true);
            
            // gameEndImageCanvas.alpha = 0;
            // while (gameEndImageCanvas.alpha < 1)
            // {
            //     gameEndImageCanvas.alpha += 0.02f;
            //     yield return wait10ms;
            // }
        }
        else{

            gameEndTextCanvas0.transform.GetChild(0).localPosition = new Vector2(0,gameEndMainTextPos_center);

            gameEndImage.sprite = null;
            gameEndImageCanvas.gameObject.SetActive(false);
        }


        yield return wait500ms;
        
        //삽화 진행

        for (int i = 0; i < curGameEndList.stories.Length; i++)
        {

            if (curGameEndList.stories[i].sprite != null/*  && i != 0 */)
            {
                gameEndTextCanvas0.transform.GetChild(0).localPosition = new Vector2(0,gameEndMainTextPos_lower);
                gameEndImage.sprite = curGameEndList.stories[i].sprite;
                gameEndImageCanvas.gameObject.SetActive(true);
                
            }
            else{
                gameEndTextCanvas0.transform.GetChild(0).localPosition = new Vector2(0,gameEndMainTextPos_center);
                gameEndImage.sprite = null;
                gameEndImageCanvas.gameObject.SetActive(false);
            }

            if (!string.IsNullOrEmpty(curGameEndList.stories[i].soundFileName))
            {
                SoundManager.instance.PlaySound(curGameEndList.stories[i].soundFileName);
            }
            //gameEndText0.text = curGameEndList.stories[i].descriptions;
            Debug.Log("index : " + curGameEndList.stories[i].descriptions);
            gameEndText0.text = reader1[int.Parse(curGameEndList.stories[i].descriptions)]["text_" + language].ToString();//curGameEndList.stories[i].descriptions;

            gameEndTextCanvas0.alpha = 0;
            while (gameEndTextCanvas0.alpha < 1)
            {
                gameEndTextCanvas0.alpha += 0.03f;
                yield return wait10ms;
            }

            yield return wait500ms;
            gameEndCanSkip = true;
            gameEndNextBtn.SetActive(true);
            yield return new WaitUntil(() => !gameEndCanSkip);
            gameEndNextBtn.SetActive(false);

        }

        gameEndImageCanvas.GetComponent<Animator>().SetBool("fadeIn", false);

        while (gameEndTextCanvas0.alpha > 0)
        {
            //gameEndImageCanvas.alpha -= 0.03f;
            gameEndTextCanvas0.alpha -= 0.05f;
            yield return wait10ms;
        }

        
        if (!string.IsNullOrEmpty(curGameEndList.postSoundFileName))
        {
            SoundManager.instance.PlaySound(curGameEndList.postSoundFileName);
            yield return new WaitForSeconds(SoundManager.instance.GetSoundLength(curGameEndList.postSoundFileName));
        }

        //삽화 노출 종료 후 엔딩 번호 & 제목 노출

        yield return wait500ms;

        gameEndSkipBtn.SetActive(false);

        gameEndText1_0.text = "<color=yellow>ending no." + curGameEndList.endingNum + "</color>";
        //gameEndText1_1.text = curGameEndList.name;
        gameEndText1_1.text = reader0[curGameEndList.endingCollectionNum]["name_" + language].ToString();

        while (gameEndTextCanvas1.alpha < 1)
        {
            gameEndTextCanvas1.alpha += 0.02f;
            yield return wait10ms;
        }
        yield return wait2000ms;
        while (gameEndTextCanvas1.alpha > 0)
        {
            gameEndTextCanvas1.alpha -= 0.02f;
            yield return wait10ms;
        }
        yield return wait500ms;

        if(curGameEndList.endingCollectionNum==7){
        LoadManager.instance.FadeOut();
            yield return wait1000ms;
            SoundManager.instance.BgmOff();
        LoadManager.instance.FadeIn();
            //yield return wait500ms;
            VideoManager.instance.PlayVideo(ResourceManager.instance.videoClips[0],isSkippable:true);
            yield return new WaitUntil(()=>!VideoManager.instance.isPlayingVideo);
        //LoadManager.instance.FadeOut();
            yield return wait500ms;
        }
        SoundManager.instance.BgmOff();

        LoadManager.instance.LoadMain();
    }
    public void PushNextBtn()
    {
        Debug.Log("44");
        gameEndCanSkip = false;
    }
    public void PushSkipBtn(){
        StopCoroutine(gameEndCoroutine);
        
        LoadManager.instance.LoadMain();
    }
    #endregion


    #region Book
    public void OpenBookUI(int bookMainNum, int bookTextNum)
    {
        bookMainImage.sprite = bookMainSprites[bookMainNum];

        if(DBManager.instance.language == "kr"){
            bookTextNum=bookTextNum*3+2;

        }
        else if(DBManager.instance.language == "en"){
            bookTextNum=bookTextNum*3;
        }
        else if(DBManager.instance.language == "jp"){
            bookTextNum=bookTextNum*3+1;

        }



        bookTextImage.sprite = bookTextSprites[bookTextNum];
        ui_book.SetActive(true);
    }


    #endregion

    #region @Tutorial
    public void WaitSkipTutorial(){
        canSkipTutorial = true;

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tutorialNum"></param>
    /// <param name="keyBlock">true시, 상호작용키로 못넘어감(퀘스트수락하는 버튼 터치해야되는 것들)</param>
    public void OpenTutorialUI(int tutorialNum, bool keyBlock = false)
    {
        waitTutorial = true;

        switch (tutorialNum)
        {
            case 0:
            case 4:
            case 8:
                keyBlock = true;
                break;
            default:
                break;
        }

        if(!keyBlock) Invoke("WaitSkipTutorial",0.5f);
        curTutorialID = tutorialNum;
        PlayerManager.instance.LockPlayer();
        SoundManager.instance.PlaySound("item_get");

        for (int i = 0; i < tutorialSet.childCount; i++)
        {
            tutorialSet.GetChild(i).gameObject.SetActive(false);
        }

        indexOfItemList = -1;//DBManager.instance.curData.itemList.FindIndex(x => x.itemID == 21);
        bool needSetPos = false;


        switch (tutorialNum)
        {
            case 0:
                indexOfItemList = GetMyItemIndex(5);
                needSetPos = true;
                break;
            case 3:
                indexOfItemList = GetMyItemIndex(21);
                needSetPos = true;
                break;
            case 4:
                indexOfItemList = GetMyItemIndex(2);
                needSetPos = true;
                break;

        }
        Debug.Log($"indexOfItemList : {indexOfItemList}");


        //if(tutorialNum == 3){
        //int indexOfItemList = DBManager.instance.curData.itemList.FindIndex(x => x.itemID == 21);
        
        if (needSetPos)
            {

                var panelPos = tutorialSet.GetChild(tutorialNum).GetChild(1).localPosition;
                Debug.Log(panelPos);

                if (indexOfItemList != -1)
                {
                    tutorialSet.GetChild(tutorialNum).GetChild(1).localPosition = new Vector2(panelPos.x, 421.3f - indexOfItemList * 63.07f);
                }
                else
                {
                    tutorialSet.GetChild(tutorialNum).GetChild(0).localPosition = new Vector2(panelPos.x, 421.3f);
                }
            }


        //}

        tutorialSet.GetChild(tutorialNum).gameObject.SetActive(true);
        ui_tutorial.gameObject.SetActive(true);
    }

    public int GetMyItemIndex(int _itemID)
    {
        return DBManager.instance.curData.itemList.FindIndex(x => x.itemID == _itemID);
    }

    public void UseItemInTutorial(int _itemID){
        InventoryManager.instance.UseItemByID(_itemID);
        Debug.Log($"UseItemInTutorial : {_itemID}");
        CloseTutorial();
        //waitTutorial = true;
    }
    public void CloseTutorial()
    {
        waitTutorial = false;
        canSkipTutorial = false;
        ui_tutorial.gameObject.SetActive(false);
        for (int i = 0; i < tutorialSet.childCount; i++)
        {
            tutorialSet.GetChild(i).gameObject.SetActive(false);
        }
        if(!PlayerManager.instance.isActing){

        PlayerManager.instance.UnlockPlayer();
        }


    }

    //0 : 중앙 분홍 박스 내 노출, 1 : 아래 노출
    public void ShowKeyTutorial(string keyString,string argumentIndex ="", int boxType = 0){
        //Debug.Log("show");
        PlayerManager.instance.onKeyTutorial = true;
        var tutorialBox = PlayerManager.instance.tutorialBox;

        if(boxType==1){
            tutorialBox = PlayerManager.instance.tutorialBox_Right;
        }
        else{
            tutorialBox.GetChild(0).gameObject.SetActive(true);
        }
        CSVReader csv = CSVReader.instance;
        //tutorialBox.GetChild(0).gameObject.SetActive(true);

        //string frontString = "", backString = "";
        //if(frontStringIndex!="") frontString = csv.GetIndexToString(int.Parse(frontStringIndex),"sysmsg");
        //if(backStringIndex!="") backString = csv.GetIndexToString(int.Parse(backStringIndex),"sysmsg");
        string argumentString = "";
        if(argumentIndex!=""){
            argumentString = csv.GetIndexToString(int.Parse(argumentIndex),"sysmsg");
            argumentString = string.Format(argumentString,keyString);

        }
        else{
            argumentString = keyString;

        }


        tutorialBox.GetChild(1).GetChild(0).GetComponent<Text>().text = argumentString;
        //frontString + keyString + backString ;
        tutorialBox.GetChild(1).GetComponent<Animator>().SetBool("activate",true);
        
    }
    
    public void HideKeyTutorial(){
        PlayerManager.instance.onKeyTutorial = false;
        //Debug.Log("hide");
        var tutorialBox = PlayerManager.instance.tutorialBox;
        tutorialBox.GetChild(0).gameObject.SetActive(false);
        tutorialBox.GetChild(1).GetComponent<Animator>().SetBool("activate",false);
        var tutorialBox_Right = PlayerManager.instance.tutorialBox_Right;
        //tutorialBox.GetChild(0).gameObject.SetActive(false);
        tutorialBox_Right.GetChild(1).GetComponent<Animator>().SetBool("activate",false);
    }


    #endregion

    
    #region HUD
    public void SetHUD(bool active)
    {
        if(active){
            hud_state.GetComponent<CanvasGroup>().alpha = 1;
        }
        else{
            hud_state.GetComponent<CanvasGroup>().alpha = 0;

        }
        //hud_inventory.SetActive(active);
        if(active){
            hud_inventory.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 1;
        }
        else{
            hud_inventory.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 0;

        }

        if (!active)
        {
            for (int i = 0; i < InventoryManager.instance.itemSlotScripts.Length; i++)
            {
                InventoryManager.instance.itemSlotScripts[i].itemSlot.itemDescriptionWindow.SetActive(false);
            }
        }
    }
    public void SetCollectionPage(bool active){
        if(MenuManager.instance==null) return;

        MenuManager.instance.SetCollectionPage(active);

    }
    public bool CheckCollectionOverListAllRecognized(){
        if(DBManager.instance.localData.antCollectionOverList.Exists(x=>!x.isRecognized)
        &&DBManager.instance.localData.endingCollectionOverList.Exists(x=>!x.isRecognized)
        ){
            return false;
        }
        else{
            return true;
        }
    }
    #endregion

    
    #region @IceGauge
    
    
    public IEnumerator FillIceGaugeCoroutine(){
        ResetIceGauge();
        for(int i=0;i<iceGaugeMother.childCount;i++){
            iceGaugeMother.GetChild(i).gameObject.SetActive(true);
            PlayerManager.instance.iceVignette.gameObject.SetActive(true);
            if(i!=4){
                yield return waitIceGaugeCoolTime;
            }


        }
        PlayerManager.instance.KillPlayer(6,"shake");
        
        //SetGameOverUI(6);
    }
    public void ResetIceGauge(){

        for(int i=0;i<iceGaugeMother.childCount;i++){
            iceGaugeMother.GetChild(i).gameObject.SetActive(false);
        }
    }
    #endregion
    
    public void OpenScreen(int screenNum){
        screenOn = true;
        for(int i=0;i<screenMother.childCount;i++){
            screenMother.GetChild(i).gameObject.SetActive(false);
        }
        screenMother.GetChild(screenNum).gameObject.SetActive(true);
        ui_screen.SetActive(true);
    }
    public void CloseScreen(){
        screenOn = false;
        ui_screen.SetActive(false);

    }
    public void OpenEndingGuide(){
        
    }
    public void CloseEndingGuide(){
        
    }
    
    public void ToggleEndingGuide(bool active){
        ui_endingGuide.SetActive(active);
        if(active){
            PlayerManager.instance.LockPlayer();
            //hud_sub_endingGuide_redDot.gameObject.SetActive(false);
        }
        else{
            PlayerManager.instance.UnlockPlayer();

        }
    }
//     public void ChangeKey(){
//         foreach(KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
// {
//     if (Input.GetKey(kcode))
//         Debug.Log("KeyCode down: " + kcode);
// }
//         GameInputManager.SetKeyMap("Jump",)
//     }
    // public Vector3 worldToUISpace(Canvas parentCanvas, Vector3 worldPos)
    // {
    //     //Convert the world for screen point so that it can be used with ScreenPointToLocalPointInRectangle function
    //     Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
    //     Vector2 movePos;

    //     //Convert the screenpoint to ui rectangle local point
    //     RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera, out movePos);
    //     //Convert the local point to world point
    //     return parentCanvas.transform.TransformPoint(movePos);
    // }
    public void LinkMenuManager(string name){
        try{
            MenuManager.instance.SendMessage(name);
        }
        catch{

        }

    }

    public void ClearStringArray(string[] array){
        Array.Clear(array,0,array.Length);
    }

    public void RefreshDirtHolder(){
        dirtHolderCountText.text = DBManager.instance.curData.curDirtItemCount.ToString();
    }
    public void PushDirtHolder(){
        
        if(DBManager.instance.curData.curDirtItemCount > 0){
            DBManager.instance.curData.curDirtItemCount -= 1;
            
            InventoryManager.instance.AddDirt(DBManager.instance.defaultGetDirtAmount);
            RefreshDirtHolder();
        }
        //InventoryManager.instance.UseItem(DBManager.instance.cache_ItemDataList[5]);
    }
    public void SetDirtOnlyHUD(bool active){
        DBManager.instance.dirtOnlyHUD = active;
        
        dirtHolder.SetActive(active);
        
        //if(InventoryManager.instance.itemSlot.Length!=0)
            InventoryManager.instance.ChangeDirtItemPostion(active);

    }

    public void SetActiveMapUI(bool active){
        
        for(int i=0;i<mapPointMother.childCount;i++){
            mapPointMother.GetChild(i).gameObject.SetActive(false);
        }

        mapPointMother.GetChild(DBManager.instance.curData.curMapNum).gameObject.SetActive(true);
        
        ui_map.SetActive(active);

    }
    public void SetMovieEffectUI(bool active){
        if(active){
        ui_movieEffect.SetActive(active);

        }
        else{
            ui_movieEffect.GetComponent<Animator>().SetBool("off",true);
        }
    }
    public void SetOnlyHudBlock(bool active){
        ui_block_only_hud.SetActive(active);
    }
    

#region @Quest
    /// <summary>
    /// 0 테스트용 퀘스트
/// 1 유치원 가기
/// 2 광장 가기
/// 3 야시장 가기
/// 4 수레 앞 개미에게 말 걸기
/// 5 버섯 농장 가기
/// 6 광장 둘러보기 ({0}/{1})
/// 7 농장 모두 방문하기 ({0}/{1})
/// 8 대왕 일개미 방 가기
/// 9 식당 가기
/// 10 나가는 길 가기
/// 11 흙더미 파기
/// 12 흙덩이 사용 ({0}/{1})
/// 13 개미탈 착용
/// 14 흰개미를 통해 게임 저장
/// 15 침대로 가기
/// 16 옷걸이의 모자를 쓰기
    /// </summary>
    /// <param name="questID">
    /// </param>
    /// 

    public void AcceptQuest(int questID){
        if(DBManager.instance.curData.questStateList.Exists(x=>x.questID == questID)) return;

        int curSlotNum = curHudQuestSlotCount;

        Debug.Log("AcceptQuest | QuestID : " + questID);
        SoundManager.instance.PlaySound("quest_accept",1.3f);

        //curQuestIdList.Add(questID);//슬롯 표시용
        DBManager.instance.curData.questStateList.Add(new QuestState(questID));

        ActivateBroadcastMsg(2f, CSVReader.instance.GetIndexToString(19,"sysmsg"));
        //curQuestIdList.Sort();
        SortQuestStateList();
        SetQuestSlotGrid();
    }
    public void SetQuestSlotGrid(int slotNum = -1){
        var theDB = DBManager.instance;
        int startNum = 0;
        int endNum = curQuestIdList.Count;
        if(slotNum != -1){
            startNum = slotNum;
            endNum = slotNum + 1;
        }
        for(int i=startNum;i<endNum;i++){

            var questID = curQuestIdList[i];
            //var tempID = theDB.curData.questStateList.FindIndex(x=>x.questID == curQuestIdList[i]);
            var questInfo = theDB.cache_questList.Find(x=>x.ID==questID);
            var questState = theDB.curData.questStateList.Find(x=>x.questID == questID);

            //if(!questState.gotReward){

            string tempQuestMainText = "";

            switch(questInfo.majorType){
                case 2 :
                case 4 :
                    tempQuestMainText = string.Format(questInfo.mainText,questState.progress,questInfo.targetVal);
                    break;
                default : 
                    tempQuestMainText = questInfo.mainText;
                    break;
            }


            questSlots[i].questID = questInfo.ID;
            questSlots[i].questMainText.text = tempQuestMainText;
            questSlots[i].questIcon.sprite = questInfo.icon;
            //완료 퀘스트 (보상 수령 대기)
            if(questState.isCompleted){
            
                Color completedColor = new Color(0.7f,0.7f,0.7f,1f);

                questSlots[i].questSlot.color = completedColor;
                questSlots[i].GetComponent<Button>().interactable = true;
                questSlots[i].questIcon.color = completedColor;
                questSlots[i].questMainText.color = completedColor;
                questSlots[i].questMainText.fontStyle = TMPro.FontStyles.Strikethrough;
                questSlots[i].questCheckObj.SetActive(true);

            }
            //미완료 퀘스트 (완료 대기)
            else{
                Color unCompletedColor = new Color(1f,1f,1f,1f);

                questSlots[i].questSlot.color = unCompletedColor;
                questSlots[i].GetComponent<Button>().interactable = false;
                questSlots[i].questIcon.color = unCompletedColor;
                questSlots[i].questMainText.color = unCompletedColor;
                questSlots[i].questMainText.fontStyle = TMPro.FontStyles.Normal;
                questSlots[i].questCheckObj.SetActive(false);
            }

            //}
            //else{

            //}

        }

        for (int i = 0 ; i < curQuestIdList.Count; i++)
        {
            questSlots[i].gameObject.SetActive(true);
        }
        for (int i = curQuestIdList.Count; i < theDB.curData.questStateList.Count;i++){
            questSlots[i].gameObject.SetActive(false);

        }

            ui_questSlotGrid.GetComponent<GridContentsAutoSizing>().AutoHeightSize(curQuestIdList.Count);

    }

    public void CompleteQuest(int questID){
        if(!curQuestIdList.Contains(questID)) return;

        var theDB = DBManager.instance;

        var tempID = DBManager.instance.curData.questStateList.FindIndex(x=>x.questID == questID);
        DBManager.instance.curData.questStateList[tempID].isCompleted = true;

        Debug.Log("CompleteQuest | QuestID : " + questID);
        SoundManager.instance.PlaySound("quest_complete",0.6f);

        //보상 꿀&아이템 없을경우, 그냥 바로 완료 처리
        var curQuest = theDB.cache_questList.Find(x=>x.ID==questID);
        if(curQuest.rewardHoneyAmount == 0 && curQuest.rewardItemList.Count==0){
            DBManager.instance.curData.questStateList[tempID].gotReward = true;
        }

        //221016 퀘스트 완료 시, 슬롯 스크롤 최상단으로
        ui_questSlotGrid.localPosition = Vector2.zero;

        ActivateBroadcastMsg(2f, CSVReader.instance.GetIndexToString(20,"sysmsg"));

        SortQuestStateList();
        SetQuestSlotGrid();
    }
    public void GetRewardCompletedQuest(int slotNum){
        var theDB = DBManager.instance;
        
        var tempID = theDB.curData.questStateList.FindIndex(x=>x.questID == curQuestIdList[slotNum]);
        DBManager.instance.curData.questStateList[tempID].gotReward = true;

        //보상지급
        var curQuest = theDB.cache_questList.Find(x=>x.ID==curQuestIdList[slotNum]);
        if(curQuest.rewardHoneyAmount > 0){
            InventoryManager.instance.AddHoney(curQuest.rewardHoneyAmount,activateDialogue:true);
        }
        if(curQuest.rewardItemList.Count != 0){
            for(int i=0;i<curQuest.rewardItemList.Count;i++){
                InventoryManager.instance.AddItem(curQuest.rewardItemList[i].itemID,curQuest.rewardItemList[i].itemAmount,activateDialogue: true);

            }

        }
        Debug.Log("GetRewardCompletedQuest | QuestID : " + curQuestIdList[slotNum]);
        SoundManager.instance.PlaySound("quest_reward",2f);
        //questSlots[curQuestIdList.Count - 1].gameObject.SetActive(false);

        SortQuestStateList();
        SetQuestSlotGrid();
    }

    //보상 수령 대기 > 진행 중 > 보상 수령 완료 순으로 정렬
    public void SortQuestStateList(){
        //Debug.Log("SortQuestStateList");
        DBManager.instance.curData.questStateList =
        DBManager.instance.curData.questStateList
        .OrderBy(x => x.gotReward) //보상 수령 대기
        .ThenByDescending(x => x.isCompleted) //진행 중
        .ThenBy(x => x.questID) //보상 수령 완료 순으로 정렬
        .ToList()
        ;

        List<int> tempQuestIdList = new List<int>();
        tempQuestIdList = DBManager.instance.curData.questStateList.Select(x=>x.questID).ToList();

        List<int> tempQuestIdList_gotReward = new List<int>();
        tempQuestIdList_gotReward = DBManager.instance.curData.questStateList.FindAll(x => x.gotReward == true).Select(x=>x.questID).ToList();


        //보상수령 완료한 퀘스트 제외
        curQuestIdList = tempQuestIdList.Except(tempQuestIdList_gotReward).ToList();
    }



#endregion
    
#region @BrodcastMsg
    public void ActivateBroadcastMsg(float duration = 2f, string msg = "nullText", params string[] arguments){
        if(broadcastCoroutine!=null) StopCoroutine(broadcastCoroutine);
        broadcastCoroutine = StartCoroutine(BroadcastMsgCoroutine(duration, msg, arguments));

    }
    IEnumerator BroadcastMsgCoroutine(float duration = 2f, string msg = "nullText", params string[] arguments){
        //yield return null;
        hud_broadcast_text.text = string.Format(msg,arguments);
        hud_broadcast.SetTrigger("on");

        if(duration == 2f){
            yield return wait2000ms;
        }
        else{
            
        }
        hud_broadcast.SetTrigger("off");

    }
#endregion


}
