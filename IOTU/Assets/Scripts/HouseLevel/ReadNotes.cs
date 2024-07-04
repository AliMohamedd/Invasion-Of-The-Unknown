using IOTU; 
using UnityEngine;

/// <summary>
/// The ReadNotes class handles the interaction with notes in the game.
/// It detects when the player enters the trigger area of a note, displays a prompt to interact,
/// and allows the player to view the note UI upon pressing the 'E' key. It disables player control
/// temporarily while the note is being read and provides an option to exit back to the game HUD.
/// </summary>
public class ReadNotes : MonoBehaviour
{
    public GameObject player; // Assign the player GameObject in the Unity Editor
    private playerController playerControl; // Reference to the playerController script attached to the player
    public GameObject noteUI; // UI panel for displaying the note
    public GameObject hud; // Game HUD
    public GameObject pickUpText; // Text prompt to pick up the note
    public bool inReach; // Flag indicating if the player is within reach of a note

    void Start()
    {
        if (player != null)
        {
            // Cache the PlayerController component if the player GameObject is assigned
            playerControl = player.GetComponent<playerController>();
        }
        else
        {
            Debug.LogError("Player GameObject is not assigned in the ReadNotes script.");
        }

        // Initialize UI states
        noteUI.SetActive(false); // Hide note UI initially
        hud.SetActive(true); // Show game HUD
        pickUpText.SetActive(false); // Hide pick up text
        inReach = false; // Player is not initially in reach of a note
    }

    void OnTriggerEnter(Collider other)
    {
        // When player enters trigger area tagged as "Reach"
        if (other.gameObject.CompareTag("Reach"))
        {
            inReach = true; // Player is in reach of the note
            pickUpText.SetActive(true); // Show pick up text prompt
        }
    }

    void OnTriggerExit(Collider other)
    {
        // When player exits trigger area tagged as "Reach"
        if (other.gameObject.CompareTag("Reach"))
        {
            inReach = false; // Player is no longer in reach of the note
            pickUpText.SetActive(false); // Hide pick up text prompt
        }
    }

    void Update()
    {
        // If player presses 'E' key, is in reach of a note, and has PlayerController component
        if (Input.GetKeyDown(KeyCode.E) && inReach && playerControl != null)
        {
            InteractWithNote(); // Perform interaction with the note
        }
    }

    void InteractWithNote()
    {
        noteUI.SetActive(true); // Show note UI
        hud.SetActive(false); // Hide game HUD

        // Disable PlayerControl script to prevent player movement
        playerControl.enabled = false;
        Cursor.visible = true; // Show cursor
        Cursor.lockState = CursorLockMode.None; // Unlock cursor
    }

    // Method called when exit button in the note UI is pressed
    public void ExitButton()
    {
        noteUI.SetActive(false); // Hide note UI
        hud.SetActive(true); // Show game HUD

        // Re-enable PlayerControl script to allow player movement
        if (playerControl != null)
        {
            playerControl.enabled = true;
        }
    }
}
