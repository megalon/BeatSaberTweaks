﻿using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BeatSaberTweaks
{
	public class MissVolumeSettingsController : ListSettingsController
	{
        protected override void GetInitValues(out int idx, out int numberOfElements)
        {
            float num = 0.1f;
            float num2 = 0.0f;
            numberOfElements = 11;
            this._volumes = new float[numberOfElements];
            for (int i = 0; i < this._volumes.Length; i++)
            {
                this._volumes[i] = num2 + num * (float)i;
            }
            float volume = Settings.NoteMissVolume;
            idx = numberOfElements - 1;
            for (int j = 0; j < this._volumes.Length; j++)
            {
                if (volume == this._volumes[j])
                {
                    idx = j;
                    return;
                }
            }
        }

        protected override void ApplyValue(int idx)
        {
            
        }

        protected override string TextForValue(int idx)
        {
            Settings.NoteMissVolume = this._volumes[idx];
            return string.Format("{0:0.0}", this._volumes[idx]);
        }

        protected float[] _volumes;
    }
}