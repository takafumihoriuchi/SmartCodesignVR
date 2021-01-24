using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MakeSoundCard : OutputCard
{
    // TODO refactor all the scripts below (currently just a copy&paste from old file)

    // コンストラクタ
    public MakeSoundCard() { }

    public override void SetOutputBehaviour(ref GameObject envObj, ref GameObject outCardText, GameObject outBehavBox, GameObject outProps)
    {
        environmentObject = envObj;
        outputProps = outProps;
    }

    public override void ConfirmOutputBehaviour()
    {
        
    }

    public override void UpdateOutputBehaviour()
    {
        
    }

    public override void OutputBehaviour()
    {

    }



    private AudioSource soundRecorder;
    public TextMeshProUGUI thenDescription;
    public GameObject micPropModel;

    private bool isRecording;
    private float speed;
    private float progress;
    private Vector3 startScale;
    private Vector3 targetScale;
    private Vector3 updatedScale;
    private bool isExpanding;

    void Start()
    {
        speed = 7.0f;
        progress = 0.0f;
        startScale = new Vector3(1.0f, 1.0f, 1.0f);
        targetScale = new Vector3(1.1f, 1.1f, 1.1f);
        isExpanding = true;
        isRecording = false;
        thenDescription.text = "Play <color=red>[(empty)] (grab microphone and press and hold \"A\" to record)</color>";
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
                soundRecorder = GetComponent<AudioSource>();
                soundRecorder.clip = Microphone.Start("", false, 60, 16000);
                thenDescription.text = "Play <color=red>[recording in process...] (release \"A\" to end recording)</color>";
                isRecording = true;
            }
            if (aGetUp || Input.GetKeyUp(KeyCode.A))
            //if (aGetUp) // save recording
            {
                Microphone.End("");
                thenDescription.text = "Play <color=red>[recorded] (recorded sound can be checked by pressing \"X\")</color>";
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
                progress = progress + speed * Time.deltaTime;
            else
                progress = progress - speed * Time.deltaTime;
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