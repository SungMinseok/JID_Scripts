using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Dialogue
{
    public string comment;
    [Header("화자 설정 : 설정하지 않으면 주인공")]
    public Transform talker;
    //[TextArea(2,2)]
    public string[] sentences;
    public bool isMonologue;
    
}
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    public Sprite playerNormalTalkCanvas;
    public Sprite playerMonologueTalkCanvas;
    // public Queue<string> sentQue;
    // public Queue<Dialogue> dialogueQue;
    public string curSentence;

    [Header("Debug──────────────────")]
    //public TextMeshPro text;
    public bool revealTextFlag;
    public bool revealTextFlag_NPC;
    public bool dialogueFlag;
    public bool canSkip;
    public bool canSkip2;
    public bool goSkip;

    

    public Coroutine nowDialogueCoroutine;
    
    void Awake()
    {
        instance = this;
    }


    public void SetFullDialogue(Dialogue[] dialogues,bool stopCheck = false)
    {
        StartCoroutine(DialogueCoroutine0(dialogues,stopCheck));
    }
    public void SetDialogue(Dialogue dialogue)
    {
        PlayerManager.instance.isTalking = true;
        StartCoroutine(DialogueCoroutine1(dialogue));
    }    

    //글자 한번에 나오기.
    public void SetDialogue_NPC(Dialogue dialogue)
    {
        //PlayerManager.instance.isTalking = true;
        StartCoroutine(DialogueCoroutine_NPC(dialogue));
    }
    public void SetRandomDialogue_NPC(string dialogueText,Transform talker,float duration, int interval){
        //Coroutine _randomDialogueCrt;
        //_randomDialogueCrt 
        talker.GetComponent<NPCScript>().randomDialogueCrt = StartCoroutine(SetRandomDialogueCoroutine(dialogueText,talker,duration,interval));
    }
    public void StopRandomDialogue_NPC(Coroutine coroutine){
        if(coroutine !=null) StopCoroutine(coroutine);
    }

    IEnumerator DialogueCoroutine0(Dialogue[] dialogues,bool stopCheck)
    {
        //Debug.Log("A");
        PlayerManager.instance.canMove = false;
        for(int i=0;i<dialogues.Length;i++){
            
            StartCoroutine(DialogueCoroutine1(dialogues[i]));
            yield return new WaitUntil(()=>!dialogueFlag);

        }

        PlayerManager.instance.isTalking = false;
        PlayerManager.instance.canMove = true;
    }

    //스킵 가능한 메시지 출력
    IEnumerator DialogueCoroutine1(Dialogue dialogue, bool oneTime = false, float typingSpeed =0.05f, float typingInterval= 1.5f)
    {
        //Debug.Log("B");
        PlayerManager.instance.isTalking = true;
        dialogueFlag= true;
        if(dialogue.talker==null) dialogue.talker = PlayerManager.instance.transform;

        if(dialogue.isMonologue){
            dialogue.talker.GetChild(0).GetChild(0).GetComponent<Image>().sprite = playerMonologueTalkCanvas;
        }
        else{
            if(dialogue.talker == PlayerManager.instance.transform)
                dialogue.talker.GetChild(0).GetChild(0).GetComponent<Image>().sprite = playerNormalTalkCanvas;
        }
            
        dialogue.talker.GetChild(0).GetChild(0).gameObject.SetActive(true);

        for(int i=0; i<dialogue.sentences.Length;i++){
            
            var tmp = dialogue.talker.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            curSentence = dialogue.sentences[i];
            if(!oneTime) nowDialogueCoroutine = StartCoroutine(RevealText(dialogue, tmp, typingSpeed, typingInterval));
             
            yield return new WaitUntil(()=>!revealTextFlag);

        }

        dialogue.talker.GetChild(0).GetChild(0).gameObject.SetActive(false);
        
        dialogueFlag= false;
        PlayerManager.instance.isTalking = false;
    }
    
    
    //스킵 불가능한 NPC 자체 메시지 출력
    IEnumerator DialogueCoroutine_NPC(Dialogue dialogue, bool oneTime = false, float typingSpeed =0.05f, float typingInterval= 1.5f)
    {
        dialogue.talker.GetChild(0).GetChild(0).gameObject.SetActive(true);

        for(int i=0; i<dialogue.sentences.Length;i++){
            
            var tmp = dialogue.talker.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            curSentence = dialogue.sentences[i];
            if(!oneTime) StartCoroutine(RevealText_NPC(dialogue, tmp, typingSpeed, typingInterval));
             
            yield return new WaitUntil(()=>!revealTextFlag_NPC);

        }

        dialogue.talker.GetChild(0).GetChild(0).gameObject.SetActive(false);
        
    }

    //스킵 가능한 메시지 출력(플레이어 or 스토리 상 NPC)
    IEnumerator RevealText(Dialogue dialogue, TextMeshProUGUI tmp, float typingSpeed, float typingInterval){

        
        goSkip = false;
        revealTextFlag = true;
        int totalVisibleCharacters = curSentence.Length;
        WaitForSeconds _typingSpeed = new WaitForSeconds(typingSpeed);
        WaitForSeconds _typingInterval = new WaitForSeconds(typingInterval);
        //Debug.Log("토탈문자갯수 : " + totalVisibleCharacters);
        if(dialogue.isMonologue){
            tmp.text = "<color=white>" + curSentence;
        }
        else{
            tmp.text = curSentence;
        }
        tmp.maxVisibleCharacters = 0;

        canSkip = true;
        for(int i=0; i<totalVisibleCharacters+1; i++){
            if(goSkip){
                goSkip = false;
                canSkip = false;
                i = totalVisibleCharacters;
            }
            tmp.maxVisibleCharacters = i;
            
            yield return _typingSpeed;

        }
        canSkip =false;
        canSkip2 = true;
        yield return _typingInterval;
        canSkip2 =false;
        //Debug.Log("문장종료");
        revealTextFlag = false;
#region 
        // while(revealTextFlag){
        //     visibleCount = count % (totalVisibleCharacters +1 );
        //     tmp.maxVisibleCharacters = visibleCount;
        //     Debug.Log("맥스비지블카운트 : " + tmp.maxVisibleCharacters );
        //     Debug.Log("비지블카운트 : " + visibleCount );

        //     if(visibleCount >= totalVisibleCharacters){
        //         yield return new WaitForSeconds(3.0f);
        //         Debug.Log("문장종료");
        //         revealTextFlag = false;
        //     }

        //     count += 1;

        // //Debug.Log("갯수세기 : " + count);
        //     yield return new WaitForSeconds(0.05f);
        // }
#endregion

    }

    //스킵 불가능한 NPC 자체 메시지 출력
    IEnumerator RevealText_NPC(Dialogue dialogue, TextMeshProUGUI tmp, float typingSpeed, float typingInterval){
        revealTextFlag_NPC = true;
        int totalVisibleCharacters = curSentence.Length;
        WaitForSeconds _typingSpeed = new WaitForSeconds(typingSpeed);
        WaitForSeconds _typingInterval = new WaitForSeconds(typingInterval);
        tmp.text = curSentence;
        tmp.maxVisibleCharacters = 0;
        for(int i=0; i<totalVisibleCharacters+1; i++){

            tmp.maxVisibleCharacters = i;
            
            yield return _typingSpeed;

        }
        yield return _typingInterval;
        revealTextFlag_NPC = false;

    }

    //스킵 불가능한 NPC 자체 랜덤 메시지 출력
    IEnumerator SetRandomDialogueCoroutine(string dialogueText, Transform talker, float duration, int interval){
        talker.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = dialogueText;
        talker.GetChild(0).GetChild(0).gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        talker.GetChild(0).GetChild(0).gameObject.SetActive(false);


    }

    void Update(){
        if(canSkip){
            if(Input.GetButtonDown("Interact")){
                goSkip = true;
            }
        }
        else if(canSkip2){
            
            if(Input.GetButtonDown("Interact")){
                revealTextFlag = false;
                StopCoroutine(nowDialogueCoroutine);
            }
        }
    }


    public class DialogueData{
        public int index { get; set; }
        public string sentences { get; set; }
    }

    public void SetDialogueData(){
        TextAsset dialogueData= Resources.Load<TextAsset>("Dialogue");
        string content = dialogueData.text;
    }
}

