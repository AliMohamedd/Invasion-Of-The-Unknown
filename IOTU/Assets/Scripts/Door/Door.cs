using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTU
{
    /// <summary>
    /// The Door class manages the interaction and animation of a door in the game. 
    /// It inherits from the Interactable class and provides functionality for opening 
    /// and closing the door based on player interaction.
    /// </summary>
    public class Door : Interactable
    {
        // Flag to indicate if the door is open
        private bool isOpen = false;

        // Flag to indicate if the door can be interacted with
        private bool canBeInteractedWith = true;

        // Reference to the Animator component
        private Animator anim;

        
        // Initialize the door by getting the Animator component.
        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        
        // Handles the interaction with the door, toggling its open/closed state.
        public override void OnInteract()
        {
            if (canBeInteractedWith)
            {
                // Toggle the door's open state
                isOpen = !isOpen;

                // Calculate the direction vectors
                Vector3 doorTransformDirection = transform.TransformDirection(Vector3.right);
                Vector3 playerTransformDirection = playerController.instance.transform.position - transform.position;

                // Calculate the dot product of the direction vectors
                float dot = Vector3.Dot(doorTransformDirection, playerTransformDirection);

                // Set the animation parameters based on interaction
                anim.SetFloat("dot", dot);
                anim.SetBool("isOpen", isOpen);

                // Invoke the appropriate game event based on the door's state
                if (!isOpen)
                {
                    GameEvents.DoorOpen?.Invoke();
                }
                else
                {
                    GameEvents.DoorClose?.Invoke();
                }
            }
        }

        
        // Handles the event when the door is focused on by the player.
        public override void OnFocus()
        {
            GameEvents.ShowInteractLabel?.Invoke();
        }

        // Handles the event when the door loses focus from the player.
        public override void onLoseFocus()
        {
            GameEvents.ShowDot?.Invoke();
        }
    }
}
