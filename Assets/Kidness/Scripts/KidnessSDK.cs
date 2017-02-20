using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KidnessSDK : MonoBehaviour
{
    public GameObject SurveyPlayerPrefab;
    public GameObject SurveyCanvasPrefab;

    private Kidness.KidnessMetricaAdapter metrica;

    void Start ()
	{
	    OnSubscribe();

        metrica = new Kidness.KidnessMetricaAdapter();
        metrica.OnActivation += OnAppMetricaActivation;
        metrica.InitAppMetrica();

        OnStartUtilsStuff();
	}

    void OnDisabled()
    {
        OnStopUtilsStuff();
        OnUnsubscribe();
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

    private void OnSurveyFinished(UserSurveyResult _surveyResult)
    {
        surveyResult = _surveyResult;
        isSurvey = false;
        metrica.ReportSurveyPlayerInfo(surveyResult);
    }

    #endregion

    #region Events

    public void ReportNonProfitUser()
    {
        metrica.ReportNonProfitUser();
    }

    public void ReportFirstInapp()
    {
        metrica.ReportFirstInapp();
        if (PlayerPrefs.GetInt(Preferences.DidBuyInapp, 0) == 0)
        {
            metrica.ReportFirstInappTimestamp(GetTotalAppUpTime());
        };
        PlayerPrefs.SetInt(Preferences.DidBuyInapp, 1);
        PlayerPrefs.Save();
    }

    public void ReportAdsWatch()
    {
        metrica.ReportAdsWatch();
    }

    #endregion

    #region Utils

    private long appUpTime = 0;
    private void OnStartUtilsStuff()
    {
        appUpTime = DateTime.Now.ToUniversalTime().Ticks;
    }

    private void OnStopUtilsStuff()
    {
        int totalTime = GetTotalAppUpTime();
        PlayerPrefs.SetInt(Preferences.UpTime, totalTime);
        PlayerPrefs.Save();
        appUpTime = DateTime.Now.ToUniversalTime().Ticks;

        metrica.ReportPlayTime(totalTime);
    }

    private int GetTotalAppUpTime()
    {
        return (int)(DateTime.Now.ToUniversalTime().Ticks - appUpTime + PlayerPrefs.GetInt(Preferences.UpTime, 0));
    }

    #endregion


    #region Test HUD

    private bool inited = false;

    private string appMetricaStatus;

    public string android_id = "";
    public AN_Locale locale;

    public UserSurveyResult surveyResult;
    private bool isSurvey = false;

    void OnGUI()
    {
        if (isSurvey)
            return;

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
            GUI.Label(new Rect(width_offset, height_offset, w, h),
                "AppMetrica not initialized");
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

        if (surveyResult == null)
        {
            if (GUI.Button(new Rect(width_offset, height_offset, w, h), "Start Player Survey"))
            {
                UserSurveyPopup survey = null;
                //if (SurveyGO != null)
                //{
                //    SurveyGO.SetActive(true);
                //    survey = SurveyGO.GetComponent<UserSurveyPopup>();
                //}
                //else
                {
                    survey = FindObjectOfType<UserSurveyPopup>();
                }
                if (survey == null)
                {
                    Canvas canvas = FindObjectOfType<Canvas>();
                    if (canvas == null)
                    {
                        GameObject canvasGO = Instantiate(SurveyCanvasPrefab, Vector3.zero, Quaternion.identity, canvas.transform);
                        canvas = canvasGO.GetComponent<Canvas>();
                    }
                    GameObject surveyGO = Instantiate(SurveyPlayerPrefab, Vector3.zero, Quaternion.identity, canvas.transform);
                    surveyGO.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                    surveyGO.SetActive(true);
                    survey = surveyGO.GetComponent<UserSurveyPopup>();
                }
                survey.OnSurveyFinished += OnSurveyFinished;
                survey.StartSurvey();
                isSurvey = true;
            }
            height_offset += h;

        }
        else
        {
            GUI.Label(new Rect(width_offset, height_offset, w, h), 
                "Player is " + surveyResult.Age + ", " + (surveyResult.IsBoy?"Boy":"Girl"));
            height_offset += h;
        }
    }


    #endregion
}

class Preferences
{
    public const string UpTime = "KidnessSDK.UpTime";
    public const string DidBuyInapp = "KidnessSDK.DidBuyInapp";
}
