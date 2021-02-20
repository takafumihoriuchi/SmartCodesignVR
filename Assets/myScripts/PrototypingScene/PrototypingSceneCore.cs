using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
using System.Text.RegularExpressions;

public class PrototypingSceneCore : MonoBehaviour
{

    [SerializeField] private GameObject envObjTrashBin = null; // tag "DeactivateOnLoad"
    [SerializeField] private GameObject envObjTree = null; // tag "DeactivateOnLoad"
    [SerializeField] private GameObject envObjStreetLight = null; // tag "DeactivateOnLoad"
    [SerializeField] private GameObject envObjStreetSign = null; // tag "DeactivateOnLoad"
    [SerializeField] private GameObject envObjBridge = null; // tag "DeactivateOnLoad"

    [SerializeField] private GameObject inPropsButton = null; // tag "DeactivateOnLoad"
    [SerializeField] private GameObject inPropsSound = null; // tag "DeactivateOnLoad"
    [SerializeField] private GameObject inPropsFire = null; // tag "DeactivateOnLoad"
    [SerializeField] private GameObject inPropsSpeed = null; // tag "DeactivateOnLoad"
    [SerializeField] private GameObject inPropsWeather = null; // tag "DeactivateOnLoad"

    [SerializeField] private GameObject outPropsLightUp = null; // tag "DeactivateOnLoad"
    [SerializeField] private GameObject outPropsMakeSound = null; // tag "DeactivateOnLoad"
    [SerializeField] private GameObject outPropsVibrate = null; // tag "DeactivateOnLoad"
    [SerializeField] private GameObject outPropsMove = null; // tag "DeactivateOnLoad"
    [SerializeField] private GameObject outPropsSend = null; // tag "DeactivateOnLoad"

    [SerializeField] private TextMeshProUGUI inputCardNameField = null;
    [SerializeField] private TextMeshProUGUI inputDescriptionField = null;
    [SerializeField] private GameObject inputStatementFieldGroup = null; // tag "DeactivateOnLoad"

    [SerializeField] private TextMeshProUGUI outputCardNameField = null;
    [SerializeField] private TextMeshProUGUI outputDescriptionField = null;
    [SerializeField] private GameObject outputStatementFieldGroup = null; // tag "DeactivateOnLoad"

    [SerializeField] private Button addInstanceButton = null;
    [SerializeField] private Button removeInstanceButton = null;

    [SerializeField] private GameObject ioArrowObject = null; // tag "DeactivateOnLoad"

    [SerializeField] private TextMeshProUGUI confirmationMessageField = null;
    [SerializeField] private Button confirmationButton = null;
    [SerializeField] private Button backToEditButton = null; // tag "DeactivateOnLoad"
    [SerializeField] private Button finalizationButton = null; // tag "DeactivateOnLoad"
    readonly string beforeConfirmMessage = "Ready to test the Smart Object?";
    readonly string afterConfirmMessage = "Do you want to keep editing or finalize?";

    [SerializeField] private GameObject menuCanvas = null; // tag "DeactivateOnLoad"
    [SerializeField] private Button backToSceneButton = null;
    [SerializeField] private Button closeMenuButton = null;

    private GameObject environmentObject;
    private GameObject inputProps;
    private GameObject outputProps;
    private List<InputCard> inputInstanceList = new List<InputCard>();
    private List<OutputCard> outputInstanceList = new List<OutputCard>();
    private List<GameObject> ioArrowList = new List<GameObject>();
    //  IO-Instance list: mapping between index and vertical positioning is in descending order
    // Arrow list: mapping between index and vertical positioning is in ascending order
    bool isConfirmed = false; // to distinguish between before/after-confirmed
    private bool menuIsOpened = false;

    const float VSHAMT = 0.5f; // vertical shift amount
    readonly Color BEIGE = new Color(0.9803f, 0.9568f, 0.9019f, 1.0f); // statement-box focused
    readonly Color LIGHT_BEIGE = new Color(0.9803f, 0.9568f, 0.9019f, 0.6f); // statement-box unfocused
    readonly Color POP_BEIGE = new Color(1.0f, 0.9210f, 0.7405f, 1.0f); // statement-box in effect
    //readonly Color POP_BEIGE = new Color(1.0f, 0.7783f, 0.8330f, 1.0f); // statement-box in effect (pinkish)
    readonly Color WHITE = new Color(1.0f, 1.0f, 1.0f, 1.0f); // arrow focused
    readonly Color LIGHT_WHITE = new Color(1.0f, 1.0f, 1.0f, 0.4f); // arrow unfocused

