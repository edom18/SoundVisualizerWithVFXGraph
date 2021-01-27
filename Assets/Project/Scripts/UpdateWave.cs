using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class SwapBuffer
{
    private RenderTexture[] _buffers = new RenderTexture[2];

    public RenderTexture Current => _buffers[0];
    public RenderTexture Other => _buffers[1];

    private int _width = 0;
    private int _height = 0;

    public int Width => _width;
    public int Height => _height;

    public SwapBuffer(int width, int height)
    {
        _width = width;
        _height = height;

        for (int i = 0; i < _buffers.Length; i++)
        {
            _buffers[i] = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            _buffers[i].enableRandomWrite = true;
            _buffers[i].Create();
        }
    }

    public void Swap()
    {
        RenderTexture temp = _buffers[0];
        _buffers[0] = _buffers[1];
        _buffers[1] = temp;
    }

    public void Release()
    {
        foreach (var buf in _buffers)
        {
            buf.Release();
        }
    }
}

public class UpdateWave : MonoBehaviour
{
    [SerializeField] private ComputeShader _shader = null;
    [SerializeField] private Texture2D _texture = null;
    [SerializeField] private float _deltaUV = 3f;
    [SerializeField] private float _S2 = 0.5f;
    [SerializeField] private float _atten = 0.999f;

    [SerializeField] private RawImage _preview = null;
    [SerializeField] private VisualEffect _vfx = null;

    private SwapBuffer _swapBuffer = null;
    private int _kernel = 0;

    private void Start()
    {
        _swapBuffer = new SwapBuffer(_texture.width, _texture.height);

        Graphics.Blit(_texture, _swapBuffer.Other);
    }

    private void Update()
    {
        UpdateBuffer();
    }

    private void UpdateBuffer()
    {
        _kernel = -_shader.FindKernel("Update");
        _shader.SetTexture(_kernel, "_WaveBufferRead", _swapBuffer.Other);
        _shader.SetTexture(_kernel, "_WaveBufferWrite", _swapBuffer.Current);

        _shader.SetFloat("_S2", _S2);
        _shader.SetFloat("_Atten", _atten);
        _shader.SetFloat("_DeltaUV", _deltaUV);

        _shader.Dispatch(_kernel, _texture.width / 8, _texture.height / 8, 1);

        _preview.texture = _swapBuffer.Current;
        _vfx.SetTexture("HeightMap", _swapBuffer.Current);
        
        _swapBuffer.Swap();
    }
}