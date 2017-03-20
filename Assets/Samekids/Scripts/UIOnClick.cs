using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOnClick : MonoBehaviour
{
    public delegate void OnClickEvent();
    public event OnClickEvent OnClick;

    void OnMouseDown()
    {
        if (OnClick != null)
            OnClick();
    }
}
