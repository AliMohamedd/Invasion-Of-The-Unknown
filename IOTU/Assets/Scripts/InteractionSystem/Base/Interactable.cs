using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Interactable class defines a base class for interactable objects in the game.
/// It sets the GameObject's layer on Awake and defines abstract methods for interaction, 
/// focusing, and losing focus, which subclasses must implement.
/// </summary>
public abstract class Interactable : MonoBehaviour
{
    public virtual void Awake()
    {
        // Set the layer of the GameObject to layer 9 (assuming this is for interaction purposes)
        gameObject.layer = 9;
    }

    // Abstract method that must be implemented by subclasses.
    // Called when the player interacts with the object.
    public abstract void OnInteract();

    // Abstract method that must be implemented by subclasses.
    // Called when the object comes into focus for interaction.
    public abstract void OnFocus();

    // Abstract method that must be implemented by subclasses.
    // Called when the object loses focus for interaction.
    public abstract void onLoseFocus();
}
