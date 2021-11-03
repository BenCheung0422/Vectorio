using UnityEngine;
//using Mirror;
using System.Collections.Generic;

// This script is ported from Automa.
// https://github.com/Vitzual/Automa

public class BuildingHandler : MonoBehaviour
{
    // Grid variable
    [HideInInspector] public Grid tileGrid;

    // Building variables
    public static BuildingHandler active;
    public static bool useResources;
    public LayerMask enemyLayer;

    public void Start()
    {
        // Get reference to active instance
        active = this;

        // Sets static variables on start
        tileGrid = new Grid();
        tileGrid.cells = new Dictionary<Vector2Int, Cell>();
    }

    // Creates a building
    public void CreateBuildable(Entity entity, Vector2 position, Quaternion rotation, bool isEnemy)
    {
        // Check if entity is null
        if (entity == null) return;

        // Check if resource should be used
        if (useResources) 
        {
            foreach (Entity.Resources resource in entity.resources) 
            {
                if (!resource.storage)
                {
                    int amount = Resource.active.GetAmount(resource.resource);
                    if (resource.add && amount + resource.amount > Resource.active.GetStorage(resource.resource)) return;
                    else if (amount < resource.amount) return;
                }
            }
        }

        // Check to make sure the tiles are not being used
        if (!isEnemy && !CheckTiles(entity, position)) return;

        // Instantiate the object like usual
        if (isEnemy) RpcInstantiateEnemy(entity, position, rotation);
        else RpcInstantiateBuilding(entity, position, rotation);
    }

    //[ClientRpc]
    private void RpcInstantiateEnemy(Entity entity, Vector2 position, Quaternion rotation)
    {
        // Use enemy handler thing
        RaycastHit2D[] hits = Physics2D.RaycastAll(position, Vector2.zero, Mathf.Infinity, enemyLayer);
        foreach (RaycastHit2D hit in hits)
            if (hit.collider != null) return;
        EnemyHandler.CreateEntity(entity, position, rotation);
    }

    //[ClientRpc]
    private void RpcInstantiateBuilding(Entity entity, Vector2 position, Quaternion rotation)
    {
        // Get game objected from scriptable manager
        GameObject obj = ScriptableManager.RequestBuildingByName(entity.name);
        if (obj == null) return;

        // Create the tile
        BaseTile lastBuilding = Instantiate(obj, position, rotation).GetComponent<BaseTile>();
        lastBuilding.name = entity.name;

        // Set the tiles on the grid class
        if (entity.cells.Length > 0)
        {
            foreach (Building.Cell cell in entity.cells)
                tileGrid.SetCell(Vector2Int.RoundToInt(new Vector2(lastBuilding.transform.position.x + cell.x, lastBuilding.transform.position.y + cell.y)), true, entity, lastBuilding);
        }

        // Update resource values promptly
        if (useResources)
        {
            foreach (Entity.Resources resource in entity.resources)
            {
                if (resource.storage)//
                {
                    if (resource.add) Resource.active.AddStorage(resource.resource, resource.amount);
                    else Resource.active.RemoveStorage(resource.resource, resource.amount);
                }
                else
                {
                    if (resource.add) Resource.active.Add(resource.resource, resource.amount);
                    else Resource.active.Remove(resource.resource, resource.amount);
                }
            }
        }

        // Call buildings setup method
        lastBuilding.Setup();
    }

    // Destroys a buildingg
    //[ClientRpc]
    public void RpcDestroyBuilding(Vector3 position)
    {
        tileGrid.DestroyCell(Vector2Int.RoundToInt(position));
    }

    // Checks to make sure tile(s) isn't occupied
    //[Server]
    public bool CheckTiles(Entity entity, Vector3 position)
    {
        if (entity.cells.Length > 0)
        {
            foreach (Building.Cell cell in entity.cells)
            {
                // Check to make sure nothing occupying tile
                Vector2Int coords = Vector2Int.RoundToInt(new Vector2(position.x + cell.x, position.y + cell.y));
                if (tileGrid.RetrieveCell(coords) != null)
                    return false;
            }
        }
        else
        {
            // Check to make sure nothing occupying tile
            Vector2Int coords = Vector2Int.RoundToInt(new Vector2(position.x, position.y));
            if (tileGrid.RetrieveCell(coords) != null)
                return false;
        }
        return true;
    }

    // Returns closest building to position given
    public BaseTile GetClosestBuilding(Vector2Int position)
    {
        BaseTile nearest = null;
        float distance = float.PositiveInfinity;

        foreach (KeyValuePair<Vector2Int, Cell> cell in tileGrid.cells)
        {
            float holder = Vector2Int.Distance(position, cell.Key);
            if (holder < distance)
            {
                distance = holder;
                nearest = cell.Value.obj;
            }
        }

        return nearest;
    }

    // Attempts to return a building
    public BaseTile TryGetBuilding(Vector2 position)
    {
        Cell cell = tileGrid.RetrieveCell(Vector2Int.RoundToInt(position));
        if (cell != null)
        {
            BaseTile building = cell.obj.GetComponent<BaseTile>();
            return building;
        }
        return null;
    }
}
