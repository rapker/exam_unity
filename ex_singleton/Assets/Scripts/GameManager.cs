using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager> {

    public bool _bTest = false;
	
    void Awake()
    {
        _bTest = true;
        Debug.Log("created GameManager _bTest : " + _bTest.ToString());
    }
}
