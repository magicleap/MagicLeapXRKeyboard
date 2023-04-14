#region

using MagicLeap.SetupTool.Editor.Interfaces;
using MagicLeap.SetupTool.Editor.Utilities;
using UnityEditor;
using UnityEngine;

#endregion

namespace MagicLeap.SetupTool.Editor.Setup
{
    /// <summary>
    /// Enables the Magic Leap 2 XR plugin
    /// </summary>
    public class EnablePluginSetupStep : ISetupStep
    {
    
        //Localization
        private const string ENABLE_PLUGIN_LABEL = "Enable Plugin";
        private const string ENABLE_PLUGIN_SETTINGS_LABEL = "Enable Magic Leap XR Settings";
        private const string CONDITION_MET_LABEL = "Done";
        private const string ENABLE_MAGICLEAP_FINISHED_UNSUCCESSFULLY_WARNING = "Unsuccessful call:[Enable Magic Leap XR]. action finished, but Magic Leap XR Settings are still not enabled.";
        private const string FAILED_TO_EXECUTE_ERROR = "Failed to execute [Enable Magic Leap XR]";
        private const string ENABLE_MAGICLEAP_ZI_FINISHED_UNSUCCESSFULLY_WARNING = "Unsuccessful call:[Enable Zero Iteration]. action finished, but Zero Iteration Settings are still not enabled.";
        private const string FAILED_TO_EXECUTE_ZI_ERROR = "Failed to execute [Enable Zero Iteration]";

        private const string ENABLE_ZERO_ITERATION_LABEL = "Enable ZI";
        private const string ENABLE_ZERO_ITERATION_DIALOG_HEADER = "Enable XR Platform";
        private const string ENABLE_ZERO_ITERATION_DIALOG_BODY = "Would you like to enable Zero Iteration to test your project without building to device?";
        private const string ENABLE_ZERO_ITERATION_DIALOG_OK = "Enable";
        private const string ENABLE_ZERO_ITERATION_DIALOG_CANCEL = "Don't enable";
        private static int _busyCounter;
        private static bool _correctBuildTarget;
        public bool Block => false;

        private static int BusyCounter
        {
            get => _busyCounter;
            set => _busyCounter = Mathf.Clamp(value, 0, 100);
        }

        private bool _subscribedToEditorChangeEvent;
        private bool _hasRootSDKPath;
        private static bool _magicLeapXRSettingsEnabled;
        /// <inheritdoc />
        public bool Busy => BusyCounter > 0;
        /// <inheritdoc />
        public bool IsComplete => MagicLeapPackageUtility.IsMagicLeapXREnabled();
        public bool IsZeroIterationEnabled { get; private set; }
        public bool CanExecute => EnableGUI();
        /// <inheritdoc />
        public void Refresh()
        {
            if (!_subscribedToEditorChangeEvent)
            {
                _hasRootSDKPath = MagicLeapPackageUtility.HasRootSDKPath;
                _correctBuildTarget = EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android;
                _magicLeapXRSettingsEnabled = MagicLeapPackageUtility.IsMagicLeapXREnabled();
                EditorApplication.projectChanged += EditorApplicationOnProjectChanged;
                IsZeroIterationEnabled = MagicLeapPackageUtility.IsZeroIterationXREnabled();
            }
        }

        private void EditorApplicationOnProjectChanged()
        {
            _hasRootSDKPath = MagicLeapPackageUtility.HasRootSDKPath;
            _correctBuildTarget = EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android;
            _magicLeapXRSettingsEnabled = MagicLeapPackageUtility.IsMagicLeapXREnabled();
            IsZeroIterationEnabled = MagicLeapPackageUtility.IsZeroIterationXREnabled();
            _subscribedToEditorChangeEvent = true;
        }

        private bool EnableGUI()
        {
            return _hasRootSDKPath && _correctBuildTarget && MagicLeapPackageUtility.HasMagicLeapSDKInstalled;
        }
        /// <inheritdoc />
        public bool Draw()
        {
            GUI.enabled = EnableGUI();
            if (_magicLeapXRSettingsEnabled)
            {
      
                if (CustomGuiContent.CustomButtons.DrawConditionButton(ENABLE_ZERO_ITERATION_LABEL, IsZeroIterationEnabled, CONDITION_MET_LABEL, ENABLE_ZERO_ITERATION_LABEL, Styles.FixButtonStyle))
                {
                    MagicLeapPackageUtility.EnableZeroIterationXRPlugin();
                    return true;
                }
            }
            else
            {
                if (CustomGuiContent.CustomButtons.DrawConditionButton(ENABLE_PLUGIN_SETTINGS_LABEL, _magicLeapXRSettingsEnabled, CONDITION_MET_LABEL, ENABLE_PLUGIN_LABEL, Styles.FixButtonStyle))
                {
                    Execute();
                    return true;
                }
            }
            

            return false;
        }

        /// <inheritdoc />
        public void Execute()
        {
            if (IsComplete || Busy) return;

            if (!MagicLeapPackageUtility.HasMagicLeapSDKInstalled)
            {
                return;
            }

            var shouldEnableZeroIteration = EditorUtility.DisplayDialog(ENABLE_ZERO_ITERATION_DIALOG_HEADER, ENABLE_ZERO_ITERATION_DIALOG_BODY, ENABLE_ZERO_ITERATION_DIALOG_OK, ENABLE_ZERO_ITERATION_DIALOG_CANCEL);
        
         
            if (!_magicLeapXRSettingsEnabled)
            {
                BusyCounter++;
                MagicLeapPackageUtility.EnableMagicLeapXRFinished += OnEnableMagicLeapPluginFinished;
                MagicLeapPackageUtility.EnableMagicLeapXRPlugin();
            }

            if (!IsZeroIterationEnabled && shouldEnableZeroIteration)
            {
                BusyCounter++;
                MagicLeapPackageUtility.EnableZeroIterationFinished += OnEnableMagicLeapZeroIterationPluginFinished;
                MagicLeapPackageUtility.EnableZeroIterationXRPlugin();
            }



            void OnEnableMagicLeapZeroIterationPluginFinished(bool success)
            {
                if (success)
                {
                    IsZeroIterationEnabled = MagicLeapPackageUtility.IsZeroIterationXREnabled();
                    if (!IsZeroIterationEnabled)
                        Debug.LogWarning(ENABLE_MAGICLEAP_ZI_FINISHED_UNSUCCESSFULLY_WARNING);
                }
                else
                {
                    Debug.LogError(FAILED_TO_EXECUTE_ZI_ERROR);
                }

                BusyCounter--;
                MagicLeapPackageUtility.EnableZeroIterationFinished -= OnEnableMagicLeapZeroIterationPluginFinished;
            }

            void OnEnableMagicLeapPluginFinished(bool success)
            {
                if (success)
                {
                    _magicLeapXRSettingsEnabled = MagicLeapPackageUtility.IsMagicLeapXREnabled();
                    if (!_magicLeapXRSettingsEnabled)
                        Debug.LogWarning(ENABLE_MAGICLEAP_FINISHED_UNSUCCESSFULLY_WARNING);
                }
                else
                {
                    Debug.LogError(FAILED_TO_EXECUTE_ERROR);
                }

                BusyCounter--;
                MagicLeapPackageUtility.EnableMagicLeapXRFinished -= OnEnableMagicLeapPluginFinished;
            }

            UnityProjectSettingsUtility.OpenXRManagementWindow();
      
        }
    }
}