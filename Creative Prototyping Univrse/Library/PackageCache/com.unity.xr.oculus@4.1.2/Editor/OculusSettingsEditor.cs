using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.XR.Oculus;

namespace Unity.XR.Oculus.Editor
{
    [CustomEditor(typeof(OculusSettings))]
    public class OculusSettingsEditor : UnityEditor.Editor
    {
        private const string kSharedDepthBuffer = "SharedDepthBuffer";
        private const string kDashSupport = "DashSupport";
        private const string kStereoRenderingModeDesktop = "m_StereoRenderingModeDesktop";
        private const string kStereoRenderingModeAndroid = "m_StereoRenderingModeAndroid";
        private const string kLowOverheadMode = "LowOverheadMode";
        private const string kOptimizeBufferDiscards = "OptimizeBufferDiscards";
        private const string kPhaseSync = "PhaseSync";
        private const string kSymmetricProjection = "SymmetricProjection";
        private const string kSubsampledLayout = "SubsampledLayout";
        private const string kFoveatedRenderingMethod = "FoveatedRenderingMethod";
        private const string kLateLatching = "LateLatching";
        private const string kLateLatchingDebug = "LateLatchingDebug";
        private const string kEnableTrackingOriginStageMode = "EnableTrackingOriginStageMode";
        private const string kSpaceWarp = "SpaceWarp";
        private const string kTargetQuest2 = "TargetQuest2";
        private const string kTargetQuestPro = "TargetQuestPro";
        private const string kTargetQuest3 = "TargetQuest3";
        private const string kSystemSplashScreen = "SystemSplashScreen";
        private const string kDepthSubmission = "DepthSubmission";

        static GUIContent s_SharedDepthBufferLabel = EditorGUIUtility.TrTextContent("Shared Depth Buffer");
        static GUIContent s_DashSupportLabel = EditorGUIUtility.TrTextContent("Dash Support");
        static GUIContent s_StereoRenderingModeLabel = EditorGUIUtility.TrTextContent("Stereo Rendering Mode");
        static GUIContent s_LowOverheadModeLabel = EditorGUIUtility.TrTextContent("Low Overhead Mode (GLES)");
        static GUIContent s_OptimizeBufferDiscardsLabel = EditorGUIUtility.TrTextContent("Optimize Buffer Discards (Vulkan)");
        static GUIContent s_PhaseSyncLabel = EditorGUIUtility.TrTextContent("Phase Sync (Deprecated)");
        static GUIContent s_SymmetricProjectionLabel = EditorGUIUtility.TrTextContent("Symmetric Projection (Vulkan)", "Supported when using Vulkan and Multiview.");
        static GUIContent s_SubsampledLayoutLabel = EditorGUIUtility.TrTextContent("Subsampled Layout (Vulkan)");
        static GUIContent s_FoveatedRenderingMethodLabel = EditorGUIUtility.TrTextContent("Foveated Rendering Method", "Choose which foveated rendering method is used when foveation is enabled. Eye Tracked Foveated Rendering is only supported on Quest Pro with proper permissions and when using Vulkan, Multiview, and ARM64.");
        static GUIContent s_LateLatchingLabel = EditorGUIUtility.TrTextContent("Late Latching (Vulkan)");
        static GUIContent s_LateLatchingDebugLabel = EditorGUIUtility.TrTextContent("Late Latching Debug Mode");
        static GUIContent s_TrackingOriginStageLabel = EditorGUIUtility.TrTextContent("Enable TrackingOrigin Stage Mode");
        static GUIContent s_SpaceWarpLabel = EditorGUIUtility.TrTextContent("Application SpaceWarp (Vulkan)");
        static GUIContent s_TargetDevicesLabel = EditorGUIUtility.TrTextContent("Target Devices");
        static GUIContent s_TargetQuest2Label = EditorGUIUtility.TrTextContent("Quest 2");
        static GUIContent s_TargetQuestProLabel = EditorGUIUtility.TrTextContent("Quest Pro");
        static GUIContent s_TargetQuest3Label = EditorGUIUtility.TrTextContent("Quest 3");
        static GUIContent s_SystemSplashScreen = EditorGUIUtility.TrTextContent("System Splash Screen");
        static GUIContent s_DepthSubmission = EditorGUIUtility.TrTextContent("Depth Submission (Vulkan)");
        static GUIContent s_ShowAndroidExperimentalLabel = EditorGUIUtility.TrTextContent("Experimental", "Experimental settings that are under active development and should be used with caution.");

        private SerializedProperty m_SharedDepthBuffer;
        private SerializedProperty m_DashSupport;
        private SerializedProperty m_StereoRenderingModeDesktop;
        private SerializedProperty m_StereoRenderingModeAndroid;
        private SerializedProperty m_LowOverheadMode;
        private SerializedProperty m_OptimizeBufferDiscards;
        private SerializedProperty m_PhaseSync;
        private SerializedProperty m_SymmetricProjection;
        private SerializedProperty m_SubsampledLayout;
        private SerializedProperty m_FoveatedRenderingMethod;
        private SerializedProperty m_LateLatching;
        private SerializedProperty m_LateLatchingDebug;
        private SerializedProperty m_EnableTrackingOriginStageMode;
        private SerializedProperty m_SpaceWarp;
        private SerializedProperty m_TargetQuest2;
        private SerializedProperty m_TargetQuestPro;
        private SerializedProperty m_TargetQuest3;
        private SerializedProperty m_SystemSplashScreen;
        private SerializedProperty m_DepthSubmission;

        static private bool m_ShowAndroidExperimental = false;

