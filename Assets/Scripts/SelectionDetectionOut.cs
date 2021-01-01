using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionDetectionOut : MonoBehaviour
{
    public GameObject[] outObjectsArr;
	public static GameObject outSelectedObject;
	public static bool outSelected;

	public float clipSpeed = 1.0f; // speed to clip to selection-box
	private Vector3 targetPos;

    void Start() {
        outSelected = false;
        targetPos = transform.position;
    }

    void Update() {
        if (outSelected) {
        	bool isGrabbed = outSelectedObject.transform.GetComponent<OVRGrabbable>().isGrabbed;
        	if (!isGrabbed) {
        		float step = clipSpeed * Time.deltaTime;
		        Vector3 currentPos = outSelectedObject.transform.position;
		        Vector3 newPos = Vector3.MoveTowards(currentPos, targetPos, step);
		        outSelectedObject.transform.position = newPos;
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
    		if (!outSelected) {
	    		outSelectedObject = other.gameObject;
	    		outSelected = true;
	    		IncreaseDrag(outSelectedObject);
	    	} else { // replace card choice
	    		DecreaseDrag(outSelectedObject);
    			outSelectedObject = other.gameObject;
    			IncreaseDrag(outSelectedObject);
	    	}
    	}
    }

    void OnTriggerExit(Collider other) {
    	if (other.gameObject == outSelectedObject) {
    		DecreaseDrag(outSelectedObject);
    		outSelectedObject = null;
    		outSelected = false;
    	}
    }

}
