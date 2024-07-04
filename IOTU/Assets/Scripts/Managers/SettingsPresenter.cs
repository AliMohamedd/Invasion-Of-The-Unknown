using System;
using UnityEngine;

namespace IOTU
{
    /// <summary>
    ///  Controller/presenter to manage the sound volume settings and UI. This
    ///  sends messages between the user-interface (the View) and the AudioSettings
    ///  ScriptableObject data (the Model).
    /// </summary>
    public class SettingsPresenter : MonoBehaviour
    {
        // Though we can use events to communicate with the Model and View, we can also
        // directly reference them for convenience
        AudioSettingsSO m_AudioSettingsSO;
        VideoSettingsSO m_VideoSettingsSO;
        GamePlaySO m_GamePlaySO;
        
        // Event subscriptions
        private void OnEnable()
        {
            // Listen for events from the View/UI
            SettingsEvents.MasterSliderChanged += SettingsEvents_MasterSliderChanged;
            SettingsEvents.SFXSliderChanged += SettingsEvents_SFXSliderChanged;
            SettingsEvents.MusicSliderChanged += SettingsEvents_MusicSliderChanged;
            SettingsEvents.BrightnessSliderChanged += SettingsEvents_BrightnessSliderChanged;
            SettingsEvents.ContrastSliderChanged += SettingsEvents_ContrastSliderChanged;
            SettingsEvents.ResolutionChanged += SettingsEvents_ResolutionValueChanged;
            SettingsEvents.DifficultyChanged += SettingsEvents_DifficultyValueChanged;

            // Listen for events from the Model
            SettingsEvents.ModelMasterVolumeChanged += SettingsEvents_ModelMasterVolumeChanged;
            SettingsEvents.ModelSFXVolumeChanged += SettingsEvents_ModelSFXVolumeChanged;
            SettingsEvents.ModelMusicVolumeChanged += SettingsEvents_ModelMusicVolumeChanged;
            SettingsEvents.ModelBrightnessVolumeChanged += SettingsEvents_ModelBrightnessSliderChanged;
            SettingsEvents.ModelContrastVolumeChanged += SettingsEvents_ModelContrastSliderChanged;
            SettingsEvents.ModelResolutionValueChanged += SettingsEvents_ModelResolutionValueChanged;
            SettingsEvents.ModelDifficultyValueChanged += SettingsEvents_ModelDifficultyValueChanged;
        }

        // Event unsubscriptions
        private void OnDisable()
        {
            SettingsEvents.MasterSliderChanged -= SettingsEvents_MasterSliderChanged;
            SettingsEvents.SFXSliderChanged -= SettingsEvents_SFXSliderChanged;
            SettingsEvents.MusicSliderChanged -= SettingsEvents_MusicSliderChanged;
            SettingsEvents.BrightnessSliderChanged -= SettingsEvents_BrightnessSliderChanged;
            SettingsEvents.ContrastSliderChanged -= SettingsEvents_ContrastSliderChanged;
            SettingsEvents.ResolutionChanged -= SettingsEvents_ResolutionValueChanged;
            SettingsEvents.DifficultyChanged -= SettingsEvents_DifficultyValueChanged;

            SettingsEvents.ModelMasterVolumeChanged -= SettingsEvents_ModelMasterVolumeChanged;
            SettingsEvents.ModelSFXVolumeChanged -= SettingsEvents_ModelSFXVolumeChanged;
            SettingsEvents.ModelMusicVolumeChanged -= SettingsEvents_ModelMusicVolumeChanged;
            SettingsEvents.ModelResolutionValueChanged -= SettingsEvents_ModelResolutionValueChanged;
            SettingsEvents.ModelDifficultyValueChanged -= SettingsEvents_ModelDifficultyValueChanged;
        }

        private void Start()
        {
            Initialize();
        }

        // Set defaults
        private void Initialize()
        {
            m_AudioSettingsSO = Resources.Load<AudioSettingsSO>("Audio/AudioSettings_Data");
            m_VideoSettingsSO = Resources.Load<VideoSettingsSO>("VideoGraphics/VideoSettings_Data");
            m_GamePlaySO = Resources.Load<GamePlaySO>("GamePlay/GamePlay");

            // Update sliders with the default values from the AudioSettingsSO
            
            float masterVolume = m_AudioSettingsSO.MasterVolume * 100f;
            float sfxVolume = m_AudioSettingsSO.SoundEffectsVolume * 100f;
            float musicVolume = m_AudioSettingsSO.MusicVolume * 100f;
            float valueBrightness = m_VideoSettingsSO.valueBrightness *100f ;
            float valueContrast = m_VideoSettingsSO.valueContrast*100f;
            int pointerRes = m_VideoSettingsSO.pointerRes;
            int pointerDiff = m_VideoSettingsSO.pointerDiff;
            bool gamePlaySave = m_GamePlaySO.NextLevel;

            // Notify the View of default values from the Model
            SettingsEvents.MasterSliderSet?.Invoke(masterVolume);
            SettingsEvents.SFXSliderSet?.Invoke(sfxVolume);
            SettingsEvents.MusicSliderSet?.Invoke(musicVolume);
            SettingsEvents.BrightnessSliderSet?.Invoke(valueBrightness);
            SettingsEvents.ContrastSliderSet?.Invoke(valueContrast);
            SettingsEvents.ResolutionSet?.Invoke(pointerRes);
            SettingsEvents.DifficultySet?.Invoke(pointerDiff);
            GameEvents.NextLevel?.Invoke(gamePlaySave);
        }

