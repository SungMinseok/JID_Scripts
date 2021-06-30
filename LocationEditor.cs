using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

// [CustomEditor(typeof(Location))]
// public class LocationEditor : Editor{
//     public Location selected;

//     private void OnEnable(){
//         if(AssetDatabase.Contains(target)){
//             selected = null;
//         }
//         else{
//             selected = (Location)target;
//         }
//     }

//     // public override void OnInspectorGUI(){
//     //     // if(selected == null) return;
//     //     // EditorGUILayout.Space();
//     //     // EditorGUILayout.LabelField("****** 몬스터 정보 입력툴 ******");

//     // }

// }