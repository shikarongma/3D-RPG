using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName ="Event/CharcterEventSO")]

public class CharacterEventSO : ScriptableObject
{
    public UnityAction<CharacterStates> OnEventRaised;

    public void RaisedEvent(CharacterStates characterStates)
    {
        OnEventRaised?.Invoke(characterStates);
    }
}
