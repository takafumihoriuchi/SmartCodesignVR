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
        descriptionText =
            "<i>I can produce sound or play music.</i>\n" +
            "<b>Steps:</b>\n" +
            "<b>1.</b> <indent=10%>Grab the microphone.</indent>\n" +
            "<b>2.</b> <indent=10%>Press and hold A-button to start recording.</indent>\n" +
            "<b>3.</b> <indent=10%>Release A-button to finish recording.</indent>\n" +
            "<b>4.</b> <indent=10%>Check recorded sound by pressing X-button.</indent>\n" +
            "Sound can be re-recorded by following the same steps.";
        contentText = "play the recorded sound.";
    }

    protected override void InitPropFields()
    {
        micPropModel = propObjects.transform.Find("microphone").gameObject;
        originalScale = micPropModel.transform.localScale;
        targetScale = Vector3.Scale(originalScale, expansionScale);
    }


    // todo OutputBehaviourOnNegativeでstopされるまではループさせる、など
    public override void OutputBehaviour()
    {
        if ((float)stopWatch.Elapsed.TotalSeconds >= clipDuration)
        {
            // OutputBehaviourをトリガー生にした場合、コルーチンで1プレイサイクルを区切ることになる。
            soundRecorder.Stop();
            stopWatch.Reset();
            // Stopwatch.Reset: Stops time interval measurement and resets the elapsed time to zero.
        }
        if (!soundRecorder.isPlaying)
        {
            soundRecorder.Play();
            stopWatch.Start();
        }
    }


    public override void OutputBehaviourNegative()
    {
        // Stop() when the inputCondition fails to hold true
        if (soundRecorder.isPlaying) soundRecorder.Stop();
    }

    // only called from core when isFocused is true
    protected override void BehaviourDuringPrototyping()
    {
        if (micPropModel.transform.GetComponent<OVRGrabbable>().isGrabbed) {
            if (OVRInput.GetDown(OVRInput.RawButton.A)) StartRecording();
            if (OVRInput.GetUp(OVRInput.RawButton.A))　SaveRecording();
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
        variableTextTMP.SetText("[recording...]");
        isRecording = true;
    }

    private void SaveRecording()
    {
        Microphone.End("");
        stopWatch.Stop();
        clipDuration = (float)stopWatch.Elapsed.TotalSeconds;
        stopWatch.Reset();
        variableTextTMP.SetText("[recorded]");
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

    // このクラスについては、OnFocusedで行う操作は特にない；動作はカードごとに違う。
    // todo propsが全て同じものなのか、別物なのか、
    // それによって次の4つのメソッドの処理は変わる
    protected override void OnFocusGranted() { return; }
    protected override void OnFocusDeprived() { return; }
    protected override void OnConfirm() { return; }
    protected override void OnBackToEdit() { return; }

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
