using UnityEngine;

public class PlayOneShotManager : MonoBehaviour {
    [SerializeField] private SoundEffectConfiguration SoundEffectConfiguration;
    [SerializeField] private PlayOneShot OneShotPrefab;

    private void OnEnable() {
        NetworkEventBus.Subscribe<PlayOneShotEvent>(playOneShot);
    }

    private void OnDisable() {
        NetworkEventBus.Unsubscribe<PlayOneShotEvent>(playOneShot);
    }

    private void playOneShot(PlayOneShotEvent soundEvent) {
        if (NetworkManager.IsServer == soundEvent.onServer) {
            var oneShot = Instantiate(OneShotPrefab);
            oneShot.transform.position = soundEvent.position;
            oneShot.Play(SoundEffectConfiguration.GetClip(soundEvent.audioClipID));
        } else {
            Debug.Log("THIS IS NOT A SOUND WE SHOULD PLAY XD XD XD XD XD X DXD ");
        }
    }
}
