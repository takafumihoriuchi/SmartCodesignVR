using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

// Prerequisites:
// - make sure to deactivate all envrionment object;
//   set deactivated all input and output objects as well
public class PrototypingSceneCore : MonoBehaviour
{
    [SerializeField] public Button confirmationBtn;
    // TODO enable "click" only when the parameters for input/output were adjusted

    [SerializeField] public GameObject trashBinObject;
    [SerializeField] public GameObject treeObject;
    [SerializeField] public GameObject streetLightObject;
    [SerializeField] public GameObject streetSignObject;
    [SerializeField] public GameObject bridgeObject;
    // Attach "in-scene GameObject" instead of "prefabs"
    // reason 1: allows to edit properties in inspector
    // reason 2: environment objects will never be duplicated

    [HideInInspector] private GameObject environmentObject;
    private List<InputCard> inputInstances = new List<InputCard>();
    private List<OutputCard> outputInstances = new List<OutputCard>();
    private int inputIdx , outputIdx = 0;


    void Start()
    {
        DevelopmentPurposeAssign(); // make sure to delete after development

        environmentObject = GetEnvObjByName(CardSelectionMediator.selectionDict["environment"]);
        environmentObject.SetActive(true);

        inputInstances.Add(GetInputInstanceByName(CardSelectionMediator.selectionDict["input"]));
        inputIdx = 0; inputInstances[inputIdx].SetInputCondition(ref environmentObject);

        outputInstances.Add(GetOutputInstanceByName(CardSelectionMediator.selectionDict["output"]));
        outputIdx = 0; outputInstances[outputIdx].SetOutputBehaviour(); // この実装で良いのか要検討

        confirmationBtn.onClick.AddListener(ConfirmSmartObject);
    }


    void Update()
    {
        //この実装で良いのか要検討
        // こんな単純な話ではない。確定まだinputConditionがなんなのかが確定されていない。
        for (int i = 0; i < inputIdx; i++)
        {
            inputInstances[i].UpdateInputCondition();
            if (inputInstances[i].inputCondition)
            {
                outputInstances[i].UpdateOutputBehaviour();
                outputInstances[i].OutputBehaviour();
            }
        }

    }

    private void ConfirmSmartObject()
    {
        for (int i = 0; i < inputIdx; i++)
        {
            inputInstances[i].ConfirmInputCondition();
        }
        for (int i = 0; i < inputIdx; i++)
        {
            outputInstances[i].ConfirmOutputBehaviour();
        }
    }


    private GameObject GetEnvObjByName(string cardName)
    {
        switch (cardName)
        {
            case "TrashBin"   : return trashBinObject;
            case "Tree"       : return treeObject;
            case "StreetLight": return streetLightObject;
            case "StreetSign" : return streetSignObject;
            case "Bridge"     : return bridgeObject;
            default: return null;
        }
    }


    private InputCard GetInputInstanceByName(string cardName)
    {
        switch (cardName)
        {
            case "Button": return new ButtonCard();
            case "Sound": return new SoundCard();
            case "Fire": return new FireCard();
            case "Speed": return new SpeedCard();
            case "Weather": return new WeatherCard();
            default: return null;
        }
    }


