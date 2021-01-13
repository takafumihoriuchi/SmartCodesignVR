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
        if (CardCombinationManagement.selectionDict["input"] != null) {
        	bool isGrabbed = CardCombinationManagement.selectionDict["input"].transform.GetComponent<OVRGrabbable>().isGrabbed;
        	if (!isGrabbed) {
        		float step = clipSpeed * Time.deltaTime;
		        Vector3 currentPos = CardCombinationManagement.selectionDict["input"].transform.position;
		        Vector3 newPos = Vector3.MoveTowards(currentPos, targetPos, step);
		        CardCombinationManagement.selectionDict["input"].transform.position = newPos;
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
    		if (CardCombinationManagement.selectionDict["input"] == null) {
	    		CardCombinationManagement.selectionDict["input"] = other.gameObject;
	    		IncreaseDrag(CardCombinationManagement.selectionDict["input"]);
	    	} else { // replace card choice
	    		DecreaseDrag(CardCombinationManagement.selectionDict["input"]);
    			CardCombinationManagement.selectionDict["input"] = other.gameObject;
    			IncreaseDrag(CardCombinationManagement.selectionDict["input"]);
	    	}
    	}
    }

    void OnTriggerExit(Collider other) {
    	if (other.gameObject == CardCombinationManagement.selectionDict["input"]) {
    		DecreaseDrag(CardCombinationManagement.selectionDict["input"]);
    		CardCombinationManagement.selectionDict["input"] = null;
    	}
    }

}