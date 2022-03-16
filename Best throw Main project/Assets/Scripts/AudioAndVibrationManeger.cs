using System;
using System.Collections;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public enum AudioType { Music, Sound }
public class AudioAndVibrationManeger : MonoBehaviour
{

    bool _soundMute = false;
    bool _musicMute = false;
    bool _vibrationMute = false;

    [Header("Sounds of the game")]
    [SerializeField] Sound[] _sounds;

    [Header("Options to start playback and stop music smoothly")]
    [SerializeField] float _timeOfLerpPlayAndStopMusic;
    [SerializeField] float _deltaOfLerpToPlayAndStop;
    Coroutine coroutine1, coroutine2;

    const string VIBRATION_MUTE = "Vibration mute";
    const string SOUND_MUTE = "Sound mute";
    const string Music_MUTE = "Music mute";

    public static AudioAndVibrationManeger instance;
    void Awake()
    {
        if (instance == null)
            instance = this;

        LoadPrefs();
    }

    void LoadPrefs()
    {

        // Sound mute
        if (PlayerPrefs.HasKey(SOUND_MUTE))
        {
            if (PlayerPrefs.GetInt(SOUND_MUTE) == 1)
            {
                _soundMute = true;
                UIManeger.instance.changeSoundBtnStatus(true);
            }
            else
            {
                _soundMute = false;
                UIManeger.instance.changeSoundBtnStatus(false);
            }
        }
        else
        {
            _soundMute = false;
            PlayerPrefs.SetInt(SOUND_MUTE, 0);
            PlayerPrefs.Save();
            UIManeger.instance.changeSoundBtnStatus(false);
        }

        // Music mute
        if (PlayerPrefs.HasKey(Music_MUTE))
        {
            if (PlayerPrefs.GetInt(Music_MUTE) == 1)
            {
                _musicMute = true;
                UIManeger.instance.changeMusicBtnStatus(true);
            }
            else
            {
                _musicMute = false;
                UIManeger.instance.changeMusicBtnStatus(false);
            }
        }
        else
        {
            _musicMute = false;
            PlayerPrefs.SetInt(Music_MUTE, 0);
            PlayerPrefs.Save();
            UIManeger.instance.changeMusicBtnStatus(false);
        }

        // For vibration stutos (mute or not mute)
        if (PlayerPrefs.HasKey(VIBRATION_MUTE))
        {
            if (PlayerPrefs.GetInt(VIBRATION_MUTE) == 1)
            {
                _vibrationMute = true;
                UIManeger.instance.changeVibrationBtnStatus(true);
            }
            else
            {
                _vibrationMute = false;
                UIManeger.instance.changeVibrationBtnStatus(false);
            }
        }
        else
        {
            _vibrationMute = false;
            PlayerPrefs.SetInt(VIBRATION_MUTE, 0);
            PlayerPrefs.Save();
            UIManeger.instance.changeVibrationBtnStatus(false);
        }
    }


    void Start()
    {

        foreach (Sound s in _sounds)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = s.clip;
            source.volume = s.volume;
            source.pitch = s.pitch;
            source.loop = s.loop;

            s.source = source;

            if (s.playOnStart)
                s.source.Play();
        }
    }

    #region Music and Audio

    public void play(string soundName)
    {

        Sound s = Array.Find(_sounds, so => so.name == soundName);

        if (s == null)
        {
            Debug.LogError("Sound with name " + soundName + " doesn't exist!");
            return;
        }

        if (s.audioType == AudioType.Music && !_musicMute)
        {
            if (coroutine2 != null)
            {
                StopCoroutine(coroutine2);
            }
            coroutine1 = StartCoroutine(playWithLerp(s));
        }
        else if (s.audioType == AudioType.Sound && !_soundMute)
        {
            s.source.Play();
        }

    }
    public void stop(string soundName)
    {
        Sound s = Array.Find(_sounds, so => so.name == soundName);

        if (s == null)
        {
            Debug.LogError("Sound with name " + soundName + " doesn't exist!");
            return;
        }

        if (s.audioType == AudioType.Music)
        {
            if (coroutine1 != null)
            {
                StopCoroutine(coroutine1);
            }
            coroutine2 = StartCoroutine(stopWithLerp(s.source));
        }
        else if (s.audioType == AudioType.Sound)
        {
            s.source.Stop();
        }

    }
    public void SetSoundMute(bool isMute)
    {
        _soundMute = isMute;
        int state = (isMute) ? 1 : 0;
        PlayerPrefs.SetInt(SOUND_MUTE, state);
        PlayerPrefs.Save();
    }
    public bool GetSoundMute()
    {
        return _soundMute;
    }
    public void SetMusicMute(bool isMute)
    {
        _musicMute = isMute;
        int state = (isMute) ? 1 : 0;
        PlayerPrefs.SetInt(Music_MUTE, state);
        PlayerPrefs.Save();
    }
    public bool GetMusicMute()
    {
        return _musicMute;
    }
    IEnumerator playWithLerp(Sound s)
    {
        AudioSource aS = s.source;
        var wait = new WaitForSeconds(_timeOfLerpPlayAndStopMusic * Time.deltaTime);
        float targetVolume = s.volume;
        aS.volume = 0;
        aS.Play();
        bool canlerp = true;
        while (canlerp)
        {
            aS.volume = aS.volume + _deltaOfLerpToPlayAndStop;
            if (aS.volume >= targetVolume)
            {
                canlerp = false;
                aS.volume = targetVolume;
            }
            yield return wait;
        }

    }
    IEnumerator stopWithLerp(AudioSource As)
    {
        var wait = new WaitForSeconds(_timeOfLerpPlayAndStopMusic * Time.deltaTime);
        float targetVolume = 0;
        float firstVolume = As.volume;
        bool canlerp = true;
        while (canlerp)
        {
            As.volume = As.volume - _deltaOfLerpToPlayAndStop;
            if (As.volume <= targetVolume)
            {
                canlerp = false;
                As.volume = targetVolume;
                As.Stop();

            }
            yield return wait;
        }
    }

    #endregion

    #region Vibration

    public void PlayCollidedBallWithFailGuardVibration()
    {
        if (!_vibrationMute)
        {
            Vibration.Vibrate(new long[] { 0, 32 }, -1);
        }
    }
    public void PlayWhenBallWentToStationOrTarget()
    {
        if (!_vibrationMute)
        {
            Vibration.Vibrate(new long[] { 0, 25 }, -1);

        }
    }
    public void SetVibrationMute(bool isMute)
    {
        _vibrationMute = isMute;
        int state = (isMute) ? 1 : 0;
        PlayerPrefs.SetInt(VIBRATION_MUTE, state);
        PlayerPrefs.Save();
    }
    public bool GetVibrationMute()
    {
        return _vibrationMute;
    }

    #endregion
}


[Serializable]
public class Sound
{
    public string name;
    public AudioType audioType;
    public AudioClip clip;

    [HideInInspector]
    public AudioSource source;

    [Range(0, 1f)]
    public float volume = 1;

    [Range(-3f, 3f)]
    public float pitch = 1;

    public bool loop = false;
    public bool playOnStart = false;
}
