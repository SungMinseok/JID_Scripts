using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
 
public class CSVReader : MonoBehaviour
{
    public static CSVReader instance;
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";      //@"\r\n|\n\r|\n|\r"
    static char[] TRIM_CHARS = { '\"' };
 
    List<Dictionary<string,object>> data_dialogue;// = CSVReader.Read ("data_dialogue");
    List<Dictionary<string,object>> data_select;// = CSVReader.Read ("data_select");
    public List<Dictionary<string,object>> data_item;// = CSVReader.Read ("data_select");
    public List<Dictionary<string,object>> data_collection;
    public List<Dictionary<string,object>> data_sysmsg;

    public int itemAmount;
    void Awake() {
        instance = this;

        data_dialogue = CSVReader.Read ("data_dialogue");
        data_select = CSVReader.Read ("data_select");
        data_item = CSVReader.Read ("data_item");
        data_collection = CSVReader.Read ("data_collection");
        data_sysmsg = CSVReader.Read ("data_sysmsg");
        
        itemAmount = data_item.Count;
        //print(data_dialogue[0]["text_kr"]);
    }
    public static List<Dictionary<string, object>> Read(string file)
    {
        var list = new List<Dictionary<string, object>>();
        TextAsset data = Resources.Load (file) as TextAsset;
 
        var lines = Regex.Split (data.text, LINE_SPLIT_RE);
 
        if(lines.Length <= 1) return list;
 
        var header = Regex.Split(lines[0], SPLIT_RE);
        for(var i=1; i < lines.Length; i++) {
 
            var values = Regex.Split(lines[i], SPLIT_RE);
            if(values.Length == 0 ||values[0] == "") continue;
 
            var entry = new Dictionary<string, object>();
            for(var j=0; j < header.Length && j < values.Length; j++ ) {
                string value = values[j];
                value = value.Replace("\\n","\n");
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                object finalvalue = value;
                int n;
                float f;
                if(int.TryParse(value, out n)) {
                    finalvalue = n;
                } else if (float.TryParse(value, out f)) {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }
            list.Add (entry);
        }
        return list;
    }
    public string GetIndexToString(int index, string target){
        //Dialogue
        //object temp = index;
        object result = "null_text";
        string curLanguage = "text_kr";
        switch(target){
            case "dialogue" :
                if(data_dialogue.Count>index){

                    result = data_dialogue[index][curLanguage];
//                    Debug.Log(index+":"+result);
                }
        //Debug.Log(data_dialogue[index][curLanguage]);
                //if(data_dialogue[index].TryGetValue(index.ToString(), out result)){
                    
        //Debug.Log(data_dialogue[index][curLanguage]);

               // }
                // else{
                    
                // Debug.Log("해당 아이디 없음");
                // }
                break;            
            case "select" :
                if(data_select.Count>index){

                    result = data_select[index][curLanguage];
                }
                break;    
            case "sysmsg" :
                if(data_select.Count>index){
                    result = data_sysmsg[index][curLanguage];
                }
                break;
            default : 
                Debug.Log("해당 아이디 없음");
                break;
        }
        // if(index >=100000 && index < 1000000){
        //     for(int i=0;i<data_dialogue.Count;i++){
        //         if(data_dialogue[i]["ID"]==temp){
        //             result = data_dialogue[(int)temp][target];     
        //             break;
        //         }
        //         Debug.Log("해당 아이디 없음");
        //         //result = "null_text";

        //     }
        // }
        // // else if(index >=10000 && index < 100000){
        // // }
        // else{
        //     //result = "null_text";
        // }
        return result.ToString();
    }
    // public string ApplyText(int index){
    //     object value;
    //     string result;
    //     //Debug.Log(index);



    //     //Dialogue
    //     if(index >=100000 && index < 1000000){
    //         result = data_dialogue[index]["text_kr"].ToString();

    //         if(data_dialogue.TryGetValue(index, out value)){
    //            result = value.text_kr;
    //         }
    //         else{
    //            DebugManager.instance.PrintDebug(index+"번 대화 데이터 존재하지 않음.");
    //            result = "null_text";
    //         }
    //         //result = dictionaryDialogueText[index].text_kr;
    //     }
    //     else if(index >=10000 && index < 100000){
            
    //         if(dictionarySelectText.TryGetValue(index, out value)){
    //             result = value.text_kr;
    //         }
    //         else{
    //             DebugManager.instance.PrintDebug(index+"번 선택지 데이터 존재하지 않음.");
    //             result = "null_text";
    //         }
    //     }
    //     else{
    //         result = "null_text";
    //     }

    //     return result;
    // }
}