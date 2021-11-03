using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Mirror;

// TODO:
// - Fix building creation in MenuButton and Hotbar (commented out)
// - Get resources properly connected to survival

public class Gamemode : MonoBehaviour
{
    // Active instance
    public static Gamemode active;

    // Gamemode information
    [Header("Gamemode Info")]
    public new string name;
    public string version;
    public Difficulty difficulty;

    [Header("Gamemode Settings")]
    public bool useResources;
    public bool initBuildings;
    public bool initEnemies;
    public bool initGuardians;

    // Register for events
    public void Start()
    {
        active = this;
        GameManager.SetupGame(difficulty);
        InitGamemode();
    }

    // Tells the gamemode how to generate inventory
    public void InitGamemode()
    {
        ScriptableManager.GenerateAllScriptables();

        if (initBuildings) Inventory.active.GenerateEntities(ScriptableManager.buildings.ToArray());
        if (initGuardians) Inventory.active.GenerateEntities(ScriptableManager.enemies.ToArray());
        if (initGuardians) Inventory.active.GenerateEntities(ScriptableManager.guardians.ToArray());

        EnemyHandler.UpdateVariant();
    }
}
