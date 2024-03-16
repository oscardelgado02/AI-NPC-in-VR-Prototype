using UnityEngine;

namespace Unity.XR.Oculus
{
    internal static class OculusSession
    {
        public static void Update()
        {
            if (NativeMethods.GetShouldRestartSession()) {
                OculusRestarter.Instance.PauseAndRestart();
            }
        }
    }
}