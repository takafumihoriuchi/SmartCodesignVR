using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelectionTracker {

	// access this dictionary from PrototypingScene
	public static Dictionary<string, string> selectionDict = new Dictionary<string, string>() {
		{"environment", null},
    	{"input", null},
    	{"output", null}
	};

}
