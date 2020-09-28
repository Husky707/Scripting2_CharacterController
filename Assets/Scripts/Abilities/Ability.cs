using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eAbilities { Shield, ShieldDome };
public abstract class Ability : MonoBehaviour
{
    public  eAbilities abilityType { get; set; }
    public  AbilityLoadout loadout { get; set; }

    public abstract bool isActivating { get; set; }
    public abstract bool isActive { get; set; }
    public abstract bool isReady { get; set; }
    public abstract void Use(Transform origin, Transform target);
    public abstract void Stop();

    public abstract void LinkEvents(AbilityLoadout listener);
}
