using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils.Collections;
using UnityEngine;

public class PlayerSpeechRecord : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textDebug;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private int sampleWindow = 64;
    [SerializeField] private float loudnessSensibility = 10f;
    [SerializeField] private float threshold = 0.35f;

    [StringInList(typeof(PropertyDrawersHelper), "MicrophoneOptions")]
    public string microphoneOption;

    public static AudioClip microphoneClip;
    private bool speechClipRecording = false;

    [SerializeField] private int maxSecondsTalking = 5;
    [SerializeField] private int maxSecondsBeingSilent = 1;

    private int timerTalking;
    private int timerBeingSilent;

    private int startPositionClip = 0;

    private void Start()
    {
        if (microphoneOption != null || microphoneOption == "") { Debug.Log("Selected option: " + microphoneOption); }
        else { microphoneOption = Microphone.devices[0]; }

        //We init the timers
        timerTalking = Timers.Instance.CreateTimer(true);
        timerBeingSilent = Timers.Instance.CreateTimer(true);

        RecordLoudness();

        //We init the models
        SpeechToAnswerSystem.Instance.InitModels(GetSampleDataFromMicrophone(), microphoneClip.channels);
    }

    private void Update()
    {
        if (Microphone.GetPosition(microphoneOption) >= microphoneClip.samples)
        {
            RecordLoudness();
        }

        if (!SpeechToAnswerSystem.Instance.audioProcessing && !speechClipRecording && GetIfLoudnessGreaterThanThreshold())
        {
            speechClipRecording = true;
            startPositionClip = Microphone.GetPosition(microphoneOption);

            //Mute the npc
            SpeechToAnswerSystem.Instance.MuteVoice();
        }

        if (speechClipRecording)
        {
            if (Timers.Instance.WaitTime(timerTalking, maxSecondsTalking))
                SendAudioAndDetectAgain();
            else if (GetIfLoudnessGreaterThanThreshold())
                Timers.Instance.ResetTimer(timerBeingSilent);
            else if (Timers.Instance.WaitTime(timerBeingSilent, maxSecondsBeingSilent))
                SendAudioAndDetectAgain();
        }

        //Just for debugging
        if (!SpeechToAnswerSystem.Instance.audioProcessing && GetIfLoudnessGreaterThanThreshold())
            textDebug.text = "Talking";
        else
            textDebug.text = "No Talking";
    }

    private void SendAudioAndDetectAgain()
    {
        speechClipRecording = false;

        Timers.Instance.ResetTimer(timerTalking);
        Timers.Instance.ResetTimer(timerBeingSilent);

        ProcessAudio();
        RecordLoudness();
    }

    //-------------Loudness Detection---------------

    private void RecordLoudness()
    {
        Microphone.End(microphoneOption);
        microphoneClip = Microphone.Start(microphoneOption, false, 20, AudioSettings.outputSampleRate);
    }

    private bool GetIfLoudnessGreaterThanThreshold()
    {
        float loudness = GetLoudnessFromMicrophone() * loudnessSensibility;
        return loudness > threshold;
    }

    private float GetLoudnessFromMicrophone()
    {
        return GetLoudnessFromAudioClip(Microphone.GetPosition(microphoneOption), microphoneClip);
    }

    private float GetLoudnessFromAudioClip(int clipPosition, AudioClip clip)
    {
        int startPosition = clipPosition - sampleWindow;

        if (startPosition > 0)
        {
            float[] waveData = new float[sampleWindow];
            clip.GetData(waveData, startPosition);

            float totalLoudness = 0f;
            for (int i = 0; i < sampleWindow; i++)
            {
                totalLoudness += Mathf.Abs(waveData[i]);
            }

            return totalLoudness / sampleWindow;
        }

        return 0f;
    }

    //-------------Process Speech---------------

    private float[] GetSampleDataFromMicrophone()
    {
        float[] samples = new float[(microphoneClip.samples - Microphone.GetPosition(microphoneOption)) * microphoneClip.channels];
        microphoneClip.GetData(samples, startPositionClip);
        return samples;
    }

    private void ProcessAudio()
    {
        // Extract sub-audio clip starting from the beginning of the recorded clip
        //AudioClip subClip = AudioClip.Create("SubClip", microphoneClip.samples - Microphone.GetPosition(microphoneOption), microphoneClip.channels, microphoneClip.frequency, false);
        float[] samples = GetSampleDataFromMicrophone();
        //subClip.SetData(samples, 0);

        //audioSource.clip = subClip;
        //audioSource.Play();

        SpeechToAnswerSystem.Instance.ListenPlayer(samples, AudioSettings.outputSampleRate, microphoneClip.channels);
    }

    private void OnApplicationQuit()
    {
        //Mute the npc
        SpeechToAnswerSystem.Instance.MuteVoice();
    }
}
