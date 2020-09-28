using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public interface IHaveHealth 
{
     int health { get; set; }

     void SetHealth(int setAmount);
     void IncrementHealth(int amount);
     void Kill();
}

public class Health : MonoBehaviour, IHaveHealth
{

    public event Action<int, int> HealthChanged_Event = delegate { };
    public event Action Death_Event = delegate { };

    public int health { get { return _health; }
                        set
                        {
                            if (value <= 0 && _health > 0)
            {
                Died();
                HealthChanged_Event(_health, _health);
                _health = 0;

            }

            else if(value != _health)
                            {
                            HealthChanged_Event(value - _health, _health);
                            _health = value;

                            }
        } }
    private int _health = 100;

    private void Awake()
    {
        health = _health;
    }

    public void Kill()
    {
        health = 0;
    }

    private void Died()
    {
        Death_Event.Invoke();

    }

    public void IncrementHealth(int amount)
    {
        health += amount;
    }

    public void SetHealth(int setAmount)
    {
        health = setAmount;
    }
        
}
