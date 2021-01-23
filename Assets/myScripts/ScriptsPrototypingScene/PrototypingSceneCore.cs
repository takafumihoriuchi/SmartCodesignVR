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

/* Prerequisites of Class:ProtoyptingSceneCore (in editor):
- Name all parent object of each object group to match values of selectionDict.
- Set tag of all parent object of each object group to "CardRepresentation". */
public class PrototypingSceneCore : MonoBehaviour
{
    private GameObject[] cardRepresentationArr;
    private GameObject selectedEnvironmentObjGroup;
    private GameObject selectedInputObjGroup;
    private GameObject selectedOutputObjGroup;

    private List<Card> inputInstances = new List<Card>();
    private List<Card> outputInstances = new List<Card>();

    void Start()
    {
        DevelopmentPurposeAssign(); // make sure to delete after development!!

        // TODO: are these needed? can they be instantialted from instances?
        // cardRepresentationArr = GameObject.FindGameObjectsWithTag("CardRepresentation");
        // DeactivateAll();
        // ExtractSelected();
        // ActivateSelected();

        AddInstanceToList(ref inputInstances,
            GetInstanceByName(CardSelectionMediator.selectionDict["input"]));
        AddInstanceToList(ref outputInstances,
            GetInstanceByName(CardSelectionMediator.selectionDict["output"]));
    }

    void Update()
    {
        // ここで if (inputSelected.inputCondition) outputSelected.outputBehavior();
        // トリガーなどでできるのか？その方が効率が良い？
    }

    private void AddInstanceToList(ref List<Card> cardList, Card cardInstance)
    {
        cardList.Add(cardInstance);
        // TODO: explicitly call function in Card.cs to initialize
        // e.g. draw the panel, etc. (Constructor is not implicitly called here)
    }

    private void RemoveInstanceFromList(ref List<Card> cardList, Card cardInstance)
    {
        cardList.Remove(cardInstance);
        // TODO: explicitly call function in Card.cs to clean-up components
        // e.g. remove edit panel (Destructor is not implicitly called here)
    }

    private Card GetInstanceByName(string cardName)
    {
        switch (cardName)
        {
            // input cards
            case "Button"   : return new ButtonCard();
            case "Sound"    : return new SoundCard();
            case "Fire"     : return new FireCard();
            case "Speed"    : return new SpeedCard();
            case "Weather"  : return new WeatherCard();
            // output cards
            case "LightUp"  : return new LightUpCard();
            case "MakeSound": return new MakeSoundCard();
            case "Vibrate"  : return new VibrateCard();
            case "Move"     : return new MoveCard();
            case "Send"     : return new SendCard();
            default         : return null;
            // Constructors are called
        }
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
