using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public enum eStates { Idle, Move, Sprint, Jump, Fall, Land, Dead };

[RequireComponent(typeof(InputGate))]
public class CharacterController : MonoBehaviour, IHaveHealth
{
    [SerializeField] UnityEvent Airborn;
    [SerializeField] UnityEvent HitGround;

    public event Action<int, int> playerChangedHealth = delegate { };
    public event Action playerDied = delegate { };

    public event Action stateAirborn = delegate { };
    public event Action stateHitGround = delegate { };
    public event Action<bool> stateRunning = delegate { };
    public event Action<bool> stateSprinting = delegate { };
    public event Action stateJumping = delegate { };
    public event Action stateIdle = delegate { };
    public event Action<eAbilities, eStates> stateAbility = delegate { };

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float sprintBoost = 2f;
    [SerializeField] private float sprintSpeed = 2f;
    [SerializeField] private float crouchSpeed = 0.75f;
    [SerializeField] private float airMoveSpeed = 0.5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float launchTime = 0.2f;
    [SerializeField] private float landRecoveryTime = 0.2f;
    [SerializeField] public int maxHealth = 10;
    [SerializeField] private Ability ability = null;
    [SerializeField] private Ability abilityDome = null;

    [Header("")]
    [SerializeField] private Transform moveCam = null;
    [SerializeField] private Collider playerCol = null;
    [SerializeField] private Rigidbody playerRb = null;
    //[SerializeField] private Collider groundCol = null;
    [SerializeField] private CharacterJumper jumper = null;
    [SerializeField] AbilityLoadout abilityLoadout = null;


    InputGate input = null;
    Rigidbody rbody = null;
    Health healthController = null;
    public int health { get => healthController.health; set => healthController.health = value; }

    float timeSinceJump = -1f;
    eActivation sprintMode = eActivation.noone;
    eStates currentState = eStates.Idle;
    bool usingAbility = false;
    bool activatingAbility = false;
    bool isAirborn = false;


    Vector3 frameMovement = Vector3.zero;

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Init
    private void Awake()
    {
        input = GetComponent<InputGate>();
        rbody = GetComponent<Rigidbody>();
        healthController = GetComponent<Health>();
    }

    private void Start()
    {
        stateIdle.Invoke();
        SetHealth(maxHealth);
        abilityLoadout?.EquipAbility(ability);
    }

    private void OnEnable()
    {
        input.Input_Axis += OnMove;
        input.Input_Sprint += OnSprint;
        input.Input_Jump += OnJump;
        input.Input_Ability += OnUseAbility;
        jumper.Event_HitGround += OnLanded;
        jumper.Event_IsAirborn += OnAirborn;

        healthController.Death_Event += OnDeath;
        healthController.HealthChanged_Event += OnHealthChanged;

        abilityLoadout.ActivatingStateChange += OnAbilityActivatingChange;
        abilityLoadout.ActiveStateChange += OnAbilityActiveChange;
    }

    private void OnDisable()
    {
        input.Input_Axis -= OnMove;
        input.Input_Jump -= OnJump;
        input.Input_Sprint -= OnSprint;
        input.Input_Ability -= OnUseAbility;
        jumper.Event_HitGround -= OnLanded;
        jumper.Event_IsAirborn -= OnAirborn;

        healthController.Death_Event -= OnDeath;
        healthController.HealthChanged_Event -= OnHealthChanged;

        abilityLoadout.ActivatingStateChange -= OnAbilityActivatingChange;
        abilityLoadout.ActiveStateChange += OnAbilityActiveChange;


    }
    #endregion

