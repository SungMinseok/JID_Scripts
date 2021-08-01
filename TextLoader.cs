﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
 
public class TextLoader : MonoBehaviour {

    public static TextLoader instance;
    public class TextFormat{
        public int id;
        public string text_kr;
        public string text_en;
    }
 
    //데이터를 담을 딕셔터리 선언
    public Dictionary<int, TextFormat> dictionaryDialogueText;
    public Dictionary<int, TextFormat> dictionarySelectText;
 
    // //엑셀데이터에서 만든 변수 선언
    // public int id;
    // public string text;
 
    //private int index;
    void Awake(){
        instance = this;
    }
 
    void Start () {
        
        dictionaryDialogueText = new Dictionary<int, TextFormat>();
        dictionarySelectText = new Dictionary<int, TextFormat>();
 
        var dialogueData = Resources.Load("Data/Dialogue_data") as TextAsset;
        var selectData = Resources.Load("Data/Select_data") as TextAsset;
 
        //상단에 using Newtonsoft.Json; 추가하셔야합니다.
        var dialogueDataList = JsonConvert.DeserializeObject<List<TextFormat>>(dialogueData.text);
        var selectDataList = JsonConvert.DeserializeObject<List<TextFormat>>(selectData.text);
 
        //각각의 내용들을 딕셔너리에 담음
        foreach(var data in dialogueDataList)
        {
            dictionaryDialogueText.Add(data.id, data);
        }
        foreach(var data in selectDataList)
        {
            dictionarySelectText.Add(data.id, data);
        }
    }

    public string ApplyText(int index){
        TextFormat value;
        string result;

        if(index >=100000 && index < 1000000){
            if(dictionaryDialogueText.TryGetValue(index, out value)){
                result = value.text_kr;
            }
            else{
                DebugManager.instance.PrintDebug(index+"번 대화 데이터 존재하지 않음.");
                result = "null_text";
            }
        }
        else if(index >=10000 && index < 100000){
            
            if(dictionarySelectText.TryGetValue(index, out value)){
                result = value.text_kr;
            }
            else{
                DebugManager.instance.PrintDebug(index+"번 선택지 데이터 존재하지 않음.");
                result = "null_text";
            }
        }
        else{
            result = "null_text";
        }

        return result;
    }
 
    //텍스트를 집어넣는 메서드
    public void ShowText()
    {
        //label.text = this.dicTextLoadText[dex].text;
        //Debug.Log(this.dicTextLoadText[dex].text);
        //this.dex++;
    }
 
    //각각의 텍스트가 잘 나오는지 확인할 테스트용 버튼
    // private void OnGUI()
    // {
    //     if (GUILayout.Button("다음 문자"))
    //     {
    //         this.ShowText();
    //     }
    // }
    // void Update(){
    //     if(Input.GetKeyDown(KeyCode.F2)){
            
    //         this.ShowText();
    //     }
    // }
}