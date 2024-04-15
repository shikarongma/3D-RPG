using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatBar : MonoBehaviour
{
    public Image healthfillRed;
    public Image healthfillGreen;
    public Image powerYellow;

    private void Update()
    {
        if (healthfillRed.fillAmount > healthfillGreen.fillAmount)
        {
            healthfillRed.fillAmount -= Time.deltaTime;
        }
    }

    public void SetHealthGreen(float health)
    {
        healthfillGreen.fillAmount = health;
    }
}
