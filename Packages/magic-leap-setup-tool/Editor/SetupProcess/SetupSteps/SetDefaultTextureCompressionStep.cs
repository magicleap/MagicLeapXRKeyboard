using System.Linq;
using MagicLeap.SetupTool.Editor.Interfaces;
using MagicLeap.SetupTool.Editor.Utilities;
using UnityEditor;
using UnityEngine;

namespace MagicLeap.SetupTool.Editor.Setup
{
	public class SetDefaultTextureCompressionStep : ISetupStep
	{
		//Localization
		private const string REQUIRED_TEXTURE_COMPRESSION_LABEL_DXTC_RGTC = "DXTC_RGTC";
		private const string REQUIRED_TEXTURE_COMPRESSION_LABEL_DXTC = "DXTC";
		private const string FIX_SETTING_BUTTON_LABEL = "Fix Setting";
		private const string CONDITION_MET_LABEL = "Done";
		private const string SET_TEXTURE_COMPRESSION_LABEL = "Use DXT texture compression";
		
		private static bool _isTextureCompressionSet = false;
		public bool CanExecute => EnableGUI();
		public bool Block => false;

		/// <inheritdoc />
		public bool Busy { get; private set; }
		/// <inheritdoc />
		public bool IsComplete => UnityProjectSettingsUtility.IsTextureCompressionSet(BuildTargetGroup.Android, REQUIRED_TEXTURE_COMPRESSION_LABEL_DXTC_RGTC) || UnityProjectSettingsUtility.IsTextureCompressionSet(BuildTargetGroup.Android, REQUIRED_TEXTURE_COMPRESSION_LABEL_DXTC);

		/// <inheritdoc />
		public void Refresh()
		{

			_isTextureCompressionSet = IsComplete;
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
			if (CustomGuiContent.CustomButtons.DrawConditionButton(SET_TEXTURE_COMPRESSION_LABEL, _isTextureCompressionSet,CONDITION_MET_LABEL,FIX_SETTING_BUTTON_LABEL, Styles.FixButtonStyle))
			{
				Busy = true;
				Execute();
				return true;
			}

			return false;
		}
		
		/// <inheritdoc />
		public void Execute()
		{
			if (IsComplete)
			{
				Busy = false;
				return;
			}

			if (!_isTextureCompressionSet)
			{
				UnityProjectSettingsUtility.SetTextureCompression(BuildTargetGroup.Android, REQUIRED_TEXTURE_COMPRESSION_LABEL_DXTC_RGTC);
				_isTextureCompressionSet = IsComplete;
			}

			Busy = false;
		}


	}
}
