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
    public List<Dictionary<string,object>> data_map;
    public List<Dictionary<string,object>> data_story;
    public List<Dictionary<string,object>> data_coupon;
    public List<Dictionary<string,object>> data_quest;
    [SerializeField]
    public List<Dictionary<string,object>> data_command;

    public int itemAmount;
    void Awake() {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        data_dialogue = CSVReader.Read ("data_dialogue");
        data_select = CSVReader.Read ("data_select");
        data_item = CSVReader.Read ("data_item");
        data_collection = CSVReader.Read ("data_collection");
        data_sysmsg = CSVReader.Read ("data_sysmsg");
        data_map = CSVReader.Read ("data_map");
        data_story = CSVReader.Read ("data_story");
        data_coupon = CSVReader.Read ("data_coupon");
        data_quest = CSVReader.Read ("data_quest");
        data_command = CSVReader.Read ("data_command");
        
        itemAmount = data_item.Count;
        //print(data_dialogue[0]["text_kr"]);

        // foreach(Dictionary<string,object> a in data_command){
        //     foreach(string key in a.Keys){
        //         Debug.Log(key);
        //     }
        //     foreach(string value in a.Values){
        //         Debug.Log(value);
        //     }
        // }
        

        var tempDic = data_command.Find(x => x["arguments"].ToString() == "개미ID");
        Debug.Log(tempDic["cheatDescription"]);



    }
    // void Start(){
    //     for(int i=0;i<200;i++){
    //         if(data_story[i]["kr"].ToString()==string.Empty){
    //             continue;
    //         }
    //         Debug.Log(data_story[i]["kr"]);
    //     }
    // }
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
        //string curLanguage = "text_"+DBManager.instance.language;
        switch(target){
            case "dialogue" :
                if(data_dialogue.Count>index){

                    result = data_dialogue[index]["text_"+DBManager.instance.language];
                    string resultString = result.ToString();

                    
                    // switch(DBManager.instance.language){
                    //     case "kr":
                    //     case "en":
                            if(!resultString.Contains("</color>")){
                                //한 줄 최소 텍스트 갯수(공백 포함)
                                int dialogueMaxCountPerRow = 18;

                                switch(DBManager.instance.language){
                                    case "kr" : 
                                        dialogueMaxCountPerRow = 18;
                                        break;
                                    case "jp" : 
                                        dialogueMaxCountPerRow = 15;
                                        break;
                                    case "en" : 
                                        dialogueMaxCountPerRow = 25;
                                        break;
                                    default :
                                        break;
                                }

                                var quotient = resultString.Length / dialogueMaxCountPerRow ;

                                if(quotient == 0){
                                    break;
                                }

                                int add = 0;

                                for(int i=1;i<=quotient;i++){
                                    int startIndex = dialogueMaxCountPerRow * i + add;
                                    add = 0;
                                    //단어가 중간에 안잘리도록 함.
                                    if(DBManager.instance.language=="en"||DBManager.instance.language=="kr"){

                                        while( startIndex + add < resultString.Length && resultString[startIndex + add]!=' '){
                                            add++;
                                        }
                                    }
                                    if(startIndex + add >= resultString.Length) break;
                                    resultString = resultString.Insert(startIndex + add + 1,"\n");

                                }


                            }
                    //         break;
                    //     case "jp":
                    //         break;
                    // }
                                


                    result = resultString;


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

                    result = data_select[index]["text_"+DBManager.instance.language];
                }
                break;    
            case "sysmsg" :
                if(data_sysmsg.Count>index){
                    result = data_sysmsg[index]["text_"+DBManager.instance.language];
                }
                break;
            case "map" :
                if(data_map.Count>index){
                    result = data_map[index]["text_"+DBManager.instance.language];
                }
                break;
            // case "story" :
            //     if(data_story.Count>index){
            //         result = data_story[index][curLanguage];
            //     }
                //break;
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