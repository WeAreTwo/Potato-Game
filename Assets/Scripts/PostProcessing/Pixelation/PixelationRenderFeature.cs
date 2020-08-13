using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PotatoGame
{

    public class PixelationRenderFeature : ScriptableRendererFeature
    {
        class PixelationPass : ScriptableRenderPass
        {
            private RenderTargetIdentifier source { get; set; }
            private RenderTargetHandle destination { get; set; }
            public Material pixelationMaterial = null;
            RenderTargetHandle temporaryColorTexture;

            public void Setup(RenderTargetIdentifier source, RenderTargetHandle destination)
            {
                this.source = source;
                this.destination = destination;
            }

            public PixelationPass(Material pixelationMaterial)
            {
                this.pixelationMaterial = pixelationMaterial;
            }



            // This method is called before executing the render pass.
            // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
            // When empty this render pass will render to the active camera render target.
            // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
            // The render pipeline will ensure target setup and clearing happens in an performance manner.
            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {

            }

            // Here you can implement the rendering logic.
            // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
            // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
            // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                CommandBuffer cmd = CommandBufferPool.Get("_OutlinePass");

                RenderTextureDescriptor opaqueDescriptor = renderingData.cameraData.cameraTargetDescriptor;
                opaqueDescriptor.depthBufferBits = 0;

                if (destination == RenderTargetHandle.CameraTarget)
                {
                    cmd.GetTemporaryRT(temporaryColorTexture.id, opaqueDescriptor, FilterMode.Point);
                    Blit(cmd, source, temporaryColorTexture.Identifier(), pixelationMaterial, 0);
                    Blit(cmd, temporaryColorTexture.Identifier(), source);

                }
                else Blit(cmd, source, destination.Identifier(), pixelationMaterial, 0);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            /// Cleanup any allocated resources that were created during the execution of this render pass.
            public override void FrameCleanup(CommandBuffer cmd)
            {

                if (destination == RenderTargetHandle.CameraTarget)
                    cmd.ReleaseTemporaryRT(temporaryColorTexture.id);
            }
        }

        [System.Serializable]
        public class PixelationSettings
        {
            public Material outlineMaterial = null;
        }

        public PixelationSettings settings = new PixelationSettings();
        PixelationPass m_pixelationPass;
        RenderTargetHandle pixelationTexture;

        public override void Create()
        {
            m_pixelationPass = new PixelationPass(settings.outlineMaterial);
            m_pixelationPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            pixelationTexture.Init("_PixelationTexture");
        }

        // Here you can inject one or multiple render passes in the renderer.
        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (settings.outlineMaterial == null)
            {
                Debug.LogWarningFormat("Missing Outline Material");
                return;
            }

            m_pixelationPass.Setup(renderer.cameraColorTarget, RenderTargetHandle.CameraTarget);
            renderer.EnqueuePass(m_pixelationPass);
        }
    }

}


