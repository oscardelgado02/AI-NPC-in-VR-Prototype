/**
 * This is a modification from the original script, this way the API is ensured and not hardcoded.
 * Also, it removes the accents from the input text.
 * Modification made by: https://github.com/oscardelgado02
 * Original script: https://www.davideaversa.it/blog/elevenlabs-text-to-speech-unity-script/
 */

/**
 * An example script on how to use ElevenLabs APIs in a Unity script.
 *
 * More info at https://www.davideaversa.it/blog/elevenlabs-text-to-speech-unity-script/
 */

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class ElevenlabsAPI : MonoBehaviour
{
    private string _apiKey;
    [SerializeField] private string _voiceId;
    [SerializeField] private string _apiUrl = "https://api.elevenlabs.io";
    [SerializeField] private string _model = "eleven_multilingual_v1";

    [Range(0, 4)]
    public int LatencyOptimization;

    // Define a delegate for the callback function
    public delegate void RequestCompleted(AudioClip audioClip);

    // Define an event based on the delegate
    public static event RequestCompleted OnRequestCompleted;

    // Define a delegate for the callback function
    public delegate void RequestFailed();

    // Define an event based on the delegate
    public static event RequestFailed OnRequestFailed;

    private void Start()
    {
        LoadApiKey();
    }

    private void LoadApiKey()
    {
        // Get the current user's folder
        string userFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);

        // Path to the JSON file inside the .elevenlabs folder
        string path = Path.Combine(userFolder, ".elevenlabs", "auth.json");

        if (File.Exists(path))
        {
            string jsonString = File.ReadAllText(path);

            // Deserialize JSON to retrieve the API key
            ApiKeyData apiKeyData = JsonConvert.DeserializeObject<ApiKeyData>(jsonString);

            // Assuming your JSON structure is {"api_key": "your_api_key_value"}
            _apiKey = apiKeyData.api_key;

            //Debug.Log("API Key: " + _apiKey);
        }
        else
        {
            Debug.LogError("JSON file not found at path: " + path);
        }
    }

    public void GetAudio(string text)
    {
        string cleanText = RemoveAccents(text);
        StartCoroutine(DoRequest(cleanText));
    }

    IEnumerator DoRequest(string message)
    {
        var postData = new TextToSpeechRequest
        {
            text = message,
            model_id = _model
        };

        // TODO: This could be easily exposed in the Unity inspector,
        // but I had no use for it in my work demo.
        var voiceSetting = new VoiceSettings
        {
            stability = 0,
            similarity_boost = 0,
            style = 0.5f,
            use_speaker_boost = true
        };
        postData.voice_settings = voiceSetting;
        var json = JsonConvert.SerializeObject(postData);
        var url = $"{_apiUrl}/v1/text-to-speech/{_voiceId}?optimize_streaming_latency={LatencyOptimization}";

        using (var postRequest = UnityWebRequest.PostWwwForm(url, "POST"))
        {
            //Start the request with a method instead of the object itself
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            postRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            postRequest.SetRequestHeader("Content-Type", "application/json");
            postRequest.SetRequestHeader("accept", "audio/mpeg");
            //postRequest.SetRequestHeader("Authorization", "Bearer "+ OATHtoken); //too much work
            postRequest.SetRequestHeader("xi-api-key", _apiKey);

            // Create the DownloadHandlerAudioClip object and set it as the download handler.
            DownloadHandlerAudioClip audioClipHandler = new DownloadHandlerAudioClip(url, AudioType.MPEG);
            postRequest.downloadHandler = audioClipHandler;

            // Send the request and wait for it to complete.
            yield return postRequest.SendWebRequest();

            if (postRequest.result != UnityWebRequest.Result.Success)
            {
                string msg = postRequest.error;
                Debug.Log(msg);
                OnRequestFailed.Invoke();
            }
            else
            {
                //We don't get a json file back for this, just the actual final mp3 file.
                OnRequestCompleted.Invoke(((DownloadHandlerAudioClip)postRequest.downloadHandler).audioClip);
            }
        }
    }

    private string RemoveAccents(string input)
    {
        // Define arrays for accented vowels and their unaccented counterparts
        char[] accentedVowels = { 'á', 'é', 'í', 'ó', 'ú', 'ñ', 'Á', 'É', 'Í', 'Ó', 'Ú', 'Ñ' };
        char[] unaccentedVowels = { 'a', 'e', 'i', 'o', 'u', 'n', 'A', 'E', 'I', 'O', 'U', 'N' };

        // Replace accented vowels with unaccented vowels
        for (int i = 0; i < accentedVowels.Length; i++)
        {
            input = input.Replace(accentedVowels[i], unaccentedVowels[i]);
        }

        // Use regular expressions to remove remaining diacritics
        input = Regex.Replace(input, @"\p{Mn}", "");

        return input;
    }

    [Serializable]
    public class TextToSpeechRequest
    {
        public string text;
        public string model_id;
        public VoiceSettings voice_settings;
    }

    [Serializable]
    public class VoiceSettings
    {
        public int stability; // 0
        public int similarity_boost; // 0
        public float style; // 0.5
        public bool use_speaker_boost; // true
    }

    // Define a class to represent the JSON structure
    [System.Serializable]
    public class ApiKeyData
    {
        public string api_key;
    }
}