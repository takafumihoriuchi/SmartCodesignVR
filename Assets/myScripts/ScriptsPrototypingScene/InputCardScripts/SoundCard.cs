using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCard : InputCard
{
    public SoundCard()
    {
        maxInstanceNum = 5;
        cardName = "Sound";
        descriptionText = "I can detect sound.";
        contentText = "When I hear a"; // dog, child, ... （ひとつくらい奇を衒ったもの）
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
