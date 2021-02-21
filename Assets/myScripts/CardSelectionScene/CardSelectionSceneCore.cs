using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CardSelectionSceneCore : MonoBehaviour
{
    // default value of type:bool is 'false'
    private bool playedDiveVoice;

    // initializing with 'null' to avoid compile warnings
    [SerializeField] private GameObject OVRCamera = null;
    [SerializeField] private AudioSource introVoice = null;
    [SerializeField] private AudioSource diveVoice = null;

    [SerializeField] private GameObject envBoxObj = null;
    [SerializeField] private GameObject inBoxObj = null;
    [SerializeField] private GameObject outBoxObj = null;

    [SerializeField] private GameObject[] envObjArr = null;
    [SerializeField] private GameObject[] inObjArr = null;
    [SerializeField] private GameObject[] outObjArr = null;

    CardSelectionDetector EnvSelectionDetector;
    CardSelectionDetector InSelectionDetector;
    CardSelectionDetector OutSelectionDetector;

    // dictionary for storing type:GameObject
    // c.f. CardSelectionMediator.selectionDict stores type:string
    public static Dictionary<string, GameObject> selectionDict
        = new Dictionary<string, GameObject>() {
        {"environment", null},
        {"input", null},
        {"output", null}
    };

    void Start()
    {
        EnvSelectionDetector
            = new CardSelectionDetector(envBoxObj, envObjArr, "environment");
        InSelectionDetector
            = new CardSelectionDetector(inBoxObj, inObjArr, "input");
        OutSelectionDetector
            = new CardSelectionDetector(outBoxObj, outObjArr, "output");

        introVoice.PlayDelayed(3.5f);
    }

    void Update()
    {
        bool selectionSet = selectionDict["environment"] !=null
                           && selectionDict["input"] !=null
                           && selectionDict["output"] !=null;
        bool diveVoiceReady = selectionSet && !diveVoice.isPlaying && !playedDiveVoice;
        bool selectionCancelled = diveVoice.isPlaying && !selectionSet;
        bool diveVoiceFinished = selectionSet && !diveVoice.isPlaying && playedDiveVoice;
        // giving priority to readable-code than small preformance improvements

        if (diveVoiceReady) {
            if (introVoice.isPlaying) introVoice.Stop();
            diveVoice.PlayDelayed(1);
            playedDiveVoice = true;
            RecordSelectedCardsAsString();
        }
        if (selectionCancelled) {
            diveVoice.Stop();
            playedDiveVoice = false;
        }
        if (diveVoiceFinished
            || (playedDiveVoice && OVRInput.GetDown(OVRInput.RawButton.A))) {
            MoveToNextScene();
        }

        EnvSelectionDetector.CenterGravityMotion();
        InSelectionDetector.CenterGravityMotion();
        OutSelectionDetector.CenterGravityMotion();
    }

    private void MoveToNextScene()
    {
        // printf for development use
        Debug.Log("Finished CardSelectionScene.\nMoving to PrototypingScene.");
        OVRCamera.GetComponent<OVRScreenFade>().FadeOut();
        SceneManager.LoadScene(2);
    }

    // for passing selection-infromation to next scene
    // todo in SmartObject class, making a dictionary <string, bool> is safer
    private void RecordSelectedCardsAsString()
    {
        SmartObject.cardSelectionDict["environment"]
            = selectionDict["environment"].name;
        SmartObject.cardSelectionDict["input"]
            = selectionDict["input"].name;
        SmartObject.cardSelectionDict["output"]
            = selectionDict["output"].name;
        // printf for development use
        Debug.Log("[env, in, out] = ["
                  + SmartObject.cardSelectionDict["environment"] + ", "
                  + SmartObject.cardSelectionDict["input"] + ", "
                  + SmartObject.cardSelectionDict["output"] + "]" );
    }

}