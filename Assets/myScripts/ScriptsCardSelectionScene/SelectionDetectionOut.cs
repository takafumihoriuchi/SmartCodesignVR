using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionDetectionOut : MonoBehaviour
{
    public GameObject[] outObjectsArr;

	public float clipSpeed = 1.0f; // speed to clip to selection-box
	private Vector3 targetPos;

    void Start() {
        targetPos = transform.position;
    }

    void Update() {
        if (CardCombinationManagement.selectionDict["output"] != null) {
        	bool isGrabbed = CardCombinationManagement.selectionDict["output"].transform.GetComponent<OVRGrabbable>().isGrabbed;
        	if (!isGrabbed) {
        		float step = clipSpeed * Time.deltaTime;
		        Vector3 currentPos = CardCombinationManagement.selectionDict["output"].transform.position;
		        Vector3 newPos = Vector3.MoveTowards(currentPos, targetPos, step);
		        CardCombinationManagement.selectionDict["output"].transform.position = newPos;
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
    	if (IsElementOf(other.gameObject, outObjectsArr)) {
    		if (CardCombinationManagement.selectionDict["output"] == null) {
	    		CardCombinationManagement.selectionDict["output"] = other.gameObject;
	    		IncreaseDrag(CardCombinationManagement.selectionDict["output"]);
	    	} else { // replace card choice
	    		DecreaseDrag(CardCombinationManagement.selectionDict["output"]);
    			CardCombinationManagement.selectionDict["output"] = other.gameObject;
    			IncreaseDrag(CardCombinationManagement.selectionDict["output"]);
	    	}
    	}
    }

    void OnTriggerExit(Collider other) {
    	if (other.gameObject == CardCombinationManagement.selectionDict["output"]) {
    		DecreaseDrag(CardCombinationManagement.selectionDict["output"]);
    		CardCombinationManagement.selectionDict["output"] = null;
    	}
    }

}
