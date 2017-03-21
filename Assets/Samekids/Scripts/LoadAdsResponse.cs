namespace Samekids
{
    public struct LoadAdsResponse
    {
        public bool success;
        public int error_code;
        public string error_message;
        public bool showAds;
        public string imgURL;
        public string gotoURL;
        public float lockTime;
        public AdsStatus localStatus;

        public LoadAdsResponse(bool showAds, string errorMessage) : this()
        {
            success = true;
            this.showAds = showAds;
            error_message = errorMessage;

            localStatus = IsReadyToShow() ? AdsStatus.Ready : AdsStatus.None;
        }

        public LoadAdsResponse(bool showAds, string imgUrl = null, string gotoUrl = null, float lockTime = 0) : this()
        {
            success = true;
            this.showAds = showAds;
            gotoURL = gotoUrl;
            imgURL = imgUrl;
            this.lockTime = lockTime;

            localStatus = IsReadyToShow() ? AdsStatus.Ready : AdsStatus.None;
        }

        public LoadAdsResponse(int errorCode, string errorMessage) : this()
        {
            success = false;
            showAds = false;
            error_code = errorCode;
            error_message = errorMessage;

            localStatus = AdsStatus.Error;
        }

        public bool IsReadyToShow()
        {
            return success && showAds && !string.IsNullOrEmpty(imgURL);
        }

        public override string ToString()
        {
            return string.Format("Success: {0}, ErrorCode: {1}, ErrorMessage: {2}, ShowAds: {3}, ImgUrl: {4}, GotoUrl: {5}, LockTime: {6}, LocalStatus: {7}",
                success, error_code, error_message, showAds, imgURL, gotoURL, lockTime, localStatus);
        }
    }

    public enum AdsStatus
    {
        None = 0,
        Ready,
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