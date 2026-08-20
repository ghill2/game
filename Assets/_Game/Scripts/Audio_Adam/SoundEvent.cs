using UnityEngine;
using UnityEngine.Audio;

namespace GameAudio
{
    /// <summary>
    /// Broad category a sound belongs to. Used to route to the correct AudioMixerGroup
    /// and to let designers reason about "what kind of sound is this" at a glance.
    /// </summary>
    public enum SoundCategory
    {
        Music,
        SpellSFX,          // 3D projectile / spell sounds
        EnvironmentLoop,   // ambient loops (wind, river, cave hum...)
        EnvironmentRFX,    // one-shot ambience scattered around the player (birds, creaks...)
        NpcSFX,            // monster hurt / death / attack
        PlayerSFX,         // player hurt / death / attack
        UISFX              // buttons, hover
    }

    /// <summary>
    /// Designer-facing sound definition. One asset = one "sound event" (e.g. "Fireball_Cast",
    /// "Goblin_Hurt", "Forest_Ambience_Loop").
    ///
    /// Playback source is a single AudioResource field: drag in a plain AudioClip for a
    /// one-off sound, or drag in an Audio Random Container asset (Assets > Create > Audio >
    /// Random Container) for randomized variations — weighting, avoid-immediate-repeat,
    /// per-clip volume/pitch randomization, sequencing, and multi-layer trigger conditions
    /// are all configured on the Random Container asset itself via Unity's built-in editor,
    /// so this script doesn't need to reimplement any of that.
    ///
    /// Requires Unity 6000.0+ (AudioResource / AudioRandomContainer were introduced there).
    /// </summary>
    [CreateAssetMenu(fileName = "New Sound Event", menuName = "Audio/Sound Event")]
    public class SoundEvent : ScriptableObject
    {
        [Header("Identity")]
        public SoundCategory category = SoundCategory.PlayerSFX;

        [Header("Playback Source")]
        [Tooltip("Drag a plain AudioClip for a single sound, or an Audio Random Container asset for randomized variations. Both work here since AudioClip and AudioRandomContainer share the AudioResource base type.")]
        public AudioResource resource;

        [Header("Volume (randomized between min/max each play)")]
        [Range(0f, 1f)] public float volumeMin = 1f;
        [Range(0f, 1f)] public float volumeMax = 1f;

        [Header("Pitch (randomized between min/max each play)")]
        [Tooltip("Only applies when Resource is a plain AudioClip. Unity ignores AudioSource.pitch when Resource is an Audio Random Container — configure pitch randomization on the container asset itself instead.")]
        [Range(-3f, 3f)] public float pitchMin = 1f;
        [Range(-3f, 3f)] public float pitchMax = 1f;

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

        // Unity's AudioRandomContainer class is marked `internal`, so it can't be named directly
        // in our own code (that's what caused the "inaccessible due to its protection level"
        // compile error). We can still assign one via the Inspector into an AudioResource field,
        // and detect it at runtime — just indirectly, by checking it's an AudioResource that
        // *isn't* a plain AudioClip, since AudioClip is the only other public AudioResource type.
        public bool IsRandomContainer => resource != null && !(resource is AudioClip);

        public float GetVolume() => Random.Range(volumeMin, volumeMax);
        public float GetPitch() => Random.Range(pitchMin, pitchMax);
    }
}
