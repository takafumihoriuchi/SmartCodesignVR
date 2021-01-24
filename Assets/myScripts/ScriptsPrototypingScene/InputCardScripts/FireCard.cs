using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FireCard : InputCard
{
    public GameObject markerObject; // => instantiateしたほうがスケーラブル
    public TextMeshProUGUI ifDescription; // => 動的に生成するようにする
    public Image rangeImageRed; // 
    public Image rangeImageBlue;
    public Image rangeImageGreen;

    private bool markerIsGrabbed;
    private float markerDistance;


    public FireCard()
    {
        markerIsGrabbed = false;
        SetRangeOpacity(0.2f, 0.2f, 0.2f);
        ifDescription.text = "If fire is <color=red>[(distance)] (grab fire and place at disired distance)</color>";
    }


    public override void SetInputCondition(ref GameObject envObj)
    {
        environmentObject = envObj;
        // some other operations, such as bringing marker-objects, images, texts, ...
        // todo ここ、もしくはConstructorの中で、UIの「1コンテント」を作成する
    }


    protected override InputConditionDelegate SetToDelegate()
    {
        if (markerDistance < 1.0f) return DetectDistanceShort;
        else if (markerDistance > 2.2f) return DetectDistanceLong;
        else return DetectDistanceMid;
    }


    public override void UpdateInputCondition()
    {
        markerDistance = Vector3.Distance(
                environmentObject.transform.position,
                markerObject.transform.position);

        if (isConfirmed) inputCondition = InputConditionDefintion();
        else
        {
            if (markerDistance < 1.0f)
            {
                SetRangeOpacity(0.7f, 0.2f, 0.2f);
                ifDescription.text = "If fire is <color=red>[short-distance]</color>";
            }
            else if (markerDistance > 2.2f)
            {
                SetRangeOpacity(0.2f, 0.2f, 0.7f);
                ifDescription.text = "If fire is <color=red>[long-distance]</color>";
            }
            else
            {
                SetRangeOpacity(0.2f, 0.7f, 0.2f);
                ifDescription.text = "If fire is <color=red>[mid-distance]</color>";
            }
        }
    }


    private bool DetectDistanceShort()
    {
        if (markerDistance < 1.0f) return true;
        else return false;
    }

    private bool DetectDistanceLong()
    {
        if (markerDistance > 2.2f) return true;
        else return false;
    }

    private bool DetectDistanceMid()
    {
        if (markerDistance >= 1.0f && markerDistance <= 2.2f) return true;
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

}