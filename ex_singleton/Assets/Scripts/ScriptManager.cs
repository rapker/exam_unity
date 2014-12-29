using UnityEngine;
using System.Collections;


public class ScriptManager : MonoBehaviour
{

    void Awake()
    {
        Debug.Log("Main GameManager test boolean 1 :" + GameManager.Instance._bTest.ToString() );
        GameManager.Instance._bTest = false;
        Debug.Log("Main GameManager test boolean 2 :" + GameManager.Instance._bTest.ToString());
    }
}
