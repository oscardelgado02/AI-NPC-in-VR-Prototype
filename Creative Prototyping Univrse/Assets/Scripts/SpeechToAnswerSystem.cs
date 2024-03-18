using HuggingFace.API;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeechToAnswerSystem
{
    //Singleton
    private SpeechToAnswerSystem()
    {
        conversation = new Conversation();
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

    private Conversation conversation;
    public bool audioProcessing = false;

    public void ListenPlayer(float[] inputAudio, int frequency, int channels)
    {
        audioProcessing = true;
        string speechToTextResult = string.Empty;
        HuggingFaceAPI.AutomaticSpeechRecognition(EncodeAsWAV(inputAudio, frequency, channels), response => {
            speechToTextResult = response;
            GenerateBotAnswer(speechToTextResult);
        }, error => {
            audioProcessing = false; //Break the process
        });
    }

    private void GenerateBotAnswer(string inputText)
    {
        string conversationOutput = string.Empty;

        if (string.IsNullOrEmpty(inputText))
        {
            //We break the wait
            Debug.Log("Perdona, no te he entendido.");
            audioProcessing = false;
        }

        //En caso de que si que se entienda la frase
        else
        {
            //HuggingFaceAPI.Conversation(inputText, response => {
            //    string reply = response;
            //    Debug.Log(reply);
            //}, error => {
            //});
            conversationOutput = "Answer to question: " + inputText;
            Debug.Log(conversationOutput);

            //We break the wait
            audioProcessing = false;
        }
    }

    private void AnswerToPlayer(string inputText)
    {
        if (!string.IsNullOrEmpty(inputText))
        {

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
}
