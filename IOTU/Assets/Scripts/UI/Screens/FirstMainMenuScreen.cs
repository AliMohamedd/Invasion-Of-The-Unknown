using UnityEngine.UIElements;
using UnityEngine;
using System;

namespace IOTU
{
    /// <summary>
    /// Represents the first main menu screen, handling UI elements such as start, options, and exit buttons.
    /// </summary>
    public class FirstMainMenuScreen : UIScreen
    {
        Button m_StartButton;        // Button to start the game
        Button m_OptionsButton;      // Button to open settings
        Button m_ExitButton;         // Button to close the game

        /// <summary>
        /// Constructor to initialize the first main menu screen with its parent VisualElement.
        /// </summary>
        /// <param name="parentElement">The parent VisualElement to attach UI elements to.</param>
        public FirstMainMenuScreen(VisualElement parentElement) : base(parentElement)
        {
            SetVisualElements();
            RegisterCallbacks();
        }

        // Sets references to UI elements by querying the root VisualElement.
        private void SetVisualElements()
        {
            m_StartButton = m_RootElement.Q<Button>("menu__button-start");
            m_OptionsButton = m_RootElement.Q<Button>("menu__button-options");
            m_ExitButton = m_RootElement.Q<Button>("menu__button-exit");
        }

        // Registers callbacks for button click events.
        private void RegisterCallbacks()
        {
            // Register callbacks for button click events
            m_EventRegistry.RegisterCallback<ClickEvent>(m_StartButton, evt => LoadScenePath("Assets/Scenes/Dream Layout (Level 1).unity"));
            m_EventRegistry.RegisterCallback<ClickEvent>(m_OptionsButton, evt => UIEvents.AudioOptionsScreenShown?.Invoke());
            m_EventRegistry.RegisterCallback<ClickEvent>(m_ExitButton, evt => Application.Quit());
        }

        // Loads a scene by its path and invokes relevant game and UI events.
        private void LoadScenePath(string scenePath)
        {
            GameEvents.NextLevel?.Invoke(false);
            UIEvents.GamePlayScreenShown?.Invoke();
            SceneEvents.LoadSceneByPath?.Invoke(scenePath);
            GameEvents.GameStarted?.Invoke();
        }
    }
}
