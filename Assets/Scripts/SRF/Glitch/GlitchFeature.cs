using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static UnityEngine.XR.XRDisplaySubsystem;

public class GlitchFeature : ScriptableRendererFeature
{
    private GlitchPass customPass;
    public float m_Intensity;
    public Texture2D _palleteTexture;
    public Shader _shader;

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

    public override void Create()
    {
        customPass = new GlitchPass(_palleteTexture, _shader, 
            _intensity, _offset, _blackScreen_Slider, _uv_Slider, _edge_Slider, _blend_Toggle);
    }
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {

#if UNITY_EDITOR
        if (renderingData.cameraData.isSceneViewCamera) return;
#endif
        renderer.EnqueuePass(customPass);
    }

    
}
