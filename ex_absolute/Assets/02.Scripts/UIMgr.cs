using UnityEngine;
using System.Collections;

public class UIMgr : MonoBehaviour {

    string url = "file:///D:\\Unity_Projects\\example\\trunk\\Spaceshooter.unity3d";

    int version = 1;

    WWW www;

	// Use this for initialization
    IEnumerator Start()
    {
        www = WWW.LoadFromCacheOrDownload(url, version);

        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log(www.error.ToString());
        }
        else
        {
            AssetBundle assetBundle = www.assetBundle;
        }
    }

    void OnGUI()
    {
        if ( www.isDone && GUI.Button(new Rect(20, 50, 100, 30), "Start Game") )
        //if (GUI.Button(new Rect(20, 50, 100, 30), "Start Game"))
        {
            LoadScene();
        }

        GUI.Label( new Rect(20, 20, 200, 30), "Downloading ..." + (www.progress * 100.0f).ToString() + "%" );
    }

    void LoadScene()
    {
        Application.LoadLevel("scLevel01");
        Application.LoadLevelAdditive("scPlay");
    }

	// Update is called once per frame
	void Update () {
	
	}

}