using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//#if UNITY_EDITOR
//using UnityEditor;

//[CustomEditor(typeof(UISettingsObject))]
//class UISettingsObjectEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        if(GUILayout.Button("Play Show"))
//        {
//            (target as UISettingsObject).PlayAnimShow();
//        }
//        if(GUILayout.Button("Play Hide"))
//        {
//            (target as UISettingsObject).PlayAnimHide();
//        }
//        base.OnInspectorGUI();
//    }
//}
//#endif

public class UISettingsObject : MonoBehaviour
{

    [SerializeField] Image img;

    public Sprite stateOn, stateOff;

    private void Start()
    {
        img.sprite = SettingsManager.Instance.IsSettingEnabled("SoundsEnabled") ? stateOn : stateOff;
        //FAI ANIMAZIONI DI TOGGLE, SHOW E HIDE
    }

    public void ToggleSprite()
    {
        img.sprite = img.sprite == stateOff ? stateOn : stateOff;
    }
}
