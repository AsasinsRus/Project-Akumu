using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static Unity.Burst.Intrinsics.X86.Avx;

public class GlitchPass : ScriptableRenderPass
{
    [SerializeField, Range(0, 1)]
    public float _intensity = 0;
    [SerializeField, Range(0, 1)]
    public float _offset = 0;
    [SerializeField, Range(0, 1)]
    public float _blackScreen_Slider = 0;
    [SerializeField, Range(0, 1)]
    public float _uv_Slider = 0;
    [SerializeField, Range(0, 1)]
    public float _edge_Slider = 0;
    [SerializeField, Range(0, 1)]
    public float _blend_Toggle = 0;

    [SerializeField] Texture2D _palleteTexture;
    [SerializeField] Shader _shader;

    Material _material;
    Texture2D _noiseTexture;
    Texture2D _noiseTexture_2;

    RTHandle m_CameraColorTarget;

    public GlitchPass(Texture2D _palleteTexture, Shader _shader, 
        float _intensity, float _offset, float _blackScreen_Slider, float _uv_Slider, float _edge_Slider, float _blend_Toggle)
    {
        this._palleteTexture = _palleteTexture;
        this._shader = _shader;

        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }
    void SetUpResources()
    {
        if (_material != null) return;

        _material = new Material(_shader);

        _noiseTexture = new Texture2D(64, 256, TextureFormat.ARGB32, false)
        {
            wrapMode = TextureWrapMode.Repeat,
            filterMode = FilterMode.Point
        };

        _noiseTexture_2 = new Texture2D(32, 16, TextureFormat.ARGB32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Point
        };

        UpdateNoiseTexture();
    }

    void UpdateNoiseTexture()
    {
        Color color = new Color();

        for (var y = 0; y < _noiseTexture.height; y++)
        {
            for (var x = 0; x < _noiseTexture.width; x++)
            {
                if (Random.value > 0.9f)
                {
                    Color temp = _palleteTexture.GetPixel(x, y / 4);
                    color = new Color(temp.r, temp.g, temp.b, 0);
                }
                _noiseTexture.SetPixel(x, y, color);
            }
        }

        for (var y = 0; y < _noiseTexture_2.height; y++)
        {
            for (var x = 0; x < _noiseTexture_2.width; x++)
            {
                if (Random.value > 0.7f)
                {
                    color = new Color(0, Random.value, Random.value, Random.value);
                }
                _noiseTexture_2.SetPixel(x, y, new Color(0, color.g, color.b, color.a));
            }
        }

        _noiseTexture.Apply();
        _noiseTexture_2.Apply();
    }

    
    /*
    public void SetTarget(RTHandle colorHandle, float intensity)
    {
        m_CameraColorTarget = colorHandle;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        ConfigureTarget(m_CameraColorTarget);
    }*/

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        SetUpResources();

        _material.SetFloat("_Intensity", _intensity);
        _material.SetFloat("_Offset", _offset);
        _material.SetFloat("_BlackScreen_Slider", _blackScreen_Slider);
        _material.SetFloat("_uv_Slider", _uv_Slider);
        _material.SetFloat("_Edge_Slider", _edge_Slider);
        _material.SetFloat("_Blend_toggle", _blend_Toggle);


        _material.SetTexture("_NoiseTex", _noiseTexture);
        _material.SetTexture("_NoiseTex_2", _noiseTexture_2);

        CommandBuffer cmd = CommandBufferPool.Get();

        //Blitter.BlitCameraTexture(cmd, m_CameraColorTarget, m_CameraColorTarget, _material, 0);
    }
}
