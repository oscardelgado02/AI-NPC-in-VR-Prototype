#if XR_CORE_UTILS && UNITY_EDITOR

using System.Linq;
using Unity.XR.CoreUtils.Editor;
using Unity.XR.Oculus;
using UnityEditor.XR.Management;
using UnityEngine.Rendering;

namespace UnityEditor.XR.Oculus
{
    [InitializeOnLoad]
    internal static class OculusProjectValidator
    {
        private const string ambientOcclusionScriptName = "ScreenSpaceAmbientOcclusion";
        private static readonly BuildTargetGroup[] buildTargetGroups = new BuildTargetGroup[] { BuildTargetGroup.Standalone, BuildTargetGroup.Android };

        static OculusProjectValidator() => AddValidationRules();

        private static void AddValidationRules()
        {
            foreach (var buildTargetGroup in buildTargetGroups)
            {
                var buildTargetRules = new BuildValidationRule[]
                {
                    new BuildValidationRule()
                    {
                        Message = "Using the Screen Space Ambient Occlusion render feature results in significant performance overhead when the application is running natively on device. Disabling or removing that render feature is recommended.",
                        Category = "Oculus",
                        HelpText = "Only removing the Screen Space Ambient Occlusion render feature from all UniversalRenderer assets that may be used will make this warning go away, but just disabling the render feature will still prevent the performance overhead.",
                        FixItAutomatic = false,
                        IsRuleEnabled = () =>
                        {
                            var settings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(buildTargetGroup);

                            if (settings)
                                return settings.Manager.activeLoaders.Any(typeof(OculusLoader).IsInstanceOfType);

                            return false;
                        },
                        CheckPredicate = () =>
                        {
                            // Checks the dependencies of all configured render pipeline assets.
                            foreach(var renderPipeline in GraphicsSettings.allConfiguredRenderPipelines)
                            {
                                var dependencies = AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(renderPipeline));
                                foreach(var dependency in dependencies)
                                {
                                    if (dependency.Contains(ambientOcclusionScriptName))
                                        return false;
                                }
                            }

                            return true;
                        }
                    }
                };

                BuildValidator.AddRules(buildTargetGroup, buildTargetRules);
            }

        }
    }
}
#endif