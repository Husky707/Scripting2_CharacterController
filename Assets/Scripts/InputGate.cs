using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum eActivation { noone, entering, leaving, remaining }
public class InputGate : MonoBehaviour
{
    public event Action<Vector3> Input_Axis = delegate { };
    public event Action<eActivation> Input_Jump = delegate{};
    public event Action<eActivation> Input_Sprint = delegate { };
    public event Action<eActivation, int> Input_Ability = delegate { };

    [HideInInspector] public bool isReadingInput = true;


    void Update()
    {
        if (!isReadingInput)
            return;

        ReadAxisInput();
        ReadJumpInput();
        ReadSprintInput();
        ReadAbilityInput();
    }

    void ReadAbilityInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Input_Ability.Invoke(eActivation.entering, 1);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            Input_Ability.Invoke(eActivation.leaving, 1);
        }
        else if (Input.GetMouseButton(1))
            Input_Ability.Invoke(eActivation.remaining, 1);

        if (Input.GetMouseButtonDown(0))
        {
            Input_Ability.Invoke(eActivation.entering, 0);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Input_Ability.Invoke(eActivation.leaving, 0);
        }
        else if (Input.GetMouseButton(0))
            Input_Ability.Invoke(eActivation.remaining, 0);
    }

    void ReadSprintInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Input_Sprint.Invoke(eActivation.entering);
            return;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
            Input_Sprint.Invoke(eActivation.remaining);
        else if (Input.GetKeyUp(KeyCode.LeftShift))
            Input_Sprint.Invoke(eActivation.leaving);
    }

    void ReadJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Input_Jump.Invoke(eActivation.entering);
            return;
        }
        else if (Input.GetKey(KeyCode.Space))
            Input_Jump.Invoke(eActivation.remaining);
        else if (Input.GetKeyUp(KeyCode.Space))
            Input_Jump.Invoke(eActivation.leaving);
    }

    void ReadAxisInput()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");
        Vector3 send = new Vector3  (xInput, 0f, yInput).normalized;

        Input_Axis.Invoke(send);

        /*
        if (xInput != 0 || yInput != 0)
        {
            Vector3 _hMovement = transform.right * xInput;
            Vector3 _forwardMovement = transform.forward * yInput;

            Vector3 movement = (_hMovement + _forwardMovement).normalized;

            Input_Axis.Invoke(movement);

        }
        else
            Input_Axis.Invoke(Vector3.zero);
            */
    }
}