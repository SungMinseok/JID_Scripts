

#if alpha
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System; 
public class CheatManager : MonoBehaviour
{
    public InputField cheat;

    void Start()

    {

        cheat.onEndEdit.AddListener(delegate { GetCheat(); });

    }


    public void GetCheat()

    {

        if (cheat.text.Length > 0)

        {
           // DebugManager.instance.PrintDebug(cheat.text);

            string[] temp = cheat.text.Split('\x020');
            
            DebugManager.instance.PrintDebug(temp[0].ToString());
            switch(temp[0]){
                case "teleport":
                    switch(temp[1]){
                        case "0" :
                            Debug.Log("00");
                            //DebugManager.instance.PrintDebug("0번이동");
                            break;
                        case "1" :
                            Debug.Log("11");
                            //DebugManager.instance.PrintDebug("1번이동");
                            break;
                    }
                    break;
            }
            cheat.text = "";

        }

    }




    // public void GetCheat22(){
    //     string[] temp = cheat.text.Split('\x020');
    //     DebugManager.instance.PrintDebug(temp);
        
    //     DebugManager.instance.PrintDebug(temp[0]);
    //     switch(temp[0]){
    //         case "teleport":
    //             switch(temp[1]){
    //                 case "0" :
    //                     DebugManager.instance.PrintDebug("0번이동");
    //                     break;
    //                 case "1" :
    //                     DebugManager.instance.PrintDebug("1번이동");
    //                     break;
    //             }
    //             break;
    //     }
    // }
}
#endif