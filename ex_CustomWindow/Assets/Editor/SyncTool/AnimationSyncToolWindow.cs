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
        tool.minSize = new Vector2(1100, 700);
        Rect pos = tool.position;
        pos.x = 0;
        pos.y = 0;
        pos.width = 1100;
        pos.height = 700;
        tool.position = pos;
    }

    public void OnEnable()
    {
        _instance = this;
    }
    public void OnDisable()
    {
        SyncToolPanel_Animation.Release();
        SyncToolPanel_Animator.Release();
        SyncToolPanel_Preview.Release();

        DestroyAnimationSyncTool();
    }
    void DestroyAnimationSyncTool()
    {

    }

#endregion Open / Close / Get

#region BaseWindowLayout
    Rect _windowActorInfo;
    public Rect _windowSyncView;

    void OnGUI()
    {
        BeginWindows();
        _windowActorInfo = GUILayout.Window(1, _windowActorInfo, DoWindow_ActorInfo, "Actor Info", GUILayout.Width(300), GUILayout.Height(100));

        _windowSyncView = GUILayout.Window(2, _windowSyncView, DoWindow_SyncView, "Sync View", GUILayout.Width(600), GUILayout.Height(430));
        _windowSyncView.x = _windowActorInfo.width;

        SyncToolPanel_Preview.Get()._rectPreviewForEvent.x = _windowSyncView.x;
        SyncToolPanel_Preview.Get()._rectPreviewForEvent.y = _windowSyncView.y;

        EndWindows();

        ProcessInput();
    }
