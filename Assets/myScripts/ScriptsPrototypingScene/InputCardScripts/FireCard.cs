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
    private float markerDistance;
    private bool markerIsGrabbed = false;

    const float ALPHA_LOW = 0.2f;
    const float ALPHA_HIGH = 0.7f;
    const float BOUNDARY_SM = 1.0f;
    const float BOUNDARY_ML = 2.2f;

    public FireCard()
    {
        // executed before gameobjects are passed to this class instance
    }

    protected override string GetCardName() { return "Fire"; }

    protected override string InitDescriptionText() {
        return "If fire is <color=red>[(distance)]</color> " +
            "(grab fire and place at disired distance)"; }

    protected override void InitPropFields()
    {
        markerObj = propObjects.transform.Find("marker").gameObject;
        GameObject tmpCanvasObj = propObjects.transform.Find("floorCanvas").gameObject;
        rangeImageRed = tmpCanvasObj.transform.
            Find("tmpImageRed").gameObject.GetComponent<Image>();
        rangeImageBlue = tmpCanvasObj.transform.
            Find("tmpImageBlue").gameObject.GetComponent<Image>();
        rangeImageGreen = tmpCanvasObj.transform.
            Find("tmpImageGreen").gameObject.GetComponent<Image>();
        SetRangeOpacity(ALPHA_LOW, ALPHA_LOW, ALPHA_LOW);
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
            statementTMP.SetText(
                "If fire is <color=red>[short-distance]</color>");
        } else if (DetectDistanceLong()) {
            SetRangeOpacity(ALPHA_LOW, ALPHA_LOW, ALPHA_HIGH);
            statementTMP.SetText(
                "If fire is <color=red>[long-distance]</color>");
        } else if (DetectDistanceMid()) {
            SetRangeOpacity(ALPHA_LOW, ALPHA_HIGH, ALPHA_LOW);
            statementTMP.SetText(
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
