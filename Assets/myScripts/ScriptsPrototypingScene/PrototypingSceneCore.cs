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
    // Environment Objects

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


    // IO Canvas Fileds

    [SerializeField] private GameObject inputCardNameField = null;
    [SerializeField] private GameObject outputCardNameField = null;
    [SerializeField] private GameObject inputDescriptionField = null;
    [SerializeField] private GameObject outputDescriptionField = null;

    [SerializeField] private GameObject inputStatementFieldGroup = null; // type:Button
    [SerializeField] private GameObject outputStatementFieldGroup = null; // type:Button

    [SerializeField] private Button addInstanceButton = null;
    [SerializeField] private Button removeInstanceButton = null;


    // Confirmation Canvas Fields

    // TODO enable "click" only when the parameters for input/output were adjusted
    // also serialize "back to edit" button, "text-field"
    [SerializeField] private Button confirmationBtn = null;
    bool confirmed = false; // 必要?? => 全てのインスタンスがIsConfirmed==trueだ、ということをまとめて保持するなら意義がある

    [SerializeField] private GameObject menuCanvas = null;
    [SerializeField] private Button backToSceneButton = null;
    [SerializeField] private Button closeMenuButton = null;
    private bool menuIsOpened = false;

    private GameObject environmentObject;
    private GameObject inputProps;
    private GameObject outputProps;
    private List<InputCard> inputInstanceList = new List<InputCard>();
    private List<OutputCard> outputInstanceList = new List<OutputCard>();
    private int instanceIdx = 0; // 
    // todo instanceIdx は必要なのか


    // for developmental use only
    private void DevelopmentPurposeAssign()
    {
        CardSelectionMediator.selectionDict["environment"] = "TrashBin";
        CardSelectionMediator.selectionDict["input"] = "Fire";
        CardSelectionMediator.selectionDict["output"] = "LightUp";

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
        inputInstanceList.Add(GetInputInstanceByName(CardSelectionMediator.selectionDict["input"]));
        inputInstanceList[instanceIdx].CardDescriptionSetup(
            ref inputCardNameField, ref inputDescriptionField);
        inputInstanceList[instanceIdx].CardStatementSetup(
            ref environmentObject, ref inputProps, inputStatementFieldGroup, GetAvailableMinimumInstanceID());
        // todo "inputStatementFieldGroup"は値渡しになっている？参照になっているような気がしている

        outputProps = GetOutPropsByName(CardSelectionMediator.selectionDict["output"]);
        outputInstanceList.Add(GetOutputInstanceByName(CardSelectionMediator.selectionDict["output"]));
        outputInstanceList[instanceIdx].CardDescriptionSetup(
            ref outputCardNameField, ref outputDescriptionField);
        outputInstanceList[instanceIdx].CardStatementSetup(
            ref environmentObject, ref outputProps, outputStatementFieldGroup, GetAvailableMinimumInstanceID());

        // UI button settings

        addInstanceButton.onClick.AddListener(AddInstanceToList);
        removeInstanceButton.onClick.AddListener(RemoveInstanceFromList);
        addInstanceButton.interactable = true;
        removeInstanceButton.interactable = false;

        confirmationBtn.onClick.AddListener(ConfirmSmartObject);
        confirmationBtn.interactable = false; // set to true when every-instances.canBeConfirmed is true

        backToSceneButton.onClick.AddListener(LoadCardSelectionScene);
        closeMenuButton.onClick.AddListener(CloseMenu);

    }

    // isFocused; 生まれた時には注目されている。生まれたてのものに注目が集まる。他は関心が薄れる。
    private void AddInstanceToList()
    {
        // AddInstance (compare with inputInstances[0].maxInstanceNum if allowed to generate new)


        if ()
    }
    private void RemoveInstanceFromList()
    {

    }
    // isFocused；末尾にあるものに注目するようにする。


    private void Update()
    {
        for (int i = 0; i < inputInstanceList.Count; i++) {
            inputInstanceList[i].UpdateInputCondition();
            outputInstanceList[i].UpdateOutputBehaviour();
        }

        if (confirmed) {
            for (int i = 0; i < inputInstanceList.Count; i++) {
                if (inputInstanceList[i].inputCondition) {
                    outputInstanceList[i].OutputBehaviour();
                } else {
                    outputInstanceList[i].OutputBehaviourNegative();
                }
            }
        }

        if (OVRInput.GetDown(OVRInput.RawButton.Start)) {
            if (!menuIsOpened) OpenMenu();
            else CloseMenu();
        }
    }


    private void ConfirmSmartObject()
    {
        for (int i = 0; i < inputInstanceList.Count; i++)
        {
            if (!(inputInstanceList[i].CanBeConfirmed
                && outputInstanceList[i].CanBeConfirmed)) return;
        }
        // all instances must be confirmable to confirm the Smart Object
        confirmed = true;
        for (int i = 0; i < inputInstanceList.Count; i++)
        {
            inputInstanceList[i].ConfirmInputCondition();
            outputInstanceList[i].ConfirmOutputBehaviour();
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

    private int GetAvailableMinimumInstanceID()
    {
        int idCandidate = 0;
        for (int i = 0; i < inputInstanceList.Count; i++)
        {
            if (inputInstanceList[i].InstanceID == -1) continue;
            if (idCandidate == inputInstanceList[i].InstanceID) idCandidate++;
        }
        return idCandidate;
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
        SceneManager.LoadScene(1); // CardSelectionScene
    }

}