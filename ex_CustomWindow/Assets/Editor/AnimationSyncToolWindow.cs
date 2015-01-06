using UnityEngine;
using System.Collections;
using UnityEditor;

public class AnimationSyncToolWindow : EditorWindow {

    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    string searchString = "search";

#region Open / Close / Get
    static AnimationSyncToolWindow m_instance;
    static public AnimationSyncToolWindow Get
    {
        get
        {
            return m_instance;
        }
    }

    [MenuItem("CustomEditor/SkillSyncTool")]
    static void OpenAnimationSyncTool()
    {
        AnimationSyncToolWindow tool = EditorWindow.GetWindow(typeof(AnimationSyncToolWindow), false, "AnimationSync") as AnimationSyncToolWindow;
        tool.minSize = new Vector2(1300, 900);
        Rect pos = tool.position;
        pos.x = 0;
        pos.y = 0;
        tool.position = pos;
    }

    public void OnEnable()
    {
        m_instance = this;
    }
    public void OnDisable()
    {
        DestroyAnimationSyncTool();
    }
    void Init()
    {
        
    }
    void DestroyAnimationSyncTool()
    {

    }

#endregion Open / Close / Get

#region BaseWindowLayout
    Rect windowActorInfo;
    Rect windowAnimList;
    Rect windowSyncView;
    Rect windowSyncEdit;
    void OnGUI()
    {
        BeginWindows();
        windowActorInfo = GUILayout.Window(1, windowActorInfo, DoWindow_ActorInfo, "1. Actor Info", GUILayout.Width(300), GUILayout.Height(100));

        windowAnimList = GUILayout.Window(2, windowAnimList, DoWindow_AnimList, "2. AnimList", GUILayout.Width(windowActorInfo.width), GUILayout.Height(500));
        windowAnimList.y = windowActorInfo.height;

        windowSyncView = GUILayout.Window(3, windowSyncView, DoWindow_SyncView, "3. Sync View", GUILayout.Width(600), GUILayout.Height(500));
        windowSyncView.x = windowActorInfo.width;

        windowSyncEdit = GUILayout.Window(4, windowSyncEdit, DoWindow_SyncEdit, "4. Sync Edit", GUILayout.Width(600), GUILayout.Height(200));
        windowSyncEdit.x = windowActorInfo.width;
        windowSyncEdit.y = windowSyncView.height;
        EndWindows();
    }
#endregion BaseWindowLayout

#region DoWindow_SelectActor
    void DoWindow_ActorInfo(int id)
    {
        GUILayout.Label("Select Actor", EditorStyles.boldLabel);
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
#endregion DoWindow_SelectActor

#region DoWindow_AnimList
    void DoWindow_AnimList(int id)
    {
        GUILayout.Label("Select Animation", EditorStyles.boldLabel);
    }
#endregion

    #region DoWindow_SyncView
    void DoWindow_SyncView(int id)
    {
        GUILayout.Label("Sync View", EditorStyles.boldLabel);
    }
#endregion DoWindow_SyncView

#region DoWindow_SyncEdit
    void DoWindow_SyncEdit(int id)
    {
        GUILayout.Label("Sync Edit", EditorStyles.boldLabel);
    }
#endregion

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