using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The KeyRandomizer class manages the spawning of random keys and notes in a game environment.
/// It selects a random key from a provided array and activates its GameObject. Depending on the 
/// selected key's index, a corresponding note is activated from a separate array to indicate its location.
/// </summary>
public class KeyRandomizer : MonoBehaviour
{
    [SerializeField] public GameObject[] keys; // Array of key GameObjects
    [SerializeField] public GameObject[] notes; // Array of note GameObjects

    // Start is called before the first frame update
    void Start()
    {
        SpawnRandomKeyWithNote();    
    }

    /// Selects a random key GameObject from the keys array and activates it. Activates a corresponding
    // note GameObject based on the selected key's index to indicate its location.
    void SpawnRandomKeyWithNote()
    {
        int selectedKey = Random.Range(0, keys.Length); // Randomly choose a key index
        keys[selectedKey].SetActive(true); // Activate the selected key GameObject
        Debug.Log("Current active key is " + (selectedKey + 1));

        // Activate a note based on the selected key's index
        if (selectedKey == 0 || selectedKey == 5 || selectedKey == 2)
        {
            // First floor note
            notes[1].SetActive(true);
        }
        else if (selectedKey == 3 || selectedKey == 1)
        {
            // Second floor note
            notes[2].SetActive(true);
        }
        else
        {
            // Basement note
            notes[0].SetActive(true);
        }
    }
}
