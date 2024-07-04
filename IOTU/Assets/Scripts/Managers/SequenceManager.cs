using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IOTU
{
    /// <summary>
    /// A SequenceManager controls the overall flow of the application using a state machine.
    /// 
    /// Use this class to define how each State will transition to the next. Each state can
    /// transition to the next state when receiving an event or reaching a specific condition.
    ///
    /// Note: this class currently is only used for demonstration/diagnostic purposes. You can use
    ///       the start and end of each state to instantiate GameObjects/play effects. Another simple
    ///       state machine for UI screens (UIManager) actually drives most of the quiz gameplay.
    /// 
    /// </summary>

    public class SequenceManager : MonoBehaviour
    {
        // Inspector fields
        [Header("Preload (Splash Screen)")]
        [Tooltip("Prefab assets that load first. These can include level management Prefabs or textures, sounds, etc.")]
        [SerializeField] GameObject[] m_PreloadedAssets;

        [Tooltip("Time in seconds to show Splash Screen")]
        [SerializeField] float m_LoadScreenTime = 5f;

        [Space(10)]
        [Tooltip("Debug state changes in the console")]
        [SerializeField] bool m_Debug;

        StateMachine m_StateMachine = new StateMachine();

        // Define all States here
        IState m_SplashScreenState;     // Startup and load assets, show a splash screen
        IState m_StartScreenState;

        IState m_FirstMainMenuState;    // Show the first main menu screens
        IState m_MainMenuState;         // Show the main menu screens
        IState m_SettingsAState;        // Show the Settings Audio Screen while in the Main Menu
        IState m_SettingsVState;        // Show the Settings Video Screen while in the Main Menu
        IState m_NewPlayState;          // Play the game
        IState m_ContinuePlayState;     // Continue the game
        IState m_PauseState;            // Pause the game during gameplay
        IState m_InventoryState;        // show the inventory screen

        IState m_GameWinState;          // Show the win screen
        IState m_GameLoseState;         // Show the lose screen

        // Access to the StateMachine's CurrentState
        public IState CurrentState => m_StateMachine.CurrentState;

        #region MonoBehaviour event messages
        private void Start()
        {
            // Set this MonoBehaviour to control the coroutines - unused in this demo
            Coroutines.Initialize(this);

            // Checks for required fields in the Inspector
            NullRefChecker.Validate(this);

            // Instantiates any assets needed before gameplay
            InstantiatePreloadedAssets();

            // Sets up States and transitions, runs initial State
            Initialize();
        }

        // Subscribe to event channels
        private void OnEnable()
        {
            SceneEvents.ExitApplication += SceneEvents_ExitApplication;
        }

        // Unsubscribe from event channels to prevent errors
        private void OnDisable()
        {
            SceneEvents.ExitApplication -= SceneEvents_ExitApplication;
        }
        #endregion

        #region Methods

        public void Initialize()
        {
            // Define the Game States
            SetStates();
            AddLinks();

            // Run first state/loading screen
            m_StateMachine.Run(m_SplashScreenState);
            UIEvents.SplashScreenShown?.Invoke();
        }

        // Define the state machine's states
        private void SetStates()
        {
            // Create States for the game. Pass in an Action to execute or null to do nothing

            // Optional names added for debugging
            // Executes GameEvents.LoadProgressUpdated every frame and GameEvents.PreloadCompleted on exit
            m_SplashScreenState = new DelayState(m_LoadScreenTime, SceneEvents.LoadProgressUpdated,
                SceneEvents.PreloadCompleted, "LoadScreenState");
            
            m_StartScreenState = new State(null, "StartScreenState", m_Debug);
            m_FirstMainMenuState = new State(null, "FirstMainMenuState", m_Debug);
            m_MainMenuState = new State(null, "MainMenuState", m_Debug);
            m_SettingsAState = new State(null, "SettingsAState", m_Debug);
            m_SettingsVState = new State(null, "SettingsVState", m_Debug);
            m_NewPlayState = new State(null, "NewPlayState", m_Debug);
            m_ContinuePlayState = new State(null, "GamePlayState", m_Debug);
            m_PauseState = new State(null, "PauseState", m_Debug);
            m_InventoryState = new State(null, "InventoryState", m_Debug);
            m_GameWinState = new State(null, "GameWinState", m_Debug);
            m_GameLoseState = new State(null, "GameLoseState", m_Debug);
        }


        // Define links between the states
        private void AddLinks()
        {

            // Transition automatically to the StartScreen once the loading time completes(1th or 2th)
            m_SplashScreenState.AddLink(new Link(m_StartScreenState));

            // EventLinks listen for the UI/game event messages to activate the transition to the next state

            // This implementation uses a wrapper around the event to make easier to register/unregister the EventLinks
            ActionWrapper FirstmainMenuScreenShownWrapper = new ActionWrapper
            {
                Subscribe = handler => UIEvents.FirstMainMenuScreenShown += handler,
                Unsubscribe = handler => UIEvents.FirstMainMenuScreenShown -= handler
            };

            ActionWrapper mainMenuScreenShownWrapper = new ActionWrapper
            {
                Subscribe = handler => UIEvents.MainMenuScreenShown += handler,
                Unsubscribe = handler => UIEvents.MainMenuScreenShown -= handler
            };

            ActionWrapper settingsAScreenShownWrapper = new ActionWrapper
            {
                Subscribe = handler => UIEvents.AudioOptionsScreenShown += handler,
                Unsubscribe = handler => UIEvents.AudioOptionsScreenShown -= handler
            };

            ActionWrapper settingsVScreenShownWrapper = new ActionWrapper
            {
                Subscribe = handler => UIEvents.VideoOptionsScreenShown += handler,
                Unsubscribe = handler => UIEvents.VideoOptionsScreenShown -= handler
            };
  
            ActionWrapper pauseScreenShownWrapper = new ActionWrapper
            {
                Subscribe = handler => UIEvents.PauseScreenShown += handler,
                Unsubscribe = handler => UIEvents.PauseScreenShown -= handler
            };

            ActionWrapper inventoryScreenShownWrapper = new ActionWrapper
            {
                Subscribe = handler => UIEvents.InventoryScreenShown += handler,
                Unsubscribe = handler => UIEvents.InventoryScreenShown -= handler
            };

            ActionWrapper loseScreenShownWrapper = new ActionWrapper
            {
                Subscribe = handler => UIEvents.InventoryScreenShown += handler,
                Unsubscribe = handler => UIEvents.InventoryScreenShown -= handler
            };

            //=================================================================
             ActionWrapper screenClosedWrapper = new ActionWrapper
            {
                Subscribe = handler => UIEvents.ScreenClosed += handler,
                Unsubscribe = handler => UIEvents.ScreenClosed -= handler
            };

            //================================================================
            ActionWrapper gameWonWrapper = new ActionWrapper
            {
                Subscribe = handler => GameEvents.GameWon += handler,
                Unsubscribe = handler => GameEvents.GameWon -= handler
            };

             ActionWrapper gameLostWrapper = new ActionWrapper
            {
                Subscribe = handler => GameEvents.GameLost += handler,
                Unsubscribe = handler => GameEvents.GameLost -= handler
            };

            ActionWrapper gameStartedWrapper = new ActionWrapper
            {
                Subscribe = handler => GameEvents.GameStarted += handler,
                Unsubscribe = handler => GameEvents.GameStarted -= handler
            };

            ActionWrapper gameContinueWrapper = new ActionWrapper
            {
                Subscribe = handler => GameEvents.GameContinued += handler,
                Unsubscribe = handler => GameEvents.GameContinued -= handler
            };

            // Once you have wrappers defined around the events, set up the EventLinks

            m_StartScreenState.AddLink(new EventLink(mainMenuScreenShownWrapper, m_MainMenuState));

            m_FirstMainMenuState.AddLink(new EventLink(gameStartedWrapper, m_NewPlayState));
            m_FirstMainMenuState.AddLink(new EventLink(settingsAScreenShownWrapper, m_SettingsAState));
           
            m_MainMenuState.AddLink(new EventLink(gameStartedWrapper, m_NewPlayState));
            m_MainMenuState.AddLink(new EventLink(gameContinueWrapper, m_ContinuePlayState));
            m_MainMenuState.AddLink(new EventLink(settingsAScreenShownWrapper, m_SettingsAState));

            m_SettingsAState.AddLink(new EventLink(screenClosedWrapper, m_MainMenuState));
            m_SettingsAState.AddLink(new EventLink(settingsVScreenShownWrapper, m_SettingsVState));

            m_SettingsVState.AddLink(new EventLink(screenClosedWrapper, m_MainMenuState));
            m_SettingsVState.AddLink(new EventLink(settingsAScreenShownWrapper, m_SettingsAState));

            m_ContinuePlayState.AddLink(new EventLink(pauseScreenShownWrapper, m_PauseState));
            m_ContinuePlayState.AddLink(new EventLink(gameWonWrapper, m_GameWinState));
            m_ContinuePlayState.AddLink(new EventLink(gameLostWrapper, m_GameLoseState));
            m_ContinuePlayState.AddLink(new EventLink(pauseScreenShownWrapper, m_PauseState));
            m_ContinuePlayState.AddLink(new EventLink(inventoryScreenShownWrapper, m_InventoryState));

            m_PauseState.AddLink(new EventLink(gameContinueWrapper, m_ContinuePlayState));
            m_PauseState.AddLink(new EventLink(mainMenuScreenShownWrapper, m_MainMenuState));

            m_InventoryState.AddLink(new EventLink(gameContinueWrapper, m_ContinuePlayState));
            
            m_GameLoseState.AddLink(new EventLink(loseScreenShownWrapper, m_ContinuePlayState));

            m_GameWinState.AddLink(new EventLink(mainMenuScreenShownWrapper, m_MainMenuState));
        }

        // Use this to preload any assets.
        // opportunity to load any textures, models, etc. in advance to avoid loading during gameplay 
        private void InstantiatePreloadedAssets()
        {
            foreach (var asset in m_PreloadedAssets)
            {
                if (asset != null)
                    Instantiate(asset);
            }
        }
        #endregion

        // Event-handling methods
        private void SceneEvents_ExitApplication()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
