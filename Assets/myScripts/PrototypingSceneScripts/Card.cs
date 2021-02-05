﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public abstract class Card
{
    protected GameObject environmentObject;
    protected GameObject propObjects;
    protected GameObject statementFieldGroup;
    public GameObject StatementFieldGroup {
        set { } get { return statementFieldGroup; }}
    protected TextMeshProUGUI cardNameField;
    protected TextMeshProUGUI descriptionField;
    protected TextMeshProUGUI indexTextTMP;
    protected TextMeshProUGUI contentTextTMP;
    protected TextMeshProUGUI variableTextTMP;

    protected string cardName;
    protected string descriptionText;
    protected string contentText;
    private int instanceID = -1;
    public int InstanceID {
        set { } get { return instanceID; } }

    protected bool isConfirmed = false;
    public bool IsConfirmed {
        set { isConfirmed = value;
            if (isConfirmed) OnConfirm();
            else OnBackToEdit(); }
        get { return isConfirmed; }}

    protected bool isFocused;
    public bool IsFocused {
        set { isFocused = value; // first set to new focus-state
            if (isFocused) OnFocusGranted(); // then call methods
            else OnFocusDeprived(); }
        get { return isFocused; }}

    protected bool canBeConfirmed = false;
    public abstract bool CanBeConfirmed { get; }

    protected string conditionKeyword = string.Empty;
    public string ConditionKeyword {
        set { conditionKeyword = value;
            variableTextTMP.SetText(conditionKeyword); }
        get { return conditionKeyword; }}

    protected abstract void InitPropFields();
    public abstract void BehaviourDuringPrototyping();

    protected abstract void OnConfirm();
    protected abstract void OnBackToEdit();
    protected abstract void OnFocusGranted();
    protected abstract void OnFocusDeprived();

    protected abstract string GetCardType();
    // index text string is used for determining
    // which statement-field-button was selected
    private string GetIndexText() {
        return "<u>" + GetCardType() + " #"
            + (instanceID + 1).ToString() + "</u>";}

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
        int instanceID) {
        this.instanceID = instanceID; // assign before setting indexText
        this.environmentObject = environmentObject;
        this.environmentObject.SetActive(true);
        this.propObjects = propObjects;
        InitPropFields();
        this.propObjects.SetActive(true);
        this.statementFieldGroup = Object.Instantiate(
            statementFieldGroup, statementFieldGroup.transform.parent);
        indexTextTMP = this.statementFieldGroup.transform.
            Find("IndexText").gameObject.GetComponent<TextMeshProUGUI>();
        indexTextTMP.SetText(GetIndexText());
        contentTextTMP = this.statementFieldGroup.transform.
            Find("ContentText").gameObject.GetComponent<TextMeshProUGUI>();
        contentTextTMP.SetText(contentText);
        variableTextTMP = this.statementFieldGroup.transform.
            Find("Variable/VariableText").gameObject.GetComponent<TextMeshProUGUI>();
        variableTextTMP.SetText(string.Empty);
        this.statementFieldGroup.SetActive(true);
    }

}



public abstract class InputCard : Card
{
    protected override string GetCardType() { return "input"; }

    protected int maxInstanceNum;
    public int MaxInstanceNum { set { } get { return maxInstanceNum; } }

    // set "public" to enable exporting SmartObject to the next scene
    public delegate bool InputEvaluationDelegate();
    public Dictionary<string, InputEvaluationDelegate> inputEvalDeleDict;

    public readonly string ALREADY_EXISTS = "<color=\"red\">already exists</color>";

    // positive-negative-TriggerFlag:
    // flags that loses it's "true" state when accessed from other class
    private bool positiveTriggerFlag = false;
    public bool PositiveTriggerFlag {
        get {
            if (positiveTriggerFlag) {
                positiveTriggerFlag = false;
                return true;
            } else return false;
        }}

    private bool negativeTriggerFlag = false;
    public bool NegativeTriggerFlag {
        get {
            if (negativeTriggerFlag) {
                negativeTriggerFlag = false;
                return true;
            } else return false;
        }}

    private bool currentEval = false;

    public void UpdateInputCondition()
    {
        bool newEval = inputEvalDeleDict[conditionKeyword]();
        if (!currentEval && newEval) positiveTriggerFlag = true;
        else if (currentEval && !newEval) negativeTriggerFlag = true;
        if (currentEval != newEval) currentEval = newEval;
    }

    public override bool CanBeConfirmed
    {
        get { canBeConfirmed = !(string.IsNullOrEmpty(conditionKeyword)
                || conditionKeyword == ALREADY_EXISTS);
            return canBeConfirmed;
        }
    }

}



public abstract class OutputCard : Card
{
    protected override string GetCardType() { return "output"; }

    public override bool CanBeConfirmed { get {
            canBeConfirmed = !string.IsNullOrEmpty(variableTextTMP.text);
            return canBeConfirmed; }}

    public abstract void OutputBehaviourOnPositive();
    public abstract void OutputBehaviourOnNegative();
}