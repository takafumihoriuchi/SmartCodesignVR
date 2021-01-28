using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public abstract class Card
{
    protected GameObject environmentObject;
    protected GameObject cardNameField;
    protected TextMeshProUGUI cardNameFieldTMP;
    protected GameObject descriptionField;
    protected TextMeshProUGUI descriptionFieldTMP;

    protected GameObject statementBox;
    protected TextMeshProUGUI statementTMP;
    protected GameObject propObjects;

    protected bool isConfirmed;

    protected abstract void BehaviourDuringPrototyping();

    protected abstract string GetCardName();
    protected abstract string SetDescriptionField();
    protected abstract string InitContentText();
    protected abstract void InitPropFields();

    public void CardDescriptionSetup(
        ref GameObject cardNameField,
        ref GameObject descriptionField,
        ref GameObject diagramImage)
    {
        this.cardNameField = cardNameField;
        cardNameFieldTMP = this.cardNameField.GetComponent<TextMeshProUGUI>();
        cardNameFieldTMP.SetText(GetCardName());
        this.cardNameField.SetActive(true);

        this.descriptionField = descriptionField;
        descriptionFieldTMP = this.descriptionField.GetComponent<TextMeshProUGUI>();
        descriptionFieldTMP.SetText(SetDescriptionField());
        this.descriptionField.SetActive(true);


    }

    public void CardStatementSetup(
        ref GameObject environmentObject,
        ref GameObject propObjects,
        GameObject statementBox)
    {
        this.environmentObject = environmentObject;
        this.environmentObject.SetActive(true);

        this.statementBox = statementBox;
        statementTMP = this.statementBox.transform.Find("ContentText")
            .gameObject.GetComponent<TextMeshProUGUI>();
        statementTMP.SetText(InitContentText());
        this.statementBox.SetActive(true);
        // need to adjust transform.position when PrototypingSceneCore.instIdx >= 1

        this.propObjects = propObjects; // call after the setting of envObject
        InitPropFields();
        this.propObjects.SetActive(true);
        // inProps is passed by value -> a copy is already made
        // todo SetActivate(true)がないとHiddenのままか；これでコピーが作られているかどうかを確認する
        // コピーは作られていないような感じ。要詳細調査。
    }


}


public abstract class InputCard : Card
{
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