        // View event handlers

        // Method to handle master volume slider changes
        public void SettingsEvents_MasterSliderChanged(float sliderValue)
        {
            // Calculate volume based on slider value (assuming sliderValue ranges from 0 to 100)
            float volume = sliderValue / 100f;
            // Invoke event with the calculated volume
            SettingsEvents.MasterVolumeChanged?.Invoke(volume);
        }

        // Method to handle SFX volume slider changes
        public void SettingsEvents_SFXSliderChanged(float sliderValue)
        {
            // Calculate volume based on slider value (assuming sliderValue ranges from 0 to 100)
            float volume = sliderValue / 100f;
            // Invoke event with the calculated volume
            SettingsEvents.SFXVolumeChanged?.Invoke(volume);
        }

        // Method to handle music volume slider changes
        public void SettingsEvents_MusicSliderChanged(float sliderValue)
        {
            // Calculate volume based on slider value (assuming sliderValue ranges from 0 to 100)
            float volume = sliderValue / 100f;
            // Invoke event with the calculated volume
            SettingsEvents.MusicVolumeChanged?.Invoke(volume);
        }

        // Method to handle brightness slider changes
        public void SettingsEvents_BrightnessSliderChanged(float sliderValue)
        {
            // Calculate brightness based on slider value (assuming sliderValue ranges from 0 to 100)
            float brightness = sliderValue / 100f;
            // Invoke event with the calculated brightness
            SettingsEvents.BrightnessVolumeChanged?.Invoke(brightness);
        }

        // Method to handle contrast slider changes
        public void SettingsEvents_ContrastSliderChanged(float sliderValue)
        {
            // Calculate contrast based on slider value (assuming sliderValue ranges from 0 to 100)
            float contrast = sliderValue / 100f;
            // Invoke event with the calculated contrast
            SettingsEvents.ContrastVolumeChanged?.Invoke(contrast);
        }

        // Method to handle resolution dropdown changes
        public void SettingsEvents_ResolutionValueChanged(int resValue)
        {
            // Invoke event with the selected resolution value
            SettingsEvents.ResolutionValueChanged?.Invoke(resValue); 
        }

        // Method to handle difficulty dropdown changes
        public void SettingsEvents_DifficultyValueChanged(int diffValue)
        {
            // Invoke event with the selected difficulty value
            SettingsEvents.DifficultyValueChanged?.Invoke(diffValue);
        }

        // Model event handlers (response if Model data externally modified,
        // e.g. loading preferences from disk)

        // Private method to handle model event for master volume changes
        private void SettingsEvents_ModelMasterVolumeChanged(float volume)
        {
            // Process the master volume change from the Model
            SettingsEvents.MasterSliderSet?.Invoke(volume);
        }
        
        // Private method to handle model event for sfx volume changes
        private void SettingsEvents_ModelSFXVolumeChanged(float volume)
        {
            // Process the SFX volume change from the Model
            SettingsEvents.SFXSliderSet?.Invoke(volume);
        }
        
        // Private method to handle model event for music volume changes
        private void SettingsEvents_ModelMusicVolumeChanged(float volume)
        {
            // Process the music volume change from the Model
            SettingsEvents.MusicSliderSet?.Invoke(volume);
        }

        // Private method to handle model event for brightness slider changes
        private void SettingsEvents_ModelBrightnessSliderChanged(float brightness)
        {
            // Invoke event to set brightness slider in the UI
            SettingsEvents.BrightnessSliderSet?.Invoke(brightness);
        }

        // Private method to handle model event for contrast slider changes
        private void SettingsEvents_ModelContrastSliderChanged(float contrast)
        {
            // Invoke event to set contrast slider in the UI
            SettingsEvents.ContrastSliderSet?.Invoke(contrast);
        }

        // Private method to handle model event for resolution changes
        private void SettingsEvents_ModelResolutionValueChanged(int resolutionValue)
        {
            // Invoke event to set resolution dropdown in the UI
            SettingsEvents.ResolutionSet?.Invoke(resolutionValue);
        }

        // Private method to handle model event for difficulty changes
        private void SettingsEvents_ModelDifficultyValueChanged(int difficultyValue)
        {
            // Invoke event to set difficulty dropdown in the UI
            SettingsEvents.DifficultySet?.Invoke(difficultyValue);
        }

    }
}
