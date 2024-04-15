using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public CharacterEventSO healthEvent;
    public PlayerStatBar playerStatBar;

    private void OnEnable()
    {
        healthEvent.OnEventRaised += OnHealthEvent;
    }

    private void OnDisable()
    {
        healthEvent.OnEventRaised -= OnHealthEvent;
    }

    private void OnHealthEvent(CharacterStates character)
    {
        var persentage = (float)character.characterData.currentHealth / character.characterData.maxHealth;
        playerStatBar.SetHealthGreen(persentage);
    }
}
