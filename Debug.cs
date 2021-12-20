using System;
using System.Diagnostics;
using UnityEngine;

public static class Debug
{
    [Conditional("UNITY_EDITOR")]
    public static void Log(object message) => 
        UnityEngine.Debug.Log(message);

}
