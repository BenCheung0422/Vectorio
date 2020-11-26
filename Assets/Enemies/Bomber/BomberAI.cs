﻿using UnityEngine;
using System.Collections;

public class BomberAI : EnemyClass
{
    // Model components
    private Rigidbody2D Bomber;

    // Movement variables
    private Vector2 Movement;

    // On start, get rigidbody and assign death effect
    void Start()
    {
        Bomber = this.GetComponent<Rigidbody2D>();
    }

    // Targetting system
    void Update()
    {
        // Find closest enemy 
        if (target == null) {
            target = FindNearestDefence();
        }
        if (target != null)
        {
            float distance = (target.transform.position - this.transform.position).sqrMagnitude;
            // Rotate towards current target
            Vector2 TargetPosition = new Vector2(target.gameObject.transform.position.x, target.gameObject.transform.position.y);

            // Move towards defense
            Vector2 lookDirection = TargetPosition - Bomber.position;

            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f;
            Bomber.rotation = angle;
            lookDirection.Normalize();
            Movement = lookDirection;
            Movement = lookDirection;
        } 
        else
        {
            Movement = new Vector2(0, 0);
        }
    }

    // Move entity towards target every frame
    private void FixedUpdate()
    {
        Bomber.AddForce(Movement * moveSpeed);
    }
}
