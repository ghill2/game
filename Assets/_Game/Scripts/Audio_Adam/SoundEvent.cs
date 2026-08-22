//using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundCategory
{
    Music,
    SpellSFX,          // 3D projectile / spell sounds
    Environment,    
    NpcSFX,            // monster hurt / death / attack
    PlayerSFX,         // player hurt / death / attack
    UISFX              // buttons, hover
}

[CreateAssetMenu(fileName = "New Sound Event", menuName = "Audio/Sound Event")]
public class SoundEvent : ScriptableObject
{
    [Header("Identity")]
    public SoundCategory category = SoundCategory.PlayerSFX;

    [Header("Playback Source")]
    public List<AudioClip> AudioClips = new List<AudioClip>();

    [Header("Volume (randomized between min/max each play)")]
    [Range(0f, 1f)] public float volumeMin = 1f;
    [Range(0f, 1f)] public float volumeMax = 1f;

    [Header("Pitch (randomized between min/max each play)")]
    [Tooltip("Only applies when Resource is a plain AudioClip. Unity ignores AudioSource.pitch when Resource is an Audio Random Container — configure pitch randomization on the container asset itself instead.")]
    [Range(-3f, 3f)] public float pitchMin = 1f;
    [Range(-3f, 3f)] public float pitchMax = 1f;

    [Header("Triggered Probability")]
    [Range(0f,1f)] public float probability = 1f;

    [Header("3D Sound Settings")]
    [Range(0f, 1f)] public float spatialBlend = 1f; // 0 = 2D, 1 = fully 3D
    public float minDistance = 1f;
    public float maxDistance = 25f;
    public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;

    [Header("Looping")]
    public bool loop = false;

    [Header("Mixer Routing")]
    [Tooltip("Leave empty to fall back to the AudioManager's default group for this category.")]
    public AudioMixerGroup mixerGroup;

    public float GetVolume() =>Random.Range(volumeMin, volumeMax);
    public float GetPitch() => Random.Range(pitchMin, pitchMax);
    public AudioClip GetClip() => AudioClips[Random.Range(0, AudioClips.Count)];
}

