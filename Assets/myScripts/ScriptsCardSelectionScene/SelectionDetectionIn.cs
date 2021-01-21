using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionDetectionIn : MonoBehaviour
{
	public GameObject[] inObjectsArr;

	public float clipSpeed = 1.0f; // speed to clip to selection-box
	private Vector3 targetPos;

    void Start() {
        targetPos = transform.position;
    }

    void Update() {
        if (CardSelectionSceneCore.selectionDict["input"] != null) {
        	bool isGrabbed = CardSelectionSceneCore.selectionDict["input"].transform.GetComponent<OVRGrabbable>().isGrabbed;
        	if (!isGrabbed) {
        		float step = clipSpeed * Time.deltaTime;
		        Vector3 currentPos = CardSelectionSceneCore.selectionDict["input"].transform.position;
		        Vector3 newPos = Vector3.MoveTowards(currentPos, targetPos, step);
                CardSelectionSceneCore.selectionDict["input"].transform.position = newPos;
        	}
        }
    }

    bool IsElementOf(GameObject obj, GameObject[] arr) {
    	foreach (GameObject elm in arr)
    		if (elm == obj) return true;
    	return false;
    }

    void IncreaseDrag(GameObject obj) {
    	obj.transform.GetComponent<Rigidbody>().drag = 20;
	    obj.transform.GetComponent<Rigidbody>().angularDrag = 10.0F;
    }
    
    void DecreaseDrag(GameObject obj) {
    	obj.transform.GetComponent<Rigidbody>().drag = 1;
    	obj.transform.GetComponent<Rigidbody>().angularDrag = 0.5F;
    }

    void OnTriggerEnter(Collider other) {
    	if (IsElementOf(other.gameObject, inObjectsArr)) {
    		if (CardSelectionSceneCore.selectionDict["input"] == null) {
                CardSelectionSceneCore.selectionDict["input"] = other.gameObject;
	    		IncreaseDrag(CardSelectionSceneCore.selectionDict["input"]);
	    	} else { // replace card choice
	    		DecreaseDrag(CardSelectionSceneCore.selectionDict["input"]);
                CardSelectionSceneCore.selectionDict["input"] = other.gameObject;
    			IncreaseDrag(CardSelectionSceneCore.selectionDict["input"]);
	    	}
    	}
    }

    void OnTriggerExit(Collider other) {
    	if (other.gameObject == CardSelectionSceneCore.selectionDict["input"]) {
    		DecreaseDrag(CardSelectionSceneCore.selectionDict["input"]);
            CardSelectionSceneCore.selectionDict["input"] = null;
    	}
    }

}