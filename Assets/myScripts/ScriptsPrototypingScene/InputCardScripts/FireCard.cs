﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FireCard : InputCard
{

    public GameObject environmentObject;
    public GameObject markerObject;
    public TextMeshProUGUI ifDescription;
    public Image rangeImageRed;
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

    // TODO change from update to another method in this class
    void Update()
    {
        markerIsGrabbed = markerObject.transform.GetComponent<OVRGrabbable>().isGrabbed;
        if (markerIsGrabbed || Input.GetKey(KeyCode.Z))
        {
            markerDistance = Vector3.Distance(environmentObject.transform.position, markerObject.transform.position);
            if (markerDistance < 1.0f)
            {
                SetRangeOpacity(1.0f, 0.2f, 0.2f);
                ifDescription.text = "If fire is <color=red>[short-distance]</color>";
            }
            else if (markerDistance > 2.2f)
            {
                SetRangeOpacity(0.2f, 0.2f, 1.0f);
                ifDescription.text = "If fire is <color=red>[long-distance]</color>";
            }
            else
            {
                SetRangeOpacity(0.2f, 1.0f, 0.2f);
                ifDescription.text = "If fire is <color=red>[mid-distance]</color>";
            }
        }
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
