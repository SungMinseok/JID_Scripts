using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TranslateText : MonoBehaviour
{
    public int key = -1;
    // Start is called before the first frame update
    void Awake(){
        //TryGetComponent<TextMeshProUGUI>(out curText);
        //curText = GetComponent<TextMeshProUGUI>();


        Text curText = GetComponent<Text>();
        
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
        string language = DBManager.instance.language;
        curText.text = CSVReader.instance.data_sysmsg[key]["text_"+language].ToString();
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
