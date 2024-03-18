using HuggingFace.API;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SpeechLib;

public class SpeechToAnswerSystem
{
    //Singleton
    private SpeechToAnswerSystem()
    {
        //conversation = new Conversation();
    }

    private static SpeechToAnswerSystem instance;
    public static SpeechToAnswerSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SpeechToAnswerSystem();
            }
            return instance;
        }
    }

    //private Conversation conversation;
    public SpVoice voice = new SpVoice();
    public bool audioProcessing = true;

    private bool[] modelInitialization = new bool[] { false, false };
    private enum modelsIdx
    {
        speechRecognition,
        textGeneration
    }

    public void InitModels(float[] samples, int numChannels)
    {
        //Speech Recognition
        InitSpeechRecognition(samples, numChannels);

        //Text Generation
        InitTextGeneration();
    }

    private void InitSpeechRecognition(float[] samples, int numChannels)
    {
        HuggingFaceAPI.AutomaticSpeechRecognition(EncodeAsWAV(samples, AudioSettings.outputSampleRate, numChannels), response => {
            Debug.Log("SpeechRecognition initialized");
            modelInitialization[(int)modelsIdx.speechRecognition] = true;
            StartPlayerInteraction();
        }, error => {
            Debug.Log("SpeechRecognition NOT initialized");
            InitSpeechRecognition(samples, numChannels);
        });
    }

    private void InitTextGeneration()
    {
        HuggingFaceAPI.TextGeneration("Hola", response =>
        {
            Debug.Log($"TextGeneration initialized");
            modelInitialization[(int)modelsIdx.textGeneration] = true;
            StartPlayerInteraction();
        }, error =>
        {
            Debug.Log("TextGeneration NOT initialized");
            InitTextGeneration();
        });
    }

    private void StartPlayerInteraction()
    {
        bool status = false;
        foreach(bool modelStatus in modelInitialization)
        {
            status |= !modelStatus;
        }
        audioProcessing = status;
    }

    public void ListenPlayer(float[] inputAudio, int frequency, int channels)
    {
        audioProcessing = true;
        string speechToTextResult = string.Empty;
        HuggingFaceAPI.AutomaticSpeechRecognition(EncodeAsWAV(inputAudio, frequency, channels), response => {
            speechToTextResult = response;
            Debug.Log($"User: {speechToTextResult}");
            GenerateBotAnswer(speechToTextResult);
        }, error => {
            audioProcessing = false; //Break the process
        });
    }

    private void GenerateBotAnswer(string inputText)
    {
        if (string.IsNullOrEmpty(inputText))
        {
            //We break the wait
            Debug.Log("Perdona, no te he entendido.");
            audioProcessing = false;
        }

        //En caso de que si que se entienda la frase
        else
        {
            string conversationOutput = string.Empty;
            HuggingFaceAPI.TextGeneration(inputText, response =>
            {
                conversationOutput = response;
                Debug.Log($"NPC: {conversationOutput}");
                AnswerToPlayer(conversationOutput);   //Sound of the npc
                audioProcessing = false; //Break the process
            }, error =>
            {
                audioProcessing = false; //Break the process
            });
        }
    }

    private void AnswerToPlayer(string inputText)
    {
        if (!string.IsNullOrEmpty(inputText))
        {
            voice.Speak(inputText, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
        }
    }

    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
    {
        using (var memoryStream = new MemoryStream(44 + samples.Length * 2))
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);

                foreach (var sample in samples)
                {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return memoryStream.ToArray();
        }
    }

    public void MuteVoice()
    {
        voice.Skip("Sentence", int.MaxValue);
    }
}
