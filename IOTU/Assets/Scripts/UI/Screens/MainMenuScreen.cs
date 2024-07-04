using UnityEngine.UIElements;
using UnityEngine;

namespace IOTU
{
    /// <summary>
    /// This class represents the main menu screen to handle buttons such as Countinue, start, exit, etc...
    /// </summary>
    public class MainMenuScreen : UIScreen
    {
        Button m_ContinueButton;    // Button to continue the game
        Button m_NewGameButton;     // Button to start a new game
        Button m_OptionsButton;     // Button to open settings
        Button m_ExitButton;        // Button to exit the game

        /// <summary>
        /// Initializes the main menu screen by setting up visual elements and registering button callbacks.
        /// </summary>
        /// <param name="parentElement">The parent VisualElement to which this screen is attached.</param>
        public MainMenuScreen(VisualElement parentElement) : base(parentElement)
        {
            SetVisualElements();
            RegisterCallbacks();
        }

        // Sets up references to UI elements by querying them from the root VisualElement.
        private void SetVisualElements()
        {
            m_ContinueButton = m_RootElement.Q<Button>("menu__button-continue");
            m_NewGameButton = m_RootElement.Q<Button>("menu__button-new-game");
            m_OptionsButton = m_RootElement.Q<Button>("menu__button-options");
            m_ExitButton = m_RootElement.Q<Button>("menu__button-exit");
        }

        // Registers click event callbacks for each button to handle user interactions.
        private void RegisterCallbacks()
        {
            m_EventRegistry.RegisterCallback<ClickEvent>(m_ContinueButton, evt => ContinueLoadScene());
            m_EventRegistry.RegisterCallback<ClickEvent>(m_NewGameButton, evt => LoadScene("Assets/Scenes/Dream Layout (Level 1).unity"));
            m_EventRegistry.RegisterCallback<ClickEvent>(m_OptionsButton, evt => UIEvents.AudioOptionsScreenShown?.Invoke());
            m_EventRegistry.RegisterCallback<ClickEvent>(m_ExitButton, evt => Application.Quit());
        }

        // Loads a scene by invoking events related to starting a new game.
        private void LoadScene(string scenePath)
        {
            GameEvents.NextLevel?.Invoke(false);
            UIEvents.GamePlayScreenShown?.Invoke();
            SceneEvents.LoadSceneByPath?.Invoke(scenePath);
            GameEvents.GameStarted?.Invoke();
        }

        // Continues the game by invoking events related to continuing from a saved state.
        private void ContinueLoadScene()
        {
            UIEvents.GamePlayScreenShown?.Invoke();
            SceneEvents.SceneIndexLoaded?.Invoke(2);
            GameEvents.GameStarted?.Invoke();
        }
    }
}
