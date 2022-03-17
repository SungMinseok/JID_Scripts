using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TranslateText : MonoBehaviour
{
    public int key = -1;
    // Start is called before the first frame update
    void Start(){
        //TryGetComponent<TextMeshProUGUI>(out curText);
        //curText = GetComponent<TextMeshProUGUI>();
        if(key!=-1) ApplyTranslation();
    }
    void OnEnable(){
        ApplyTranslation();
    }
    public void ApplyTranslation(){
        Text curText = GetComponent<Text>();
        string language = DBManager.instance.language;
        curText.text = CSVReader.instance.data_sysmsg[key]["text_"+language].ToString();
    }
}
