using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventBridge;
using System;

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

    Component[] envPartsComponent;
    GameObject[] envPartsGameObject;
    Renderer brushTipRend;
    Material originalBrushMaterial;
    Material[] originalEnvObjMaterial;
    bool brushHasPaint = false;
    bool brushHasWater = false;


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
        brushTipRend = brushTip.GetComponent<Renderer>();
        originalBrushMaterial = brushTipRend.material;
        envPartsComponent = environmentObject.GetComponentsInChildren<MeshRenderer>(true);
        envPartsGameObject = ConvertComponentArrayToGameObjectArray(envPartsComponent);
        originalEnvObjMaterial = GetMaterialArray(envPartsGameObject);
        // todo store original materials of envObj in a dictionary
        // make a dictionary

        // not needed??
        paintBucket["red"] = propObjects.transform.Find("PaintBucketRed").gameObject;
        paintBucket["blue"] = propObjects.transform.Find("PaintBucketBlue").gameObject;
        paintBucket["yellow"] = propObjects.transform.Find("PaintBucketYellow").gameObject;
        paintBucket["green"] = propObjects.transform.Find("PaintBucketGreen").gameObject;
        paintBucket["violet"] = propObjects.transform.Find("PaintBucketViolet").gameObject;
        paintBucket["orange"] = propObjects.transform.Find("PaintBucketOrange").gameObject;
        paintBucket["pink"] = propObjects.transform.Find("PaintBucketPink").gameObject;
        waterBucket = propObjects.transform.Find("WaterBucket").gameObject;

        eventBridgeHandler = paintBrush.RequestEventHandlers();
        eventBridgeHandler.TriggerEnter += OnTriggerEnterBrushTip;
        eventBridgeHandler.CollisionEnter += OnCollisionEnterBrushTip;

    }

    // refactor later (put functions outside; this is a trigger event)
    void OnTriggerEnterBrushTip(Collider other)
    {
        string colliderName = other.transform.gameObject.name;
        if (colliderName == "LEDPaint")
        {
            brushHasPaint = true;
            brushHasWater = false;
            brushTipRend.material = other.GetComponent<Renderer>().material;
        }
        if (colliderName == "water")
        {
            brushHasWater = true;
            brushHasPaint = false;
            brushTipRend.material = other.GetComponent<Renderer>().material;
        }
    }

    // refactor later (put functions outside; this is a trigger event)
    void OnCollisionEnterBrushTip(Collision other)
    {
        if (!brushHasPaint && !brushHasWater) return; // no paint yet

        int partIdx = Array.IndexOf(envPartsGameObject, other.gameObject);
        if (partIdx == -1) return; // not found; "other" is not envObj
        Renderer envPartRend = envPartsGameObject[partIdx].GetComponent<Renderer>();

        if (brushHasPaint)
        {
            envPartRend.material = brushTipRend.material;
        }
        else if (brushHasWater)
        {
            envPartRend.material = originalEnvObjMaterial[partIdx];
            brushTipRend.material = originalBrushMaterial;
            brushHasWater = false;
        }


    }

    GameObject[] ConvertComponentArrayToGameObjectArray(Component[] compArr)
    {
        GameObject[] objArr = new GameObject[compArr.Length];
        for (int i = 0; i < compArr.Length; i++)
            objArr[i] = compArr[i].gameObject;
        return objArr;
    }

    Material[] GetMaterialArray(GameObject[] objArr)
    {
        Material[] matArr = new Material[objArr.Length];
        for (int i = 0; i < objArr.Length; i++)
            matArr[i] = objArr[i].GetComponent<Renderer>().material;
        return matArr;
    }

    //bool IsElementOfArray(GameObject key, GameObject[] arr)
    //{
    //    foreach (GameObject elm in arr)
    //    {
    //        if (elm == key) return true;
    //    }
    //    return false;
    //}


    public override void ConfirmOutputBehaviour() { }

    public override void UpdateOutputBehaviour() { }

    public override void OutputBehaviour() { }

    


    protected override void BehaviourDuringPrototyping()
    {

        // when tip of brush is triggered by paint, change tip material to paint material


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
