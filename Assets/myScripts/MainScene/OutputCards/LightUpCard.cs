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
    Material defaultBrushTipMaterial;

    GameObject waterObject;
    Renderer waterRend;
    Material waterMaterial;

    Material[] edittedMaterials;


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
        paintBrush = propObj.transform.Find("PaintBrush").gameObject;
        brushTip = paintBrush.transform.Find("brushTip").gameObject;
        brushTipRend = brushTip.GetComponent<Renderer>();
        defaultBrushTipMaterial = brushTipRend.material;

        waterObject = propObj.transform.Find("WaterBucket/water").gameObject;
        waterRend = waterObject.GetComponent<Renderer>();
        waterMaterial = waterRend.material;

        edittedMaterials = smartObj.defaultMaterials;

        eventBridgeHandler = paintBrush.RequestEventHandlers(); // unique for each instance
    }


    /// <summary>
    /// during testing
    /// </summary>

    // negative trigger for all instances is called before all positive triggers
    public override void OutputBehaviourOnNegative()
    {
        smartObj.ApplyMaterials(smartObj.defaultMaterials);
    }

    public override void OutputBehaviourOnPositive()
    {
        smartObj.ApplyMaterials(edittedMaterials);
    }

    /// <summary>
    /// transition between prototyping and preview
    /// </summary>

    protected override void OnConfirm()
    {
        if (isFocused)
        {
            propObj.SetActive(false);
            smartObj.ApplyMaterials(smartObj.defaultMaterials);
        }
    }

    protected override void OnBackToEdit()
    {
        if (isFocused)
        {
            propObj.SetActive(true);
            smartObj.ApplyMaterials(edittedMaterials);
        }
    }


    /// <summary>
    /// during prototyping
    /// </summary>

    // called every frame
    // todo => SmartObject class の Update() で同じことができるのでは。他のクラスでは実態がある。
    public override void BehaviourDuringPrototyping() { }

    protected override void OnFocusGranted() {
        eventBridgeHandler.TriggerEnter += OnTriggerEnterBrushTip;
        eventBridgeHandler.CollisionEnter += OnCollisionEnterBrushTip;
        smartObj.ApplyMaterials(edittedMaterials);
    }

    protected override void OnFocusDeprived() {
        eventBridgeHandler.TriggerEnter -= OnTriggerEnterBrushTip;
        eventBridgeHandler.CollisionEnter -= OnCollisionEnterBrushTip;
    }

    // when brush is dipped in the buckets
    // independent from other instances (c.f. "eventBridgeHandler" is instance independent)
    void OnTriggerEnterBrushTip(Collider other)
    {
        string colliderName = other.transform.gameObject.name;
        if (colliderName == "LEDPaint" || colliderName == "water")
            brushTipRend.material = other.GetComponent<Renderer>().material;
    }

    // when brush is rubbed on smartObj
    // independent from other instances (c.f. "eventBridgeHandler" is instance independent)
    void OnCollisionEnterBrushTip(Collision other)
    {
        // reject if no paint/water is on the brush
        if (brushTipRend.material == defaultBrushTipMaterial) return;

        // reject if the collided object is not a part of envObj
        int partIdx = Array.IndexOf(smartObj.meshRendGameObjects, other.gameObject);
        if (partIdx == -1) return; // not found
        Renderer envPartRend = other.gameObject.GetComponent<Renderer>();

        if (brushTipRend.sharedMaterial.name.Contains(waterMaterial.name)) // is water
        {
            envPartRend.material = smartObj.defaultMaterials[partIdx];
            edittedMaterials[partIdx] = smartObj.defaultMaterials[partIdx];
            brushTipRend.material = defaultBrushTipMaterial;
        }
        else // is paint
        {
            envPartRend.material = brushTipRend.material;
            edittedMaterials[partIdx] = brushTipRend.material;
        }

        if (ObjectHasPaintedParts()) ConditionKeyword = "painted";
        else variableTextTMP.SetText(string.Empty);
    }


    private bool ObjectHasPaintedParts()
    {
        int n = smartObj.defaultMaterials.Length;
        for (int i = 0; i < n; i++)
            if (edittedMaterials[i] != smartObj.defaultMaterials[i])
                return true;
        return false;
    }


}
