using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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


public class PrototypingSceneCore : MonoBehaviour
{
    [SerializeField] private GameObject menuCanvas = null;
    [SerializeField] private Button backToSceneButton = null;
    [SerializeField] private Button closeMenuButton = null;
    private bool menuIsOpened = false;

    [SerializeField] private Button confirmationBtn = null;
    // TODO enable "click" only when the parameters for input/output were adjusted

    [SerializeField] private GameObject envObjTrashBin = null;
    [SerializeField] private GameObject envObjTree = null;
    [SerializeField] private GameObject envObjStreetLight = null;
    [SerializeField] private GameObject envObjStreetSign = null;
    [SerializeField] private GameObject envObjBridge = null;

    [SerializeField] private GameObject inPropsButton = null;
    [SerializeField] private GameObject inPropsSound = null;
    [SerializeField] private GameObject inPropsFire = null;
    [SerializeField] private GameObject inPropsSpeed = null;
    [SerializeField] private GameObject inPropsWeather = null;

    [SerializeField] private GameObject outPropsLightUp = null;
    [SerializeField] private GameObject outPropsMakeSound = null;
    [SerializeField] private GameObject outPropsVibrate = null;
    [SerializeField] private GameObject outPropsMove = null;
    [SerializeField] private GameObject outPropsSend = null;

    [SerializeField] private GameObject inputSelectionText = null;
    [SerializeField] private GameObject inputConditionBox = null;
    [SerializeField] private GameObject outputSelectionText = null;
    [SerializeField] private GameObject outputBehaviourBox = null;

    private GameObject environmentObject;
    private GameObject inputProps;
    private GameObject outputProps;
    private List<InputCard> inputInstances = new List<InputCard>();
    private List<OutputCard> outputInstances = new List<OutputCard>();
    private int instIdx = 0;
    // using list to make it scalable to handling multiple instances


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


    private void Start()
    {
        DevelopmentPurposeAssign(); // make sure to delete after development

        DeactivateAllCardRepresentations();

        environmentObject = GetEnvObjByName(CardSelectionMediator.selectionDict["environment"]);

        inputProps = GetInPropsByName(CardSelectionMediator.selectionDict["input"]);
        inputInstances.Add(GetInputInstanceByName(CardSelectionMediator.selectionDict["input"]));
        inputInstances[instIdx].CardSetup(ref environmentObject, ref inputSelectionText, inputConditionBox, inputProps);

        outputProps = GetOutPropsByName(CardSelectionMediator.selectionDict["output"]);
        outputInstances.Add(GetOutputInstanceByName(CardSelectionMediator.selectionDict["output"]));
        outputInstances[instIdx].CardSetup(ref environmentObject, ref outputSelectionText, outputBehaviourBox, outputProps);

        // UI button settings
        confirmationBtn.onClick.AddListener(ConfirmSmartObject);
        backToSceneButton.onClick.AddListener(LoadCardSelectionScene);
        closeMenuButton.onClick.AddListener(CloseMenu);
    }


    private void Update()
    {
        for (int i = 0; i <= instIdx; i++) {
            inputInstances[i].UpdateInputCondition();
            outputInstances[i].UpdateOutputBehaviour();
            if (inputInstances[i].inputCondition) {
                outputInstances[i].OutputBehaviour();
            }
        }

        if (OVRInput.GetDown(OVRInput.RawButton.Start)) {
            if (!menuIsOpened) OpenMenu();
            else CloseMenu();
        }
    }

    private void OpenMenu()
    {
        menuIsOpened = true;
        menuCanvas.SetActive(true);
    }
    private void CloseMenu()
    {
        menuIsOpened = false;
        menuCanvas.SetActive(false);
    }
    private void LoadCardSelectionScene()
    {
        SceneManager.LoadScene(1);
    }


    private void ConfirmSmartObject()
    {
        for (int i = 0; i <= instIdx; i++)
        {
            inputInstances[i].ConfirmInputCondition();
            outputInstances[i].ConfirmOutputBehaviour();
        }
    }


    private GameObject GetEnvObjByName(string cardName)
    {
        switch (cardName)
        {
            case "TrashBin": return envObjTrashBin;
            case "Tree": return envObjTree;
            case "StreetLight": return envObjStreetLight;
            case "StreetSign": return envObjStreetSign;
            case "Bridge": return envObjBridge;
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
            case "LightUp": return new LightUpCard();
            case "MakeSound": return new MakeSoundCard();
            case "Vibrate": return new VibrateCard();
            case "Move": return new MoveCard();
            case "Send": return new SendCard();
            default: return null;
        }
    }

    // for making sure that all card representations are initially deactivated
    private void DeactivateAllCardRepresentations()
    {
        GameObject[] cardRepresentations;
        cardRepresentations = GameObject.FindGameObjectsWithTag("DeactivateOnLoad");
        foreach (GameObject prop in cardRepresentations)
        {
            prop.SetActive(false);
        }
    }

}


/*
 * manipulation of list if necessary when creating multiple instances
 * private void AddInstanceToList(){}
 * private void RemoveInstanceFromList(){}
 * 
 */