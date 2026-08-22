using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace GameAudio
{
    public enum MusicState
    {
        Cautious,
        Combat
    }

    /// <summary>One stage/level's music pair. Add one entry per stage, in stage order.</summary>
    [System.Serializable]
    public class StageMusicSet
    {
        public string stageName;
        public AudioClip cautiousTrack;
        public AudioClip combatTrack;
    }

    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance { get; private set; }

        [Header("Mixer")]
        public AudioMixerGroup musicMixerGroup;

        [Header("Stages (index = stage number)")]
        public List<StageMusicSet> stages = new List<StageMusicSet>();

        [Header("Crossfade")]
        [SerializeField] private float defaultFadeDuration = 2f;

        private AudioSource _sourceA;
        private AudioSource _sourceB;
        private AudioSource _activeSource;
        private AudioSource _inactiveSource;

        private int _currentStage = -1;
        private MusicState _currentState = MusicState.Cautious;
        private Coroutine _fadeRoutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _sourceA = gameObject.AddComponent<AudioSource>();
            _sourceB = gameObject.AddComponent<AudioSource>();
            foreach (var s in new[] { _sourceA, _sourceB })
            {
                s.loop = true;
                s.playOnAwake = false;
                s.spatialBlend = 0f;
                s.outputAudioMixerGroup = musicMixerGroup;
                s.volume = 0f;
            }
            _activeSource = _sourceA;
            _inactiveSource = _sourceB;
        }

        /// <summary>Switch to a stage's music set. Optionally set state at the same time.</summary>
        public void SetStage(int stageIndex, MusicState? state = null, float fadeDuration = -1f)
        {
            if (stageIndex < 0 || stageIndex >= stages.Count) return;
            _currentStage = stageIndex;
            if (state.HasValue) _currentState = state.Value;
            PlayCurrentTrack(fadeDuration);
        }

        /// <summary>Switch between Cautious and Combat within the current stage.</summary>
        public void SetState(MusicState state, float fadeDuration = -1f)
        {
            if (_currentState == state && _activeSource.isPlaying) return;
            _currentState = state;
            PlayCurrentTrack(fadeDuration);
        }

        public MusicState CurrentState => _currentState;
        public int CurrentStage => _currentStage;

        private void PlayCurrentTrack(float fadeDuration)
        {
            if (_currentStage < 0 || _currentStage >= stages.Count) return;
            var set = stages[_currentStage];
            AudioClip clip = _currentState == MusicState.Combat ? set.combatTrack : set.cautiousTrack;
            if (clip == null) return;
            if (_activeSource.clip == clip && _activeSource.isPlaying) return;

            float duration = fadeDuration >= 0f ? fadeDuration : defaultFadeDuration;

            if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
            _fadeRoutine = StartCoroutine(CrossfadeTo(clip, duration));
        }

        private IEnumerator CrossfadeTo(AudioClip clip, float duration)
        {
            _inactiveSource.clip = clip;
            _inactiveSource.volume = 0f;
            _inactiveSource.Play();

            float t = 0f;
            float startActiveVol = _activeSource.volume;
            while (t < duration)
            {
                t += Time.deltaTime;
                float frac = duration <= 0f ? 1f : t / duration;
                _inactiveSource.volume = Mathf.Lerp(0f, 1f, frac);
                _activeSource.volume = Mathf.Lerp(startActiveVol, 0f, frac);
                yield return null;
            }
            _inactiveSource.volume = 1f;
            _activeSource.volume = 0f;
            _activeSource.Stop();

            var tmp = _activeSource;
            _activeSource = _inactiveSource;
            _inactiveSource = tmp;
        }

        public void StopMusic(float fadeDuration = -1f)
        {
            if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
            _fadeRoutine = StartCoroutine(FadeOutAndStop(fadeDuration >= 0f ? fadeDuration : defaultFadeDuration));
        }

        private IEnumerator FadeOutAndStop(float duration)
        {
            float t = 0f;
            float startVol = _activeSource.volume;
            while (t < duration)
            {
                t += Time.deltaTime;
                _activeSource.volume = Mathf.Lerp(startVol, 0f, t / duration);
                yield return null;
            }
            _activeSource.Stop();
            _activeSource.volume = 0f;
        }
    }
}
