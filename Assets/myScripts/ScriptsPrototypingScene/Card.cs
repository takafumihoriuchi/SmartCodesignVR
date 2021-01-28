using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public abstract class Card
{
    protected GameObject cardNameField;
    protected TextMeshProUGUI cardNameFieldTMP;
    protected GameObject descriptionField;
    protected TextMeshProUGUI descriptionFieldTMP;
    // todo 変数で対応できないかの確認

    protected GameObject environmentObject;
    protected GameObject propObjects;
    protected GameObject statementFieldGroup;
    protected TextMeshProUGUI indexTextTMP;
    protected TextMeshProUGUI contentTextTMP;
    protected TextMeshProUGUI variableTextTMP;

    protected string cardName;
    protected string descriptionText;
    protected string contentText;
    protected abstract string getIndexText(int instanceIdx);

    protected bool isConfirmed;

    protected abstract void BehaviourDuringPrototyping();

    protected abstract void InitPropFields();


    public void CardDescriptionSetup(
        ref GameObject cardNameField,
        ref GameObject descriptionField)
    {
        this.cardNameField = cardNameField;
        cardNameFieldTMP = this.cardNameField.GetComponent<TextMeshProUGUI>();
        cardNameFieldTMP.SetText(cardName);
        this.cardNameField.SetActive(true);

        this.descriptionField = descriptionField;
        descriptionFieldTMP = this.descriptionField.GetComponent<TextMeshProUGUI>();
        descriptionFieldTMP.SetText(descriptionText);
        this.descriptionField.SetActive(true);
    }

    public void CardStatementSetup(
        ref GameObject environmentObject,
        ref GameObject propObjects,
        GameObject statementFieldGroup,
        int instanceIdx)
    {
        this.environmentObject = environmentObject;
        this.environmentObject.SetActive(true);

        this.propObjects = propObjects;
        InitPropFields();
        this.propObjects.SetActive(true);

        this.statementFieldGroup = statementFieldGroup;
        indexTextTMP = this.statementFieldGroup.transform.Find("IndexText").gameObject.GetComponent<TextMeshProUGUI>();
        indexTextTMP.SetText(getIndexText(instanceIdx));
        contentTextTMP = this.statementFieldGroup.transform.Find("ContentText").gameObject.GetComponent<TextMeshProUGUI>();
        contentTextTMP.SetText(contentText);
        variableTextTMP = this.statementFieldGroup.transform.Find("Variable/VariableText").gameObject.GetComponent<TextMeshProUGUI>();
        // variableTextTMP is set dynamically
        this.statementFieldGroup.SetActive(true);
        // todo need to adjust transform.position when PrototypingSceneCore.instIdx >= 1
        // must instantiate() first
        // something like "transform.position.y += xxx*i" ??
    }


}


public abstract class InputCard : Card
{
    protected int maxInstanceNum;

    [HideInInspector] public bool inputCondition = false;

    protected delegate bool InputConditionDelegate();

    protected InputConditionDelegate InputConditionDefinition;

    protected abstract InputConditionDelegate DetermineInputEvaluationDelegate();

    public void ConfirmInputCondition() {
        isConfirmed = true;
        InputConditionDefinition = DetermineInputEvaluationDelegate();
    }

    public void UpdateInputCondition() {
        UpdatesForInputConditionEvaluation();
        if (isConfirmed) {
            inputCondition = InputConditionDefinition();
        }
        else BehaviourDuringPrototyping();
    }

    protected abstract void UpdatesForInputConditionEvaluation();

    protected override string getIndexText(int instanceIdx) {
        return "<u>input #" + instanceIdx.ToString() + "</u>";
    }

}


public abstract class OutputCard : Card
{
    public abstract void OutputBehaviour();
    public abstract void UpdateOutputBehaviour();
    public abstract void ConfirmOutputBehaviour();
    public abstract void OutputBehaviourNegative();

    protected override string getIndexText(int instanceIdx) {
        return "<u>output #" + instanceIdx.ToString() + "</u>";
    }
}
