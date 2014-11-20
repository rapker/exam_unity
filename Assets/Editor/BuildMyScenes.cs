using UnityEngine;
using UnityEditor;
using System.Collections;

public class BuildMyScenes : MonoBehaviour {

    [MenuItem("Build/Build AssetBundle %#d")]
    public static void BuildSceneToAssetBundle()
    {
        string[] sceneNames = new string[] { "Assets/01.Scenes/scPlay.unity"
                                        , "Assets/01.Scenes/scLevel01.unity" };

        BuildPipeline.BuildStreamedSceneAssetBundle(sceneNames, "Spaceshooter2.unity3d", BuildTarget.StandaloneWindows);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}