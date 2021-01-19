using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OutputMakeSound : MonoBehaviour
{
    private AudioSource soundRecorder;
    public TextMeshProUGUI thenDescription;
    public GameObject micPropModel;

    private IEnumerator coroutineExpShrink;

    void Start() {
        coroutineExpShrink = MicExpandShrink();
        thenDescription.text = "Play <color=red>[(empty)] (grab microphone and press and hold \"A\" to record)</color>";
    }

    void Update() {
        bool micIsGrabbed = micPropModel.transform.GetComponent<OVRGrabbable>().isGrabbed;
        bool aGetDown = OVRInput.GetDown(OVRInput.RawButton.A);
        bool aGetUp = OVRInput.GetUp(OVRInput.RawButton.A);
        bool xGetDown = OVRInput.GetDown(OVRInput.RawButton.X);
        //if (micIsGrabbed)
        if (micIsGrabbed || Input.GetKey("space"))
        {
            //if (aGetDown) // start recording
            if (aGetDown || Input.GetKeyDown("A"))
            {
                soundRecorder = GetComponent<AudioSource>();
                soundRecorder.clip = Microphone.Start("", false, 60, 16000);
                thenDescription.text = "Play <color=red>[recording in process...] (release \"A\" to end recording)</color>";
                StartCoroutine(coroutineExpShrink);
            }
            //if (aGetUp) // save recording
            if (aGetUp || Input.GetKeyUp("A"))
            {
                Microphone.End("");
                thenDescription.text = "Play <color=red>[recorded] (recorded sound can be checked by pressing \"X\")</color>";
                StopCoroutine(coroutineExpShrink);
                micPropModel.transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
        //if (xGetDown) // test-play recording
        if (xGetDown || Input.GetKeyDown("X"))
        {
            soundRecorder.Play(); // TODO: current method only for development
        }
    }

    private IEnumerator MicExpandShrink() {
        float speed = 10.0f;
        float progress = 0.0f;
        Vector3 startScale = new Vector3(1.0f, 1.0f, 1.0f);
        Vector3 targetScale = new Vector3(1.25f, 1.25f, 1.25f);
        Vector3 updatedScale;
        bool isExpanding = true;
        while (true) {
            if (isExpanding) progress = progress + speed * Time.deltaTime;
            else progress = progress - speed * Time.deltaTime;
            if (progress >= 1.0f) {
                progress = 1.0f;
                isExpanding = false;
            } else if (progress <= 0) {
                progress = 0.0f;
                isExpanding = true;
            }
            updatedScale = Vector3.Lerp(startScale, targetScale, progress);
            micPropModel.transform.localScale = updatedScale;
        }
    }

}


/*
int minFreq, maxFreq;
foreach (var device in Microphone.devices) {
    Debug.Log("Name: " + device);
    Microphone.GetDeviceCaps(device, out minFreq, out maxFreq);
    Debug.Log("minFreq: " + minFreq + ", maxFreq: " + maxFreq);
}
Name: Android audio input
minFreq: 16000, maxFreq:16000
Name: Android camcorder input
minFreq: 16000, maxFreq:16000
Name: Android voice recognition input
minFreq: 16000, maxFreq:16000
 */