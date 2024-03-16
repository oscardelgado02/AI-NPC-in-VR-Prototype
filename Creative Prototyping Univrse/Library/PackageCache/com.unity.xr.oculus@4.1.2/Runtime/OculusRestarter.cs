using System;
using System.Collections;

using UnityEngine;
using UnityEngine.XR.Management;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Unity.XR.Oculus
{
    internal class OculusRestarter : MonoBehaviour
    {
        internal Action onAfterRestart;
        internal Action onAfterShutdown;
        internal Action onQuit;
        internal Action onAfterCoroutine;

        static readonly String k_GameObjectName = "~oculusrestarter";

        static OculusRestarter()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += (state) =>
            {
                if (state == PlayModeStateChange.EnteredPlayMode)
                    m_pauseAndRestartAttempts = 0;
            };
#endif
            TimeBetweenRestartAttempts = 5.0f;
        }


        public void ResetCallbacks ()
        {
            onAfterRestart = null;
            onAfterShutdown = null;
            onAfterCoroutine = null;
            onQuit = null;
            m_pauseAndRestartAttempts = 0;
        }

        /// <summary>
        /// True if the restarter is currently running
        /// </summary>
        public bool isRunning => m_Coroutine != null;

        private static OculusRestarter s_Instance = null;

        private Coroutine m_Coroutine;

        private Coroutine m_pauseAndRestartCoroutine;

        private static int m_pauseAndRestartAttempts = 0;

        public static float TimeBetweenRestartAttempts
        {
            get;
            set;
        }

        public static int PauseAndRestartAttempts
        {
            get
            {
                return m_pauseAndRestartAttempts;
            }
        }

        public static OculusRestarter Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    var go = GameObject.Find(k_GameObjectName);
                    if (go == null)
                    {
                        go = new GameObject(k_GameObjectName);
                        go.hideFlags = HideFlags.HideAndDontSave;
                        go.AddComponent<OculusRestarter>();
                    }
                    s_Instance = go.GetComponent<OculusRestarter>();
                }
                return s_Instance;
            }
        }

        /// <summary>
        /// Pause and then restart.
        /// If the restart triggers another restart, the pause adds some delay between restarts.
        /// </summary>
        public void PauseAndRestart()
        {
            if (m_pauseAndRestartCoroutine != null)
            {
                Debug.LogError("Only one pause then shutdown/restart can be executed at a time");
                return;
            }

            Debug.Log("Please make sure the device is connected. Will try to restart xr periodically.");
            m_pauseAndRestartCoroutine = StartCoroutine(PauseAndRestartCoroutine(TimeBetweenRestartAttempts));
        }

        public IEnumerator PauseAndRestartCoroutine(float pauseTimeInSeconds)
        {
            try
            {
                yield return new WaitForSeconds(pauseTimeInSeconds);
                m_pauseAndRestartAttempts += 1;
                if (m_Coroutine == null)
                {
                    m_Coroutine = StartCoroutine(RestartCoroutine(true));
                }
                else
                {
                    Debug.LogError(String.Format("Restart/Shutdown already in progress so skipping this attempt."));
                }
            }
            finally
            {
                m_pauseAndRestartCoroutine = null;
                onAfterCoroutine?.Invoke();
            }
        }

        private IEnumerator RestartCoroutine (bool shouldRestart)
        {
            try
            {
                yield return null;

                // Always shutdown the loader
                XRGeneralSettings.Instance.Manager.DeinitializeLoader();
                yield return null;

                onAfterShutdown?.Invoke();

                // Restart?
                if (shouldRestart)
                {
                    yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

                    XRGeneralSettings.Instance.Manager.StartSubsystems();

                    if (XRGeneralSettings.Instance.Manager.activeLoader == null)
                    {
                        Debug.LogError("Failure to restart OculusLoader after shutdown.");
                    }
                    else
                    {
                        Debug.Log("OculusLoader restart successful.");
                        m_pauseAndRestartAttempts = 0;
                    }

                    onAfterRestart?.Invoke();
                }
            }
            finally
            {
                m_Coroutine = null;
                onAfterCoroutine?.Invoke();

                // this is invoked if the restart failed, typically because Oculus Link still hasn't been restored.
                if (XRGeneralSettings.Instance.Manager.activeLoader == null) {
                    PauseAndRestart();
                }
            }
        }
    }
}
