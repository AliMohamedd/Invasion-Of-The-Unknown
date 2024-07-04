using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace IOTU
{
    /// <summary>
    /// The KeyController class handles the behavior of a key object in the game.
    /// It overrides methods from the Interactable base class to define interactions and focus behavior.
    /// </summary>
    public class KeyController : Interactable
    {
        
        // Called when the player interacts with the key object.
        // Deactivates the GameObject, shows a dot (indicating interaction), and triggers a game event indicating the key was found.
        public override void OnInteract()
        {
            gameObject.SetActive(false); // Deactivate the key GameObject
            GameEvents.ShowDot?.Invoke(); // Show a dot (visual cue for interaction)
            GameEvents.GameKeyFound?.Invoke(true); // Trigger game event indicating key was found
        }

        // Called when the key object comes into focus for interaction.
        // Shows an interact label.
        public override void OnFocus()
        {
            GameEvents.ShowInteractLabel?.Invoke(); // Show interact label
        }
  
        // Called when the key object loses focus for interaction.
        // Shows a dot (indicating interaction).
        public override void onLoseFocus()
        {
            GameEvents.ShowDot?.Invoke(); // Show a dot (indicating interaction)
        }
    }
}
