﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using ANMiniJSON;
namespace Samekids
{
    public class SamekidsAPI : MonoBehaviour
    {
        //private const string SERVER = "samekids.com";
        private const string SERVER = "78.24.219.173";
        private const string ADS_API = SERVER + "/api/showAds/";

        void Start()
        {

        }

        public void CheckAvalibleAds(Action<bool, string> callback, string android_id, string google_aid)
        {
            Dictionary<string, object> parms = new Dictionary<string, object>();
            //string userInfo = Json.Serialize(new Dictionary<string, string>() {{"google_aid", google_aid}, {"android_id", android_id} });
            //string userInfo = "{" + string.Format("google_aid: {0}, android_id: {1}", google_aid, android_id) + "}";
            parms.Add("user", new Dictionary<string, string>() { { "google_aid", google_aid }, { "android_id", android_id } });
            parms.Add("application", Application.bundleIdentifier);

            byte[] postData = Encoding.ASCII.GetBytes(Json.Serialize(parms));

            /*
            {user: 
               {
                  google_aid:   google_aid,
                  android_id: android_id,
              },
            application: app_pack
            }
            */

            /*
            успешный
            {
              success: true
              show_ads: bool 
            }

            Не успешный
            {
              success: false
              error_code: int,
              error_message: string
            }
            */
            Action <bool, string> callbackInterpreter = (success, data) =>
            {
                if (success)
                {
                    try
                    {
                        object json = Json.Deserialize(data);
                        Dictionary<string, object> dict = json as Dictionary<string, object>;
                        string successJson = dict["success"].ToString();
                        if (!string.IsNullOrEmpty(successJson) && successJson.ToLower() == "true")
                        {
                            string showAdsJson = dict["show_ads"].ToString();
                            if (!string.IsNullOrEmpty(showAdsJson) && showAdsJson.ToLower() == "true")
                            {
                                callback(true, dict["img"].ToString());
                            }
                            else
                            {
                                callback(false, "Ads is not available");
                            }
                        }
                        else
                        {
                            callback(false, "SERVER ERROR: " + dict["error_message"].ToString());
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        callback(false, "Local Exception");
                    }

                }
                else
                {
                    if (callback != null)
                        callback(false, data);
                }
            };

            SendPost(ADS_API, postData, callbackInterpreter);
        }

        private void SendPost(string url, Dictionary<string, string> parms, Action<bool, string> callback)
        {
            WWWForm form = new WWWForm();
            foreach (KeyValuePair<string, string> pair in parms)
            {
                form.AddField(pair.Key, pair.Value);
            }
            WWW www = new WWW(url, form);

            StartCoroutine(WaitForRequest(www, callback));
        }

        private void SendPost(string url, byte[] postData, Action<bool, string> callback)
        {
            WWW www = new WWW(url, postData);
            Debug.Log("SamekidsAPI :: SendPost. url=" + url + ", data="+ postData);
            StartCoroutine(WaitForRequest(www, callback));
        }

        private IEnumerator WaitForRequest(WWW www, Action<bool, string> callback)
        {
            yield return www;

            if (www.error == null)
            {
                Debug.Log("WWW Ok!: " + www.data);
                if (callback != null)
                    callback(true, www.data);
            }
            else
            {
                Debug.Log("WWW Error: " + www.error);
                if (callback != null)
                    callback(false, www.error);
            }
        }

    }
}