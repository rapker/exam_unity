using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

#if UNITY_EDITOR
using UnityEditorInternal;

public class AnimationSyncToolWindow : EditorWindow {

    enum EAnimType
    {
        EAnimType_None = 0,
        EAnimType_Animation,
        EAnimType_Animator
    }

#region Open / Close / Get
    static AnimationSyncToolWindow _instance = null;
    static public AnimationSyncToolWindow Get
    {
        get
        {
            return _instance;
        }
    }

    [MenuItem("CustomEditor/SkillSyncTool")]
    static void OpenAnimationSyncTool()
    {
        AnimationSyncToolWindow tool = EditorWindow.GetWindow(typeof(AnimationSyncToolWindow), false, "AnimationSync") as AnimationSyncToolWindow;
        tool.minSize = new Vector2(900, 637);
        Rect pos = tool.position;
        pos.x = 0;
        pos.y = 0;
        pos.width = 900;
        pos.height = 637;
        tool.position = pos;
    }

    public void OnEnable()
    {
        _instance = this;
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
    Rect _windowActorInfo;
    Rect _windowSyncView;
    Rect _windowSyncEdit;
    void OnGUI()
    {
        BeginWindows();
        _windowActorInfo = GUILayout.Window(1, _windowActorInfo, DoWindow_ActorInfo, "Actor Info", GUILayout.Width(300), GUILayout.Height(100));

        _windowSyncView = GUILayout.Window(2, _windowSyncView, DoWindow_SyncView, "Sync View", GUILayout.Width(600), GUILayout.Height(430));
        _windowSyncView.x = _windowActorInfo.width;

        _windowSyncEdit = GUILayout.Window(3, _windowSyncEdit, DoWindow_SyncEdit, "Sync Edit", GUILayout.Width(600), GUILayout.Height(205));
        _windowSyncEdit.x = _windowActorInfo.width;
        _windowSyncEdit.y = _windowSyncView.height;
        EndWindows();
    }
#endregion BaseWindowLayout

#region DoWindow_SelectActor

    GameObject _CharacterPrefab = null;
    Animation _Animation = null;
    Animator _Animator = null;

    EAnimType _eAnimType = EAnimType.EAnimType_None;

    float fMenuHeight = 20.0f;
    float fLabelWidth = 140.0f;
    float fValueWidth = 200.0f;

    void DoWindow_ActorInfo(int id)
    {
        GameObject charPrefab;

        GUILayout.BeginVertical("Box");
        {
            GUILayout.BeginHorizontal(GUILayout.Height(fMenuHeight));
            {
                EditorGUILayout.LabelField("Actor Prefab", GUILayout.Width(fLabelWidth));
                charPrefab = EditorGUILayout.ObjectField(_CharacterPrefab, typeof(GameObject), false, GUILayout.Width(fValueWidth)) as GameObject;
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        if (_CharacterPrefab != charPrefab)
            _CharacterPrefab = charPrefab;

        if (null == _CharacterPrefab)
        {
            _Animation = null;
            _Animator = null;
            _eAnimType = EAnimType.EAnimType_None;
            EditorGUILayout.LabelField("Please select Actor Prefab");
            return;
        }

        _Animation = _CharacterPrefab.GetComponent<Animation>();
        _Animator = _CharacterPrefab.GetComponent<Animator>();

        if (null == _Animation && null == _Animator)
        {
            EditorGUILayout.LabelField("Not Found Anim Component In Actor Prefab");
            return;
        }


        GUILayout.BeginVertical("Box");
        {
            if (_Animation)
            {
                _eAnimType = EAnimType.EAnimType_Animation;

                DrawAnimationBaseInfo();
                DrawAnimationStateList();
            }
            else
            {
                _eAnimType = EAnimType.EAnimType_Animator;

                DrawAnimatorBaseInfo();
                DrawAnimatorLayers();
            }
        }
        GUILayout.EndVertical();




        //GUILayout.Label("Select Actor", EditorStyles.boldLabel);
        //myString = EditorGUILayout.TextField("Select Actor Prefab", myString);

        //groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        //{
        //    myBool = EditorGUILayout.Toggle("Toggle", myBool);
        //    myFloat = EditorGUILayout.Slider("Slider", myFloat, 0, 1);
        //}
        //EditorGUILayout.EndToggleGroup();


        //GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
        //{
        //    GUILayout.FlexibleSpace();
        //    searchString = GUILayout.TextField(searchString, GUI.skin.FindStyle("ToolbarSeachTextField"));
        //    if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
        //    {
        //        // Remove focus if cleared
        //        searchString = "";
        //        GUI.FocusControl(null);
        //    }
        //    DrawState();
        //}
        //GUILayout.EndHorizontal();
    }
#endregion DoWindow_SelectActor


#region DrawAnimation
    void DrawAnimationBaseInfo()
    {
        GUILayout.Label("Animation Info");
    }
    void DrawAnimationStateList()
    {

    }
#endregion DrawAnimation


    #region DrawAnimator
    void DrawAnimatorBaseInfo()
    {
        GUILayout.BeginVertical("Box");
        {
            GUILayout.Label("Animator Info");
        }
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal(GUILayout.Height(fMenuHeight));
        {
            EditorGUILayout.LabelField("Controller", GUILayout.Width(fLabelWidth));
            EditorGUILayout.ObjectField(_Animator.runtimeAnimatorController, typeof(AnimatorController), false, GUILayout.Width(fValueWidth));
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(GUILayout.Height(fMenuHeight));
        {
            EditorGUILayout.LabelField("Avatar", GUILayout.Width(fLabelWidth));
            EditorGUILayout.ObjectField(_Animator.avatar, typeof(Avatar), false, GUILayout.Width(fValueWidth));
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(GUILayout.Height(fMenuHeight));
        {
            EditorGUILayout.Toggle("Apply Root Motion", _Animator.applyRootMotion, GUILayout.Width(fLabelWidth));
            GUILayout.Label("", GUILayout.Width(fValueWidth));
        }
        GUILayout.EndHorizontal();
    }

    int curIndexLayer_ForAnimator = 0;
    int curStateIndex_ForAnimator = 0;
    string[] dispLayerNames = null;

    float animationDuration = 1;

    Vector2 scrollPos_State;

    private List<AnimatorControllerLayer> listLayer = new List<AnimatorControllerLayer>();

    void DrawAnimatorLayers()
    {
        GUILayout.BeginVertical();
        {
            GUILayout.BeginVertical("Box");
            AnimatorController controller = _Animator.runtimeAnimatorController as AnimatorController;
            if (controller)
            {
                listLayer.Clear();
                GetDisplayLayerNames(controller);
                GUILayout.BeginHorizontal(GUILayout.Height(fMenuHeight));
                {
                    EditorGUILayout.LabelField("Layer", GUILayout.Width(fLabelWidth));
                    curIndexLayer_ForAnimator = EditorGUILayout.Popup(curIndexLayer_ForAnimator, dispLayerNames, GUILayout.Width(fValueWidth));
                }
                GUILayout.EndHorizontal();
                dispLayerNames = null;

                DrawAnimatorStateList(controller);
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndVertical();
    }

    void GetDisplayLayerNames(AnimatorController controller)
    {
        dispLayerNames = new string[controller.layerCount];

        for (int iLayer = 0; iLayer < controller.layerCount; iLayer++)
        {
            listLayer.Add(controller.GetLayer(iLayer));
            dispLayerNames[iLayer] = controller.GetLayer(iLayer).name;
        }
    }

    void DrawAnimatorStateList(AnimatorController controller)
    {
        if (listLayer.Count <= 0)
            return;

        for (int i = 0; i < listLayer[curIndexLayer_ForAnimator].stateMachine.stateCount; i++ )
        {
            State state = listLayer[curIndexLayer_ForAnimator].stateMachine.GetState(i);
        }
            

        //scrollPos_State = GUILayout.BeginScrollView(scrollPos_State);

        //if (arrayGUIState != null)
        //{
        //    int selectedStateIndex = GUILayout.SelectionGrid(curStateIndex_ForAnimator, arrayGUIState, 1);

        //    if (selectedStateIndex != curStateIndex)
        //    {
        //        curStateIndex = selectedStateIndex;

        //        OnStateChange();

        //    }
        //}
        //GUILayout.EndScrollView();
    }

    //void GetDisplayLayerNames(AnimatorController controller)
    //{
        //Motion curMotion = null;
        //if (curState)
        //    curMotion = curState.GetMotion();

        //arrayGUIState = new GUIContent[dicStateInfo[curIndexLayer].Count];
        //for (int i = 0; i < dicStateInfo[curIndexLayer].Count; i++)
        //{
        //    State state = dicStateInfo[curIndexLayer][i].state;

        //    arrayGUIState[i] = new GUIContent();
        //    arrayGUIState[i].text = state.name;
        //    if (state.GetMotion() != null)
        //    {
        //        if (curMotion == state.GetMotion())
        //            arrayGUIState[i].text = state.name + " [clip: " + state.GetMotion().name + "]";

        //        arrayGUIState[i].tooltip = "Clip : " + state.GetMotion().name;
        //    }
        //    else
        //    {
        //        if (curState && curState == state)
        //            arrayGUIState[i].text = state.name + " [no clip]";
        //    }

        //}
    //}
#endregion DrawAnimator

#region DoWindow_SyncView
    void DoWindow_SyncView(int id)
    {
        GUILayout.Label("Sync View", EditorStyles.boldLabel);
    }
#endregion DoWindow_SyncView

#region DoWindow_SyncEdit
    void DoWindow_SyncEdit(int id)
    {
        if (_eAnimType == EAnimType.EAnimType_None)
        {
            GUILayout.Label("...", EditorStyles.boldLabel);
            return;
        }
        GUILayout.Label("Edit Sync Data", EditorStyles.boldLabel);
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

#endif //UNITY_EDITOR