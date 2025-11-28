using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000302 RID: 770
[StructLayout(3)]
public struct MaterialFingerprint
{
	// Token: 0x060012C9 RID: 4809 RVA: 0x0006262C File Offset: 0x0006082C
	public MaterialFingerprint(UberShaderMatUsedProps used)
	{
		Material material = used.material;
		this._TransparencyMode = MaterialFingerprint.GetMatTransparencyMode(material);
		this._Cutoff = MaterialFingerprint._Round(material.GetFloat(ShaderProps._Cutoff), 100, used._Cutoff);
		this._ColorSource = ((used._ColorSource > 0) ? material.GetInt(ShaderProps._ColorSource) : 0);
		this._BaseColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._BaseColor), 100, used._BaseColor);
		this._GChannelColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._GChannelColor), 100, used._GChannelColor);
		this._BChannelColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._BChannelColor), 100, used._BChannelColor);
		this._AChannelColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._AChannelColor), 100, used._AChannelColor);
		this._TexMipBias = MaterialFingerprint._Round(material.GetFloat(ShaderProps._TexMipBias), 100, used._TexMipBias);
		this._BaseMap = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._BaseMap, used._BaseMap);
		this._BaseMap_ST = MaterialFingerprint._Round(material.GetVector(ShaderProps._BaseMap_ST), 100, used._BaseMap_ST);
		this._BaseMap_WH = MaterialFingerprint._Round(material.GetVector(ShaderProps._BaseMap_WH), 100, used._BaseMap_WH);
		this._TexelSnapToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._TexelSnapToggle), 100, used._TexelSnapToggle);
		this._TexelSnap_Factor = MaterialFingerprint._Round(material.GetFloat(ShaderProps._TexelSnap_Factor), 100, used._TexelSnap_Factor);
		this._UVSource = ((used._UVSource > 0) ? material.GetInt(ShaderProps._UVSource) : 0);
		this._AlphaDetailToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._AlphaDetailToggle), 100, used._AlphaDetailToggle);
		this._AlphaDetail_ST = MaterialFingerprint._Round(material.GetVector(ShaderProps._AlphaDetail_ST), 100, used._AlphaDetail_ST);
		this._AlphaDetail_Opacity = MaterialFingerprint._Round(material.GetFloat(ShaderProps._AlphaDetail_Opacity), 100, used._AlphaDetail_Opacity);
		this._AlphaDetail_WorldSpace = MaterialFingerprint._Round(material.GetFloat(ShaderProps._AlphaDetail_WorldSpace), 100, used._AlphaDetail_WorldSpace);
		this._MaskMapToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._MaskMapToggle), 100, used._MaskMapToggle);
		this._MaskMap = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._MaskMap, used._MaskMap);
		this._MaskMap_ST = MaterialFingerprint._Round(material.GetVector(ShaderProps._MaskMap_ST), 100, used._MaskMap_ST);
		this._MaskMap_WH = MaterialFingerprint._Round(material.GetVector(ShaderProps._MaskMap_WH), 100, used._MaskMap_WH);
		this._LavaLampToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._LavaLampToggle), 100, used._LavaLampToggle);
		this._GradientMapToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._GradientMapToggle), 100, used._GradientMapToggle);
		this._GradientMap = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._GradientMap, used._GradientMap);
		this._DoTextureRotation = MaterialFingerprint._Round(material.GetFloat(ShaderProps._DoTextureRotation), 100, used._DoTextureRotation);
		this._RotateAngle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._RotateAngle), 100, used._RotateAngle);
		this._RotateAnim = MaterialFingerprint._Round(material.GetFloat(ShaderProps._RotateAnim), 100, used._RotateAnim);
		this._UseWaveWarp = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseWaveWarp), 100, used._UseWaveWarp);
		this._WaveAmplitude = MaterialFingerprint._Round(material.GetFloat(ShaderProps._WaveAmplitude), 100, used._WaveAmplitude);
		this._WaveFrequency = MaterialFingerprint._Round(material.GetFloat(ShaderProps._WaveFrequency), 100, used._WaveFrequency);
		this._WaveScale = MaterialFingerprint._Round(material.GetFloat(ShaderProps._WaveScale), 100, used._WaveScale);
		this._WaveTimeScale = MaterialFingerprint._Round(material.GetFloat(ShaderProps._WaveTimeScale), 100, used._WaveTimeScale);
		this._UseWeatherMap = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseWeatherMap), 100, used._UseWeatherMap);
		this._WeatherMap = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._WeatherMap, used._WeatherMap);
		this._WeatherMapDissolveEdgeSize = MaterialFingerprint._Round(material.GetFloat(ShaderProps._WeatherMapDissolveEdgeSize), 100, used._WeatherMapDissolveEdgeSize);
		this._ReflectToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ReflectToggle), 100, used._ReflectToggle);
		this._ReflectBoxProjectToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ReflectBoxProjectToggle), 100, used._ReflectBoxProjectToggle);
		this._ReflectBoxCubePos = MaterialFingerprint._Round(material.GetVector(ShaderProps._ReflectBoxCubePos), 100, used._ReflectBoxCubePos);
		this._ReflectBoxSize = MaterialFingerprint._Round(material.GetVector(ShaderProps._ReflectBoxSize), 100, used._ReflectBoxSize);
		this._ReflectBoxRotation = MaterialFingerprint._Round(material.GetVector(ShaderProps._ReflectBoxRotation), 100, used._ReflectBoxRotation);
		this._ReflectMatcapToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ReflectMatcapToggle), 100, used._ReflectMatcapToggle);
		this._ReflectMatcapPerspToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ReflectMatcapPerspToggle), 100, used._ReflectMatcapPerspToggle);
		this._ReflectNormalToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ReflectNormalToggle), 100, used._ReflectNormalToggle);
		this._ReflectTex = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._ReflectTex, used._ReflectTex);
		this._ReflectNormalTex = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._ReflectNormalTex, used._ReflectNormalTex);
		this._ReflectAlbedoTint = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ReflectAlbedoTint), 100, used._ReflectAlbedoTint);
		this._ReflectTint = MaterialFingerprint._Round(material.GetColor(ShaderProps._ReflectTint), 100, used._ReflectTint);
		this._ReflectOpacity = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ReflectOpacity), 100, used._ReflectOpacity);
		this._ReflectExposure = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ReflectExposure), 100, used._ReflectExposure);
		this._ReflectOffset = MaterialFingerprint._Round(material.GetVector(ShaderProps._ReflectOffset), 100, used._ReflectOffset);
		this._ReflectScale = MaterialFingerprint._Round(material.GetVector(ShaderProps._ReflectScale), 100, used._ReflectScale);
		this._ReflectRotate = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ReflectRotate), 100, used._ReflectRotate);
		this._HalfLambertToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._HalfLambertToggle), 100, used._HalfLambertToggle);
		this._ParallaxPlanarToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ParallaxPlanarToggle), 100, used._ParallaxPlanarToggle);
		this._ParallaxToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ParallaxToggle), 100, used._ParallaxToggle);
		this._ParallaxAAToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ParallaxAAToggle), 100, used._ParallaxAAToggle);
		this._ParallaxAABias = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ParallaxAABias), 100, used._ParallaxAABias);
		this._DepthMap = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._DepthMap, used._DepthMap);
		this._ParallaxAmplitude = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ParallaxAmplitude), 100, used._ParallaxAmplitude);
		this._ParallaxSamplesMinMax = MaterialFingerprint._Round(material.GetVector(ShaderProps._ParallaxSamplesMinMax), 100, used._ParallaxSamplesMinMax);
		this._UvShiftToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UvShiftToggle), 100, used._UvShiftToggle);
		this._UvShiftSteps = MaterialFingerprint._Round(material.GetVector(ShaderProps._UvShiftSteps), 100, used._UvShiftSteps);
		this._UvShiftRate = MaterialFingerprint._Round(material.GetVector(ShaderProps._UvShiftRate), 100, used._UvShiftRate);
		this._UvShiftOffset = MaterialFingerprint._Round(material.GetVector(ShaderProps._UvShiftOffset), 100, used._UvShiftOffset);
		this._UseGridEffect = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseGridEffect), 100, used._UseGridEffect);
		this._UseCrystalEffect = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseCrystalEffect), 100, used._UseCrystalEffect);
		this._CrystalPower = MaterialFingerprint._Round(material.GetFloat(ShaderProps._CrystalPower), 100, used._CrystalPower);
		this._CrystalRimColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._CrystalRimColor), 100, used._CrystalRimColor);
		this._LiquidVolume = MaterialFingerprint._Round(material.GetFloat(ShaderProps._LiquidVolume), 100, used._LiquidVolume);
		this._LiquidFill = MaterialFingerprint._Round(material.GetFloat(ShaderProps._LiquidFill), 100, used._LiquidFill);
		this._LiquidFillNormal = MaterialFingerprint._Round(material.GetVector(ShaderProps._LiquidFillNormal), 100, used._LiquidFillNormal);
		this._LiquidSurfaceColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._LiquidSurfaceColor), 100, used._LiquidSurfaceColor);
		this._LiquidSwayX = MaterialFingerprint._Round(material.GetFloat(ShaderProps._LiquidSwayX), 100, used._LiquidSwayX);
		this._LiquidSwayY = MaterialFingerprint._Round(material.GetFloat(ShaderProps._LiquidSwayY), 100, used._LiquidSwayY);
		this._LiquidContainer = MaterialFingerprint._Round(material.GetFloat(ShaderProps._LiquidContainer), 100, used._LiquidContainer);
		this._LiquidPlanePosition = MaterialFingerprint._Round(material.GetVector(ShaderProps._LiquidPlanePosition), 100, used._LiquidPlanePosition);
		this._LiquidPlaneNormal = MaterialFingerprint._Round(material.GetVector(ShaderProps._LiquidPlaneNormal), 100, used._LiquidPlaneNormal);
		this._VertexFlapToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._VertexFlapToggle), 100, used._VertexFlapToggle);
		this._VertexFlapAxis = MaterialFingerprint._Round(material.GetVector(ShaderProps._VertexFlapAxis), 100, used._VertexFlapAxis);
		this._VertexFlapDegreesMinMax = MaterialFingerprint._Round(material.GetVector(ShaderProps._VertexFlapDegreesMinMax), 100, used._VertexFlapDegreesMinMax);
		this._VertexFlapSpeed = MaterialFingerprint._Round(material.GetFloat(ShaderProps._VertexFlapSpeed), 100, used._VertexFlapSpeed);
		this._VertexFlapPhaseOffset = MaterialFingerprint._Round(material.GetFloat(ShaderProps._VertexFlapPhaseOffset), 100, used._VertexFlapPhaseOffset);
		this._VertexWaveToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._VertexWaveToggle), 100, used._VertexWaveToggle);
		this._VertexWaveDebug = MaterialFingerprint._Round(material.GetFloat(ShaderProps._VertexWaveDebug), 100, used._VertexWaveDebug);
		this._VertexWaveEnd = MaterialFingerprint._Round(material.GetVector(ShaderProps._VertexWaveEnd), 100, used._VertexWaveEnd);
		this._VertexWaveParams = MaterialFingerprint._Round(material.GetVector(ShaderProps._VertexWaveParams), 100, used._VertexWaveParams);
		this._VertexWaveFalloff = MaterialFingerprint._Round(material.GetVector(ShaderProps._VertexWaveFalloff), 100, used._VertexWaveFalloff);
		this._VertexWaveSphereMask = MaterialFingerprint._Round(material.GetVector(ShaderProps._VertexWaveSphereMask), 100, used._VertexWaveSphereMask);
		this._VertexWavePhaseOffset = MaterialFingerprint._Round(material.GetFloat(ShaderProps._VertexWavePhaseOffset), 100, used._VertexWavePhaseOffset);
		this._VertexWaveAxes = MaterialFingerprint._Round(material.GetVector(ShaderProps._VertexWaveAxes), 100, used._VertexWaveAxes);
		this._VertexRotateToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._VertexRotateToggle), 100, used._VertexRotateToggle);
		this._VertexRotateAngles = MaterialFingerprint._Round(material.GetVector(ShaderProps._VertexRotateAngles), 100, used._VertexRotateAngles);
		this._VertexRotateAnim = MaterialFingerprint._Round(material.GetFloat(ShaderProps._VertexRotateAnim), 100, used._VertexRotateAnim);
		this._VertexLightToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._VertexLightToggle), 100, used._VertexLightToggle);
		this._InnerGlowOn = MaterialFingerprint._Round(material.GetFloat(ShaderProps._InnerGlowOn), 100, used._InnerGlowOn);
		this._InnerGlowColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._InnerGlowColor), 100, used._InnerGlowColor);
		this._InnerGlowParams = MaterialFingerprint._Round(material.GetVector(ShaderProps._InnerGlowParams), 100, used._InnerGlowParams);
		this._InnerGlowTap = MaterialFingerprint._Round(material.GetFloat(ShaderProps._InnerGlowTap), 100, used._InnerGlowTap);
		this._InnerGlowSine = MaterialFingerprint._Round(material.GetFloat(ShaderProps._InnerGlowSine), 100, used._InnerGlowSine);
		this._InnerGlowSinePeriod = MaterialFingerprint._Round(material.GetFloat(ShaderProps._InnerGlowSinePeriod), 100, used._InnerGlowSinePeriod);
		this._InnerGlowSinePhaseShift = MaterialFingerprint._Round(material.GetFloat(ShaderProps._InnerGlowSinePhaseShift), 100, used._InnerGlowSinePhaseShift);
		this._StealthEffectOn = MaterialFingerprint._Round(material.GetFloat(ShaderProps._StealthEffectOn), 100, used._StealthEffectOn);
		this._UseEyeTracking = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseEyeTracking), 100, used._UseEyeTracking);
		this._EyeTileOffsetUV = MaterialFingerprint._Round(material.GetVector(ShaderProps._EyeTileOffsetUV), 100, used._EyeTileOffsetUV);
		this._EyeOverrideUV = MaterialFingerprint._Round(material.GetFloat(ShaderProps._EyeOverrideUV), 100, used._EyeOverrideUV);
		this._EyeOverrideUVTransform = MaterialFingerprint._Round(material.GetVector(ShaderProps._EyeOverrideUVTransform), 100, used._EyeOverrideUVTransform);
		this._UseMouthFlap = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseMouthFlap), 100, used._UseMouthFlap);
		this._MouthMap = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._MouthMap, used._MouthMap);
		this._MouthMap_ST = MaterialFingerprint._Round(material.GetVector(ShaderProps._MouthMap_ST), 100, used._MouthMap_ST);
		this._UseVertexColor = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseVertexColor), 100, used._UseVertexColor);
		this._WaterEffect = MaterialFingerprint._Round(material.GetFloat(ShaderProps._WaterEffect), 100, used._WaterEffect);
		this._HeightBasedWaterEffect = MaterialFingerprint._Round(material.GetFloat(ShaderProps._HeightBasedWaterEffect), 100, used._HeightBasedWaterEffect);
		this._WaterCaustics = MaterialFingerprint._Round(material.GetFloat(ShaderProps._WaterCaustics), 100, used._WaterCaustics);
		this._UseDayNightLightmap = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseDayNightLightmap), 100, used._UseDayNightLightmap);
		this._UseSpecular = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseSpecular), 100, used._UseSpecular);
		this._UseSpecularAlphaChannel = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseSpecularAlphaChannel), 100, used._UseSpecularAlphaChannel);
		this._Smoothness = MaterialFingerprint._Round(material.GetFloat(ShaderProps._Smoothness), 100, used._Smoothness);
		this._UseSpecHighlight = MaterialFingerprint._Round(material.GetFloat(ShaderProps._UseSpecHighlight), 100, used._UseSpecHighlight);
		this._SpecularDir = MaterialFingerprint._Round(material.GetVector(ShaderProps._SpecularDir), 100, used._SpecularDir);
		this._SpecularPowerIntensity = MaterialFingerprint._Round(material.GetVector(ShaderProps._SpecularPowerIntensity), 100, used._SpecularPowerIntensity);
		this._SpecularColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._SpecularColor), 100, used._SpecularColor);
		this._SpecularUseDiffuseColor = MaterialFingerprint._Round(material.GetFloat(ShaderProps._SpecularUseDiffuseColor), 100, used._SpecularUseDiffuseColor);
		this._EmissionToggle = MaterialFingerprint._Round(material.GetFloat(ShaderProps._EmissionToggle), 100, used._EmissionToggle);
		this._EmissionColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._EmissionColor), 100, used._EmissionColor);
		this._EmissionMap = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._EmissionMap, used._EmissionMap);
		this._EmissionMaskByBaseMapAlpha = MaterialFingerprint._Round(material.GetFloat(ShaderProps._EmissionMaskByBaseMapAlpha), 100, used._EmissionMaskByBaseMapAlpha);
		this._EmissionUVScrollSpeed = MaterialFingerprint._Round(material.GetVector(ShaderProps._EmissionUVScrollSpeed), 100, used._EmissionUVScrollSpeed);
		this._EmissionDissolveProgress = MaterialFingerprint._Round(material.GetFloat(ShaderProps._EmissionDissolveProgress), 100, used._EmissionDissolveProgress);
		this._EmissionDissolveAnimation = MaterialFingerprint._Round(material.GetVector(ShaderProps._EmissionDissolveAnimation), 100, used._EmissionDissolveAnimation);
		this._EmissionDissolveEdgeSize = MaterialFingerprint._Round(material.GetFloat(ShaderProps._EmissionDissolveEdgeSize), 100, used._EmissionDissolveEdgeSize);
		this._EmissionIntensityInDynamic = MaterialFingerprint._Round(material.GetFloat(ShaderProps._EmissionIntensityInDynamic), 100, used._EmissionIntensityInDynamic);
		this._EmissionUseUVWaveWarp = MaterialFingerprint._Round(material.GetFloat(ShaderProps._EmissionUseUVWaveWarp), 100, used._EmissionUseUVWaveWarp);
		this._GreyZoneException = MaterialFingerprint._Round(material.GetFloat(ShaderProps._GreyZoneException), 100, used._GreyZoneException);
		this._Cull = MaterialFingerprint._Round(material.GetFloat(ShaderProps._Cull), 100, used._Cull);
		this._StencilReference = MaterialFingerprint._Round(material.GetFloat(ShaderProps._StencilReference), 100, used._StencilReference);
		this._StencilComparison = MaterialFingerprint._Round(material.GetFloat(ShaderProps._StencilComparison), 100, used._StencilComparison);
		this._StencilPassFront = MaterialFingerprint._Round(material.GetFloat(ShaderProps._StencilPassFront), 100, used._StencilPassFront);
		this._USE_DEFORM_MAP = MaterialFingerprint._Round(material.GetFloat(ShaderProps._USE_DEFORM_MAP), 100, used._USE_DEFORM_MAP);
		this._DeformMap = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._DeformMap, used._DeformMap);
		this._DeformMapIntensity = MaterialFingerprint._Round(material.GetFloat(ShaderProps._DeformMapIntensity), 100, used._DeformMapIntensity);
		this._DeformMapMaskByVertColorRAmount = MaterialFingerprint._Round(material.GetFloat(ShaderProps._DeformMapMaskByVertColorRAmount), 100, used._DeformMapMaskByVertColorRAmount);
		this._DeformMapScrollSpeed = MaterialFingerprint._Round(material.GetVector(ShaderProps._DeformMapScrollSpeed), 100, used._DeformMapScrollSpeed);
		this._DeformMapUV0Influence = MaterialFingerprint._Round(material.GetVector(ShaderProps._DeformMapUV0Influence), 100, used._DeformMapUV0Influence);
		this._DeformMapObjectSpaceOffsetsU = MaterialFingerprint._Round(material.GetVector(ShaderProps._DeformMapObjectSpaceOffsetsU), 100, used._DeformMapObjectSpaceOffsetsU);
		this._DeformMapObjectSpaceOffsetsV = MaterialFingerprint._Round(material.GetVector(ShaderProps._DeformMapObjectSpaceOffsetsV), 100, used._DeformMapObjectSpaceOffsetsV);
		this._DeformMapWorldSpaceOffsetsU = MaterialFingerprint._Round(material.GetVector(ShaderProps._DeformMapWorldSpaceOffsetsU), 100, used._DeformMapWorldSpaceOffsetsU);
		this._DeformMapWorldSpaceOffsetsV = MaterialFingerprint._Round(material.GetVector(ShaderProps._DeformMapWorldSpaceOffsetsV), 100, used._DeformMapWorldSpaceOffsetsV);
		this._RotateOnYAxisBySinTime = MaterialFingerprint._Round(material.GetVector(ShaderProps._RotateOnYAxisBySinTime), 100, used._RotateOnYAxisBySinTime);
		this._USE_TEX_ARRAY_ATLAS = MaterialFingerprint._Round(material.GetFloat(ShaderProps._USE_TEX_ARRAY_ATLAS), 100, used._USE_TEX_ARRAY_ATLAS);
		this._BaseMap_Atlas = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._BaseMap_Atlas, used._BaseMap_Atlas);
		this._BaseMap_AtlasSlice = MaterialFingerprint._Round(material.GetFloat(ShaderProps._BaseMap_AtlasSlice), 100, used._BaseMap_AtlasSlice);
		this._BaseMap_AtlasSliceSource = MaterialFingerprint._Round(material.GetFloat(ShaderProps._BaseMap_AtlasSliceSource), 100, used._BaseMap_AtlasSliceSource);
		this._EmissionMap_Atlas = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._EmissionMap_Atlas, used._EmissionMap_Atlas);
		this._EmissionMap_AtlasSlice = MaterialFingerprint._Round(material.GetFloat(ShaderProps._EmissionMap_AtlasSlice), 100, used._EmissionMap_AtlasSlice);
		this._DeformMap_Atlas = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._DeformMap_Atlas, used._DeformMap_Atlas);
		this._DeformMap_AtlasSlice = MaterialFingerprint._Round(material.GetFloat(ShaderProps._DeformMap_AtlasSlice), 100, used._DeformMap_AtlasSlice);
		this._WeatherMap_Atlas = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._WeatherMap_Atlas, used._WeatherMap_Atlas);
		this._WeatherMap_AtlasSlice = MaterialFingerprint._Round(material.GetFloat(ShaderProps._WeatherMap_AtlasSlice), 100, used._WeatherMap_AtlasSlice);
		this._DEBUG_PAWN_DATA = MaterialFingerprint._Round(material.GetFloat(ShaderProps._DEBUG_PAWN_DATA), 100, used._DEBUG_PAWN_DATA);
		this._SrcBlend = MaterialFingerprint._Round(material.GetFloat(ShaderProps._SrcBlend), 100, used._SrcBlend);
		this._DstBlend = MaterialFingerprint._Round(material.GetFloat(ShaderProps._DstBlend), 100, used._DstBlend);
		this._SrcBlendAlpha = MaterialFingerprint._Round(material.GetFloat(ShaderProps._SrcBlendAlpha), 100, used._SrcBlendAlpha);
		this._DstBlendAlpha = MaterialFingerprint._Round(material.GetFloat(ShaderProps._DstBlendAlpha), 100, used._DstBlendAlpha);
		this._ZWrite = MaterialFingerprint._Round(material.GetFloat(ShaderProps._ZWrite), 100, used._ZWrite);
		this._AlphaToMask = MaterialFingerprint._Round(material.GetFloat(ShaderProps._AlphaToMask), 100, used._AlphaToMask);
		this._Color = MaterialFingerprint._Round(material.GetColor(ShaderProps._Color), 100, used._Color);
		this._Surface = MaterialFingerprint._Round(material.GetFloat(ShaderProps._Surface), 100, used._Surface);
		this._Metallic = MaterialFingerprint._Round(material.GetFloat(ShaderProps._Metallic), 100, used._Metallic);
		this._SpecColor = MaterialFingerprint._Round(material.GetColor(ShaderProps._SpecColor), 100, used._SpecColor);
		this._DayNightLightmapArray = MaterialFingerprint._GetTexPropGuid(material, ShaderProps._DayNightLightmapArray, used._DayNightLightmapArray);
		this._DayNightLightmapArray_ST = MaterialFingerprint._Round(material.GetVector(ShaderProps._DayNightLightmapArray_ST), 100, used._DayNightLightmapArray_ST);
		this._DayNightLightmapArray_AtlasSlice = MaterialFingerprint._Round(material.GetFloat(ShaderProps._DayNightLightmapArray_AtlasSlice), 100, used._DayNightLightmapArray_AtlasSlice);
		this.isValid = true;
	}

	// Token: 0x060012CA RID: 4810 RVA: 0x00063A10 File Offset: 0x00061C10
	private static int4 _Round(Color c, int mul, int usedCount)
	{
		if (usedCount <= 0)
		{
			return int4.zero;
		}
		return new int4(Mathf.RoundToInt(c.r * (float)mul), Mathf.RoundToInt(c.g * (float)mul), Mathf.RoundToInt(c.b * (float)mul), Mathf.RoundToInt(c.a * (float)mul));
	}

	// Token: 0x060012CB RID: 4811 RVA: 0x00063A64 File Offset: 0x00061C64
	private static int4 _Round(Vector4 v, int mul, int usedCount)
	{
		if (usedCount <= 0)
		{
			return int4.zero;
		}
		return new int4(Mathf.RoundToInt(v.x * (float)mul), Mathf.RoundToInt(v.y * (float)mul), Mathf.RoundToInt(v.z * (float)mul), Mathf.RoundToInt(v.w * (float)mul));
	}

	// Token: 0x060012CC RID: 4812 RVA: 0x00063AB8 File Offset: 0x00061CB8
	private static int _Round(float f, int mul, int usedCount)
	{
		if (usedCount <= 0)
		{
			return 0;
		}
		return Mathf.RoundToInt(f * (float)mul);
	}

	// Token: 0x060012CD RID: 4813 RVA: 0x00063ACC File Offset: 0x00061CCC
	private static TexFormatInfo _GetTexFormatInfo(Material mat, string texPropName, int usedCount)
	{
		if (usedCount > 0)
		{
			Texture2D texture2D = mat.GetTexture(texPropName) as Texture2D;
			if (texture2D != null)
			{
				return new TexFormatInfo(texture2D);
			}
		}
		return default(TexFormatInfo);
	}

	// Token: 0x060012CE RID: 4814 RVA: 0x00063B03 File Offset: 0x00061D03
	private static string _GetTexPropGuid(Material mat, int texPropId, int usedCount)
	{
		return string.Empty;
	}

	// Token: 0x060012CF RID: 4815 RVA: 0x00063B10 File Offset: 0x00061D10
	[MethodImpl(256)]
	public static GTShaderTransparencyMode GetMatTransparencyMode(Material mat)
	{
		return (GTShaderTransparencyMode)mat.GetInteger(ShaderProps._TransparencyMode);
	}

	// Token: 0x0400175B RID: 5979
	public GTShaderTransparencyMode _TransparencyMode;

	// Token: 0x0400175C RID: 5980
	public int _Cutoff;

	// Token: 0x0400175D RID: 5981
	public int _ColorSource;

	// Token: 0x0400175E RID: 5982
	public int4 _BaseColor;

	// Token: 0x0400175F RID: 5983
	public int4 _GChannelColor;

	// Token: 0x04001760 RID: 5984
	public int4 _BChannelColor;

	// Token: 0x04001761 RID: 5985
	public int4 _AChannelColor;

	// Token: 0x04001762 RID: 5986
	public int _TexMipBias;

	// Token: 0x04001763 RID: 5987
	public string _BaseMap;

	// Token: 0x04001764 RID: 5988
	public int4 _BaseMap_ST;

	// Token: 0x04001765 RID: 5989
	public int4 _BaseMap_WH;

	// Token: 0x04001766 RID: 5990
	public int _TexelSnapToggle;

	// Token: 0x04001767 RID: 5991
	public int _TexelSnap_Factor;

	// Token: 0x04001768 RID: 5992
	public int _UVSource;

	// Token: 0x04001769 RID: 5993
	public int _AlphaDetailToggle;

	// Token: 0x0400176A RID: 5994
	public int4 _AlphaDetail_ST;

	// Token: 0x0400176B RID: 5995
	public int _AlphaDetail_Opacity;

	// Token: 0x0400176C RID: 5996
	public int _AlphaDetail_WorldSpace;

	// Token: 0x0400176D RID: 5997
	public int _MaskMapToggle;

	// Token: 0x0400176E RID: 5998
	public string _MaskMap;

	// Token: 0x0400176F RID: 5999
	public int4 _MaskMap_ST;

	// Token: 0x04001770 RID: 6000
	public int4 _MaskMap_WH;

	// Token: 0x04001771 RID: 6001
	public int _LavaLampToggle;

	// Token: 0x04001772 RID: 6002
	public int _GradientMapToggle;

	// Token: 0x04001773 RID: 6003
	public string _GradientMap;

	// Token: 0x04001774 RID: 6004
	public int _DoTextureRotation;

	// Token: 0x04001775 RID: 6005
	public int _RotateAngle;

	// Token: 0x04001776 RID: 6006
	public int _RotateAnim;

	// Token: 0x04001777 RID: 6007
	public int _UseWaveWarp;

	// Token: 0x04001778 RID: 6008
	public int _WaveAmplitude;

	// Token: 0x04001779 RID: 6009
	public int _WaveFrequency;

	// Token: 0x0400177A RID: 6010
	public int _WaveScale;

	// Token: 0x0400177B RID: 6011
	public int _WaveTimeScale;

	// Token: 0x0400177C RID: 6012
	public int _UseWeatherMap;

	// Token: 0x0400177D RID: 6013
	public string _WeatherMap;

	// Token: 0x0400177E RID: 6014
	public int _WeatherMapDissolveEdgeSize;

	// Token: 0x0400177F RID: 6015
	public int _ReflectToggle;

	// Token: 0x04001780 RID: 6016
	public int _ReflectBoxProjectToggle;

	// Token: 0x04001781 RID: 6017
	public int4 _ReflectBoxCubePos;

	// Token: 0x04001782 RID: 6018
	public int4 _ReflectBoxSize;

	// Token: 0x04001783 RID: 6019
	public int4 _ReflectBoxRotation;

	// Token: 0x04001784 RID: 6020
	public int _ReflectMatcapToggle;

	// Token: 0x04001785 RID: 6021
	public int _ReflectMatcapPerspToggle;

	// Token: 0x04001786 RID: 6022
	public int _ReflectNormalToggle;

	// Token: 0x04001787 RID: 6023
	public string _ReflectTex;

	// Token: 0x04001788 RID: 6024
	public string _ReflectNormalTex;

	// Token: 0x04001789 RID: 6025
	public int _ReflectAlbedoTint;

	// Token: 0x0400178A RID: 6026
	public int4 _ReflectTint;

	// Token: 0x0400178B RID: 6027
	public int _ReflectOpacity;

	// Token: 0x0400178C RID: 6028
	public int _ReflectExposure;

	// Token: 0x0400178D RID: 6029
	public int4 _ReflectOffset;

	// Token: 0x0400178E RID: 6030
	public int4 _ReflectScale;

	// Token: 0x0400178F RID: 6031
	public int _ReflectRotate;

	// Token: 0x04001790 RID: 6032
	public int _HalfLambertToggle;

	// Token: 0x04001791 RID: 6033
	public int _ParallaxPlanarToggle;

	// Token: 0x04001792 RID: 6034
	public int _ParallaxToggle;

	// Token: 0x04001793 RID: 6035
	public int _ParallaxAAToggle;

	// Token: 0x04001794 RID: 6036
	public int _ParallaxAABias;

	// Token: 0x04001795 RID: 6037
	public string _DepthMap;

	// Token: 0x04001796 RID: 6038
	public int _ParallaxAmplitude;

	// Token: 0x04001797 RID: 6039
	public int4 _ParallaxSamplesMinMax;

	// Token: 0x04001798 RID: 6040
	public int _UvShiftToggle;

	// Token: 0x04001799 RID: 6041
	public int4 _UvShiftSteps;

	// Token: 0x0400179A RID: 6042
	public int4 _UvShiftRate;

	// Token: 0x0400179B RID: 6043
	public int4 _UvShiftOffset;

	// Token: 0x0400179C RID: 6044
	public int _UseGridEffect;

	// Token: 0x0400179D RID: 6045
	public int _UseCrystalEffect;

	// Token: 0x0400179E RID: 6046
	public int _CrystalPower;

	// Token: 0x0400179F RID: 6047
	public int4 _CrystalRimColor;

	// Token: 0x040017A0 RID: 6048
	public int _LiquidVolume;

	// Token: 0x040017A1 RID: 6049
	public int _LiquidFill;

	// Token: 0x040017A2 RID: 6050
	public int4 _LiquidFillNormal;

	// Token: 0x040017A3 RID: 6051
	public int4 _LiquidSurfaceColor;

	// Token: 0x040017A4 RID: 6052
	public int _LiquidSwayX;

	// Token: 0x040017A5 RID: 6053
	public int _LiquidSwayY;

	// Token: 0x040017A6 RID: 6054
	public int _LiquidContainer;

	// Token: 0x040017A7 RID: 6055
	public int4 _LiquidPlanePosition;

	// Token: 0x040017A8 RID: 6056
	public int4 _LiquidPlaneNormal;

	// Token: 0x040017A9 RID: 6057
	public int _VertexFlapToggle;

	// Token: 0x040017AA RID: 6058
	public int4 _VertexFlapAxis;

	// Token: 0x040017AB RID: 6059
	public int4 _VertexFlapDegreesMinMax;

	// Token: 0x040017AC RID: 6060
	public int _VertexFlapSpeed;

	// Token: 0x040017AD RID: 6061
	public int _VertexFlapPhaseOffset;

	// Token: 0x040017AE RID: 6062
	public int _VertexWaveToggle;

	// Token: 0x040017AF RID: 6063
	public int _VertexWaveDebug;

	// Token: 0x040017B0 RID: 6064
	public int4 _VertexWaveEnd;

	// Token: 0x040017B1 RID: 6065
	public int4 _VertexWaveParams;

	// Token: 0x040017B2 RID: 6066
	public int4 _VertexWaveFalloff;

	// Token: 0x040017B3 RID: 6067
	public int4 _VertexWaveSphereMask;

	// Token: 0x040017B4 RID: 6068
	public int _VertexWavePhaseOffset;

	// Token: 0x040017B5 RID: 6069
	public int4 _VertexWaveAxes;

	// Token: 0x040017B6 RID: 6070
	public int _VertexRotateToggle;

	// Token: 0x040017B7 RID: 6071
	public int4 _VertexRotateAngles;

	// Token: 0x040017B8 RID: 6072
	public int _VertexRotateAnim;

	// Token: 0x040017B9 RID: 6073
	public int _VertexLightToggle;

	// Token: 0x040017BA RID: 6074
	public int _InnerGlowOn;

	// Token: 0x040017BB RID: 6075
	public int4 _InnerGlowColor;

	// Token: 0x040017BC RID: 6076
	public int4 _InnerGlowParams;

	// Token: 0x040017BD RID: 6077
	public int _InnerGlowTap;

	// Token: 0x040017BE RID: 6078
	public int _InnerGlowSine;

	// Token: 0x040017BF RID: 6079
	public int _InnerGlowSinePeriod;

	// Token: 0x040017C0 RID: 6080
	public int _InnerGlowSinePhaseShift;

	// Token: 0x040017C1 RID: 6081
	public int _StealthEffectOn;

	// Token: 0x040017C2 RID: 6082
	public int _UseEyeTracking;

	// Token: 0x040017C3 RID: 6083
	public int4 _EyeTileOffsetUV;

	// Token: 0x040017C4 RID: 6084
	public int _EyeOverrideUV;

	// Token: 0x040017C5 RID: 6085
	public int4 _EyeOverrideUVTransform;

	// Token: 0x040017C6 RID: 6086
	public int _UseMouthFlap;

	// Token: 0x040017C7 RID: 6087
	public string _MouthMap;

	// Token: 0x040017C8 RID: 6088
	public int4 _MouthMap_ST;

	// Token: 0x040017C9 RID: 6089
	public int _UseVertexColor;

	// Token: 0x040017CA RID: 6090
	public int _WaterEffect;

	// Token: 0x040017CB RID: 6091
	public int _HeightBasedWaterEffect;

	// Token: 0x040017CC RID: 6092
	public int _WaterCaustics;

	// Token: 0x040017CD RID: 6093
	public int _UseDayNightLightmap;

	// Token: 0x040017CE RID: 6094
	public int _UseSpecular;

	// Token: 0x040017CF RID: 6095
	public int _UseSpecularAlphaChannel;

	// Token: 0x040017D0 RID: 6096
	public int _Smoothness;

	// Token: 0x040017D1 RID: 6097
	public int _UseSpecHighlight;

	// Token: 0x040017D2 RID: 6098
	public int4 _SpecularDir;

	// Token: 0x040017D3 RID: 6099
	public int4 _SpecularPowerIntensity;

	// Token: 0x040017D4 RID: 6100
	public int4 _SpecularColor;

	// Token: 0x040017D5 RID: 6101
	public int _SpecularUseDiffuseColor;

	// Token: 0x040017D6 RID: 6102
	public int _EmissionToggle;

	// Token: 0x040017D7 RID: 6103
	public int4 _EmissionColor;

	// Token: 0x040017D8 RID: 6104
	public string _EmissionMap;

	// Token: 0x040017D9 RID: 6105
	public int _EmissionMaskByBaseMapAlpha;

	// Token: 0x040017DA RID: 6106
	public int4 _EmissionUVScrollSpeed;

	// Token: 0x040017DB RID: 6107
	public int _EmissionDissolveProgress;

	// Token: 0x040017DC RID: 6108
	public int4 _EmissionDissolveAnimation;

	// Token: 0x040017DD RID: 6109
	public int _EmissionDissolveEdgeSize;

	// Token: 0x040017DE RID: 6110
	public int _EmissionIntensityInDynamic;

	// Token: 0x040017DF RID: 6111
	public int _EmissionUseUVWaveWarp;

	// Token: 0x040017E0 RID: 6112
	public int _GreyZoneException;

	// Token: 0x040017E1 RID: 6113
	public int _Cull;

	// Token: 0x040017E2 RID: 6114
	public int _StencilReference;

	// Token: 0x040017E3 RID: 6115
	public int _StencilComparison;

	// Token: 0x040017E4 RID: 6116
	public int _StencilPassFront;

	// Token: 0x040017E5 RID: 6117
	public int _USE_DEFORM_MAP;

	// Token: 0x040017E6 RID: 6118
	public string _DeformMap;

	// Token: 0x040017E7 RID: 6119
	public int _DeformMapIntensity;

	// Token: 0x040017E8 RID: 6120
	public int _DeformMapMaskByVertColorRAmount;

	// Token: 0x040017E9 RID: 6121
	public int4 _DeformMapScrollSpeed;

	// Token: 0x040017EA RID: 6122
	public int4 _DeformMapUV0Influence;

	// Token: 0x040017EB RID: 6123
	public int4 _DeformMapObjectSpaceOffsetsU;

	// Token: 0x040017EC RID: 6124
	public int4 _DeformMapObjectSpaceOffsetsV;

	// Token: 0x040017ED RID: 6125
	public int4 _DeformMapWorldSpaceOffsetsU;

	// Token: 0x040017EE RID: 6126
	public int4 _DeformMapWorldSpaceOffsetsV;

	// Token: 0x040017EF RID: 6127
	public int4 _RotateOnYAxisBySinTime;

	// Token: 0x040017F0 RID: 6128
	public int _USE_TEX_ARRAY_ATLAS;

	// Token: 0x040017F1 RID: 6129
	public string _BaseMap_Atlas;

	// Token: 0x040017F2 RID: 6130
	public int _BaseMap_AtlasSlice;

	// Token: 0x040017F3 RID: 6131
	public int _BaseMap_AtlasSliceSource;

	// Token: 0x040017F4 RID: 6132
	public string _EmissionMap_Atlas;

	// Token: 0x040017F5 RID: 6133
	public int _EmissionMap_AtlasSlice;

	// Token: 0x040017F6 RID: 6134
	public string _DeformMap_Atlas;

	// Token: 0x040017F7 RID: 6135
	public int _DeformMap_AtlasSlice;

	// Token: 0x040017F8 RID: 6136
	public string _WeatherMap_Atlas;

	// Token: 0x040017F9 RID: 6137
	public int _WeatherMap_AtlasSlice;

	// Token: 0x040017FA RID: 6138
	public int _DEBUG_PAWN_DATA;

	// Token: 0x040017FB RID: 6139
	public int _SrcBlend;

	// Token: 0x040017FC RID: 6140
	public int _DstBlend;

	// Token: 0x040017FD RID: 6141
	public int _SrcBlendAlpha;

	// Token: 0x040017FE RID: 6142
	public int _DstBlendAlpha;

	// Token: 0x040017FF RID: 6143
	public int _ZWrite;

	// Token: 0x04001800 RID: 6144
	public int _AlphaToMask;

	// Token: 0x04001801 RID: 6145
	public int4 _Color;

	// Token: 0x04001802 RID: 6146
	public int _Surface;

	// Token: 0x04001803 RID: 6147
	public int _Metallic;

	// Token: 0x04001804 RID: 6148
	public int4 _SpecColor;

	// Token: 0x04001805 RID: 6149
	public string _DayNightLightmapArray;

	// Token: 0x04001806 RID: 6150
	public int4 _DayNightLightmapArray_ST;

	// Token: 0x04001807 RID: 6151
	public int _DayNightLightmapArray_AtlasSlice;

	// Token: 0x04001808 RID: 6152
	private const bool _k_UNITY_2023_1_OR_NEWER = true;

	// Token: 0x04001809 RID: 6153
	public bool isValid;
}
