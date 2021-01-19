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

    void Start() {
        thenDescription.text = "Play <color=red>[(empty)] (grab microphone and press and hold \"A\" to record)</color>";
    }

    void Update() {
        bool micIsGrabbed = micPropModel.transform.GetComponent<OVRGrabbable>().isGrabbed;
        bool aGetDown = OVRInput.GetDown(OVRInput.RawButton.A);
        bool aGetUp = OVRInput.GetUp(OVRInput.RawButton.A);
        bool xGetDown = OVRInput.GetDown(OVRInput.RawButton.X);
        if (micIsGrabbed)
        {
            if (aGetDown) // start recording
            {
                soundRecorder = GetComponent<AudioSource>();
                soundRecorder.clip = Microphone.Start("", false, 60, 16000);
                thenDescription.text = "Play <color=red>[recording in process...] (release \"A\" to end recording)</color>";
            }
            if (aGetUp) // save recording
            {
                Microphone.End("");
                thenDescription.text = "Play <color=red>[recorded] (recorded sound can be checked by un-grabbing mic and pressing \"X\")</color>";
            }
        }
        if (xGetDown)
        {
            soundRecorder.Play();
            // TODO: currently for development; design an official way of checking the recorded.
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