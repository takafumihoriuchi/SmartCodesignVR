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

    const float ALPHA_LOW = 0.1f;
    const float ALPHA_HIGH = 0.5f;
    const float BOUNDARY_SM = 1.0f;
    const float BOUNDARY_ML = 2.2f;

    public FireCard()
    {
        maxInstanceNum = 3; // setting unique for InputCards
        cardName = "Fire";
        descriptionText =
            //"Specifies the distance of fire from the object." +
            "<i>I can detect the presence of fire.</i>\n" +
            "<b>Steps:</b>\n" +
            "<b>1.</b> <indent=10%>Grab the fire by holding the trigger on controller.</indent>\n" +
            "<b>2.</b> <indent=10%>Move it near/away to the object.</indent>\n" +
            "<b>3.</b> <indent=10%>Release the fire on the ground.</indent>\n" +
            "Maximum number of instances: 3";
        contentText = "When I see fire in distance";
    }

    protected override void InitPropFields()
    {
        markerObj = propObjects.transform.Find("marker").gameObject;
        GameObject tmpCanvasObj = propObjects.transform.Find("floorCanvas").gameObject; // *
        rangeImageRed = tmpCanvasObj.transform.
            Find("tmpImageRed").gameObject.GetComponent<Image>(); // *
        rangeImageBlue = tmpCanvasObj.transform.
            Find("tmpImageBlue").gameObject.GetComponent<Image>(); // *
        rangeImageGreen = tmpCanvasObj.transform.
            Find("tmpImageGreen").gameObject.GetComponent<Image>(); // *
        SetRangeOpacity(ALPHA_LOW, ALPHA_LOW, ALPHA_LOW);
        // todo * can be rewritten as: rangeImageRed = propObjects.transform.Find("floorCanvas/tmpImageRed").gameObject.GetComponent<Image>();
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
            variableTextTMP.SetText("very close");
        } else if (DetectDistanceLong()) {
            SetRangeOpacity(ALPHA_LOW, ALPHA_LOW, ALPHA_HIGH);
            variableTextTMP.SetText("close by");
        } else if (DetectDistanceMid()) {
            SetRangeOpacity(ALPHA_LOW, ALPHA_HIGH, ALPHA_LOW);
            variableTextTMP.SetText("far away");
        }

    }

    private bool DetectDistanceShort()
    {
        if (markerDistance < BOUNDARY_SM) return true;
        else return false;
    }

    private bool DetectDistanceMid()
    {
        if (markerDistance >= BOUNDARY_SM && markerDistance <= BOUNDARY_ML)
            return true;
        else
            return false;
    }

    private bool DetectDistanceLong()
    {
        if (markerDistance > BOUNDARY_ML) return true;
        else return false;
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


    // todo 床に描画してるSpriteが参照型で全て同じものなのか、別物なのか、
    // それによって次の4つのメソッドの処理は変わる
    // 現状の下のメソッドでは「コピーが複数存在する」と考えて実装している。
    // TODO => 参照の方が都合がいいから、参照型で実装する。そういうふうに実装する。
    protected override void OnFocusGranted()
    {
        SetRangeOpacity(ALPHA_LOW, ALPHA_LOW, ALPHA_LOW);
        return;
    }
    protected override void OnFocusDeprived()
    {
        // 他のインスタンスの色spriteと干渉しちゃうから、offにする（alphaを0にする）
        SetRangeOpacity(0.0f, 0.0f, 0.0f);
        return;
    }

    protected override void OnConfirm()
    {
        // TODO
        InputConditionDefinition = DetermineInputEvaluationDelegate(); // todo 全てのinputcardに必要な重要な命令だから、ここに置くべきではない。InputCard classに移すこと。
        // TODO

        if (isFocused) SetRangeOpacity(0.0f, 0.0f, 0.0f);
    }
    protected override void OnBackToEdit()
    {
        if (isFocused) SetRangeOpacity(ALPHA_LOW, ALPHA_LOW, ALPHA_LOW);
    }

    

}
