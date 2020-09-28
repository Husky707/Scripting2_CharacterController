using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Player : MonoBehaviour
{

    [SerializeField] private CharacterController player = null;
    [SerializeField] private Image meter = null;
    int maxHealth;


    private void OnEnable()
    {
        player.playerChangedHealth += OnHealthChange;
        maxHealth = player.maxHealth;
    }

    private void OnDisable()
    {
        player.playerChangedHealth -= OnHealthChange;
    }



    private void OnHealthChange(int delta, int prev)
    {
        int newHealth = prev + delta;
        meter.fillAmount = (float)newHealth / (float)maxHealth;
    }

}
