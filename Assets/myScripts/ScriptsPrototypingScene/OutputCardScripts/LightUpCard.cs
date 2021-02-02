using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventBridge;
using System;

// Template for sub-classes of OutputCard:
// - set/update variableTextTMP.SetText("")

public class LightUpCard : OutputCard
{
    IComponentEventHandler eventBridgeHandler;

    GameObject paintBrush;
    GameObject brushTip;
    Renderer brushTipRend;
    Material originalBrushMaterial;

    Component[] envPartsComponent;
    GameObject[] envPartsGameObject;
    Material[] originalEnvObjMaterial;
    Material[] edittedEnvObjMaterial;

    // todo インスタンス間で共有できないから、別の手法の方が良い
    bool brushHasPaint = false;
    bool brushHasWater = false;

    int paintCount = 0;


    public LightUpCard()
    {
        cardName = "Light Up";

        descriptionText =
            "<i>I can light up LEDs in different colors.</i>\n" +
            "<b>Steps:</b>\n" +
            "<b>1.</b> <indent=10%>Pickup paint brush with the trigger button.</indent>\n" +
            "<b>2.</b> <indent=10%>Dip paint brush in paint bucket.</indent>\n" +
            "<b>3.</b> <indent=10%>Rub the paint on the object.</indent>\n" +
            "<b>*</b> Different parts can be set to different colors.\n" +
            "<b>*</b> Paint can be removed by putting \"water\" on the object.";

        contentText = "turn on LED lights in the colors of";
    }


    protected override void InitPropFields()
    {
        paintBrush = propObjects.transform.Find("PaintBrush").gameObject;
        brushTip = paintBrush.transform.Find("brushTip").gameObject;
        brushTipRend = brushTip.GetComponent<Renderer>();
        originalBrushMaterial = brushTipRend.material;

        envPartsComponent = environmentObject.GetComponentsInChildren<MeshRenderer>(true);
        envPartsGameObject = ConvertComponentArrayToGameObjectArray(envPartsComponent);
        originalEnvObjMaterial = GetMaterialArray(envPartsGameObject);
        edittedEnvObjMaterial = GetMaterialArray(envPartsGameObject);

        // todo 参照渡しを意識して組み直す
        eventBridgeHandler = paintBrush.RequestEventHandlers();
    }

    // called every frame
    protected override void BehaviourDuringPrototyping() { }

    // todo 相手の色を上書きしないように気を遣わなくてはいけない；
    // 最初に自分のnegative、その後（もしあれば）相手のpositive
    public override void OutputBehaviourOnPositive() {
        ApplyMaterial(ref envPartsGameObject, edittedEnvObjMaterial);
    }

    public override void OutputBehaviourOnNegative() {
        ApplyMaterial(ref envPartsGameObject, originalEnvObjMaterial);
    }

    protected override void OnFocusGranted() {
        eventBridgeHandler.TriggerEnter += OnTriggerEnterBrushTip;
        eventBridgeHandler.CollisionEnter += OnCollisionEnterBrushTip;
        // load colors
        ApplyMaterial(ref envPartsGameObject, edittedEnvObjMaterial);
    }

    protected override void OnFocusDeprived() {
        eventBridgeHandler.TriggerEnter -= OnTriggerEnterBrushTip;
        eventBridgeHandler.CollisionEnter -= OnCollisionEnterBrushTip;
        // => こいつを消してしまえば、ロードした時にそのインスタンスの色に塗るだけだからうまくいく。
        // => 全てのインスタンスがfalseの時に色が残ってしまう
    }


    // when brush is dipped in the buckets
    // independent from other instances
    // c.f. "eventBridgeHandler" is instance independent
    void OnTriggerEnterBrushTip(Collider other)
    {
        string colliderName = other.transform.gameObject.name;

        if (colliderName == "LEDPaint" || colliderName == "water")
        {
            brushTipRend.material = other.GetComponent<Renderer>().material;
        }

        //// check if triggered on paint bucket
        //if (colliderName == "LEDPaint")
        //{
        //    brushHasPaint = true;
        //    brushHasWater = false;
        //    brushTipRend.material = other.GetComponent<Renderer>().material;
        //    return;
        //}
        //// check if triggered on water bucket
        //if (colliderName == "water")
        //{
        //    brushHasWater = true;
        //    brushHasPaint = false;
        //    brushTipRend.material = other.GetComponent<Renderer>().material;
        //    return;
        //}
    }


