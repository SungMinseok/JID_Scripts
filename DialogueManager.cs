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
    Coroutine curWaitSkipCoroutine;

    [Header("Debug──────────────────")]
    //public TextMeshPro text;
    public bool revealTextFlag;
    public bool revealTextFlag_NPC;
    public bool dialogueFlag;
    [Tooltip("현재 진행 중인 대화 텍스트 모두 출력 가능한 상태")]
    public bool canSkip;    
    [Tooltip("다음 대화 출력 가능 상태")]
    public bool canSkip2;
    public bool goSkip;

    
    WaitForSeconds wait500ms = new WaitForSeconds(0.5f);
    public Coroutine nowDialogueCoroutine;
    
    void Awake()
    {
        instance = this;
    }


    public void SetDialogue(Dialogue dialogue, string argument = null)
    {
        PlayerManager.instance.isTalking = true;
        StartCoroutine(DialogueCoroutine(dialogue, argument));
    }    

    //글자 한번에 나오기.
    public void SetDialogue_NPC(Dialogue dialogue)
    {
        //PlayerManager.instance.isTalking = true;
        StartCoroutine(DialogueCoroutine_NPC(dialogue));
    }

    //스킵 가능한 메시지 출력
    IEnumerator DialogueCoroutine(Dialogue dialogue, string argument = null ,bool oneTime = false, float typingSpeed =0.05f, float typingInterval= 1.5f)
    {
        PlayerManager.instance.isTalking = true;
        dialogueFlag= true;
        if(dialogue.talker==null) dialogue.talker = PlayerManager.instance.transform;

        //독백
        if(dialogue.isMonologue){
            dialogue.talker.GetChild(0).GetChild(0).GetComponent<Image>().sprite = playerMonologueTalkCanvas;
        }
        else{
            if(dialogue.talker == PlayerManager.instance.transform)
                dialogue.talker.GetChild(0).GetChild(0).GetComponent<Image>().sprite = playerNormalTalkCanvas;
        }
            
        dialogue.talker.GetChild(0).GetChild(0).gameObject.SetActive(true);

        if(dialogue.talker.TryGetComponent<NPCScript>(out NPCScript tempNpcScript)){
            tempNpcScript.isTalkingWithPlayer = true;
        }

        for(int i=0; i<dialogue.sentences.Length;i++){
            
            var tmp = dialogue.talker.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

            curSentence = CSVReader.instance.GetIndexToString(int.Parse(dialogue.sentences[i]),"dialogue");

            if(argument != null){
                curSentence = string.Format(curSentence, argument);
            }

            if(!oneTime) nowDialogueCoroutine = StartCoroutine(RevealText(dialogue, tmp, DBManager.instance.waitTime_dialogueTypingInterval, DBManager.instance.waitTime_dialogueInterval));
             
            yield return new WaitUntil(()=>!revealTextFlag);

        }

        dialogue.talker.GetChild(0).GetChild(0).gameObject.SetActive(false);
        
        if(dialogue.talker.TryGetComponent<NPCScript>(out tempNpcScript)){
            tempNpcScript.isTalkingWithPlayer = false;
        }
        
        dialogueFlag= false;
        
        PlayerManager.instance.isTalking = false;
        
    }
    
    
    //스킵 불가능한 NPC 자체 메시지 출력
    IEnumerator DialogueCoroutine_NPC(Dialogue dialogue, bool oneTime = false, float typingSpeed =0.05f, float typingInterval = 1.5f)
    {
        dialogue.talker.GetComponent<NPCScript>().talkCanvas.gameObject.SetActive(true);

        for(int i=0; i<dialogue.sentences.Length;i++){
            
            var tmp = dialogue.talker.GetComponent<NPCScript>().talkCanvas.GetChild(0).GetComponent<TextMeshProUGUI>();
            //curSentence = dialogue.sentences[i];
            curSentence = CSVReader.instance.GetIndexToString(int.Parse(dialogue.sentences[i]),"dialogue");
            if(!oneTime) StartCoroutine(RevealText_NPC(dialogue, tmp, typingSpeed, typingInterval));
             
            yield return new WaitUntil(()=>!revealTextFlag_NPC);

        }

        dialogue.talker.GetComponent<NPCScript>().talkCanvas.gameObject.SetActive(false);
        
    }

    //스킵 가능한 메시지 출력(플레이어 or 스토리 상 NPC)
    IEnumerator RevealText(Dialogue dialogue, TextMeshProUGUI tmp, float typingSpeed, float typingInterval){

        
        goSkip = false;
        canSkip2 =false;
        revealTextFlag = true;
        int totalVisibleCharacters = curSentence.Length;
        WaitForSeconds _typingSpeed = new WaitForSeconds(typingSpeed);
        WaitForSeconds _typingInterval = new WaitForSeconds(typingInterval);
        //Debug.Log("토탈문자갯수 : " + totalVisibleCharacters);

        // if(curSentence.Length>20){
        //     curSentence = curSentence.Insert(20,"\n");
        //     totalVisibleCharacters++;
        // }
        // if(curSentence.Length>40){
        //     curSentence = curSentence.Insert(40,"\n");
        //     totalVisibleCharacters++;
        // }
        // if(curSentence.Length>60){
        //     curSentence = curSentence.Insert(60,"\n");
        //     totalVisibleCharacters++;
        // }

        if(dialogue.isMonologue){
            tmp.text = "<color=white>" + curSentence;
        }
        else{
            tmp.text = curSentence;
        }


        tmp.maxVisibleCharacters = 0;
        //Invoke("WaitSkip",0.5f);
        //canSkip = true;
        for(int i=0; i<totalVisibleCharacters+1; i++){
            if(i==6) canSkip = true;

            if(goSkip){
                goSkip = false;
                canSkip = false;
                i = totalVisibleCharacters;
            }
            tmp.maxVisibleCharacters = i;
            //print(i);

            if(DBManager.instance.localData.onTalkingSound){

                if(i<totalVisibleCharacters && curSentence[i] != '.'&& curSentence[i] != ' ' && i%2 == 0){

                    if(dialogue.talker == PlayerManager.instance.transform){
                        if(!dialogue.isMonologue)
                            SoundManager.instance.PlaySound("lucky_talk_"+Random.Range(1,11));

                    }
                    else if(dialogue.talker.GetComponent<NPCScript>().isMerchant){
                        SoundManager.instance.PlaySound("merchant_0"+Random.Range(1,8));

                    }
                    else if(dialogue.talker.GetComponent<NPCScript>().isKid){
                        SoundManager.instance.PlaySound("little_ant_talking_"+Random.Range(1,7));

                    }
                    else{
                        SoundManager.instance.PlaySound("ant_talking_"+Random.Range(1,10));

                    }

                }
            }
            
            yield return _typingSpeed;

        }
        canSkip =false;
        canSkip2 = true;
        if(DBManager.instance.waitTime_dialogueInterval>0){

            yield return _typingInterval;
            canSkip2 =false;
        }
        else{
            yield return new WaitUntil(()=>!canSkip2);
        }
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
    // public IEnumerator WaitSkipCoroutine(){
    //     yield return wait500ms;
    //     yield return wait500ms;
        
    //     canSkip = true;
    // }

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
            
            if(DBManager.instance.localData.onTalkingSound){

                if(i<totalVisibleCharacters && curSentence[i] != '.'&& curSentence[i] != ' ' && i%2 == 0){

                    if(dialogue.talker.GetComponent<NPCScript>().isMerchant){
                        SoundManager.instance.PlaySound("merchant_0"+Random.Range(1,8));

                    }
                    else if(dialogue.talker.GetComponent<NPCScript>().isKid){
                        SoundManager.instance.PlaySound("little_ant_talking_"+Random.Range(1,7));

                    }
                    else{
                        SoundManager.instance.PlaySound("ant_talking_"+Random.Range(1,10));

                    }

                }
            }


            yield return _typingSpeed;

        }
        yield return _typingInterval;
        revealTextFlag_NPC = false;

    }

    //스킵 불가능한 NPC 자체 랜덤 메시지 출력
    // IEnumerator SetRandomDialogueCoroutine(string dialogueText, Transform talker, float duration, int interval){
    //     talker.GetComponent<NPCScript>().talkCanvas.GetChild(0).GetComponent<TextMeshProUGUI>().text = dialogueText;
    //     talker.GetComponent<NPCScript>().talkCanvas.gameObject.SetActive(true);

    //     yield return new WaitForSeconds(duration);

    //     talker.GetComponent<NPCScript>().talkCanvas.gameObject.SetActive(false);


    // }

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


}

