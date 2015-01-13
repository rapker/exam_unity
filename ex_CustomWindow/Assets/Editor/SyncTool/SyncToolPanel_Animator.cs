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
    public AnimatorInfo m_Info;
    public Animator _Animator = null;

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

    public void _Update(float deltaTime)
    {
        if (null == _Animator)
            return;

        _Animator.Update(deltaTime);
    }

    public void DrawAnimList(GameObject actorPrefab)
    {
        if (null == actorPrefab)
            return;

        if (null == _Animator)
            return;

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

        DrawLayers();
    }

    Vector2 scrollPos_State;

    void DrawLayers()
    {
        GUILayout.BeginVertical();
        {
            GUILayout.BeginVertical("Box");
            AnimatorController controller = _Animator.runtimeAnimatorController as AnimatorController;
            if (controller && null != m_Info.dispLayerNames)
            {
                //GetDisplayLayerNames(controller);
                GUILayout.BeginHorizontal(GUILayout.Height(AnimationSyncToolWindow.fMenuHeight));
                {
                    EditorGUILayout.LabelField("Layer", GUILayout.Width(AnimationSyncToolWindow.fLabelWidth));
                    //m_Info.curLayerIndex = EditorGUILayout.Popup(m_Info.curLayerIndex, m_Info.dispLayerNames, GUILayout.Width(AnimationSyncToolWindow.fValueWidth));
                    int newSelectedLayer = EditorGUILayout.Popup(m_Info.curLayerIndex, m_Info.dispLayerNames, GUILayout.Width(AnimationSyncToolWindow.fValueWidth));
                    if( m_Info.curLayerIndex != newSelectedLayer )
                    {
                        m_Info.curLayerIndex = newSelectedLayer;
                        ChangeLayer();
                    }
                }
                GUILayout.EndHorizontal();
                DrawState();
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

    public void GetDisplayLayerNames(AnimatorController controller)
    {
        if (null == controller)
            return;

        if (null != m_Info.dispLayerNames)
            m_Info.dispLayerNames = null;

        m_Info.listLayer.Clear();
        m_Info.dispLayerNames = new string[controller.layerCount];

        for (int iLayer = 0; iLayer < controller.layerCount; iLayer++)
        {
            m_Info.listLayer.Add(controller.GetLayer(iLayer));
            m_Info.dispLayerNames[iLayer] = controller.GetLayer(iLayer).name;
        }
    }

    void DrawState()
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
            ChangeState();
        }
    }

    public void ChangeLayer()
    {
        if (m_Info.curLayerIndex < 0)
            return;

        m_Info.curStateIndex = 0;

        //GenerateStateGUIContentArray();

        if (m_Info.listLayer.Count > m_Info.curLayerIndex && m_Info.listLayer[m_Info.curLayerIndex].stateMachine.stateCount > 0)
        {
            State curState = m_Info.listLayer[m_Info.curLayerIndex].stateMachine.GetState(0);

            AnimatorController controller = _Animator.runtimeAnimatorController as AnimatorController;
            //controller.GetLayer(0).stateMachine.GetState(0).SetAnimationClip(curState.GetMotion() as AnimationClip);
        }
    }

    void ChangeState()
    {
        PlayReady_FromPreview();
        SyncToolPanel_Preview.Get().Stop();
    }

    public void PlayReady_FromPreview()
    {
        StateMachine machine = m_Info.listLayer[m_Info.curLayerIndex].stateMachine;
        State curState = machine.GetState(m_Info.curStateIndex);
        if (curState && curState.GetMotion())
        {
            SyncToolPanel_Preview.Get().animationDuration = curState.GetMotion().averageDuration;
            AnimatorController controller = _Animator.runtimeAnimatorController as AnimatorController;
            //controller.GetLayer(0).stateMachine.GetState(0).SetAnimationClip(curState.GetMotion() as AnimationClip);
            _Animator.CrossFade(curState.name, 0f, 0, 0f);
        }
    }

}
#endif  //UNITY_EDITOR