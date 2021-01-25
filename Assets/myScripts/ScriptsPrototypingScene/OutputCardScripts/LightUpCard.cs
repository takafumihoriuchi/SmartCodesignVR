using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightUpCard : OutputCard
{

    public LightUpCard()
    {

    }

    protected override string GetCardName() { return "Light Up"; }
    protected override string InitDescriptionText() { return ""; }
    protected override void InitPropFields() { }
    public override void ConfirmOutputBehaviour() { }
    public override void UpdateOutputBehaviour() { }
    public override void OutputBehaviour() { }
    protected override void BehaviourDuringPrototyping() { }

}
