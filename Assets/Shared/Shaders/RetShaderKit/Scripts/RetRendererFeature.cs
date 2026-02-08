using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

namespace RetShaderKit
{
    public class RetRendererFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class RetSettings
        {
            public Material masterMaterial;
            [Range(0.01f, 1.0f)] public float pixelationFactor = 0.5f;
            public Vector3 colorResolution = new Vector3(32, 32, 32);
            public Vector3 ditherResolution = new Vector3(16, 16, 16);
            [Range(0, 1)] public float ditherMode = 1.0f;
            public float ditheringScale = 1.0f;
            [Range(1.0f, 2.0f)] public float contrastBoost = 1.3f; // Added Contrast here
        }

        public RetSettings settings = new RetSettings();

        class RetRenderPass : ScriptableRenderPass
        {
            public RetSettings passSettings;

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                if (passSettings.masterMaterial == null) return;

                UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
                UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
                TextureHandle screenColor = resourceData.activeColorTexture;

                if (!screenColor.IsValid()) return;

                // --- THIS IS THE CRITICAL PART: PUSH VALUES TO SHADER ---
                passSettings.masterMaterial.SetFloat("_PixelationFactor", passSettings.pixelationFactor);
                passSettings.masterMaterial.SetVector("_ColorResolution", passSettings.colorResolution);
                passSettings.masterMaterial.SetVector("_DitherResolution", passSettings.ditherResolution);
                passSettings.masterMaterial.SetFloat("_HighResDitherMatrix", passSettings.ditherMode);
                passSettings.masterMaterial.SetFloat("_DitheringScale", passSettings.ditheringScale);
                passSettings.masterMaterial.SetFloat("_ContrastBoost", passSettings.contrastBoost);

                TextureDesc desc = new TextureDesc(cameraData.cameraTargetDescriptor.width, cameraData.cameraTargetDescriptor.height);
                desc.format = cameraData.cameraTargetDescriptor.graphicsFormat;
                desc.name = "Ret_Scratchpad";
                TextureHandle tempTex = renderGraph.CreateTexture(desc);

                using (var builder = renderGraph.AddRasterRenderPass<PassData>("RetPostPass", out var passData))
                {
                    passData.material = passSettings.masterMaterial;
                    passData.source = screenColor;
                    builder.UseTexture(screenColor, AccessFlags.Read);
                    builder.SetRenderAttachment(tempTex, 0);
                    builder.SetRenderFunc((PassData data, RasterGraphContext context) => {
                        Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), data.material, 0);
                    });
                }

                using (var builder = renderGraph.AddRasterRenderPass<PassData>("RetCopyPass", out var passData))
                {
                    passData.source = tempTex;
                    builder.UseTexture(tempTex, AccessFlags.Read);
                    builder.SetRenderAttachment(screenColor, 0);
                    builder.SetRenderFunc((PassData data, RasterGraphContext context) => {
                        Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), 0, false);
                    });
                }
            }
        }

        private class PassData { public Material material; public TextureHandle source; }
        RetRenderPass _myPass;

        public override void Create() {
            _myPass = new RetRenderPass { passSettings = settings, renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
            if (settings.masterMaterial != null) renderer.EnqueuePass(_myPass);
        }
    }
}