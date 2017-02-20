using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kidness
{
    internal class KidnessMetricaAdapter
    {
        private IYandexAppMetrica metrica;

        internal event ConfigUpdateHandler OnActivation;

        internal void InitAppMetrica()
        {
            metrica = AppMetrica.Instance;
            metrica.OnActivation += OnAppMetricaActivation;
            metrica.ActivateWithAPIKey("71ca63a9-eb21-4617-8a23-69d9d2717dea");
            //metrica.SetLoggingEnabled();
            metrica.ReportEvent("Kidness SDK init event");
        }

        private void OnAppMetricaActivation(YandexAppMetricaConfig config)
        {
            OnActivation(config);
        }

        private void TestMetricaEvents()
        {
            ReportFirstInapp();
        }

        #region Events

        internal void ReportEvent(string message, Dictionary<string, object> parameters = null)
        {
            if (metrica == null)
                metrica = AppMetrica.Instance;
            if (parameters == null)
                parameters = new Dictionary<string, object>();

            metrica.ReportEvent(message, parameters);
        }

        internal void ReportSurveyPlayerInfo(UserSurveyResult survey)
        {
            SurveySex sex = SurveySex.unknown;
            sex = survey.IsBoy ? SurveySex.male : SurveySex.female;

            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("sex", sex.ToString());
            dictionary.Add("age", survey.Age);

            string name = Events.PlayerInfo.ToString();
            ReportEvent(name, dictionary);
        }

        internal void ReportFirstInapp()
        {
            ReportEvent(Events.RevenueInfo.ToString(),
                new Dictionary<string, object>() {{EventParams.buy_in_app.ToString(), "true"}});
        }

        internal void ReportFirstInappTimestamp(int time)
        {
            ReportEvent(Events.RevenueInfo.ToString(),
                new Dictionary<string, object>() { { EventParams.time_to_in_app.ToString(), time } });
        }

        internal void ReportAdsWatch()
        {
            ReportEvent(Events.RevenueInfo.ToString(),
                new Dictionary<string, object>() { { EventParams.ads_watch_count.ToString(), 1 } });
        }

        internal void ReportPlayTime(int time)
        {
            ReportEvent(Events.AppInfo.ToString(),
                new Dictionary<string, object>() { { EventParams.play_time_count.ToString(), time } });
        }

        internal void ReportNonProfitUser()
        {
            ReportEvent(Events.PlayerInfo.ToString(),
                new Dictionary<string, object>() { { EventParams.non_profit.ToString(), "true" } });
        }

        #endregion

        private enum SurveySex
        {
            unknown = 0,
            male = 1,
            female = 2
        }

        private enum Events
        {
            RevenueInfo,
            PlayerInfo,
            AppInfo
        }

        private enum EventParams
        {
            buy_in_app,
            time_to_in_app,
            ads_watch_count,
            play_time_count,
            non_profit
        }

    }
}