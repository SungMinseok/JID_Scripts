using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SerializeField]
public enum Action{
    Move,
    None
}
[SerializeField]
public class TransferManager : MonoBehaviour
{
    public Action action;
    
    // //오브젝트를 불러올 때 실행된다. 
    // void OnEnable(){
    //     action = new Action();
    // }
    // //Inspector GUI를 오버라이딩
    // public override void OnInspectorGUI ()
    // {
    //     //기존 GUI를 유지한다.
    //     //base.OnInspectorGUI ();​
    //     EditorGUILayout.ColorField ("Color Field", Color.white);
    //     EditorGUILayout.CurveField ("AnimationCurve Field", AnimationCurve.Linear (0, 3, 5, 5));
    //     EditorGUILayout.DelayedDoubleField ("DelayedDouble Field", 500);
    //     EditorGUILayout.HelpBox ("The helpbox", MessageType.Info);
    //     EditorGUILayout.IntField ("Int Field", 5);
    //     EditorGUILayout.Knob (new Vector2 (30f, 30f), 50f, 20f, 80f, "Knob", Color.black, Color.gray, true);
    //     EditorGUILayout.LabelField ("Label Field", "my label");
    //     EditorGUILayout.LayerField ("Layer Field", 0);
    //     EditorGUILayout.ObjectField ("Object Field", null, typeof(Sprite), true);
    //     EditorGUILayout.PasswordField ("Password Field", "mypassword");
    //     EditorGUILayout.Separator ();
    //     EditorGUILayout.Slider ("Slider", 20f, 10f, 90f);
    //     EditorGUILayout.Space ();
    //     EditorGUILayout.TagField ("Tag Field", "Player2");
    //     EditorGUILayout.TextArea ("this is a text area.");
    //     EditorGUILayout.TextField ("Text Field", "this is a text field.");
    //     EditorGUILayout.Toggle ("Toggle", true);
    //     EditorGUILayout.ToggleLeft ("ToggleLeft", false);
    //     EditorGUILayout.Vector2Field ("Vector2  field", new Vector2 (50f, 30f));
    // }
}
