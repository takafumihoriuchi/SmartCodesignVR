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
    GameObject[] envPartsGameObject; // used for output-behaviour
    Material[] originalEnvObjMaterial;
    Material[] edittedEnvObjMaterial; // used for output-behaviour

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
        edittedEnvObjMaterial = GetMaterialArray(envPartsGameObject);

        eventBridgeHandler = paintBrush.RequestEventHandlers();
        eventBridgeHandler.TriggerEnter += OnTriggerEnterBrushTip;
        //eventBridgeHandler.CollisionEnter += OnCollisionEnterBrushTip;
    }



    void OnTriggerEnterBrushTip(Collider other)
    {
        string colliderName = other.transform.gameObject.name;

        // check if triggered on paint bucket
        if (colliderName == "LEDPaint")
        {
            brushHasPaint = true;
            brushHasWater = false;
            brushTipRend.material = other.GetComponent<Renderer>().material;
            return;
        }
        // check if triggered on water bucket
        if (colliderName == "water")
        {
            brushHasWater = true;
            brushHasPaint = false;
            brushTipRend.material = other.GetComponent<Renderer>().material;
            return;
        }

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
    }


    // when brush is rubbed on envObj
    //void OnCollisionEnterBrushTip(Collision other)
    //{
    //    // nothing on brush
    //    if (!brushHasPaint && !brushHasWater) return;

    //    int partIdx = Array.IndexOf(envPartsGameObject, other.gameObject);
    //    if (partIdx == -1) return; // not found
    //    Renderer envPartRend = other.gameObject.GetComponent<Renderer>();

    //    if (brushHasPaint) // then put color on envbj
    //    {
    //        envPartRend.material = brushTipRend.material;
    //        edittedEnvObjMaterial[partIdx] = brushTipRend.material;
    //    }
    //    else if (brushHasWater) // then erase color on envObj and brush
    //    {
    //        envPartRend.material = originalEnvObjMaterial[partIdx];
    //        edittedEnvObjMaterial[partIdx] = originalEnvObjMaterial[partIdx];
    //        brushTipRend.material = originalBrushMaterial;
    //        brushHasWater = false;
    //    }
    //}



    public override void ConfirmOutputBehaviour()
    {
        isConfirmed = true;
        eventBridgeHandler.TriggerEnter -= OnTriggerEnterBrushTip;
        //eventBridgeHandler.CollisionEnter -= OnCollisionEnterBrushTip;
        // todo when confirmation is canceled, add listeners again
    }


    public override void UpdateOutputBehaviour()
    {
        // all process taken care in trigger-events
    }


    public override void OutputBehaviour()
    {
        //int nParts = envPartsGameObject.Length;
        //for (int i = 0; i < nParts; i++)
        //{
        //    envPartsGameObject[i].GetComponent<Renderer>().material
        //        = edittedEnvObjMaterial[i];
        //}
        ApplyMaterial(ref envPartsGameObject, edittedEnvObjMaterial);
    }
    

    public override void OutputBehaviourNegative()
    {
        //int nParts = envPartsGameObject.Length;
        //for (int i = 0; i < nParts; i++)
        //{
        //    envPartsGameObject[i].GetComponent<Renderer>().material
        //        = originalEnvObjMaterial[i];
        //}
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
        // all process taken care in trigger-events
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


// "MonoBehaviour.OnDestroy()" <= cannot be used; not subclass of MonoBehaviour
// todo insert these to disable the trigger events of brushTip
// eventBridgeHandler.TriggerEnter -= OnTriggerEnterBrushTip;
// eventBridgeHandler.CollisionEnter -= OnCollisionEnterBrushTip; 
