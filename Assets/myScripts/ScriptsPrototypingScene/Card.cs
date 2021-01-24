using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : MonoBehaviour
{
    protected GameObject environmentObject;

    // パネルへの表示の仕方はinput・outputに共通か
}


public abstract class InputCard : Card
{
    public bool inputCondition;
    protected bool isConfirmed; // movable to Card-Class??
    protected delegate bool InputConditionDelegate();
    protected InputConditionDelegate InputConditionDefintion;

    public void ConfirmInputCondition() {
        isConfirmed = true;
        InputConditionDefintion = SetToDelegate();
    }

    public abstract void SetInputCondition(ref GameObject envObj); //ここで渡すべきobjectを渡す
    public abstract void UpdateInputCondition();
    protected abstract InputConditionDelegate SetToDelegate();
}


public abstract class OutputCard : Card
{
    public abstract void SetOutputBehaviour();
    public abstract void ConfirmOutputBehaviour();
    public abstract void UpdateOutputBehaviour();
    public abstract void OutputBehaviour();
    //なんらかの形で鍵をかける必要があるか(e.g. 録音中に再生が始まらないように)

}