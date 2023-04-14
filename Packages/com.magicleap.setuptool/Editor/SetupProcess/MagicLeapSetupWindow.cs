#region

using System;
using System.Collections.Generic;
using System.Reflection;
using MagicLeap.SetupTool.Editor.Setup;
using MagicLeap.SetupTool.Editor.Utilities;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

namespace MagicLeap.SetupTool.Editor
{
    public class MagicLeapSetupWindow : EditorWindow
    {

    #region TEXT AND LABELS

        private const string WINDOW_PATH = "Magic Leap/Project Setup Tool";
        private const string WINDOW_TITLE_LABEL = "Magic Leap Project Setup";
        private const string SUBTITLE_LABEL = "PROJECT SETUP TOOL";
        private const string HELP_BOX_TEXT = "These steps are required to develop Unity applications for the Magic Leap 2.";
        private const string LOADING_TEXT = "  Please wait. Loading and Importing...";
        private const string LINKS_TITLE = "Helpful Links:";
        private const string APPLY_ALL_BUTTON_LABEL = "Apply All";
        private const string CLOSE_BUTTON_LABEL = "Close";
        private const string GETTING_STARTED_HELP_TEXT = "Read the getting started guide";

    #endregion

    #region HELP URLS
        private const string GETTING_STARTED_URL = "https://developer-docs.magicleap.cloud/docs/guides/getting-started/";
    #endregion
    
        private const string FONT_PATH = "Lomino/Lomino_Md.ttf";
        private const string LOGO_PATH = "magic-leap-window-title";
        private static readonly SetSdkFolderSetupStep _setSdkFolderSetupStep = new SetSdkFolderSetupStep();
        private static readonly BuildTargetSetupStep _buildTargetSetupStep = new BuildTargetSetupStep();
        private static readonly SetDefaultTextureCompressionStep _defaultTextureCompressionStep = new SetDefaultTextureCompressionStep();
        private static readonly EnablePluginSetupStep _enablePluginSetupStep = new EnablePluginSetupStep();
        private static readonly UpdateManifestSetupStep _updateManifestSetupStep = new UpdateManifestSetupStep();
        private static readonly SwitchActiveInputHandlerStep _switchActiveInputHandlerStep = new SwitchActiveInputHandlerStep();
        private static readonly ImportMagicLeapSdkSetupStep _importMagicLeapSdkSetupStep = new ImportMagicLeapSdkSetupStep();
        private static readonly ColorSpaceSetupStep _colorSpaceSetupStep = new ColorSpaceSetupStep();
        private static readonly UpdateGraphicsApiSetupStep _updateGraphicsApiSetupStep = new UpdateGraphicsApiSetupStep();
        private static readonly SetScriptingBackendStep _setScriptingBackendStep = new SetScriptingBackendStep();
        private static readonly SetTargetArchitectureStep _setTargetArchitectureStep = new SetTargetArchitectureStep();
    
    
        private static MagicLeapSetupWindow _setupWindow;
        private static ApplyAllRunner _applyAllRunner;
        private static Texture2D _logo;
        private static bool _loading;
      

        private void OnEnable()
        {
            GetApplyAllRunner().Tick();
            _logo = (Texture2D)Resources.Load(LOGO_PATH, typeof(Texture2D));
            FullRefresh();
            RefreshSteps();
            Application.quitting+= ApplicationOnQuitting;
            

        }

        private void ApplicationOnQuitting()
        {
            Application.quitting -= ApplicationOnQuitting;
        }

        //Check if there was an editor refresh and GC collected the runner. If so, re-create it.
        private static ApplyAllRunner GetApplyAllRunner()
        {
            return _applyAllRunner ??= new ApplyAllRunner(_setSdkFolderSetupStep, _buildTargetSetupStep, _colorSpaceSetupStep,
                                                          _setScriptingBackendStep, _setTargetArchitectureStep, _importMagicLeapSdkSetupStep,
                                                          _enablePluginSetupStep, _defaultTextureCompressionStep,_updateManifestSetupStep, _updateGraphicsApiSetupStep);
        }

        private void OnDisable()
        {
            EditorPrefs.SetBool(EditorKeyUtility.AutoShowEditorPrefKey, !GetApplyAllRunner().AllAutoStepsComplete);
        }

        private void OnDestroy()
        {
            EditorPrefs.SetBool(EditorKeyUtility.WindowClosedEditorPrefKey, true);
            EditorApplication.projectChanged -= FullRefresh;
        }



        public void OnGUI()
        {
            DrawHeader();
            _loading = AssetDatabase.IsAssetImportWorkerProcess() || EditorApplication.isUpdating || EditorApplication.isCompiling || ApplyAllRunner.Running || ImportMagicLeapSdkSetupStep.IsCheckingForInstalledPackage;
               
       
            if (_loading )
            {
                DrawWaitingInfo();
                return;
            }

            DrawInfoBox();
        
            GUILayout.BeginVertical(EditorStyles.helpBox);
            {
                GUILayout.Space(5);
                if (_setSdkFolderSetupStep.Draw()) Repaint();

                if (_buildTargetSetupStep.Draw()) Repaint();

                if (_setScriptingBackendStep.Draw()) Repaint();

                if (_setTargetArchitectureStep.Draw()) Repaint();

                if (_colorSpaceSetupStep.Draw()) Repaint();
                
                if (_importMagicLeapSdkSetupStep.Draw()) Repaint();

                if (_defaultTextureCompressionStep.Draw()) Repaint();

                if (_switchActiveInputHandlerStep.Draw()) Repaint();

                if (_enablePluginSetupStep.Draw()) Repaint();

                if (_updateManifestSetupStep.Draw()) Repaint();
                
                if (_updateGraphicsApiSetupStep.Draw()) Repaint();

                GUI.backgroundColor = Color.clear;
            }

            GUILayout.EndVertical();
            GUILayout.Space(10);
            DrawHelpLinks();
            DrawFooter();

        }

