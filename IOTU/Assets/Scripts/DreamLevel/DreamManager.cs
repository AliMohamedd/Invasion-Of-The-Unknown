using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The DreamManager class handles the activation of a specified GameObject when the player enters a trigger.
/// It is used to create dynamic changes in the game environment based on player interactions.
/// </summary>
public class DreamManager : MonoBehaviour
{
    [Header("Hallway Repetitions")]
    [Tooltip("GameObject to be activated when the player enters the trigger.")]
    [SerializeField] GameObject turnOnGameObject;

    /// Handles the trigger enter event. When the player enters the trigger,
    /// it activates the specified game object.
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player
        if (other.CompareTag("Player"))
        {
            // Activate the game object specified in the inspector
            turnOnGameObject.SetActive(true);
        }
    }
}
