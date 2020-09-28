using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnergyCore : MonoBehaviour
{

    public UnityEvent EnergyGained;
    public UnityEvent EnergyRecharged;
    public UnityEvent EnergyLost;
    public UnityEvent OutOfEnergy;

    [Header("Settings")]
    [SerializeField] private int maxEnergy = 5;
    [SerializeField] private float rechargeRate = 5f;

    [Header("UI")]
    [SerializeField] GameObject uiObj = null;

    public int currentEnergy { get { return _currentEnergy; } private set { EnergyLevelChanged(value); } }
    private int _currentEnergy;

    private bool atMax = false;
    float ticRecharge = 0f;

    private void Start()
    {
        _currentEnergy = maxEnergy;
    }


    private void Update()
    {
        if(!atMax)
        {
            ticRecharge += Time.deltaTime;
            if(ticRecharge >= rechargeRate)
            {
                ticRecharge = 0f;
                RechargeEnergy();
            }
        }
    }

    public void DecreaseEnergy()
    {
        currentEnergy--;
    }

    public void IncrementEnergy()
    {
        currentEnergy++;
    }

    private void EnergyLevelChanged(int newVal)
    {
        if (_currentEnergy == newVal)
            return;

        int delta = newVal - _currentEnergy;
        _currentEnergy = newVal;

        if (_currentEnergy >= maxEnergy)
        {
            _currentEnergy = maxEnergy;
            atMax = true;
        }
        else if( _currentEnergy <= 0)
        {
            _currentEnergy = 0;
            OutOfEnergy.Invoke();
        }
        else
        {
            atMax = false;
        }

        if (delta < 0)
            EnergyLost.Invoke();
        else
            EnergyGained.Invoke();

        UpdateUI();
    }

    private void RechargeEnergy()
    {
        currentEnergy++;
        EnergyRecharged.Invoke();
    }

    private void UpdateUI()
    {

    }
}