        public override void OnInspectorGUI()
        {
            if (serializedObject == null || serializedObject.targetObject == null)
                return;

            if (m_SharedDepthBuffer == null) m_SharedDepthBuffer = serializedObject.FindProperty(kSharedDepthBuffer);
            if (m_DashSupport == null) m_DashSupport = serializedObject.FindProperty(kDashSupport);
            if (m_StereoRenderingModeDesktop == null) m_StereoRenderingModeDesktop = serializedObject.FindProperty(kStereoRenderingModeDesktop);
            if (m_StereoRenderingModeAndroid == null) m_StereoRenderingModeAndroid = serializedObject.FindProperty(kStereoRenderingModeAndroid);
            if (m_LowOverheadMode == null) m_LowOverheadMode = serializedObject.FindProperty(kLowOverheadMode);
            if (m_OptimizeBufferDiscards == null) m_OptimizeBufferDiscards = serializedObject.FindProperty(kOptimizeBufferDiscards);
            if (m_PhaseSync == null) m_PhaseSync = serializedObject.FindProperty(kPhaseSync);
            if (m_SymmetricProjection == null) m_SymmetricProjection = serializedObject.FindProperty(kSymmetricProjection);
            if (m_SubsampledLayout == null) m_SubsampledLayout = serializedObject.FindProperty(kSubsampledLayout);
            if (m_FoveatedRenderingMethod == null) m_FoveatedRenderingMethod = serializedObject.FindProperty(kFoveatedRenderingMethod);
            if (m_LateLatching == null) m_LateLatching = serializedObject.FindProperty(kLateLatching);
            if (m_LateLatchingDebug == null) m_LateLatchingDebug = serializedObject.FindProperty(kLateLatchingDebug);
            if (m_EnableTrackingOriginStageMode == null) m_EnableTrackingOriginStageMode = serializedObject.FindProperty(kEnableTrackingOriginStageMode);
            if (m_SpaceWarp == null) m_SpaceWarp = serializedObject.FindProperty(kSpaceWarp);
            if (m_TargetQuest2 == null) m_TargetQuest2 = serializedObject.FindProperty(kTargetQuest2);
            if (m_TargetQuestPro == null) m_TargetQuestPro = serializedObject.FindProperty(kTargetQuestPro);
            if (m_TargetQuest3 == null) m_TargetQuest3 = serializedObject.FindProperty(kTargetQuest3);
            if (m_SystemSplashScreen == null) m_SystemSplashScreen = serializedObject.FindProperty(kSystemSplashScreen);
            if (m_DepthSubmission == null) m_DepthSubmission = serializedObject.FindProperty(kDepthSubmission);

            serializedObject.Update();

            EditorGUIUtility.labelWidth = 240.0f;

            BuildTargetGroup selectedBuildTargetGroup = EditorGUILayout.BeginBuildTargetSelectionGrouping();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorGUILayout.HelpBox("Oculus settings cannnot be changed when the editor is in play mode.", MessageType.Info);
                EditorGUILayout.Space();
            }
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            if (selectedBuildTargetGroup == BuildTargetGroup.Standalone)
            {
                EditorGUILayout.PropertyField(m_StereoRenderingModeDesktop, s_StereoRenderingModeLabel);
                EditorGUILayout.PropertyField(m_SharedDepthBuffer, s_SharedDepthBufferLabel);
                EditorGUILayout.PropertyField(m_DashSupport, s_DashSupportLabel);
            }
            else if (selectedBuildTargetGroup == BuildTargetGroup.Android)
            {
                EditorGUILayout.PropertyField(m_StereoRenderingModeAndroid, s_StereoRenderingModeLabel);
                EditorGUILayout.PropertyField(m_LowOverheadMode, s_LowOverheadModeLabel);
                EditorGUILayout.PropertyField(m_OptimizeBufferDiscards, s_OptimizeBufferDiscardsLabel);
                EditorGUILayout.PropertyField(m_PhaseSync, s_PhaseSyncLabel);
                EditorGUILayout.PropertyField(m_SymmetricProjection, s_SymmetricProjectionLabel);
                EditorGUILayout.PropertyField(m_SubsampledLayout, s_SubsampledLayoutLabel);
                EditorGUILayout.PropertyField(m_FoveatedRenderingMethod, s_FoveatedRenderingMethodLabel);
                EditorGUILayout.PropertyField(m_EnableTrackingOriginStageMode, s_TrackingOriginStageLabel);
                EditorGUILayout.PropertyField(m_DepthSubmission, s_DepthSubmission);
                EditorGUILayout.PropertyField(m_LateLatching, s_LateLatchingLabel);
                EditorGUILayout.PropertyField(m_LateLatchingDebug, s_LateLatchingDebugLabel);

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_SystemSplashScreen, s_SystemSplashScreen);
                EditorGUILayout.Space();

                GUILayout.Label(s_TargetDevicesLabel, EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(m_TargetQuest2, s_TargetQuest2Label);
                EditorGUILayout.PropertyField(m_TargetQuestPro, s_TargetQuestProLabel);
                EditorGUILayout.PropertyField(m_TargetQuest3, s_TargetQuest3Label);

                EditorGUILayout.Space();

                if (m_ShowAndroidExperimental = EditorGUILayout.Foldout(m_ShowAndroidExperimental, s_ShowAndroidExperimentalLabel))
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(m_SpaceWarp, s_SpaceWarpLabel);
                    EditorGUI.indentLevel--;
                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndBuildTargetSelectionGrouping();

            serializedObject.ApplyModifiedProperties();

            EditorGUIUtility.labelWidth = 0.0f;
        }
    }
}
