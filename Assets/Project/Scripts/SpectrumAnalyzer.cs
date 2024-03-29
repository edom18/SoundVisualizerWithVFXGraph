﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SpectrumAnalyzer : MonoBehaviour
{
    [SerializeField] private int _resolution = 1024;
    [SerializeField] private float _lowFreqThreshold = 14700f;
    [SerializeField] private float _midFreqThreshold = 29400f;
    [SerializeField] private float _highFreqThreshold = 44100;

    [SerializeField] private float _power = 0.1f;
    [SerializeField] private VisualEffect _vfx = null;
    [SerializeField] private UpdateWave _updateWave = null;

    [SerializeField] private float _threshold = 0.5f;

    private AudioSource _audio = null;

    private void Start()
    {
        _audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        var spectrum = _audio.GetSpectrumData(_resolution, 0, FFTWindow.Hamming);

        var deltaFreq = AudioSettings.outputSampleRate / _resolution;

        float low = 0f, mid = 0f, high = 0f;

        for (int i = 1; i < spectrum.Length - 1; ++i)
        {
            var freq = deltaFreq * i;
            if (freq <= _lowFreqThreshold) low += spectrum[i];
            else if (freq <= _midFreqThreshold) mid += spectrum[i];
            else if (freq <= _highFreqThreshold) high += spectrum[i];
        }

        // Debug.Log($"Low:{low}, Mid:{mid}, High:{high}");

        Debug.DrawLine(new Vector3(0, 0, 0), new Vector3(0, low, 0), Color.red);
        Debug.DrawLine(new Vector3(1, 0, 0), new Vector3(1, mid, 0), Color.blue);
        Debug.DrawLine(new Vector3(2, 0, 0), new Vector3(2, high, 0), Color.green);

        float target = low;
        float p = target < _threshold ? 0 : 1f;
        _updateWave.Power = target * p * _power;

        _vfx.SetVector4("Color", new Color(0.1f, high, mid, 1));
    }
}