    [SerializeField] private GameObject editorEventSystem = null; // for developmental use only
    [SerializeField] private GameObject HMDEventSystem = null; // for developmental use only

    private AudioSource[] clickSound;

    // for developmental use only
    private void DevelopmentPurposeSettings()
    {
        bool isQuest2 = OVRManager.systemHeadsetType == OVRManager.SystemHeadsetType.Oculus_Quest_2;
        Debug.Log("isQuest2: " + isQuest2);
        bool onHMD = isQuest2;

        // for enabling button-clicks (editor use) or laser-pointer (HMD use)
        HMDEventSystem.SetActive(onHMD);
        editorEventSystem.SetActive(!onHMD);
        GameObject[] objectsWithRaycaster
            = GameObject.FindGameObjectsWithTag("ObjectWithRaycaster");
        foreach (GameObject obj in objectsWithRaycaster)
        {
            obj.GetComponent<OVRRaycaster>().enabled = onHMD;
            obj.GetComponent<GraphicRaycaster>().enabled = !onHMD;
        }

        bool isPlayTest = SmartObject.cardSelectionDict["environment"] == null;
        if (isPlayTest)
        {
            // environment
            SmartObject.cardSelectionDict["environment"] = "TrashBin";
            //SmartObject.cardSelectionDict["environment"] = "Tree";
            //SmartObject.cardSelectionDict["environment"] = "StreetLight";
            //SmartObject.cardSelectionDict["environment"] = "StreetSign";
            //SmartObject.cardSelectionDict["environment"] = "Bridge";

            // input
            //SmartObject.cardSelectionDict["input"] = "Fire";
            SmartObject.cardSelectionDict["input"] = "Weather";

            // output
            //SmartObject.cardSelectionDict["output"] = "LightUp";
            SmartObject.cardSelectionDict["output"] = "MakeSound";
        }
    }


    // keep in mind DRY: Don't Repeat Yourself
    private void Start()
    {
        // to delete after development
        DevelopmentPurposeSettings();

        // instance preparation
        SetActiveStateByTag("DeactivateOnLoad", false);
        SetActiveStateByTag("ActivateOnLoad", true);
        environmentObject = GetEnvObjByName(SmartObject.cardSelectionDict["environment"]);
        inputProps = GetInPropsByName(SmartObject.cardSelectionDict["input"]);
        outputProps = GetOutPropsByName(SmartObject.cardSelectionDict["output"]);

        AddInstanceToList();

        // UI text settings
        inputInstanceList[0].CardDescriptionSetup(
            ref inputCardNameField, ref inputDescriptionField);
        outputInstanceList[0].CardDescriptionSetup(
            ref outputCardNameField, ref outputDescriptionField);
        confirmationMessageField.SetText(beforeConfirmMessage);
        clickSound = editorEventSystem.GetComponents<AudioSource>();
        // todo ボタンの効果音を鳴らす処理を整理された形で実装する。

        // button settings
        addInstanceButton.onClick.AddListener(AddInstanceToList);
        addInstanceButton.interactable = CheckInstanceListCapacity();
        removeInstanceButton.onClick.AddListener(RemoveInstanceFromList);
        removeInstanceButton.interactable = false;
        confirmationButton.onClick.AddListener(ConfirmSmartObject);
        confirmationButton.interactable = false;
        backToEditButton.onClick.AddListener(GoBackToEditMode);
        backToEditButton.interactable = false;
        finalizationButton.onClick.AddListener(FinalizeSmartObject);
        finalizationButton.interactable = false;
        backToSceneButton.onClick.AddListener(LoadCardSelectionScene);
        closeMenuButton.onClick.AddListener(CloseMenu);
    }


