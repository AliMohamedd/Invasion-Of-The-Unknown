using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTU
{
    /// <summary>
    /// The WinScript class handles the behavior when the player wins the game.
    /// It listens to game events for key found and next level, and triggers actions when the player interacts with it or focuses on it.
    /// </summary>
    public class WinScript : Interactable
    {
        private bool keyIsFound = false; // Flag indicating if the key is found
        private bool nextLevel = true; // Flag indicating if the next level is ready

        private void OnEnable()
        {
            // Subscribe to relevant game events when the script is enabled
            GameEvents.GameKeyFound += GameEvents_GameWon;
            GameEvents.NextLevel += GameEvents_NextLevel;
        }

        private void OnDisable()
        {
            // Unsubscribe from game events when the script is disabled to prevent memory leaks
            GameEvents.GameKeyFound -= GameEvents_GameWon;
            GameEvents.NextLevel -= GameEvents_NextLevel;
        }

        // Event handler for when the game key is found
        private void GameEvents_GameWon(bool val)
        {
            keyIsFound = val; // Update the keyIsFound flag
        }
        
        // Event handler for when the next level event is triggered
        private void GameEvents_NextLevel(bool val)
        {
            nextLevel = val; // Update the nextLevel flag
        }
    
        // Called when the player focuses on the win area.
        // If the key is found, triggers UI events, game events, and cursor management.
        public override void OnFocus()
        {
            if (keyIsFound)
            {
                // Trigger UI events
                UIEvents.FirstMainMenuScreenShown?.Invoke();

                // Trigger scene events
                SceneEvents.LastSceneUnloaded();

                // Trigger game events
                GameEvents.GameWon?.Invoke();
                GameEvents.GameUI?.Invoke();
                GameEvents.NextLevel?.Invoke(false); // Disable next level (assuming it's a toggle)

                // Manage cursor visibility and lock state
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public override void onLoseFocus()
        {
            
        }
        
        public override void OnInteract()
        {
            
        }

    }
}
