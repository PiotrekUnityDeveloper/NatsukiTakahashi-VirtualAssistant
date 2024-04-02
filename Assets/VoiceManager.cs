using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security;
using UnityEngine;
using UnityEngine.Networking;

public class VoiceManager : MonoBehaviour
{

    private Dictionary<string, VoiceDefinition> voiceDictionary = new Dictionary<string, VoiceDefinition>(); //voiceovers dict
    public List<VoiceDefinition> voiceDefs = new List<VoiceDefinition>(); //this stores our voiceovers and their keys
    public AudioSource characterAudioSource; //audio source

    public bool useFromFile = true;
    public string fileSources = "";

    private void Awake()
    {
        fileSources = Path.Combine(Path.GetDirectoryName(Application.dataPath), "res", "voice");
        InitializeDefinitions();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void InitializeDefinitions()
    {
        foreach(VoiceDefinition vdef in voiceDefs)
        {
            AddVoiceDefinition(vdef);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayClip(string key) // Play a clip based on voiceClip
    {
        if(characterAudioSource.isPlaying) characterAudioSource.Stop();

        if (useFromFile)
        {
            fileSources = Environment.CurrentDirectory + "\\res\\voice\\natsuki\\" + key;
            characterAudioSource.clip = GetClipFromFile(fileSources);
        }
        else
        {
            characterAudioSource.clip = GetVoiceDefinition(key).voiceClip;
        }

        characterAudioSource.Play();
    }

    public static AudioClip GetClipFromFile(string filePath)
    {
        // Check if the file exists
        if (File.Exists(filePath))
        {
            // Create UnityWebRequest
            UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.WAV);

            // Send the request
            request.SendWebRequest();

            // Wait until request is done
            while (!request.isDone) { }

            // Check for errors
            if (request.result == UnityWebRequest.Result.Success)
            {
                // Get the audio clip
                AudioClip clip = DownloadHandlerAudioClip.GetContent(request);

                return clip;
            }
            else
            {
                Debug.LogError("Error loading audio file: " + request.error);
            }
        }
        else
        {
            Debug.LogError("File does not exist at path: " + filePath);
        }

        return null;
    }

    public void StopClip()
    {

    }

    // Method to add a VoiceDefinition to the dictionary
    public void AddVoiceDefinition(VoiceDefinition voiceDef)
    {
        voiceDictionary.Add(voiceDef.key, voiceDef);
    }

    // Method to get a VoiceDefinition by its key
    public VoiceDefinition GetVoiceDefinition(string key)
    {
        if (voiceDictionary.ContainsKey(key))
        {
            return voiceDictionary[key];
        }
        else
        {
            // Handle case where key is not found
            return null; // Or throw an exception, or handle it according to your needs
        }
    }

    public VoiceDefinition GetVoiceDefinitionFromList(string key)
    {
        foreach(VoiceDefinition vdef in voiceDefs)
        {
            if(vdef.key == key)
            {
                return vdef;
            }
        }

        return null;
    }

}

[System.Serializable]
public class VoiceDefinition
{
    public string key; // used to authenticate the voice-over we want to play
    public AudioClip voiceClip; // the voice-over file
}