    private OutputCard GetOutputInstanceByName(string cardName)
    {
        switch (cardName)
        {
            case "LightUp"  : return new LightUpCard();
            case "MakeSound": return new MakeSoundCard();
            case "Vibrate"  : return new VibrateCard();
            case "Move"     : return new MoveCard();
            case "Send"     : return new SendCard();
            default: return null;
        }
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




/*

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

// Prerequisites of Class:ProtoyptingSceneCore (in editor):
// - Name all parent object of each object group to match values of selectionDict.
// - Set tag of all parent object of each object group to "CardRepresentation".

// - make sure to deactivate all envrionment object

public class PrototypingSceneCore : MonoBehaviour
{
    // are these needed??
    //private GameObject[] cardRepresentationArr;
    //private GameObject selectedEnvironmentObjGroup;
    //private GameObject selectedInputObjGroup;
    //private GameObject selectedOutputObjGroup;

    [SerializeField] public GameObject trashBinObject;
    [SerializeField] public GameObject treeObject;
    [SerializeField] public GameObject streetLightObject;
    [SerializeField] public GameObject streetSignObject;
    [SerializeField] public GameObject bridgeObject;
    // Attach "in-scene GameObject" instead of "prefabs"
    // reason 1: allows to edit properties in inspector
    // reason 2: environment objects will never be duplicated

    [HideInInspector] public GameObject environmentObject;
    private List<Card> inputInstances = new List<Card>();
    private List<Card> outputInstances = new List<Card>();
    private int inputIdx = 0, outputIdx = 0;

    void Start()
    {
        DevelopmentPurposeAssign(); // make sure to delete after development!!

        // are these needed? can they be instantialted from instances?
        // cardRepresentationArr = GameObject.FindGameObjectsWithTag("CardRepresentation");
        // DeactivateAll();
        // ExtractSelected();
        // ActivateSelected();

        ActivateEnvObject(ref environmentObject,
            GetEnvObjByName(CardSelectionMediator.selectionDict["environment"]));
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

    private void ActivateEnvObject(ref GameObject envObj, GameObject obj)
    {
        envObj = obj;
        envObj.SetActive(true);
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

    private GameObject GetEnvObjByName(string cardName)
    {
        switch (cardName)
        {
            case "TrashBin"   : return trashBinObject;
            case "Tree"       : return treeObject;
            case "StreetLight": return streetLightObject;
            case "StreetSign" : return streetSignObject;
            case "Bridge"     : return bridgeObject;
            default: return null;
        }
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
            default: return null;
            // Constructors are called
        }
    }

    // are these needed??
    //private void ExtractSelected()
    //{
    //    selectedEnvironmentObjGroup = LocateFromDict("environment");
    //    selectedInputObjGroup = LocateFromDict("input");
    //    selectedOutputObjGroup = LocateFromDict("output");
    //}

    //private void ActivateSelected()
    //{
    //    selectedEnvironmentObjGroup.SetActive(true);
    //    selectedInputObjGroup.SetActive(true);
    //    selectedOutputObjGroup.SetActive(true);
    //}

    //private void DeactivateAll()
    //{
    //    foreach (GameObject cardGroupObj in cardRepresentationArr)
    //    {
    //        cardGroupObj.SetActive(false);
    //    }
    //}

    //private GameObject LocateFromDict(string category)
    //{
    //    foreach (GameObject cardGroupObj in cardRepresentationArr)
    //    {
    //        if (cardGroupObj.name
    //            == CardSelectionMediator.selectionDict[category])
    //        {
    //            return cardGroupObj;
    //        }
    //    }
    //    Debug.Log("Failed to extract object of category: " + category);
    //    return null;
    //}

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


2021.1.23.22.56

//ActivateEnvObject(ref environmentObject,
//    GetEnvObjByName(CardSelectionMediator.selectionDict["environment"]));

environmentObject = GetEnvObjByName(CardSelectionMediator.selectionDict["environment"]);
environmentObject.SetActive(true);

//AddInstanceToList(ref inputInstances,
//    GetInputInstanceByName(CardSelectionMediator.selectionDict["input"]));

inputInstances.Add(GetInputInstanceByName(CardSelectionMediator.selectionDict["input"]));

//AddInstanceToList(ref outputInstances,
//    GetOutputInstanceByName(CardSelectionMediator.selectionDict["output"]));

outputInstances.Add(GetOutputInstanceByName(CardSelectionMediator.selectionDict["output"]));

...

    //private void AddInstanceToList(ref List<Card> cardList, Card cardInstance)
    //{
    //    cardList.Add(cardInstance);
    //    // TODO: explicitly call function in Card.cs to initialize
    //    // e.g. draw the panel, etc. (Constructor is not implicitly called here)
    //}

    private void ActivateEnvObject(ref GameObject envObj, GameObject obj)
    {
        envObj = obj;
        envObj.SetActive(true);
    }

    private void RemoveInstanceFromList(ref List<Card> cardList, Card cardInstance)
    {
        cardList.Remove(cardInstance);
        // TODO: explicitly call function in Card.cs to clean-up components
        // e.g. remove edit panel (Destructor is not implicitly called here)
    }

 */