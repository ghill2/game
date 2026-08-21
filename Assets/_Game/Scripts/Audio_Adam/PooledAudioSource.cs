using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace GameAudio
{
    /// <summary>
    /// A poolable AudioSource wrapper. Handles three playback shapes:
    ///  - one-shot at a fixed world position (impact, hurt, attack sfx, UI sfx)
    ///  - one-shot that follows a moving transform (spell projectile in flight)
    ///  - a loop identified by a string id (environment ambience loops)
    /// Calls back into the pool when a non-looping sound finishes.
    ///
    /// Completion is detected by polling AudioSource.isPlaying rather than scheduling off
    /// clip length, because AudioResource (which covers both AudioClip and Audio Random
    /// Container) doesn't expose a reliable length for containers — their effective duration
    /// depends on which sub-clip and layers get chosen at play time.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class PooledAudioSource : MonoBehaviour
    {
        public AudioSource Source { get; private set; }
        public bool IsFollowing { get; private set; }
        public string LoopId { get; private set; }

        private Transform _followTarget;
        private Action<PooledAudioSource> _onFinished;
        private Coroutine _releaseRoutine;

        private void Awake()
        {
            Source = GetComponent<AudioSource>();
            Source.playOnAwake = false;
        }

        public void Play(SoundEvent evt, Vector3 position, Action<PooledAudioSource> onFinished)
        {
            transform.position = position;
            IsFollowing = false;
            _followTarget = null;
            LoopId = null;
            Configure(evt);
            _onFinished = onFinished;
            Source.Play();
            ScheduleAutoRelease(evt);
        }

        public void PlayFollowing(SoundEvent evt, Transform target, Action<PooledAudioSource> onFinished)
        {
            _followTarget = target;
            IsFollowing = true;
            LoopId = null;
            if (target != null) transform.position = target.position;
            Configure(evt);
            _onFinished = onFinished;
            Source.Play();
            ScheduleAutoRelease(evt);
        }

        public void PlayLoop(SoundEvent evt, Vector3 position, string loopId)
        {
            transform.position = position;
            IsFollowing = false;
            _followTarget = null;
            LoopId = loopId;
            Configure(evt);
            Source.loop = true;
            Source.Play();
        }

        public void PlayLoopFollowing(SoundEvent evt, Transform target, string loopId)
        {
            _followTarget = target;
            IsFollowing = true;
            if (target != null) transform.position = target.position;
            LoopId = loopId;
            Configure(evt);
            Source.loop = true;
            Source.Play();
        }

        private void ScheduleAutoRelease(SoundEvent evt)
        {
            if (_releaseRoutine != null) StopCoroutine(_releaseRoutine);
            if (evt.loop) return; // loops are only stopped explicitly
            _releaseRoutine = StartCoroutine(WaitForFinishThenRelease());
        }

        private IEnumerator WaitForFinishThenRelease()
        {
            yield return null; // let Play() actually start before we start checking
            yield return new WaitWhile(() => Source.isPlaying);
            _releaseRoutine = null;
            FinishAndRelease();
        }

        private void Configure(SoundEvent evt)
        {
            Source.resource = evt.resource;
            Source.volume = evt.GetVolume();
            // AudioSource.pitch is ignored (and logs a warning if out of [0.0001..3]) when the
            // resource is an Audio Random Container, which manages its own pitch randomization.
            Source.pitch = evt.IsRandomContainer ? 1f : evt.GetPitch();
            Source.loop = evt.loop;
            Source.spatialBlend = evt.spatialBlend;
            Source.minDistance = evt.minDistance;
            Source.maxDistance = evt.maxDistance;
            Source.rolloffMode = evt.rolloffMode;
            Source.outputAudioMixerGroup = evt.mixerGroup;
        }

        private void Update()
        {
            if (IsFollowing && _followTarget != null)
                transform.position = _followTarget.position;
        }

        /// <summary>Stops playback immediately and returns this source to the pool right away.</summary>
        public void StopImmediate()
        {
            if (_releaseRoutine != null)
            {
                StopCoroutine(_releaseRoutine);
                _releaseRoutine = null;
            }
            Source.Stop();
            FinishAndRelease();
        }

        private void FinishAndRelease()
        {
            IsFollowing = false;
            _followTarget = null;
            LoopId = null;
            var cb = _onFinished;
            _onFinished = null;
            cb?.Invoke(this);
        }
    }
}
