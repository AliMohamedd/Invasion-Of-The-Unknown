using UnityEngine.UIElements;
using UnityEngine;

namespace IOTU
{
    /// <summary>
    /// Represents the screen for adjusting video settings such as brightness, contrast, resolution, and difficulty.
    /// </summary>
    public class VideoOptionsScreens : UIScreen
    {
        // UI element references
        Slider m_BrightnessSlider;
        Slider m_ContrastSlider;
        Label m_CurrentDiffLabel;
        Label m_CurrentResLabel;
        Button m_AudioButton;
        Button m_BackButton;
        Button m_SwitchResRight;
        Button m_SwitchResLeft;
        Button m_SwitchDiffRight;
        Button m_SwitchDiffLeft;
        
        // Arrays for resolution options and difficulty levels
        int[] resolutionX = {1920, 1024, 1280, 1280, 1600};
        int[] resolutionY = {1080, 768, 720, 960, 1200};
        int pointer;

        string[] difficulty = { "Easy", "Medium", "Hard"};
        int pointerDiff;

        /// <summary>
        /// Initializes the Video Options screen by setting up visual elements, registering callbacks, and subscribing to events.
        /// </summary>
        /// <param name="rootElement">The root VisualElement to which this screen is attached.</param>
        public VideoOptionsScreens(VisualElement rootElement): base(rootElement)
        {
            SetVisualElements();
            RegisterCallbacks();
            SubscribeToEvents();

            m_IsTransparent = true;
            SettingsEvents.SettingsInitialized?.Invoke();
        }

        // Disables the screen and unsubscribes from events when the screen is disabled.
        public override void Disable()
        {
            base.Disable();
            UnsubscribeFromEvents();
        }

        // Subscribes to events when the screen is enabled.
        private void SubscribeToEvents()
        {
            // Subscribe to settings change events
            SettingsEvents.BrightnessSliderSet += BrightnessSetHandler;
            SettingsEvents.ContrastSliderSet += ContrastSetHandler;
            SettingsEvents.ResolutionSet += ResolutionSetHandler;
            SettingsEvents.DifficultySet += DifficultiSetHandler;
        }

        // Unsubscribes from events when the screen is disabled.
        private void UnsubscribeFromEvents()
        {
            // Unsubscribe from settings change events
            SettingsEvents.BrightnessSliderSet -= BrightnessSetHandler;
            SettingsEvents.ContrastSliderSet -= ContrastSetHandler;
            SettingsEvents.ResolutionSet -= ResolutionSetHandler;
            SettingsEvents.DifficultySet -= DifficultiSetHandler;
        }

        // Sets up references to UI elements by querying them from the root VisualElement.
        private void SetVisualElements()
        {
            m_BrightnessSlider = m_RootElement.Q<Slider>("settings__brightness-volume-slider");
            m_ContrastSlider = m_RootElement.Q<Slider>("settings__contrast-volume-slider");
           
            m_AudioButton = m_RootElement.Q<Button>("audio-button");
            m_BackButton = m_RootElement.Q<Button>("back-button");

            m_SwitchResRight = m_RootElement.Q<Button>("right-switch-res");
            m_SwitchResLeft = m_RootElement.Q<Button>("left-switch-res");
            m_CurrentResLabel= m_RootElement.Q<Label>("current-resolution-label");

            m_SwitchDiffRight = m_RootElement.Q<Button>("right-switch-diff");
            m_SwitchDiffLeft = m_RootElement.Q<Button>("left-switch-diff");
            m_CurrentDiffLabel = m_RootElement.Q<Label>("current-diff-label");
        }

