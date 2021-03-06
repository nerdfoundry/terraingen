using nfg.Unity.Jobs;
using nfg.Util;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace nfg.Unity.TerrainGen {

    [BurstCompile]
    public struct JobCreateColorMap : IJobFor {
        [ReadOnly]
        public SettingsCoherentNoise settingsNoise;
        [ReadOnly]
        public NativeArray<Vector3> n_vecMesh;
        [ReadOnly]
        public NativeArray<Vector3> n_vecNoise;
        [ReadOnly]
        public NativeArray<RegionEntryData> n_regions;
        [ReadOnly]
        public NativeCurve n_regionBlendCurve;
        [ReadOnly]
        public NativeFastNoiseLite fastNoiseGen;

        public NativeArray<Color> n_colorMap;

        public void Execute(int index) {
            float chunkCenterOffset = (settingsNoise.Width - 1) / -2f;

            Vector3 worldPoint = n_vecMesh[index]
                + new Vector3(
                    chunkCenterOffset + settingsNoise.PositionOffset.x,
                    0,
                    chunkCenterOffset - settingsNoise.PositionOffset.y
                );

            // Find fun noise offset so we get interesting blends
            float worldPerlinValue = fastNoiseGen.GetNoise(worldPoint.x, worldPoint.y, worldPoint.z);

            Color color = RegionTextureGenerator.GenerateColorForHeight(
                n_regions,
                n_regionBlendCurve,
                n_vecNoise[index].y,
                worldPerlinValue,
                settingsNoise.Scale
            );

            // Color color = Color.Lerp(Color.black, Color.white, n_vecNoise[index].y);

            n_colorMap[index] = color;
        }
    }

}