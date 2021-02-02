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

    GameObject waterObject;
    Renderer waterRend;
    Material waterMaterial;

    Component[] envPartsComponent;
    GameObject[] envPartsGameObject;
    Material[] originalEnvObjMaterial;
    Material[] edittedEnvObjMaterial;


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

        waterObject = propObjects.transform.Find("WaterBucket/water").gameObject;
        waterRend = waterObject.GetComponent<Renderer>();
        waterMaterial = waterRend.material;

        envPartsComponent = environmentObject.GetComponentsInChildren<MeshRenderer>(true);
        envPartsGameObject = ConvertComponentArrayToGameObjectArray(envPartsComponent);
        originalEnvObjMaterial = GetMaterialArray(envPartsGameObject);
        edittedEnvObjMaterial = GetMaterialArray(envPartsGameObject);

        eventBridgeHandler = paintBrush.RequestEventHandlers(); // unique for each instance
    }


    /// <summary>
    /// during testing
    /// </summary>

    public override void OutputBehaviourOnNegative()
    {
        ApplyMaterial(ref envPartsGameObject, originalEnvObjMaterial);
    }
    // negative trigger for all instances is called before all positive triggers
    public override void OutputBehaviourOnPositive()
    {
        ApplyMaterial(ref envPartsGameObject, edittedEnvObjMaterial);
    }


    /// <summary>
    /// transition between prototyping and testing
    /// </summary>

    protected override void OnConfirm()
    {
        if (!isFocused) return;
        propObjects.SetActive(false);
    }

    protected override void OnBackToEdit()
    {
        if (!isFocused) return;
        propObjects.SetActive(true);
    }


    /// <summary>
    /// during prototyping
    /// </summary>

    // called every frame
    protected override void BehaviourDuringPrototyping() { }

    protected override void OnFocusGranted() {
        eventBridgeHandler.TriggerEnter += OnTriggerEnterBrushTip;
        eventBridgeHandler.CollisionEnter += OnCollisionEnterBrushTip;
        ApplyMaterial(ref envPartsGameObject, edittedEnvObjMaterial);
    }
    protected override void OnFocusDeprived() {
        eventBridgeHandler.TriggerEnter -= OnTriggerEnterBrushTip;
        eventBridgeHandler.CollisionEnter -= OnCollisionEnterBrushTip;
    }


    // when brush is dipped in the buckets
    // independent from other instances
    // c.f. "eventBridgeHandler" is instance independent
    void OnTriggerEnterBrushTip(Collider other)
    {
        string colliderName = other.transform.gameObject.name;
        if (colliderName == "LEDPaint" || colliderName == "water")
            brushTipRend.material = other.GetComponent<Renderer>().material;
    }


    // when brush is rubbed on envObj
    // independent from other instances
    // c.f. "eventBridgeHandler" is instance independent
    void OnCollisionEnterBrushTip(Collision other)
    {
        // reject if no paint/water is on the brush
        if (brushTipRend.material == originalBrushMaterial) return;

        // reject if the collided object is not a part of envObj
        int partIdx = Array.IndexOf(envPartsGameObject, other.gameObject);
        if (partIdx == -1) return; // not found
        Renderer envPartRend = other.gameObject.GetComponent<Renderer>();

        if (brushTipRend.material == waterMaterial)
        {
            envPartRend.material = originalEnvObjMaterial[partIdx];
            edittedEnvObjMaterial[partIdx] = originalEnvObjMaterial[partIdx];
            brushTipRend.material = originalBrushMaterial;
        }
        else // paint
        {
            envPartRend.material = brushTipRend.material;
            edittedEnvObjMaterial[partIdx] = brushTipRend.material;
        }

        if (ObjectHasPaintedParts()) variableTextTMP.SetText("painted");
        else variableTextTMP.SetText(string.Empty);
    }


    private bool ObjectHasPaintedParts()
    {
        int n = originalEnvObjMaterial.Length;
        for (int i = 0; i < n; i++)
            if (edittedEnvObjMaterial[i] != originalEnvObjMaterial[i])
                return true;
        return false;
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
