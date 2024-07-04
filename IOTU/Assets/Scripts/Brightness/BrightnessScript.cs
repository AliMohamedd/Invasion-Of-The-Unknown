using IOTU;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

/// <summary>
/// The BrightnessScript class in the YourNamespace namespace manages the brightness settings 
/// of the game. It provides methods to adjust the brightness level based on user input.
/// </summary>
public class BrightnessScript : MonoBehaviour
{
    [Tooltip("The PostProcessProfile used to adjust color grading settings.")]
    [SerializeField] public PostProcessProfile profile;

    // Reference to the video settings scriptable object
    VideoSettingsSO videoSettingsSO;

    
    // Updates the brightness and contrast based on the values from the video settings scriptable object.
    private void Update()
    {
        // Load the video settings scriptable object
        videoSettingsSO = Resources.Load<VideoSettingsSO>("VideoGraphics/VideoSettings_Data");

        // Retrieve the brightness and contrast values from the scriptable object
        float exposure = videoSettingsSO.valueBrightness;
        float contrast = videoSettingsSO.valueContrast;

        // Adjust brightness and contrast using the retrieved values
        SetBrightness((exposure * 6) - 3);
        SetContrast((contrast * 200) - 100);
    }

    
    // Sets the brightness level using the ColorGrading settings in the post-processing profile.
    public void SetBrightness(float value)
    {
        // Try to get the ColorGrading settings from the post-processing profile
        profile.TryGetSettings(out ColorGrading colorGrading);

        // If the ColorGrading settings are found, set the post exposure value
        if (colorGrading != null)
        {
            colorGrading.postExposure.value = value;
        }
    }

  
    // Sets the contrast level using the ColorGrading settings in the post-processing profile.
    public void SetContrast(float value)
    {
        // Try to get the ColorGrading settings from the post-processing profile
        profile.TryGetSettings(out ColorGrading colorGrading);

        // If the ColorGrading settings are found, set the contrast value
        if (colorGrading != null)
        {
            colorGrading.contrast.value = value;
        }
    }
}
