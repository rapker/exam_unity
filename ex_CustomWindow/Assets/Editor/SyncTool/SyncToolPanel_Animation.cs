using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

#if UNITY_EDITOR
using UnityEditorInternal;

public struct AnimationInfo
{
    public GameObject actorPrefab;

    public int curStateIndex;
    
    public string[] clipNames;
    public string[] clipNamesForDisplay;
}

[System.Serializable]
public class SyncToolPanel_Animation
{
    static SyncToolPanel_Animation m_instance = null;

    public AnimationInfo m_Info;
    public Animation _Animation = null;

    static public SyncToolPanel_Animation Get()
    {
        if (m_instance != null)
            return m_instance;

        m_instance = new SyncToolPanel_Animation();
        return m_instance;
    }

    static public void Release()
    {
        if (null != m_instance)
        {
            m_instance.m_Info.actorPrefab = null;
            m_instance.m_Info.clipNames = null;
            m_instance.m_Info.clipNamesForDisplay = null;
            m_instance = null;
        }
    }

    public void _Update(float deltaTime, float normalizedTime)
    {
        if (null == _Animation)
           return;

        AnimationState state = _Animation[m_Info.clipNames[m_Info.curStateIndex]];
        if( state )
        {
            state.normalizedTime = normalizedTime;
            _Animation.Sample();
        }
    }

    public void DrawAnimList(GameObject actorPrefab)
    {
        if (null == actorPrefab)
            return;

        if (null == _Animation)
            return;


        GUILayout.BeginVertical("Box");
        {
            GUILayout.Label("Animation Info");
        }
        GUILayout.EndVertical();

        int selectedStateIndex = GUILayout.SelectionGrid(m_Info.curStateIndex, m_Info.clipNames, 1);
        if (selectedStateIndex != m_Info.curStateIndex)
        {
            m_Info.curStateIndex = selectedStateIndex;
            ChangeState();
        }
    }

    public void GetDisplayAnimationClipName()
    {
        if (null == _Animation)
            return;

        if (null != m_Info.clipNames)
            m_Info.clipNames = null;

        if (null != m_Info.clipNamesForDisplay)
            m_Info.clipNamesForDisplay = null;

        int iClipCount = _Animation.GetClipCount();
        m_Info.clipNames = new string[iClipCount];
        m_Info.clipNamesForDisplay = new string[iClipCount];

        int iClipIndex = 0;
        foreach (var ani in _Animation)
        {
            m_Info.clipNames[iClipIndex] = (ani as AnimationState).name;
            m_Info.clipNamesForDisplay[iClipIndex] = "[" + (ani as AnimationState).name + "]";
            ++iClipIndex;
        }
    }

    public void ChangeState()
    {
        if (null == _Animation)
            return;

        PlayReady_FromPreview();
        //SyncToolPanel_Preview.Get().Stop();
    }

    public void PlayReady_FromPreview()
    {
        //AnimationMode.StartAnimationMode();

        AnimationClip clip = _Animation.GetClip(m_Info.clipNames[m_Info.curStateIndex]);
        _Animation.clip = clip;
        _Animation.Play(m_Info.clipNames[m_Info.curStateIndex]);
        //_Animation.CrossFade(m_Info.clipNames[m_Info.curStateIndex]);

        AnimationState state = _Animation[m_Info.clipNames[m_Info.curStateIndex]];
        if (null == state || null == state.clip)
            return;

        SyncToolPanel_Preview.Get().animationDuration = state.length;
        state.enabled = true;
        state.weight = 1;
        state.blendMode = AnimationBlendMode.Blend;
        state.normalizedTime = 0;
        //state.clip.isLooping
    }

    public void Stop_FromPreview()
    {
        AnimationState state = _Animation[m_Info.clipNames[m_Info.curStateIndex]];
        state.enabled = false;
        _Animation.Stop();
        AnimationMode.StopAnimationMode();
    }
}
#endif //UNITY_EDITOR