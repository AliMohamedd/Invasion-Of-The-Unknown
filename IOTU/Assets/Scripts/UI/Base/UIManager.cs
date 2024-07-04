using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace IOTU
{
    /// <summary>
    /// The UI Manager manages the UI screens (View base class) using GameEvents paired
    /// to each View screen. A stack maintains a history of previously shown screens, so
    /// the UI Manager can "go back" until it reaches the default UI screen, the home screen.
    ///
    /// To add a new UIScreen under the UIManager's management:
    ///    -Define a new UIScreen field
    ///    -Create a new instance of that screen in Initialize (e.g. new SplashScreen(root.Q<VisualElement>("splash__container"));
    ///    -Register the UIScreen in the RegisterScreens method
    ///    -Subscribe/unsubscribe from the appropriate UIEvent to show the screen
    ///
    /// Alternatively, use Reflection to add the UIScreen to the RegisterScreens method
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Tooltip("Required UI Document")]
        [SerializeField] UIDocument m_Document;

        // Load screen with a progress bar, displays after application launch
        UIScreen m_SplashScreen;
        UIScreen m_StartScreen;

        // First screen that loads after the splash screen, shown only when firs start and no save in game
        UIScreen m_FirsMainMenuScreen;

        // Primary modal screen (e.g. main menu)
        UIScreen m_MainMenuScreen;

        // The screen of audio settings
        UIScreen m_AudioOptionsScreen;

        // The screen of video settings
        UIScreen m_VideoOptionsScreens;

        // The main gameplay screen 
        UIScreen m_GamePlayScreen;

        // In-game screen that allows the user to quit or continue
        UIScreen m_PauseScreen;

        // The screen of inventory
        UIScreen m_InventoryScreen;

        // The screen of losing
        UIScreen m_LoseScreen;

        // The currently active UIScreen
        UIScreen m_CurrentScreen;

        // A stack of previously displayed UIScreens
        Stack<UIScreen> m_History = new Stack<UIScreen>();

        // A list of all Views to show/hide
        List<UIScreen> m_Screens = new List<UIScreen>();



        public UIScreen CurrentScreen => m_CurrentScreen;
        public UIDocument Document => m_Document;

        // Register event listeners to game events
        private void OnEnable()
        {
            SubscribeToEvents();

            // Because non-MonoBehaviours can't run coroutines, the Coroutines helper utility allows us to
            // designate a MonoBehaviour to manage starting/stopping coroutines
            Coroutines.Initialize(this);

            Initialize();
        }

        // Unregister the listeners to prevent errors
        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            // Wait for the SplashScreen to finish loading then load the StartScreen
            SceneEvents.PreloadCompleted += SceneEvents_PreloadCompleted;
            // Pair GameEvents with methods to Show each screen
            UIEvents.SplashScreenShown += UIEvents_SplashScreenShown;
            UIEvents.FirstMainMenuScreenShown += UIEvents_FirstMainMenuScreenShown;
            UIEvents.MainMenuScreenShown += UIEvents_MainMenuScreenShown;
            UIEvents.AudioOptionsScreenShown += UIEvents_AudioOptionsScreenShown;
            UIEvents.VideoOptionsScreenShown += UIEvents_VideoOptionsScreenShown;
            UIEvents.GamePlayScreenShown += UIEvents_GameScreenShown;
            UIEvents.PauseScreenShown += UIEvents_PauseScreenShown;
            UIEvents.InventoryScreenShown += UIEvents_InventoryScreenShown;
            UIEvents.LoseScreenShown += UIEvents_LoseScreenShown;
            UIEvents.ScreenClosed += UIEvents_ScreenClosed;
        }

        private void UnsubscribeFromEvents()
        {
            SceneEvents.PreloadCompleted -= SceneEvents_PreloadCompleted;

            UIEvents.SplashScreenShown -= UIEvents_SplashScreenShown;
            UIEvents.FirstMainMenuScreenShown -= UIEvents_FirstMainMenuScreenShown;
            UIEvents.MainMenuScreenShown -= UIEvents_MainMenuScreenShown;
            UIEvents.AudioOptionsScreenShown -= UIEvents_AudioOptionsScreenShown;
            UIEvents.VideoOptionsScreenShown -= UIEvents_VideoOptionsScreenShown;
            UIEvents.GamePlayScreenShown -= UIEvents_GameScreenShown;
            UIEvents.PauseScreenShown -= UIEvents_PauseScreenShown;
            UIEvents.InventoryScreenShown -= UIEvents_InventoryScreenShown;
            UIEvents.LoseScreenShown -= UIEvents_LoseScreenShown;
            UIEvents.ScreenClosed -= UIEvents_ScreenClosed;
        }

        // Event-handling methods

        // Show the SplashScreen and don't keep in history
        private void UIEvents_SplashScreenShown()
        {
            Show(m_SplashScreen, false);
        }

        // Show the StartScreen but don't keep it in the history

         private void SceneEvents_PreloadCompleted()
        {
            Show(m_StartScreen, false);
        }
        

        // Clear the History and make the HomeScreen (MainMenu) the only View
        public void UIEvents_FirstMainMenuScreenShown()
        {
            m_CurrentScreen = m_FirsMainMenuScreen;

            HideScreens();
            m_History.Push(m_FirsMainMenuScreen);
            m_FirsMainMenuScreen.Show();
        }
        public void UIEvents_MainMenuScreenShown()
        {
            m_CurrentScreen = m_MainMenuScreen;

            HideScreens();
            m_History.Push(m_MainMenuScreen);
            m_MainMenuScreen.Show();
        }

        private void UIEvents_AudioOptionsScreenShown()
        {
            Show(m_AudioOptionsScreen, false);
        }

         private void UIEvents_VideoOptionsScreenShown()
        {
            Show(m_VideoOptionsScreens, false);
        }

        private void UIEvents_GameScreenShown()
        {
            Show(m_GamePlayScreen);
            
        }
        private void UIEvents_PauseScreenShown()
        {
            Show(m_PauseScreen, false);
            m_History.Pop();
        }

         private void UIEvents_InventoryScreenShown()
        {
            Show(m_InventoryScreen, false);
            m_History.Pop();
        }

         private void UIEvents_LoseScreenShown()
        {
            Show(m_LoseScreen, false);
            m_History.Pop();
        }

        // Remove the top UI screen from the stack and make that active (i.e., go back one screen)
        public void UIEvents_ScreenClosed()
        {
            if (m_History.Count != 0)
            {
                Show(m_History.Peek(), false);
            }
        }


        // Methods

        // Clears history and hides all Views except the Start Screen
        private void Initialize()
        {
            NullRefChecker.Validate(this);

            VisualElement root = m_Document.rootVisualElement;

            m_SplashScreen = new SplashScreen(root.Q<VisualElement>("splash__container"));
            m_StartScreen= new StartScreen(root.Q<VisualElement>("start__container"));
            m_FirsMainMenuScreen = new FirstMainMenuScreen(root.Q<VisualElement>("first-menu__container"));
            m_MainMenuScreen = new MainMenuScreen(root.Q<VisualElement>("menu__container"));
            m_AudioOptionsScreen = new AudioOptionsScreen(root.Q<VisualElement>("audio-options__container"));
            m_VideoOptionsScreens = new VideoOptionsScreens(root.Q<VisualElement>("video-options__container"));
            m_GamePlayScreen = new GamePlayScreen(root.Q<VisualElement>("game-play__container"));
            m_PauseScreen = new PauseScreen(root.Q<VisualElement>("pause__container"));
            m_InventoryScreen = new InventoryScreen(root.Q<VisualElement>("inventory__container"));
            m_LoseScreen = new LoseScreen(root.Q<VisualElement>("lose__container"));

            // Notify the GameController the UIScreen for LevelSelection has been setup
            //LevelSelectionEvents.Initialized?.Invoke(m_LevelSelectionScreen as LevelSelectionScreen);

            RegisterScreens();
            HideScreens();
        }

        // Store each UIScreen into a master list so we can hide all of them easily.
        private void RegisterScreens()
        {
            m_Screens = new List<UIScreen>
            {
                m_SplashScreen,
                m_StartScreen,
                m_FirsMainMenuScreen,
                m_MainMenuScreen,
                m_AudioOptionsScreen,
                m_VideoOptionsScreens,
                m_GamePlayScreen,
                m_PauseScreen,
                m_InventoryScreen,
                m_LoseScreen
            };
        }

        // Clear history and hide all Views
        private void HideScreens()
        {
            m_History.Clear();

            foreach (UIScreen screen in m_Screens)
            {
                screen.Hide();
            }
        }

        // Finds the first registered UI View of the specified type T
        public T GetScreen<T>() where T : UIScreen
        {
            foreach (var screen in m_Screens)
            {
                if (screen is T typeOfScreen)
                {
                    return typeOfScreen;
                }
            }
            return null;
        }

        // Shows a View of a specific type T, with the option to add it
        // to the history stack
        public void Show<T>(bool keepInHistory = true) where T : UIScreen
        {
            foreach (var screen in m_Screens)
            {
                if (screen is T)
                {
                    Show(screen, keepInHistory);
                    break;
                }
            }
        }

        // 
        public void Show(UIScreen screen, bool keepInHistory = true)
        {
            if (screen == null)
                return;

            if (m_CurrentScreen != null)
            {
                
                m_CurrentScreen.Hide();

                if (keepInHistory)
                {
                    m_History.Push(m_CurrentScreen);
                }
            }

            screen.Show();
            m_CurrentScreen = screen;
        }

        // Shows a UIScreen with the keepInHistory always enabled
        public void Show(UIScreen screen)
        {
            Show(screen, true);
        }
    }
}

