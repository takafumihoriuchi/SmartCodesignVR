using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCard : OutputCard
{
    public MoveCard()
    {
        cardName = "Move";
        descriptionText =
            "I can move myself.";
        contentText = "move in the assigned trajectory.";
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
