using UnityEditor;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class PackageExporterWindow : EditorWindow
{
    private string[] packageNames;
    private int selectedPackageIndex = 0;

    [MenuItem("Tools/Export Selected Package")]
    public static void ShowWindow()
    {
        GetWindow<PackageExporterWindow>("Package Exporter");
    }

    private void OnEnable()
    {
        string packagesPath = "Packages";
        List<string> packageList = new List<string>();

        // Populate the list with names of all directories in the Packages folder
        foreach (var directory in Directory.GetDirectories(packagesPath))
        {
            packageList.Add(Path.GetFileName(directory));
        }

        packageNames = packageList.ToArray();
    }

    private void OnGUI()
    {
        GUILayout.Label("Select a Package to Export", EditorStyles.boldLabel);

        // Dropdown to select the package
        selectedPackageIndex = EditorGUILayout.Popup("Package", selectedPackageIndex, packageNames);

        // Export button
        if (GUILayout.Button("Export Package"))
        {
            ExportSelectedPackage();
        }
    }

    private void ExportSelectedPackage()
    {
        if (packageNames.Length == 0)
        {
            Debug.LogWarning("No packages available to export.");
            return;
        }


        string selectedPackage = packageNames[selectedPackageIndex];
        string packagesPath = Path.Combine("Packages", selectedPackage);

        // Open file dialog to choose where to save the exported package
        string packageExportPath = EditorUtility.SaveFilePanel("Save Package",
                                                               "",
                                                               selectedPackage + ".unitypackage",
                                                               "unitypackage");





        // Check if the user cancelled the file dialog
        if (string.IsNullOrEmpty(packageExportPath))
        {
            return;
        }

        // Export the selected package
        AssetDatabase.ExportPackage(packagesPath, packageExportPath, ExportPackageOptions.Recurse);
        Debug.Log("Exported package: " + packageExportPath);
    }
}
