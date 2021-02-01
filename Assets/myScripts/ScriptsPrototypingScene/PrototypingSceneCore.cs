﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;


public class PrototypingSceneCore : MonoBehaviour
{

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

    [SerializeField] private TextMeshProUGUI inputCardNameField = null;
    [SerializeField] private TextMeshProUGUI inputDescriptionField = null;
    [SerializeField] private GameObject inputStatementFieldGroup = null; // contains Button component

    [SerializeField] private TextMeshProUGUI outputCardNameField = null;
    [SerializeField] private TextMeshProUGUI outputDescriptionField = null;
    [SerializeField] private GameObject outputStatementFieldGroup = null; // contains Button component

    [SerializeField] private Button addInstanceButton = null;
    [SerializeField] private Button removeInstanceButton = null;

    [SerializeField] private GameObject ioArrowObject = null;

    [SerializeField] private TextMeshProUGUI confirmationMessageField = null;
    [SerializeField] private Button confirmationButton = null;
    [SerializeField] private Button backToEditButton = null;
    [SerializeField] private Button finalizationButton = null;
    readonly string beforeConfirmMessage = "Ready to test the Smart Object?";
    readonly string afterConfirmMessage = "Do you want to keep editing or finalize?";

    [SerializeField] private GameObject menuCanvas = null;
    [SerializeField] private Button backToSceneButton = null;
    [SerializeField] private Button closeMenuButton = null;

    private GameObject environmentObject;
    private GameObject inputProps;
    private GameObject outputProps;
    private List<InputCard> inputInstanceList = new List<InputCard>();
    private List<OutputCard> outputInstanceList = new List<OutputCard>();
    private List<GameObject> ioArrowList = new List<GameObject>();
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

        DeactivateAllCardRepresentations();
        environmentObject = GetEnvObjByName(SmartObject.cardSelectionDict["environment"]);

        int idx = 0;
        int minInstanceID = AvailableMinInstanceID();

        inputProps = GetInPropsByName(SmartObject.cardSelectionDict["input"]);
        inputInstanceList.Add(GetInputInstanceByName(SmartObject.cardSelectionDict["input"]));
        inputInstanceList[idx].CardDescriptionSetup(
            ref inputCardNameField, ref inputDescriptionField);
        inputInstanceList[idx].CardStatementSetup(
            ref environmentObject, ref inputProps, ref inputStatementFieldGroup, minInstanceID);

        outputProps = GetOutPropsByName(SmartObject.cardSelectionDict["output"]);
        outputInstanceList.Add(GetOutputInstanceByName(SmartObject.cardSelectionDict["output"]));
        outputInstanceList[idx].CardDescriptionSetup(
            ref outputCardNameField, ref outputDescriptionField);
        outputInstanceList[idx].CardStatementSetup(
            ref environmentObject, ref outputProps, ref outputStatementFieldGroup, minInstanceID);

        ioArrowList.Add(Instantiate(ioArrowObject, ioArrowObject.transform.parent));
        ioArrowList[0].SetActive(true);

        GrantFocusToTargetInstances(idx);

        // ボタンとしてのstatementBoxの追加と、それが押下された時にIsFocusedがtrueになる処理
        //inputInstanceList[idx].StatementFieldGroup.GetComponent<Button>().onClick.AddListener(StatementBoxOnClick);
        //outputInstanceList[idx].StatementFieldGroup.GetComponent<Button>().onClick.AddListener(StatementBoxOnClick);
        inputInstanceList[idx].StatementFieldGroup.
            GetComponent<Button>().onClick.AddListener(
            () => { StatementFieldOnClick(inputInstanceList[idx].InstanceID); }
            );
        outputInstanceList[idx].StatementFieldGroup.
            GetComponent<Button>().onClick.AddListener(
            () => { StatementFieldOnClick(outputInstanceList[idx].InstanceID); }
            );
        // todo この方法で良いのかの確認



        // UI button settings

        addInstanceButton.onClick.AddListener(AddInstanceToList);
        addInstanceButton.interactable = CheckInstanceListCapacity();
        removeInstanceButton.onClick.AddListener(RemoveInstanceFromList);
        removeInstanceButton.interactable = false;

