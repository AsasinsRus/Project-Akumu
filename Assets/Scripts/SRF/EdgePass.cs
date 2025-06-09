using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EdgePass : SceneRenderPipeline
{
    /*float GetDepthStrength()
    {
        float difference = 0;
        foreach (float bias in depthBiases)
            difference += clamp(bias, 0, 1);
        return smoothstep(0.01f, 0.02f, difference);
    }*/
}
