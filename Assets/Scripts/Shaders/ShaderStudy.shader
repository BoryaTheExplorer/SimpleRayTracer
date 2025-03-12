Shader "Unlit/ShaderStudy"
{
    Properties
    {
        _CircleRadius("Circle Radius", Range(0, 4)) = 0.5
        _CircleCenter("Circle Center", Vector) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct MeshData
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 vertex : SV_POSITION;
            };

            float _CircleRadius;
            float4 _CircleCenter;

            v2f vert (MeshData v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uv *= 16;

                o.normal = UnityObjectToWorldNormal( v.normal );
                
                return o;
            }

            float circle(float2 center, float2 samplePoint, float radius)
            {
                return distance(center, samplePoint) - radius;
            }
            float rectangle(float2 samplePoint, float halfSize)
            {
                float2 cmpWiseEdgeDist = abs(samplePoint) - halfSize;
                float outDist = length(max(cmpWiseEdgeDist, 0));
                float inDist = min(max(cmpWiseEdgeDist.x, cmpWiseEdgeDist.y), 0);
                return outDist + inDist;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //float4 circleMask = float4(step(circle(_CircleCenter.xy, i.uv, _CircleRadius), 0).xxx, 1);
                //float4 secondCircleMask = float4(step(circle(_CircleCenter.xy + float2(2, 5), i.uv, _CircleRadius), 0).xxx, 1);
                
                //float4 circleColor = lerp(0, float4(1, 0, 1, 1), circleMask);
                //float4 secondCircleColor = lerp(0, float4(1, 0, 0, 1), secondCircleMask);

                float circle1 = circle(_CircleCenter, i.uv.xy, _CircleRadius);
                float circle2 = circle(_CircleCenter + float2 (5, 5), i.uv.xy, _CircleRadius);


                float rec = rectangle(i.uv + float2(-8, -8), 4);
                //circle1 = step(-circle1, 0);
                //circle2 = step(-circle2, 0);
                
                float col = length(min(float3(circle1, -rec, circle2), 0));
                col = step(col, 0.5);
                //float4 endColor = max(circleColor, secondCircleColor);
                return col;
                //return length(min(float2(circle1, circle2), 0));
                //return i.normal.xyzz;
            }
            ENDCG
        }
    }
}
