using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class MainSceneManager : MonoBehaviour
{
    /// <summary>
    /// card selection related fields
    /// </summary>

    CardSelectionDetector EnvSelectionDetector;
    [SerializeField] private GameObject envBoxObj = null;
    [SerializeField] private GameObject[] envObjArr = null;

    CardSelectionDetector InSelectionDetector;
    [SerializeField] private GameObject inBoxObj = null;
    [SerializeField] private GameObject[] inObjArr = null;

    CardSelectionDetector OutSelectionDetector;
    [SerializeField] private GameObject outBoxObj = null;
    [SerializeField] private GameObject[] outObjArr = null;

    /// <summary>
    /// prototyping related fields
    /// </summary>

    [SerializeField] private GameObject envObjTrashBin = null;                  // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject envObjTree = null;                      // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject envObjStreetLight = null;               // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject envObjStreetSign = null;                // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject envObjBridge = null;                    // put tag "DeactivateOnLoad"

    [SerializeField] private GameObject inPropButton = null;                   // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject inPropSound = null;                    // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject inPropFire = null;                     // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject inPropSpeed = null;                    // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject inPropWeather = null;                  // put tag "DeactivateOnLoad"

    [SerializeField] private GameObject outPropLightUp = null;                 // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject outPropMakeSound = null;               // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject outPropVibrate = null;                 // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject outPropMove = null;                    // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject outPropSend = null;                    // put tag "DeactivateOnLoad"

    [SerializeField] private TextMeshProUGUI inputCardNameField = null;
    [SerializeField] private TextMeshProUGUI inputDescriptionField = null;
    [SerializeField] private GameObject inputStatementFieldGroup = null;        // put tag "DeactivateOnLoad"

    [SerializeField] private TextMeshProUGUI outputCardNameField = null;
    [SerializeField] private TextMeshProUGUI outputDescriptionField = null;
    [SerializeField] private GameObject outputStatementFieldGroup = null;       // put tag "DeactivateOnLoad"

    [SerializeField] private Button addInstanceButton = null;
    [SerializeField] private Button removeInstanceButton = null;

    [SerializeField] private GameObject ioArrowObject = null;                   // put tag "DeactivateOnLoad"

    [SerializeField] private TextMeshProUGUI confirmationMessageField = null;
    [SerializeField] private Button buildButton = null;
    [SerializeField] private Button exportButton = null;                        // put tag "DeactivateOnLoad"
    [SerializeField] private Button reEditButton = null;                        // put tag "DeactivateOnLoad"
    readonly string beforeConfirmMessage = "Ready to test the Smart Object?";
    readonly string afterConfirmMessage = "Do you want to keep editing or finalize?";
    // todo change behavior after preview mode ("confirm")

    private GameObject inputSelection;
    private GameObject outputSelection;

    private GameObject envObj;
    private SmartObject smartObj;
    private GameObject inputProp;
    private GameObject outputProp;

    // IO-Instance list: mapping between index and vertical positioning is in descending order
    // Arrow list: mapping between index and vertical positioning is in ascending order
    private List<InputCard> inputCardInstanceList = new List<InputCard>();
    private List<OutputCard> outputCardInstanceList = new List<OutputCard>();
    private List<GameObject> ioArrowList = new List<GameObject>();

    bool isBuilt = false;

    const float VSHAMT = 0.5f;                                                  // vertical shift amount of statement fields
    readonly Color BEIGE = new Color(0.9803f, 0.9568f, 0.9019f, 1.0f);          // when statement-box is focused
    readonly Color LIGHT_BEIGE = new Color(0.9803f, 0.9568f, 0.9019f, 0.6f);    // when statement-box is unfocused
    readonly Color POP_BEIGE = new Color(1.0f, 0.9210f, 0.7405f, 1.0f);         // when statement-box is in effect
    readonly Color WHITE = new Color(1.0f, 1.0f, 1.0f, 1.0f);                   // when arrow is focused
    readonly Color LIGHT_WHITE = new Color(1.0f, 1.0f, 1.0f, 0.4f);             // when arrow is unfocused

    [SerializeField] private GameObject editorEventSystem = null;               // for developmental use only
    [SerializeField] private GameObject HMDEventSystem = null;                  // for developmental use only

    private AudioSource[] clickSound;

    /// <summary>
    /// member methods
    /// </summary>

    void Start()
    {
        DevConfigSetting();
        SetActiveStateByTag("DeactivateOnLoad", false);
        SetActiveStateByTag("ActivateOnLoad", true);
        ClearCardDescriptionTextFields();
        CardSelectionBoxRegistration();
        UIButtonSetting();        
    }

    void Update()
    {
        CardSelectionBoxCenterDragMotionUpdate();
        CardSelectionDetectionUpdate();

        if (!isBuilt) // prototyping phase
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
            if (isConfirmable)
            {
                confirmationButton.interactable = true;
                confirmationMessageField.SetText(beforeConfirmMessage);
                // ここでメッセージを出すように変更；インタラクティブに随時更新するようにする
            }
            else confirmationButton.interactable = false;
        }
        else // during preview phase
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

    }

    private void CardSelectionDetectionUpdate()
    {
        // triggered once when box content is changed
        if (EnvSelectionDetector.TriggerFlag)
        {
            GameObject envSelection = EnvSelectionDetector.SelectedCardObj;
            if (envSelection != null)
            {
                envObj = InstantiateEnvObjByName(envSelection.name);
                envObj.SetActive(true);
                smartObj = envObj.AddComponent<SmartObject>();
                // todo 他の処理たちについて、smartObjで受け渡すようにコードを改変する
            }
            else
            {
                Destroy(envObj);
            }
        }


        if (InSelectionDetector.TriggerFlag)
        {
            inputSelection = InSelectionDetector.SelectedCardObj;
            if (inputSelection != null) // identical to Start() for each selection
            {
                inputProp = InstantiateInputPropByName(inputSelection.name);
                inputProp.SetActive(true);
                AddInputCardInstanceToList();
                inputCardInstanceList[0].CardDescriptionSetup(ref inputCardNameField, ref inputDescriptionField);
            }
            else
            {
                Destroy(inputProp);
                foreach (InputCard inputCardInstance in inputCardInstanceList)
                    Destroy(inputCardInstance.StatementFieldGroup);
                inputCardInstanceList.Clear();
            }
        }

        // todo 関数かしたい；inputとの重複が多い
        if (OutSelectionDetector.TriggerFlag)
        {
            outputSelection = OutSelectionDetector.SelectedCardObj;
            if (outputSelection != null)
            {
                outputProp = InstantiateOutputPropByName(outputSelection.name);
                outputProp.SetActive(true);
                AddOutputCardInstanceToList();
                outputCardInstanceList[0].CardDescriptionSetup(ref outputCardNameField, ref outputDescriptionField);
            }
            else
            {
                Destroy(outputProp);
                foreach (OutputCard outputCardInstance in outputCardInstanceList)
                    Destroy(outputCardInstance.StatementFieldGroup);
                outputCardInstanceList.Clear();
            }
        }
    }

    private void CardSelectionBoxCenterDragMotionUpdate()
    {
        EnvSelectionDetector.CenterDragMotion();
        InSelectionDetector.CenterDragMotion();
        OutSelectionDetector.CenterDragMotion();
    }

    // todo buildButtonがどう言う時に押せるようになるのかの処理は、これからUpdate()の中に書く
    // todo その時には、inputとoutputがともに選択されていることなどを確認する処理が必要だと思われる

    // buildButton OnClick
    private void BuildSmartObject()
    {
        isBuilt = true;

        int cnt = inputCardInstanceList.Count;
        for (int i = 0; i < cnt; i++)
        {
            inputCardInstanceList[i].IsConfirmed = true;
            outputCardInstanceList[i].IsConfirmed = true;
        }

        buildButton.interactable = false;
        buildButton.gameObject.SetActive(false);
        exportButton.interactable = true;
        exportButton.gameObject.SetActive(true);
        reEditButton.interactable = true;
        reEditButton.gameObject.SetActive(true);
        confirmationMessageField.SetText(afterConfirmMessage);

        addInstanceButton.interactable = false;
        removeInstanceButton.interactable = false;

        SetCardStatementFieldInteractability(false);
        SetUnfocusedCardStatementFieldColors(BEIGE);
        SetUnfocusedArrowColors(WHITE);
    }

    // reEditButton OnClick
    private void ReEditSmartObject()
    {
        isBuilt = false;

        int cnt = inputCardInstanceList.Count;
        for (int i = 0; i < cnt; i++)
        {
            inputCardInstanceList[i].IsConfirmed = false;
            outputCardInstanceList[i].IsConfirmed = false;
        }

        buildButton.interactable = true;
        buildButton.gameObject.SetActive(true);
        exportButton.interactable = false;
        exportButton.gameObject.SetActive(false);
        reEditButton.interactable = false;
        reEditButton.gameObject.SetActive(false);
        confirmationMessageField.SetText(beforeConfirmMessage);

        addInstanceButton.interactable = CheckInstanceListCapacity();
        removeInstanceButton.interactable = !(inputCardInstanceList.Count <= 1);

        SetCardStatementFieldInteractability(true);
        SetUnfocusedCardStatementFieldColors(LIGHT_BEIGE);
        SetUnfocusedArrowColors(LIGHT_WHITE);
    }

    // exportButton OnClick
    private void ExportSmartObject()
    {
        Debug.Log("exporting smart object...");
    }


    private void SetUnfocusedCardStatementFieldColors(Color color)
    {
        int cnt = inputCardInstanceList.Count;
        for (int i = 0; i < cnt; i++)
        {
            if (inputCardInstanceList[i].IsFocused) continue;
            else SetCardStatementFieldColor(i, color);
        }
    }

    private void SetUnfocusedArrowColors(Color color)
    {
        int cnt = ioArrowList.Count;
        for (int i = 0; i < cnt; i++)
        {
            if (inputCardInstanceList[RevIdx(i)].IsFocused) continue;
            else ioArrowList[i].GetComponent<Image>().color = color;
        }
    }

    private void SetCardStatementFieldColor(int idx, Color color)
    {
        if (inputSelection != null) // todo この != たちを解決する
            inputCardInstanceList[idx].StatementFieldGroup.GetComponent<Image>().color = color;
        if (outputSelection != null)
            outputCardInstanceList[idx].StatementFieldGroup.GetComponent<Image>().color = color;
    }

    private void SetCardStatementFieldInteractability(bool tf)
    {
        int cnt = inputCardInstanceList.Count;
        for (int i = 0; i < cnt; i++)
        {
            inputCardInstanceList[i].StatementFieldGroup.
                GetComponent<Button>().interactable = tf;
            outputCardInstanceList[i].StatementFieldGroup.
                GetComponent<Button>().interactable = tf;
        }
    }

    // todo inとoutで、インタフェース部分は別れていてもいいけど、内部の共通部分は統合したい（共通部分だけ抜き出すば十分）
    private void AddOutputCardInstanceToList()
    {
        if (inputCardInstanceList.Count == 0)
        {
            outputCardInstanceList.Add(GetOutputCardInstanceByName(outputSelection.name));
            int idx = outputCardInstanceList.Count - 1;
            int minInstanceID = AvailableMinInstanceID(outputCardInstanceList);
            outputCardInstanceList[idx].CardStatementSetup(
                minInstanceID, ref smartObj, ref outputProp, ref outputStatementFieldGroup);
            ShiftFocusToTargetInstance(outputCardInstanceList, idx);
            ShiftStatementFieldPositions(outputCardInstanceList);
            outputCardInstanceList[idx].StatementFieldGroup.
                GetComponent<Button>().onClick.AddListener(StatementFieldOnClick);
        }
        else
        {
            while (outputCardInstanceList.Count < inputCardInstanceList.Count)
            {
                outputCardInstanceList.Add(GetOutputCardInstanceByName(outputSelection.name));
                int idx = outputCardInstanceList.Count - 1;
                //int minInstanceID = AvailableMinInstanceID(outputCardInstanceList);
                int minInstanceID = inputCardInstanceList[idx].InstanceID;
                outputCardInstanceList[idx].CardStatementSetup(
                    minInstanceID, ref smartObj, ref outputProp, ref outputStatementFieldGroup);
                //ShiftFocusToTargetInstance(outputCardInstanceList, idx);
                if (inputCardInstanceList[idx].IsFocused)
                    ShiftFocusToTargetInstance(outputCardInstanceList, idx);
                ShiftStatementFieldPositions(outputCardInstanceList);
                outputCardInstanceList[idx].StatementFieldGroup.
                    GetComponent<Button>().onClick.AddListener(StatementFieldOnClick);
            }
        }
    }


    private void AddInputCardInstanceToList()
    {
        inputCardInstanceList.Add(GetInputCardInstanceByName(inputSelection.name));
        int idx = inputCardInstanceList.Count - 1; // get tail of updated list; call after adding new instance
        int minInstanceID = AvailableMinInstanceID(inputCardInstanceList);
        inputCardInstanceList[idx].CardStatementSetup(
            minInstanceID, ref smartObj, ref inputProp, ref inputStatementFieldGroup);
        ShiftFocusToTargetInstance(inputCardInstanceList, idx);
        ShiftStatementFieldPositions(inputCardInstanceList);
        inputCardInstanceList[idx].StatementFieldGroup.
            GetComponent<Button>().onClick.AddListener(StatementFieldOnClick);

        // check for allowability of adding and removing instances
        addInstanceButton.interactable = CheckInstanceListCapacity();
        removeInstanceButton.interactable = !(inputCardInstanceList.Count <= 1);
        // todo こいつはAddCardInstanceToList()の方での処理にしたいけど、InstanceListCapacityが1のinputCardがあったら問題が生じる

        ioArrowList.Add(CreateIOArrow());
        ShiftFocusToTargetIOArrow(idx);
    }

    // addInstanceButton OnClick
    private void AddCardInstanceToList()
    {
        AddInputCardInstanceToList();
        if (outputSelection != null)
            AddOutputCardInstanceToList(); // must be called after AddInputCardInstanceToList()
    }

    // removeInstanceButton OnClick
    private void RemoveCardInstanceFromList()
    {
        for (int i = 0; i < inputCardInstanceList.Count; i++)
        {
            if (inputCardInstanceList[i].IsFocused)
            {
                Destroy(inputCardInstanceList[i].StatementFieldGroup);
                inputCardInstanceList.RemoveAt(i);
                if (outputSelection != null)
                {
                    Destroy(outputCardInstanceList[i].StatementFieldGroup);
                    outputCardInstanceList.RemoveAt(i);
                }
                break;
            }
        }

        // move focus to the youngest instance
        GrantFocusToTargetCardInstances(inputCardInstanceList.Count - 1);
        ShiftStatementFieldPositions(inputCardInstanceList);
        if (outputSelection != null)
            ShiftStatementFieldPositions(outputCardInstanceList); // todo 全体に散らばっている if (outputSelection != null) をなんとかする

        ReduceIOArrow();
        ShiftFocusToTargetIOArrow(0, true);

        addInstanceButton.interactable = CheckInstanceListCapacity();
        removeInstanceButton.interactable = !(inputCardInstanceList.Count <= 1);
    }

    private void GrantFocusToTargetCardInstances(int targetIdx)
    {
        SetCardInstanceFocusState(targetIdx, true);
        SetCardStatementFieldColor(targetIdx, BEIGE);
    }

    private void SetCardInstanceFocusState(int idx, bool tf)
    {
        inputCardInstanceList[idx].IsFocused = tf;
        if (outputSelection != null)
            outputCardInstanceList[idx].IsFocused = tf;
    }


    private void StatementFieldOnClick()
    {
        int instanceIdx = GetCurrentSelectedCardInstanceIndex();
        if (inputCardInstanceList.Count > 0)
        {
            ShiftFocusToTargetInstance(inputCardInstanceList, instanceIdx);
            ShiftFocusToTargetIOArrow(instanceIdx);
        }
        if (outputCardInstanceList.Count > 0)
        {
            ShiftFocusToTargetInstance(outputCardInstanceList, instanceIdx);
        }   
    }

    // remove the top-most arrow
    private void ReduceIOArrow()
    {
        int tail = ioArrowList.Count - 1;
        Destroy(ioArrowList[tail]);
        ioArrowList.RemoveAt(tail);
    }




    /// <summary>
    /// helper methods
    /// </summary>

    private bool CheckInstanceListCapacity()
    {
        return inputCardInstanceList.Count < inputCardInstanceList[0].MaxInstanceNum;
    }

    private int GetCurrentSelectedCardInstanceIndex()
    {
        // get "instance-index" from "statement-field-button-label"
        string statementIdText = EventSystem.current.currentSelectedGameObject.
            transform.Find("IndexText").gameObject.GetComponent<TMP_Text>().text; // todo modify to method without "find()"
        string extrudedIdText = Regex.Replace(statementIdText, @"[^0-9]", "");
        int extrudedId = int.Parse(extrudedIdText);
        int instanceID = extrudedId - 1;
        int instanceIdx = GetInstanceListIndexFromInstanceID(instanceID);
        return instanceIdx;
    }

    // returns index in Instance List 
    // returns -1 when not found in Instance List
    private int GetInstanceListIndexFromInstanceID(int id)
    {
        int cnt = inputCardInstanceList.Count;
        for (int i = 0; i < cnt; i++)
        {
            if (inputCardInstanceList[i].InstanceID == id) return i;
        }
        return -1;
    }

    private void ShiftFocusToTargetInstance<Type>(List<Type> cardInstanceList, int idx)
        where Type : Card
    {
        int cnt = cardInstanceList.Count;
        for (int i = 0; i < cnt; i++)
        {
            if (i == idx)
            {
                cardInstanceList[idx].IsFocused = true;
                cardInstanceList[idx].StatementFieldGroup.GetComponent<Image>().color = BEIGE;
            }
            else
            {
                cardInstanceList[idx].IsFocused = false;
                cardInstanceList[idx].StatementFieldGroup.GetComponent<Image>().color = LIGHT_BEIGE;
            }
        }
    }

    private void ShiftStatementFieldPositions<Type>(List<Type> cardInstanceList)
        where Type : Card
    {
        int cnt = cardInstanceList.Count;
        int tail = cnt - 1;
        Vector3 basePosition = cardInstanceList[tail].StatementFieldGroup.transform.localPosition;
        for (int i = 0; i < cnt; i++)
        {
            Vector3 verticalShiftAmount = new Vector3(0, VSHAMT * (cnt - 1 - i), 0);
            cardInstanceList[i].StatementFieldGroup.transform.localPosition = basePosition + verticalShiftAmount;
        }
    }

    // parameter int:idx is passed assuming the use with IOInstance index order
    // set optional parameter to 'true' if calling with real index
    // todo make the IOArrow order match with IOStatements
    private void ShiftFocusToTargetIOArrow(int idx, bool realIndex = false)
    {
        if (!realIndex)
            idx = RevIdx(idx);
        for (int i = 0; i < ioArrowList.Count; i++)
        {
            if (i == idx) ioArrowList[i].GetComponent<Image>().color = WHITE;
            else ioArrowList[i].GetComponent<Image>().color = LIGHT_WHITE;
        }
    }

    private int RevIdx(int idx)
    {
        return ioArrowList.Count - 1 - idx;
    }


    private GameObject InstantiateEnvObjByName(string cardName)
    {
        switch (cardName)
        {
            case "TrashBin": return Instantiate(envObjTrashBin);
            case "Tree": return Instantiate(envObjTree);
            case "StreetLight": return Instantiate(envObjStreetLight);
            case "StreetSign": return Instantiate(envObjStreetSign);
            case "Bridge": return Instantiate(envObjBridge);
            default: return null;
        }
    }


    private GameObject InstantiateInputPropByName(string cardName)
    {
        switch (cardName)
        {
            case "Button": return Instantiate(inPropButton);
            case "Sound": return Instantiate(inPropSound);
            case "Fire": return Instantiate(inPropFire);
            case "Speed": return Instantiate(inPropSpeed);
            case "Weather": return Instantiate(inPropWeather);
            default: return null;
        }
    }

    private InputCard GetInputCardInstanceByName(string cardName)
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


    private GameObject InstantiateOutputPropByName(string cardName)
    {
        switch (cardName)
        {
            case "LightUp": return Instantiate(outPropLightUp);
            case "MakeSound": return Instantiate(outPropMakeSound);
            case "Vibrate": return Instantiate(outPropVibrate);
            case "Move": return Instantiate(outPropMove);
            case "Send": return Instantiate(outPropSend);
            default: return null;
        }
    }

    private OutputCard GetOutputCardInstanceByName(string cardName)
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

    // returns position adjusted io-arrow gameObject
    private GameObject CreateIOArrow()
    {
        GameObject newArrow = Instantiate(ioArrowObject, ioArrowObject.transform.parent);
        newArrow.transform.localPosition += new Vector3(0, VSHAMT * (ioArrowList.Count), 0);
        newArrow.SetActive(true);
        return newArrow;
    }

    private int AvailableMinInstanceID<Type>(List<Type> cardInstanceList)
        where Type : Card
    {
        int cnt = cardInstanceList.Count;
        int idCandidate = 0;
        for (int i = 0; i < cnt; i++)
        {
            if (cardInstanceList[i].InstanceID == -1)
                continue; // skip itself (set to initial value)
            if (cardInstanceList[i].InstanceID == idCandidate)
                idCandidate++;
        }
        return idCandidate;
    }

    private void UIButtonSetting()
    {
        addInstanceButton.onClick.AddListener(AddCardInstanceToList);
        addInstanceButton.interactable = false;
        removeInstanceButton.onClick.AddListener(RemoveCardInstanceFromList);
        removeInstanceButton.interactable = false;
        buildButton.onClick.AddListener(BuildSmartObject);
        buildButton.interactable = false;
        exportButton.onClick.AddListener(ExportSmartObject);
        exportButton.interactable = false;
        reEditButton.onClick.AddListener(ReEditSmartObject);
        reEditButton.interactable = false;
    }

    private void CardSelectionBoxRegistration()
    {
        EnvSelectionDetector
            = new CardSelectionDetector(envBoxObj, envObjArr);
        InSelectionDetector
            = new CardSelectionDetector(inBoxObj, inObjArr);
        OutSelectionDetector
            = new CardSelectionDetector(outBoxObj, outObjArr);
    }

    private void ClearCardDescriptionTextFields()
    {
        inputCardNameField.SetText(string.Empty);
        outputCardNameField.SetText(string.Empty);
        inputDescriptionField.SetText(string.Empty);
        outputDescriptionField.SetText(string.Empty);
    }


    private void SetActiveStateByTag(string tag, bool state)
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in taggedObjects) obj.SetActive(state);
    }

    // decide between button-clicks (editor use) or laser-pointer (HMD use)
    private void DevConfigSetting()
    {
        bool isQuest2 =
            OVRManager.systemHeadsetType
            == OVRManager.SystemHeadsetType.Oculus_Quest_2;
        HMDEventSystem.SetActive(isQuest2);
        editorEventSystem.SetActive(!isQuest2);
        GameObject[] objectsWithRaycaster = GameObject.FindGameObjectsWithTag("ObjectWithRaycaster");
        foreach (GameObject obj in objectsWithRaycaster)
        {
            obj.GetComponent<OVRRaycaster>().enabled = isQuest2;
            obj.GetComponent<GraphicRaycaster>().enabled = !isQuest2;
        }
    }

}

/*

まずは、選択された側から即座に対象がPrototypingエリアに反映されるようにする
その選択が箱から解除された時には、その設定だけがリセットされるようにする
 
 */