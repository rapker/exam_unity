using UnityEngine;
using System.Collections;
using UnityEditor;

public class CustomWindow : EditorWindow {

    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    string searchString = "search";

    Rect windowRect;
    Rect windowRect2;

	[MenuItem("CustomEditor/SkillSyncTool")]
    static void Init()
    {
        CustomWindow tool = (CustomWindow)EditorWindow.GetWindow(typeof(CustomWindow));
    }

    void OnGUI()
    {
        BeginWindows();
        windowRect = GUILayout.Window(1, windowRect, DoWindow, "Palette", GUILayout.Height(600));
        EndWindows();
    }

    void DoWindow( int id )
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        myString = EditorGUILayout.TextField("Text Field", myString);


        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        {
            myBool = EditorGUILayout.Toggle("Toggle", myBool);
            myFloat = EditorGUILayout.Slider("Slider", myFloat, 0, 1);
        }
        EditorGUILayout.EndToggleGroup();


        GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
        {
            GUILayout.FlexibleSpace();
            searchString = GUILayout.TextField(searchString, GUI.skin.FindStyle("ToolbarSeachTextField"));
            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
            {
                // Remove focus if cleared
                searchString = "";
                GUI.FocusControl(null);
            }
            //DrawState();
        }
        GUILayout.EndHorizontal();
    }

    void DrawState()
    {
        GUILayout.BeginVertical(GUILayout.Width(200));
        {
            GUILayout.BeginVertical("Box");
            GUILayout.Label(" States ");
            GUILayout.EndVertical();
        }
        GUILayout.EndVertical();
    }
}