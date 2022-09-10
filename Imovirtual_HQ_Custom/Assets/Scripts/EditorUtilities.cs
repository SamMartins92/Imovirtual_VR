/// ////////////////////////////////////////////////////////////////////
///                                       _           
///               _ __ ___   __ _ ___ ___(_)_   _____ 
///              | '_ ` _ \ / _` / __/ __| \ \ / / _ \
///              | | | | | | (_| \__ \__ \ |\ V /  __/
///              |_| |_| |_|\__,_|___/___/_| \_/ \___|
///                            MASSIVE UNITY FRAMEWORK
/// 
/// @summary 
/// /   MASSIVE > Helpers > SerialThread
///     
/// @description
/// /   Provides a implementation of AbstractSerialThread class to 
/// /   communicate with Serial Controller Devices
/// 
/// @contact
/// /   maxbessa@utad.pt
///     
/// ////////////////////////////////////////////////////////////////////

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MASSIVE
{
    public class EditorUtilities
    {
        public static GUIStyle BoldText
        {
            get
            {
                return new GUIStyle() { fontStyle = FontStyle.Bold, fontSize = 12 };
            }
        }

        public static void Text(string text, GUIStyle textStyle, bool center)
        {

            if (center)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(text, textStyle);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

            }
            else
            {


                GUILayout.Label(text, textStyle);
                GUI.backgroundColor = Color.white;

            }

            GUI.backgroundColor = Color.white;
        }

        public static void Text(string text, GUIStyle textStyle, bool center, int width)
        {

            if (center)
            {


                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(text, textStyle, GUILayout.Width(width));
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

            }
            else
            {


                GUILayout.Label(text, textStyle, GUILayout.Width(width));
                GUI.backgroundColor = Color.white;

            }

            GUI.backgroundColor = Color.white;
        }

        public static void Header(string text)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(text, EditorStyles.boldLabel);
        }

        public static void Separator()
        {
            GUI.color = Color.white;
            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(2) });
            GUI.color = Color.white;
        }
    }
}
#endif