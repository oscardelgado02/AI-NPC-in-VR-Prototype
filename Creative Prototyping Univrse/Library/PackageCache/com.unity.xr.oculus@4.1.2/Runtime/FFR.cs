using System;
using UnityEngine;
using UnityEngine.Android;

namespace Unity.XR.Oculus
{
    public static partial class Utils
    {
        static void PermissionGrantedCallback(string permissionName)
        {
            if (permissionName == "com.oculus.permission.EYE_TRACKING")
            {
                NativeMethods.SetEyeTrackedFoveatedRenderingEnabled(true);
            }
        }

        /// <summary>
        /// Returns true if the user has allowed the Eye Tracking permission.
        /// </summary>
        public static bool IsEyeTrackingPermissionGranted()
        {
            return Permission.HasUserAuthorizedPermission("com.oculus.permission.EYE_TRACKING");
        }

        /// <summary>
        /// Gets or sets dynamic foveated rendering which will change the foveated rendering level automatically based on the current performance. 
        /// Works for both FFR and ETFR.
        /// </summary>
        public static bool useDynamicFoveatedRendering
        {
            get
            {
                return NativeMethods.GetTiledMultiResSupported();
            }
            set
            {
                if (!NativeMethods.GetTiledMultiResSupported())
                {
                    Debug.LogWarning("Can't enable dynamic FFR on current platform");
                }

                NativeMethods.SetTiledMultiResDynamic(value);
            }
        }

        /// <summary>
        /// Gets or sets the foveated rendering level. This works for both FFR and ETFR
        ///  level can be 0, 1, 2, 3, or 4:
        ///
        /// * -1 (get) not supported on current platform
        /// * 0 foveated rendering disabled
        /// * 1 low foveated rendering level
        /// * 2 medium foveated rendering level
        /// * 3 high foveated rendering level
        /// * 4 high top foveated rendering level
        /// </summary>
        public static int foveatedRenderingLevel
        {
            get
            {
                if (!NativeMethods.GetTiledMultiResSupported())
                {
                    Debug.LogWarning("Can't get foveation level on current platform");
                    return -1;
                }

                return NativeMethods.GetTiledMultiResLevel();
            }
            set
            {
                if (!NativeMethods.GetTiledMultiResSupported())
                {
                    Debug.LogWarning("Can't set foveation level on current platform");
                }

                NativeMethods.SetTiledMultiResLevel(value);
            }
        }

        /// <summary>
        /// Returns whether or not eye tracked foveated rendering (ETFR) is supported.
        /// You cannot enable ETFR using eyeTrackedFoveatedRenderingEnabled if the feature isn't supported.
        /// </summary>
        public static bool eyeTrackedFoveatedRenderingSupported
        {
            get
            {
                return NativeMethods.GetEyeTrackedFoveatedRenderingSupported();
            }
        }

        /// <summary>
        /// Gets or sets whether eye tracked foveated rendering is enabled or not.
        /// </summary>
        public static bool eyeTrackedFoveatedRenderingEnabled
        {
            get
            {
                if (eyeTrackedFoveatedRenderingSupported)
                    return NativeMethods.GetEyeTrackedFoveatedRenderingEnabled();
                return false;
            }
            set
            {
                if (eyeTrackedFoveatedRenderingSupported)
                {
                    if (value)
                    {
                        if (IsEyeTrackingPermissionGranted())
                        {
                            NativeMethods.SetEyeTrackedFoveatedRenderingEnabled(value);
                        }
                        else
                        {
                            var permissionCallbacks = new PermissionCallbacks();
                            permissionCallbacks.PermissionGranted += PermissionGrantedCallback;
                            Permission.RequestUserPermission("com.oculus.permission.EYE_TRACKING", permissionCallbacks);
                        }
                    }
                    else
                    {
                        NativeMethods.SetEyeTrackedFoveatedRenderingEnabled(value);
                    }
                }
            }
        }

        /// <summary>
        /// Set the degree of foveation. Only supported on mobile. See [Oculus Documention](https://developer.oculus.com/documentation/quest/latest/concepts/mobile-ffr/).
        /// </summary>
        /// <param name="level">
        ///  level can be 0, 1, 2, 3, or 4:
        /// 
        /// * 0 disables foveated rendering
        /// * 1 low foveated rendering level
        /// * 2 medium foveated rendering level
        /// * 3 high foveated rendering level
        /// * 4 high top foveated rendering level
        /// </param>
        [Obsolete("Please use foveatedRenderingLevel instead.", false)]
        public static bool SetFoveationLevel(int level)
        {
            if (!NativeMethods.GetTiledMultiResSupported())
            {
                Debug.LogWarning("Can't set foveation level on current platform");
                return false;
            }

            NativeMethods.SetTiledMultiResLevel(level);
            return true;
        }


        /// <summary>
        /// Enable or disable dynamic foveated rendering. Only supported on mobile. See [Oculus Documention](https://developer.oculus.com/documentation/quest/latest/concepts/mobile-ffr/).
        /// </summary>
        /// <param name="enable">
        /// Set to true to enable dynamic foveated rendering or false to disable it.
        /// </param>
        [Obsolete("Please use useDynamicFoveatedRendering instead", false)]
        public static bool EnableDynamicFFR(bool enable)
        {
            if (!NativeMethods.GetTiledMultiResSupported())
            {
                Debug.LogWarning("Can't enable dynamic FFR on current platform");
                return false;
            }

            NativeMethods.SetTiledMultiResDynamic(enable);
            return true;
        }

        /// <summary>
        /// Returns the level of foveated rendering. Only supported on mobile. See [Oculus Documentation](https://developer.oculus.com/documentation/quest/latest/concepts/mobile-ffr/).
        /// </summary>
        /// <returns>
        /// * -1 error
        /// * 0 disables foveated rendering
        /// * 1 low foveated rendering level
        /// * 2 medium foveated rendering level
        /// * 3 high foveated rendering level
        /// * 4 high top foveated rendering level
        /// </returns>
        [Obsolete("Please use foveatedRenderingLevel instead.", false)]
        public static int GetFoveationLevel()
        {
            if (!NativeMethods.GetTiledMultiResSupported())
            {
                Debug.LogWarning("Can't get foveation level on current platform");
                return -1;
            }

            return NativeMethods.GetTiledMultiResLevel();
        }
    }
}
