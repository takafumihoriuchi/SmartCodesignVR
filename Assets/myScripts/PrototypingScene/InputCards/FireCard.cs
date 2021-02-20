using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FireCard : InputCard
{
    private GameObject markerObj;

    private SpriteRenderer spriteInnerCore;
    private SpriteRenderer spriteOuterCore;
    private SpriteRenderer spriteMantle;
    private SpriteRenderer spriteCrust;
    private AudioSource fireSound;

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

    public bool DetectDistanceVeryClose() {
        UpdateMarkerDistance();
        UpdateGrabStatus();
        PlaySoundIfGrabbed();
        if (markerDistance < BOUNDARY_XSS) return true;
        else return false;
    }
    public bool DetectDistanceClose() {
        UpdateMarkerDistance();
        UpdateGrabStatus();
        PlaySoundIfGrabbed();
        if (markerDistance >= BOUNDARY_XSS && markerDistance <= BOUNDARY_SM) return true;
        else return false;
    }
    public bool DetectDistanceMiddle() {
        UpdateMarkerDistance();
        UpdateGrabStatus();
        PlaySoundIfGrabbed();
        if (markerDistance >= BOUNDARY_SM && markerDistance <= BOUNDARY_ML) return true;
        else return false;
    }
    public bool DetectDistanceFarAway() {
        UpdateMarkerDistance();
        UpdateGrabStatus();
        PlaySoundIfGrabbed();
        if (markerDistance > BOUNDARY_ML) return true;
        else return false;
    }

    private void UpdateMarkerDistance() {
        markerDistance = Vector3.Distance(environmentObject.transform.position, markerObj.transform.position);
    }
    private void UpdateGrabStatus() {
        markerIsGrabbed = markerObj.transform.GetComponent<OVRGrabbable>().isGrabbed;
    }
    private void PlaySoundIfGrabbed() {
        if (markerIsGrabbed && !fireSound.isPlaying) fireSound.Play();
        else if (!markerIsGrabbed && fireSound.isPlaying) fireSound.Stop();
    }


    protected override void InitPropFields()
    {
        markerObj = propObjects.transform.Find("marker").gameObject;
        fireSound = markerObj.GetComponent<AudioSource>();

        spriteInnerCore = propObjects.transform.Find("floorCanvas/distanceInnerCore").gameObject.GetComponent<SpriteRenderer>();
        spriteOuterCore = propObjects.transform.Find("floorCanvas/distanceOuterCore").gameObject.GetComponent<SpriteRenderer>();
        spriteMantle = propObjects.transform.Find("floorCanvas/distanceMantle").gameObject.GetComponent<SpriteRenderer>();
        spriteCrust = propObjects.transform.Find("floorCanvas/distanceCrust").gameObject.GetComponent<SpriteRenderer>();

        SetStrataOpacity(ALPHA_LOW, ALPHA_LOW, ALPHA_LOW, ALPHA_LOW);
    }

    public override void BehaviourDuringPrototyping()
    {
        UpdateMarkerDistance();
        UpdateGrabStatus();
        PlaySoundIfGrabbed();

        if (markerIsGrabbed || Input.GetKey(KeyCode.Z)) // 'Z' is for development purpose only
        {
            if (DetectDistanceVeryClose())
            {
                SetStrataOpacity(ALPHA_HIGH, ALPHA_LOW, ALPHA_LOW, ALPHA_LOW);
                ConditionKeyword = VERY_CLOSE; 
            }
            else if (DetectDistanceClose())
            {
                SetStrataOpacity(ALPHA_LOW, ALPHA_HIGH, ALPHA_LOW, ALPHA_LOW);
                ConditionKeyword = CLOSE;
            }
            else if (DetectDistanceMiddle())
            {
                SetStrataOpacity(ALPHA_LOW, ALPHA_LOW, ALPHA_HIGH, ALPHA_LOW);
                ConditionKeyword = MIDDLE;
            }
            else if (DetectDistanceFarAway())
            {
                SetStrataOpacity(ALPHA_LOW, ALPHA_LOW, ALPHA_LOW, ALPHA_HIGH);
                ConditionKeyword = FAR_AWAY;
            }
        }
    }


    private void SetStrataOpacity(float ic, float oc, float ma, float cr)
    {
        spriteInnerCore.color = new Color(1.0f, 1.0f, 1.0f, ic);
        spriteOuterCore.color = new Color(1.0f, 1.0f, 1.0f, oc);
        spriteMantle.color = new Color(1.0f, 1.0f, 1.0f, ma);
        spriteCrust.color = new Color(1.0f, 1.0f, 1.0f, cr);
    }


    protected override void OnFocusGranted()
    {
        SetStrataOpacity(ALPHA_LOW, ALPHA_LOW, ALPHA_LOW, ALPHA_LOW);
        return;
    }

    protected override void OnFocusDeprived()
    {
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
    

}


/*
 * 3D model for "markerObj (= fire)" was downloaded from:
 * https://poly.google.com/view/ceW6BDy5SAu
 */