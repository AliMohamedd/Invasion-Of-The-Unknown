using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The DoorRemover class handles the replacement of an openable door with a different door 
/// when a trigger event occurs. It manages the activation and deactivation of specified game objects.
/// </summary>
public class DoorRemover : MonoBehaviour
{
    [Header("Door Switch")]
    [Tooltip("GameObject to be turned off when the trigger is entered.")]
    [SerializeField] GameObject turnOffGameObject;

    [Tooltip("GameObject representing the openable door to be turned off.")]
    [SerializeField] GameObject openableDoor;

    [Tooltip("GameObject representing the replacement door to be turned on.")]
    [SerializeField] GameObject replacementDoor;
  

    // Handles the trigger enter event. When another collider enters the trigger,
    // it disables the specified game objects and enables the replacement door.
    private void OnTriggerEnter(Collider other)
    {
        // Deactivate the game objects specified in the inspector
        turnOffGameObject.SetActive(false);
        openableDoor.SetActive(false);
        
        // Activate the replacement door game object
        replacementDoor.SetActive(true);
    }
}
