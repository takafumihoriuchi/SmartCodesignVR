using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutputMakeSound : MonoBehaviour
{
    public Button recordButton;
    private bool isRecording;
    private AudioSource soundRecorder;

    void Start() {
        isRecording = false;
        recordButton.onClick.AddListener(RecordOutputSound);
    }

    void Update() {
    }

    void RecordOutputSound()
    {
        if (!isRecording)
        {
            soundRecorder = GetComponent<AudioSource>();
            //AudioSource.clip = Microphone.Start("Android audio input",false,60,16000);
            soundRecorder.clip = Microphone.Start("",false,60,16000);
            isRecording = true;
        } else
        {
            //soundRecorder.clip = Microphone.End("Android audio input");
            Microphone.End("");
            isRecording = false;
            soundRecorder.Play();
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