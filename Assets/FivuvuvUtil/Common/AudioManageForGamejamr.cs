using fivuvuvUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FivuvuvUtil
{
    public class AudioManagerForGamejam : MonoSingleton<AudioManagerForGamejam>
    {
        [Header("SFXSource")]
        public AudioSource SFXaudioSource;
        [Header("BGMSource")]
        public AudioSource BGMaudioSource;
        [Header("��Ƶ�ļ�")]
        public AudioClip[] SFXClips;
        public AudioClip[] BGMClips;
        public Dictionary<string, AudioClip> audioClipDict = new Dictionary<string, AudioClip>();
        public override void Awake()
        {
            base.Awake();
            foreach (AudioClip audioClip in SFXClips)//����Ƶ�ļ������ֵ�
            {
                audioClipDict.Add(audioClip.name, audioClip);
            }
            foreach (AudioClip audioClip in BGMClips)//����Ƶ�ļ������ֵ�
            {
                audioClipDict.Add(audioClip.name, audioClip);
            }
        }
        private void Start()
        {
            DontDestroyOnLoad(this);
        }
        public void PlaySFX(string audioName)
        {
            SFXaudioSource.PlayOneShot(audioClipDict[audioName]);
        }
        //����BGM
        public void PlayBGM(string audioName)
        {
            BGMaudioSource.clip = audioClipDict[audioName];
            BGMaudioSource.Play();
        }
        
    }
}

