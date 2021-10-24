using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Mirror;

// TODO:
// - Fix building creation in MenuButton and Hotbar (commented out)
// - Get resources properly connected to survival

public class Gamemode : MonoBehaviour
{
    // Gamemode information
    public new string name;
    public string version;

    // Register for events
    public void Start()
    {
        Events.active.onLeftMousePressed += PlaceBuilding;
        Events.active.setupBuildables += InitEntities;
    }

    // Tells the gamemode how to handle building placements
    public virtual void PlaceBuilding()
    {
        Debug.Log("Mode does not contain definition for building placed");
    }

    // Tells the gamemode how to generate inventory
    public virtual void InitEntities()
    {
        ScriptableManager.GenerateAllScriptables();
    }
}
