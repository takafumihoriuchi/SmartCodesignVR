﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCard : OutputCard
{

    public MoveCard()
    {

    }

    protected override string GetCardName() { return "Move"; }
    protected override string InitDescriptionText() { return ""; }
    protected override void InitPropFields() { }
    public override void ConfirmOutputBehaviour() { }
    public override void UpdateOutputBehaviour() { }
    public override void OutputBehaviour() { }
    protected override void BehaviourDuringPrototyping() { }

}
