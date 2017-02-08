using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KidnessSDK : MonoBehaviour
{

	void Start ()
	{
	    OnSubscribe();
	}

    void OnDisabled()
    {
        OnUnsubscribe();
    }

    private void OnSubscribe ()
    {
        AndroidNativeUtility.OnAndroidIdLoaded += OnOnAndroidIdLoaded;
        AndroidNativeUtility.LocaleInfoLoaded += OnLocaleInfoLoaded;
    }

    private void OnLocaleInfoLoaded(AN_Locale anLocale)
    {
        locale = anLocale;
    }

    private void OnUnsubscribe()
    {
        AndroidNativeUtility.OnAndroidIdLoaded -= OnOnAndroidIdLoaded;
    }

    #region Callbacks
    private void OnOnAndroidIdLoaded(string s)
    {
        android_id = s;
    }

#endregion


#region Test HUD

    private bool inited = false;

    public string android_id = "";
    public AN_Locale locale;

    void OnGUI()
    {
        const int w = 400;
        const int h = 80;
        int height_offset = 0;
        int width_offset = 10;

        GUI.Label(new Rect(width_offset, height_offset, w, h), "Kidness SDK Sample, inited ?= " + inited);
        height_offset += h;

        if (!inited)
        {
            if (GUI.Button(new Rect(width_offset, height_offset, w, h), "Init Android SDK"))
            {
                AndroidNativeUtility anu = AndroidNativeUtility.Instance;
                inited = true;
            }
            height_offset += h;
        }


        if (string.IsNullOrEmpty(android_id))
        {
            if (GUI.Button(new Rect(width_offset, height_offset, w, h), "Load Android ID"))
            {
                AndroidNativeUtility.Instance.LoadAndroidId();
            }
            height_offset += h;
        }
        else
        {
            GUI.Label(new Rect(width_offset, height_offset, w, h), "Android ID = " + android_id);
            height_offset += h;
        }

        if (locale == null)
        {
            if (GUI.Button(new Rect(width_offset, height_offset, w, h), "Load locale"))
            {
                AndroidNativeUtility.Instance.LoadLocaleInfo();
            }
            height_offset += h;
        }
        else
        {
            GUI.Label(new Rect(width_offset, height_offset, w, h), "Device locale = " + locale.OriginalData);
            height_offset += h;
        }

    }
#endregion
}
