using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Dialogue
{
    public string comment;
    public Transform talker;//holder 넣자.
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


    public void SetDialogue(Dialogue[] dialogues)
    {
        //for(int i=0 ; i<dialogues.Length; i++){
            //if(!dialogueFlag1)
        StartCoroutine(DialogueCoroutine0(dialogues));
        //}
    }

    IEnumerator DialogueCoroutine0(Dialogue[] dialogues)
    {
        Debug.Log("대화 번들 갯수 : " + dialogues.Length);
        //dialogueFlag1= true;
        for(int i=0;i<dialogues.Length;i++){
            
            StartCoroutine(DialogueCoroutine1(dialogues[i]));
            Debug.Log("대화 번들 시작 : " + i);
            yield return new WaitUntil(()=>!dialogueFlag);

        }
        //dialogueFlag1= false;
    }

    IEnumerator DialogueCoroutine1(Dialogue dialogue)
    {
        dialogueFlag= true;
        //yield return null;
        dialogue.talker.GetChild(0).GetChild(0).gameObject.SetActive(true);
        
        //Debug.Log("문장갯수 : " + dialogue.sentences.Length);

        for(int i=0; i<dialogue.sentences.Length;i++){
            
            var tmp = dialogue.talker.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            curSentence = dialogue.sentences[i];
            StartCoroutine(RevealText(dialogue, tmp));
            yield return new WaitUntil(()=>!revealTextFlag);

        }

        dialogue.talker.GetChild(0).GetChild(0).gameObject.SetActive(false);
        
        dialogueFlag= false;
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

    IEnumerator RevealText(Dialogue dialogue, TextMeshProUGUI tmp){
        revealTextFlag = true;
        int totalVisibleCharacters = curSentence.Length;
        Debug.Log("토탈문자갯수 : " + totalVisibleCharacters);

        tmp.text = curSentence;
        tmp.maxVisibleCharacters = 0;
        for(int i=0; i<totalVisibleCharacters+1; i++){
            tmp.maxVisibleCharacters = i;
            
            yield return new WaitForSeconds(0.05f);

        }
        yield return new WaitForSeconds(3.0f);
        Debug.Log("문장종료");
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
}
