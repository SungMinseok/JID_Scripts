using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#region Screen0 : 알번데기방 상자 비밀번호
public class PasswordBoxScript : MonoBehaviour
{
    public string answer = "7613";
    [Header("UI_Screen_0")]
    public Sprite[] numSprites;
    public Image[] passwordBox;
    [Header("Debug")]
    public int[] curPassword;
    public bool isPlaying;

    void Awake(){
        curPassword = new int[4];
    }
    void OnEnable(){
        isPlaying = true;
    }
    void OnDisable(){
        isPlaying = false;
    }
    public void ChangeNum(int index){

        if(!isPlaying) return;
        //int curNum = curPassword[index];
        if(curPassword[index]!=9){
            passwordBox[index].sprite = numSprites[++curPassword[index]];

        }
        else{
            curPassword[index] = 0;
            passwordBox[index].sprite = numSprites[0];

        }
        
        CheckNum();

    }
    public void SetRandomNum(){

    }
    public void CheckNum(){
        string temp = string.Join("", curPassword);
        Debug.Log(temp);
        if(temp.Equals(answer)){
            Debug.Log("Pass");
            InventoryManager.instance.AddItem(32);
            StartCoroutine(SuccessCoroutine());
        }
    }
    IEnumerator SuccessCoroutine(){
        SoundManager.instance.PlaySound("minigame_complete");
        yield return new WaitForSeconds(1f);
        LoadManager.instance.FadeOut();
        yield return new WaitForSeconds(2f);
        LoadManager.instance.FadeIn();
        InventoryManager.instance.AddItem(32);
    }
}
#endregion
