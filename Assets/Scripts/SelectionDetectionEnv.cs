using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionDetectionEnv : MonoBehaviour
{
	public GameObject[] envObjectsArr;

	public float clipSpeed = 1.0f; // speed to clip to selection-box
	private Vector3 targetPos;

    void Start() {
        targetPos = transform.position;
    }

    void Update() {
        if (CardCombinationManagement.selectionDict["environment"] != null) {
        	bool isGrabbed = CardCombinationManagement.selectionDict["environment"].transform.GetComponent<OVRGrabbable>().isGrabbed;
        	if (!isGrabbed) {
        		float step = clipSpeed * Time.deltaTime;
		        Vector3 currentPos = CardCombinationManagement.selectionDict["environment"].transform.position;
		        Vector3 newPos = Vector3.MoveTowards(currentPos, targetPos, step);
		        CardCombinationManagement.selectionDict["environment"].transform.position = newPos;
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
    	if (IsElementOf(other.gameObject, envObjectsArr)) {
    		if (CardCombinationManagement.selectionDict["environment"] == null) {
	    		CardCombinationManagement.selectionDict["environment"] = other.gameObject;
	    		IncreaseDrag(CardCombinationManagement.selectionDict["environment"]);
	    	} else { // replace card choice
	    		DecreaseDrag(CardCombinationManagement.selectionDict["environment"]);
    			CardCombinationManagement.selectionDict["environment"] = other.gameObject;
    			IncreaseDrag(CardCombinationManagement.selectionDict["environment"]);
	    	}
    	}
    }

    void OnTriggerExit(Collider other) {
    	if (other.gameObject == CardCombinationManagement.selectionDict["environment"]) {
    		DecreaseDrag(CardCombinationManagement.selectionDict["environment"]);
    		CardCombinationManagement.selectionDict["environment"] = null;
    	}
    }

}

/*
- example usage of Distance():
if (Vector3.Distance(newPos, targetPos) < 0.001f) atBoxCenter = true;
- isKinematic = true is similart to disableing a rigidbody object
envSelectedObject.transform.GetComponent<Rigidbody>().isKinematic = true;
*/
