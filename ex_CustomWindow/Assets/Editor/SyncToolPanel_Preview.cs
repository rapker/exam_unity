using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;

#if UNITY_EDITOR
using UnityEditorInternal;

[System.Serializable]
public class SyncToolPanel_Preview
{
    const string pathPreviewToolSet = "Assets/Editor/MecanimPreviewToolSet.prefab";
    //const string pathPreviewToolSet = "Assets/Plugin/MecanimEventEditor/MecanimPreviewToolSet.prefab";

    static SyncToolPanel_Preview m_instance = null;
    Camera m_preCamera;
    GameObject m_curActorGO;
    GameObject m_PreviewToolSet;

    static public SyncToolPanel_Preview Get()
    {
        if (m_instance != null)
            return m_instance;

        m_instance = new SyncToolPanel_Preview();
        return m_instance;
    }

    static public void Release()
    {
        if (null != m_instance)
        {
            m_instance.DestoryPreviewTools();
            m_instance.ClearActor();
            m_instance = null;
        }
    }

    GameObject GetPreviewToolSet_GO()
    {
        if (m_PreviewToolSet != null)
            return m_PreviewToolSet;

        DestoryPreviewTools();

        GameObject goPreview = AssetDatabase.LoadAssetAtPath(pathPreviewToolSet, typeof(GameObject)) as GameObject;
        m_PreviewToolSet = GameObject.Instantiate(goPreview) as GameObject;

        MecanimPreviewToolSet previewToolSet = m_PreviewToolSet.GetComponent<MecanimPreviewToolSet>();
        previewToolSet.previewCamara.gameObject.SetActive(true);
        previewToolSet.previewCamara.aspect = 1;

        return m_PreviewToolSet;
    }
    public MecanimPreviewToolSet GetPreviewToolSet()
    {
        GameObject objectPreview = GetPreviewToolSet_GO();

        if (objectPreview != null)
        {
            return objectPreview.GetComponent<MecanimPreviewToolSet>();
        }
        return null;
    }
    void DestoryPreviewTools()
    {
        GameObject prevOne = GameObject.Find(MecanimPreviewToolSet.strPreviewClone);

        // Make sure there's no preview tools present in the scene as it'll cause some unpredictable problems.
        while (prevOne != null)
        {
            GameObject.DestroyImmediate(prevOne);
            prevOne = GameObject.Find(MecanimPreviewToolSet.strPreviewClone);
        }
    }

    public void ClearActor()
    {
        if (null != m_curActorGO)
        {
            GameObject.DestroyImmediate(m_curActorGO);
            m_curActorGO = null;
        }
    }

    public void ChangeActorForAnimator( AnimatorInfo _Info)
    {
        if( null == _Info.actorPrefab )
        {
            ClearActor();
            return;
        }

        GameObject newActorGO = GameObject.Find(_Info.actorPrefab.name+"(Clone)");
        if (null == m_curActorGO && null == newActorGO)
        {
            m_curActorGO = GameObject.Instantiate(_Info.actorPrefab) as GameObject;
            BuildActor();
        }
        else if (m_curActorGO != newActorGO)
        {
            ClearActor();
            m_curActorGO = GameObject.Instantiate(_Info.actorPrefab) as GameObject;
            BuildActor();
        }

        bool isGridShow = true;
        //isGridShow = GUILayout.Toggle(isGridShow, "Show Grid", EditorStyles.radioButton, GUILayout.Width(100));
        ShowGrid(isGridShow);
    }

    void BuildActor()
    {
        if (null == m_curActorGO)
            return;

        foreach (Transform child in GetPreviewToolSet().slotForActor.transform)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }

        // Add the new built actor to the slot for actor
        m_curActorGO.transform.parent = GetPreviewToolSet().slotForActor.transform;
        m_curActorGO.transform.localPosition = Vector3.zero;

        GetPreviewToolSet().trackball.Init(m_curActorGO);
    }


    const int SliderPanelLeft = 250;

    const int RenderTop = 100;
    const int RenderWidth = 600;
    const int RenderHeight = 400;
    RenderTexture m_renderTexture = new RenderTexture(RenderWidth, RenderWidth, 1);

    public Rect rectPreviewCanvas = new Rect(0.0f, RenderTop, RenderWidth - 50, RenderHeight);

    public void Update()
    {
        GetPreviewToolSet().previewCamara.targetTexture = m_renderTexture;
        GetPreviewToolSet().trackball.UpdateByEditor();
    }
    public void DrawPreview()
    {
        bool isAlphaBlend = true;
        GUI.DrawTexture(rectPreviewCanvas, m_renderTexture, ScaleMode.ScaleAndCrop, isAlphaBlend);
    }

    public void SetMousePosition(Vector3 pos)
    {
        pos.y = -pos.y;
        GetPreviewToolSet().trackball.mousePos = pos;
    }

    public void SetDragging(bool isDragging)
    {
        if (isDragging)
            GetPreviewToolSet().trackball.previewCamera = GetPreviewToolSet().previewCamara;
        else
            GetPreviewToolSet().trackball.previewCamera = null;

        GetPreviewToolSet().trackball.isDragging = isDragging;
    }

    public void MouseWheel(bool bUp)
    {
        if (bUp)
            GetPreviewToolSet().trackball.distance += 0.5f;
        else
            GetPreviewToolSet().trackball.distance -= 0.5f;
    }
    void ResetCameraPosition()
    {
        if (GetPreviewToolSet() != null && GetPreviewToolSet().trackball)
            GetPreviewToolSet().trackball.SetLastDirGoDefault();
    }
    void ShowGrid(bool bGridShow)
    {
        GridOverlay overlay = GetPreviewToolSet().previewCamara.GetComponent<GridOverlay>();
        if (overlay != null)
        {
            overlay.IsgridShow = bGridShow;
        }
    }
}
#endif  //UNITY_EDITOR