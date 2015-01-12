using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;

#if UNITY_EDITOR
using UnityEditorInternal;

    
[System.Serializable]
public class SyncToolPanel_Preview
{
    const string pathPreviewToolSet = "Assets/Editor/SyncTool/PreviewToolSet.prefab";
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
        Stop();

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
            BuildActor_ForAnimator();
        }
        else if (m_curActorGO != newActorGO)
        {
            ClearActor();
            m_curActorGO = GameObject.Instantiate(_Info.actorPrefab) as GameObject;
            BuildActor_ForAnimator();
        }

        //isGridShow = GUILayout.Toggle(isGridShow, "Show Grid", EditorStyles.radioButton, GUILayout.Width(100));
        ShowGrid(isGridShow);
    }

    void BuildActor_ForAnimator()
    {
        if (null == m_curActorGO)
            return;

        Animator[] animators = m_curActorGO.GetComponentsInChildren<Animator>(true);
        if (animators.Length > 0)
        {
            SyncToolPanel_Animator.Get()._Animator = animators[0];
            SyncToolPanel_Animator.Get()._Animator.fireEvents = false; // We don't need to fire events since we take care of them in Update().
            //curActorGO.AddComponent<AnimationEventReceiver>();

            AnimatorController animatorController = SyncToolPanel_Animator.Get()._Animator.runtimeAnimatorController as AnimatorController;
            AnimatorController.SetAnimatorController(SyncToolPanel_Animator.Get()._Animator, animatorController);
            SyncToolPanel_Animator.Get().GetDisplayLayerNames(animatorController);
        }

        foreach (Transform child in GetPreviewToolSet().slotForActor.transform)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }

        // Add the new built actor to the slot for actor
        m_curActorGO.transform.parent = GetPreviewToolSet().slotForActor.transform;
        m_curActorGO.transform.localPosition = Vector3.zero;

        GetPreviewToolSet().trackball.Init(m_curActorGO);
        SyncToolPanel_Animator.Get().ChangeLayer();
    }


    const int MenuPanelHeight = 130;

    const int RecentSelectionPanelWidth = 300;
    const int RawSelectionPanelWidth = 220;
    const int SliderPanelLeft = 250;

    const int EditPanelWidth = 300;
    const int EditPanelHeight = 400;

    const int RenderLeft = 10;
    const int RenderTop = 40;
    const int RenderWidth = 600;
    const int RenderHeight = 400;

    int RenderBottom = RenderHeight + RenderTop;

    int EditPanelTop = 520;


    RenderTexture m_renderTexture = new RenderTexture(RenderWidth, RenderWidth, 1);

    public Rect rectPreviewCanvas = new Rect(RenderLeft, RenderTop, RenderWidth, RenderHeight);
    public Rect _rectPreviewForEvent = new Rect(0, 0, RenderWidth, RenderHeight + 50);

    float deltaTime = 0;
    float lastRealTime = 0;

    // Elapsed time since the animation started
    float elapsedTime = 0;
    // The normalized time of the animation being played.
    float normalizedTime = 0;
    // deltaTime by the time range slider
    float commandDelta = 0;

    const float FRAMERATE = 50f; // I have no idea what rate is the best....please adjust it as you want!
    const float oneFrameTime = 1 / FRAMERATE;

    float valueSlider = 0;

    float timeScaleFactor = 1;

    public float animationDuration = 1;

    public void Update()
    {
        deltaTime += (Time.realtimeSinceStartup - lastRealTime) * timeScaleFactor;
        if (Mathf.Abs(deltaTime) < oneFrameTime * timeScaleFactor)
            return; // No update until before deltaTime is more than oneFrameTime


        if (isPlaying)
        {
            if (isPause)
                deltaTime = 0;

            if (!isPause || commandDelta != 0)
            {
                deltaTime += commandDelta;
                commandDelta = 0;

                if (Mathf.Abs(deltaTime) >= oneFrameTime * timeScaleFactor)
                {
                    elapsedTime += deltaTime;
                    _Update(deltaTime);
                    //deltaTime = 0;
                }
            }
        }
        else
        {
            // Keep simulating particle effects even after the animation is complete.
            if (valueSlider >= 1)
            {
                GetPreviewToolSet().SimulateParticles(elapsedTime);

                elapsedTime += deltaTime;
            }
        }

        lastRealTime = Time.realtimeSinceStartup;
        
        deltaTime = 0;

        GetPreviewToolSet().previewCamara.targetTexture = m_renderTexture;
        GetPreviewToolSet().trackball.UpdateByEditor();
    }
    void _Update(float deltaTime)
    {
        if (null == SyncToolPanel_Animator.Get()._Animator)
            return;

        GetPreviewToolSet().SimulateParticles(elapsedTime);

        SyncToolPanel_Animator.Get()._Animator.Update(deltaTime);

        Selection.activeGameObject = GetPreviewToolSet().rootParticle;
        normalizedTime = elapsedTime / animationDuration;
        if (normalizedTime > 1f)
        {
            if (isPlaying && isLoop)
            {
                valueSlider = 0;
                normalizedTime = 0;
                Play();
            }
            else
            {
                isPlaying = false;
                valueSlider = 1;
                normalizedTime = 1;
                //ActivateParticleSystem();
            }
        }
        else
        {
            valueSlider = normalizedTime;
        }

        //if (dicStateInfo != null)
        //{
        //    foreach (AnimationEvent aniEvent in dicStateInfo[curIndexLayer][curStateIndex].listAniEvent)
        //    {
        //        if (aniEvent.stringParameter != curStateIdentifier)
        //            continue;

        //        float eventTime = aniEvent.time * animationDuration;
        //        if (elapsedTime - deltaTime <= eventTime && eventTime < elapsedTime)
        //        {
        //            switch (GetAnimationEventType(aniEvent))
        //            {
        //                case ANIEVENT_TYPE.PARTICLE:
        //                    AddEffect(aniEvent.objectReferenceParameter, elapsedTime);
        //                    break;
        //                case ANIEVENT_TYPE.AUDIO:
        //                    AddAudio(aniEvent.objectReferenceParameter as GameObject);
        //                    break;

        //            }
        //        }

        //    }
        //}
    }


    int cellHeight = 30;
    int sliderHeight = 15;
    int eventHeight = 14;
    int SliderWidth = 600;
    int commonOffsetY_SliderPanel = 45;

    int eventLineNum = 4;

    public void DrawPreview()
    {
        GUILayout.BeginHorizontal(GUILayout.Height(RenderBottom - MenuPanelHeight));
        {
            GUI.DrawTexture(rectPreviewCanvas, m_renderTexture, ScaleMode.ScaleAndCrop, isAlphaBlend);
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(rectPreviewCanvas.height);

        GUILayout.BeginHorizontal();
        {
            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal();
                {
                    int nTimeElapsed = (int)(Mathf.Min(elapsedTime, animationDuration) * 1000f);
                    float fTimeElapsed = nTimeElapsed / 1000f;

                    int nAnimationDuration = (int)(animationDuration * 1000f);
                    float fAnimationDuration = nAnimationDuration / 1000f;

                    int nNormalizedTime = (int)(normalizedTime * 1000f);
                    float fNormalizedTime = Mathf.Min(nNormalizedTime / 1000f, 1);

                    GUILayout.Label("current time : [" + fTimeElapsed + "/" + fAnimationDuration + "]" + "   (normalized time : " + fNormalizedTime + ")");
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    if (!isPlaying)
                    {
                        if (GUILayout.Button("Play", GUILayout.Width(60)))
                        {
                            Play();
                            isPause = false;
                        }
                    }
                    else
                    {
                        bool bTemp = GUILayout.Toggle(isPause, "Pause", GUI.skin.button, GUILayout.Width(60));
                        if (isPause != bTemp)
                        {
                            isPause = bTemp;

                            ParticleSystem particleSystem = GetPreviewToolSet().GetParticleSystem();
                            if (isPause)
                                particleSystem.Pause(true);
                            else
                                particleSystem.Play(true);
                        }
                    }

                    if (GUILayout.Button("Stop", GUILayout.Width(60)))
                    {
                        Stop();
                    }

                    if (GUILayout.Button("+1 Frame", GUILayout.Width(80)))
                    {
                        if (!isPlaying)
                        {
                            Play();
                            isPause = true;
                        }


                        if (isPause)
                        {
                            commandDelta = oneFrameTime;
                            GetPreviewToolSet().SetParticlePlaybackSpeed(1);
                            GetPreviewToolSet().SimulateParticles(elapsedTime + oneFrameTime);
                        }

                        isPause = true;
                    }

                    if (GUILayout.Button("-1 Frame", GUILayout.Width(80)))
                    {
                        if (isPause && elapsedTime > 0)
                        {
                            if (oneFrameTime > elapsedTime)
                                commandDelta = -elapsedTime;
                            else
                                commandDelta = -oneFrameTime;

                            float simulateTime = Mathf.Min(elapsedTime, oneFrameTime);
                            GetPreviewToolSet().SimulateParticles(elapsedTime - simulateTime);
                        }

                        isPause = true;
                    }

                    GUILayout.Space(20);

                    GUILayout.Label("Time Scale Factor :", GUILayout.Width(120));
                    float tempFactor = EditorGUILayout.FloatField(timeScaleFactor, GUILayout.Width(30));
                    if (tempFactor != timeScaleFactor)
                    {
                        timeScaleFactor = tempFactor;
                    }

                    GUILayout.Space(30);



                    isLoop = GUILayout.Toggle(isLoop, "Loop", GUILayout.Width(50));

                    GUILayout.Space(30);

                    GUILayout.Label("FPS:", GUILayout.Width(30));
                    EditorGUILayout.FloatField(FRAMERATE, GUILayout.Width(30));

                }
                GUILayout.EndHorizontal();


                const int SliderPanelLeft = 50;

                GUILayout.BeginHorizontal();
                {
                    float tempValueSlider = GUI.HorizontalSlider(new Rect(0.0f + SliderPanelLeft, RenderBottom + commonOffsetY_SliderPanel + 10, SliderWidth, sliderHeight),
                                                                valueSlider,
                                                                0,
                                                                1,
                                                                EditorStyles.textField,
                                                                EditorStyles.objectFieldThumb);
                    if (tempValueSlider != valueSlider)
                    {
                        if (!isPlaying && valueSlider == 0)
                        {
                            //ResetParticles();
                        }

                        float deltaTime = (tempValueSlider - valueSlider) * animationDuration;
                        elapsedTime = tempValueSlider * animationDuration;
                        _Update(deltaTime);
                        valueSlider = tempValueSlider;
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    CreateLineMaterial();
                    lineMaterial.SetPass(0);

                    Color c = new Color(1f, 1f, 0f, 0.75f);
                    GL.Color(c);

                    GL.Begin(GL.LINES);

                    for (int i = 1; i <= eventLineNum; i++)
                    {
                        int offset = i * 30 + commonOffsetY_SliderPanel;

                        GL.Vertex3(SliderPanelLeft - 45, RenderBottom + offset, 0);
                        GL.Vertex3(SliderPanelLeft + SliderWidth, RenderBottom + offset, 0);

                        GL.Vertex3(SliderPanelLeft - 45, RenderBottom + cellHeight + offset, 0);
                        GL.Vertex3(SliderPanelLeft + SliderWidth, RenderBottom + cellHeight + offset, 0);
                    }

                    GL.Vertex3(SliderPanelLeft, RenderBottom + 1 * cellHeight + commonOffsetY_SliderPanel, 0);
                    GL.Vertex3(SliderPanelLeft, RenderBottom + (eventLineNum + 1) * cellHeight + commonOffsetY_SliderPanel, 0);

                    GL.Vertex3(SliderPanelLeft + SliderWidth, RenderBottom + 1 * cellHeight + commonOffsetY_SliderPanel, 0);
                    GL.Vertex3(SliderPanelLeft + SliderWidth, RenderBottom + (eventLineNum + 1) * cellHeight + commonOffsetY_SliderPanel, 0);

                    GL.End();
                }
                GUILayout.EndHorizontal();

                int offsetX = 47;
                int offsetY = 5 + commonOffsetY_SliderPanel;

                GUILayout.BeginArea(new Rect(SliderPanelLeft - offsetX, RenderBottom + offsetY + 1 * cellHeight, 100f, 30f));
                GUILayout.Label("Particle");
                GUILayout.EndArea();

                /*
                mouseoverFunctionName = "";

                if (Event.current.type == EventType.mouseDown)
                {
                    if (Event.current.button == 0 && rectEventTimeLine_Outline.Contains(Event.current.mousePosition))
                    {
                        selectedAniEvent = null;
                        selectedEventIndex = -1;

                        beingDraggedEvent = null;
                    }
                }

                if (dicStateInfo != null && curState != null && dicStateInfo.Count > curIndexLayer && dicStateInfo[curIndexLayer].Count > curStateIndex && dicStateInfo[curIndexLayer][curStateIndex].state != null)
                {
                    foreach (var aniEvent in dicStateInfo[curIndexLayer][curStateIndex].listAniEvent)
                    {
                        DrawEvent(aniEvent);
                    }

                }
                */

                GUILayout.BeginArea(new Rect(SliderPanelLeft - offsetX, RenderBottom + offsetY + 2 * cellHeight, 100f, 30f));
                GUILayout.Label("Sound");
                GUILayout.EndArea();

                GUILayout.BeginArea(new Rect(SliderPanelLeft - offsetX, RenderBottom + offsetY + 3 * cellHeight, 100f, 30f));
                GUILayout.Label("Etc");
                GUILayout.EndArea();

                GUILayout.Space(170);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Camera Reset", GUILayout.Width(100)))
                    ResetCameraPosition();

                isPanning = GUILayout.Toggle(isPanning, "Panning", EditorStyles.radioButton, GUILayout.Width(130));
                if (GetPreviewToolSet() != null && GetPreviewToolSet().trackball != null)
                    GetPreviewToolSet().trackball.IsPanOn = isPanning;

                isGridShow = GUILayout.Toggle(isGridShow, "Show Grid", EditorStyles.radioButton, GUILayout.Width(100));
                ShowGrid(isGridShow);

                isAlphaBlend = GUILayout.Toggle(isAlphaBlend, "Alpha Blend", EditorStyles.radioButton, GUILayout.Width(100));

                if (GUILayout.Button("Save All Events ", GUILayout.Width(150)))
                {
                    /*
                    int curIndex = 0;

                    foreach (var pair in dicAnimationEventsByClip)
                    {
                        if (pair.Value.Count > 0)
                        {
                            // It seems like the AnimationUtility.SetAnimationEvents() function either only works in runtime 
                            // or sets events from a temporarily created AnimationClip. (it doesn't work anyway!)
                            // So there's no other way but to set their serialized properties

                            AnimationClip clip = pair.Key;
                            ModelImporter modelImporter = (ModelImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(clip));
                            SerializedObject so = new SerializedObject(modelImporter);
                            SerializedProperty clips = so.FindProperty("m_ClipAnimations");

                            for (int i = 0; i < modelImporter.clipAnimations.Length; i++)
                            {
                                if (clips.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue == clip.name)
                                    SetEvents(clips.GetArrayElementAtIndex(i), pair.Value.ToArray());
                            }

                            curIndex++;
                            EditorUtility.DisplayProgressBar("Saving events..", pair.Key.name, curIndex / (float)dicAnimationEventsByClip.Count);

                            so.ApplyModifiedProperties();
                            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(clip), ImportAssetOptions.Default);
                        }
                    }
                    */

                    EditorUtility.ClearProgressBar();

                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndVertical();
    }

    private Material lineMaterial;
    void CreateLineMaterial()
    {

        if (!lineMaterial)
        {
            lineMaterial = new Material("Shader \"Lines/Colored Blended\" {" +
                   "SubShader { Pass { " +
                "    Blend SrcAlpha OneMinusSrcAlpha " +
                "    ZWrite Off Cull Off Fog { Mode Off } " +
                "    BindChannels {" +
                "      Bind \"vertex\", vertex Bind \"color\", color }" +
                   "} } }");
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;

        }
    }




    bool isPlaying = false;
    bool isPanning = true;
    bool isGridShow = true;
    bool isAlphaBlend = false;
    bool isPause = false;
    bool isLoop = false;

    void Play()
    {
        if (SyncToolPanel_Animator.Get()._Animator == null)
            return;

        SyncToolPanel_Animator.Get()._Animator.transform.localPosition = Vector3.zero;
        //ResetParticles();
        isPlaying = true;
        elapsedTime = 0;
        //SyncToolPanel_Animator.Get()._Animator.CrossFade("New State", 0f, 0, 0f);
        SyncToolPanel_Animator.Get().PlayReady_FromPreview();
        //SyncToolPanel_Animator.Get()._Animator.ForceStateNormalizedTime(0);

        Selection.activeGameObject = GetPreviewToolSet().rootParticle;


    }

    public void Stop()
    {
        valueSlider = 0;
        elapsedTime = 0;
        isPlaying = false;
        isPause = false;

        if (SyncToolPanel_Animator.Get()._Animator == null)
            return;
        
        //SyncToolPanel_Animator.Get()._Animator.CrossFade("New State", 0f, 0, 0f);
        //SyncToolPanel_Animator.Get()._Animator.ForceStateNormalizedTime(0);
        SyncToolPanel_Animator.Get()._Animator.transform.localPosition = Vector3.zero;
        SyncToolPanel_Animator.Get()._Animator.transform.localRotation = Quaternion.identity;
        
        SyncToolPanel_Animator.Get()._Animator.Update(0);
        //SyncToolPanel_Animator.Get()._Animator.Update(0);

        //ResetParticles();
    }

    void Pause(bool bPause)
    {
        isPlaying = !bPause;
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