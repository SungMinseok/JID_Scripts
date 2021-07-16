using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Dialogue
{
    [SerializeField]
    public string comment;
    [SerializeField]
    [Header("화자 설정 : 설정하지 않으면 주인공")]
    public Transform talker;
    [SerializeField]
    [TextArea(2,2)]
    public string[] sentences;
    
}
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    public Queue<string> sentQue;
    public Queue<Dialogue> dialogueQue;
    public string curSentence;
    //public TextMeshPro text;
    bool revealTextFlag;
    bool dialogueFlag;
    public bool canSkip;
    
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
    public void SetRandomDialogue(string dialogueText,Transform talker,float duration, int interval){
        Coroutine _randomDialogueCrt;
        _randomDialogueCrt = StartCoroutine(SetRandomDialogueCoroutine(dialogueText,talker,duration,interval));
        talker.GetComponent<NPCScript>().randomDialogueCrt = _randomDialogueCrt;
    }

    IEnumerator DialogueCoroutine0(Dialogue[] dialogues,bool stopCheck)
    {
        PlayerManager.instance.canMove = false;
        for(int i=0;i<dialogues.Length;i++){
            
            StartCoroutine(DialogueCoroutine1(dialogues[i]));
            yield return new WaitUntil(()=>!dialogueFlag);

        }

        PlayerManager.instance.isTalking = false;
        PlayerManager.instance.canMove = true;
    }

    IEnumerator DialogueCoroutine1(Dialogue dialogue, float typingSpeed =0.05f, float typingInterval= 1.5f)
    {
        PlayerManager.instance.isTalking = true;
        dialogueFlag= true;
        if(dialogue.talker==null) dialogue.talker = PlayerManager.instance.transform;
            
        dialogue.talker.GetChild(0).GetChild(0).gameObject.SetActive(true);

        for(int i=0; i<dialogue.sentences.Length;i++){
            
            var tmp = dialogue.talker.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            curSentence = dialogue.sentences[i];
            StartCoroutine(RevealText(dialogue, tmp, typingSpeed, typingInterval));
            yield return new WaitUntil(()=>!revealTextFlag);

        }

        dialogue.talker.GetChild(0).GetChild(0).gameObject.SetActive(false);
        
        dialogueFlag= false;
        PlayerManager.instance.isTalking = false;
#region 
        // while (sentQue.Count > 0)
        // {
        //     curSentence = sentQue.Dequeue();
        //     var tmp = dialogue.talker.GetChild(0).GetComponent<TextMeshProUGUI>();
        //     tmp.text = curSentence;
        //     //yield return new WaitForSeconds(0.001f);
        //     yield return null;
        //     //Debug.Log(dialogue.talker.GetChild(0).GetComponent<TextMeshProUGUI>().textInfo.characterCount);
        //     //revealTextFlag = true;
        //     StartCoroutine(RevealText(dialogue, tmp));
        //     yield return new WaitUntil(()=>!revealTextFlag);
        //     //yield return new WaitForSeconds(3f);
        // }
        // dialogue.talker.gameObject.SetActive(false);
        #endregion
    }
    IEnumerator DialogueCoroutine2(Dialogue dialogue, bool atOnce=true, float typingSpeed =0.05f, float typingInterval= 1.5f)
    {
        //PlayerManager.instance.isTalking = true;
        dialogueFlag= true;
        dialogue.talker.GetChild(0).GetChild(0).gameObject.SetActive(true);
        

        for(int i=0; i<dialogue.sentences.Length;i++){
            
            var tmp = dialogue.talker.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            curSentence = dialogue.sentences[i];
            StartCoroutine(RevealText(dialogue, tmp, typingSpeed, typingInterval));
            yield return new WaitUntil(()=>!revealTextFlag);

        }

        dialogue.talker.GetChild(0).GetChild(0).gameObject.SetActive(false);
        
        dialogueFlag= false;

        
        PlayerManager.instance.isTalking = false;
    }
    IEnumerator RevealText(Dialogue dialogue, TextMeshProUGUI tmp, float typingSpeed, float typingInterval){
        revealTextFlag = true;
        int totalVisibleCharacters = curSentence.Length;
        WaitForSeconds _typingSpeed = new WaitForSeconds(typingSpeed);
        WaitForSeconds _typingInterval = new WaitForSeconds(typingInterval);
        //Debug.Log("토탈문자갯수 : " + totalVisibleCharacters);

        tmp.text = curSentence;
        tmp.maxVisibleCharacters = 0;
        for(int i=0; i<totalVisibleCharacters+1; i++){
            tmp.maxVisibleCharacters = i;
            
            yield return _typingSpeed;

        }
        yield return _typingInterval;
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

    IEnumerator SetRandomDialogueCoroutine(string dialogueText, Transform talker, float duration, int interval){
        talker.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = dialogueText;
        talker.GetChild(0).GetChild(0).gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        talker.GetChild(0).GetChild(0).gameObject.SetActive(false);


    }

    IEnumerator each ;
}
