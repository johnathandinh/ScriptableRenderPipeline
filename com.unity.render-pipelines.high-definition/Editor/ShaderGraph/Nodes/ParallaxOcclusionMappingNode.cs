using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine.Experimental.Rendering.HDPipeline;
using System;
using UnityEngine.Rendering;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    [Title("Input", "High Definition Render Pipeline", "Parallax Occlusion Mapping")]
    class ParallaxOcclusionMappingNode : AbstractMaterialNode, IGeneratesBodyCode
    {
        public ParallaxOcclusionMappingNode()
        {
            name = "Parallax Occlusion Mapping";
            UpdateNodeAfterDeserialization();
        }

        public override string documentationURL
        {
            // This still needs to be added.
            get { return "https://github.com/Unity-Technologies/ShaderGraph/wiki/Parallax-Occlusion-Mapping-Node"; }
        }

        // [SerializeField] float      m_Lod;
        // [SerializeField] float      m_LodThreshold;
        // [SerializeField] float      m_Amplitude;
        // [SerializeField] int        m_numStep;
        // [SerializeField] Vector2    m_UVs;

        // Input slots
        private const int kHeightmapSlotId = 2;
        private const string kHeightmapSlotName = "Heightmap";
        private const int kAmplitudeSlotId = 3;
        private const string kAmplitudeSlotName = "Amplitude";
        private const int kStepsSlotId = 4;
        private const string kStepsSlotName = "Steps";
        private const int kUVsSlotId = 5;
        private const string kUVsSlotName = "UVs";
        private const int kLodSlotId = 6;
        private const string kLodSlotName = "Lod";
        private const int kLodThresholdSlotId = 7;
        private const string kLodThresholdSlotName = "LodThreshold";
        private const int kViewDirTSSlotId = 8;
        private const string kViewDirTSSlotName = "ViewDirTS";


        // Output slots
        private const int kPixelDepthOffsetOutputSlotId = 0;
        private const string kPixelDepthOffsetOutputSlotName = "PixelDepthOffset";
        private const int kParallaxUVsOutputSlotId = 1;
        private const string kParallaxUVsOutputSlotName = "ParallaxUVs";

        public override bool hasPreview { get { return false; } }

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new Texture2DMaterialSlot(kHeightmapSlotId, kHeightmapSlotName, kHeightmapSlotName, SlotType.Input, ShaderStageCapability.Fragment));
            AddSlot(new Vector1MaterialSlot(kAmplitudeSlotId, kAmplitudeSlotName, kAmplitudeSlotName, SlotType.Input, 1.0f, ShaderStageCapability.Fragment));
            AddSlot(new Vector1MaterialSlot(kStepsSlotId, kStepsSlotName, kStepsSlotName, SlotType.Input, 5.0f, ShaderStageCapability.Fragment));
            AddSlot(new Vector2MaterialSlot(kUVsSlotId, kUVsSlotName, kUVsSlotName, SlotType.Input, Vector2.zero, ShaderStageCapability.Fragment));
            AddSlot(new Vector1MaterialSlot(kLodSlotId, kLodSlotName, kLodSlotName, SlotType.Input, 0.0f, ShaderStageCapability.Fragment));
            AddSlot(new Vector1MaterialSlot(kLodThresholdSlotId, kLodThresholdSlotName, kLodThresholdSlotName, SlotType.Input, 0.0f, ShaderStageCapability.Fragment));
            AddSlot(new Vector3MaterialSlot(kViewDirTSSlotId, kViewDirTSSlotName, kViewDirTSSlotName, SlotType.Input, Vector3.zero, ShaderStageCapability.Fragment));

            AddSlot(new Vector1MaterialSlot(kPixelDepthOffsetOutputSlotId, kPixelDepthOffsetOutputSlotName, kPixelDepthOffsetOutputSlotName, SlotType.Output, 0.0f, ShaderStageCapability.Fragment));
            AddSlot(new Vector2MaterialSlot(kParallaxUVsOutputSlotId, kParallaxUVsOutputSlotName, kParallaxUVsOutputSlotName, SlotType.Output, Vector2.zero, ShaderStageCapability.Fragment));
            RemoveSlotsNameNotMatching(new[] {
                kPixelDepthOffsetOutputSlotId,
                kParallaxUVsOutputSlotId,
                kHeightmapSlotId,
                kAmplitudeSlotId,
                kStepsSlotId,
                kUVsSlotId,
                kLodSlotId,
                kLodThresholdSlotId,
                kViewDirTSSlotId
            });
        }

        public void GenerateNodeCode(ShaderGenerator visitor, GraphContext graphContext, GenerationMode generationMode)
        {
            string heightmap = GetSlotValue(kHeightmapSlotId, generationMode);
            string amplitude = GetSlotValue(kAmplitudeSlotId, generationMode);
            string steps = GetSlotValue(kHeightmapSlotId, generationMode);
            string uvs = GetSlotValue(kUVsSlotId, generationMode);
            string lod = GetSlotValue(kLodSlotId, generationMode);
            string lodThreshold = GetSlotValue(kLodThresholdSlotId, generationMode);
            string viewDirTS = GetSlotValue(kViewDirTSSlotId, generationMode);

            string p = "PerPixelHeightDisplacementParam p; p.uv = " + uvs + ";"; // TODO: random name here;
            visitor.AddShaderChunk(p);
            visitor.AddShaderChunk(String.Format(@"
                {0} {6};
                {0}2 {1} = ParallaxOcclusionMapping({2}, {3}, {4}, {5}, p, out {6})",
                precision,
                GetVariableNameForSlot(kParallaxUVsOutputSlotId),
                lod,
                lodThreshold,
                steps,
                viewDirTS,
                p,
                GetVariableNameForSlot(kPixelDepthOffsetOutputSlotId)
                ));
        }
    }
}
