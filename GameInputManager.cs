using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
 
public static class GameInputManager
{
    static Dictionary<string, KeyCode> keyMapping;
    static string[] keyMaps = new string[3]
    {
        "Jump",
        "Interact",
        "Pet",
        //"Forward",
        //"Backward",
        //"Left",
        //"Right"
    };
    static KeyCode[] defaults = new KeyCode[3]
    {
        KeyCode.Space,
        KeyCode.E,
        KeyCode.R,
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
        if(localData.petKey==KeyCode.None){
            localData.petKey = KeyCode.R;
        }
        if(localData.AddDirtKey==KeyCode.None){
            localData.AddDirtKey = KeyCode.Q;
        }

        SetKeyMap("Jump",localData.jumpKey);
        SetKeyMap("Interact",localData.interactKey);
        SetKeyMap("Pet",localData.petKey);
        SetKeyMap("AddDirt",localData.AddDirtKey);
        
        
        MenuManager.instance.keyText_jump.text = DBManager.instance.localData.jumpKey.ToString();
        MenuManager.instance.keyText_interact.text = DBManager.instance.localData.interactKey.ToString();
        MenuManager.instance.keyText_adddirt.text = DBManager.instance.localData.AddDirtKey.ToString();
        //MenuManager.instance.keyText_pet.text = DBManager.instance.localData.petKey.ToString();
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
 