using UnityEngine;
using System.Collections;

public class UIMgr : MonoBehaviour {

    void OnGUI()
    {
        if( GUI.Button(new Rect(20,20,100,30), "Start Game") )
        {
            LoadScene();
        }
    }

    void LoadScene()
    {
        Application.LoadLevel( "scLevel01" );
        Application.LoadLevelAdditive( "scPlay" );
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}