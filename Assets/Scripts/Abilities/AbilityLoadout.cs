using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum eAbilityState {Ready }
public class AbilityLoadout : MonoBehaviour
{
    public event Action<bool> ActiveStateChange = delegate { };
    public event Action<bool> ReadyStateChange = delegate { };
    public event Action<bool> ActivatingStateChange = delegate { };

    public Ability equippedAbility { get; private set;}
    public bool abilityReady { get { return _ready; }  set { _ready = value; ReadyStateChange.Invoke(value); } }
    public bool abilityActive { get { return _active; }  set { _active = value; ActiveStateChange.Invoke(value); } }
    public bool abilityActivating { get { return _activating; }  set { _activating = value; ActivatingStateChange.Invoke(value); } }
    private bool _ready = true;
    private bool _active = false;
    private bool _activating = false;

    public void EquipAbility(Ability newAbility)
    {
        RemoveCurrentAbility();
        CreateNewAbilityObject(newAbility);
        abilityActive = false;
        abilityReady = true;
        abilityActivating = false;
    }

    public void UseAbility(Transform origin, Transform target)
    {
        equippedAbility?.Use(origin, target);
    }

    public void StopAbility()
    {
        equippedAbility?.Stop();
    }

    private void RemoveCurrentAbility()
    {
        StopAbility();
        foreach(Transform child in this.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    private void CreateNewAbilityObject(Ability newAbility)
    {
        equippedAbility = Instantiate(newAbility, transform.position, Quaternion.identity);
        equippedAbility.transform.SetParent(this.transform);
        equippedAbility.LinkEvents(this);
    }
       
}
