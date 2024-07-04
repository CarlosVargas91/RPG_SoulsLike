using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private float sfxMinimunDistance;
    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;
    public bool playBGM;
    private int bgmIndex;

    private bool canPlaySFX;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;

        Invoke("allowSFX", 1f);
    }

    private void Update()
    {
        if (!playBGM)
            stopAllBGM();
        else
            if (!bgm[bgmIndex].isPlaying)
                playBGMFunction(bgmIndex);
    }

    public void playSFX(int _sfxIndex, Transform _source)
    {
        //if (sfx[_sfxIndex].isPlaying)
        //  return;

        if (canPlaySFX == false)
            return;

        if (_source != null && Vector2.Distance(PlayerManager.instance.player.transform.position, _source.position) > sfxMinimunDistance)
            return;

        if (_sfxIndex < sfx.Length)
        {
            sfx[_sfxIndex].pitch = Random.Range(.85f, 1.1f);
            sfx[_sfxIndex].Play();
        }
    }

    public void stopSFX(int _index) => sfx[_index].Stop();

    public void stopSFXWithTime(int _index) => StartCoroutine(decreaseVolume(sfx[_index]));
    private IEnumerator decreaseVolume(AudioSource _audio)
    {
        float defaultVolume = _audio.volume;

        while (_audio.volume > .1f)
        {
            _audio.volume -= _audio.volume * .2f;
            yield return new WaitForSeconds(.25f);

            if (_audio.volume <= .1f)
            {
                _audio.Stop();
                _audio.volume = defaultVolume;
                break;
            }
        }
    }

    public void playRandomBGM()
    {
        bgmIndex = Random.Range(0, bgm.Length);
        playBGMFunction(bgmIndex);

    }

    public void playBGMFunction(int _bgmIndex)
    {
        bgmIndex = _bgmIndex;
        stopAllBGM();
        bgm[bgmIndex].Play();
    }

    public void stopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }

    private void allowSFX() => canPlaySFX = true;
}
