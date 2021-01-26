using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightUpCard : OutputCard
{
    private GameObject paintBrush;
    private Dictionary<string, GameObject> paintBucket
        = new Dictionary<string, GameObject> {
            {"red", null}, {"blue", null}, {"yellow", null},
            {"green", null}, {"violet", null}, {"orange", null}, {"pink", null},
        };

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


// 色を指定する => どうやったら楽しいか
// 平面の color palette を実装しても対して面白くない
// 「paintが入ったバケツ」と「ブラシ」がオブジェクトとしてあって、
// ブラシをバケツに漬けるとブラシ先の色が変わり、
// ブラシをenvObjに付けると、envObjがブラシ先の色に変わる。