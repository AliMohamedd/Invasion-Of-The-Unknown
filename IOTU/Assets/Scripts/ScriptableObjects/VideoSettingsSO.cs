using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace IOTU
{
    /// <summary>
    /// The VideoSettingsSO class in the IOTU namespace is a ScriptableObject that holds video settings 
    /// such as brightness, contrast, resolution, and difficulty pointer values. It subscribes to 
    /// SettingsEvents to listen for changes in these settings.
    /// </summary>
    [CreateAssetMenu(fileName = "VideoSettings", menuName = "IOTU/VideoSettings", order = 1)]
    public class VideoSettingsSO : DescriptionSO
    {
        // Default ScriptableObject data
        const int k_DefaultPointerRes = 4;
        const int k_DefaultPointerDiff = 0;

        const float k_DefaultBrightness = 0.5f;
        const float k_DefaultContrast = 0.5f;

        [Header("Display Settings")]
        [Tooltip("Brightness level")]
        [SerializeField] private float m_Brightness = k_DefaultBrightness;
        
        [Tooltip("Contrast level")]
        [SerializeField] private float m_Contrast = k_DefaultContrast;

        [Header("Resolution Settings")]
        [Tooltip("Resolution pointer")]
        [SerializeField] private int m_pointerRes = k_DefaultPointerRes;
        
        [Tooltip("Difficulty pointer")]
        [SerializeField] private int m_pointerDiff = k_DefaultPointerDiff;

        // Properties
        public float valueBrightness { get => m_Brightness; set => m_Brightness = value; }
        public float valueContrast { get => m_Contrast; set => m_Contrast = value; }

        public int pointerRes { get => m_pointerRes; set => m_pointerRes = value; }
        public int pointerDiff { get => m_pointerDiff; set => m_pointerDiff = value; }

        // Event subscriptions
        private void OnEnable()
        {
            // Subscribe to brightness and contrast changes
            SettingsEvents.BrightnessVolumeChanged += SettingsEvents_BrightnessVolumeChanged;
            SettingsEvents.ContrastVolumeChanged += SettingsEvents_ContrastVolumeChanged;

            // Subscribe to resolution and difficulty changes
            SettingsEvents.ResolutionValueChanged += SettingsEvents_ResolutionValumeChanged;
            SettingsEvents.DifficultyValueChanged += SettingsEvents_DifficultiValumeChanged;

            // Validate null references
            NullRefChecker.Validate(this);
        }

        // Event unsubscriptions
        private void OnDisable()
        {
            // Unsubscribe from brightness and contrast changes
            SettingsEvents.BrightnessVolumeChanged -= SettingsEvents_BrightnessVolumeChanged;
            SettingsEvents.ContrastVolumeChanged -= SettingsEvents_ContrastVolumeChanged;

            // Unsubscribe from resolution and difficulty changes
            SettingsEvents.ResolutionValueChanged -= SettingsEvents_ResolutionValumeChanged;
            SettingsEvents.DifficultyValueChanged -= SettingsEvents_DifficultiValumeChanged;
        }

        // Event handlers for brightness and contrast changes
        private void SettingsEvents_BrightnessVolumeChanged(float val)
        {
            PlayerPrefs.SetFloat("Brightness", val);
            PlayerPrefs.Save();
        }

        private void SettingsEvents_ContrastVolumeChanged(float val)
        {
            PlayerPrefs.SetFloat("Contrast", val);
            PlayerPrefs.Save();
        }

        // Event handlers for resolution and difficulty changes
        private void SettingsEvents_ResolutionValumeChanged(int point)
        {
            PlayerPrefs.SetInt("Resolution", point);
            PlayerPrefs.Save();
        }

        private void SettingsEvents_DifficultiValumeChanged(int point)
        {
            PlayerPrefs.SetInt("Difficulty", point);
            PlayerPrefs.Save();
        }
    }
}
