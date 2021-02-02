using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventBridge;
using System;

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
        eventBridgeHandler.TriggerEnter += OnTriggerEnterBrushTip;
        eventBridgeHandler.CollisionEnter += OnCollisionEnterBrushTip;
    }


    // when brush is dipped in the buckets
    void OnTriggerEnterBrushTip(Collider other)
    {
        string colliderName = other.transform.gameObject.name;
        if (colliderName == "LEDPaint") // check if triggered on paint bucket
        {
            brushHasPaint = true;
            brushHasWater = false;
            brushTipRend.material = other.GetComponent<Renderer>().material;
            return;
        }
        if (colliderName == "water") // check if triggered on water bucket
        {
            brushHasWater = true;
            brushHasPaint = false;
            brushTipRend.material = other.GetComponent<Renderer>().material;
            return;
        }
    }


    // when brush is rubbed on envObj
    void OnCollisionEnterBrushTip(Collision other)
    {
        if (!isFocused) return; // exclude unfocused instances

        // nothing on brush
        if (!brushHasPaint && !brushHasWater) return;

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

    // todo 相手の色を上書きしないように気を遣わなくてはいけない；
    // 最初に自分のnegative、その後（もしあれば）相手のpositive
    // todo PositiveとNegativeが切り替わったかは、set get の set で条件分岐で実現可能
    public override void OutputBehaviourOnPositive()
    {
        ApplyMaterial(ref envPartsGameObject, edittedEnvObjMaterial);
    }
    
    // TODO 複数インスタンスある時に、多分これだとピカピカ光っちゃう（他のとconflictしてスイッチングする）
    // LEDだからピカピカして欲しいならそれでもいいけど、点滅が早すぎてユーザーフレンドリーじゃないと思う
    public override void OutputBehaviourOnNegative()
    {
        ApplyMaterial(ref envPartsGameObject, originalEnvObjMaterial);
    }

    // objArr.Length and matArr.Length should be the same
    void ApplyMaterial(ref GameObject[] objArr, Material[] matArr)
    {
        int nParts = objArr.Length;
        for (int i = 0; i < nParts; i++)
        {
            objArr[i].GetComponent<Renderer>().material
                = matArr[i];
        }
    }

    protected override void BehaviourDuringPrototyping()
    {
        // all processes are taken care in the trigger-events
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


    // TODO 要検討；propsが複製なのか参照なのかで変わってくる
    // TODO 参照の場合、今の実装だとまずいことになるから、
    // （他のインスタンスで変更した色がこのインスタンスでも変更されてしまう）
    // （つまり、TriggerEnterなどを+=,-=しても、他のインスタンスでつけたらこっちでもつけたのと同じことになってしまう）
    // その場合は、SetActivateする前に、インスタンスごとにinstantiate()で自前のを用意して、
    // それをSetActivate()したのちに、Focusが移るたびに、SetActiveをtrueにしたりfalseにしたりする

    protected override void OnFocusGranted()
    {
        // load colors
        ApplyMaterial(ref envPartsGameObject, edittedEnvObjMaterial);
    }
    protected override void OnFocusDeprived()
    {
        // save colors
        // => already saved 'dynamically' during interaction
        // reset colors
        //ApplyMaterial(ref envPartsGameObject, originalEnvObjMaterial);
        // => こいつを消してしまえば、ロードした時にそのインスタンスの色に塗るだけだからうまくいく。
    }

    protected override void OnConfirm()
    {
        // propsを隠したい

        if (isFocused)
        {
            eventBridgeHandler.TriggerEnter -= OnTriggerEnterBrushTip;
            eventBridgeHandler.CollisionEnter -= OnCollisionEnterBrushTip;
        }

        return;
    }
    protected override void OnBackToEdit()
    {
        // propsを再登場させたい

        // todo propsをreferenceとして受け取るということで、全体を再検討する。
        // （referenceの方が無駄が少ない）
        if (isFocused)
        {
            eventBridgeHandler.TriggerEnter += OnTriggerEnterBrushTip;
            eventBridgeHandler.CollisionEnter += OnCollisionEnterBrushTip;
        }

        return;
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