﻿using System.Collections;
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
// - deactivate all envrionment object;
// - deavtivate the top-level gameobject of input/output props
public class PrototypingSceneCore : MonoBehaviour
{
    [SerializeField] private Button confirmationBtn;
    // TODO enable "click" only when the parameters for input/output were adjusted

    [SerializeField] private GameObject envObjTrashBin;
    [SerializeField] private GameObject envObjTree;
    [SerializeField] private GameObject envObjStreetLight;
    [SerializeField] private GameObject envObjStreetSign;
    [SerializeField] private GameObject envObjBridge;

    [SerializeField] private GameObject inPropsButton;
    [SerializeField] private GameObject inPropsSound;
    [SerializeField] private GameObject inPropsFire;
    [SerializeField] private GameObject inPropsSpeed;
    [SerializeField] private GameObject inPropsWeather;

    [SerializeField] private GameObject outPropsLightUp;
    [SerializeField] private GameObject outPropsMakeSound;
    [SerializeField] private GameObject outPropsVibrate;
    [SerializeField] private GameObject outPropsMove;
    [SerializeField] private GameObject outPropsSend;

    [SerializeField] private GameObject inputSelectionText;
    [SerializeField] private GameObject outputSelectionText;
    [SerializeField] private GameObject inputConditionBox;
    [SerializeField] private GameObject outputBehaviourBox;

    private GameObject environmentObject;
    private GameObject inputProps;
    private GameObject outputProps;
    private List<InputCard> inputInstances = new List<InputCard>();
    private List<OutputCard> outputInstances = new List<OutputCard>();
    private int instIdx = 0;


    void Start()
    {
        DevelopmentPurposeAssign(); // make sure to delete after development

        environmentObject = GetEnvObjByName(CardSelectionMediator.selectionDict["environment"]);

        inputProps = GetInPropsByName(CardSelectionMediator.selectionDict["input"]);
        inputInstances.Add(GetInputInstanceByName(CardSelectionMediator.selectionDict["input"]));
        inputInstances[instIdx].SetInputCondition(
            ref environmentObject, ref inputSelectionText, inputConditionBox, inputProps);

        outputProps = GetOutPropsByName(CardSelectionMediator.selectionDict["output"]);
        outputInstances.Add(GetOutputInstanceByName(CardSelectionMediator.selectionDict["output"]));
        outputInstances[instIdx].SetOutputBehaviour(
            ref environmentObject, ref outputSelectionText, outputBehaviourBox, outputProps);

        confirmationBtn.onClick.AddListener(ConfirmSmartObject);
    }


    void Update()
    {
        for (int i = 0; i < instIdx; i++) {
            inputInstances[i].UpdateInputCondition();
            outputInstances[i].UpdateOutputBehaviour();
            if (inputInstances[i].inputCondition)
                outputInstances[i].OutputBehaviour();
        }

    }

    private void ConfirmSmartObject()
    {
        foreach (InputCard inInst in inputInstances)
        {
            inInst.ConfirmInputCondition();
        }
        foreach (OutputCard outInst in outputInstances)
        {
            outInst.ConfirmOutputBehaviour();
        }
    }


    private GameObject GetEnvObjByName(string cardName)
    {
        switch (cardName)
        {
            case "TrashBin"   : return envObjTrashBin;
            case "Tree"       : return envObjTree;
            case "StreetLight": return envObjStreetLight;
            case "StreetSign" : return envObjStreetSign;
            case "Bridge"     : return envObjBridge;
            default: return null;
        }
    }

    private GameObject GetInPropsByName(string cardName)
    {
        switch (cardName)
        {
            case "Button": return inPropsButton;
            case "Sound": return inPropsSound;
            case "Fire": return inPropsFire;
            case "Speed": return inPropsSpeed;
            case "Weather": return inPropsWeather;
            default: return null;
        }
    }

    private GameObject GetOutPropsByName(string cardName)
    {
        switch (cardName)
        {
            case "LightUp": return outPropsLightUp;
            case "MakeSound": return outPropsMakeSound;
            case "Vibrate": return outPropsVibrate;
            case "Move": return outPropsMove;
            case "Send": return outPropsSend;
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