        // Registers callbacks for UI elements to handle user interactions.
        private void RegisterCallbacks()
        {
            // Register callbacks for slider value changes
            m_EventRegistry.RegisterValueChangedCallback<float>(m_BrightnessSlider, BrightnessChangeHandler);
            m_EventRegistry.RegisterValueChangedCallback<float>(m_ContrastSlider, ContrastChangeHandler);

            // Register callbacks for button clicks
            m_EventRegistry.RegisterCallback<ClickEvent>(m_BackButton, evt => CloseWindow()); 
            m_EventRegistry.RegisterCallback<ClickEvent>(m_AudioButton, evt => UIEvents.AudioOptionsScreenShown?.Invoke());
            m_EventRegistry.RegisterCallback<ClickEvent>(m_SwitchResRight, evt => RightSwitchResolutionChangeHandler());
            m_EventRegistry.RegisterCallback<ClickEvent>(m_SwitchResLeft, evt => LeftSwitchResolutionChangeHandler());
            m_EventRegistry.RegisterCallback<ClickEvent>(m_SwitchDiffRight, evt => RightSwitchAiDiffChangeHandler());
            m_EventRegistry.RegisterCallback<ClickEvent>(m_SwitchDiffLeft, evt => LeftSwitchAiDiffChangeHandler());
        }

        // Handles the change in brightness slider value and invokes corresponding event.
        private void BrightnessChangeHandler(float newValue)
        {
            SettingsEvents.BrightnessSliderChanged?.Invoke(newValue);
        }

        // Handles the change in contrast slider value and invokes corresponding event.
        private void ContrastChangeHandler(float newValue)
        {
            SettingsEvents.ContrastSliderChanged?.Invoke(newValue);
        }

        // Handles the action when switching to the next resolution option.
        private void RightSwitchResolutionChangeHandler()
        {
            if (pointer < 4) pointer++;
            else pointer = 0;

            Screen.SetResolution(resolutionX[pointer], resolutionY[pointer], true);
            m_CurrentResLabel.text = resolutionX[pointer] + " x " + resolutionY[pointer];

            SettingsEvents.ResolutionChanged?.Invoke(pointer);
        }

        // Handles the action when switching to the previous resolution option.
        private void LeftSwitchResolutionChangeHandler()
        {
            if (pointer > 0) pointer--;
            else pointer = 4;

            Screen.SetResolution(resolutionX[pointer], resolutionY[pointer], true);
            m_CurrentResLabel.text = resolutionX[pointer] + " x " + resolutionY[pointer];

            SettingsEvents.ResolutionChanged?.Invoke(pointer);
        }

        // Handles the action when switching to the next difficulty level.
        private void RightSwitchAiDiffChangeHandler()
        {
            if (pointerDiff < 2) pointerDiff++;
            else pointerDiff = 0;

            m_CurrentDiffLabel.text = difficulty[pointerDiff];
            SettingsEvents.DifficultyChanged?.Invoke(pointerDiff);
        }

        // Handles the action when switching to the previous difficulty level.
        private void LeftSwitchAiDiffChangeHandler()
        {
            if (pointerDiff > 0) pointerDiff--;
            else pointerDiff = 2;

            m_CurrentDiffLabel.text = difficulty[pointerDiff];
            SettingsEvents.DifficultyChanged?.Invoke(pointerDiff);
        }

        // Handles the notification when the brightness value is set externally.
        private void BrightnessSetHandler(float value)
        {
            m_BrightnessSlider.value = value;
        }

        // Handles the notification when the contrast value is set externally.
        private void ContrastSetHandler(float value)
        {
            m_ContrastSlider.value = value;
        }

        // Handles the notification when the resolution setting is updated externally.
        private void ResolutionSetHandler(int index)
        {
            pointer = index;
            m_CurrentResLabel.text = resolutionX[pointer] + " x " + resolutionY[pointer];
        }

        // Handles the notification when the difficulty setting is updated externally.
        private void DifficultiSetHandler(int index)
        {
            pointerDiff = index;
            m_CurrentDiffLabel.text = difficulty[pointerDiff];
        }
        
        // Closes the window and triggers the screen closed event.
        private void CloseWindow()
        {
            UIEvents.ScreenClosed?.Invoke();
        }
    }
}
