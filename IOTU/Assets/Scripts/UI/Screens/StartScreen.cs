using UnityEngine.UIElements;

namespace IOTU
{
    /// <summary>
    /// This fullscreen UI hides the main menu until the user is ready to begin
    /// </summary>

    public class StartScreen : UIScreen
    {
        Button m_StartButton;

        bool First;

        // Constructor 
        public StartScreen(VisualElement parentElement) : base(parentElement)
        {
            m_RootElement = parentElement;

            m_StartButton = m_RootElement.Q<Button>("start__start-button");

            // The custom Event Registry unregisters the callback automatically on disable
            m_EventRegistry.RegisterCallback<ClickEvent>(m_StartButton, evt => loadMenu());
            SubscribeToEvents();
        }

        public override void Disable()
        {
            base.Disable();
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            GameEvents.NextLevel += First_Changed;
        }

        private void UnsubscribeFromEvents()
        {
            GameEvents.NextLevel -= First_Changed;
        }
        private void First_Changed(bool val)
        {
            First = val;
        }
        
        private void loadMenu()
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
    }
}
