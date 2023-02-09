using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingGuideSlot : MonoBehaviour
{
    public int trueEndingID;
    public int endingMapID;
    public int endingNameID;
    public TranslateText endingMapText;
    public TranslateText endingNameText;
    public Text endingSlotIndexText;
    public GameObject clearObject;

    // Start is called before the first frame update
    void Awake()
    {
        //if(DBManager.instance.EndingCollectionOver())
        // endingMapText.key = endingMapID;
        // endingNameText.key = endingNameID;
        endingSlotIndexText.text = (transform.GetSiblingIndex()+1).ToString();
    }
    void OnEnable(){
        
        if(DBManager.instance.GetStateTrueEndingCollection(trueEndingID)){
            clearObject.SetActive(true);
        }
        else{
            clearObject.SetActive(false);
        }
    }

}
