using UnityEngine;


namespace Lacobus.Sound
{
    [CreateAssetMenu(fileName = "Sound Cluster", menuName = "Lacobus/Create new sound cluster")]
    public class SoundCluster : ScriptableObject
    {
        [SerializeField]
        public string ClusterName;
        [SerializeField]
        public SoundManager.SoundData[] Sounds;
    }
}
