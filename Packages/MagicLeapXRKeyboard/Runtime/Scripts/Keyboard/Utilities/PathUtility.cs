using System.IO;

namespace MagicLeap.XRKeyboard
{
	public class PathUtility
	{
		public static string GetUniqueEnumeratedFileName(string directoryPath, string filePrefix, string fileExtension)
		{
			int fileNumber = 1;
			string uniqueFileName;

			do
			{
				uniqueFileName = Path.Combine(directoryPath, $"{filePrefix}_{fileNumber}{fileExtension}");
				fileNumber++;
			} while (File.Exists(uniqueFileName));

			return uniqueFileName.Replace("\\","/");
		}
	}
}
