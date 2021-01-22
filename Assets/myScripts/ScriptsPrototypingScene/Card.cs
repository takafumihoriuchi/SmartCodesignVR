using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : MonoBehaviour
{



}


public abstract class InputCard : Card
{

    public bool inputCondition;

}


public abstract class OutputCard : Card
{

    public abstract void OutputBehaviour();


}
