using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;

public class MakeSoundCard : OutputCard
{
    private GameObject micPropModel;
    private AudioSource soundRecorder;
    private Stopwatch stopWatch = new Stopwatch();
    private float clipDuration;
    private bool isRecording = false;
    private bool isExpanding = true;
    private float progress = 0.0f;
    private Vector3 originalScale;
    private Vector3 targetScale;
    private Vector3 updatedScale;

    readonly Vector3 expansionScale = new Vector3(1.1f, 1.1f, 1.1f);
    const float SPEED = 7.0f;



    public MakeSoundCard()
    {
        // executed before gameobjects are passed to this class instance
        cardName = "Make Sound";
    }

    protected override string SetDescriptionField()
    {
        return "(some descriptions)";
    }

    protected override string InitContentText()
    {
        return "Play <color=red>[(empty)]</color> " +
            "(grab microphone and press and hold \"A\" to record)";
    }

    protected override void InitPropFields()
    {
        micPropModel = propObjects.transform.Find("microphone").gameObject;
        originalScale = micPropModel.transform.localScale;
        targetScale = Vector3.Scale(originalScale, expansionScale);
    }



    public override void ConfirmOutputBehaviour()
    {
        isConfirmed = true;
        // another line here will probably be needed in other OutputCard subclasses
    }

    public override void UpdateOutputBehaviour()
    {
        if (isConfirmed)
        {
            return; // once confirmed, no more updates are needed for this perticular subclass
        }
        else
        {
            BehaviourDuringPrototyping();
        }
    }

    

    public override void OutputBehaviour()
    {
        if ((float)stopWatch.Elapsed.TotalSeconds >= clipDuration)
        {
            soundRecorder.Stop();
            stopWatch.Reset();
        }
        if (!soundRecorder.isPlaying)
        {
            soundRecorder.Play();
            stopWatch.Start();
        }
    }


    public override void OutputBehaviourNegative()
    {
        // todo Stop() when the inputCondition fails to hold true
    }


    protected override void BehaviourDuringPrototyping()
    {
        if (micPropModel.transform.GetComponent<OVRGrabbable>().isGrabbed) {
            if (OVRInput.GetDown(OVRInput.RawButton.A)) StartRecording();
            if (OVRInput.GetUp(OVRInput.RawButton.A)) SaveRecording();
        }

        if (OVRInput.GetDown(OVRInput.RawButton.X)) soundRecorder.Play();
        // todo design the UX for this interaction

        MicrophoneAnimation();
    }

    

    private void StartRecording()
    {
        soundRecorder = environmentObject.GetComponent<AudioSource>();
        stopWatch.Start();
        soundRecorder.clip = Microphone.Start("", false, 60, 16000);
        statementTMP.SetText("Play <color=red>[recording in process]</color>");
        isRecording = true;
    }

    private void SaveRecording()
    {
        Microphone.End("");
        stopWatch.Stop();
        clipDuration = (float)stopWatch.Elapsed.TotalSeconds;
        stopWatch.Reset();
        statementTMP.SetText("Play <color=red>[recorded]</color>");
        isRecording = false;
    }

    private void MicrophoneAnimation()
    {
        if (isRecording)
        {
            if (isExpanding) progress += SPEED * Time.deltaTime;
            else progress -= SPEED * Time.deltaTime;

            if (progress >= 1.0f)
            {
                progress = 1.0f;
                isExpanding = false;
            }
            else if (progress <= 0.0f)
            {
                progress = 0.0f;
                isExpanding = true;
            }

            updatedScale = Vector3.Lerp(originalScale, targetScale, progress);
            micPropModel.transform.localScale = updatedScale;
        }
        else
        {
            micPropModel.transform.localScale = originalScale;
        }
    }

}

/* 
 * int minFreq, maxFreq;
 * foreach (var device in Microphone.devices) {
 *      Debug.Log("Name: " + device);
 *      Microphone.GetDeviceCaps(device, out minFreq, out maxFreq);
 *      Debug.Log("minFreq: " + minFreq + ", maxFreq: " + maxFreq);
 * }
 * 
 * >>
 * 
 * Name: Android audio input
 * minFreq: 16000, maxFreq:16000
 * Name: Android camcorder input
 * minFreq: 16000, maxFreq:16000
 * Name: Android voice recognition input
 * minFreq: 16000, maxFreq:16000
 * 
 */
