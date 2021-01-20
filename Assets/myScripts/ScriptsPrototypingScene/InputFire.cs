using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputFire : MonoBehaviour
{
    public GameObject environmentObject;
    public GameObject fireMarker;
    public Image rangeImageRed;
    public Image rangeImageBlue;
    public Image rangeImageGreen;
    public TextMeshProUGUI ifDescription;

    private bool markerIsGrabbed;
    private float markerDistance;

    // Start is called before the first frame update
    void Start()
    {
        markerIsGrabbed = false;
        ifDescription.text = "If fire is <color=red>[(distance)] (grab fire and place at disired distance)</color>";
        SetRangeOpacity(0.2f, 0.2f, 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        markerIsGrabbed = fireMarker.transform.GetComponent<OVRGrabbable>().isGrabbed;
        if (markerIsGrabbed || Input.GetKey(KeyCode.Z)) {
            markerDistance = Vector3.Distance(environmentObject.transform.position, fireMarker.transform.position);
            if (markerDistance < 1.0f) {
                SetRangeOpacity(1.0f, 0.2f, 0.2f);
                ifDescription.text = "If fire is <color=red>[short-distance]</color>";
            } else if (markerDistance > 2.2f) {
                SetRangeOpacity(0.2f, 0.2f, 1.0f);
                ifDescription.text = "If fire is <color=red>[long-distance]</color>";
            } else {
                SetRangeOpacity(0.2f, 1.0f, 0.2f);
                ifDescription.text = "If fire is <color=red>[mid-distance]</color>";
            }
        }
    }

    void SetRangeOpacity(float r, float b, float g)
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

// TODO: increase distance range (about 5?) (e.g. very close, close, mid, far, very far)