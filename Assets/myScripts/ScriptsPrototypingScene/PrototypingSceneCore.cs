using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;


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
    readonly Color LIGHT_BEIGE = new Color(0.9803f, 0.9568f, 0.9019f, 0.4f); // statement-box unfocused
    readonly Color SHADED_BEIGE = new Color(0.7803f, 0.7568f, 0.7019f, 1.0f); // statement-box unfocused (not used)
    readonly Color WHITE = new Color(1.0f, 1.0f, 1.0f, 1.0f); // arrow focused
    readonly Color LIGHT_WHITE = new Color(1.0f, 1.0f, 1.0f, 0.4f); // arrow unfocused


    // for developmental use only
    private void DevelopmentPurposeAssign()
    {
        SmartObject.cardSelectionDict["environment"] = "TrashBin";
        SmartObject.cardSelectionDict["input"] = "Fire";
        SmartObject.cardSelectionDict["output"] = "LightUp";

        Debug.Log("[env, in, out] = ["
            + SmartObject.cardSelectionDict["environment"] + ", "
            + SmartObject.cardSelectionDict["input"] + ", "
            + SmartObject.cardSelectionDict["output"] + "]");
    }


    // keep in mind DRY: Don't Repeat Yourself
    private void Start()
    {
        DevelopmentPurposeAssign(); // to delete after development

        DeactivateTaggedObjects();
        ActivateTaggedObjects();
        environmentObject = GetEnvObjByName(SmartObject.cardSelectionDict["environment"]);
        inputProps = GetInPropsByName(SmartObject.cardSelectionDict["input"]);
        outputProps = GetOutPropsByName(SmartObject.cardSelectionDict["output"]);

        AddInstanceToList();

        ///

        //int idx = 0;
        //int minInstanceID = AvailableMinInstanceID();
        //inputInstanceList.Add(GetInputInstanceByName(SmartObject.cardSelectionDict["input"]));
        //outputInstanceList.Add(GetOutputInstanceByName(SmartObject.cardSelectionDict["output"]));
        //ioArrowList.Add(CreateIOArrow());

        ////inputInstanceList[idx].CardDescriptionSetup(
        ////    ref inputCardNameField, ref inputDescriptionField); // only needed for first instance => call in Start()

        //inputInstanceList[idx].CardStatementSetup(
        //    ref environmentObject, ref inputProps, ref inputStatementFieldGroup, minInstanceID);

        ////outputInstanceList[idx].CardDescriptionSetup(
        ////    ref outputCardNameField, ref outputDescriptionField); // only needed for first instance => call in Start()

        //outputInstanceList[idx].CardStatementSetup(
        //    ref environmentObject, ref outputProps, ref outputStatementFieldGroup, minInstanceID);

        ////ioArrowList.Add(Instantiate(ioArrowObject, ioArrowObject.transform.parent));
        ////ioArrowList[0].SetActive(true);

        ////GrantFocusToTargetInstances(idx);
        //ShiftFocusToTargetInstances(idx);
        //ShiftFocusToTargetIOArrow(idx);

        //inputInstanceList[idx].StatementFieldGroup.
        //    GetComponent<Button>().onClick.AddListener(
        //    () => { StatementFieldOnClick(inputInstanceList[idx].InstanceID); }
        //    );
        //outputInstanceList[idx].StatementFieldGroup.
        //    GetComponent<Button>().onClick.AddListener(
        //    () => { StatementFieldOnClick(outputInstanceList[idx].InstanceID); }
        //    );

        ///

        inputInstanceList[0].CardDescriptionSetup(
            ref inputCardNameField, ref inputDescriptionField);

        outputInstanceList[0].CardDescriptionSetup(
            ref outputCardNameField, ref outputDescriptionField);

        // Button settings
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

        confirmationMessageField.SetText(beforeConfirmMessage);
    }



    private void AddInstanceToList()
    {
        inputInstanceList.Add(GetInputInstanceByName(SmartObject.cardSelectionDict["input"]));
        outputInstanceList.Add(GetOutputInstanceByName(SmartObject.cardSelectionDict["output"]));
        ioArrowList.Add(CreateIOArrow()); // must be calledbefore "DepriveFocusFromOtherAndGrantToTargetInstances(idx);"
        int idx = inputInstanceList.Count - 1; // tail of updated list; call after adding new instance
        int minInstanceID = AvailableMinInstanceID(); // can be called either before or after adding new instance
        inputInstanceList[idx].CardStatementSetup(ref environmentObject, ref inputProps, ref inputStatementFieldGroup, minInstanceID);
        outputInstanceList[idx].CardStatementSetup(ref environmentObject, ref outputProps, ref outputStatementFieldGroup, minInstanceID);
        //GrantFocusToTargetInstances(idx);
        //DepriveFocusFromOtherInstances(idx);
        ShiftFocusToTargetInstances(idx);
        ShiftFocusToTargetIOArrow(idx);

        // ボタンの登録　（一番初めのinstanceのボタン登録はStart()で個別にやる）
        inputInstanceList[idx].StatementFieldGroup.
            GetComponent<Button>().onClick.AddListener(
            () => { StatementFieldOnClick(inputInstanceList[idx].InstanceID); }
            );
        outputInstanceList[idx].StatementFieldGroup.
            GetComponent<Button>().onClick.AddListener(
            () => { StatementFieldOnClick(outputInstanceList[idx].InstanceID); }
            );
        // どのインスタンスのボタンが押されたのかの情報が欲しい => TODO この方法(lambda expression)で実現できているのかの動作確認
        // inputInstanceList[idx].InstanceID は最初にセットされた後は、書き換えられない想定だからユニークなインスタンスに紐ずく

        ShiftStatementFieldPositions(); // 他人を押し上げる

        // check for allawability of adding and removing instances
        addInstanceButton.interactable = CheckInstanceListCapacity();
        removeInstanceButton.interactable = !(inputInstanceList.Count <= 1);
    }



    // Update() の中は必要最低限に留めて、発火を活用した方が効率的
    private void Update()
    {
        int instanceCount = inputInstanceList.Count;
        int focusedIdx = GetFocusedInstanceIndex();
        inputInstanceList[focusedIdx].UpdateInputCondition();
        outputInstanceList[focusedIdx].UpdateOutputBehaviour();

        // Check if the selected keywords have no overlaps
        if (!ConditionKeywordIsUnique(focusedIdx))
        {
            inputInstanceList[focusedIdx].ConditionKeyword = inputInstanceList[focusedIdx].ALREADY_EXISTS;
        }

        // check if all instances has been set a value
        bool isConfirmable = CheckConfirmability(); // CanBeConfirmed プロパティを各カードクラスで更新する
        if (isConfirmable) confirmationButton.interactable = true;
        else confirmationButton.interactable = false;

        if (isConfirmed)
        {
            for (int i = 0; i < instanceCount; i++) // 全てのインスタンスが対象
            {
                if (inputInstanceList[i].inputCondition)
                {
                    outputInstanceList[i].OutputBehaviour();
                }
                else
                {
                    outputInstanceList[i].OutputBehaviourNegative();
                }
            }
        }

        if (OVRInput.GetDown(OVRInput.RawButton.Start))
        {
            if (!menuIsOpened) OpenMenu();
            else CloseMenu();
        }

    }


    private bool ConditionKeywordIsUnique(int focusedIdx)
    {
        if (string.IsNullOrEmpty(inputInstanceList[focusedIdx].ConditionKeyword)) return true;
        for (int i = 0; i < inputInstanceList.Count; i++)
        {
            if (i == focusedIdx) continue;
            if (inputInstanceList[i].ConditionKeyword
                == inputInstanceList[focusedIdx].ConditionKeyword) return false;
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


    private bool CheckInstanceListCapacity()
    {
        return inputInstanceList.Count < inputInstanceList[0].MaxInstanceNum;
    }


    // IO-Statement-Field Button OnClick
    // recieves instance-ID of the clicked statement-box
    private void StatementFieldOnClick(int instanceID)
    {
        int idx = GetInstanceListIndexFromInstanceID(instanceID);
        ShiftFocusToTargetInstances(idx);
        ShiftFocusToTargetIOArrow(idx);
    }


    private void ShiftStatementFieldPositions()
    {
        Vector3 inputBasePosition = inputStatementFieldGroup.transform.localPosition;
        Vector3 outputBasePosition = outputStatementFieldGroup.transform.localPosition;
        int instanceCount = inputInstanceList.Count;
        for (int i = 0; i < instanceCount; i++)
        {
            Vector3 verticalShiftAmount = new Vector3(0, VSHAMT * (instanceCount - 1 - i), 0);
            inputInstanceList[i].StatementFieldGroup.transform.localPosition = inputBasePosition + verticalShiftAmount;
            outputInstanceList[i].StatementFieldGroup.transform.localPosition = outputBasePosition + verticalShiftAmount;
        }
    }


    // remove focused IO-Instance
    private void RemoveInstanceFromList()
    {
        for (int i = 0; i < inputInstanceList.Count; i++)
        {
            if (inputInstanceList[i].IsFocused)
            {
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
        if (inputInstanceList.Count <= 1) removeInstanceButton.interactable = false;
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
        ioArrowList.RemoveAt(ioArrowList.Count-1);
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


    // parameter int:idx is passed assuming the use with IOInstance index order
    // set optional parameter to 'true' if calling with real index
    private void ShiftFocusToTargetIOArrow(int idx, bool realIndex=false)
    {
        int revIdx;
        if (realIndex) revIdx = idx;
        else revIdx = ioArrowList.Count - 1 - idx;
        for (int i = 0; i < ioArrowList.Count;  i++)
        {
            if (i == revIdx) ioArrowList[revIdx].GetComponent<Image>().color = WHITE;
            else ioArrowList[revIdx].GetComponent<Image>().color = LIGHT_WHITE;
        }
        
    }


    private void SetIOInstanceFocusState(int idx, bool tf)
    {
        inputInstanceList[idx].IsFocused = tf;
        outputInstanceList[idx].IsFocused = tf;
    }


    private void SetIOStatementFieldColor(int idx, Color color)
    {
        inputInstanceList[idx].StatementFieldGroup.GetComponent<Image>().color = color;
        outputInstanceList[idx].StatementFieldGroup.GetComponent<Image>().color = color;
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


    // Confirmation Button OnClick
    private void ConfirmSmartObject()
    {
        isConfirmed = true;

        int instanceCount = inputInstanceList.Count;
        for (int i = 0; i < instanceCount; i++) {
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


    // Back-To-Edit Button OnClick
    private void GoBackToEditMode()
    {
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


    private void ChangeUnfocusedArrowColors(Color color)
    {
        for (int i = 0; i < ioArrowList.Count; i++)
        {
            if (inputInstanceList[i].IsFocused) continue;
            else ioArrowList[i].GetComponent<Image>().color = color;
        }
    }


    // Finalization Button OnClick
    private void FinalizeSmartObject()
    {
        // todo pack smart object information data and pass to next scene
        // => CardSelectionMediator class に新しいフィールドを用意する
        // Smart Object を記述するのに何が必要かを考える（environment object, input-delegate, output-behaviour, props, ...）
        // delegateのdictionaryと、選択されているキーワードのリスト(<=新しく作成する)も渡す
        Debug.Log("Moving to InteractionScene");
        // SceneManager.LoadScene(3); // InteractionScene
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


    // safety measure for preventing human errors
    private void ActivateTaggedObjects()
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("ActivateOnLoad");
        foreach (GameObject obj in taggedObjects) obj.SetActive(false);
    }


    // safety measure for preventing human errors
    private void DeactivateTaggedObjects()
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("DeactivateOnLoad");
        foreach (GameObject obj in taggedObjects) obj.SetActive(false);
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