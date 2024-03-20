using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SpeechLib;
using OpenAI;

public class SpeechToAnswerSystem : MonoBehaviour
{
    //--------------ATTRIBUTES-----------------

    //NPC Control
    public bool audioProcessing = false;
    public bool initialConversation = false;

    //Pop Ups
    private enum popUpsIdx { listening, processing, speaking }
    [SerializeField] private List<GameObject> uiPopUps;

    //Windows synth
    public SpVoice voice = new SpVoice();

    //ChatGPT Models
    private OpenAIApi openai = new OpenAIApi();
    private List<ChatMessage> messages = new List<ChatMessage>();
    private string prompt = "A partir de ahora, eres un romano del año 53 AC. Te llamas Atreus, y tienes un buen sentido del humor. " +
        "No introduzcas nunca números en el texto, si quieres decir un número, hazlo con palabras. Por ejemplo, en vez de 53, escribe cincuenta y tres. " +
        "Utiliza frases cortas. " +
        "No rompas el personaje. No sabes nada acerca del futuro, ni siquiera en que año estás, solo que un loco de la plaza dice que es el año 53 antes de Cristo," +
        " pero nadie sabe quien es Cristo. También crees que yo también soy otro ciudadano más. No digas que pertenecemos al pasado, vivelo como si para ti fuese el presente. " +
        "Nunca menciones que eres un modelo de IA. Ahora, presentate después de la frase que te diré a continuación.";

    //ElevenLabs API
    [SerializeField] private ElevenlabsAPI elevenlabsAPI;

    //AudioSource
    [SerializeField] private AudioSource audioSource;

    //--------------METHODS-----------------

    private void Start()
    {
        //ElevenLabs
        ElevenlabsAPI.OnRequestCompleted += PlayAudioSource;
        ElevenlabsAPI.OnRequestFailed += StopAudioSource;

        ActivatePopUp(-1);  //Hide all PopUps
    }

    private void Update() { CheckIfNPCIsSpeaking(); }

    public void GenerateInitialConversation()
    {
        if (!initialConversation)
        {
            initialConversation = true;
            audioProcessing = true;
            GenerateNPCAnswer("Hola!");
        }
    }

    //GenerateNPCAnswer with speech
    public void GenerateNPCAnswer(float[] inputAudio, int frequency, int channels)
    {
        audioProcessing = true;
        WhisperGPT(inputAudio, frequency, channels);
    }

    //GenerateNPCAnswer only with text
    private void GenerateNPCAnswer(string inputText)
    {
        audioProcessing = true;
        SendChatGPTReply(inputText);
    }

    //Generate voice with text
    public void StartNPCVoice(string inputText)
    {
        if (Settings.Instance.useElevenLabs)
            elevenlabsAPI.GetAudio(inputText);
        else
            StartWindowsSynthVoice(inputText);
    }

    public void StopNPCVoice()
    {
        ActivatePopUp(-1);  //Hide all PopUps
        if (Settings.Instance.useElevenLabs)
            StopAudioSource();
        else
            MuteWindowsSynthVoice();
    }

    //-------ChatGPT-------

    private async void WhisperGPT(float[] inputAudio, int frequency, int channels)
    {
        ActivatePopUp((int)popUpsIdx.listening);    //We activate the listening popUp

        var req = new CreateAudioTranscriptionsRequest
        {
            FileData = new FileData() { Data = EncodeAsWAV(inputAudio, frequency, channels), Name = "audio.wav" },
            // File = Application.persistentDataPath + "/" + fileName,
            Model = "whisper-1",
            Language = "es"
        };
        var res = await openai.CreateAudioTranscription(req);

        if (res.Text.ToLower().Contains("amara.org"))
            StartNPCVoice("Perdona, no te he entendido, ¿puedes repetirmelo?");
        else
            SendChatGPTReply(res.Text);
    }

    private async void SendChatGPTReply(string inputText)
    {
        ActivatePopUp((int)popUpsIdx.processing);    //We activate the listening popUp

        var newMessage = new ChatMessage()
        {
            Role = "user",
            Content = inputText
        };

        if (messages.Count == 0) newMessage.Content = prompt + "\n" + inputText;

        messages.Add(newMessage);

        // Complete the instruction
        var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
        {
            Model = "gpt-3.5-turbo-0613",
            Messages = messages
        });

        if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
        {
            var message = completionResponse.Choices[0].Message;
            message.Content = message.Content.Trim();

            messages.Add(message);
            StartNPCVoice(message.Content);
        }
        else
        {
            Debug.LogWarning("No text was generated from this prompt.");
            audioProcessing = false; //Break the process
        }
    }

    //AudioSource
    private void PlayAudioSource(AudioClip audioClip)
    {
        audioProcessing = false; //Break the process
        ActivatePopUp((int)popUpsIdx.speaking);
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    private void StopAudioSource()
    {
        audioProcessing = false; //Break the process
        audioSource.Stop();
    }

    //Windows synth
    private void StartWindowsSynthVoice(string inputText)
    {
        audioProcessing = false; //Break the process
        ActivatePopUp((int)popUpsIdx.speaking);
        voice.Speak(inputText, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
    }
    private void MuteWindowsSynthVoice()
    {
        audioProcessing = false; //Break the process
        voice.Skip("Sentence", int.MaxValue);
    }

    //PopUps
    private void ActivatePopUp(int idx)
    {
        for (int i = 0; i < uiPopUps.Count; i++)
        {
            uiPopUps[i].SetActive(i == idx);
        }
    }

    private void CheckIfNPCIsSpeaking()
    {
        if (!audioProcessing)
        {
            if (Settings.Instance.useElevenLabs && !audioSource.isPlaying)
                ActivatePopUp(-1);  //Hide the PopUps
            else if (!Settings.Instance.useElevenLabs && voice.Status.RunningState == SpeechRunState.SRSEDone)
                ActivatePopUp(-1);  //Hide the PopUps
        }
    }

    //AudioClip to WAV
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
