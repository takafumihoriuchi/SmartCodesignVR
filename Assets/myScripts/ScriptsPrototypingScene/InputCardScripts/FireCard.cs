using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FireCard : InputCard
{
    private GameObject markerObj;
    private Image rangeImageRed;
    private Image rangeImageBlue;
    private Image rangeImageGreen;
    private bool markerIsGrabbed = false;
    private float markerDistance;

    const float ALPHA_LOW = 0.2f;
    const float ALPHA_HIGH = 0.7f;
    const float BOUNDARY_SM = 1.0f;
    const float BOUNDARY_ML = 2.2f;


    public FireCard()
    {
        // executed before gameobjects are passed to this class instance
    }

    // todo most of the code in this method can be moved to the parent class
    public override void SetInputCondition(ref GameObject envObj,
        ref GameObject inCardText, GameObject inCondBox, GameObject inProps)
    {
        environmentObject = envObj; // envObj is passed by reference -> no copy is made
        environmentObject.SetActive(true);

        inputSelectionText = inCardText;
        cardNameTMP = inputSelectionText.GetComponent<TextMeshPro>();
        cardNameTMP.SetText("Fire");
        inputSelectionText.SetActive(true);

        inputConditionBox = inCondBox;
        inputConditionTMP = inputConditionBox.transform.Find("DescriptionText").gameObject.GetComponent<TextMeshPro>();
        inputConditionTMP.SetText("If fire is <color=red>[(distance)] (grab fire and place at disired distance)</color>");
        inputConditionBox.SetActive(true);
        // need to adjust transform.position when PrototypingSceneCore.instIdx >= 1

        inputProps = inProps; // inProps is passed by value -> a copy is already made
        markerObj = inputProps.transform.Find("marker").gameObject;
        rangeImageRed = inputProps.transform.
            Find("tmpImageRed").gameObject.GetComponent<Image>();
        rangeImageBlue = inputProps.transform.
            Find("tmpImageBlue").gameObject.GetComponent<Image>();
        rangeImageGreen = inputProps.transform.
            Find("tmpImageGreen").gameObject.GetComponent<Image>();
        SetRangeOpacity(ALPHA_LOW, ALPHA_LOW, ALPHA_LOW);
        inputProps.SetActive(true);
        // todo SetActivate(true)がないとHiddenのままか；これでコピーが作られているかどうかを確認する
    }


    protected override InputConditionDelegate DetermineInputEvaluationDelegate()
    {
        if (markerDistance < BOUNDARY_SM) return DetectDistanceShort;
        else if (markerDistance > BOUNDARY_ML) return DetectDistanceLong;
        else return DetectDistanceMid;
    }

    protected override void UpdatesForInputConditionEvaluation()
    {
        markerDistance = Vector3.Distance(
                environmentObject.transform.position,
                markerObj.transform.position);
    }

    protected override void BehaviourDuringPrototyping()
    {
        markerIsGrabbed
            = markerObj.transform.GetComponent<OVRGrabbable>().isGrabbed;

        if (!markerIsGrabbed) return;

        if (DetectDistanceShort()) {
            SetRangeOpacity(ALPHA_HIGH, ALPHA_LOW, ALPHA_LOW);
            inputConditionTMP.SetText(
                "If fire is <color=red>[short-distance]</color>");
        } else if (DetectDistanceLong()) {
            SetRangeOpacity(ALPHA_LOW, ALPHA_LOW, ALPHA_HIGH);
            inputConditionTMP.SetText(
                "If fire is <color=red>[long-distance]</color>");
        } else if (DetectDistanceMid()) {
            SetRangeOpacity(ALPHA_LOW, ALPHA_HIGH, ALPHA_LOW);
            inputConditionTMP.SetText(
                "If fire is <color=red>[mid-distance]</color>");
        }
    }

    private bool DetectDistanceShort()
    {
        if (markerDistance < BOUNDARY_SM) return true;
        else return false;
    }

    private bool DetectDistanceLong()
    {
        if (markerDistance > BOUNDARY_ML) return true;
        else return false;
    }

    private bool DetectDistanceMid()
    {
        if (markerDistance >= BOUNDARY_SM && markerDistance <= BOUNDARY_ML)
            return true;
        else
            return false;
    }

    private void SetRangeOpacity(float r, float b, float g)
    {
        Color tmpColor;
        // red
        tmpColor = rangeImageRed.color;
        tmpColor.a = r;
        rangeImageRed.color = tmpColor;
        // blue
        tmpColor = rangeImageBlue.color;
        tmpColor.a = b;
        rangeImageBlue.color = tmpColor;
        // green
        tmpColor = rangeImageGreen.color;
        tmpColor.a = g;
        rangeImageGreen.color = tmpColor;
    }

}