#endregion BaseWindowLayout

    bool isDraggingOnRender = false;
    bool isDraggingOnEventTimeline = false;
    void ProcessInput()
    {
        //Event evt = Event.current;

        //if (evt.type == EventType.ContextClick)
        //{
        //    Vector2 mousePos = evt.mousePosition;
        //    if (rectEventTimeLine.Contains(mousePos))
        //    {
        //        float normalizedPosX = (mousePos.x - (rectEventTimeLine.xMin + 16)) / (float)(rectEventTimeLine.width - 32);
        //        normalizedPosX = Mathf.Clamp(normalizedPosX, 0, 1);
        //        ContextParams[(int)ContextType.NEW_EVENT].paramFloat0 = normalizedPosX;

        //        GenericMenu menu = new GenericMenu();

        //        menu.AddItem(new GUIContent("New Animation Event"), false, ContextCallback, ContextType.NEW_EVENT);
        //        menu.AddItem(new GUIContent("Delete"), false, ContextCallback, ContextType.DELETE_EVENT);
        //        menu.AddSeparator("");
        //        menu.AddItem(new GUIContent("Paste Single"), false, ContextCallback, ContextType.PASTE_SINGLE_EVENT);
        //        menu.AddSeparator("");
        //        menu.AddItem(new GUIContent("Copy To Clipboard"), false, ContextCallback, ContextType.COPY_TO_CLIPBOARD);
        //        menu.AddItem(new GUIContent("Paste From Clipboard"), false, ContextCallback, ContextType.PASTE_FROM_CLIPBOARD);

        //        menu.ShowAsContext(); evt.Use();
        //    }
        //}

        if (Event.current.type == EventType.scrollWheel)
        {
            if (Event.current.delta.y > 0) // up
            {
                SyncToolPanel_Preview.Get().MouseWheel(true);
            }
            else if (Event.current.delta.y < 0) // down
            {
                SyncToolPanel_Preview.Get().MouseWheel(false);
            }

            //if (cameraFollower != null)
            //    cameraFollower.SetDistance();
        }

        SyncToolPanel_Preview.Get().SetMousePosition(Event.current.mousePosition);

        if (Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.Delete)
        {
            //ContextCallback(ContextType.DELETE_EVENT);
        }

        if (isDraggingOnRender && Event.current.type == EventType.ignore)
        {
            isDraggingOnRender = false;
            SyncToolPanel_Preview.Get().SetDragging(false);

            //if (isDraggingOnEventTimeline && Event.current.button == 0)
            //    EndDraggingEventTimeline();
        }

        if (Event.current.type == EventType.mouseDown)
        {
            //if (_windowSyncView.Contains(Event.current.mousePosition))
            if (SyncToolPanel_Preview.Get()._rectPreviewForEvent.Contains(Event.current.mousePosition))
            {
                isDraggingOnRender = true;
                SyncToolPanel_Preview.Get().SetDragging(true);
            }
            //else if (rectEventTimeLine_Outline.Contains(Event.current.mousePosition))
            //{
            //    if (!isDraggingOnEventTimeline && Event.current.button == 0)
            //        StartDraggingEventTimeline();
            //}
        }
        else if (Event.current.type == EventType.MouseUp)
        {
            isDraggingOnRender = false;
            isDraggingOnEventTimeline = false;
            SyncToolPanel_Preview.Get().SetDragging(false);

            //if (isDraggingOnEventTimeline && Event.current.button == 0)
            //    EndDraggingEventTimeline();
        }

        //if (isDraggingOnEventTimeline)
        //{
        //    rectBoxOnEventTimeline.xMax = Event.current.mousePosition.x;
        //    rectBoxOnEventTimeline.yMax = Event.current.mousePosition.y;
        //    GUI.Box(rectBoxOnEventTimeline, "");
        //}
    }
    void ContextCallback(object obj)
    {
        //switch ((ContextType)obj)
        //{
        //    case ContextType.NEW_EVENT:
        //        AnimationEvent newAniEvent = new AnimationEvent();
        //        newAniEvent.time = ContextParams[(int)ContextType.NEW_EVENT].paramFloat0;
        //        newAniEvent.stringParameter = curStateIdentifier; // In order to tell which state the event is in.
        //        dicStateInfo[curIndexLayer][curStateIndex].listAniEvent.Add(newAniEvent);
        //        GenerateEventNameArray();
        //        break;
        //    case ContextType.DELETE_EVENT:
        //        if (selectedAniEvent != null)
        //            dicStateInfo[curIndexLayer][curStateIndex].listAniEvent.Remove(selectedAniEvent);

        //        foreach (var evt in listAniEventInBox)
        //        {
        //            dicStateInfo[curIndexLayer][curStateIndex].listAniEvent.Remove(evt);
        //        }

        //        GenerateEventNameArray();
        //        break;
        //    case ContextType.COPY_SINGLE_EVENT:
        //        aniEventInfoForSingleCopy = null;
        //        if (selectedAniEvent != null)
        //        {
        //            aniEventInfoForSingleCopy = CreateAniEventInfo(selectedAniEvent);
        //        }
        //        else
        //            Debug.LogError("No selected event");
        //        break;
        //    case ContextType.PASTE_SINGLE_EVENT:
        //        if (aniEventInfoForSingleCopy != null)
        //        {
        //            AnimationEvent copiedEvent = AnimatorControllerCopyMachine.CopyEvent(aniEventInfoForSingleCopy.aniEvent);
        //            copiedEvent.stringParameter = curStateIdentifier; // In order to tell which state the event is in.
        //            dicStateInfo[curIndexLayer][curStateIndex].listAniEvent.Add(copiedEvent);
        //            GenerateEventNameArray();
        //        }
        //        else
        //            Debug.LogError("No source event");
        //        break;
        //    case ContextType.COPY_TO_CLIPBOARD:
        //        foreach (var aniEvent in listAniEventInBox)
        //        {
        //            if (listClipboard.Find(m => m.aniEvent == aniEvent) == null)
        //            {
        //                listClipboard.Add(CreateAniEventInfo(aniEvent));
        //            }
        //        }

        //        MecanimEventClipboardWindow.Get().arrayEvent = listClipboard.Select(m =>
        //            m.indexLayer + ",  " + m.state.name + ",  " + m.aniEvent.functionName + ",  " + m.aniEvent.time).ToArray();
        //        MecanimEventClipboardWindow.Get().Focus();

        //        break;
        //    case ContextType.PASTE_FROM_CLIPBOARD:
        //        foreach (var info in listClipboard)
        //        {
        //            AnimationEvent copiedEvent = AnimatorControllerCopyMachine.CopyEvent(info.aniEvent);
        //            copiedEvent.stringParameter = curStateIdentifier; // In order to tell which state the event is in.
        //            dicStateInfo[curIndexLayer][curStateIndex].listAniEvent.Add(copiedEvent);
        //        }
        //        GenerateEventNameArray();
        //        MecanimEventClipboardWindow.Get().Focus();
        //        break;
        //}
    }
