using System;
using System.Collections;
using UnityEngine;

using Samekids;

public class SamekidsSDK : MonoBehaviour
{
    public GameObject SurveyPlayerPrefab;
    public GameObject SurveyCanvasPrefab;

    private SamekidsMetricaAdapter metrica;

#region getters
    private SamekidsAds ads;
    internal SamekidsAds SamekidsAds
    {
        get
        {
            if (ads == null)
            {
                ads = FindObjectOfType<SamekidsAds>();
                if (ads == null)
                    ads = gameObject.AddComponent<SamekidsAds>();

                ads.OnAdsShown += OnAdsShown;
                ads.OnAdsError += OnAdsError;
                ads.OnAdsClosed += OnAdsClosed;

                ads.Init(SamekidsApi);
            }
            return ads;
        }
    }
    private SamekidsAPI api;
    internal SamekidsAPI SamekidsApi
    {
        get
        {
            if (api == null)
            {
                api = FindObjectOfType<SamekidsAPI>();
                if (api == null)
                    api = gameObject.AddComponent<SamekidsAPI>();
            }
            return api;
        }
    }
#endregion

    void Start ()
	{
	    OnSubscribe();
        OnStartUtilsStuff();
	}

    void OnDisabled()
    {
        OnStopUtilsStuff();
        OnUnsubscribe();
    }

    private void OnSubscribe ()
    {
        AndroidNativeUtility.OnAndroidIdLoaded += OnAndroidIdLoaded;
        AndroidNativeUtility.OnGoogleAidLoaded += OnGoogleAidLoaded;
        AndroidNativeUtility.LocaleInfoLoaded += OnLocaleInfoLoaded;
        AndroidNativeUtility.OnDeviceCodeLoaded += OnDeviceCodeLoaded;
    }

    private void OnUnsubscribe()
    {
        AndroidNativeUtility.OnAndroidIdLoaded -= OnAndroidIdLoaded;
        AndroidNativeUtility.OnGoogleAidLoaded -= OnGoogleAidLoaded;
        AndroidNativeUtility.LocaleInfoLoaded -= OnLocaleInfoLoaded;
        AndroidNativeUtility.OnDeviceCodeLoaded -= OnDeviceCodeLoaded;
    }

    #region Callbacks
    private void OnAndroidIdLoaded(string s)
    {
        android_id = s;
    }
    private void OnGoogleAidLoaded(string s)
    {
        google_aid = s;
    }
    private void OnLocaleInfoLoaded(AN_Locale anLocale)
    {
        locale = anLocale;
    }

    private void OnDeviceCodeLoaded(AN_DeviceCodeResult anDeviceCodeResult)
    {
        deviceCode = anDeviceCodeResult;
    }

    private void OnAppMetricaActivation(YandexAppMetricaConfig config)
    {
        appMetricaStatus += "Initialized: location=" + config.Location + ", appVersion=" + config.AppVersion;
    }

    private void OnSurveyFinished(UserSurveyResult _surveyResult)
    {
        isSurveyActive = false;
        if (_surveyResult == null)
        {
            Debug.Log("SamekidsSDK :: OnSurveyFinished, no results.. =(");
        }
        else
        {
            surveyResult = _surveyResult;
            metrica.ReportSurveyPlayerInfo(surveyResult);
        }
    }

    private void OnAdsShown(string url)
    {
        adsStatus = "shown. " + url;
        isAdsActive = true;
    }

    private void OnAdsClosed(string url)
    {
        adsStatus = "shown and closed. " + url;
        isAdsActive = false;
    }

    private void OnAdsError(string error)
    {
        adsStatus = "error! " + error;
        isAdsActive = false;
    }

    #endregion

    #region Metrica Events

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

