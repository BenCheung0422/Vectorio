﻿using UnityEngine;
using Michsky.UI.ModernUIPack;
using TMPro;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    // Active instance
    public List<Enemy> enemies;
    public static EnemySpawner active;
    public int maxEnemiesAllowed = 200;

    public void Start()
    {
        active = this;
        InvokeRepeating("SpawnEnemies", 1, 1);

        enemies.AddRange(ScriptableLoader.enemies.Values);
    }

    public void SpawnEnemies()
    {
        // Check if enemy handler is active
        if (EnemyHandler.active.variant == null ||
            EnemyHandler.active.enemies.Count > maxEnemiesAllowed) return;

        // Setup variables
        Vector2 spawnPos;
        float chance;

        // Calculate chance (or ignore)
        if (Debugger.active.ignoreSpawnValues) chance = 0f;
        else chance = Random.value;

        // Calculate heat percentage
        float percentage = (float)Resource.active.GetHeat() / EnemyHandler.active.variant.maxHeat;
        Debug.Log(percentage);
        
        // Loop through all enemies
        foreach(Enemy enemy in enemies)
        {
            // If spawn percentage is above the chance value, spawn
            if (enemy.spawnPercentage <= percentage && (enemy.spawnChance * (percentage + 1)) >= chance)
            {
                // Get location around border
                if (Random.value > 0.5f)
                {
                    if (Random.value > 0.5f) spawnPos = new Vector2(Border.west, Random.Range(Border.south, Border.north));
                    else spawnPos = new Vector2(Border.east, Random.Range(Border.south, Border.north));
                }
                else
                {
                    if (Random.value > 0.5f) spawnPos = new Vector2(Random.Range(Border.west, Border.east), Border.north);
                    else spawnPos = new Vector2(Random.Range(Border.west, Border.east), Border.south);
                }

                // Create enemy
                EnemyHandler.active.CreateEntity(enemy, spawnPos, Quaternion.identity);
            }
        }
    }
}
