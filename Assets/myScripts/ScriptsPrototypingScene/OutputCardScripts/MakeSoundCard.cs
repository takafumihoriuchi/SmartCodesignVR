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
    // todo どの変数がローカルメンバでもいいかを再検討する

    readonly Vector3 startScale = new Vector3(1.0f, 1.0f, 1.0f);
    readonly Vector3 targetScale = new Vector3(1.1f, 1.1f, 1.1f);
    const float SPEED = 7.0f;


    public MakeSoundCard()
    {
        // executed before gameobjects are passed to this class instance
    }

    // todo most of the code in this method can be moved to the parent class
    public override void SetOutputBehaviour(ref GameObject envObj,
        ref GameObject outCardText, GameObject outBehavBox, GameObject outProps)
    {
        environmentObject = envObj;
        environmentObject.SetActive(true);

        outputSelectionText = outCardText;
        cardNameTMP = outputSelectionText.GetComponent<TextMeshPro>();
        cardNameTMP.SetText("Make Sound"); // string型の変数を用意すれば対処可能
        outputSelectionText.SetActive(true);

        outputBehaviourBox = outBehavBox;
        outputBehaviourTMP = outputBehaviourBox.transform.Find("DescriptionText").gameObject.GetComponent<TextMeshPro>();
        outputBehaviourTMP.SetText("Play <color=red>[(empty)] (grab microphone and press and hold \"A\" to record)</color>");
        outputBehaviourBox.SetActive(true);

        outputProps = outProps;
        micPropModel = outputProps.transform.Find("microphone").gameObject;
        outputProps.SetActive(true);
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
        if (micPropModel.transform.GetComponent<OVRGrabbable>().isGrabbed)
        {
            if (OVRInput.GetDown(OVRInput.RawButton.A)) // start recording
            {
                soundRecorder = micPropModel.GetComponent<AudioSource>();
                soundRecorder.clip = Microphone.Start("", false, 60, 16000);
                outputBehaviourTMP.SetText("Play <color=red>[recording in process...] (release \"A\" to end recording)</color>");
                isRecording = true;
            }
            if (OVRInput.GetUp(OVRInput.RawButton.A)) // save recording
            {
                Microphone.End("");
                outputBehaviourTMP.SetText("Play <color=red>[recorded] (recorded sound can be checked by pressing \"X\")</color>");
                isRecording = false;
            }
        }

        if (OVRInput.GetDown(OVRInput.RawButton.X)) // play recording
        {
            soundRecorder.Play();
        }

        MicrophoneAnimation();
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