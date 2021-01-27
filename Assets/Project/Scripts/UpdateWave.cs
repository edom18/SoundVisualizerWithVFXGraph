using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] private RawImage _preview = null;

    private SwapBuffer _swapBuffer = null;
    
    private void Start()
    {
        _swapBuffer = new SwapBuffer(_texture.width, _texture.height);

        int kernel = -_shader.FindKernel("Update");
        _shader.SetTexture(kernel, "_WaveBufferRead", _texture);
        _shader.SetTexture(kernel, "_WaveBufferWrite", _swapBuffer.Current);
        
        _shader.Dispatch(kernel, _texture.width / 8, _texture.height / 8, 1);

        _preview.texture = _swapBuffer.Current;
    }

    private void Update()
    {
        
    }
}
