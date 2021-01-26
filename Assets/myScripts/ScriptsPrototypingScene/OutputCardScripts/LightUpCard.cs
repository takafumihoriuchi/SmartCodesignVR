using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightUpCard : OutputCard
{
    private GameObject paintBrush;
    private GameObject brushTip;
    private Dictionary<string, GameObject> paintBucket
        = new Dictionary<string, GameObject> {
            {"red", null}, {"blue", null}, {"yellow", null},
            {"green", null}, {"violet", null}, {"orange", null}, {"pink", null},
        };
    private Dictionary<string, GameObject> ledPaint
        = new Dictionary<string, GameObject> {
            {"red", null}, {"blue", null}, {"yellow", null},
            {"green", null}, {"violet", null}, {"orange", null}, {"pink", null},
        };

    public LightUpCard()
    {

    }

    protected override string GetCardName() { return "Light Up"; }

    protected override string InitDescriptionText() {
        return "Light up LED in <color=red>[color]</color> " +
            "(paint the " + environmentObject.name + " using the brush)"; }

    protected override void InitPropFields()
    {
        paintBrush = propObjects.transform.Find("PaintBrush").gameObject;
        brushTip = paintBrush.transform.Find("brushTip").gameObject;

        paintBucket["red"] = propObjects.transform.Find("PaintBucketRed").gameObject;
        paintBucket["blue"] = propObjects.transform.Find("PaintBucketBlue").gameObject;
        paintBucket["yellow"] = propObjects.transform.Find("PaintBucketYellow").gameObject;
        paintBucket["green"] = propObjects.transform.Find("PaintBucketGreen").gameObject;
        paintBucket["violet"] = propObjects.transform.Find("PaintBucketViolet").gameObject;
        paintBucket["orange"] = propObjects.transform.Find("PaintBucketOrange").gameObject;
        paintBucket["pink"] = propObjects.transform.Find("PaintBucketPink").gameObject;
    }

    public override void ConfirmOutputBehaviour() { }

    public override void UpdateOutputBehaviour() { }

    public override void OutputBehaviour() { }

    private bool brushHasPaint = false;


    protected override void BehaviourDuringPrototyping()
    {
        // when "tip" of paintBrush collided with paint in paintBucket
        // どの階層のオブジェクトにrigid-bodyを付ける必要があるのか

        if (brushHasPaint)
        {
            // when collided with environmentObject,
            // change material to the same as the tip of the brush
            // mesh-renderer を持っている子オブジェクトまでenvObjを掘っていく必要ある
            // if has component <Renderer> 風な処理が欲しい
        }
    }

}

// todo CardSelectionDetector.csのようなEventBridgeで衝突を検知する

// まずは BehaviourDuringPrototyping() { } を記述する（ベタ書きでいい）
// 次に、それをFireCard.csを倣ってリファクタリングする（confirm後は、色が自動で変わるだけ）