    private void LaunchUserSurvey()
    {
        UserSurveyPopup survey = null;
        survey = FindObjectOfType<UserSurveyPopup>();
        if (survey == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasGO = Instantiate(SurveyCanvasPrefab, Vector3.zero, Quaternion.identity);
                canvas = canvasGO.GetComponent<Canvas>();
            }
            GameObject surveyGO = Instantiate(SurveyPlayerPrefab, Vector3.zero, Quaternion.identity, canvas.transform);
            surveyGO.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            surveyGO.SetActive(true);
            survey = surveyGO.GetComponent<UserSurveyPopup>();
        }
        survey.OnSurveyFinished += OnSurveyFinished;
        survey.StartSurvey();
        isSurveyActive = true;
    }

    #region Utils

    private long appUpTime = 0;
    private void OnStartUtilsStuff()
    {
#if !UNITY_EDITOR
        appMetricaStatus = null;
        adsStatus = null;
        android_id = null;
        google_aid = null;
        locale = null;
        deviceCode = null;
        surveyResult = null;
#endif
        appUpTime = DateTime.Now.ToUniversalTime().Ticks;

        metrica = new SamekidsMetricaAdapter();
        metrica.OnActivation += OnAppMetricaActivation;
        metrica.InitAppMetrica();

        StartCoroutine("OnDelayedUtilsStuff");
    }

    private IEnumerator OnDelayedUtilsStuff()
    {
        yield return new WaitForSeconds(1f);
        AndroidNativeUtility.Instance.LoadAndroidId();
        AndroidNativeUtility.Instance.LoadGoogleAid();
        AndroidNativeUtility.Instance.LoadLocaleInfo();
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
    private bool isSurveyActive = false;
    private bool isAdsActive = false;
    private string appMetricaStatus;
    private string adsStatus;
    private string android_id = "android_id_111111";
    private string google_aid = "google_aid_111111";
    private AN_Locale locale;
    private AN_DeviceCodeResult deviceCode;
    private UserSurveyResult surveyResult;

    void OnGUI()
    {
        if (isSurveyActive || isAdsActive)
            return;

        const int w = 400;
        const int h = 70;
        int height_offset = 0;
        int width_offset = 10;

        string statusString = "";
        statusString += "Samekids SDK Sample\n";
        statusString += (string.IsNullOrEmpty(appMetricaStatus)
            ? "AppMetrica not initialized"
            : "AppMetrica initialized: " + appMetricaStatus)
            + "\n";
        statusString += "Android ID = " + android_id + "\n";
        statusString += "Google AID = " + google_aid + "\n";
        statusString += (locale!=null?"Device locale = " + locale.OriginalData: "No device locale info") + "\n";

        GUI.Label(new Rect(width_offset, height_offset, w, 100), statusString);
        height_offset += 100;

        //if (deviceCode == null)
        //{
        //    if (GUI.Button(new Rect(width_offset, height_offset, w, h), "Load Device code"))
        //    {
        //        AndroidNativeUtility.Instance.ObtainUserDeviceCode("752952462331-tqgdkq6lg7o417rkpg63o5uqlud4asrn.apps.googleusercontent.com");
        //    }
        //    height_offset += h;
        //}
        //else
        //{
        //    GUI.Label(new Rect(width_offset, height_offset, w, h), "Device ID = " + deviceCode.ToString());
        //    height_offset += h;
        //}

        if (surveyResult == null)
        {
            if (GUI.Button(new Rect(width_offset, height_offset, w, h), "Start Player Survey"))
            {
                LaunchUserSurvey();
            }
            height_offset += h;

        }
        else
        {
            GUI.Label(new Rect(width_offset, height_offset, w, h), 
                "Player is " + surveyResult.Age + ", " + (surveyResult.IsBoy?"Boy":"Girl"));
            height_offset += h;
        }

        if (GUI.Button(new Rect(width_offset, height_offset, w, h), "Request Ads"))
        {
//#if UNITY_EDITOR
//            SamekidsAds.RequestAds(SamekidsApi, "android_id11111", "google_aid11111");
//#else
            SamekidsAds.RequestAds(android_id, google_aid);
//#endif
        }
        height_offset += h;

        if (GUI.Button(new Rect(width_offset, height_offset, w, h), "Show Ads"))
        {
            SamekidsAds.ShowAds();
        }
        height_offset += h;


        if (string.IsNullOrEmpty(adsStatus))
        {
            GUI.Label(new Rect(width_offset, height_offset, w, h), "Ads: " + adsStatus);
        }
        height_offset += h;
    }
#endregion
}

class Preferences
{
    public const string UpTime = "SamekidsSDK.UpTime";
    public const string DidBuyInapp = "SamekidsSDK.DidBuyInapp";
}
