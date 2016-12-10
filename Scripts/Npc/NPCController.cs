using UnityEngine;
using System.Collections;
using System;

public class NPCController : CharacterInfo
{
    void Start()
    {
        CurrentHealth = MaxHealth;
    }

    void Update()
    {

    }

    public override void OnDeath()
    {
        
    }
}
