using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioDatabase", menuName = "Scriptable Objects/AudioDatabase")]
public class AudioDatabase : ScriptableObject
{
    public Audio[] Audio;
}
[Serializable]
public class Audio
{
    public string Name;
    public AudioType AudioType;
    public AudioClip Clip;
    public AudioMixerGroup MixerGroup;
    public bool IsLoop = false;

    [Range(0f, 1f)]
    public float Volume = 1;
    [Range(0.1f, 3f)]
    public float Pitch = 1;

    public List<AudioSource> Source = new List<AudioSource>();
}
public enum AudioType
{
    BGM = 0,
    SFX = 1,
    Voice = 2,
}