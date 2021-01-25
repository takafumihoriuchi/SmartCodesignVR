using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherCard : InputCard
{

    public WeatherCard() { }
    protected override string GetCardName() { return "Weather"; }
    protected override string InitDescriptionText() { return ""; }
    protected override void InitPropFields() { }
    protected override InputConditionDelegate DetermineInputEvaluationDelegate() { return null; }
    protected override void UpdatesForInputConditionEvaluation() { }
    protected override void BehaviourDuringPrototyping() { }

}
