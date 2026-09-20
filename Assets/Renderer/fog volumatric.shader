 Shader "Custom/fogvolumatric"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _FarDistance ("Far Distance", float) = 100
        _TransitionDistance("Transition Distance", float) = 50
        _StepSize ("Step size", Range(0.1, 20)) = 1
        _NoiseOffset("Noise Offset", float) = 0
        _TransitionDensityMultiplier ("Transition Density multiplier", Range(0, 20)) = 1
        _FarDensityMultiplier ("Far Density multiplier", Range(0, 20)) = 1
        [HDR] _LightContribution("Light Contribution", Color) = (1,1,1,1)
        _LightScattering("LightScattering", Range (0, 1)) = 0.2
        _WaterHeight("Water Height", float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        Pass

        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            
            float4 _Color;
            float _FarDistance;
            float _TransitionDistance;
            float _FarDensityMultiplier;
            float _TransitionDensityMultiplier;
            float _StepSize;
            float _NoiseOffset;
            float4 _LightContribution;
            float _LightScattering;
            float _WaterHeight;
            float henyey_greenstein(float angle, float scattering)
            {
                return (1.0 - scattering * scattering) / (4.0 * PI * pow(max (1.0 + scattering * scattering - (2.0 * scattering) * angle, 0.0001) , 1.5f));
            }
            float get_transition_density()
            {
                return _TransitionDensityMultiplier / 1000;
            }
            float get_far_density()
            {
                return _FarDensityMultiplier / 1000;
            }
          half4 frag(Varyings IN) : SV_Target

           {
               float4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, IN.texcoord);
               float depth = SampleSceneDepth(IN.texcoord);
               float3 worldPos = ComputeWorldSpacePosition(IN.texcoord, depth, UNITY_MATRIX_I_VP);
                
               float3 entryPoint = _WorldSpaceCameraPos;
               float3 viewDir = worldPos - _WorldSpaceCameraPos;
               float viewLength = length(viewDir);
               float transitionDistLimit = min(viewLength, _TransitionDistance);
               float farDistLimit = min(viewLength, _FarDistance);
               float3 rayDir = normalize(viewDir);
               float2 pixelCoords = IN.texcoord * _BlitTexture_TexelSize.zw;
               float distTravelled = InterleavedGradientNoise(pixelCoords, (int)(_Time.y/ max(HALF_EPS, unity_DeltaTime.x))) * _NoiseOffset;
               // 1. Unified memory for the entire ray
               float transmittance = 1.0;
               float3 lightAccumulation = float3(0, 0, 0);
               // --- LOOP 1 ---
               while(distTravelled < transitionDistLimit)
               {
                   float3 rayPos = entryPoint + rayDir * distTravelled;
                   float transitionDensity = get_transition_density();
                   if (rayPos.y >= _WaterHeight) 
                   {
                       transitionDensity = 0;
                   }
                   if (transitionDensity > 0)
                   {
                       Light mainLight = GetMainLight(TransformWorldToShadowCoord(rayPos));
                       float angle = dot(rayDir, mainLight.direction);
                       // 4. Added * transmittance for self-occlusion
                       lightAccumulation += _LightContribution.rgb * henyey_greenstein(angle, _LightScattering) 
                                            * transitionDensity * mainLight.shadowAttenuation * _StepSize * transmittance;
                       transmittance *= exp(-transitionDensity * _StepSize);    
                   }
                   distTravelled += _StepSize;
               }
               // --- LOOP 2 ---
               while(distTravelled > transitionDistLimit && distTravelled < farDistLimit)
               {
                   float3 rayPos = entryPoint + rayDir * distTravelled;
                   float farDensity = get_far_density();
                   if (rayPos.y >= _WaterHeight)  
                   {
                       farDensity = 0;
                   }
                   if (farDensity > 0)
                   {
                       Light mainLight = GetMainLight(TransformWorldToShadowCoord(rayPos));
                       float angle = dot(rayDir, mainLight.direction);
                       // 4. Added * transmittance for self-occlusion
                       lightAccumulation += _LightContribution.rgb * henyey_greenstein(angle, _LightScattering) * farDensity * mainLight.shadowAttenuation * _StepSize * transmittance;
                       transmittance *= exp(-farDensity * _StepSize);
                   }
                   distTravelled += _StepSize;
               }
               // 2 & 3. Proper additive light blend using the unified variables
               float3 ambientFog = _Color.rgb * (1.0 - saturate(transmittance));
               float3 finalColor = (color.rgb * transmittance) + ambientFog + lightAccumulation;
               return float4(finalColor, color.a);
           }
            ENDHLSL
        }
    }
} 