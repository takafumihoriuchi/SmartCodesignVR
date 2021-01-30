using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCard : InputCard
{
    public ButtonCard()
    {
        maxInstanceNum = 1;
        cardName = "Button";
        descriptionText = "I can detect whether I am pushed.";
        contentText = "When I pressed the button at"; // attached position
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