    private void Update()
    {
        if (health <= 0) return;

        if(timeSinceJump >= 0)
        {
            timeSinceJump += Time.deltaTime;
            if (currentState == eStates.Jump)
                OnAirborn();

            if(timeSinceJump >= 10f)
            {
                Debug.Log("Jump duration error");
                currentState = eStates.Idle;
                timeSinceJump = - 1f;
            }
        }

        MovePlayer();

    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Health

    void OnDeath()
    {
        playerCol.enabled = false;
        playerRb.useGravity = false;
        playerRb.isKinematic = true;
        playerDied.Invoke();
        input.isReadingInput = false;
    }

    void OnHealthChanged(int delta, int was)
    {
        playerChangedHealth.Invoke(delta, was);
    }

    public void SetHealth(int setAmount)
    {
        healthController.SetHealth(setAmount);
    }

    public void IncrementHealth(int amount)
    {
        healthController.IncrementHealth(amount);
    }

    public void Kill()
    {
        healthController.Kill();
    }
    #endregion


    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Abilities

    void UseAbility(eActivation actState, int mouseButton)
    {
        if (actState == eActivation.noone)
            return;

        eAbilities currentAbility = abilityLoadout.equippedAbility.abilityType;
        if (mouseButton == 0 && currentAbility == eAbilities.ShieldDome && actState == eActivation.entering)
        {
            if(CanSwapAbility(eAbilities.Shield))
            {
                Debug.Log("Attemtpting to swap ability to " + ability.ToString());
                SwapAbility(ability);
            }
            else
            {
                return;
            }
        }
        else if(mouseButton == 1 && currentAbility == eAbilities.Shield && actState == eActivation.entering)
        {
            if(CanSwapAbility(eAbilities.ShieldDome))
            {
                Debug.Log("Attemtpting to swap ability to " + abilityDome.ToString());
                SwapAbility(abilityDome);
            }
            else
            {
                return;
            }
        }

        if (actState == eActivation.entering)
        {
            if (CanUseAbility(actState, mouseButton))
            {
                abilityLoadout.UseAbility(this.transform, null);
                stateAbility.Invoke(abilityLoadout.equippedAbility.abilityType, currentState);
            }
        }
        else if(actState == eActivation.leaving)
        {
            if (abilityLoadout.equippedAbility.abilityType == eAbilities.Shield)
                StopAbility();
        }
        else//Holding down
        {

        }
    }

    public void SwapAbility(Ability toAbility)
    {
        Debug.Log("Swapped to " + toAbility.ToString());
        abilityLoadout.EquipAbility(toAbility);
    }

    public void StopAbility()
    {

        AnimMatchState();
        abilityLoadout.StopAbility();
    }

    private bool CanSwapAbility(eAbilities toAbility)
    {
        if (abilityLoadout.abilityActive || abilityLoadout.abilityActivating)
            return false;

        return true;
    }

    private bool CanUseAbility(eActivation actState, int mouseButton)
    {
        eAbilities currentAbility = abilityLoadout.equippedAbility.abilityType;
        bool otherChecks = true;
        if (currentState == eStates.Fall || currentState == eStates.Land)
            otherChecks = false;

        if (actState != eActivation.entering)
            otherChecks = false;

        if (abilityLoadout.abilityActivating)
            otherChecks = false;

        if (currentAbility == eAbilities.Shield && mouseButton != 0)
            return false;
        if(currentAbility == eAbilities.ShieldDome && mouseButton != 1)
            return false;

        return abilityLoadout.abilityReady && otherChecks;
    }

    #endregion


    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Movement

    float smoothingRef;
    float prevMag = 0;
    void MovePlayer()
    {

        if (IsMotionless())
            return;

        if(CanMove())
        {
            SetMovementState();
            PerformMovement();
        }
    }

    void SetMovementState()
    {
        eAbilities currentAbility = abilityLoadout.equippedAbility.abilityType;
        if (sprintMode == eActivation.entering)
        {
            currentState = eStates.Sprint;
            if (usingAbility && currentAbility == eAbilities.Shield)
                stateAbility.Invoke(currentAbility, currentState);
            else
                stateSprinting.Invoke(health > (float)maxHealth / 2.0f);
        }
        else if(currentState == eStates.Idle || (currentState == eStates.Sprint && sprintMode == eActivation.noone))
        { 
            currentState = eStates.Move;
            if (usingAbility && currentAbility == eAbilities.Shield)
                stateAbility.Invoke(currentAbility, currentState);
            else
                stateRunning.Invoke(health > (float)maxHealth / 2.0f);
        }
    }

    private void AttemptIdle()
    {
        eAbilities currentAbility = abilityLoadout.equippedAbility.abilityType;
        currentState = eStates.Idle;
        if (usingAbility && currentAbility != eAbilities.ShieldDome)
        {
            stateAbility.Invoke(currentAbility, currentState);
        }
        else
        {
                stateIdle.Invoke();
        }
    }

    private bool IsMotionless()
    {
        if (frameMovement.magnitude <= 0)
        {
            if (prevMag != 0 || currentState != eStates.Idle)
            {
                prevMag = 0;
                if (CanIdle())
                    AttemptIdle();
            }
            prevMag = 0;
            return true;
        }
        return false;
    }

    void PerformMovement()
    {
        Vector3 direction = frameMovement;
        float targetAngle = (Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg) + moveCam.eulerAngles.y;
        float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothingRef, 0.1f);
        transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        //cCon.Move((moveDir.normalized * moveSpeed * Time.deltaTime));

        float speed = GetNetSpeed();

        rbody.MovePosition(transform.position + (moveDir.normalized * speed * Time.deltaTime));
    }

    private bool CanMove()
    {
        if (usingAbility || activatingAbility)
        {
            eAbilities currentAbility = abilityLoadout.equippedAbility.abilityType;
            if (currentAbility == eAbilities.Shield)
                return true;
            else if (currentAbility == eAbilities.ShieldDome && activatingAbility)
                return false;
        }

        return true;
    }