    private void Update()
    {

        if (!isConfirmed) // during prototyping
        {
            int focusedIdx = GetFocusedInstanceIndex();
            inputInstanceList[focusedIdx].BehaviourDuringPrototyping();
            outputInstanceList[focusedIdx].BehaviourDuringPrototyping();

            // Check if the selected keywords have no overlaps
            // todo ideally call from properties (set/get)
            if (!ConditionKeywordIsUnique(focusedIdx))
                inputInstanceList[focusedIdx].ConditionKeyword
                    = inputInstanceList[focusedIdx].ALREADY_EXISTS;

            // check if all instances has been set a value
            bool isConfirmable = CheckConfirmability();
            if (isConfirmable) confirmationButton.interactable = true;
            else confirmationButton.interactable = false;
        }
        else // during testing
        {
            for (int i = 0; i < inputInstanceList.Count; i++)
                inputInstanceList[i].UpdateInputCondition();
            // reactions when input conditions are triggered (fired for one frame)
            for (int i = 0; i < inputInstanceList.Count; i++)
                if (inputInstanceList[i].NegativeTriggerFlag)
                {
                    outputInstanceList[i].OutputBehaviourOnNegative();
                    SetIOStatementFieldColor(i, BEIGE);
                }
            // all positive triggers are called after all negative triggers
            for (int i = 0; i < inputInstanceList.Count; i++)
                if (inputInstanceList[i].PositiveTriggerFlag)
                {
                    outputInstanceList[i].OutputBehaviourOnPositive();
                    SetIOStatementFieldColor(i, POP_BEIGE);
                }
        }

        // Oculus Touch Controller events
        if (OVRInput.GetDown(OVRInput.RawButton.Start))
        {
            if (!menuIsOpened) OpenMenu();
            else CloseMenu();
        }
        
    }

    // addInstanceButton OnClick
    private void AddInstanceToList()
    {
        //if (inputInstanceList.Count > 0) clickSound[0].Play();
        // todo

        inputInstanceList.Add(GetInputInstanceByName(SmartObject.cardSelectionDict["input"]));
        outputInstanceList.Add(GetOutputInstanceByName(SmartObject.cardSelectionDict["output"]));
        ioArrowList.Add(CreateIOArrow());
        int idx = inputInstanceList.Count - 1;
        // get tail of updated list; call after adding new instance
        int minInstanceID = AvailableMinInstanceID();
        inputInstanceList[idx].CardStatementSetup(
            ref environmentObject, ref inputProps, ref inputStatementFieldGroup, minInstanceID);
        outputInstanceList[idx].CardStatementSetup(
            ref environmentObject, ref outputProps, ref outputStatementFieldGroup, minInstanceID);

        ShiftFocusToTargetInstances(idx);
        ShiftFocusToTargetIOArrow(idx);
        ShiftStatementFieldPositions();

        inputInstanceList[idx].StatementFieldGroup.
            GetComponent<Button>().onClick.AddListener(StatementFieldOnClick);
        outputInstanceList[idx].StatementFieldGroup.
            GetComponent<Button>().onClick.AddListener(StatementFieldOnClick);

        // check for allawability of adding and removing instances
        addInstanceButton.interactable = CheckInstanceListCapacity();
        removeInstanceButton.interactable = !(inputInstanceList.Count <= 1);
    }


    // removeInstanceButton OnClick
    // remove the focused IO-Instance
    private void RemoveInstanceFromList()
    {
        //clickSound[1].Play(); // todo エラー

        for (int i = 0; i < inputInstanceList.Count; i++)
        {
            if (inputInstanceList[i].IsFocused)
            {
                Destroy(inputInstanceList[i].StatementFieldGroup);
                Destroy(outputInstanceList[i].StatementFieldGroup);
                inputInstanceList.RemoveAt(i);
                outputInstanceList.RemoveAt(i);
                break;
            }
        }
        // move focus to the youngest instance
        GrantFocusToTargetInstances(inputInstanceList.Count - 1);
        ShiftStatementFieldPositions();
        ReduceIOArrow();
        ShiftFocusToTargetIOArrow(0, true);
        addInstanceButton.interactable = CheckInstanceListCapacity();
        removeInstanceButton.interactable = !(inputInstanceList.Count <= 1);
    }


    // Statement-Field-Box OnClick
    private void StatementFieldOnClick()
    {
        //clickSound[0].Play(); // todo
        // get "instance-index" from "statement-field-button-label"
        string statementIdText = EventSystem.current.currentSelectedGameObject.
            transform.Find("IndexText").gameObject.GetComponent<TMP_Text>().text;
        string extrudedIdText = Regex.Replace(statementIdText, @"[^0-9]", "");
        int extrudedId = int.Parse(extrudedIdText);
        int instanceID = extrudedId - 1;
        int instanceIdx = GetInstanceListIndexFromInstanceID(instanceID);
        ShiftFocusToTargetInstances(instanceIdx);
        ShiftFocusToTargetIOArrow(instanceIdx);
    }


