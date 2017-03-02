using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ANMiniJSON;
namespace Samekids
{
    public class SamekidsAPI : MonoBehaviour
    {
        private const string SERVER = "kindess.com";
        private const string ADS_API = SERVER + "/api/showAds/";

        void Start()
        {

        }

        public void CheckAvalibleAds(Action<bool, string> callback)
        {
            Dictionary<string, string> parms = new Dictionary<string, string>();
            //Json.Serialize()
            string google_aid = "google_aid_111111";
            string android_id = "android_id_222222";
            string userInfo = "{" + string.Format("google_aid: {0}, android_id: {1}", google_aid, android_id) + "}";
            parms.Add("user", userInfo);
            parms.Add("application", Application.bundleIdentifier);
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
            Action<bool, string> callbackInterpreter = (success, data) =>
            {
                if (success)
                {
                    object json = Json.Deserialize(data);
                    Dictionary<string, string> dict = json as Dictionary<string, string>;
                    string successJson = dict["success"];
                    if (successJson == "true")
                    {
                        callback(true, dict["show_ads"]);
                    }
                    else
                    {
                        callback(false, dict["error_message"]);
                    }
                }
                else
                {
                    if (callback != null)
                        callback(false, data);
                }
            };

            SendPost(ADS_API, parms, callbackInterpreter);
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