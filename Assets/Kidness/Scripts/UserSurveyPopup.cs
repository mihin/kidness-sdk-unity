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

    public Button ButtonSexBoy;
    public Button ButtonSexGirl;

    public Slider SliderAge;
    public Text TextAge;
    public Button ButtonAge;

    private UserSurveyResult SurveyResult;

    void Start ()
    {
        SliderAge.onValueChanged.AddListener(OnAgeSlide);
        ButtonSexBoy.onClick.AddListener(() => OnSexButtonClick(true));
        ButtonSexGirl.onClick.AddListener(() => OnSexButtonClick(false));
        ButtonAge.onClick.AddListener(FinishSurvey);

        if (SurveyResult == null)
            Reset();
    }

    private void OnAgeSlide(float val)
    {
        TextAge.text = "I'm " + val.ToString();
        if (!ButtonAge.interactable)
        {
            ButtonAge.enabled = true;
            ButtonAge.interactable = true;
        }

        SurveyResult.Age = Mathf.FloorToInt(val);
    }

    private void OnSexButtonClick(bool isBoy)
    {
        SurveyResult.IsBoy = isBoy;
        ShowQuestionAge();
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

        //ButtonAge.enabled = false;
        ButtonAge.interactable = false;

        TextAge.text = "I'm " + SliderAge.value.ToString();
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
    public void FinishSurvey()
    {
        OnSurveyFinished(SurveyResult);
        Reset();
    }

}



public class UserSurveyResult
{
    public bool IsBoy;
    public int Age;
}