using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace IOTU
{
    /// <summary>
    /// The GamePlaySO class in the IOTU namespace is a ScriptableObject that holds gameplay-related settings
    /// such as whether the next level should be loaded. It subscribes to NextLevel events to update its state
    /// based on game events.
    /// </summary>
    [CreateAssetMenu(fileName = "GamePlay", menuName = "IOTU/GamePlay", order = 1)]
    public class GamePlaySO : DescriptionSO
    {
        // Default ScriptableObject data
        const bool k_DefaultNextLevel = false;
        
        // Serialized field for Next Level setting
        [Header("Saving")]
        [Tooltip("Next Level")]
        [SerializeField] private bool m_NextLevel = k_DefaultNextLevel;

        // Properties
        public bool NextLevel { get => m_NextLevel; set => m_NextLevel = value; }
       
        // Event subscriptions
        private void OnEnable()
        {
            GameEvents.NextLevel += GameEvents_NextLevel;
        }

        // Event unsubscriptions
        private void OnDisable()
        {
            GameEvents.NextLevel -= GameEvents_NextLevel;
        }

        // Method to handle the NextLevel event
        private void GameEvents_NextLevel(bool val)
        {
            if (val)
            {
               PlayerPrefs.SetInt("NextLevel", 1);
               PlayerPrefs.Save();
            }
            else
            {
               PlayerPrefs.SetInt("NextLevel", 0);
               PlayerPrefs.Save();
            }
        }
    }
}
