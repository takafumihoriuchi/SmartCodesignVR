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
    public abstract void SetInputCondition(ref GameObject environmentObject);
    public abstract void UpdateInputCondition();
}


public abstract class OutputCard : Card
{

    public abstract void OutputBehaviour();

}