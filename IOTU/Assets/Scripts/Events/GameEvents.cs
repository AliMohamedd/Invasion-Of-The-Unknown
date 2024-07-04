using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace IOTU
{
    /// <summary>
    /// Public static delegates to manage gameplay (note these are "events" in the conceptual sense
    /// and not the strict C# sense).
    /// </summary>
    public static class GameEvents
    {
        #region Gameplay events

        // Event to show or hide a note.
        public static Action<bool> noteShown;

        // Event to indicate that a game key has been found.
        public static Action<bool> GameKeyFound;
        
        // Event to indicate that a monster has found the player.
        public static Action<bool> GameMonesterFoundMe;

        // Event to indicate that the player is progressing to the next level.
        public static Action<bool> NextLevel;

        // Event to show a dot.
        public static Action ShowDot;

        // Event to show the interact label (e.g., text indicating that the player can interact with an object).
        public static Action ShowInteractLabel; 

        // Event to hide all UI elements or indicators.
        public static Action HideAll;

        // Event triggered when a door is opened.
        public static Action DoorOpen;

        // Event triggered when a door is closed.
        public static Action DoorClose;

        // Event triggered when footsteps are heard or need to be played.
        public static Action FootSteps;
        
        #endregion


        #region Game state change events:

        // Start the game
        public static Action GameStarted;

        // Continue the game
        public static Action GameContinued;

        // Pause the game during gameplay
        public static Action GamePaused;

        // Return to gameplay from the pause screen
        public static Action GameUnpaused;

        // Quit the game while on the pause screen
        public static Action GameAborted;

        // Story is complete
        public static Action GameWon;

        // Player died
        public static Action GameLost;

        // GamePlay UI shown 
        public static Action GameUI;
        
        #endregion

    }
}
