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
        
    }

    public override void UpdateOutputBehaviour()
    {
        
    }

    public override void OutputBehaviour()
    {
        soundRecorder.Play();
    }



    void Update()
    {
        bool micIsGrabbed = micPropModel.transform.GetComponent<OVRGrabbable>().isGrabbed;
        bool aGetDown = OVRInput.GetDown(OVRInput.RawButton.A);
        bool aGetUp = OVRInput.GetUp(OVRInput.RawButton.A);
        bool xGetDown = OVRInput.GetDown(OVRInput.RawButton.X);

        if (micIsGrabbed || Input.GetKey(KeyCode.Z))
        //if (micIsGrabbed)
        {
            if (aGetDown || Input.GetKeyDown(KeyCode.A))
            //if (aGetDown) // start recording
            {
                soundRecorder = micPropModel.GetComponent<AudioSource>();
                soundRecorder.clip = Microphone.Start("", false, 60, 16000);
                outputBehaviourTMP.SetText("Play <color=red>[recording in process...] (release \"A\" to end recording)</color>");
                isRecording = true;
            }
            if (aGetUp || Input.GetKeyUp(KeyCode.A))
            //if (aGetUp) // save recording
            {
                Microphone.End("");
                outputBehaviourTMP.SetText("Play <color=red>[recorded] (recorded sound can be checked by pressing \"X\")</color>");
                isRecording = false;
            }
        }
        if (xGetDown || Input.GetKeyDown(KeyCode.X))
        //if (xGetDown) // test-play recording
        {
            soundRecorder.Play(); // TODO: current method only for development
        }


        if (isRecording)
        { // TODO: needs refactoring (e.g. coroutine)
            if (isExpanding)
                progress += SPEED * Time.deltaTime;
            else
                progress -= SPEED * Time.deltaTime;
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


/*
// attempt to use subroutine

private bool isRunningCoroutine;
...
Start() {
    StartCoroutine(coroutineExpShrink);
}
...
Update() {
    if (...) {
        StartCoroutine(coroutineExpShrink);
    } else {
        StopCoroutine(coroutineExpShrink);
    }
}

private IEnumerator MicExpandShrink()
{
    float speed = 2.0f;
    float progress = 0.0f;
    Vector3 startScale = new Vector3(1.0f, 1.0f, 1.0f);
    Vector3 targetScale = new Vector3(1.25f, 1.25f, 1.25f);
    Vector3 updatedScale;
    bool isExpanding = true;
    while (isRecording)
    {
        Debug.Log("recording now... (and changing scales)");
        yield return new WaitForSeconds(1.0f);
        if (isExpanding) progress = progress + speed * Time.deltaTime;
        else progress = progress - speed * Time.deltaTime;
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
    micPropModel.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    Debug.Log("isRecording is false now");
    yield break;
}

 */