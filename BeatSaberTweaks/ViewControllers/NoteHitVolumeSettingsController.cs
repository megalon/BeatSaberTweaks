﻿using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BeatSaberTweaks
{
	public class NoteHitVolumeSettingsController : ListSettingsController
	{
        protected float[] _volumes;

        protected override void GetInitValues(out int idx, out int numberOfElements)
        {
            float num = 0.1f;
            float num2 = 0.0f;
            numberOfElements = 11;
            _volumes = new float[numberOfElements];
            for (int i = 0; i < _volumes.Length; i++)
            {
                _volumes[i] = num2 + num * i;
            }
            float volume = Settings.NoteHitVolume;
            idx = numberOfElements - 1;
            for (int j = 0; j < _volumes.Length; j++)
            {
                if (volume == _volumes[j])
                {
                    idx = j;
                    return;
                }
            }
        }

        protected override void ApplyValue(int idx)
        {
            Settings.NoteHitVolume = _volumes[idx];
            NoteHitVolume.UpdateVolumes();
        }

        protected override string TextForValue(int idx)
        {
            return string.Format("{0:0.0}", _volumes[idx]);
        }
    }
}