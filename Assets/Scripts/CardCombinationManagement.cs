using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCombinationManagement : MonoBehaviour
{
    void Start() {

    }

    void Update() {
        if (SelectionDetectionEnv.envSelected && SelectionDetectionIn.inSelected && SelectionDetectionOut.outSelected) {
        	Debug.Log("three cards have been selected; [env, in, out] = [" + SelectionDetectionEnv.envSelectedObject.name + ", " + SelectionDetectionIn.inSelectedObject.name + ", " + SelectionDetectionOut.outSelectedObject.name + "]");
        }
    }

}
