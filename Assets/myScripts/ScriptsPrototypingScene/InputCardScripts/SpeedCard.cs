using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedCard : InputCard
{

    public SpeedCard() { }
    protected override string GetCardName() { return "Speed"; }
    protected override string InitContentText() { return ""; }
    protected override void InitPropFields() { }
    protected override InputConditionDelegate DetermineInputEvaluationDelegate() { return null; }
    protected override void UpdatesForInputConditionEvaluation() { }
    protected override void BehaviourDuringPrototyping() { }

}
