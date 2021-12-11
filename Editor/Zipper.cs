using UnityEngine;
using UnityEditor;
using System.IO;
using System.IO.Compression;
using System;
using UnityEditor.Callbacks;

namespace AarquieSolutions.ZipProjectTool
{
    public class Zipper : EditorWindow
    {
        private const string AssetsSubDirectory = "Assets";
        private const string ProjectSettingsSubDirectory = "ProjectSettings";
        private const string PackagesSubDirectory = "Packages";
        private const string TempSubDirectory = "Temp";
        private const string TempProjectSubDirectory = "Project";
        private const string ZipArchiveFolder = "Zip Archive";

        private static readonly string projectPath = Application.dataPath.Replace($"/{AssetsSubDirectory}", "");
        private static readonly string tempPath = $"{projectPath}/{ TempSubDirectory}";
        private static readonly string tempProjectPath = $"{tempPath }/{ TempProjectSubDirectory}";

        private static string fileName;

        [MenuItem("Window/Zip Project/Zip")]
        private static void ZipProject()
        {
            string fileName = $"{tempPath}/{ZipArchiveFolder}/{Application.productName} {DateTime.Now:MM_dd_yy_hh_mm_ss}";
            ZipProject(fileName);
        }

        [MenuItem("Window/Zip Project/Settings")]
        public static void ShowWindow()
        {
            GetWindow(typeof(Zipper));
        }

        private static void ZipProject(string fileName)
        {
            try
            {
                DirectoryInfo sourceAssets = new DirectoryInfo($"{projectPath}/{AssetsSubDirectory}");
                DirectoryInfo sourceProjectSettings = new DirectoryInfo($"{projectPath}/{ProjectSettingsSubDirectory}");
                DirectoryInfo sourcePackage = new DirectoryInfo($"{projectPath}/{ PackagesSubDirectory}");

                DirectoryInfo targetProjectSettings = Directory.CreateDirectory($"{tempProjectPath }/{ ProjectSettingsSubDirectory}");
                DirectoryInfo targetPackage = Directory.CreateDirectory($"{tempProjectPath }/{ PackagesSubDirectory}");
                DirectoryInfo targetAssets = Directory.CreateDirectory($"{tempProjectPath }/{ AssetsSubDirectory}");

                CopyDirectory.CopyAll(sourceAssets, targetAssets);
                CopyDirectory.CopyAll(sourceProjectSettings, targetProjectSettings);
                CopyDirectory.CopyAll(sourcePackage, targetPackage);

                Directory.CreateDirectory($"{tempPath}/{ZipArchiveFolder}");
                ZipFile.CreateFromDirectory(tempProjectPath, $"{fileName}.zip");

                Directory.Delete(tempProjectPath, true);

                Debug.Log($"Copying and zipping complete. File location:{tempPath}/{ZipArchiveFolder}");
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        private void OnGUI()
        {
            fileName = Application.productName;
            GUILayout.Label("Settings", EditorStyles.boldLabel);
            fileName = EditorGUILayout.TextField("Text Field", fileName);

            if (GUILayout.Button("Zip"))
            {
                ZipProject(fileName);
            }
        }

        [PostProcessBuildAttribute(1)]
        private static void ZipBuild(BuildTarget buildTarget, string pathToBuild)
        {
            string sourcePath = pathToBuild.Replace($"/{Application.productName}.exe","");
            string destinationPath = $"{tempPath}/Build{buildTarget}.zip";
            
            if (File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
            }
            
            ZipFile.CreateFromDirectory(sourcePath, destinationPath);
            Debug.Log($"Created zip archive of build. File location:{tempPath}");
        }
    }
}