#region DoWindow_SelectActor

    GameObject _CharacterPrefab = null;
    //Animation _Animation = null;

    EAnimType _eAnimType = EAnimType.EAnimType_None;

    static public float fMenuHeight = 20.0f;
    static public float fLabelWidth = 140.0f;
    static public float fValueWidth = 200.0f;

    bool IsValidActor(GameObject charPrefab, ref string rErrMsg)
    {
        if (_CharacterPrefab != charPrefab)
            _CharacterPrefab = charPrefab;

        if (null == _CharacterPrefab)
        {
            SyncToolPanel_Animation.Get()._Animation = null;
            SyncToolPanel_Animation.Get().m_Info.actorPrefab = null;

            SyncToolPanel_Animator.Get()._Animator = null;
            SyncToolPanel_Animator.Get().m_Info.actorPrefab = null;

            _eAnimType = EAnimType.EAnimType_None;
            //EditorGUILayout.LabelField("Please select Actor Prefab");
            rErrMsg = "Please select Actor Prefab";
            return false;
        }

        if (null != _CharacterPrefab.GetComponent<Animation>() )
        {
            _eAnimType = EAnimType.EAnimType_Animation;
        }
        else if (null != _CharacterPrefab.GetComponent<Animator>() )
        {
            _eAnimType = EAnimType.EAnimType_Animator;
        }
        else
        {
            //EditorGUILayout.LabelField("Not Found Anim Component In Actor Prefab");
            rErrMsg = "Not Found Anim Component In Actor Prefab";
            return false;
        }

        return true;
    }


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

        //if (_CharacterPrefab == charPrefab)
        //    return;

        string rErrMsg = "";
        if ( false == IsValidActor(charPrefab, ref rErrMsg) )
        {
            EditorGUILayout.LabelField(rErrMsg);
            SyncToolPanel_Preview.Get().ClearActor();
            return;
        }

        GUILayout.BeginVertical("Box");
        {
            if (_eAnimType == EAnimType.EAnimType_Animation)
            {
                if (SyncToolPanel_Animation.Get().m_Info.actorPrefab != charPrefab)
                {
                    SyncToolPanel_Animation.Get().m_Info.actorPrefab = _CharacterPrefab;
                    SyncToolPanel_Preview.Get().ChangeActorForAnimation(SyncToolPanel_Animation.Get().m_Info);
                }
                SyncToolPanel_Animation.Get().DrawAnimList(_CharacterPrefab);
            }
            else if (_eAnimType == EAnimType.EAnimType_Animator)
            {
                if (SyncToolPanel_Animator.Get().m_Info.actorPrefab != charPrefab)
                {
                    SyncToolPanel_Animator.Get().m_Info.actorPrefab = _CharacterPrefab;
                    SyncToolPanel_Preview.Get().ChangeActorForAnimator(SyncToolPanel_Animator.Get().m_Info);
                }
                
                SyncToolPanel_Animator.Get().DrawAnimList(_CharacterPrefab);
            }
        }
        GUILayout.EndVertical();
    }

    public bool IsLegacyType() { return (_eAnimType == EAnimType.EAnimType_Animation); }
    public bool IsMecanimType() { return (_eAnimType == EAnimType.EAnimType_Animator);  }
#endregion DoWindow_SelectActor
    void Update()
    {
        SyncToolPanel_Preview.Get().Update();
        Repaint();
    }

#region DoWindow_SyncView
    void DoWindow_SyncView(int id)
    {
        GUILayout.Label("Sync View", EditorStyles.boldLabel);
        SyncToolPanel_Preview.Get().DrawPreview();
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
}

#endif //UNITY_EDITOR