using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [Header("Mixer and Database")]
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private AudioDatabase audioDatabase;

    [Header("Pooling")]
    [Tooltip("Max AudioSources created per sound (SFX). Prevents infinite AudioSource growth.")]
    [SerializeField] private int maxInstancesPerSound = 6;

    [Tooltip("Parent transform for runtime-created AudioSources.")]
    [SerializeField] private Transform audioRoot;

    [Header("BGM Crossfade")]
    [SerializeField] private AudioSource bgmSourceA;
    [SerializeField] private AudioSource bgmSourceB;

    private AudioSource _bgmActive;
    private AudioSource _bgmInactive;
    private Coroutine _bgmFadeCo;

    // Quick lookup by sound name/key
    private readonly Dictionary<string, Audio> _audioMap = new Dictionary<string, Audio>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (audioRoot == null)
            audioRoot = transform;

        BuildAudioMap();
    }

    private void BuildAudioMap()
    {
        _audioMap.Clear();
        if (audioDatabase == null || audioDatabase.Audio == null) return;

        foreach (var a in audioDatabase.Audio)
        {
            if (a == null || string.IsNullOrEmpty(a.Name)) continue;

            if (!_audioMap.ContainsKey(a.Name))
                _audioMap.Add(a.Name, a);

            // Defensive: ensure list exists
            if (a.Source == null)
                a.Source = new List<AudioSource>();
        }
    }

    /// <summary>
    /// Play a sound by name.
    /// isIgnoreIfPlaying = true: if ANY source of this sound is currently playing -> do nothing.
    /// isIgnoreIfPlaying = false: allow multi-instance (pooling), reuse stopped source or create new up to maxInstancesPerSound.
    /// </summary>
    public void Play(string soundName, bool loop = false, bool isIgnoreIfPlaying = false)
    {
        // Reuse the same pooled logic; ignore the returned source.
        PlayAndReturnSource(soundName, loop, isIgnoreIfPlaying);
    }

    /// <summary>
    /// Play and optionally receive callbacks when finished and/or when each loop completes.
    /// - isIgnoreIfPlaying = true: if any instance is playing -> returns null (no new play)
    /// - loopCountLimit: for looping sounds, stop after N loops then invoke onFinished. Use 0 for infinite.
    /// </summary>
    public AudioSource PlayWithCallbacks(
        string soundName,
        bool loop,
        bool isIgnoreIfPlaying,
        Action onFinished = null,
        Action<int> onLoopCompleted = null,
        int loopCountLimit = 0
    )
    {
        // Use your existing Play(...) logic but return the AudioSource being used.
        // If your current Play(...) is void, create an internal method that returns the chosen AudioSource.
        AudioSource src = PlayAndReturnSource(soundName, loop, isIgnoreIfPlaying);
        if (src == null) return null;

        // Start tracking
        StartCoroutine(TrackPlaybackCoroutine(src, loop, onFinished, onLoopCompleted, loopCountLimit));
        return src;
    }

    private IEnumerator TrackPlaybackCoroutine(
        AudioSource src,
        bool loop,
        Action onFinished,
        Action<int> onLoopCompleted,
        int loopCountLimit
    )
    {
        if (src == null || src.clip == null)
            yield break;

        // Use time to detect loop boundary
        float lastTime = src.time;
        int loopCount = 0;

        // If not looping, wait until playback ends
        if (!loop)
        {
            // Wait until the source stops (or gets destroyed)
            while (src != null && src.isPlaying)
                yield return null;

            onFinished?.Invoke();
            yield break;
        }

        // Looping: detect each loop completion by time wrapping around
        while (src != null && src.isPlaying)
        {
            float t = src.time;

            // Detect wrap-around: time suddenly drops (e.g. 9.98 -> 0.02)
            if (t + 0.05f < lastTime)
            {
                loopCount++;
                onLoopCompleted?.Invoke(loopCount);

                if (loopCountLimit > 0 && loopCount >= loopCountLimit)
                {
                    src.Stop();
                    onFinished?.Invoke();
                    yield break;
                }
            }

            lastTime = t;
            yield return null;
        }

        // If stopped externally
        if (loopCountLimit > 0)
            onFinished?.Invoke();
    }

    /// <summary>
    /// Select/reuse/create an AudioSource, start playback, and return the source used.
    /// Returns null if sound not found, ignored due to isIgnoreIfPlaying, or pool limit reached.
    /// </summary>
    private AudioSource PlayAndReturnSource(string soundName, bool loop, bool isIgnoreIfPlaying)
    {
        if (!_audioMap.TryGetValue(soundName, out var sound) || sound == null)
            return null;

        // Clean null sources (in case objects got destroyed)
        sound.Source.RemoveAll(s => s == null);

        // Check if any source is already playing + find the first stopped source
        bool anyPlaying = false;
        int stoppedIndex = -1;

        for (int i = 0; i < sound.Source.Count; i++)
        {
            var src = sound.Source[i];

            if (src.isPlaying)
            {
                anyPlaying = true;
            }
            else if (stoppedIndex == -1)
            {
                // Keep the first available stopped source for reuse
                stoppedIndex = i;
            }
        }

        // Your intended behavior:
        // If ignore flag is true AND sound is already playing -> do not play again.
        if (isIgnoreIfPlaying && anyPlaying)
            return null;

        AudioSource useSrc;

        // Reuse a stopped source if exists
        if (stoppedIndex != -1)
        {
            useSrc = sound.Source[stoppedIndex];
        }
        else
        {
            // No stopped source available -> create new if under limit
            if (sound.Source.Count >= maxInstancesPerSound)
            {
                // Optional policy: return the first playing source, restart it, or just return null.
                // Current policy: do nothing to avoid overload.
                return null;
            }

            useSrc = CreateSourceForSound(sound);
            sound.Source.Add(useSrc);
        }

        // Apply runtime overrides
        useSrc.loop = loop;

        // Reset time then play (helps avoid edge glitches on some devices)
        useSrc.time = 0f;
        useSrc.Play();

        return useSrc;
    }

    private void EnsureBgmSources()
    {
        if (bgmSourceA == null)
        {
            var go = new GameObject("BGM_Source_A");
            go.transform.SetParent(transform, false);
            bgmSourceA = go.AddComponent<AudioSource>();
            bgmSourceA.playOnAwake = false;
            bgmSourceA.loop = true;
        }

        if (bgmSourceB == null)
        {
            var go = new GameObject("BGM_Source_B");
            go.transform.SetParent(transform, false);
            bgmSourceB = go.AddComponent<AudioSource>();
            bgmSourceB.playOnAwake = false;
            bgmSourceB.loop = true;
        }

        if (_bgmActive == null)
        {
            _bgmActive = bgmSourceA;
            _bgmInactive = bgmSourceB;
        }
    }

    /// <summary>
    /// Crossfade current BGM into a new BGM.
    /// </summary>
    public void CrossfadeBgm(string newBgmName, float fadeOutTime = 0.5f, float fadeInTime = 0.5f, float targetVolume = 1f)
    {
        EnsureBgmSources();

        if (!_audioMap.TryGetValue(newBgmName, out var sound) || sound == null || sound.Clip == null)
            return;

        // If already playing the same clip, do nothing
        if (_bgmActive.clip == sound.Clip && _bgmActive.isPlaying)
            return;

        if (_bgmFadeCo != null) StopCoroutine(_bgmFadeCo);
        _bgmFadeCo = StartCoroutine(CrossfadeCoroutine(sound.Clip, fadeOutTime, fadeInTime, targetVolume));
    }

    private IEnumerator CrossfadeCoroutine(AudioClip newClip, float fadeOutTime, float fadeInTime, float targetVolume)
    {
        // Prepare inactive source with new clip
        _bgmInactive.clip = newClip;
        _bgmInactive.volume = 0f;
        _bgmInactive.loop = true;
        _bgmInactive.Play();

        // Fade out active, fade in inactive (overlap)
        float t = 0f;
        float activeStartVol = _bgmActive.isPlaying ? _bgmActive.volume : 0f;

        float duration = Mathf.Max(0.01f, Mathf.Max(fadeOutTime, fadeInTime));

        while (t < duration)
        {
            t += Time.unscaledDeltaTime; // UI-safe, not affected by timeScale
            float k = Mathf.Clamp01(t / duration);

            float outK = fadeOutTime <= 0f ? 1f : Mathf.Clamp01(t / fadeOutTime);
            float inK = fadeInTime <= 0f ? 1f : Mathf.Clamp01(t / fadeInTime);

            _bgmActive.volume = Mathf.Lerp(activeStartVol, 0f, outK);
            _bgmInactive.volume = Mathf.Lerp(0f, targetVolume, inK);

            yield return null;
        }

        // Stop old
        _bgmActive.Stop();
        _bgmActive.volume = 0f;

        // Swap active/inactive
        var tmp = _bgmActive;
        _bgmActive = _bgmInactive;
        _bgmInactive = tmp;

        _bgmFadeCo = null;
    }
    public void CrossfadeBgmWhenRemaining(string currentBgmName, string nextBgmName, float remainingThreshold = 1.0f)
    {
        StartCoroutine(CrossfadeWhenRemainingCo(currentBgmName, nextBgmName, remainingThreshold));
    }

    private IEnumerator CrossfadeWhenRemainingCo(string currentName, string nextName, float remainingThreshold)
    {
        while (true)
        {
            float remain = GetAudioRemainingTime(currentName);
            if (remain > 0f && remain <= remainingThreshold)
            {
                CrossfadeBgm(nextName, fadeOutTime: remainingThreshold, fadeInTime: remainingThreshold, targetVolume: 1f);
                yield break;
            }
            yield return null;
        }
    }
    /// <summary>
    /// Stop all instances of a sound by name.
    /// </summary>
    public void Stop(string soundName)
    {
        if (!_audioMap.TryGetValue(soundName, out var sound) || sound == null)
            return;

        if (sound.Source == null) return;

        for (int i = 0; i < sound.Source.Count; i++)
        {
            var src = sound.Source[i];
            if (src != null && src.isPlaying)
                src.Stop();
        }
    }

    /// <summary>
    /// Returns true if any instance of this sound is currently playing.
    /// </summary>
    public bool IsPlaying(string soundName)
    {
        if (!_audioMap.TryGetValue(soundName, out var sound) || sound == null)
            return false;

        if (sound.Source == null) return false;

        for (int i = 0; i < sound.Source.Count; i++)
        {
            var src = sound.Source[i];
            if (src != null && src.isPlaying)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Stops all sounds managed by this AudioManager.
    /// </summary>
    public void StopAll()
    {
        foreach (var kv in _audioMap)
        {
            var sound = kv.Value;
            if (sound?.Source == null) continue;

            for (int i = 0; i < sound.Source.Count; i++)
            {
                var src = sound.Source[i];
                if (src != null && src.isPlaying)
                    src.Stop();
            }
        }
    }

    private AudioSource CreateSourceForSound(Audio sound)
    {
        // Create a dedicated GameObject for the AudioSource (clearer and safer)
        var go = new GameObject($"Audio_{sound.Name}");
        go.transform.SetParent(audioRoot, false);

        var src = go.AddComponent<AudioSource>();

        // Setup default properties from database
        src.clip = sound.Clip;
        src.volume = sound.Volume;
        src.pitch = sound.Pitch;
        src.loop = sound.IsLoop; // default loop from data (can be overridden at Play)
        src.playOnAwake = false;

        // Optional (if your Audio class has these fields)
        // src.outputAudioMixerGroup = sound.mixerGroup;
        // src.spatialBlend = sound.spatialBlend;

        return src;
    }

    /// <summary>
    /// Get the length (in seconds) of an audio by name.
    /// Returns 0 if audio or clip is not found.
    /// </summary>
    public float GetAudioLength(string soundName)
    {
        if (!_audioMap.TryGetValue(soundName, out var sound) || sound == null)
            return 0f;

        if (sound.Clip == null)
            return 0f;

        return sound.Clip.length;
    }
    public float GetAudioLength(string soundName, float pitch)
    {
        float len = GetAudioLength(soundName);
        return pitch <= 0 ? len : len / pitch;
    }
    /// <summary>
    /// Get how long (in seconds) the audio has been playing.
    /// Returns 0 if the sound is not playing.
    /// </summary>
    public float GetAudioElapsedTime(string soundName)
    {
        if (!_audioMap.TryGetValue(soundName, out var sound) || sound == null)
            return 0f;

        if (sound.Source == null)
            return 0f;

        for (int i = 0; i < sound.Source.Count; i++)
        {
            var src = sound.Source[i];
            if (src != null && src.isPlaying)
            {
                return src.time;
            }
        }

        return 0f;
    }
    /// <summary>
    /// Get remaining play time (in seconds) of a playing audio.
    /// Returns 0 if the sound is not playing.
    /// </summary>
    public float GetAudioRemainingTime(string soundName)
    {
        if (!_audioMap.TryGetValue(soundName, out var sound) || sound == null)
            return 0f;

        if (sound.Source == null || sound.Clip == null)
            return 0f;

        for (int i = 0; i < sound.Source.Count; i++)
        {
            var src = sound.Source[i];
            if (src != null && src.isPlaying)
            {
                float remaining = sound.Clip.length - src.time;
                if (remaining < 0f) remaining = 0f;

                // Adjust by pitch (important if pitch != 1)
                float pitch = Mathf.Abs(src.pitch);
                return pitch > 0f ? remaining / pitch : remaining;
            }
        }

        return 0f;
    }
    void OnApplicationPause(bool pause)
    {
        if (pause)
            AudioListener.pause = true;
        else
            AudioListener.pause = false;
    }
    #region change volume
    public void ChangeSoundVolume(float value)
    {
        //Debug.Log(Math.Clamp(Mathf.Log10(value) * 20, -80, 0));
        _ = masterMixer.SetFloat("SFXVolume", Math.Clamp(Mathf.Log10(value) * 20, -80,0));
    }

    public void ChangeMusicVolume(float value)
    {
        //Debug.Log(Math.Clamp(Mathf.Log10(value) * 20, -80, 0));
        _ = masterMixer.SetFloat("BGMVolume", Math.Clamp(Mathf.Log10(value) * 20, -80, 0));
    }
    #endregion


#if UNITY_EDITOR
    [ContextMenu("Rebuild Audio Map")]
    private void Editor_Rebuild()
    {
        BuildAudioMap();
    }
#endif
}
