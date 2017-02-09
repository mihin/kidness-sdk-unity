using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KidnessSDK : MonoBehaviour
{

	void Start ()
	{
	    OnSubscribe();
	    InitAppMetrica();
	}

    void OnDisabled()
    {
        OnUnsubscribe();
    }

    private void InitAppMetrica()
    {
        IYandexAppMetrica metrica = AppMetrica.Instance;
        metrica.OnActivation += OnAppMetricaActivation;
        metrica.ActivateWithAPIKey("71ca63a9-eb21-4617-8a23-69d9d2717dea");
        //metrica.SetLoggingEnabled();
        metrica.ReportEvent("Kidness SDK init event");
    }

    private void OnSubscribe ()
    {
        AndroidNativeUtility.OnAndroidIdLoaded += OnOnAndroidIdLoaded;
        AndroidNativeUtility.LocaleInfoLoaded += OnLocaleInfoLoaded;
    }

    private void OnUnsubscribe()
    {
        AndroidNativeUtility.OnAndroidIdLoaded -= OnOnAndroidIdLoaded;
        AndroidNativeUtility.LocaleInfoLoaded -= OnLocaleInfoLoaded;
    }

#region Callbacks
    private void OnOnAndroidIdLoaded(string s)
    {
        android_id = s;
    }
    private void OnLocaleInfoLoaded(AN_Locale anLocale)
    {
        locale = anLocale;
    }

    private void OnAppMetricaActivation(YandexAppMetricaConfig config)
    {
        appMetricaStatus += "Location=" + config.Location + ", " + config.ApiKey;
    }

#endregion


#region Test HUD

    private bool inited = false;

    private string appMetricaStatus;

    public string android_id = "";
    public AN_Locale locale;

    void OnGUI()
    {
        const int w = 400;
        const int h = 80;
        int height_offset = 0;
        int width_offset = 10;

        GUI.Label(new Rect(width_offset, height_offset, w, h), "Kidness SDK Sample");
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

        if (!string.IsNullOrEmpty(appMetricaStatus))
        {
            GUI.Label(new Rect(width_offset, height_offset, w, h),
                "AppMetrica initialized: " + appMetricaStatus);
            height_offset += h;
        }
        else
        {
            var metrica = AppMetrica.Instance;
            GUI.Label(new Rect(width_offset, height_offset, w, h),
                "AppMetrica not initialized. version " + metrica.LibraryVersion);
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
