using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kidness
{
    public class KidnessMetricaAdapter
    {
        private IYandexAppMetrica metrica;

        public event ConfigUpdateHandler OnActivation;

        public void InitAppMetrica()
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

        public void ReportPlayerInfo(UserSurveyResult survey)
        {
            SurveySex sex = SurveySex.Unknown;
            sex = survey.IsBoy ? SurveySex.Boy : SurveySex.Girl;

            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("sex", sex.ToString());
            dictionary.Add("age", survey.Age);

            string name = "PlayerInfo";
            ReportEvent(name, dictionary);
        }

        public void ReportEvent(string message, Dictionary<string, object> parameters = null)
        {
            if (metrica == null)
                metrica = AppMetrica.Instance;
            if (parameters == null)
                parameters = new Dictionary<string, object>();

            metrica.ReportEvent(message, parameters);
        }
    }

    enum SurveySex
    {
         Unknown = 0,
         Boy = 1,
         Girl = 2
    }


}