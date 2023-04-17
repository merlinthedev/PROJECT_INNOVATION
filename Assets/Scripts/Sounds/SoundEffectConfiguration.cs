using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundEffectConfiguration", menuName = "SoundEffectConfiguration", order = 0)]
public class SoundEffectConfiguration : ScriptableObject {
    [Serializable]
    public class SoundEffect {
        public string Name;
        public AudioClip Clip;
    }

    public List<SoundEffect> SoundEffects;

    public AudioClip GetClip(int ID) {
        return SoundEffects[ID].Clip;
    }

    public AudioClip GetClip(string name) {
        foreach (var soundEffect in SoundEffects) {
            if (soundEffect.Name == name) {
                return soundEffect.Clip;
            }
        }

        return null;
    }

    public int GetClipIndex(string name) {
        for (int i = 0; i < SoundEffects.Count; i++) {
            if (SoundEffects[i].Name == name) {
                return i;
            }
        }

        return -1;
    }
}
