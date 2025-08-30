using System.Collections;
using System.Collections.Generic;
using DataStorage;
using DataStorage.Generated;
using UnityEngine;

namespace Managers
{
    public class MusicManager : MonoBehaviour
    {
        [SerializeField] private List<MusicAssets> _musicAssets;
        
        private AudioManager _audioManager;

        private void Start()
        {
            _audioManager = ManagersOwner.GetManager<AudioManager>();
            StartPlayingMusic();
        }

        private MusicAssets GetRandomMusicAsset()
        {
            int generatedIdx = Random.Range(0, _musicAssets.Count);
            return _musicAssets[generatedIdx];
        }
        private void StartPlayingMusic()
        {
            _audioManager.PlaySound(GetRandomMusicAsset(), (nextId) =>
            {
                return GetRandomMusicAsset();
            });

        }
    }

}