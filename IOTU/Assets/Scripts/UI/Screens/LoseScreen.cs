using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace IOTU
{
    /// <summary>
    /// Represents the screen displayed when the player loses the game.
    /// </summary>
    public class LoseScreen : UIScreen
    {
        /// <summary>
        /// Initializes the lose screen and starts the process to load the game play screen again after a delay.
        /// </summary>
        /// <param name="parentElement">The parent VisualElement to which this screen is attached.</param>
        public LoseScreen(VisualElement parentElement) : base(parentElement)
        {
            LoadAgain(); // Start the process to load the game play screen again
        }

        // waits for 2 seconds and then invokes the event to show the game play screen again.
        private void LoadAgain()
        {
            // Delay for 2 seconds without blocking the main thread
            Task.Delay(2000);

            // Invoke event to show the gameplay screen again
            UIEvents.GamePlayScreenShown?.Invoke();
        }
    }
}
