using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace IOTU
{
    /// <summary>
    /// The NoteController class handles the behavior of a note object in the game.
    /// It overrides methods from the Interactable base class to define interactions and focus behavior.
    /// </summary>
    public class NoteController : Interactable
    {
        [Header("UI Text")]
        [SerializeField] private GameObject noteCanvas; // Canvas displaying the note UI
        [SerializeField] private TMP_Text noteTextAreaUI; // Text area within the note UI

        [Space(10)]
        [SerializeField]
        [TextArea] private string noteText; // Text content of the note

        private bool isOpen = false; // Flag indicating if the note UI is open

        // Called when the player interacts with the note object.
        // Toggles the visibility of the note UI and sets its text content.
        public override void OnInteract()
        {
            if (!isOpen)
            {
                // If the note UI is closed, open it
                Time.timeScale = 0; // Pause game time
                noteTextAreaUI.text = noteText; // Set the text of the note UI
                noteCanvas.SetActive(true); // Activate the note UI canvas
                isOpen = true; // Set isOpen flag to true
                GameEvents.noteShown?.Invoke(isOpen); // Trigger event indicating the note is shown
                GameEvents.HideAll?.Invoke(); // Trigger event to hide all other UI elements
            }
            else
            {
                // If the note UI is open, close it
                Time.timeScale = 1; // Resume game time
                noteCanvas.SetActive(false); // Deactivate the note UI canvas
                noteTextAreaUI.text = null; // Clear the text of the note UI
                isOpen = false; // Set isOpen flag to false
                GameEvents.noteShown?.Invoke(isOpen); // Trigger event indicating the note is hidden
                GameEvents.ShowInteractLabel?.Invoke(); // Trigger event to show interact label
            }
        }

        // Called when the note object comes into focus for interaction.
        // Shows an interact label.
        public override void OnFocus()
        {
            GameEvents.ShowInteractLabel?.Invoke(); // Show interact label
        }

        // Called when the note object loses focus for interaction.
        // Shows a dot (indicating interaction).
        public override void onLoseFocus()
        {
            GameEvents.ShowDot?.Invoke(); // Show a dot (indicating interaction)
        }
    }
}
