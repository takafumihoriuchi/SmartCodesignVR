using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedCard : InputCard
{
    public SpeedCard()
    {
        maxInstanceNum = 5;
        cardName = "Speed";
        descriptionText = "I can detect whether I am moving fast or slow.";
        contentText = "When am moving";
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
