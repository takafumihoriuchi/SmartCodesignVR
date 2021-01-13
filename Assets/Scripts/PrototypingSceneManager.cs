using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypingSceneManager : MonoBehaviour
{
	// TODO: better way to implement this?
	public GameObject EnvGroupTrashBin;
	public GameObject EnvGroupTree;
	public GameObject EnvGroupStreetLight;
	public GameObject EnvGroupStreetSign;
	public GameObject EnvGroupBridge;
	public GameObject InGroupButton;
	public GameObject InGroupSound;
	public GameObject InGroupFire;
	public GameObject InGroupSpeed;
	public GameObject InGroupWeather;
	public GameObject OutGroupLightUp;
	public GameObject OutGroupMakeSound;
	public GameObject OutGroupVibrate;
	public GameObject OutGroupMove;
	public GameObject OutGroupSend;

    void Start()
    {
    	DisableAll();

    	// for development use only:
    	CardSelectionTracker.selectionDict["environment"] = "TrashBin";
    	CardSelectionTracker.selectionDict["input"] = "Button";
    	CardSelectionTracker.selectionDict["output"] = "MakeSound";
        Debug.Log("[env, in, out] = [" 
        	+ CardSelectionTracker.selectionDict["environment"] + ", "
        	+ CardSelectionTracker.selectionDict["input"] + ", "
        	+ CardSelectionTracker.selectionDict["output"] + "]");
        
        ActivateSelected();
    }


    void Update()
    {

    }





    // TODO: improve the way of implementation
    void ActivateSelected() {
    	switch (CardSelectionTracker.selectionDict["environment"]) {
        	case "TrashBin":
        		EnvGroupTrashBin.SetActive(true);
        		// call subroutines or functions depending on the value of the selection
        		break;
        	case "Tree":
        		EnvGroupTree.SetActive(true);
        		break;
        	case "StreetLight":
        		EnvGroupStreetLight.SetActive(true);
        		break;
        	case "StreetSign":
        		EnvGroupStreetSign.SetActive(true);
        		break;
        	case "Bridge":
        		EnvGroupBridge.SetActive(true);
        		break;
        	default:
        		Debug.Log("Environment card: invalid selection");
        		break;
        }
        switch (CardSelectionTracker.selectionDict["input"]) {
        	case "Button":
        		EnvGroupButton.SetActive(true);
        		break;
        	case "Sound":
        		EnvGroupSound.SetActive(true);
        		break;
        	case "Fire":
        		EnvGroupFire.SetActive(true);
        		break;
        	case "Speed":
        		EnvGroupSpeed.SetActive(true);
        		break;
        	case "Weather":
        		EnvGroupWeather.SetActive(true);
        		break;
        	default:
        		Debug.Log("Input card: invalid selection");
        		break;
        }
        switch (CardSelectionTracker.selectionDict["input"]) {
        	case "LightUp":
        		EnvGroupLightUp.SetActive(true);
        		break;
        	case "MakeSound":
        		EnvGroupMakeSound.SetActive(true);
        		break;
        	case "Vibrate":
        		EnvGroupVibrate.SetActive(true);
        		break;
        	case "Move":
        		EnvGroupMove.SetActive(true);
        		break;
        	case "Send":
        		EnvGroupSend.SetActive(true);
        		break;
        	default:
        		Debug.Log("Output card: invalid selection");
        		break;
        }
    }


	// TODO: improve the way of implementation
    void DisableAll() {
		EnvGroupTrashBin.SetActive(false);
		EnvGroupTree.SetActive(false);
		EnvGroupStreetLight.SetActive(false);
		EnvGroupStreetSign.SetActive(false);
		EnvGroupBridge.SetActive(false);
		InGroupButton.SetActive(false);
		InGroupSound.SetActive(false);
		InGroupFire.SetActive(false);
		InGroupSpeed.SetActive(false);
		InGroupWeather.SetActive(false);
		OutGroupLightUp.SetActive(false);
		OutGroupMakeSound.SetActive(false);
		OutGroupVibrate.SetActive(false);
		OutGroupMove.SetActive(false);
		OutGroupSend.SetActive(false);
    }


}
