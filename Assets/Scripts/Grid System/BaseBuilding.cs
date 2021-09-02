﻿using UnityEngine;
using Mirror;

[HideInInspector]
public class BaseBuilding : NetworkBehaviour, IDamageable
{
    // Building scriptable
    public Building building;

    // IDamageable interface variables
    public int health { get; set; }
    public int maxHealth { get; set; }

    // Sets the buildings stats
    public void SetBuildingStats()
    {
        if (building == null)
        {
            Debug.LogError(transform.name + " does not have a scriptable attached to it!");
        }
        else
        {
            health = building.health;
            maxHealth = building.health;
        }
    }

    // Damages the entity (IDamageable interface method)
    public void DamageEntity(int dmg)
    {
        health -= dmg;
        if (health <= 0) DestroyEntity();
    }

    // Destroys the entity (IDamageable interface method)
    public void DestroyEntity()
    {
        // Create the particle
        if (building.particle != null)
        {
            ParticleSystemRenderer holder = Instantiate(building.particle, transform.position, Quaternion.identity).GetComponent<ParticleSystemRenderer>();
            if (building.material != null)
            {
                holder.material = building.material;
                holder.trailMaterial = building.material;
            }

        }
        Destroy(gameObject);
    }

    // Heals the entity (IDamageable interface method)
    public void HealEntity(int amount)
    {
        health += amount;
        if (health > maxHealth) health = maxHealth;
    }

    public int GetHealth() { return building.health; }
    public int GetMaxHealth() { return building.maxHealth; }
    public string GetDescription() { return building.description; }
}
