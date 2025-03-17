using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverBtn; //button for testing server 
    [SerializeField] private Button hostBtn; //button for host 
    [SerializeField] private Button clientBtn; //button for client 
    [SerializeField] private Button damagebtn; //damage button 
    [SerializeField] private HealthSystemForDummies clientHealthSystem; //health meter
    [SerializeField] private AudioSource audioSource; // Audio source component
    [SerializeField] private AudioClip damageSound;  // Sound clip for damage

    private void Awake(){
        //setting up buttons doe the event listeners
        //starting the game as host / registers health 
        hostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            RegisterHealthSystem();
        });
        //starting game as client, register health
        clientBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            RegisterHealthSystem();
        });
        //handling damage and adding sound 
        damagebtn.onClick.AddListener(()=>{
            {
                PlayDamageSound();

                //getting local network object for player 
                var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
                var player = playerObject.GetComponent<PlayerNetwork>();
                player.Damage();
            }

        });


    }

    void RegisterHealthSystem(){
        //registering health with client side 
        var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
                var player = playerObject.GetComponent<PlayerNetwork>();
                player.healthSystem = GetComponentInParent<HealthSystemForDummies>();
    }

    private void PlayDamageSound(){
        //method to add damage sound 
    if (audioSource == null)
    {
        Debug.LogWarning("AudioSource is missing! Assign an AudioSource in the Inspector.");
        return;
    }

    if (damageSound == null)
    {
        Debug.LogWarning("Damage sound is missing! Assign an AudioClip in the Inspector.");
        return;
    }

    audioSource.PlayOneShot(damageSound);
}

 
}



