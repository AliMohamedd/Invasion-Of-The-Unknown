using UnityEngine.UIElements;
using UnityEngine;

namespace IOTU
{
    /// <summary>
    /// This fullscreen UI hides the main menu until the user is ready to begin
    /// </summary>

    public class StartScreen : UIScreen
    {
        Button m_StartButton;

        bool First;

        GamePlaySO m_gamePlaySO;

        // Constructor 
        public StartScreen(VisualElement parentElement) : base(parentElement)
        {
            m_RootElement = parentElement;

            m_StartButton = m_RootElement.Q<Button>("start__start-button");

            // The custom Event Registry unregisters the callback automatically on disable
            m_EventRegistry.RegisterCallback<ClickEvent>(m_StartButton, evt => LoadMenu());
            
            m_gamePlaySO = Resources.Load<GamePlaySO>("GamePlay/GamePlay_Data");
            LoadData();
        }

        
        private void LoadMenu()
        {
            if (First) 
            {
                UIEvents.MainMenuScreenShown?.Invoke();
            }
            else
            {
                UIEvents.FirstMainMenuScreenShown?.Invoke();
            }
            GameEvents.GameUI?.Invoke();
        }

        private void LoadData()
        {
            if (PlayerPrefs.HasKey("NextLevel"))
            {
                if (PlayerPrefs.GetInt("NextLevel") == 1)
                {
                    First = true;
                }
                else
                {
                    First = false;
                }
            }
            else
            {
                First = m_gamePlaySO.NextLevel;
            }
        }
    }
}
