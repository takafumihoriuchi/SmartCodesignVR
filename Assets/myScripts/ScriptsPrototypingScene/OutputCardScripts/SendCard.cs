using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendCard : OutputCard
{
    // send emails from inside Unity (can be extended to the design of SUI)

    public SendCard()
    {

    }

    protected override string GetCardName() { return "Send"; }
    protected override string InitDescriptionText() { return ""; }
    protected override void InitPropFields() { }
    public override void ConfirmOutputBehaviour() { }
    public override void UpdateOutputBehaviour() { }
    public override void OutputBehaviour() { }
    protected override void BehaviourDuringPrototyping() { }

}
