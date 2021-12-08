using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// Acts as a bin for all resources. Still a WIP since
// a lot of logic still falls within the Resource.cs
// class, but you can imagine this as literally a bin
// for resources. Can have one bin that all players
// use, or split them up (teams, individual, etc.)

public class ResourceBin : NetworkBehaviour
{
    // Active local instance
    public static ResourceBin active;

    // Connect to syncer
    public void Start()
    {
        if (hasAuthority) active = this;
    }

    // Update collector grab for all players
    [Command]
    public void CmdCollector(int id, int amount)
    {
        if (hasAuthority) Server.active.SrvSyncCollector(id, amount);
    }

    // Rpc collector on all clients
    [ClientRpc]
    public void RpcCollect(int id, int amount)
    {
        // Reset the tile 
        if (isClientOnly)
        {
            if (Server.entities.ContainsKey(id))
                Server.entities[id].SyncEntity(amount);
            else Debug.Log("[SERVER] Client received a collector with a runtime ID that doesn't exist. " +
                "This will cause major issues with desyncing!");
        }
    }
}
