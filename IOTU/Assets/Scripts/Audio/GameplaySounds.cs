using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTU
{
    /// <summary>
    /// The GameplaySounds class in the IOTU namespace manages the playback of various audio effects
    /// in response to game and UI events. It requires an AudioManager component and uses it to play
    /// the appropriate audio clips for events such as correct or incorrect answers, game won or lost,
    /// and button clicks. 
    /// </summary>
    [RequireComponent(typeof(AudioManager))]
    public class GameplaySounds : MonoBehaviour
    {
        [Tooltip("Required AudioManager component")]
        [SerializeField] AudioManager m_AudioManager;

        // Constant value to set a delay for end game sounds
        const float k_EndGameDelay = 0.5f;

        // Flag to store if the player is a winner
        bool m_IsWinner;

        // Subscribe to game events

        // Note: we could formalize the event-handling method names to $"{sender}_{event}",
        // but instead we just reuse some simple methods to playback clicks and beeps
        // for flexibility
        private void OnEnable()
        {
            if (m_AudioManager == null)
                m_AudioManager = GetComponent<AudioManager>();

           
            GameEvents.GameWon += PlayGameWonSound;
            GameEvents.GameLost += PlayGameLostSound;
            GameEvents.GameStarted += PlayGameBackGroundSound;
            GameEvents.DoorOpen += PlayDoorOpenSound;
            GameEvents.DoorClose += PlayDoorCloseSound;
            GameEvents.FootSteps += playFootStepsSound;
            GameEvents.GameUI += PlayUiBackGroundSound;
            

            UIEvents.MainMenuScreenShown += PlayClickSound;
            UIEvents.PauseScreenShown += PlayClickSound;
            UIEvents.VideoOptionsScreenShown += PlayClickSound;
            UIEvents.AudioOptionsScreenShown += PlayClickSound;
            UIEvents.InventoryScreenShown += PlayClickSound;  
            UIEvents.ScreenClosed += PlayClickSound;
         
        }

        // Unsubscribe to prevent errors
        private void OnDisable()
        {
            GameEvents.GameWon -= PlayGameWonSound;
            GameEvents.GameLost -= PlayGameLostSound;
            GameEvents.GameStarted -= PlayGameBackGroundSound;
            GameEvents.DoorOpen -= PlayDoorOpenSound;
            GameEvents.DoorClose -= PlayDoorCloseSound;
            GameEvents.FootSteps -= playFootStepsSound;
            GameEvents.GameUI -= PlayUiBackGroundSound;

            UIEvents.MainMenuScreenShown -= PlayClickSound;
            UIEvents.PauseScreenShown -= PlayClickSound;
            UIEvents.VideoOptionsScreenShown -= PlayClickSound;
            UIEvents.AudioOptionsScreenShown -= PlayClickSound;
            UIEvents.InventoryScreenShown -= PlayClickSound;  
            UIEvents.ScreenClosed -= PlayClickSound;
        
        }

        private void Start()
        {
            // Verifies required fields in the Inspector
            NullRefChecker.Validate(this);

            // Verifies the required AudioClips in the Audio Settings ScriptableObject
            NullRefChecker.Validate(m_AudioManager.AudioSettings);
        }

        // Mehod to play the game won sound effect
        private void PlayGameWonSound()
        {   
            // Play the game won sound effect at the origin (Vector3.zero) without looping.
            m_AudioManager.PlaySFXAtPoint(m_AudioManager.AudioSettings.GameWonSound, Vector3.zero, k_EndGameDelay, false);
        }

        // Meyhod to play the game lost sound effect
        private void PlayGameLostSound()
        {
            // Play the game lost sound effect at the origin (Vector3.zero) without looping.
            m_AudioManager.PlaySFXAtPoint(m_AudioManager.AudioSettings.GameLostSound, Vector3.zero, k_EndGameDelay, false);
        }

        // Method to play the background music for the game.
        private void PlayGameBackGroundSound()
        {
            // Play the gameplay background music with looping enabled.
            m_AudioManager.PlayMusic(m_AudioManager.AudioSettings.GamePlay, 0, true);
        }

        // Method to play the background music for the UI.
        private void PlayUiBackGroundSound()
        {
            // Play the UI background music with looping enabled.
            m_AudioManager.PlayMusic(m_AudioManager.AudioSettings.StartingUi, 0, true);
        }

        // Method to play the sound effect for opening a door.
        private void PlayDoorOpenSound()
        {
            // Play the door opening sound effect at the origin (Vector3.zero) without looping.
            m_AudioManager.PlaySFXAtPoint(m_AudioManager.AudioSettings.DoorOpenSound, Vector3.zero, 0, false);
        }

        // Method to play the sound effect for closing a door.
        private void PlayDoorCloseSound()
        {
            // Play the door closing sound effect at the origin (Vector3.zero) without looping.
            m_AudioManager.PlaySFXAtPoint(m_AudioManager.AudioSettings.DoorCloseSound, Vector3.zero, 0, false);
        }

        // Method to play the sound effect for footsteps.
        private void playFootStepsSound()
        {
            // Play the footsteps sound effect at the origin (Vector3.zero) without looping.
            m_AudioManager.PlaySFXAtPoint(m_AudioManager.AudioSettings.FootStepsSound, Vector3.zero, 0, false);
        }

        // Play the end screen sound based on whether the player has won or lost
        private void PlayEndScreenSound()
        {
            // Play the click for the Continue button
            PlayClickSound();

            // Play win/lose sound after short delay
            if (m_IsWinner)
                PlayGameWonSound();
            else
                PlayGameLostSound();
        }

        // Store the win/lose state to delay playback of the endscreen sound

        // Set the player as a loser
        private void SetLoser()
        {
            m_IsWinner = false;
        }

        // Set the player as a winner
        private void SetWinner()
        {
            m_IsWinner = true;
        }

        // Plays UI button click sound
        private void PlayClickSound()
        {
            m_AudioManager.PlaySFXAtPoint(m_AudioManager.AudioSettings.TapClickSound, Vector3.zero, 0f, false);
        }
        
        // Method to play the click sound, overloaded to accept a list of strings.
        private void PlayClickSound(List<string> strings)
        {   
            // Call the base PlayClickSound method to play the click sound.
            PlayClickSound();
        }
        
        // Method to play the click sound, overloaded to accept a single string.
        private void PlayClickSound(string str)
        {   
            // Call the base PlayClickSound method to play the click sound.
            PlayClickSound();
        }
        
        // Method to play the click sound, overloaded to accept an integer index.
        private void PlayClickSound(int index)
        {
            // Call the base PlayClickSound method to play the click sound.
            PlayClickSound();
        }
    }
}
