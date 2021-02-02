using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendCard : OutputCard
{
    // send emails from inside Unity (can be extended to the design of SUI)

    public SendCard()
    {
        cardName = "Send";
        descriptionText =
            "I can send an Email or SMS.";
        contentText = "send a message to the following address:";
        // e.g. add UI-text-box under the parent of the variable box.
        // OVR offers a VR keyboard
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
