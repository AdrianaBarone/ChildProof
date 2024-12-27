using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickable : MonoBehaviour
{
    public string promptMessage;

    public void BasePick()
    {
        Pick();
    }

    protected virtual void Pick()
    {
        //Funzione vuota che sarà sovrascritta di volta in volta dalle sottoclassi
    }
}
