#region

using MagicLeap.SetupTool.Editor.Interfaces;
using MagicLeap.SetupTool.Editor.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

#endregion

namespace MagicLeap.SetupTool.Editor.Setup
{
    /// <summary>
    /// Changes the graphics APIs to work with Magic Leap and Zero Iteration
    /// </summary>
    public class UpdateGraphicsApiSetupStep : ISetupStep
    {
        //Localization
        private const string SET_CORRECT_GRAPHICS_API_LABEL = "Add Vulkan to Graphics API";
        private const string SET_CORRECT_GRAPHICS_BUTTON_LABEL = "Update";
        private const string CONDITION_MET_LABEL = "Done";
        
        private static int _busyCounter;
        private static bool _hasCorrectGraphicConfiguration;
        public bool Block => false;
        public bool CanExecute => EnableGUI();
        private static int BusyCounter
        {
            get => _busyCounter;
            set => _busyCounter = Mathf.Clamp(value, 0, 100);
        }

        /// <inheritdoc />
        public bool Busy => BusyCounter > 0;
        /// <inheritdoc />
        public bool IsComplete => UnityProjectSettingsUtility.OnlyHasGraphicsDeviceType(BuildTarget.Android, GraphicsDeviceType.Vulkan);

        /// <inheritdoc />
        public void Refresh()
        {
            _hasCorrectGraphicConfiguration = UnityProjectSettingsUtility.OnlyHasGraphicsDeviceType(BuildTarget.Android, GraphicsDeviceType.Vulkan);
        }

        private bool EnableGUI()
        {
            var correctBuildTarget = EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android;
            return correctBuildTarget;
        }
        /// <inheritdoc />
        public bool Draw()
        {
            GUI.enabled = EnableGUI();
            if (CustomGuiContent.CustomButtons.DrawConditionButton(SET_CORRECT_GRAPHICS_API_LABEL,
                _hasCorrectGraphicConfiguration, CONDITION_MET_LABEL, SET_CORRECT_GRAPHICS_BUTTON_LABEL,
                Styles.FixButtonStyle))
            {
                Execute();
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public void Execute()
        {
            if (IsComplete || Busy) return;

            UpdateGraphicsSettings();
        }

        /// <summary>
        /// Checks if the graphics configuration supports Magic Leap and Zero Iteration
        /// </summary>
        /// <returns></returns>
        private static bool CorrectGraphicsConfiguration()
        {
            var correctSetup = false;
            var hasGraphicsDevice = false;
            #region Android

            hasGraphicsDevice = UnityProjectSettingsUtility.HasGraphicsDeviceTypeAtIndex(BuildTarget.Android,GraphicsDeviceType.Vulkan, 0);
            correctSetup = hasGraphicsDevice && !UnityProjectSettingsUtility.GetAutoGraphicsApi(BuildTarget.Android);
            if (!correctSetup) return false;

            #endregion

            #region Windows
            
            hasGraphicsDevice = UnityProjectSettingsUtility.HasGraphicsDeviceTypeAtIndex(BuildTarget.StandaloneWindows, GraphicsDeviceType.OpenGLCore, 0);
            correctSetup = hasGraphicsDevice &&  !UnityProjectSettingsUtility.GetAutoGraphicsApi(BuildTarget.StandaloneWindows);
            if (!correctSetup) return false;

            #endregion

            #region OSX

            hasGraphicsDevice = UnityProjectSettingsUtility.HasGraphicsDeviceTypeAtIndex(BuildTarget.StandaloneOSX, GraphicsDeviceType.Metal, 0);
            correctSetup = hasGraphicsDevice && !UnityProjectSettingsUtility.GetAutoGraphicsApi(BuildTarget.StandaloneOSX);
            if (!correctSetup) return false;

            #endregion

            #region Linux

            hasGraphicsDevice = UnityProjectSettingsUtility.HasGraphicsDeviceTypeAtIndex(BuildTarget.StandaloneLinux64,
                GraphicsDeviceType.OpenGLCore, 0);
            correctSetup = hasGraphicsDevice && !UnityProjectSettingsUtility.GetAutoGraphicsApi(BuildTarget.StandaloneLinux64);
            if (!correctSetup) return false;

            #endregion

            return correctSetup;
        }


        /// <summary>
        /// Changes the graphics settings for all Magic Leap 2 platforms
        /// </summary>
        public static void UpdateGraphicsSettings()
        {
            BusyCounter++;
            var androidResetRequired = UnityProjectSettingsUtility.UseOnlyThisGraphicsApi(BuildTarget.Android, GraphicsDeviceType.Vulkan);
            var standaloneWindowsResetRequired = UnityProjectSettingsUtility.SetGraphicsApi(BuildTarget.StandaloneWindows, GraphicsDeviceType.OpenGLCore,0);
            var standaloneWindows64ResetRequired =UnityProjectSettingsUtility.SetGraphicsApi(BuildTarget.StandaloneWindows64,GraphicsDeviceType.OpenGLCore, 0);
            var standaloneOSXResetRequired =UnityProjectSettingsUtility.SetGraphicsApi(BuildTarget.StandaloneOSX, GraphicsDeviceType.Metal, 0);
            var standaloneLinuxResetRequired = UnityProjectSettingsUtility.SetGraphicsApi(BuildTarget.StandaloneLinux64, GraphicsDeviceType.OpenGLCore,0);

            UnityProjectSettingsUtility.SetAutoGraphicsApi(BuildTarget.Android, false);
            UnityProjectSettingsUtility.SetAutoGraphicsApi(BuildTarget.StandaloneWindows, false);
            UnityProjectSettingsUtility.SetAutoGraphicsApi(BuildTarget.StandaloneWindows64, false);
            UnityProjectSettingsUtility.SetAutoGraphicsApi(BuildTarget.StandaloneOSX, false);
            UnityProjectSettingsUtility.SetAutoGraphicsApi(BuildTarget.StandaloneLinux64, false);


            ApplyAllRunner.Stop();
  
            
            if (androidResetRequired
            || standaloneWindowsResetRequired
            || standaloneWindows64ResetRequired
            || standaloneOSXResetRequired
            || standaloneLinuxResetRequired)
                UnityProjectSettingsUtility.UpdateGraphicsApi(true);
            else
                UnityProjectSettingsUtility.UpdateGraphicsApi(false);

            _hasCorrectGraphicConfiguration = UnityProjectSettingsUtility.OnlyHasGraphicsDeviceType(BuildTarget.Android, GraphicsDeviceType.Vulkan);
            BusyCounter--;
        }
    }
}