    private bool CanIdle()
    {
        if (currentState == eStates.Fall || currentState == eStates.Jump)
            return false;

        if (usingAbility || activatingAbility)
        {
            eAbilities currentAbility = abilityLoadout.equippedAbility.abilityType;
            if (currentAbility == eAbilities.Shield)
                return true;
            else if (currentAbility == eAbilities.ShieldDome && activatingAbility)
                return false;
        }

        return true;
    }

    private float GetNetSpeed()
    {
        float sprintMod = 0f;
        if (sprintMode == eActivation.entering)
            sprintMod = sprintBoost;
        else if (sprintMode == eActivation.remaining)
            sprintMod = sprintSpeed - moveSpeed;

        float abilityMod = 0f;
        if(usingAbility)
        {
            eAbilities currentAbility = abilityLoadout.equippedAbility.abilityType;
            if (currentAbility == eAbilities.Shield)
                abilityMod = crouchSpeed - moveSpeed;
        }

        float airbornMod = 0f;
        if(currentState == eStates.Fall || currentState == eStates.Jump)
        {
            airbornMod = airMoveSpeed - moveSpeed;
            abilityMod = 0f;
        }

        float speed = moveSpeed + sprintMod + abilityMod + airbornMod;
        return speed;
    }

    #endregion

    private void Sprint(eActivation actState)
    {
        if (actState == eActivation.leaving)
            sprintMode = eActivation.noone;

        else if (actState == eActivation.entering && CanEnterSprint())
            sprintMode = actState;

        else if (actState == eActivation.remaining)
        {
            if (sprintMode == eActivation.entering)
                sprintMode = eActivation.remaining;
        }
    }

    private bool CanEnterSprint()
    {
        if (sprintMode == eActivation.remaining)
            return false;

        if(usingAbility || activatingAbility)
        {
            if (abilityLoadout.equippedAbility.abilityType == eAbilities.ShieldDome)
                return false;
        }

        if (currentState == eStates.Fall || currentState == eStates.Jump)
            return false;

        return true;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Jumping

    private void Jump(eActivation actState)
    {
        if(CanSustainJump(actState))
           rbody.AddForce(Vector3.up * (jumpForce / 10f));
        else if(CanJump())
        {
            currentState = eStates.Jump;
            rbody.AddForce(Vector3.up * jumpForce);
            stateJumping.Invoke();
            timeSinceJump = 0f;
        }
    }

    private bool CanJump()
    {
        if (usingAbility || activatingAbility)
            return false;

        if (currentState == eStates.Jump || currentState == eStates.Fall || currentState == eStates.Land)
            return false;

        return true;
    }

    private bool CanSustainJump(eActivation actState)
    {
        if (actState != eActivation.remaining)
            return false;

        if ((currentState == eStates.Fall || currentState == eStates.Jump)
                && (timeSinceJump >= 0f && timeSinceJump <= launchTime))
            return true;

        return false;
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Event Intake

    void OnUseAbility(eActivation actState, int mouseButton)
    {
        UseAbility(actState, mouseButton);
    }

    void OnAbilityActivatingChange(bool isActivating)
    {
        activatingAbility = isActivating;
    }

    void OnAbilityActiveChange(bool isActive)
    {
        usingAbility = isActive;
    }


    void OnJump(eActivation actState)
    {
        Jump(actState);
    }

    void OnAirborn()
    {
        if(timeSinceJump <= 0f || (timeSinceJump >= launchTime))
        {
            //Airborn.Invoke();
            stateAirborn.Invoke();
            currentState = eStates.Fall;
            timeSinceJump = -1f;
        }
    }

    public void OnLanded()
    {
        //HitGround.Invoke();
        stateHitGround.Invoke();
        timeSinceJump = -1f;
        currentState = eStates.Idle;
    }

    void OnSprint(eActivation state)
    {
        Sprint(state);
    }

    private void OnMove(Vector3 dir)
    {
        frameMovement = dir;  
    }

    #endregion

    private void AnimMatchState()
    {
        switch (currentState)
        {
            case eStates.Dead:
                playerDied.Invoke();
                break;
            case eStates.Fall:
                stateAirborn.Invoke();
                break;
            case eStates.Idle:
                stateIdle.Invoke();
                break;
            case eStates.Jump:
                stateJumping.Invoke();
                break;
            case eStates.Land:
                stateHitGround.Invoke();
                break;
            case eStates.Move:
                stateRunning.Invoke(health > (float)maxHealth / 2.0f);
                break;
            case eStates.Sprint:
                stateSprinting.Invoke(health > (float)maxHealth / 2.0f);
                break;
        }
    }

}
