using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAnimator : MonoBehaviour
{
    Animator animator = null;
    [SerializeField] private CharacterController controll = null;

    const string idleState = "Idle";
    const string runState = "Run";
    const string runHurtState = "RunHurt";
    const string sprintState = "Sprint";
    const string sprintHurtState = "Sprint";
    const string jumpState = "Jump";
    const string fallState = "Fall";
    const string landState = "Land";
    const string hardLandState = "HardLand";
    const string deathState = "Death";

    const string shieldState = "Shield";
    const string shieldRunState = "ShieldRun";
    const string shieldSprintState = "ShieldSprint";
    const string shildDomeState = "ShieldDome";



    #region Init
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        controll.stateHitGround += OnLand;
        controll.stateIdle += OnIdle;
        controll.stateJumping += OnJump;
        controll.stateRunning += OnRun;
        controll.stateSprinting += OnSprint;
        controll.stateAirborn += OnFall;
        controll.stateAbility += OnAbility;


        controll.playerDied += OnDeath;
    }

    private void OnDisable()
    {
        controll.stateHitGround -= OnLand;
        controll.stateIdle -= OnIdle;
        controll.stateJumping -= OnJump;
        controll.stateRunning -= OnRun;
        controll.stateSprinting -= OnSprint;
        controll.stateAirborn -= OnFall;
        controll.stateAbility -= OnAbility;

    }

    #endregion

    public void OnIdle()
    {
        animator.CrossFadeInFixedTime(idleState, 0.2f);
    }

    public void OnRun(bool isNotHurt)
    {
        if(isNotHurt)
            animator.CrossFadeInFixedTime(runState, 0.2f);
        else
            animator.CrossFadeInFixedTime(runHurtState, 0.2f);
    }

    public void OnSprint(bool isHurt)
    {
        if(!isHurt)
            animator.CrossFadeInFixedTime(sprintState, 0.2f);
        else
            animator.CrossFadeInFixedTime(sprintHurtState, 0.2f);
    }

    public void OnJump()
    {
        animator.CrossFadeInFixedTime(jumpState, 0.2f);
    }

    public void OnLand()
    {
        animator.CrossFadeInFixedTime(landState, 0.05f);
        //animator.Play(landState);
    }

    public void OnFall()
    {
        //animator.CrossFadeInFixedTime(fallState, 0.1f);
    }

    public void OnDeath()
    {
        animator.CrossFadeInFixedTime(deathState, 0.2f);
        //Debug.Log("Played dead");
    }


    public void OnAbilityStateChange(eAbilities abilityType, eStates newState)
    {

    }

    public void OnAbility(eAbilities type, eStates duringState)
    {
        if (type == eAbilities.Shield)
        {
            OnSheild(duringState);
        }
        else if(type == eAbilities.ShieldDome)
        {
            OnShieldDome();
        }
    }

    public void OnSheild(eStates baseState)
    {
        switch(baseState)
        {
            case eStates.Idle:
            {
                    animator.CrossFadeInFixedTime(shieldState, 0.3f);
                    break;
            }
            case eStates.Move:
                {
                    animator.CrossFadeInFixedTime(shieldRunState, 0.3f);
                    break;
                }
            case eStates.Sprint:
                {
                    animator.CrossFadeInFixedTime(shieldSprintState, 0.3f);
                    break;
                }
            default:
                Debug.Log("No shield animation for the " + baseState.ToString() + " state.");
                break;
        }
    }

    public void OnShieldDome()
    {
        animator.CrossFadeInFixedTime(shildDomeState, 0.4f);
    }
}