        private void OnFocus()
        {
            RefreshSteps();
        }


        private void OnInspectorUpdate()
        {
            Repaint();
            _loading = AssetDatabase.IsAssetImportWorkerProcess() || EditorApplication.isCompiling || EditorApplication.isUpdating;

            if (_loading || (!ImportMagicLeapSdkSetupStep.HasMagicLeapSdkInPackageManager && _importMagicLeapSdkSetupStep.Busy) || (!ImportMagicLeapSdkSetupStep.HasMagicLeapSdkInPackageManager && ImportMagicLeapSdkSetupStep.Running))
            {
                return;
            }

            GetApplyAllRunner().Tick();
        }


        private static void RefreshSteps()
        {
            _defaultTextureCompressionStep.Refresh();
            _setSdkFolderSetupStep.Refresh();
            _buildTargetSetupStep.Refresh();
            _enablePluginSetupStep.Refresh();
            _switchActiveInputHandlerStep.Refresh();
            _updateManifestSetupStep.Refresh();
            _colorSpaceSetupStep.Refresh();
            _setScriptingBackendStep.Refresh();
            _setTargetArchitectureStep.Refresh();
            _updateGraphicsApiSetupStep.Refresh();
        }

        private static void Open()
        {
            _setupWindow = GetWindow<MagicLeapSetupWindow>(false, WINDOW_TITLE_LABEL);
         
            _setupWindow.minSize = new Vector2(350, 655);
            _setupWindow.maxSize = new Vector2(350, 675);
            _setupWindow.Show();
            EditorApplication.delayCall += ApplyAllRunner.CheckLastAutoSetupState;
           //EditorPrefs.DeleteKey("MagicLeapSDKRoot");
            
            EditorApplication.projectChanged += FullRefresh;
            EditorSceneManager.sceneOpened += (s, l) => { GetApplyAllRunner().Tick(); };
            EditorApplication.quitting += () =>
            {
                EditorPrefs.SetBool(EditorKeyUtility.WindowClosedEditorPrefKey, false);
            };
    

        }

        public static void ForceOpen()
        {
        
                    EditorApplication.delayCall +=()=>
                                          {
                                              if (EditorPrefs.GetBool(EditorKeyUtility.WindowClosedEditorPrefKey, false)) return;
                                              Open();
                                          };

        }

        [MenuItem(WINDOW_PATH)]
        public static void MenuOpen()
        {
            Open();
        }

        private static void FullRefresh()
        {
      
            EditorApplication.delayCall += RefreshSteps;
            _importMagicLeapSdkSetupStep.Refresh();
        }


        #region Draw Window Controls

        private void DrawHeader()
        {
            //Draw Magic Leap brand image
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(_logo);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
          
            GUILayout.Space(5);
            GUILayout.BeginVertical();
            {
                EditorGUILayout.LabelField(SUBTITLE_LABEL, Styles.TitleStyleDefaultFont);
                GUILayout.EndVertical();
            }

            CustomGuiContent.DrawUILine(Color.grey, 1, 5);
            GUI.backgroundColor = Color.white;
            GUILayout.Space(2);
        }

        private void DrawInfoBox()
        {
            GUILayout.Space(5);
          
            var content = new GUIContent(HELP_BOX_TEXT);
            EditorGUILayout.LabelField(content, Styles.InfoTitleStyle);

            GUILayout.Space(5);
            GUI.backgroundColor = Color.white;
        }

        private void DrawHelpLinks()
        {
            var currentGUIEnabledStatus = GUI.enabled;
            GUI.enabled = true;
            GUI.backgroundColor = Color.white;
            GUILayout.BeginVertical(EditorStyles.helpBox);
            {
                GUILayout.Space(2);
                EditorGUILayout.LabelField(LINKS_TITLE, Styles.HelpTitleStyle);
                CustomGuiContent.DisplayLink(GETTING_STARTED_HELP_TEXT, GETTING_STARTED_URL, 3);

                GUILayout.Space(2);
                GUILayout.Space(2);
            }
            GUILayout.EndVertical();
            GUI.enabled = currentGUIEnabledStatus;
        }

        private void DrawWaitingInfo()
        {

            GUILayout.Space(5);
            var content = new GUIContent(LOADING_TEXT);
            EditorGUILayout.LabelField(content, Styles.InfoTitleStyle);
            GUI.enabled = false;

            GUILayout.Space(5);
            GUI.backgroundColor = Color.white;
        }

        private void DrawFooter()
        {
            GUILayout.FlexibleSpace();
            var currentGUIEnabledStatus = GUI.enabled;
            GUI.enabled = !_loading;
            if (GetApplyAllRunner().AllAutoStepsComplete)
            {
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button(CLOSE_BUTTON_LABEL, GUILayout.MinWidth(20), GUILayout.MinHeight(30))) Close();
            }
            else
            {
                GUI.backgroundColor = Color.yellow;
                if (GUILayout.Button(APPLY_ALL_BUTTON_LABEL, GUILayout.MinWidth(20), GUILayout.MinHeight(30))) GetApplyAllRunner().RunApplyAll();
            }

      
            GUI.enabled = currentGUIEnabledStatus;
            GUI.backgroundColor = Color.clear;
        }

        #endregion
    }
}