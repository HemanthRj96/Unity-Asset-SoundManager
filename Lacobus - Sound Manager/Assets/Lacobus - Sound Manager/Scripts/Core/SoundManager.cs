using System.Collections.Generic;
using UnityEngine;
using System;


namespace Lacobus.Sound
{
    public class SoundManager : MonoBehaviour
    {
        // Fields

        [SerializeField] private SoundCluster[] _soundClusters;

        private Dictionary<string, Dictionary<string, SoundData>> _soundLookup = new Dictionary<string, Dictionary<string, SoundData>>();


        // Properties

        public static SoundManager ActiveManager { get; private set; }


        // Public methods

        /// <summary>
        /// Method to play sound on a gameobject
        /// </summary>
        /// <param name="clusterTag">Target cluster tag</param>
        /// <param name="clipName">Name of the clip to be played</param>
        /// <param name="source">Target sound source</param>
        public void PlaySound(string clusterTag, string clipName, GameObject source)
        {
            AudioSource audioSource = null;

            if (!soundFetcher(clusterTag, clipName, out SoundData sound))
                return;

            if (source)
            {
                if (source.TryGetComponent(out audioSource))
                    internalSoundPlayer(audioSource, sound);
                else
                {
                    audioSource = source.AddComponent<AudioSource>();
                    internalSoundPlayer(audioSource, sound);
                }
            }
        }

        /// <summary>
        /// Method to play a sound at a location
        /// </summary>
        /// <param name="location">Location where the sound has to be played from</param>
        /// <param name="clipName">Audio clip to be played</param>
        public void PlaySoundAt(Vector3 location, string clusterTag, string clipName)
        {
            if (!soundFetcher(clusterTag, clipName, out SoundData sound))
                return;
            PlaySoundAt(location, sound.clip);
        }

        /// <summary>
        /// Method to play a sound at a location
        /// </summary>
        /// <param name="target">Target GameObject where the sound has to be played from</param>
        /// <param name="clipName">Audio clip to be played</param>
        public void PlaySoundAt(GameObject target, string clusterTag, string clipName)
        {
            PlaySoundAt(target.transform.position, clusterTag, clipName);
        }

        /// <summary>
        /// Method to play a sound at a location
        /// </summary>
        /// <param name="location">Target location where the sound has to be played</param>
        /// <param name="clip">Audio clip to be played</param>
        public void PlaySoundAt(Vector3 location, AudioClip clip)
        {
            AudioSource.PlayClipAtPoint(clip, location);
        }

        /// <summary>
        /// Method to play a sound at a location
        /// </summary>
        /// <param name="target">Target GameObject where the sound has to be played from</param>
        /// <param name="clip">Audio clip to be played</param>
        public void PlaySoundAt(GameObject target, AudioClip clip)
        {
            PlaySoundAt(target.transform.position, clip);
        }

        /// <summary>
        /// Method to get SoundData
        /// </summary>
        /// <param name="soundName">Target sound name</param>
        public SoundData GetSoundData(string clusterTag, string clipName)
        {
            soundFetcher(clusterTag, clipName, out SoundData sound);
            return sound;
        }


        // Private methods

        private void initializeLookups()
        {
            foreach (var cluster in _soundClusters)
            {
                Dictionary<string, SoundData> tempSounds = new Dictionary<string, SoundData>();
                foreach (var s in cluster.Sounds)
                    tempSounds.Add(s.clipName, s);
                _soundLookup.Add(cluster.ClusterName, tempSounds);
            }
        }

        private bool soundFetcher(string clusterTag, string clipName, out SoundData sound)
        {
            if (_soundLookup.ContainsKey(clusterTag))
            {
                if (_soundLookup[clusterTag].ContainsKey(clipName))
                {
                    sound = _soundLookup[clusterTag][clipName];
                    return true;
                }
                else
                {
                    sound = null;
                    soundClipError(clipName);
                    return false;
                }
            }
            else
            {
                sound = null;
                clusterError(clusterTag);
                return false;
            }
        }

        private void internalSoundPlayer(AudioSource source, SoundData sound)
        {
            if (source == null)
                return;

            source.loop = sound.Loop;
            source.clip = sound.clip;
            source.volume = sound.Volume;
            source.pitch = sound.Pitch;
            source.spatialBlend = sound.SpatialBlend;

            switch (sound.PlayMode)
            {
                case SoundPlayMode.Play:
                    source.Play();
                    break;
                case SoundPlayMode.PlayOneShot:
                    source.PlayOneShot(source.clip);
                    break;
                case SoundPlayMode.PlayDelayed:
                    source.PlayDelayed(sound.Delay);
                    break;
            }
        }

        private void clusterError(string containerTag)
        {
            Debug.LogError($"CONTAINER TAG : {containerTag} NOT FOUND!!");
        }

        private void soundClipError(string clipName)
        {
            Debug.LogError($"SOUND CLIP : {clipName} NOT FOUND!!");
        }


        // Lifecycle methods

        private void Awake()
        {
            // singleton
            if (ActiveManager == null)
            {
                ActiveManager = this;
                DontDestroyOnLoad(this);
            }
            else
                Destroy(gameObject);

            initializeLookups();
        }


        // Nested types

        public enum SoundPlayMode
        {
            Play,
            PlayOneShot,
            PlayDelayed,
        }

        [Serializable]
        public class SoundData
        {
            public string clipName;
            public AudioClip clip = null;
            public SoundPlayMode PlayMode;
            public bool Loop;
            [Range(0, 1)]
            public float Volume;
            [Range(-3, 3)]
            public float Pitch = 1;
            [Range(0, 1)]
            public float SpatialBlend;
            [Range(0, 100)]
            public float Delay;
            public bool ShouldPlayOnAwake = false;
        }
    }
}