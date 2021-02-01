using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public abstract class Card
{
    protected TextMeshProUGUI cardNameField;
    //protected TextMeshProUGUI cardNameFieldTMP;
    protected TextMeshProUGUI descriptionField;
    //protected TextMeshProUGUI descriptionFieldTMP;
    // todo 変数で対応できないかの確認

    protected GameObject environmentObject;
    protected GameObject propObjects;
    protected GameObject statementFieldGroup;
    public GameObject StatementFieldGroup { set { } get { return statementFieldGroup; } } // todo is this a return by reference??
    protected TextMeshProUGUI indexTextTMP;
    protected TextMeshProUGUI contentTextTMP;
    protected TextMeshProUGUI variableTextTMP;

    protected string cardName;
    protected string descriptionText;
    protected string contentText;
    private int instanceID = -1;
    public int InstanceID { set { } get { return instanceID; } }

    protected abstract void OnConfirm();
    protected abstract void OnBackToEdit();
    protected bool isConfirmed = false;
    public bool IsConfirmed {
        set {
            isConfirmed = value;
            if (isConfirmed) OnConfirm(); // todo もしpropsが参照なら、OnConfirmをInputCard Classで定義して、その中で　InputConditionDefinition = DetermineInputEvaluationDelegate();　を呼ぶことができる。
            else OnBackToEdit();
        }
        get {
            return isConfirmed;
        }
    }

    protected abstract void OnFocusGranted();
    protected abstract void OnFocusDeprived();
    protected bool isFocused; // todo この状態に応じて操作を許可・不許可にする // この状態がfalseのインスタンス（自分）については、反映されないようにする
    public bool IsFocused {
        set {
            isFocused = value;
            if (isFocused) OnFocusGranted();
            else OnFocusDeprived();
        }
        get {
            return isFocused;
        }
    }

    protected bool canBeConfirmed = false; // Coreの処理として、全てのインスタンスでこれがtrueならConfirmできる
    public bool CanBeConfirmed { set { } get { return canBeConfirmed; } }

    protected abstract void BehaviourDuringPrototyping();

    protected abstract void InitPropFields();

    private string getIndexText() {
        return "<u>" + getIndexSubText() + " #" + (instanceID + 1).ToString() + "</u>";
    }
    protected abstract string getIndexSubText();

    // only necessary for the first card instance
    public void CardDescriptionSetup(
        ref TextMeshProUGUI cardNameField,
        ref TextMeshProUGUI descriptionField) {
        this.cardNameField = cardNameField;
        this.descriptionField = descriptionField;
        cardNameField.SetText(cardName);
        descriptionField.SetText(descriptionText);
    }

    public void CardStatementSetup(
        ref GameObject environmentObject,
        ref GameObject propObjects,
        ref GameObject statementFieldGroup,
        int instanceID)
    {
        this.instanceID = instanceID; // substitute before setting indexText

        this.environmentObject = environmentObject;
        this.environmentObject.SetActive(true);

        this.propObjects = propObjects;
        InitPropFields();
        this.propObjects.SetActive(true);

        // TODO 位置の調整；自分だけじゃなくて、他人の位置を操作しないといけない。（というか、自分の位置はオリジナルと同じ）
        // => 自分は動かないから、addInstanceが押された時に、他の奴らを動かせばいい。
        this.statementFieldGroup = Object.Instantiate(
            statementFieldGroup, statementFieldGroup.transform.parent);
        //this.statementFieldGroup = Object.Instantiate(
        //    statementFieldGroup, statementFieldGroup.transform.position,
        //    Quaternion.identity, statementFieldGroup.transform);
        // => indicating that the transform is identical to the original
        indexTextTMP = this.statementFieldGroup.transform.Find("IndexText").gameObject.GetComponent<TextMeshProUGUI>();
        indexTextTMP.SetText(getIndexText());
        contentTextTMP = this.statementFieldGroup.transform.Find("ContentText").gameObject.GetComponent<TextMeshProUGUI>();
        contentTextTMP.SetText(contentText);
        variableTextTMP = this.statementFieldGroup.transform.Find("Variable/VariableText").gameObject.GetComponent<TextMeshProUGUI>();
        variableTextTMP.SetText(string.Empty);
        // variableTextTMP is set dynamically
        // 直接activateするのではなく、
        // instantiateして複製してから、それをactivateする (after adjustening transform.position.y)
        // need to adjust transform.position when PrototypingSceneCore.instIdx >= 1
        // must instantiate() first
        // something like "transform.position.y += xxx*i" ??
        this.statementFieldGroup.SetActive(true);
    }


}


public abstract class InputCard : Card
{
    protected int maxInstanceNum;
    public int MaxInstanceNum { set { } get { return maxInstanceNum; } }

    public delegate bool InputEvaluationDelegate();
    public Dictionary<string, InputEvaluationDelegate> inputEvalDeleDict;
    protected string conditionKeyword = string.Empty;
    public string ConditionKeyword {
        set { conditionKeyword = value; variableTextTMP.SetText(conditionKeyword); }
        get { return conditionKeyword; } }
    public readonly string ALREADY_EXISTS = "already exists";

    [HideInInspector] public bool inputCondition = false;

    public void UpdateInputCondition()
    {
        if (isConfirmed) {
            inputCondition = inputEvalDeleDict[conditionKeyword]();
            // Confirm後のテスト中は常時判定する必要があるからここに置いている
        } else {
            BehaviourDuringPrototyping();
            canBeConfirmed = !(string.IsNullOrEmpty(conditionKeyword) || conditionKeyword == ALREADY_EXISTS);
        }
    }

    protected override string getIndexSubText() {return "input";}

}


// todo リファクタの余地あり
public abstract class OutputCard : Card
{
    public abstract void OutputBehaviour();
    public abstract void OutputBehaviourNegative();

    public void UpdateOutputBehaviour()
    {
        if (isConfirmed)
        {
            return;
            // no actions are needed (as for current implemented cards)
        }
        else
        {
            BehaviourDuringPrototyping();
            canBeConfirmed = !string.IsNullOrEmpty(variableTextTMP.text);
        }
    }

    protected override string getIndexSubText() {return "output";}
}