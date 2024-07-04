using System;

namespace IOTU
{
    /// <summary>
    /// Public static delegates to manage UI changes (note these are "events" in the conceptual sense
    /// and not the strict C# sense).
    /// </summary>
    public static class UIEvents
    {
        #region Menu/screen events:

        // Close the screen and go back
        public static Action ScreenClosed;
        
        // Loading Screen
        public static Action SplashScreenShown;

        // Show the First Main Menu selection 
        public static Action FirstMainMenuScreenShown;

        // Show the Main Menu selection
        public static Action MainMenuScreenShown;

        // Show the user settings (video options)
        public static Action VideoOptionsScreenShown;

        // Show the user settings (audio options)
        public static Action AudioOptionsScreenShown;

        // Show inventory
        public static Action InventoryScreenShown;

        // Show the main gameplay screen 
        public static Action GamePlayScreenShown;

        // Show a pause screen during gameplay to abort the game
        public static Action PauseScreenShown;

        // Show the lose Screen
        public static Action LoseScreenShown;

        #endregion


    }
}
