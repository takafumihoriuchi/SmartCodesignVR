using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypingSceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    	Debug.Log("in PrototypingScene, Start()");
        Debug.Log("[env, in, out] = [" + CardSelectionTracker.selectionDict["environment"] + ", " + CardSelectionTracker.selectionDict["input"] + ", " + CardSelectionTracker.selectionDict["output"] + "]");
    }

    // Update is called once per frame
    void Update()
    {
    	Debug.Log("in PrototypingScene, Update()");
        Debug.Log("[env, in, out] = [" + CardSelectionTracker.selectionDict["environment"] + ", " + CardSelectionTracker.selectionDict["input"] + ", " + CardSelectionTracker.selectionDict["output"] + "]");
    }
}
