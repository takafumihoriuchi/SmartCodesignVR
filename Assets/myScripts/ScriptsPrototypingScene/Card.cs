using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : MonoBehaviour
{
    
    // パネルへの表示の仕方はinput・outputに共通か
}


public abstract class InputCard : Card
{

    public bool inputCondition;

}


public abstract class OutputCard : Card
{

    public abstract void OutputBehaviour();

}