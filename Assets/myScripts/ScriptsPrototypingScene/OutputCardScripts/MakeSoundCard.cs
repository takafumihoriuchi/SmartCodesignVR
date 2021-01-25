using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MakeSoundCard : OutputCard
{
    private GameObject micPropModel;
    private AudioSource soundRecorder;
    private bool isRecording = false;
    private bool isExpanding = true;
    private float progress = 0.0f;
    private Vector3 updatedScale;

    readonly Vector3 startScale = new Vector3(1.0f, 1.0f, 1.0f);
    readonly Vector3 targetScale = new Vector3(1.1f, 1.1f, 1.1f);
    const float SPEED = 7.0f;


    public MakeSoundCard()
    {
        // executed before gameobjects are passed to this class instance
    }

    protected override string GetCardName() { return "Make Sound"; }

    protected override string InitDescriptionText()
    {
        return "Play <color=red>[(empty)] " +
            "(grab microphone and press and hold \"A\" to record)</color>";
    }

    protected override void InitPropFields()
    {
        micPropModel = propObjects.transform.Find("microphone").gameObject;
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
        if (!soundRecorder.isPlaying) soundRecorder.Play();
    }


    protected override void BehaviourDuringPrototyping()
    {
        if (micPropModel.transform.GetComponent<OVRGrabbable>().isGrabbed) {
            if (OVRInput.GetDown(OVRInput.RawButton.A)) StartRecording();
            if (OVRInput.GetUp(OVRInput.RawButton.A)) SaveRecording();
        }

        if (OVRInput.GetDown(OVRInput.RawButton.X)) soundRecorder.Play();

        MicrophoneAnimation();
    }

    

    private void StartRecording()
    {
        soundRecorder = micPropModel.GetComponent<AudioSource>();
        soundRecorder.clip = Microphone.Start("", false, 60, 16000);
        statementTMP.SetText("Play <color=red>[recording in process...] (release \"A\" to end recording)</color>");
        isRecording = true;
    }

    private void SaveRecording()
    {
        Microphone.End("");
        statementTMP.SetText("Play <color=red>[recorded] (recorded sound can be checked by pressing \"X\")</color>");
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

            updatedScale = Vector3.Lerp(startScale, targetScale, progress);
            micPropModel.transform.localScale = updatedScale;
        }
        else
        {
            micPropModel.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
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