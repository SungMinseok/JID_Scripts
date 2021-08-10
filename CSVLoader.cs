using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVLoader : MonoBehaviour
{
    List<Dictionary<string,object>> data_dialogue;// = CSVReader.Read ("data_dialogue");
    List<Dictionary<string,object>> data_select;// = CSVReader.Read ("data_select");
    List<Dictionary<string,object>> data_item;// = CSVReader.Read ("data_select");
    void Awake() {
        //List<Dictionary<string,object>> data = CSVReader.Read ("example");
        data_dialogue = CSVReader.Read ("data_dialogue");
        data_select = CSVReader.Read ("data_select");
        data_item = CSVReader.Read ("data_select");

        //Debug.Log("?");
        // for(var i=0; i < data.Count; i++) {
        //     Debug.Log(i);
        // Debug.Log ("name " + data[i]["name"] + " " +
        //            "age " + data[i]["age"] + " " +
        //            "speed " + data[i]["speed"] + " " +
        //            "desc " + data[i]["description"]);
        // }
        // for(var i=0; i < data_dialogue.Count; i++) {
        //     Debug.Log ("ID " + data_dialogue[i]["ID"] + " " +
        //            "text_kr " + data_dialogue[i]["text_kr"]);
        // }

        // for(var i=0;i<data_dialogue.Count;i++){

        print(data_dialogue[0]["text_kr"]);
        // }
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
