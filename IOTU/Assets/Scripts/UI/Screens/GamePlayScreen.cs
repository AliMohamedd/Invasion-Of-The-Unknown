using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace IOTU
{
    /// <summary>
    /// Represents the gameplay screen, managing UI elements such as point and interaction labels.
    /// </summary>
    public class GamePlayScreen : UIScreen
    {
        Label m_Point;               // UI element to display points
        Label m_CanInteract;         // UI element to indicate interaction availability

        /// <summary>
        /// Constructor to initialize the gameplay screen with its parent VisualElement.
        /// </summary>
        /// <param name="parentElement">The parent VisualElement to attach UI elements to.</param>
        public GamePlayScreen(VisualElement parentElement) : base(parentElement)
        {
            SetVisualElements();
            RegisterCallbacks();    // Currently no callbacks registered
            SubscribeToEvents();
            GameEvents.GameStarted?.Invoke();
        }

        // Disables the screen and unsubscribes from events.
        public override void Disable()
        {
            base.Disable();
            UnsubscribeFromEvents();
        }

        // Sets references to UI elements by querying the root VisualElement.
        private void SetVisualElements()
        {
            m_Point = m_RootElement.Q<Label>("dot_label");
            m_CanInteract = m_RootElement.Q<Label>("interac_label");

            // Initially hide interaction label
            m_Point.visible = true;
            m_CanInteract.visible = false;
        }
        
        private void RegisterCallbacks()
        {

        }

        // Subscribes to relevant game events.
        private void SubscribeToEvents()
        {
            GameEvents.ShowDot += GameEvents_ShowDot;
            GameEvents.ShowInteractLabel += GameEvents_ShowInteract;
            GameEvents.HideAll += GameEvents_HideAll;
        }

        // Unsubscribes from game events.
        private void UnsubscribeFromEvents()
        {
            GameEvents.ShowDot -= GameEvents_ShowDot;
            GameEvents.ShowInteractLabel -= GameEvents_ShowInteract;
            GameEvents.HideAll -= GameEvents_HideAll;
        }

        // Event handler for showing the dot label.
        private void GameEvents_ShowDot()
        {
            m_Point.visible = true;
            m_CanInteract.visible = false;
        }

        // Event handler for showing the interaction label.
        private void GameEvents_ShowInteract()
        {
            m_Point.visible = false;
            m_CanInteract.visible = true;
        }

        // Event handler for hiding all labels.
        private void GameEvents_HideAll()
        {
            m_Point.visible = false;
            m_CanInteract.visible = false;
        }
    }
}
