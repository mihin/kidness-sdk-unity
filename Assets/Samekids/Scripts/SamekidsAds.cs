using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Samekids
{
    [RequireComponent(typeof (Renderer))]
    public class SamekidsAds : MonoBehaviour
    {
        public event OnAdsShownEvent OnAdsShown;
        public delegate void OnAdsShownEvent(string url);

        public event OnAdsErrorEvent OnAdsError;
        public delegate void OnAdsErrorEvent(string error);

        public event OnAdsShownEvent OnAdsClosed;

        private string nextAdsURL;
        private SpriteRenderer renderer;
        private Camera myCamera;

        private AdsStatus status;

        private void Start()
        {
            status = AdsStatus.None;
            renderer = GetComponent<SpriteRenderer>();
            renderer.enabled = false;

            myCamera = transform.parent.GetComponent<Camera>();
            myCamera.enabled = false;
        }

        private void Update()
        {
            if (status == AdsStatus.SuccessShown)
            {
                if (Input.GetMouseButton(0))
                    CloseAds();
            }
        }

        public void RequestAds(SamekidsAPI samekidsApi, string android_id, string google_aid)
        {
            samekidsApi.CheckAvalibleAds(OnAdsAvailableRequest, android_id, google_aid);
        }

        private void OnAdsAvailableRequest(bool success, string imgUrl)
        {
            Debug.Log("OnAdsAvailableRequest :: success?="+ success + ", imgUrl = " + imgUrl);

            if (success)
            {
                nextAdsURL = imgUrl;
                //nextAdsURL = "https://docs.unity3d.com/uploads/Main/ShadowIntro.png";
            }
            else
            {
                nextAdsURL = null;
            }
        }

        public bool ShowAds()
        {
            if (string.IsNullOrEmpty(nextAdsURL))
            {
                status = AdsStatus.Error;
                Debug.LogWarning("SamekidsAds :: ShowAds failed: url is empty");
                return false;
            }
            status = AdsStatus.None;

            //myCamera.enabled = true;
            //renderer.enabled = true;
            StartCoroutine("LoadImage");
            return true;
        }

        private void ProcessAdsClick()
        {
            status = AdsStatus.SuccessClicked;
            CloseAds();
        }

        private void CloseAds()
        {
            status = AdsStatus.SuccessClosed;
            renderer.enabled = false;
            myCamera.enabled = false;

            if (OnAdsClosed != null)
                OnAdsClosed(nextAdsURL);
        }

        private void OnImageLoaded(WWW www)
        {
            int width = Screen.width;
            int height = Screen.height;

            Texture2D tex = new Texture2D(width, height, TextureFormat.DXT1, false);
            www.LoadImageIntoTexture(tex);

            myCamera.enabled = true;
            renderer.enabled = true;
            //renderer.material.mainTexture = tex;
            renderer.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
            renderer.transform.localPosition = renderer.transform.localPosition +  new Vector3(-tex.width/2, -tex.height/2, 0)/100f;

            status = AdsStatus.SuccessShown;
            if (OnAdsShown != null)
                OnAdsShown(www.url);
        }


        private IEnumerator LoadImage()
        {
            status = AdsStatus.Loading;

            WWW www = new WWW(nextAdsURL);
            yield return www;

            nextAdsURL = www.url;

            if (string.IsNullOrEmpty(www.error))
                OnImageLoaded(www);
            else
            {
                Debug.LogWarning("SamekidsAds :: Problems with loading image " + www.url + ". Error: " + www.error);
                status = AdsStatus.Error;
                if (OnAdsError != null)
                    OnAdsError(www.error);
            }
        }


    }

    internal enum AdsStatus
    {
        None = 0,
        Loading,
        Error,
        SuccessShown,
        SuccessClosed,
        SuccessClicked,
        Skipped,
        Canceled
    }
}