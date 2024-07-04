using UnityEngine.UIElements;
using UnityEngine;

namespace IOTU
{
    /// <summary>
    /// Represents the pause screen with buttons to return to main menu or exit the game.
    /// </summary>
    public class PauseScreen : UIScreen
    {
        Button m_MainMenuButton; // Button to return to the main menu
        Button m_ExitButton;     // Button to exit the game

        bool m_First;            // Flag indicating if it's the first main menu

        /// <summary>
        /// Initializes the pause screen by setting up visual elements, registering callbacks, and subscribing to events.
        /// </summary>
        /// <param name="parentElement">The parent VisualElement to which this screen is attached.</param>
        public PauseScreen(VisualElement parentElement) : base(parentElement)
        {
            SetVisualElements();
            RegisterCallbacks();
            SubscribeToEvents();
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
            GameEvents.NextLevel += FirstChanged;
        }

        // Unsubscribes from events when the screen is disabled.
        private void UnsubscribeFromEvents()
        {
            GameEvents.NextLevel -= FirstChanged;
        }

        // Sets up references to UI elements by querying them from the root VisualElement.
        private void SetVisualElements()
        {
            m_MainMenuButton = m_RootElement.Q<Button>("start-menu-button");
            m_ExitButton = m_RootElement.Q<Button>("exit_button");
        }

        // Registers click event callbacks for each button to handle user interactions.
        private void RegisterCallbacks()
        {
            m_EventRegistry.RegisterCallback<ClickEvent>(m_MainMenuButton, evt => BackToMainMenu());
            m_EventRegistry.RegisterCallback<ClickEvent>(m_ExitButton, evt => Application.Quit());
        }

        // Handles the action when returning to the main menu based on the `First` flag.
        private void BackToMainMenu()
        {
            if (m_First)
            {
                UIEvents.MainMenuScreenShown?.Invoke();
            }
            else
            {
                UIEvents.FirstMainMenuScreenShown?.Invoke();
            }

            GameEvents.GameUI?.Invoke();
            SceneEvents.LastSceneUnloaded?.Invoke();
        }

        // Updates the `First` flag based on the event value.
        private void FirstChanged(bool val)
        {
            m_First = val;
        }
    }
}
