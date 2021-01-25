using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCard : InputCard
{
    public ButtonCard() {}
    protected override string GetCardName() { return "Button"; }
    protected override string InitDescriptionText() { return ""; }
    protected override void InitPropFields() { }
    protected override InputConditionDelegate DetermineInputEvaluationDelegate() { return null; }
    protected override void UpdatesForInputConditionEvaluation() { }
    protected override void BehaviourDuringPrototyping() { }
}
