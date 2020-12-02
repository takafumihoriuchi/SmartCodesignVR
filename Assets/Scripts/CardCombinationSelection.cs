using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// needs code refactoring to make it scalable
public class CardCombinationSelection : MonoBehaviour
{
	public Text textObject;
	private bool select_trashcan;
    private bool select_motion;
    private bool select_makesound;

    void Start() {}
	void Update() {}

    public void OnClickMake() {
    	if (select_trashcan && select_motion && select_makesound) {
    		
    	}
    }

    private void CheckCombination() {
    	if (select_trashcan && select_motion && select_makesound) {
    		textObject.text = "A trash can that alerts when someone litters.\n(Mission: respect the environment)";
    	}
    }

    // environment
    public void OnClickTrashcan() {
    	select_trashcan = !select_trashcan;
    	CheckCombination();
    }

    // input
    public void OnClickMotion() {
    	select_motion = !select_motion;
    	CheckCombination();
    }

    // output
    public void OnClickMakesound() {
    	select_makesound = !select_makesound;
    	CheckCombination();
    }

}
