using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PotatoGame
{
    public class OutlineRenderFeature : ScriptableRendererFeature
    {
        OutlinePass outlinePass;

        public override void Create()
        {
            outlinePass = new OutlinePass(RenderPassEvent.BeforeRenderingPostProcessing);
        }

        //ScripstableRendererFeature is an abstract class, you need this method
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            outlinePass.Setup(renderer.cameraColorTarget);
            renderer.EnqueuePass(outlinePass);
        }
    }
    
    
    public class OutlinePass : ScriptableRenderPass
    {
        private static readonly string shaderPath = "URP/Outline";
        static readonly string k_RenderTag = "Render Outline Effects";
        static readonly int MainTexId = Shader.PropertyToID("_MainTex");
        static readonly int TempTargetId = Shader.PropertyToID("_TempTargetOutline");
        
        //PROPERTIES
        static readonly int PatternIndex = Shader.PropertyToID("_PatternIndex");
        static readonly int DitherThreshold = Shader.PropertyToID("_DitherThreshold");
        static readonly int DitherStrength = Shader.PropertyToID("_DitherStrength");
        static readonly int DitherScale = Shader.PropertyToID("_DitherScale");
        
        Outline outline;
        Material outlineMaterial;
        RenderTargetIdentifier currentTarget;
    
        public OutlinePass(RenderPassEvent evt)
        {
            renderPassEvent = evt;
            var shader = Shader.Find(shaderPath);
            if (shader == null)
            {
                Debug.LogError("Shader not found.");
                return;
            }
            this.outlineMaterial = CoreUtils.CreateEngineMaterial(shader);
        }
    
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (this.outlineMaterial == null)
            {
                Debug.LogError("Material not created.");
                return;
            }
    
            if (!renderingData.cameraData.postProcessEnabled) return;
    
            var stack = VolumeManager.instance.stack;
            
            this.outline = stack.GetComponent<Outline>();
            if (this.outline == null) { return; }
            if (!this.outline.IsActive()) { return; }
    
            var cmd = CommandBufferPool.Get(k_RenderTag);
            Render(cmd, ref renderingData);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    
        public void Setup(in RenderTargetIdentifier currentTarget)
        {
            this.currentTarget = currentTarget;
        }
    
        void Render(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ref var cameraData = ref renderingData.cameraData;
            var source = currentTarget;
            int destination = TempTargetId;
    
            //getting camera width and height 
            var w = cameraData.camera.scaledPixelWidth;
            var h = cameraData.camera.scaledPixelHeight;
            
            //setting parameters here 
            cameraData.camera.depthTextureMode = cameraData.camera.depthTextureMode | DepthTextureMode.Depth;
            this.outlineMaterial.SetInt(PatternIndex, this.outline.patternIndex.value);
            this.outlineMaterial.SetFloat(DitherThreshold, this.outline.ditherThreshold.value);
            this.outlineMaterial.SetFloat(DitherStrength, this.outline.ditherStrength.value);
            this.outlineMaterial.SetFloat(DitherScale, this.outline.ditherScale.value);

            int shaderPass = 0;
            cmd.SetGlobalTexture(MainTexId, source);
            cmd.GetTemporaryRT(destination, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
            cmd.Blit(source, destination);
            cmd.Blit(destination, source, this.outlineMaterial, shaderPass);
        }
    }
}