using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OutputMakeSound : MonoBehaviour
{
    private bool isRecording;
    private AudioSource soundRecorder;
    [SerializeField] private Button recordButton;
    [SerializeField] private TextMeshProUGUI whenDescription;
    private int tmpRecordNum;
    private bool tmpRecordingSaved;

    void Start() {
        isRecording = false;
        tmpRecordingSaved = false;
        recordButton.onClick.AddListener(RecordOutputSound);
        tmpRecordNum = 0;
    }

    void Update() {
    }

    void RecordOutputSound()
    {
        if (!isRecording) {
            if (!tmpRecordingSaved) {
                soundRecorder = GetComponent<AudioSource>();
                soundRecorder.clip = Microphone.Start("", false, 60, 16000);
                isRecording = true;
                tmpRecordNum++;
                whenDescription.text = "Play [recording in process...] (click again to stop&finalize)";
            } else {
                soundRecorder.Play();
            }
        } else { // TODO: enable re-recording of sound
            Microphone.End("");
            whenDescription.text = "Play Record#" + tmpRecordNum;
            isRecording = false;
            tmpRecordingSaved = true;
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