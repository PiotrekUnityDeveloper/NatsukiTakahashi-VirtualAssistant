using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceManager : MonoBehaviour
{

    private Dictionary<string, VoiceDefinition> voiceDictionary = new Dictionary<string, VoiceDefinition>(); //voiceovers dict
    public List<VoiceDefinition> voiceDefs = new List<VoiceDefinition>(); //this stores our voiceovers and their keys
    public AudioSource characterAudioSource; //audio source

    private void Awake()
    {
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
        characterAudioSource.clip = GetVoiceDefinition(key).voiceClip;
        characterAudioSource.Play();
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
