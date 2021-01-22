using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelectionMediator
{
    // parameters are set in CardSelectionScene
    public static Dictionary<string, string> selectionDict
        = new Dictionary<string, string>() {
        {"environment", null},
        {"input", null},
        {"output", null}
    };
}

/*
 Prerequisites of Class:ProtoyptingSceneCore (in editor):
- Name all parent object of each object group to match values of selectionDict.
- Set tag of all parent object of each object group to "CardRepresentation".
 */

public class PrototypingSceneCore : MonoBehaviour
{
    private GameObject[] cardRepresentationArr;
    private GameObject selectedEnvironmentObjGroup;
    private GameObject selectedInputObjGroup;
    private GameObject selectedOutputObjGroup;

    void Start()
    {
        DevelopmentPurposeAssign();
        // todo delete the line above after development
        cardRepresentationArr
            = GameObject.FindGameObjectsWithTag("CardRepresentation");
        DeactivateAll();
        ExtractSelected();
        ActivateSelected();

        // var inputSelected = new xxx (make instance??)
        // var outputSelected = new yyy
        
    }


    void Update()
    {
        // ここで if (inputSelected.inputCondition) outputSelected.outputBehavior();
        // トリガーなどでできるのか？その方が効率が良い？
    }

    private void ExtractSelected()
    {
        selectedEnvironmentObjGroup = LocateFromDict("environment");
        selectedInputObjGroup = LocateFromDict("input");
        selectedOutputObjGroup = LocateFromDict("output");
    }

    private void ActivateSelected()
    {
        selectedEnvironmentObjGroup.SetActive(true);
        selectedInputObjGroup.SetActive(true);
        selectedOutputObjGroup.SetActive(true);
    }

    private void DeactivateAll()
    {
        foreach (GameObject cardGroupObj in cardRepresentationArr)
        {
            cardGroupObj.SetActive(false);
        }
    }

    private GameObject LocateFromDict(string category)
    {
        foreach (GameObject cardGroupObj in cardRepresentationArr)
        {
            if (cardGroupObj.name
                == CardSelectionMediator.selectionDict[category])
            {
                return cardGroupObj;
            }
        }
        Debug.Log("Failed to extract object of category: " + category);
        return null;
    }

    // for developmental use only
    private void DevelopmentPurposeAssign()
    {
        CardSelectionMediator.selectionDict["environment"] = "TrashBin";
        CardSelectionMediator.selectionDict["input"] = "Fire";
        CardSelectionMediator.selectionDict["output"] = "MakeSound";
        Debug.Log("[env, in, out] = ["
            + CardSelectionMediator.selectionDict["environment"] + ", "
            + CardSelectionMediator.selectionDict["input"] + ", "
            + CardSelectionMediator.selectionDict["output"] + "]");
    }

}
