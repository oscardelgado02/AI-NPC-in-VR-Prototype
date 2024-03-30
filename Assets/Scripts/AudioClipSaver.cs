using UnityEngine;
using System.IO;
using System;

public sealed class AudioClipSaver
{
    // Save AudioClip to a WAV file
    public static void SaveToWav(AudioClip audioClip, string filePath)
    {
        // Create a new Wav file
        using (FileStream fileStream = CreateEmpty(filePath))
        {
            // Write the AudioClip data to the file
            ConvertAndWrite(fileStream, audioClip);
        }
    }

    // Create an empty WAV file
    private static FileStream CreateEmpty(string filepath)
    {
        // Create the directory if it doesn't exist
        Directory.CreateDirectory(Path.GetDirectoryName(filepath));
        // Create the file
        return new FileStream(filepath, FileMode.Create);
    }

    // Convert AudioClip data to WAV format and write to file
    private static void ConvertAndWrite(FileStream fileStream, AudioClip audioClip)
    {
        // Create a buffer to hold the audio data
        float[] samples = new float[audioClip.samples * audioClip.channels];
        // Get the audio data from the AudioClip
        audioClip.GetData(samples, 0);

        // Convert the float samples to 16-bit PCM format
        short[] intData = new short[samples.Length];
        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * 32767f);
        }

        // Write the WAV file header
        WriteHeader(fileStream, audioClip);

        // Write the audio data to the file
        byte[] byteData = new byte[intData.Length * 2];
        Buffer.BlockCopy(intData, 0, byteData, 0, byteData.Length);
        fileStream.Write(byteData, 0, byteData.Length);
    }

    // Write the WAV file header
    private static void WriteHeader(FileStream fileStream, AudioClip audioClip)
    {
        // Calculate the total file size
        int fileSize = 36 + audioClip.samples * 2;

        // Set the sample rate
        int sampleRate = audioClip.frequency;

        // Set the number of channels
        int numChannels = audioClip.channels;

        // Set the audio format (PCM)
        int format = 1;

        // Set the bits per sample
        int bitDepth = 16;

        // Write the header to the file
        fileStream.Seek(0, SeekOrigin.Begin);
        fileStream.Write(new byte[] { 82, 73, 70, 70 }, 0, 4);
        fileStream.Write(BitConverter.GetBytes(fileSize), 0, 4);
        fileStream.Write(new byte[] { 87, 65, 86, 69 }, 0, 4);
        fileStream.Write(new byte[] { 102, 109, 116, 32 }, 0, 4);
        fileStream.Write(BitConverter.GetBytes(16), 0, 4);
        fileStream.Write(BitConverter.GetBytes((short)format), 0, 2);
        fileStream.Write(BitConverter.GetBytes((short)numChannels), 0, 2);
        fileStream.Write(BitConverter.GetBytes(sampleRate), 0, 4);
        fileStream.Write(BitConverter.GetBytes(sampleRate * numChannels * bitDepth / 8), 0, 4);
        fileStream.Write(BitConverter.GetBytes((short)(numChannels * bitDepth / 8)), 0, 2);
        fileStream.Write(BitConverter.GetBytes((short)bitDepth), 0, 2);
        fileStream.Write(new byte[] { 100, 97, 116, 97 }, 0, 4);
        fileStream.Write(BitConverter.GetBytes(audioClip.samples * 2), 0, 4);
    }
}
