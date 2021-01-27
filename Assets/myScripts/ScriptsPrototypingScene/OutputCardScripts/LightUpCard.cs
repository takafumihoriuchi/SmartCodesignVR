using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventBridge;

public class LightUpCard : OutputCard
{
    GameObject paintBrush;
    GameObject brushTip;
    Dictionary<string, GameObject> paintBucket
        = new Dictionary<string, GameObject> {
            {"red", null}, {"blue", null}, {"yellow", null},
            {"green", null}, {"violet", null}, {"orange", null}, {"pink", null},
        };
    //Dictionary<string, GameObject> ledPaint
    //    = new Dictionary<string, GameObject> {
    //        {"red", null}, {"blue", null}, {"yellow", null},
    //        {"green", null}, {"violet", null}, {"orange", null}, {"pink", null},
    //    };
    GameObject waterBucket;
    GameObject water;

    IComponentEventHandler eventBridgeHandler;

    Renderer brushTipRend;
    Renderer envObjRend;

    public LightUpCard()
    {
        //
    }

    protected override string GetCardName() { return "Light Up"; }

    protected override string InitContentText() {
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
        waterBucket = propObjects.transform.Find("WaterBucket").gameObject;

        brushTipRend = brushTip.GetComponent<Renderer>();
        envObjRend = environmentObject.GetComponent<Renderer>();

        //eventBridgeHandler = paintBrush.RequestEventHandlers();
        // => todo "paintBrush"にしたら検知された。brushTipをpaintBrushと同じ形式に変換できないか
        eventBridgeHandler = brushTip.RequestEventHandlers();
        eventBridgeHandler.TriggerEnter += OnTriggerEnterBrushTip;
        eventBridgeHandler.CollisionEnter += OnCollisionEnterBrushTip;
    }

    void OnTriggerEnterBrushTip(Collider other)
    {
        string colliderName = other.transform.gameObject.name;
        Debug.Log("(OnTrigger) other.transform.gameObject.name: " + other.transform.gameObject.name);
        if (colliderName == "LEDPaint" || colliderName == "water")
        {
            //brushTipRend.material = other.GetComponent<Renderer>().material;
            //brushTipRend.sharedMaterial = other.GetComponent<Renderer>().sharedMaterial;
        }
    }
    void OnCollisionEnterBrushTip(Collision other)
    {
        Debug.Log("(OnCollision) other.transform.gameObject.name: " + other.transform.gameObject.name);
        //if (other.transform.gameObject.name == )
    }

    public override void ConfirmOutputBehaviour() { }

    public override void UpdateOutputBehaviour() { }

    public override void OutputBehaviour() { }

    bool brushHasPaint = false;


    protected override void BehaviourDuringPrototyping()
    {
        bool matOnBrushTip = false;

        // when tip of brush is triggered by paint, change tip material to paint material


        matOnBrushTip = true;

        // when "tip" of paintBrush collided with paint in paintBucket
        // どの階層のオブジェクトにrigid-bodyを付ける必要があるのか

        if (brushHasPaint)
        {
            // when collided with environmentObject,
            // change material to the same as the tip of the brush
            // mesh-renderer を持っている子オブジェクトまでenvObjを掘っていく必要ある
            // if has component <Renderer> 風な処理が欲しい
            // まずはenvObj全体のmaterialを一括で変更する処理にすればいい
            // water が選択されたら、original material に戻す
        }
    }


}


// "MonoBehaviour.OnDestroy()" <= cannot be used; not subclass of MonoBehaviour
// todo insert these to disable the trigger events of brushTip
// eventBridgeHandler.TriggerEnter -= OnTriggerEnterBrushTip;
// eventBridgeHandler.CollisionEnter -= OnCollisionEnterBrushTip; 
