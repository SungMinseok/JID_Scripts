using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
 
public static class GameInputManager
{
    static Dictionary<string, KeyCode> keyMapping;
    static string[] keyMaps = new string[2]
    {
        "Jump",
        "Interact",
        //"Forward",
        //"Backward",
        //"Left",
        //"Right"
    };
    static KeyCode[] defaults = new KeyCode[2]
    {
        KeyCode.Space,
        KeyCode.E,
        //KeyCode.W,
        //KeyCode.S,
        //KeyCode.A,
        //KeyCode.D
    };
 
    static GameInputManager()
    {
        InitializeDictionary();
    }
 
    private static void InitializeDictionary()
    {
        keyMapping = new Dictionary<string, KeyCode>();

        var localData = DBManager.instance.localData;
        // for(int i=0;i<keyMaps.Length;++i)
        // {
        //     keyMapping.Add(keyMaps[i], defaults[i]);
        // }
        if(localData.jumpKey==KeyCode.None){
            localData.jumpKey = KeyCode.Space;
        }
        if(localData.interactKey==KeyCode.None){
            localData.interactKey = KeyCode.E;
        }

        SetKeyMap("Jump",localData.jumpKey);
        SetKeyMap("Interact",localData.interactKey);
        
        
        MenuManager.instance.keyText_jump.text = DBManager.instance.localData.jumpKey.ToString();
        MenuManager.instance.keyText_interact.text = DBManager.instance.localData.interactKey.ToString();
    }
 
    public static void SetKeyMap(string keyMap,KeyCode key)
    {
        //if (!keyMapping.ContainsKey(keyMap))
        //    throw new ArgumentException("Invalid KeyMap in SetKeyMap: " + keyMap);
        keyMapping[keyMap] = key;
    }
    public static string ReadKey(string keyMap){
        if (!keyMapping.ContainsKey(keyMap)) return "";
        else return keyMapping[keyMap].ToString();
    }
 
    public static bool GetKeyDown(string keyMap)
    {
        return Input.GetKeyDown(keyMapping[keyMap]);
    }
    public static bool GetKey(string keyMap)
    {
        return Input.GetKey(keyMapping[keyMap]);
    }
    // public static KeyCode GetCurrentKeyCode(){
    //     if (!keyMapping.ContainsKey(keyMap)){

    //     }
    //     return
    // }
}
 