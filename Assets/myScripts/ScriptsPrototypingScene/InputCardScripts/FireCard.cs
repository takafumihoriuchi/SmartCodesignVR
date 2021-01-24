﻿using System.Collections;
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
    }

    public override void ConfirmInputCondition()
    {
        if (markerDistance < 1.0f)
        {
            inputCondition  = ; // 毎回Updateしなくてはいけない機能
        }
        else if (markerDistance > 2.2f)
        {
            
        }
        else
        {
            
        }
        inputCondition = ;
    }

    public override void UpdateInputCondition()
    {
        markerIsGrabbed = markerObject.transform.GetComponent<OVRGrabbable>().isGrabbed;

        if (markerIsGrabbed)
        {
            markerDistance = Vector3.Distance(environmentObject.transform.position, markerObject.transform.position);
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

        // inputCondition = xxx;
        // 何かが違う…；ここでは、発火条件が何かをdefineしたい。
        // ここでは、マーカーがどこに置かれたかで条件が決まる。
        // 置かれた状態で確定ボタンが押される必要がある。（<= 重要）
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
