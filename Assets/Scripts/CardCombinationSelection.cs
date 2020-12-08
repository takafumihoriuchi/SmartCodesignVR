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

    private GameObject[] SmartObjectsTrashcanRelated;

    void Start() {
		SmartObjectsTrashcanRelated = GameObject.FindGameObjectsWithTag("SmartObjectTrashcan");
		foreach (GameObject SmartObject in SmartObjectsTrashcanRelated) {
			SmartObject.SetActive(false);
		}
    }

	void Update() {}

    public void OnClickMake() {
    	if (select_trashcan && select_motion && select_makesound) {
    		foreach (GameObject SmartObject in SmartObjectsTrashcanRelated) {
				SmartObject.SetActive(true);
			}
    	}
    }

    private void CheckCombination() {
    	if (select_trashcan && select_motion && select_makesound) {
    		textObject.text = "A trash-can encourages people to try again when someone misses their throw. (Mission: playful / respect)";
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
