using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerNetwork : NetworkBehaviour
{
   // Caches, references to health system -- heavily edited asset code 
   public HealthSystemForDummies healthSystem;

   //value for health 
    public float Health = 100;
    private void Update(){
        if (!IsOwner) return; // allowing movemnet for the owner 

        //initializing movement 
        Vector3 moveDir = new Vector3(0, 0, 0);

        //intitializing the input moevemnet 
        if (Input.GetKey(KeyCode.W)) moveDir.z = +1f;
        if (Input.GetKey(KeyCode.S)) moveDir.z = -1f;
        if (Input.GetKey(KeyCode.A)) moveDir.x = -1f;
        if (Input.GetKey(KeyCode.D)) moveDir.x = +1f;

        //implementing the movement 
        float moveSpeed = 3f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

}

//handling recieving damage and updateing health 
public void ReceiveDamage(int value){
            Health -= value;
            healthSystem.AddToCurrentHealth(-value);
            Debug.Log($"==========>> Received Damage #{value}");
            
}
//sending damage request to server 
public void Damage(){
    // if (!IsServer && IsOwner) //Only send an RPC to the server on the client that owns the NetworkObject that owns this NetworkBehaviour instance
       {
           SendServerRpc(20, NetworkObjectId);
       }

}
//notifying clients of recieved damage 
   [ClientRpc]
   void BroadcastClientRpc(int value, ulong sourceNetworkObjectId)
   {
       Debug.Log($"Client Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId} with object Id {NetworkObjectId}");
     
       
   }

    //server processing of damage and informing clients of it 
   [ServerRpc]
   void SendServerRpc(int value, ulong sourceNetworkObjectId)
   {
       Debug.Log($"Server Received Damage #{value} given by NetworkObject #{sourceNetworkObjectId}");
       //broadcasting the damage to all clients 
       BroadcastClientRpc(value, sourceNetworkObjectId);

        //applying damage to connected client
       foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Debug.Log($"uid {uid}, source #{sourceNetworkObjectId}");
            //does not apply it on the player who inititated damage 
            if (uid == sourceNetworkObjectId){
                continue;
            }
            //getting network object for player. 
            var playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid);
            var player = playerObject.GetComponent<PlayerNetwork>();
            //applying damage to player
            player.ReceiveDamage(value);
        }
    
   }
}