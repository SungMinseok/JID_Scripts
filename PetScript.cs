using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PetScript : MonoBehaviour
{
    enum PetState{
        idle,
        move,
    }
    [SerializeField]
    float speed = 0.1f;
    [SerializeField]
    PetState petState;

    bool isLeft;
    bool checkTurning;



    [SerializeField]
    Vector2 leftPos, rightPos;

    public Dialogue[] saveDialogue;
    public Select saveSelect;
    public Dialogue[] idleDialogue;
    public Dialogue[] startDialogue;
    public float idleDialogueCoolTime;
    WaitForSeconds waitIdleDialogue;
    WaitUntil waitTalking;
    WaitForSeconds wait5s;
    Coroutine idleDialogueCoroutine;
    bool idleDialogueCoroutineFlag;
    public GameObject talkCanvas;
    RectTransform talkCanvasRect0, talkCanvasRect1;
    public Location trig88;




    Transform thePlayer;
    SpriteRenderer sr;
    void Start(){
        thePlayer = PlayerManager.instance.playerBody;
        sr = GetComponent<SpriteRenderer>();
        leftPos = GetComponent<RectTransform>().localPosition;
        rightPos = new Vector2(-leftPos.x, leftPos.y);

        waitIdleDialogue = new WaitForSeconds(idleDialogueCoolTime);
        waitTalking = new WaitUntil(()=>!PlayerManager.instance.isTalking);
        wait5s = new WaitForSeconds(5f);
        talkCanvas = transform.GetChild(0).GetChild(0).gameObject;

        talkCanvasRect0 = talkCanvas.GetComponent<RectTransform>();
        talkCanvasRect1 = talkCanvas.transform.GetChild(0).GetComponent<RectTransform>();
    }
    void OnDisable(){
        StopAllCoroutines();
    }
    void OnEnable(){
    }
    void Update(){
        //Debug.Log(Vector2.Distance(gameObject.transform.position,temp0.position));
        
        //오른쪽 볼 때, 왼쪽에 위치

            if(thePlayer.transform.localScale.x >= 0){
                isLeft = true;
            }
            else{
                isLeft = false;
            }
        

        //if(checkTurning){
        //    checkTurning = false;
        SetTalkCanvasDirection();
        //}

        if(PlayerManager.instance.isActing){
            if(idleDialogueCoroutine!=null){
                idleDialogueCoroutineFlag = false;
                talkCanvas.SetActive(false);
                StopCoroutine(idleDialogueCoroutine);
            }
        }
        else{
            if(!idleDialogueCoroutineFlag){
                idleDialogueCoroutineFlag = true;
                idleDialogueCoroutine = StartCoroutine(IdleDialogueCoroutine());

            }

        }

        //Debug.Log(idleDialogueCoroutine);

    }
    private void FixedUpdate() {
        if(!PlayerManager.instance.isTalking){
        
        if(isLeft){
            Move("left");
            //Vector2.MoveTowards(transform.position, leftPos, Time.deltaTime*speed);
        }
        else{
            //Vector2.MoveTowards(transform.position, rightPos, Time.deltaTime*speed);
            Move("right");
            
        }
        }
    }
    void Move(string direction){
        if(direction == "left"){

            if(transform.localPosition.x>=leftPos.x){
                transform.Translate(Vector2.left * speed);
                sr.flipX = false;
                //checkTurning = true;
                //SetTalkCanvasDirection();
                //transform.localScale = new Vector2();
            }
        }
        else{

            if(transform.localPosition.x<=rightPos.x){
                transform.Translate(Vector2.right * speed);
                sr.flipX = true;
                //checkTurning = true;
                //SetTalkCanvasDirection();
                //transform.localScale = Vector2.left;
            }
        }
    }
    public void TrySave(){
        PlayerManager.instance.isTalking = true;
        PlayerManager.instance.LockPlayer();
        StartCoroutine(TrySaveCoroutine());
    }
    IEnumerator TrySaveCoroutine(){
        DialogueManager.instance.SetDialogue(saveDialogue[0],null);
        yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
        SelectManager.instance.SetSelect(saveSelect,null);
        yield return new WaitUntil(()=>!PlayerManager.instance.isSelecting);
        if(SelectManager.instance.GetSelect()==0){
            MenuManager.instance.savePanel.SetActive(true);
            yield return new WaitUntil(()=>!MenuManager.instance.savePanel.activeSelf);
            DialogueManager.instance.SetDialogue(saveDialogue[Random.Range(1,4)],null);
            yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
        }
        else{
            DialogueManager.instance.SetDialogue(saveDialogue[Random.Range(4,7)],null);
            yield return new WaitUntil(()=>!PlayerManager.instance.isTalking);
        }
        PlayerManager.instance.isTalking = false;
        PlayerManager.instance.UnlockPlayer();
    }
    IEnumerator IdleDialogueCoroutine(){
        while(true){
            Debug.Log("시작");

            yield return waitIdleDialogue;

            //int tempSentenceNum = Random.Range(0,dialogues[0].sentences.Length);
            string curDialogueText = CSVReader.instance.GetIndexToString(int.Parse(idleDialogue[0].sentences[Random.Range(0,10)]),"dialogue");

            talkCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = curDialogueText;//dialogues[0].sentences[tempSentenceNum];
            talkCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>().maxVisibleCharacters = curDialogueText.Length;
            talkCanvas.gameObject.SetActive(true);

            yield return wait5s;
            talkCanvas.SetActive(false);
            
        }
    }
    public void SetTalkCanvasDirection(){
        var tempVal = sr.flipX ? -1 : 1;

        talkCanvasRect0.localPosition = new Vector2(tempVal * 27.7f, talkCanvasRect0.localPosition.y);
        talkCanvasRect0.localScale = new Vector2(tempVal, talkCanvasRect0.localScale.y);
        
        talkCanvasRect1.localScale = new Vector2(tempVal, talkCanvasRect1.localScale.y);
        
    }
    public void SetStartDialogue(){
        PlayerManager.instance.LockPlayer();
        TriggerScript.instance.Action(trig88);
        //StartCoroutine(StartDialogueCoroutine());
    }
    IEnumerator StartDialogueCoroutine(){
        for(int i=0;i<5;i++){

            DialogueManager.instance.SetDialogue(saveDialogue[i]);
            yield return waitTalking;
        }

        
    }
}
