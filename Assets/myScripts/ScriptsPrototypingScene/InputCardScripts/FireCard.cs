using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FireCard : InputCard
{
    private GameObject markerObj;

    private SpriteRenderer spriteInnerCore;
    private SpriteRenderer spriteOuterCore;
    private SpriteRenderer spriteMantle;
    private SpriteRenderer spriteCrust;

    private float markerDistance;
    private bool markerIsGrabbed = false;

    const float BOUNDARY_XSS = 0.8f;
    const float BOUNDARY_SM = 1.6f;
    const float BOUNDARY_ML = 2.4f;

    readonly string VERY_CLOSE = "very close";
    readonly string CLOSE = "close";
    readonly string MIDDLE = "middle";
    readonly string FAR_AWAY = "far away";

    const float ALPHA_LOW = 0.1f;
    const float ALPHA_HIGH = 0.4f;

    public FireCard()
    {
        maxInstanceNum = 4;

        cardName = "Fire";

        descriptionText =
            "<i>I can detect the presence (i.e. distance) of fire.</i>\n" +
            "<b>Steps:</b>\n" +
            "<b>1.</b> <indent=10%>Grab the fire by holding the trigger on controller.</indent>\n" +
            "<b>2.</b> <indent=10%>Move it near/away to the object.</indent>\n" +
            "<b>3.</b> <indent=10%>Release the fire on the ground.</indent>\n" +
            "Maximum number of instances: 4";

        contentText = "When I see fire in distance";

        inputEvalDeleDict = new Dictionary<string, InputEvaluationDelegate>
        {
            {VERY_CLOSE, DetectDistanceVeryClose},
            {CLOSE, DetectDistanceClose},
            {MIDDLE, DetectDistanceMiddle},
            {FAR_AWAY, DetectDistanceFarAway}
        };
    }

    private bool DetectDistanceVeryClose() {
        UpdateMarkerDistance();
        if (markerDistance < BOUNDARY_XSS) return true;
        else return false;
    }
    private bool DetectDistanceClose() {
        UpdateMarkerDistance();
        if (markerDistance >= BOUNDARY_XSS && markerDistance <= BOUNDARY_SM) return true;
        else return false;
    }
    private bool DetectDistanceMiddle() {
        UpdateMarkerDistance();
        if (markerDistance >= BOUNDARY_SM && markerDistance <= BOUNDARY_ML) return true;
        else return false;
    }
    private bool DetectDistanceFarAway() {
        UpdateMarkerDistance();
        if (markerDistance > BOUNDARY_ML) return true;
        else return false;
    }

    private void UpdateMarkerDistance() {
        markerDistance = Vector3.Distance(environmentObject.transform.position, markerObj.transform.position);
    }

    protected override void InitPropFields()
    {
        markerObj = propObjects.transform.Find("marker").gameObject;

        spriteInnerCore = propObjects.transform.Find("floorCanvas/distanceInnerCore").gameObject.GetComponent<SpriteRenderer>();
        spriteOuterCore = propObjects.transform.Find("floorCanvas/distanceOuterCore").gameObject.GetComponent<SpriteRenderer>();
        spriteMantle = propObjects.transform.Find("floorCanvas/distanceMantle").gameObject.GetComponent<SpriteRenderer>();
        spriteCrust = propObjects.transform.Find("floorCanvas/distanceCrust").gameObject.GetComponent<SpriteRenderer>();

        SetStrataOpacity(ALPHA_LOW, ALPHA_LOW, ALPHA_LOW, ALPHA_LOW);
    }

    protected override void BehaviourDuringPrototyping()
    {
        UpdateMarkerDistance();

        markerIsGrabbed = markerObj.transform.GetComponent<OVRGrabbable>().isGrabbed;

        if (markerIsGrabbed || Input.GetKey(KeyCode.Z)) // 'Z' is for development purpose only
        {
            if (DetectDistanceVeryClose())
            {
                SetStrataOpacity(ALPHA_HIGH, ALPHA_LOW, ALPHA_LOW, ALPHA_LOW);
                conditionKeyword = VERY_CLOSE; 
            }
            else if (DetectDistanceClose())
            {
                SetStrataOpacity(ALPHA_LOW, ALPHA_HIGH, ALPHA_LOW, ALPHA_LOW);
                conditionKeyword = CLOSE;
            }
            else if (DetectDistanceMiddle())
            {
                SetStrataOpacity(ALPHA_LOW, ALPHA_LOW, ALPHA_HIGH, ALPHA_LOW);
                conditionKeyword = MIDDLE;
            }
            else if (DetectDistanceFarAway())
            {
                SetStrataOpacity(ALPHA_LOW, ALPHA_LOW, ALPHA_LOW, ALPHA_HIGH);
                conditionKeyword = FAR_AWAY;
            }
            variableTextTMP.SetText(conditionKeyword);
        }
    }


    private void SetStrataOpacity(float ic, float oc, float ma, float cr)
    {
        spriteInnerCore.color = new Color(1.0f, 1.0f, 1.0f, ic);
        spriteOuterCore.color = new Color(1.0f, 1.0f, 1.0f, oc);
        spriteMantle.color = new Color(1.0f, 1.0f, 1.0f, ma);
        spriteCrust.color = new Color(1.0f, 1.0f, 1.0f, cr);
    }


    // todo 床に描画してるSpriteが参照型で全て同じものなのか、別物なのか、
    // それによって次の4つのメソッドの処理は変わる
    // 現状の下のメソッドでは「コピーが複数存在する」と考えて実装している。
    // TODO => 参照の方が都合がいいから、参照型で実装する。そういうふうに実装する。
    // => 現状では、propをrefで受け取っているから、参照になっている。 todo 要対応
    // 現状では完全に参照だ
    protected override void OnFocusGranted()
    {
        SetStrataOpacity(ALPHA_LOW, ALPHA_LOW, ALPHA_LOW, ALPHA_LOW);
        return;
    }
    protected override void OnFocusDeprived()
    {
        // 他のインスタンスの色spriteと干渉しちゃうから、offにする（alphaを0にする）
        // => 現状ではrefで受け取っているから、正確にはこれは必要ない処理
        // SetStrataOpacity(0.0f, 0.0f, 0.0f, 0.0f);
        return;
    }

    protected override void OnConfirm()
    {
        if (isFocused) SetStrataOpacity(0.0f, 0.0f, 0.0f, 0.0f);
    }
    protected override void OnBackToEdit()
    {
        if (isFocused) SetStrataOpacity(ALPHA_LOW, ALPHA_LOW, ALPHA_LOW, ALPHA_LOW);
    }
    // 参照渡しを念頭に置いて設計すればこれらメソッドは要らないかもしれない
    

}
