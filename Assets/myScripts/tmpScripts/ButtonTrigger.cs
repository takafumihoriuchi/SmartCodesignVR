using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{
	private bool pressInProgress;

	private void OnTriggerEnter(Collider col) {
		if ((col.tag == "ButtonActivator") && !pressInProgress) {
			pressInProgress = true;
		}
	}

	private void OnTriggerExit(Collider col) {
		if (col.tag == "ButtonActivator") {
			pressInProgress = false;
		}
	}

}
