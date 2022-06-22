using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum TextType{
    system,item,map,collection
}
public class TranslateText : MonoBehaviour
{
    public TextType textType = TextType.system;
    public int key = -1;
    public bool isEndingGuideItemMapName;
    [Header("키 튜토리얼 번역")]
    public bool isKeyTutorial;
    public string keyMap;
    // Start is called before the first frame update
    void Awake(){
        //TryGetComponent<TextMeshProUGUI>(out curText);
        //curText = GetComponent<TextMeshProUGUI>();


        Text curText = GetComponent<Text>();

        if(isEndingGuideItemMapName){
            key = GetComponentInParent<EndingGuideItemSlot>().itemGetMapID;
        }
        
        if(key!=-1){
            ApplyTranslation(curText);
            ApplyFont(curText);
            
        }

    }
    void OnEnable(){
        Text curText = GetComponent<Text>();
        ApplyTranslation(curText);
        ApplyFont(curText);
    }
    public void ApplyTranslation(Text curText){

        if(isKeyTutorial){
            if(keyMap == string.Empty) keyMap = "Interact";
            string keyString = GameInputManager.ReadKey(keyMap);
            Debug.Log(keyString);
            curText.text = string.Format(CSVReader.instance.GetIndexToString(key,"sysmsg"), keyString);
            return;
        }


        string language = DBManager.instance.language;
        switch(textType){
            case TextType.system :
                curText.text = CSVReader.instance.data_sysmsg[key]["text_"+language].ToString();
                break;
            case TextType.item :
                curText.text = CSVReader.instance.data_item[key]["name_"+language].ToString();
                break;
            case TextType.map :
                curText.text = CSVReader.instance.data_map[key]["text_"+language].ToString();
                break;
            case TextType.collection :
                curText.text = CSVReader.instance.data_collection[key]["name_"+language].ToString();
                break;
        }
    }
    public void ApplyFont(Text curText){
        string curLang = DBManager.instance.language;
        switch(curLang){
            case "kr" :
            case "en" :
                curText.font = MenuManager.instance.fonts[0];
                break;
                
            case "jp" :
                curText.font = MenuManager.instance.fonts[1];
                break;
        }
    }
}
