using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrateCard : OutputCard
{
    public VibrateCard()
    {
        cardName = "Vibrate";
        descriptionText =
            "I can vibrate.";
        contentText = "vibrate at the frequency of";
        // shake the object to determine the frequency
        // もしくは、Moveのtrajectoryのコードを使いまわしてもいいかも。
        // 時間制限をつけて、その間にshakeしてもらう。その時の小刻みな軌跡を辿る。
    }

    protected override void InitPropFields()
    {

    }

    public override void OutputBehaviourOnPositive()
    {

    }

    public override void OutputBehaviourOnNegative()
    {

    }

    public override void BehaviourDuringPrototyping()
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
