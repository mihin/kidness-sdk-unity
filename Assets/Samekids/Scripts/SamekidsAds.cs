using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Samekids
{
    [RequireComponent(typeof (Renderer))]
    public class SamekidsAds : MonoBehaviour
    {
        [SerializeField]
        private float ADS_LOCK_TIMER = 3; // sec

        private string ADS_FOLLOW_LINK = "https://play.google.com/store/apps/details?id=biz.neoline.masha&hl=ru";

        public event OnAdsShownEvent OnAdsShown;
        public delegate void OnAdsShownEvent(string url);

        public event OnAdsErrorEvent OnAdsError;
        public delegate void OnAdsErrorEvent(string error);

        public event OnAdsShownEvent OnAdsClosed;

        [SerializeField]
        private UIOnClick CloseButton;
        private Transform trCloseButton;

        private SpriteRenderer renderer;
        private Camera myCamera;

        private string nextAdsURL;
        private AdsStatus status;

        private void Start()
        {
            status = AdsStatus.None;
            renderer = GetComponent<SpriteRenderer>();
            renderer.enabled = false;

            myCamera = transform.parent.GetComponent<Camera>();
            myCamera.enabled = false;

            CloseButton.OnClick += ProcessCloseClick;
            trCloseButton = CloseButton.transform;
            trCloseButton.gameObject.SetActive(false);
        }

        private void OnDisables()
        {
            CloseButton.OnClick -= ProcessCloseClick;
        }

        private void Update()
        {
            if (status == AdsStatus.SuccessShown || status == AdsStatus.SuccessShownLocked)
            {
                if (Input.GetMouseButtonUp(0))
                    ProcessAdsClick();
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

            StartCoroutine("LoadImage");
            return true;
        }

        private void AdsShown(string url)
        {
            if (OnAdsShown != null)
                OnAdsShown(url);

            if (ADS_LOCK_TIMER > 0)
            {
                status = AdsStatus.SuccessShownLocked;
                StartCoroutine("StartLockAdsTimer");
                trCloseButton.gameObject.SetActive(false);
            }
            else
            {
                status = AdsStatus.SuccessShown;
                trCloseButton.gameObject.SetActive(true);
            }
        }

        private IEnumerator StartLockAdsTimer()
        {
            yield return new WaitForSeconds(ADS_LOCK_TIMER);
            OnUnlockAdsTimer();
        }

        private void OnUnlockAdsTimer()
        {
            status = AdsStatus.SuccessShown;

            // show Cross btn
            trCloseButton.gameObject.SetActive(true);

            Debug.Log("SamekidsAds. Ads was wached and unlocked.");
        }

        private void ProcessAdsClick()
        {
            if (status == AdsStatus.SuccessShownLocked || status == AdsStatus.SuccessShown)
                Debug.Log("SamekidsAds. ProcessAdsClick.");

            Application.OpenURL(ADS_FOLLOW_LINK);

            StopCoroutine("StartLockAdsTimer");
            OnUnlockAdsTimer();
            status = AdsStatus.SuccessShownAndClicked;

            Wait(CloseAds, 0.5f);
        }

        private void ProcessCloseClick()
        {
            if (status == AdsStatus.SuccessShownLocked)
            {
                Debug.Log("SamekidsAds. ProcessCloseClick. Ads is shown and locked still..");
                return;
            }

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
            renderer.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
            renderer.transform.localPosition = new Vector3(-tex.width/2, -tex.height/2, 0)/100f;

            trCloseButton.localPosition = new Vector3(-tex.width/2f, tex.height/2f, 0)/100f;

            string url = www.url;
            AdsShown(url);
        }


        private IEnumerator LoadImage()
        {
            status = AdsStatus.Loading;
            string imageUrl = nextAdsURL;
            if (!imageUrl.StartsWith("http"))
                imageUrl = "http://" + nextAdsURL;
            WWW www = new WWW(imageUrl);
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

        #region Utils

        public void Wait(Action action, float sec)
        {
            StartCoroutine(Waiting(action, sec));
        }
        private IEnumerator Waiting(Action action, float sec)
        {
            if (action == null)
                yield break;
            yield return new WaitForSeconds(sec);
            action();
        }

        #endregion


    }

    internal enum AdsStatus
    {
        None = 0,
        Loading,
        Error,
        SuccessShownLocked,
        SuccessShown,
        SuccessShownAndClicked,
        SuccessClosed,
        Skipped,
        Canceled
    }
}