using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherCard : InputCard
{
    public WeatherCard()
    {
        maxInstanceNum = 5; // sunny, cloudy, rainy, snowing, thunderstorm
        cardName = "Weather";
        descriptionText = "I can get information about the weather forecast.";
        contentText = "When it is about to";
    }

    protected override void InitPropFields()
    {

    }

    protected override InputConditionDelegate DetermineInputEvaluationDelegate()
    {
        return null;
    }

    protected override void UpdatesForInputConditionEvaluation()
    {

    }

    protected override void BehaviourDuringPrototyping()
    {

    }

    protected override void OnFocusGranted()
    {

    }

    protected override void OnFocusDeprived()
    {

    }

    protected override void OnConfirm()
    {

    }

    protected override void OnBackToEdit()
    {

    }

}
