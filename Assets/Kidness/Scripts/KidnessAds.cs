using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Renderer))]
public class KidnessAds : MonoBehaviour
{
    public event OnAdsShownEvent OnAdsShown;
    public delegate void OnAdsShownEvent(string url);

    public event OnAdsErrorEvent OnAdsError;
    public delegate void OnAdsErrorEvent(string error);

    private SpriteRenderer renderer;

    void Start ()
    {
        renderer = GetComponent<SpriteRenderer>();
        renderer.enabled = false;
    }
	
	void Update ()
    {

	}

    public void ShowAds()
    {
        renderer.enabled = true;
        StartCoroutine("LoadImage");
    }

    private void OnImageLoaded(WWW www)
    {
        int width = Screen.width;
        int height = Screen.height;

        Texture2D tex = new Texture2D(width, height, TextureFormat.DXT1, false);
        www.LoadImageIntoTexture(tex);

        renderer.enabled = true;
        //renderer.material.mainTexture = tex;
        renderer.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
        renderer.transform.position = new Vector3(-tex.width/2, -tex.height/2, 0)/100f;

        if (OnAdsShown != null)
            OnAdsShown(www.url);
    }

    public string url = "https://docs.unity3d.com/uploads/Main/ShadowIntro.png";

    IEnumerator LoadImage()
    {
        WWW www = new WWW(url);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
            OnImageLoaded(www);
        else
        {
            Debug.LogWarning("KidnessAds :: Problems with loading image " + www.url + ". Error: " + www.error);
            if (OnAdsError != null)
                OnAdsError(www.error);
        }
    }
}
