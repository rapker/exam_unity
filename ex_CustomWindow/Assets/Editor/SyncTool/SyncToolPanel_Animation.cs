using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

#if UNITY_EDITOR
using UnityEditorInternal;


[System.Serializable]
public class SyncToolPanel_Animation
{
    static SyncToolPanel_Animation m_instance = null;

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
            m_instance = null;
    }

    public void DrawAnimList()
    {
        GUILayout.Label("test label");
    }
}
#endif //UNITY_EDITOR