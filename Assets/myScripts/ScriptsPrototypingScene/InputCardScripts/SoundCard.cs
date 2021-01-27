using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCard : InputCard
{
    // 物が置いてあって（牛とか猫とか）、その鳴き声が聞こえたら、xxxする、など？？

    public SoundCard() { }
    protected override string GetCardName() { return "Sound"; }
    protected override string InitContentText() { return ""; }
    protected override void InitPropFields() { }
    protected override InputConditionDelegate DetermineInputEvaluationDelegate() { return null; }
    protected override void UpdatesForInputConditionEvaluation() { }
    protected override void BehaviourDuringPrototyping() { }

}
