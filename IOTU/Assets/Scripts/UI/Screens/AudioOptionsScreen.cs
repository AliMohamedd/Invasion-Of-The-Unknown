using System;
using UnityEngine.UIElements;

namespace IOTU
{
    /// <summary>
    /// Represents the audio settings screen (AudioSettingsScreen.uxml), handles UI interaction and updates. The Sliders display
    /// values on their corresponding Labels with some custom behavior.
    /// </summary>
    public class AudioOptionsScreen : UIScreen
    {
        // UI element references
        Slider m_MasterVolumeSlider;
        Slider m_MusicVolumeSlider;
        Slider m_SFXVolumeSlider;

        Button m_VideoButton;
        Button m_BackButton;

        public AudioOptionsScreen(VisualElement rootElement): base(rootElement)
        {
            SetVisualElements();
            RegisterCallbacks();
            SubscribeToEvents();

            m_IsTransparent = true;
            SettingsEvents.SettingsInitialized?.Invoke();
        }

        public override void Disable()
        {
            base.Disable();
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            // Subscribe to "*Set" events from Presenter Initialization
            SettingsEvents.MasterSliderSet += MasterVolumeSetHandler;
            SettingsEvents.SFXSliderSet += SFXVolumeSetHandler;
            SettingsEvents.MusicSliderSet += MusicVolumeSetHandler;
        }

        private void UnsubscribeFromEvents()
        {
            // Unsubscribe from "*Set" events
            SettingsEvents.MasterSliderSet -= MasterVolumeSetHandler;
            SettingsEvents.SFXSliderSet -= SFXVolumeSetHandler;
            SettingsEvents.MusicSliderSet -= MusicVolumeSetHandler;
        }

        // Find and set references to UI elements
        private void SetVisualElements()
        {
            m_MasterVolumeSlider = m_RootElement.Q<Slider>("settings__master-volume-slider");
            m_MusicVolumeSlider = m_RootElement.Q<Slider>("settings__music-volume-slider");
            m_SFXVolumeSlider = m_RootElement.Q<Slider>("settings__sfx-volume-slider");

            m_VideoButton = m_RootElement.Q<Button>("video-button");
            m_BackButton = m_RootElement.Q<Button>("back-button");
        }

        // Register callbacks for slider value changes; Event Registry automatically unregisters callbacks
        // on disable
        private void RegisterCallbacks()
        {
            m_EventRegistry.RegisterValueChangedCallback<float>(m_MasterVolumeSlider, MasterVolumeChangeHandler);
            m_EventRegistry.RegisterValueChangedCallback<float>(m_MusicVolumeSlider, MusicVolumeChangeHandler);
            m_EventRegistry.RegisterValueChangedCallback<float>(m_SFXVolumeSlider, SFXVolumeChangeHandler);

            m_EventRegistry.RegisterCallback<ClickEvent>(m_BackButton, evt => CloseWindow());
            m_EventRegistry.RegisterCallback<ClickEvent>(m_VideoButton, evt => UIEvents.VideoOptionsScreenShown?.Invoke());
        }

        // Notify the Presenter

        // Update master volume label text / notify the presenter when slider changes
        private void MasterVolumeChangeHandler(float newValue)
        {
            //m_MasterVolumeLabel.text = newValue.ToString("F0");
            SettingsEvents.MasterSliderChanged?.Invoke(newValue);
        }

        // Update SFX volume label text / notify the presenter when slider changes
        private void SFXVolumeChangeHandler(float newValue)
        {
            //m_SFXVolumeLabel.text = newValue.ToString("F0");
            SettingsEvents.SFXSliderChanged?.Invoke(newValue);
        }

        // Update music volume label text / notify the presenter when slider changes
        private void MusicVolumeChangeHandler(float newValue)
        {
            //m_MusicVolumeLabel.text = newValue.ToString("F0");
            SettingsEvents.MusicSliderChanged?.Invoke(newValue);
        }

        // Receive notifications from the Presenter

        // Update master volume values from Presenter Initializtion
        private void MasterVolumeSetHandler(float volume)
        {
            m_MasterVolumeSlider.value = volume;
            //m_MasterVolumeLabel.text = volume.ToString("F0");
        }

        // Update SFX volume values from Presenter Initializtion
        private void SFXVolumeSetHandler(float volume)
        {
            m_SFXVolumeSlider.value = volume;
            //m_SFXVolumeLabel.text = volume.ToString("F0");
        }

        // Update music volume values from Presenter Initializtion
        private void MusicVolumeSetHandler(float volume)
        {
            m_MusicVolumeSlider.value = volume;
            //m_MusicVolumeLabel.text = volume.ToString("F0");
        }

        private void CloseWindow()
        {
            UIEvents.ScreenClosed?.Invoke();
        }
    }
}