    // Confirmation Button OnClick
    private void ConfirmSmartObject()
    {
        //clickSound[0].Play(); // todo

        isConfirmed = true;

        int instanceCount = inputInstanceList.Count;
        for (int i = 0; i < instanceCount; i++)
        {
            inputInstanceList[i].IsConfirmed = true;
            outputInstanceList[i].IsConfirmed = true;
        }

        confirmationButton.interactable = false;
        confirmationButton.gameObject.SetActive(false);
        backToEditButton.interactable = true;
        backToEditButton.gameObject.SetActive(true);
        finalizationButton.interactable = true;
        finalizationButton.gameObject.SetActive(true);
        confirmationMessageField.SetText(afterConfirmMessage);

        addInstanceButton.interactable = false;
        removeInstanceButton.interactable = false;

        ChangeStatementFieldInteractability(false);
        ChangeUnfocusedStatementFieldColors(BEIGE);
        ChangeUnfocusedArrowColors(WHITE);
    }


    // Finalization Button OnClick
    private void FinalizeSmartObject()
    {
        //clickSound[0].Play(); // todo
        // todo pack smart object information data and pass to next scene
        // => CardSelectionMediator class に新しいフィールドを用意する
        // Smart Object を記述するのに何が必要かを考える
        // （environment object, input-delegate, output-behaviour, props, ...）
        // delegateのdictionaryと、選択されているキーワードのリスト(<=新しく作成する)も渡す
        Debug.Log("Moving to InteractionScene");
        SceneManager.LoadScene(3); // InteractionScene
    }


    // Back-To-Edit Button OnClick
    private void GoBackToEditMode()
    {
        //clickSound[1].Play(); // todo

        isConfirmed = false;

        int instanceCount = inputInstanceList.Count;
        for (int i = 0; i < instanceCount; i++)
        {
            inputInstanceList[i].IsConfirmed = false;
            outputInstanceList[i].IsConfirmed = false;
        }

        confirmationButton.interactable = true;
        confirmationButton.gameObject.SetActive(true);
        backToEditButton.interactable = false;
        backToEditButton.gameObject.SetActive(false);
        finalizationButton.interactable = false;
        finalizationButton.gameObject.SetActive(false);
        confirmationMessageField.SetText(beforeConfirmMessage);

        addInstanceButton.interactable = CheckInstanceListCapacity();
        removeInstanceButton.interactable = !(inputInstanceList.Count <= 1);

        ChangeStatementFieldInteractability(true);
        ChangeUnfocusedStatementFieldColors(LIGHT_BEIGE);
        ChangeUnfocusedArrowColors(LIGHT_WHITE);
    }


    private int AvailableMinInstanceID()
    {
        int idCandidate = 0;
        for (int i = 0; i < inputInstanceList.Count; i++)
        {
            if (inputInstanceList[i].InstanceID == -1) continue; // skip itself (set to initial value)
            if (idCandidate == inputInstanceList[i].InstanceID) idCandidate++;
        }
        return idCandidate;
    }


    private bool CheckInstanceListCapacity()
    {
        return inputInstanceList.Count < inputInstanceList[0].MaxInstanceNum;
    }


    private bool ConditionKeywordIsUnique(int focusedIdx)
    {
        if (string.IsNullOrEmpty(inputInstanceList[focusedIdx].ConditionKeyword))
            return true;
        for (int i = 0; i < inputInstanceList.Count; i++)
        {
            if (i == focusedIdx) continue; // skip itself
            if (inputInstanceList[i].ConditionKeyword
                == inputInstanceList[focusedIdx].ConditionKeyword)
                return false;
        }
        return true;
    }


    private int GetFocusedInstanceIndex()
    {
        for (int i = 0; i < inputInstanceList.Count; i++)
        {
            if (inputInstanceList[i].IsFocused) return i;
        }
        return -1;
    }


    private void GrantFocusToTargetInstances(int targetIdx)
    {
        SetIOInstanceFocusState(targetIdx, true);
        SetIOStatementFieldColor(targetIdx, BEIGE);
    }


    private void ShiftFocusToTargetInstances(int targetIdx)
    {
        int instanceCount = inputInstanceList.Count;
        for (int i = 0; i < instanceCount; i++)
        {
            if (i == targetIdx)
            {
                SetIOInstanceFocusState(targetIdx, true);
                SetIOStatementFieldColor(targetIdx, BEIGE);
            }
            else
            {
                SetIOInstanceFocusState(i, false);
                SetIOStatementFieldColor(i, LIGHT_BEIGE);
            }
        }
    }