    // when brush is rubbed on envObj
    // independent from other instances
    // c.f. "eventBridgeHandler" is instance independent
    void OnCollisionEnterBrushTip(Collision other)
    {
        // nothing on brush
        //if (!brushHasPaint && !brushHasWater) return;

        // reject if no paint/water is on the brush
        if (brushTipRend.material == originalBrushMaterial) return;

        // reject if the collided object is not a part of envObj
        int partIdx = Array.IndexOf(envPartsGameObject, other.gameObject);
        if (partIdx == -1) return; // not found
        Renderer envPartRend = other.gameObject.GetComponent<Renderer>();

        

        if (brushHasPaint) // then put color on envbj
        {
            envPartRend.material = brushTipRend.material;
            edittedEnvObjMaterial[partIdx] = brushTipRend.material;

            if (paintCount == 0) { // todo refactor // todo paintCound==0の間はConfirmできないようにする、など
                variableTextTMP.SetText("painted"); // <= only for filling in the box
            }
            paintCount++;
        }
        else if (brushHasWater) // then erase color on envObj and brush
        {
            envPartRend.material = originalEnvObjMaterial[partIdx];
            edittedEnvObjMaterial[partIdx] = originalEnvObjMaterial[partIdx];
            brushTipRend.material = originalBrushMaterial;
            brushHasWater = false;
            paintCount--;
            if (paintCount == 0)
            {
                variableTextTMP.SetText(string.Empty); // 重要
            }
        }
    }


    protected override void OnConfirm()
    {
        if (!isFocused) return;
        // hide props from the focused instance
        propObjects.SetActive(false);
    }

    protected override void OnBackToEdit()
    {
        if (!isFocused) return;
        // show props from the focused instance
        propObjects.SetActive(true);
    }


    // objArr.Length and matArr.Length should be the same
    void ApplyMaterial(ref GameObject[] objArr, Material[] matArr)
    {
        int nParts = objArr.Length;
        for (int i = 0; i < nParts; i++)
        {
            objArr[i].GetComponent<Renderer>().material = matArr[i];
        }
    }

    GameObject[] ConvertComponentArrayToGameObjectArray(Component[] compArr)
    {
        GameObject[] objArr = new GameObject[compArr.Length];
        int len = compArr.Length;
        for (int i = 0; i < len; i++)
            objArr[i] = compArr[i].gameObject;
        return objArr;
    }

    Material[] GetMaterialArray(GameObject[] objArr)
    {
        Material[] matArr = new Material[objArr.Length];
        int len = objArr.Length;
        for (int i = 0; i < len; i++)
            matArr[i] = objArr[i].GetComponent<Renderer>().material;
        return matArr;
    }


}


/*

// ideally, make brush tip "onTrigger" and do everything in OnTriggerBrushTip()
// check if triggered on envObj

if (!brushHasPaint && !brushHasWater) return; // nothing on brush
int partIdx = Array.IndexOf(envPartsGameObject, other.transform.gameObject);
if (partIdx == -1) return; // not found
Debug.Log("Collided with: " + other.transform.gameObject.name);
Renderer envPartRend = other.transform.gameObject.GetComponent<Renderer>();
if (brushHasPaint) // then put color on envbj
{
    envPartRend.material = brushTipRend.material;
    edittedEnvObjMaterial[partIdx] = brushTipRend.material;
}
else if (brushHasWater) // then erase color on envObj and brush
{
    envPartRend.material = originalEnvObjMaterial[partIdx];
    edittedEnvObjMaterial[partIdx] = originalEnvObjMaterial[partIdx];
    brushTipRend.material = originalBrushMaterial;
    brushHasWater = false;
}
 
 */