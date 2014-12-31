using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CustomElem
{
    public bool IsTrue;
}

public class CustomInspector : MonoBehaviour
{
    public CustomElem _CustomElem;
    public List<CustomElem> _CustomElems;
}