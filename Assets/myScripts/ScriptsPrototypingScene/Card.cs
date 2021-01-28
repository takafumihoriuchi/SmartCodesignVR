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

    protected GameObject environmentObject;
    protected GameObject propObjects;
    protected GameObject statementFieldGroup;
    protected TextMeshProUGUI statementTMP;

    protected string cardName;
    protected bool isConfirmed;

    protected abstract void BehaviourDuringPrototyping();

    //protected abstract string GetCardName();
    protected abstract string SetDescriptionField();

    protected abstract void InitPropFields();
    //protected abstract string InitContentText();

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
        descriptionFieldTMP.SetText(SetDescriptionField());
        this.descriptionField.SetActive(true);
    }

    public void CardStatementSetup(
        ref GameObject environmentObject,
        ref GameObject propObjects,
        GameObject statementFieldGroup)
    {
        this.environmentObject = environmentObject;
        this.environmentObject.SetActive(true);

        this.propObjects = propObjects;
        InitPropFields();
        this.propObjects.SetActive(true);

        this.statementFieldGroup = statementFieldGroup;
        // instance number # () / index text
        // statement content text
        // statement variable text () <- これが今までの"ContentText"に対応する
        //statementTMP = this.statementFieldGroup.transform.Find("ContentText")
        //    .gameObject.GetComponent<TextMeshProUGUI>();
        //statementTMP.SetText(InitContentText());
        this.statementFieldGroup.SetActive(true);
        // todo need to adjust transform.position when PrototypingSceneCore.instIdx >= 1

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
}


public abstract class OutputCard : Card
{
    public abstract void OutputBehaviour();
    public abstract void UpdateOutputBehaviour();
    public abstract void ConfirmOutputBehaviour();
    public abstract void OutputBehaviourNegative();
}
