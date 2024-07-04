using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTU
{
    /// <summary>
    /// Public static delegates to manage settings changes (note these are "events" in the conceptual sense
    /// and not the strict C# sense).
    /// </summary>
    public static class SettingsEvents 
    {
        // Notify set up script that UI is ready
        public static Action SettingsInitialized;

        // Presenter --> View: update UI sliders
        public static Action<float> MasterSliderSet;
        public static Action<float> SFXSliderSet;
        public static Action<float> MusicSliderSet;
        public static Action<float> BrightnessSliderSet;
        public static Action<float> ContrastSliderSet;
        public static Action<int> ResolutionSet;
        public static Action<int> DifficultySet;

        // View -> Presenter: update UI sliders
        public static Action<float> MasterSliderChanged;
        public static Action<float> SFXSliderChanged;
        public static Action<float> MusicSliderChanged;
        public static Action<float> BrightnessSliderChanged;
        public static Action<float> ContrastSliderChanged;
        public static Action<int> ResolutionChanged;
        public static Action<int> DifficultyChanged;

        // Presenter -> Model: update volume settings
        public static Action<float> MasterVolumeChanged;
        public static Action<float> SFXVolumeChanged;
        public static Action<float> MusicVolumeChanged;
        public static Action<float> BrightnessVolumeChanged;
        public static Action<float> ContrastVolumeChanged;
        public static Action<int> ResolutionValueChanged;
        public static Action<int> DifficultyValueChanged;

        // Model -> Presenter: model values changed (e.g. loading saved values)
        public static Action<float> ModelMasterVolumeChanged;
        public static Action<float> ModelSFXVolumeChanged;
        public static Action<float> ModelMusicVolumeChanged;
        public static Action<float> ModelBrightnessVolumeChanged;
        public static Action<float> ModelContrastVolumeChanged;
        public static Action<int> ModelResolutionValueChanged;
        public static Action<int> ModelDifficultyValueChanged;

    }
}
