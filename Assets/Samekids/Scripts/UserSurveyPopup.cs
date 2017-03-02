using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserSurveyPopup : MonoBehaviour
{
    public event OnSurveyFinishedEvent OnSurveyFinished;
    public delegate void OnSurveyFinishedEvent(UserSurveyResult surveyResult);

    public GameObject PanelMain;
    public GameObject PanelSex;
    public GameObject PanelAge;

    public Button ButtonClose;

    public Button ButtonSexBoy;
    public Button ButtonSexGirl;

    private UserSurveyResult SurveyResult;

    void Start ()
    {
        ButtonClose.onClick.AddListener(() => FinishSurvey(false));
        ButtonSexBoy.onClick.AddListener(() => OnSexButtonClick(true));
        ButtonSexGirl.onClick.AddListener(() => OnSexButtonClick(false));

        if (SurveyResult == null)
            Reset();
    }

    private void OnSexButtonClick(bool isBoy)
    {
        SurveyResult.IsBoy = isBoy;
        ShowQuestionAge();
    }

    void OnAgeButtonClick(Button b)
    {
        int age = -1;
        if (b != null && int.TryParse(b.name, out age))
            SurveyResult.Age = age;
        else
            SurveyResult.Age = -1;

        FinishSurvey();
    }

    private void ShowQuestionSex()
    {
        PanelSex.SetActive(true);
        PanelAge.SetActive(false);
    }
    private void ShowQuestionAge()
    {
        PanelSex.SetActive(false);
        PanelAge.SetActive(true);
    }

    private void Reset()
    {
        PanelMain.SetActive(false);
        PanelSex.SetActive(false);
        PanelAge.SetActive(false);
        SurveyResult = null;
    }

    public void StartSurvey()
    {
        PanelMain.SetActive(true);
        ShowQuestionSex();
        SurveyResult = new UserSurveyResult();
    }
    public void FinishSurvey(bool success = true)
    {
        if (success)
            OnSurveyFinished(SurveyResult);
        else
            OnSurveyFinished(null);
        Reset();
    }

}



public class UserSurveyResult
{
    public bool IsBoy;
    public int Age;
}