using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CardSelectionSceneCore : MonoBehaviour
{
    public GameObject OVRCamera;
    public AudioSource introVoice;
	public AudioSource diveVoice;
    private bool playedDiveVoice;
    // default value of type:bool is 'false'

    // for storing type:GameObject
    // c.f. CardSelectionMediator.selectionDict stores type:string
    public static Dictionary<string, GameObject> selectionDict
        = new Dictionary<string, GameObject>() {
        {"environment", null},
        {"input", null},
        {"output", null}
    };

    void Start()
    {
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
        if (diveVoiceFinished) {
            MoveToNextScene();
        }

    }

    private void MoveToNextScene()
    {
        // printf for development use
        Debug.Log("Finished CardSelectionScene.\nMoving to PrototypingScene.");
        OVRCamera.GetComponent<OVRScreenFade>().FadeOut();
        SceneManager.LoadScene(2);
    }

    private void RecordSelectedCardsAsString()
    {
        CardSelectionMediator.selectionDict["environment"]
            = selectionDict["environment"].name;
        CardSelectionMediator.selectionDict["input"]
            = selectionDict["input"].name;
        CardSelectionMediator.selectionDict["output"]
            = selectionDict["output"].name;
        // printf for development use
        Debug.Log("[env, in, out] = ["
                  + CardSelectionMediator.selectionDict["environment"] + ", "
                  + CardSelectionMediator.selectionDict["input"] + ", "
                  + CardSelectionMediator.selectionDict["output"] + "]" );
    }

}
