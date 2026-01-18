using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace Rossoforge.Addressables.Editor
{
    [InitializeOnLoad]
    public static class AddressablesDefineGenerator
    {
        static ListRequest listRequest;

        static AddressablesDefineGenerator()
        {
            listRequest = Client.List();
            EditorApplication.update += Progress;
        }

        static void Progress()
        {
            if (!listRequest.IsCompleted)
                return;

            if (listRequest.Status == StatusCode.Success)
            {
                bool installed = false;

                foreach (var package in listRequest.Result)
                {
                    if (package.name == "com.unity.addressables")
                    {
                        installed = true;
                        break;
                    }
                }

                SetDefine(installed);
            }

            EditorApplication.update -= Progress;
        }

        static void SetDefine(bool installed)
        {
            var target = NamedBuildTarget.FromBuildTargetGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup
            );

            // Nuevo API: devuelve un string, no una lista
            string defines = PlayerSettings.GetScriptingDefineSymbols(target);

            bool hasDefine = defines.Contains("HAS_ADDRESSABLES");

            if (installed && !hasDefine)
            {
                if (!string.IsNullOrEmpty(defines))
                    defines += ";HAS_ADDRESSABLES";
                else
                    defines = "HAS_ADDRESSABLES";

                PlayerSettings.SetScriptingDefineSymbols(target, defines);
            }
            else if (!installed && hasDefine)
            {
                defines = string.Join(";", defines
                    .Split(';')
                    .Where(d => d != "HAS_ADDRESSABLES"));

                PlayerSettings.SetScriptingDefineSymbols(target, defines);
            }
        }
    }
}
