using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectrumAnalyzer : MonoBehaviour
{
    [SerializeField] private int _resolution = 1024;
    [SerializeField] private float _lowFreqThreshold = 14700f;
    [SerializeField] private float _midFreqThreshold = 29400f;
    [SerializeField] private float _highFreqThreshold = 44100;
    
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
            if      (freq <= _lowFreqThreshold) low += spectrum[i];
            else if (freq <= _midFreqThreshold) mid += spectrum[i];
            else if (freq <= _highFreqThreshold) high += spectrum[i];
        }

        Debug.Log($"Low:{low}, Mid:{mid}, High:{high}");

        Debug.DrawLine(new Vector3(0, 0, 0), new Vector3(0, low, 0), Color.red);
        Debug.DrawLine(new Vector3(1, 0, 0), new Vector3(1, mid, 0), Color.blue);
        Debug.DrawLine(new Vector3(2, 0, 0), new Vector3(2, high, 0), Color.green);
    }
}