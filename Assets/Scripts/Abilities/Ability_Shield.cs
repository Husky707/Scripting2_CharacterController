using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ability_Shield : Ability
{
    public UnityEvent AbilityActivating;
    public UnityEvent AbilityActivated;
    public UnityEvent AbilityEneded;
    public UnityEvent AbilityReady;
    public UnityEvent CouldNotUse;

    [Header("Settings")]
    [SerializeField] private float activeTime = 4f;
    [SerializeField] private float timeToActivate = 0.5f;
    [SerializeField] private float cooldownTime = 4f;

    [Header("Audio")]
    [SerializeField] private AudioClip activeSound = null;
    [SerializeField] private AudioClip activateSound = null;
    [SerializeField] private AudioClip deactivateSound = null;

    GameObject shield = null;
    CharacterController player = null;
    EnergyCore energyCore = null;
    AudioSource audio = null;

    public override bool isActivating { get { return _acting; } set { _acting = value; if(loadout!=null) loadout.abilityActivating = value; } }
    public override bool isActive { get { return _active; } set { _active = value; if (loadout != null) loadout.abilityActive = value; } }
    public override bool isReady { get { return _ready; } set { _ready = value; if (loadout != null) loadout.abilityReady = value; } }
    private bool _active = false;
    private bool _acting = false;
    private bool _ready = true;
    private void Awake()
    {
        abilityType = eAbilities.Shield;
        AbilityHub hub = FindObjectOfType<AbilityHub>();
        player = FindObjectOfType<CharacterController>();
        shield = hub.GetTarget(abilityType);
        energyCore = FindObjectOfType<EnergyCore>();
        if (shield == null)
            Debug.Log("Couldn't find shield object");
        audio = shield.GetComponent<AudioSource>();
        shield = shield.transform.GetChild(0).gameObject;

    }

    float durationTic = 0f;
    float activationTic = 0f;
    float cooldownTic = 0f;
    private void Update()
    {

        if(isActivating)
        {
            activationTic += Time.deltaTime;
            if(activationTic >= timeToActivate)
            {
                activationTic = 0f;
                isActivating = false;
                ActivateAbility();
            }
        }

        if(isActive)
        {
            durationTic += Time.deltaTime;
            if(durationTic >= activeTime)
            {
                durationTic = 0f;
                DeactivateAbility();
            }
            else
            {
                //UpdateAbility();
            }
        }

        if(!isReady)
        {
            cooldownTic += Time.deltaTime;
            if(cooldownTic >= cooldownTime)
            {
                cooldownTic = 0f;
                ResetCooldown();
            }
        }
    }
    public override void Use(Transform origin, Transform target)
    {
        if(CanUse())
        {
            AbilityActivating.Invoke();
            isActivating = true;
            isReady = false;

            audio.clip = activateSound;
            audio.loop = false;
            //audio.Play();
        }
        else
            CouldNotUse.Invoke();
    }

    private void ResetCooldown()
    {
        cooldownTic = 0f;
        isReady = true;
        AbilityReady.Invoke();
    }

    private void ActivateAbility()
    {
        if (isActive)
            return;

        isActive = true;
        shield.SetActive(true);
        energyCore.DecreaseEnergy();

        audio.clip = activateSound;
        //audio.loop = true;
        //audio.PlayDelayed(0.4f);
        audio.Play();
    }

    private void DeactivateAbility()
    {
        if (!isActive)
            return;

        durationTic = 0f;
        isActive = false;
        shield.SetActive(false);

        audio.clip = deactivateSound;
        audio.loop = false;
        audio.Play();
    }

    public override void Stop()
    {
        shield.SetActive(false);
        isActive = false;
        isActivating = false;
        activationTic = 0f;
        durationTic = 0f;

        if(cooldownTic < cooldownTime - 0.6f)
            cooldownTic = cooldownTime - 0.6f;
    }


    private bool CanUse()
    {
        if (!isReady || isActivating || isActive)
            return false;

        if (energyCore.currentEnergy <= 0)
            return false;

        return true;
    }

    public override void LinkEvents(AbilityLoadout listener)
    {
        loadout = listener;
    }
}