    private void SetIOInstanceFocusState(int idx, bool tf)
    {
        inputInstanceList[idx].IsFocused = tf;
        outputInstanceList[idx].IsFocused = tf;
    }


    private void ShiftStatementFieldPositions()
    {
        Vector3 inputBasePosition = inputStatementFieldGroup.transform.localPosition;
        Vector3 outputBasePosition = outputStatementFieldGroup.transform.localPosition;
        int instanceCount = inputInstanceList.Count;
        for (int i = 0; i < instanceCount; i++)
        {
            Vector3 verticalShiftAmount = new Vector3(0, VSHAMT * (instanceCount - 1 - i), 0);
            inputInstanceList[i].StatementFieldGroup.transform.localPosition
                = inputBasePosition + verticalShiftAmount;
            outputInstanceList[i].StatementFieldGroup.transform.localPosition
                = outputBasePosition + verticalShiftAmount;
        }
    }


    // returns position adjusted io-arrow gameObject
    private GameObject CreateIOArrow()
    {
        GameObject newArrow = Instantiate(ioArrowObject, ioArrowObject.transform.parent);
        newArrow.transform.localPosition += new Vector3(0, VSHAMT * (ioArrowList.Count), 0);
        newArrow.SetActive(true);
        return newArrow;
    }


    // remove the top-most arrow
    private void ReduceIOArrow()
    {
        int idx = ioArrowList.Count - 1;
        Destroy(ioArrowList[idx]);
        ioArrowList.RemoveAt(idx);
    }


    // parameter int:idx is passed assuming the use with IOInstance index order
    // set optional parameter to 'true' if calling with real index
    private void ShiftFocusToTargetIOArrow(int idx, bool realIndex=false)
    {
        if (!realIndex) idx = RevIdx(idx);
        for (int i = 0; i < ioArrowList.Count;  i++)
        {
            if (i == idx) ioArrowList[i].GetComponent<Image>().color = WHITE;
            else ioArrowList[i].GetComponent<Image>().color = LIGHT_WHITE;
        }
        
    }


    private int RevIdx(int idx)
    {
        return ioArrowList.Count - 1 - idx;
    }


    private bool CheckConfirmability()
    {
        bool bflag = true;
        int instanceCount = inputInstanceList.Count;
        for (int i = 0; i < instanceCount; i++)
        {
            bflag &= inputInstanceList[i].CanBeConfirmed;
            bflag &= outputInstanceList[i].CanBeConfirmed;
        }
        return bflag;
    }


    // returns index in Instance List 
    // returns -1 when not found in Instance List
    private int GetInstanceListIndexFromInstanceID(int id)
    {
        for (int idx = 0; idx < inputInstanceList.Count; idx++)
        {
            if (inputInstanceList[idx].InstanceID == id) return idx;
        }
        return -1;
    }


    private void ChangeStatementFieldInteractability(bool tf)
    {
        for (int i = 0; i < inputInstanceList.Count; i++)
        {
            inputInstanceList[i].StatementFieldGroup.
                GetComponent<Button>().interactable = tf;
            outputInstanceList[i].StatementFieldGroup.
                GetComponent<Button>().interactable = tf;
        }
    }


    private void ChangeUnfocusedStatementFieldColors(Color color)
    {
        for (int i = 0; i < inputInstanceList.Count; i++)
        {
            if (inputInstanceList[i].IsFocused) continue;
            else SetIOStatementFieldColor(i, color);
        }
    }


    private void SetIOStatementFieldColor(int idx, Color color)
    {
        inputInstanceList[idx].StatementFieldGroup.GetComponent<Image>().color = color;
        outputInstanceList[idx].StatementFieldGroup.GetComponent<Image>().color = color;
    }


    private void ChangeUnfocusedArrowColors(Color color)
    {
        for (int i = 0; i < ioArrowList.Count; i++)
        {
            if (inputInstanceList[RevIdx(i)].IsFocused) continue;
            else ioArrowList[i].GetComponent<Image>().color = color;
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


    // OnClick of a button in Menu Canvas
    private void LoadCardSelectionScene()
    {
        SceneManager.LoadScene(1); // CardSelectionScene
    }


    // safety measure for preventing human errors
    private void SetActiveStateByTag(string tag, bool state)
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in taggedObjects) obj.SetActive(state);
    }


}