        confirmationMessageField.SetText(beforeConfirmMessage);
        confirmationButton.onClick.AddListener(ConfirmSmartObject);
        confirmationButton.interactable = false; // set to true when every-instances.canBeConfirmed is true
        confirmationButton.gameObject.SetActive(true);
        backToEditButton.onClick.AddListener(GoBackToEditMode);
        backToEditButton.interactable = false;
        backToEditButton.gameObject.SetActive(false);
        finalizationButton.onClick.AddListener(FinalizeSmartObject);
        finalizationButton.interactable = false;
        finalizationButton.gameObject.SetActive(false);


        backToSceneButton.onClick.AddListener(LoadCardSelectionScene);
        closeMenuButton.onClick.AddListener(CloseMenu);

    }



    // todo Startの中の処理と合わせて、リファクタの余地あり; ボタン登録も関数化可能(indexを引数に渡す)
    private void AddInstanceToList()
    {
        inputInstanceList.Add(GetInputInstanceByName(SmartObject.cardSelectionDict["input"]));
        outputInstanceList.Add(GetOutputInstanceByName(SmartObject.cardSelectionDict["output"]));
        int idx = inputInstanceList.Count - 1; // tail of updated list

        // note that we are skipping CardDescriptionSetup(); this is only necessary in the first instance generated in Start()
        int minInstanceID = AvailableMinInstanceID();

        inputInstanceList[idx].CardStatementSetup(
            ref environmentObject, ref inputProps, ref inputStatementFieldGroup, minInstanceID);
        outputInstanceList[idx].CardStatementSetup(
            ref environmentObject, ref outputProps, ref outputStatementFieldGroup, minInstanceID);

        // 矢印 => インスタンスに付随する必要はない。instance.Countに応じて、個数と位置を調整する
        AddIOArrow(); // must call this before "DepriveFocusFromOtherAndGrantToTargetInstances(idx);"
        // todo eliminate these kind of dependencies on sequences

        //GrantFocusToTargetInstances(idx);
        //DepriveFocusFromOtherInstances(idx);
        DepriveFocusFromOtherAndGrantToTargetInstances(idx);

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

        ShiftStatementFields(); // 他人を押し上げる

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

    


    // TODO 本当にこれで良いのかがわからない。つまり、idxの渡し方が
    // todo IsFocusedなものとそうでないものとで色の濃淡を変化させる
    // todo on click でfocusが移り変わる機能の実装
    // recieves the instance-ID of the clicked statement-box
    private void StatementFieldOnClick(int instanceID)
    {
        int idx = GetInstanceListIndexFromInstanceID(instanceID);
        //GrantFocusToTargetInstances(idx);
        //DepriveFocusFromOtherInstances(idx);
        DepriveFocusFromOtherAndGrantToTargetInstances(idx);
    }

    private void ShiftStatementFields()
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


    // AddInstanceとは異なり、focuseされている要素をremoveする
    private void RemoveInstanceFromList() // removeボタンが押せるということは、removeしていい、ということ
    {
        for (int i = 0; i < inputInstanceList.Count; i++)
        {
            if (inputInstanceList[i].IsFocused) // <- 消す対象
            {
                inputInstanceList.RemoveAt(i);
                outputInstanceList.RemoveAt(i);
                break;
            }
        }
        GrantFocusToTargetInstances(inputInstanceList.Count - 1); // focus on tail instance

        ShiftStatementFields();
        ReduceIOArrow();

        if (inputInstanceList.Count <= 1)
            removeInstanceButton.interactable = false;
    }

    // ずらし方がIOinstanceとは異なる。ここでは、新しいのが上に積み上げられる
    private void AddIOArrow()
    {
        int n = ioArrowList.Count;
        GameObject newIOArrow = Instantiate(ioArrowList[n - 1], ioArrowList[n - 1].transform.parent);
        newIOArrow.transform.localPosition += new Vector3(0, VSHAMT, 0);
        ioArrowList.Add(newIOArrow);
    }

    // reduceしてはいけない時には、すでに呼び出せないようになっている
    private void ReduceIOArrow()
    {
        ioArrowList.RemoveAt(ioArrowList.Count-1);
    }


    // 呼び出す側で xputInstanceList.IndexOf(xxx)（特にbuttonがpressされた時）
    private void DepriveFocusFromOtherInstances(int focusedIdx)
    {
        int instanceCount = inputInstanceList.Count;
        for (int i = 0; i < instanceCount; i++)
        {
            if (i == focusedIdx) continue;
            else
            {
                inputInstanceList[i].IsFocused = false;
                outputInstanceList[i].IsFocused = false;
                // statement (is button) の色を変える
                inputInstanceList[focusedIdx].StatementFieldGroup.GetComponent<Image>().color = LIGHT_BEIGE;
                outputInstanceList[focusedIdx].StatementFieldGroup.GetComponent<Image>().color = LIGHT_BEIGE;
                ioArrowList[focusedIdx].GetComponent<Image>().color = LIGHT_WHITE;
            }
        }
    }

    private void GrantFocusToTargetInstances(int targetIdx)
    {
        inputInstanceList[targetIdx].IsFocused = true;
        outputInstanceList[targetIdx].IsFocused = true;
        // statement (is button) の色を変える
        inputInstanceList[targetIdx].StatementFieldGroup.GetComponent<Image>().color = BEIGE;
        outputInstanceList[targetIdx].StatementFieldGroup.GetComponent<Image>().color = BEIGE;
        ioArrowList[targetIdx].GetComponent<Image>().color = WHITE;
    }

    private void DepriveFocusFromOtherAndGrantToTargetInstances(int targetIdx)
    {
        // NOTE: focus transition order must be "deprivae from other THEN focus on target" for some specific card types
        int instanceCount = inputInstanceList.Count;
        for (int i = 0; i < instanceCount; i++)
        {
            if (i == targetIdx) continue;
            inputInstanceList[i].IsFocused = false;
            outputInstanceList[i].IsFocused = false;
            inputInstanceList[i].StatementFieldGroup.GetComponent<Image>().color = LIGHT_BEIGE;
            outputInstanceList[i].StatementFieldGroup.GetComponent<Image>().color = LIGHT_BEIGE;
            ioArrowList[i].GetComponent<Image>().color = LIGHT_WHITE;
        }
        inputInstanceList[targetIdx].IsFocused = true;
        outputInstanceList[targetIdx].IsFocused = true;
        inputInstanceList[targetIdx].StatementFieldGroup.GetComponent<Image>().color = BEIGE;
        outputInstanceList[targetIdx].StatementFieldGroup.GetComponent<Image>().color = BEIGE;
        ioArrowList[targetIdx].GetComponent<Image>().color = WHITE;
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

        ChangeStatementFieldButtonInteractability(false);
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

        ChangeStatementFieldButtonInteractability(true);
        ChangeUnfocusedStatementFieldColors(LIGHT_BEIGE);
        ChangeUnfocusedArrowColors(LIGHT_WHITE);
    }

    private void ChangeStatementFieldButtonInteractability(bool tf)
    {
        int instanceCount = inputInstanceList.Count;
        for (int i = 0; i < instanceCount; i++)
        {
            inputInstanceList[i].StatementFieldGroup.
                GetComponent<Button>().interactable = tf;
            outputInstanceList[i].StatementFieldGroup.
                GetComponent<Button>().interactable = tf;
        }
    }

    private void ChangeUnfocusedStatementFieldColors(Color color)
    {
        int instanceCount = inputInstanceList.Count;
        for (int i = 0; i < instanceCount; i++)
        {
            if (inputInstanceList[i].IsFocused) continue;
            else
            {
                inputInstanceList[i].StatementFieldGroup.
                    GetComponent<Image>().color = color;
                outputInstanceList[i].StatementFieldGroup.
                    GetComponent<Image>().color = color;
            }
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
        return -1; // todo may want to make it throw error
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

/*

private void DepriveFocusFromOthers(InputCard inInstance, OutputCard outInstance)
{
    int focusedIdx = inputInstanceList.IndexOf(inInstance);
    // index should be identical for input and output instance list
    if (focusedIdx != outputInstanceList.IndexOf(outInstance))
    {
        Debug.Log("Warning: Indexes of Input and Output instance list are not aligned.");
        // some error handling here
    }
    int instanceCount = inputInstanceList.Count;
    for (int i = 0; i < instanceCount; i++)
    {
        if (i == focusedIdx) continue;
        else
        {
            inputInstanceList[i].IsFocused = false;
            outputInstanceList[i].IsFocused = false;
        }
    }
}

*/