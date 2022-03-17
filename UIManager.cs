using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    [Header("UI_Fader")]
    public Animator ui_fader;
    [Header("UI_HUD")]
    public GameObject hud_state;
    public GameObject hud_inventory;
    // [Header("UI_Fog")]
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
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        thePlayer = PlayerManager.instance;

        ResetGameOverUI();
        //playerOriginPos = thePlayer.transform.position;
        // offset = transform.position - worldToUISpace(ui_fog_canvas, PlayerManager.instance.transform.position);
    }
    void Update(){

        if(dirtGauge.fillAmount != DBManager.instance.curData.curDirtAmount / DBManager.instance.maxDirtAmount){
            dirtGauge.fillAmount = DBManager.instance.curData.curDirtAmount / DBManager.instance.maxDirtAmount;
        }

        if(honeyText.text != DBManager.instance.curData.curHoneyAmount.ToString()){
            honeyText.text = DBManager.instance.curData.curHoneyAmount.ToString();
        }

        
        //Convert the player's position to the UI space then apply the offset
        // ui_fog.transform.position = worldToUISpace(ui_fog_canvas, PlayerManager.instance.transform.position) + offset;
    }

    public void ActivateEffect(int num,float timer,bool bgOn = true){
        onEffect = true;
        StartCoroutine(ActivateEffectCoroutine(num, timer, bgOn));
    }
    IEnumerator ActivateEffectCoroutine(int num,float timer,bool bgOn){
        
        var canvasGroup = effects.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        
        

        if(bgOn){
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

        if(bgOn){
            effects.GetChild(0).gameObject.SetActive(false);
        }

        canvasGroup.alpha = 1;
        
        onEffect = false;
    }

    public void SetFadeOut(float speed = 1f){
        ui_fader.SetTrigger("fadeOut");
    }
    public void SetFadeIn(float speed = 1f){
        ui_fader.SetTrigger("fadeIn");
    }
    public void ResetFader(float value){
        var defaultColor = ui_fader.GetComponent<Image>().color;
        ui_fader.GetComponent<Image>().color = new Color(defaultColor.r,defaultColor.g,defaultColor.b,value);
    }
    public void FirstSceneLoad(){
        ResetFader(1);
        ui_fader.SetTrigger("fadeIn");
    }
#region GameOver
    public void SetGameOverUI(int num){
        //PlayerManager.instance.LockPlayer();
        PlayerManager.instance.isGameOver = true;
        StartCoroutine(SetGameOverUICoroutine(num));
    }
    public void ResetGameOverUI(){
        gameOverBtns.SetActive(false);
        gameOverNewImage.gameObject.SetActive(false);
    }
    IEnumerator SetGameOverUICoroutine(int num){
        
        DBManager.instance.EndingCollectionOver(num);

        yield return new WaitForSeconds(1.5f);
        //SetFadeOut();
        LoadManager.instance.FadeOut();
        yield return new WaitForSeconds(1.5f);

        if(MinigameManager.instance.nowMinigameNum!=-1) MinigameManager.instance.minigameScriptTransforms[MinigameManager.instance.nowMinigameNum].gameObject.SetActive(false);
        ui_gameOver_image.sprite = DBManager.instance.endingCollectionSprites[num];
        //ui_gameOver_image.sprite = gameOverSprites[num]; DBManager.instance.endingCollectionSprites[tempCardNum[i]]
        ui_gameOver.SetActive(true);
        LoadManager.instance.FadeIn();

        SoundManager.instance.PlaySound("gameover"+Random.Range(0,3));
        yield return new WaitForSeconds(2f);
        gameOverBtns.gameObject.SetActive(true);

        
        if(!DBManager.instance.CheckEndingCollectionOver(num)){

            UIManager.instance.gameOverNewImage.gameObject.SetActive(true);
        }
    }
#endregion

#region GameEnd
    public void SetGameEndUI(int num){
        PlayerManager.instance.isGameOver = true;
        for(int i=0;i<DBManager.instance.cache_GameEndDataList.Count;i++){
            if(DBManager.instance.cache_GameEndDataList[i].endingNum == num){
                Debug.Log(i + "번 엔딩 코루틴 시작");
                StartCoroutine(SetGameEndUICoroutine(i));
                return;
            }
        }

    }
    IEnumerator SetGameEndUICoroutine(int num){
        GameEndList curGameEndList = DBManager.instance.cache_GameEndDataList[num];
        DBManager.instance.EndingCollectionOver(curGameEndList.endingCollectionNum);
        var reader0 = CSVReader.instance.data_collection;
        var reader1 = CSVReader.instance.data_story;
        string language = DBManager.instance.language;

        LoadManager.instance.FadeOut();
        yield return wait2000ms;


        gameEndImageCanvas.alpha = 0;
        gameEndTextCanvas0.alpha = 0;
        gameEndTextCanvas1.alpha = 0;
        gameEndNextBtn.SetActive(false);
        //gameEndSkipBtn.SetActive(false);

        ui_gameEnd.SetActive(true);
        LoadManager.instance.FadeIn();


        //for(int i=0;i<curGameEndList.storySprites.Length;i++){

        gameEndImage.sprite = curGameEndList.stories[0].sprite;

        gameEndImageCanvas.alpha = 0;
        while(gameEndImageCanvas.alpha < 1){
            gameEndImageCanvas.alpha += 0.01f;
            yield return wait10ms;
        }

        yield return wait500ms;
        //}

        for(int i=0;i<curGameEndList.stories.Length;i++){

            if(curGameEndList.stories[i].sprite != null && i!=0){
                gameEndImage.sprite = curGameEndList.stories[i].sprite;
            }

            //gameEndText0.text = curGameEndList.stories[i].descriptions;
            gameEndText0.text = reader1[int.Parse(curGameEndList.stories[i].descriptions)]["text_"+language].ToString();//curGameEndList.stories[i].descriptions;

            gameEndTextCanvas0.alpha = 0;
            while(gameEndTextCanvas0.alpha < 1){
                gameEndTextCanvas0.alpha += 0.01f;
                yield return wait10ms;
            }   

            yield return wait1000ms;
            gameEndCanSkip = true;
            gameEndNextBtn.SetActive(true);
            yield return new WaitUntil(()=>!gameEndCanSkip);
            gameEndNextBtn.SetActive(false);

        }


        while(gameEndTextCanvas0.alpha > 0){
            gameEndImageCanvas.alpha -= 0.01f;
            gameEndTextCanvas0.alpha -= 0.01f;
            yield return wait10ms;
        }   

        yield return wait500ms;

        gameEndText1_0.text = "ending no."+curGameEndList.endingNum;
        //gameEndText1_1.text = curGameEndList.name;
        gameEndText1_1.text = reader0[curGameEndList.endingCollectionNum]["name_"+language].ToString();

        while(gameEndTextCanvas1.alpha < 1){
            gameEndTextCanvas1.alpha += 0.01f;
            yield return wait10ms;
        }
        yield return wait2000ms;
        while(gameEndTextCanvas1.alpha > 0){
            gameEndTextCanvas1.alpha -= 0.01f;
            yield return wait10ms;
        }
        yield return wait500ms;
        
        LoadManager.instance.LoadMain();
    }
    public void PushNextBtn(){
        
            gameEndCanSkip = false;
    }
#endregion






    public void SetHUD(bool active){
        hud_state.SetActive(active);
        hud_inventory.SetActive(active);
    }
    public void SetFadeHUD(bool active){
    }
    public void ChangeSprite(Sprite sprite){
        
    }

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

}
