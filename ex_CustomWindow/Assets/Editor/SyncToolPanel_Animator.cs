using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

#if UNITY_EDITOR
using UnityEditorInternal;

public struct AnimatorInfo
{
    public GameObject actorPrefab;

    public int curLayerIndex;
    public int curStateIndex;

    public string[] dispLayerNames;

    public List<AnimatorControllerLayer> listLayer;
    public string[] clipNames;
}


[System.Serializable]
public class SyncToolPanel_Animator
{
    static SyncToolPanel_Animator m_instance = null;
    AnimatorInfo m_Info;

    static public SyncToolPanel_Animator Get()
    {
        if (m_instance != null)
            return m_instance;

        m_instance = new SyncToolPanel_Animator();
        m_instance.m_Info.listLayer = new List<AnimatorControllerLayer>();
        return m_instance;
    }

    static public void Release()
    {
        if (null != m_instance)
        {
            m_instance.m_Info.actorPrefab = null;
            m_instance.m_Info.dispLayerNames = null;
            m_instance.m_Info.listLayer = null;
            m_instance.m_Info.clipNames = null;
            m_instance = null;
        }
    }

    public void DrawAnimList(GameObject actorPrefab, Animator _Animator)
    {
        if (null == actorPrefab || null == _Animator)
            return;

        m_Info.actorPrefab = actorPrefab;

        GUILayout.BeginVertical("Box");
        {
            GUILayout.Label("Animator Info");
        }
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal(GUILayout.Height(AnimationSyncToolWindow.fMenuHeight));
        {
            EditorGUILayout.LabelField("Controller", GUILayout.Width(AnimationSyncToolWindow.fLabelWidth));
            EditorGUILayout.ObjectField(_Animator.runtimeAnimatorController, typeof(AnimatorController), false, GUILayout.Width(AnimationSyncToolWindow.fValueWidth));
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(GUILayout.Height(AnimationSyncToolWindow.fMenuHeight));
        {
            EditorGUILayout.LabelField("Avatar", GUILayout.Width(AnimationSyncToolWindow.fLabelWidth));
            EditorGUILayout.ObjectField(_Animator.avatar, typeof(Avatar), false, GUILayout.Width(AnimationSyncToolWindow.fValueWidth));
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(GUILayout.Height(AnimationSyncToolWindow.fMenuHeight));
        {
            EditorGUILayout.Toggle("Apply Root Motion", _Animator.applyRootMotion, GUILayout.Width(AnimationSyncToolWindow.fLabelWidth));
            GUILayout.Label("", GUILayout.Width(AnimationSyncToolWindow.fValueWidth));
        }
        GUILayout.EndHorizontal();

        DrawAnimatorLayers( _Animator );
    }

    Vector2 scrollPos_State;

    void DrawAnimatorLayers(Animator _Animator)
    {
        GUILayout.BeginVertical();
        {
            GUILayout.BeginVertical("Box");
            AnimatorController controller = _Animator.runtimeAnimatorController as AnimatorController;
            if (controller)
            {
                m_Info.listLayer.Clear();
                GetDisplayLayerNames(controller);
                GUILayout.BeginHorizontal(GUILayout.Height(AnimationSyncToolWindow.fMenuHeight));
                {
                    EditorGUILayout.LabelField("Layer", GUILayout.Width(AnimationSyncToolWindow.fLabelWidth));
                    m_Info.curLayerIndex = EditorGUILayout.Popup(m_Info.curLayerIndex, m_Info.dispLayerNames, GUILayout.Width(AnimationSyncToolWindow.fValueWidth));
                }
                GUILayout.EndHorizontal();
                DrawAnimatorStateList(controller);
            }
            else
            {
                GUILayout.Label( "Please setting. Animator controller" );
                SyncToolPanel_Preview.Get().ClearActor();
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndVertical();
    }

    void GetDisplayLayerNames(AnimatorController controller)
    {
        if (null != m_Info.dispLayerNames)
            m_Info.dispLayerNames = null;

        m_Info.dispLayerNames = new string[controller.layerCount];

        for (int iLayer = 0; iLayer < controller.layerCount; iLayer++)
        {
            m_Info.listLayer.Add(controller.GetLayer(iLayer));
            m_Info.dispLayerNames[iLayer] = controller.GetLayer(iLayer).name;
        }
    }

    void DrawAnimatorStateList(AnimatorController controller)
    {
        if (m_Info.listLayer.Count <= 0)
        {
            SyncToolPanel_Preview.Get().ClearActor();
            return;
        }
        
        StateMachine machine = m_Info.listLayer[m_Info.curLayerIndex].stateMachine;

        if (null != m_Info.clipNames)
            m_Info.clipNames = null;

        m_Info.clipNames = new string[machine.stateCount];

        for (int i = 0; i < machine.stateCount; i++)
        {
            State state = machine.GetState(i);
            if (null == state.GetMotion())
            {
                m_Info.clipNames[i] = state.name + "[no clip]";
            }
            else
            {
                m_Info.clipNames[i] = state.name + "[clip: " + state.GetMotion().name + "]";
            }
        }

        int selectedStateIndex = GUILayout.SelectionGrid(m_Info.curStateIndex, m_Info.clipNames, 1);
        if (selectedStateIndex != m_Info.curStateIndex)
        {
            m_Info.curStateIndex = selectedStateIndex;
        }

        SyncToolPanel_Preview.Get().ChangeActorForAnimator(m_Info);
    }
}
#endif  //UNITY_EDITOR