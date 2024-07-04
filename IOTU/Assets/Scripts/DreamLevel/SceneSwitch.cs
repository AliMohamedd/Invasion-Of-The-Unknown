using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IOTU
{
    /// <summary>
    /// The SceneSwitch class handles the transition to a new scene when the player enters a trigger.
    /// It invokes game events to manage scene loading and unloading.
    /// </summary>
    public class SceneSwitch : MonoBehaviour
    {
        
        // Handles the trigger enter event. When another collider enters the trigger,
        // it initiates the process of loading a new scene.
        private async void OnTriggerEnter(Collider other)
        {
            // Call the method to load a new scene
            LoadNewScene();
        }

       
        // Loads a new scene and invokes events to handle the scene transition.
        private async void LoadNewScene()
        {
            // Invoke the event to signal the loading of the next level
            GameEvents.NextLevel?.Invoke(true);

            // Invoke the event to signal that the last scene is unloaded
            SceneEvents.LastSceneUnloaded();

            // Invoke the event to signal the loading of the new scene with the specified index
            SceneEvents.SceneIndexLoaded(2);
        }
    }
}
