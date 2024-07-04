using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The HouseManager class handles the activation and deactivation of specified game objects
/// when a trigger event occurs. It switches the states of the activator and deactivator objects.
/// </summary>
public class HouseManager : MonoBehaviour
{
    [Tooltip("GameObject to be turned on or off based on the activator's state.")]
    [SerializeField] GameObject turnOnGameobject;

    [Tooltip("GameObject that triggers the activation or deactivation of other objects.")]
    [SerializeField] GameObject activator;

    [Tooltip("GameObject to be activated when the activator is active.")]
    [SerializeField] GameObject deactivator;

    // Handles the trigger enter event. When another collider enters the trigger,
    // it toggles the states of the specified game objects based on the activator's state.
    private void OnTriggerEnter(Collider other)
    {
        // Check if the activator is active
        if (activator.activeSelf == true)
        {
            // Activate the deactivator and turnOnGameobject, and deactivate the activator
            deactivator.SetActive(true);
            turnOnGameobject.SetActive(true);
            activator.SetActive(false);
        }
        else
        {
            // Deactivate the deactivator and turnOnGameobject, and activate the activator
            deactivator.SetActive(false);
            turnOnGameobject.SetActive(false);
            activator.SetActive(true);
        }
    }
}
