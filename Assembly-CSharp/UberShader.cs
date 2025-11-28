using System;
using UnityEngine;

// Token: 0x020008FC RID: 2300
public static class UberShader
{
	// Token: 0x1700056E RID: 1390
	// (get) Token: 0x06003AC8 RID: 15048 RVA: 0x0013652D File Offset: 0x0013472D
	public static Material ReferenceMaterial
	{
		get
		{
			UberShader.InitDependencies();
			return UberShader.kReferenceMaterial;
		}
	}

	// Token: 0x1700056F RID: 1391
	// (get) Token: 0x06003AC9 RID: 15049 RVA: 0x00136539 File Offset: 0x00134739
	public static Shader ReferenceShader
	{
		get
		{
			UberShader.InitDependencies();
			return UberShader.kReferenceShader;
		}
	}

	// Token: 0x17000570 RID: 1392
	// (get) Token: 0x06003ACA RID: 15050 RVA: 0x00136545 File Offset: 0x00134745
	public static Material ReferenceMaterialNonSRP
	{
		get
		{
			UberShader.InitDependencies();
			return UberShader.kReferenceMaterialNonSRP;
		}
	}

	// Token: 0x17000571 RID: 1393
	// (get) Token: 0x06003ACB RID: 15051 RVA: 0x00136551 File Offset: 0x00134751
	public static Shader ReferenceShaderNonSRP
	{
		get
		{
			UberShader.InitDependencies();
			return UberShader.kReferenceShaderNonSRP;
		}
	}

	// Token: 0x17000572 RID: 1394
	// (get) Token: 0x06003ACC RID: 15052 RVA: 0x0013655D File Offset: 0x0013475D
	public static UberShaderProperty[] AllProperties
	{
		get
		{
			UberShader.InitDependencies();
			return UberShader.kProperties;
		}
	}

	// Token: 0x06003ACD RID: 15053 RVA: 0x0013656C File Offset: 0x0013476C
	public static bool IsAnimated(Material m)
	{
		if (m == null)
		{
			return false;
		}
		if ((double)UberShader.UvShiftToggle.GetValue<float>(m) <= 0.5)
		{
			return false;
		}
		Vector2 value = UberShader.UvShiftRate.GetValue<Vector2>(m);
		return value.x > 0f || value.y > 0f;
	}

	// Token: 0x06003ACE RID: 15054 RVA: 0x001365C7 File Offset: 0x001347C7
	private static UberShaderProperty GetProperty(int i)
	{
		UberShader.InitDependencies();
		return UberShader.kProperties[i];
	}

	// Token: 0x06003ACF RID: 15055 RVA: 0x001365C7 File Offset: 0x001347C7
	private static UberShaderProperty GetProperty(int i, string expectedName)
	{
		UberShader.InitDependencies();
		return UberShader.kProperties[i];
	}

	// Token: 0x06003AD0 RID: 15056 RVA: 0x001365D8 File Offset: 0x001347D8
	private static void InitDependencies()
	{
		if (UberShader.gInitialized)
		{
			return;
		}
		UberShader.kReferenceShader = Shader.Find("GorillaTag/UberShader");
		UberShader.kReferenceMaterial = new Material(UberShader.kReferenceShader);
		UberShader.kReferenceShaderNonSRP = Shader.Find("GorillaTag/UberShaderNonSRP");
		UberShader.kReferenceMaterialNonSRP = new Material(UberShader.kReferenceShaderNonSRP);
		UberShader.kProperties = UberShader.EnumerateAllProperties(UberShader.kReferenceShader);
		UberShader.gInitialized = true;
	}

	// Token: 0x06003AD1 RID: 15057 RVA: 0x00136539 File Offset: 0x00134739
	public static Shader GetShader()
	{
		UberShader.InitDependencies();
		return UberShader.kReferenceShader;
	}

	// Token: 0x06003AD2 RID: 15058 RVA: 0x00136640 File Offset: 0x00134840
	private static UberShaderProperty[] EnumerateAllProperties(Shader uberShader)
	{
		int propertyCount = uberShader.GetPropertyCount();
		UberShaderProperty[] array = new UberShaderProperty[propertyCount];
		for (int i = 0; i < propertyCount; i++)
		{
			UberShaderProperty uberShaderProperty = new UberShaderProperty
			{
				index = i,
				flags = uberShader.GetPropertyFlags(i),
				type = uberShader.GetPropertyType(i),
				nameID = uberShader.GetPropertyNameId(i),
				name = uberShader.GetPropertyName(i),
				attributes = uberShader.GetPropertyAttributes(i)
			};
			if (uberShaderProperty.type == 3)
			{
				uberShaderProperty.rangeLimits = uberShader.GetPropertyRangeLimits(uberShaderProperty.index);
			}
			string[] attributes = uberShaderProperty.attributes;
			if (attributes != null && attributes.Length != 0)
			{
				foreach (string text in attributes)
				{
					if (!string.IsNullOrWhiteSpace(text))
					{
						bool flag = text.StartsWith("Toggle(");
						uberShaderProperty.isKeywordToggle = flag;
						if (flag)
						{
							string keyword = text.Split('(', 1)[1].RemoveEnd(")", 2);
							uberShaderProperty.keyword = keyword;
						}
					}
				}
			}
			array[i] = uberShaderProperty;
		}
		return array;
	}

	// Token: 0x04004A2E RID: 18990
	private static Shader kReferenceShader;

	// Token: 0x04004A2F RID: 18991
	private static Material kReferenceMaterial;

	// Token: 0x04004A30 RID: 18992
	private static Shader kReferenceShaderNonSRP;

	// Token: 0x04004A31 RID: 18993
	private static Material kReferenceMaterialNonSRP;

	// Token: 0x04004A32 RID: 18994
	private static UberShaderProperty[] kProperties;

	// Token: 0x04004A33 RID: 18995
	private static bool gInitialized = false;

	// Token: 0x04004A34 RID: 18996
	public static UberShaderProperty TransparencyMode = UberShader.GetProperty(0);

	// Token: 0x04004A35 RID: 18997
	public static UberShaderProperty Cutoff = UberShader.GetProperty(1);

	// Token: 0x04004A36 RID: 18998
	public static UberShaderProperty ColorSource = UberShader.GetProperty(2);

	// Token: 0x04004A37 RID: 18999
	public static UberShaderProperty BaseColor = UberShader.GetProperty(3);

	// Token: 0x04004A38 RID: 19000
	public static UberShaderProperty GChannelColor = UberShader.GetProperty(4);

	// Token: 0x04004A39 RID: 19001
	public static UberShaderProperty BChannelColor = UberShader.GetProperty(5);

	// Token: 0x04004A3A RID: 19002
	public static UberShaderProperty AChannelColor = UberShader.GetProperty(6);

	// Token: 0x04004A3B RID: 19003
	public static UberShaderProperty BaseMap = UberShader.GetProperty(7);

	// Token: 0x04004A3C RID: 19004
	public static UberShaderProperty BaseMap_WH = UberShader.GetProperty(8);

	// Token: 0x04004A3D RID: 19005
	public static UberShaderProperty TexelSnapToggle = UberShader.GetProperty(9);

	// Token: 0x04004A3E RID: 19006
	public static UberShaderProperty TexelSnap_Factor = UberShader.GetProperty(10);

	// Token: 0x04004A3F RID: 19007
	public static UberShaderProperty UVSource = UberShader.GetProperty(11);

	// Token: 0x04004A40 RID: 19008
	public static UberShaderProperty AlphaDetailToggle = UberShader.GetProperty(12);

	// Token: 0x04004A41 RID: 19009
	public static UberShaderProperty AlphaDetail_ST = UberShader.GetProperty(13);

	// Token: 0x04004A42 RID: 19010
	public static UberShaderProperty AlphaDetail_Opacity = UberShader.GetProperty(14);

	// Token: 0x04004A43 RID: 19011
	public static UberShaderProperty AlphaDetail_WorldSpace = UberShader.GetProperty(15);

	// Token: 0x04004A44 RID: 19012
	public static UberShaderProperty MaskMapToggle = UberShader.GetProperty(16);

	// Token: 0x04004A45 RID: 19013
	public static UberShaderProperty MaskMap = UberShader.GetProperty(17);

	// Token: 0x04004A46 RID: 19014
	public static UberShaderProperty MaskMap_WH = UberShader.GetProperty(18);

	// Token: 0x04004A47 RID: 19015
	public static UberShaderProperty LavaLampToggle = UberShader.GetProperty(19);

	// Token: 0x04004A48 RID: 19016
	public static UberShaderProperty GradientMapToggle = UberShader.GetProperty(20);

	// Token: 0x04004A49 RID: 19017
	public static UberShaderProperty GradientMap = UberShader.GetProperty(21);

	// Token: 0x04004A4A RID: 19018
	public static UberShaderProperty DoTextureRotation = UberShader.GetProperty(22);

	// Token: 0x04004A4B RID: 19019
	public static UberShaderProperty RotateAngle = UberShader.GetProperty(23);

	// Token: 0x04004A4C RID: 19020
	public static UberShaderProperty RotateAnim = UberShader.GetProperty(24);

	// Token: 0x04004A4D RID: 19021
	public static UberShaderProperty UseWaveWarp = UberShader.GetProperty(25);

	// Token: 0x04004A4E RID: 19022
	public static UberShaderProperty WaveAmplitude = UberShader.GetProperty(26);

	// Token: 0x04004A4F RID: 19023
	public static UberShaderProperty WaveFrequency = UberShader.GetProperty(27);

	// Token: 0x04004A50 RID: 19024
	public static UberShaderProperty WaveScale = UberShader.GetProperty(28);

	// Token: 0x04004A51 RID: 19025
	public static UberShaderProperty WaveTimeScale = UberShader.GetProperty(29);

	// Token: 0x04004A52 RID: 19026
	public static UberShaderProperty UseWeatherMap = UberShader.GetProperty(30);

	// Token: 0x04004A53 RID: 19027
	public static UberShaderProperty WeatherMap = UberShader.GetProperty(31);

	// Token: 0x04004A54 RID: 19028
	public static UberShaderProperty WeatherMapDissolveEdgeSize = UberShader.GetProperty(32);

	// Token: 0x04004A55 RID: 19029
	public static UberShaderProperty ReflectToggle = UberShader.GetProperty(33);

	// Token: 0x04004A56 RID: 19030
	public static UberShaderProperty ReflectBoxProjectToggle = UberShader.GetProperty(34);

	// Token: 0x04004A57 RID: 19031
	public static UberShaderProperty ReflectBoxCubePos = UberShader.GetProperty(35);

	// Token: 0x04004A58 RID: 19032
	public static UberShaderProperty ReflectBoxSize = UberShader.GetProperty(36);

	// Token: 0x04004A59 RID: 19033
	public static UberShaderProperty ReflectBoxRotation = UberShader.GetProperty(37);

	// Token: 0x04004A5A RID: 19034
	public static UberShaderProperty ReflectMatcapToggle = UberShader.GetProperty(38);

	// Token: 0x04004A5B RID: 19035
	public static UberShaderProperty ReflectMatcapPerspToggle = UberShader.GetProperty(39);

	// Token: 0x04004A5C RID: 19036
	public static UberShaderProperty ReflectNormalToggle = UberShader.GetProperty(40);

	// Token: 0x04004A5D RID: 19037
	public static UberShaderProperty ReflectTex = UberShader.GetProperty(41);

	// Token: 0x04004A5E RID: 19038
	public static UberShaderProperty ReflectNormalTex = UberShader.GetProperty(42);

	// Token: 0x04004A5F RID: 19039
	public static UberShaderProperty ReflectAlbedoTint = UberShader.GetProperty(43);

	// Token: 0x04004A60 RID: 19040
	public static UberShaderProperty ReflectTint = UberShader.GetProperty(44);

	// Token: 0x04004A61 RID: 19041
	public static UberShaderProperty ReflectOpacity = UberShader.GetProperty(45);

	// Token: 0x04004A62 RID: 19042
	public static UberShaderProperty ReflectExposure = UberShader.GetProperty(46);

	// Token: 0x04004A63 RID: 19043
	public static UberShaderProperty ReflectOffset = UberShader.GetProperty(47);

	// Token: 0x04004A64 RID: 19044
	public static UberShaderProperty ReflectScale = UberShader.GetProperty(48);

	// Token: 0x04004A65 RID: 19045
	public static UberShaderProperty ReflectRotate = UberShader.GetProperty(49);

	// Token: 0x04004A66 RID: 19046
	public static UberShaderProperty HalfLambertToggle = UberShader.GetProperty(50);

	// Token: 0x04004A67 RID: 19047
	public static UberShaderProperty ZFightOffset = UberShader.GetProperty(51);

	// Token: 0x04004A68 RID: 19048
	public static UberShaderProperty ParallaxPlanarToggle = UberShader.GetProperty(52);

	// Token: 0x04004A69 RID: 19049
	public static UberShaderProperty ParallaxToggle = UberShader.GetProperty(53);

	// Token: 0x04004A6A RID: 19050
	public static UberShaderProperty ParallaxAAToggle = UberShader.GetProperty(54);

	// Token: 0x04004A6B RID: 19051
	public static UberShaderProperty ParallaxAABias = UberShader.GetProperty(55);

	// Token: 0x04004A6C RID: 19052
	public static UberShaderProperty DepthMap = UberShader.GetProperty(56);

	// Token: 0x04004A6D RID: 19053
	public static UberShaderProperty ParallaxAmplitude = UberShader.GetProperty(57);

	// Token: 0x04004A6E RID: 19054
	public static UberShaderProperty ParallaxSamplesMinMax = UberShader.GetProperty(58);

	// Token: 0x04004A6F RID: 19055
	public static UberShaderProperty UvShiftToggle = UberShader.GetProperty(59);

	// Token: 0x04004A70 RID: 19056
	public static UberShaderProperty UvShiftSteps = UberShader.GetProperty(60);

	// Token: 0x04004A71 RID: 19057
	public static UberShaderProperty UvShiftRate = UberShader.GetProperty(61);

	// Token: 0x04004A72 RID: 19058
	public static UberShaderProperty UvShiftOffset = UberShader.GetProperty(62);

	// Token: 0x04004A73 RID: 19059
	public static UberShaderProperty UseGridEffect = UberShader.GetProperty(63);

	// Token: 0x04004A74 RID: 19060
	public static UberShaderProperty UseCrystalEffect = UberShader.GetProperty(64);

	// Token: 0x04004A75 RID: 19061
	public static UberShaderProperty CrystalPower = UberShader.GetProperty(65);

	// Token: 0x04004A76 RID: 19062
	public static UberShaderProperty CrystalRimColor = UberShader.GetProperty(66);

	// Token: 0x04004A77 RID: 19063
	public static UberShaderProperty LiquidVolume = UberShader.GetProperty(67);

	// Token: 0x04004A78 RID: 19064
	public static UberShaderProperty LiquidFill = UberShader.GetProperty(68);

	// Token: 0x04004A79 RID: 19065
	public static UberShaderProperty LiquidFillNormal = UberShader.GetProperty(69);

	// Token: 0x04004A7A RID: 19066
	public static UberShaderProperty LiquidSurfaceColor = UberShader.GetProperty(70);

	// Token: 0x04004A7B RID: 19067
	public static UberShaderProperty LiquidSwayX = UberShader.GetProperty(71);

	// Token: 0x04004A7C RID: 19068
	public static UberShaderProperty LiquidSwayY = UberShader.GetProperty(72);

	// Token: 0x04004A7D RID: 19069
	public static UberShaderProperty LiquidContainer = UberShader.GetProperty(73);

	// Token: 0x04004A7E RID: 19070
	public static UberShaderProperty LiquidPlanePosition = UberShader.GetProperty(74);

	// Token: 0x04004A7F RID: 19071
	public static UberShaderProperty LiquidPlaneNormal = UberShader.GetProperty(75);

	// Token: 0x04004A80 RID: 19072
	public static UberShaderProperty VertexFlapToggle = UberShader.GetProperty(76);

	// Token: 0x04004A81 RID: 19073
	public static UberShaderProperty VertexFlapAxis = UberShader.GetProperty(77);

	// Token: 0x04004A82 RID: 19074
	public static UberShaderProperty VertexFlapDegreesMinMax = UberShader.GetProperty(78);

	// Token: 0x04004A83 RID: 19075
	public static UberShaderProperty VertexFlapSpeed = UberShader.GetProperty(79);

	// Token: 0x04004A84 RID: 19076
	public static UberShaderProperty VertexFlapPhaseOffset = UberShader.GetProperty(80);

	// Token: 0x04004A85 RID: 19077
	public static UberShaderProperty VertexWaveToggle = UberShader.GetProperty(81);

	// Token: 0x04004A86 RID: 19078
	public static UberShaderProperty VertexWaveDebug = UberShader.GetProperty(82);

	// Token: 0x04004A87 RID: 19079
	public static UberShaderProperty VertexWaveEnd = UberShader.GetProperty(83);

	// Token: 0x04004A88 RID: 19080
	public static UberShaderProperty VertexWaveParams = UberShader.GetProperty(84);

	// Token: 0x04004A89 RID: 19081
	public static UberShaderProperty VertexWaveFalloff = UberShader.GetProperty(85);

	// Token: 0x04004A8A RID: 19082
	public static UberShaderProperty VertexWaveSphereMask = UberShader.GetProperty(86);

	// Token: 0x04004A8B RID: 19083
	public static UberShaderProperty VertexWavePhaseOffset = UberShader.GetProperty(87);

	// Token: 0x04004A8C RID: 19084
	public static UberShaderProperty VertexWaveAxes = UberShader.GetProperty(88);

	// Token: 0x04004A8D RID: 19085
	public static UberShaderProperty VertexRotateToggle = UberShader.GetProperty(89);

	// Token: 0x04004A8E RID: 19086
	public static UberShaderProperty VertexRotateAngles = UberShader.GetProperty(90);

	// Token: 0x04004A8F RID: 19087
	public static UberShaderProperty VertexRotateAnim = UberShader.GetProperty(91);

	// Token: 0x04004A90 RID: 19088
	public static UberShaderProperty VertexLightToggle = UberShader.GetProperty(92);

	// Token: 0x04004A91 RID: 19089
	public static UberShaderProperty InnerGlowOn = UberShader.GetProperty(93);

	// Token: 0x04004A92 RID: 19090
	public static UberShaderProperty InnerGlowColor = UberShader.GetProperty(94);

	// Token: 0x04004A93 RID: 19091
	public static UberShaderProperty InnerGlowParams = UberShader.GetProperty(95);

	// Token: 0x04004A94 RID: 19092
	public static UberShaderProperty InnerGlowTap = UberShader.GetProperty(96);

	// Token: 0x04004A95 RID: 19093
	public static UberShaderProperty InnerGlowSine = UberShader.GetProperty(97);

	// Token: 0x04004A96 RID: 19094
	public static UberShaderProperty InnerGlowSinePeriod = UberShader.GetProperty(98);

	// Token: 0x04004A97 RID: 19095
	public static UberShaderProperty InnerGlowSinePhaseShift = UberShader.GetProperty(99);

	// Token: 0x04004A98 RID: 19096
	public static UberShaderProperty StealthEffectOn = UberShader.GetProperty(100);

	// Token: 0x04004A99 RID: 19097
	public static UberShaderProperty UseEyeTracking = UberShader.GetProperty(101);

	// Token: 0x04004A9A RID: 19098
	public static UberShaderProperty EyeTileOffsetUV = UberShader.GetProperty(102);

	// Token: 0x04004A9B RID: 19099
	public static UberShaderProperty EyeOverrideUV = UberShader.GetProperty(103);

	// Token: 0x04004A9C RID: 19100
	public static UberShaderProperty EyeOverrideUVTransform = UberShader.GetProperty(104);

	// Token: 0x04004A9D RID: 19101
	public static UberShaderProperty UseMouthFlap = UberShader.GetProperty(105);

	// Token: 0x04004A9E RID: 19102
	public static UberShaderProperty MouthMap = UberShader.GetProperty(106);

	// Token: 0x04004A9F RID: 19103
	public static UberShaderProperty MouthMap_Atlas = UberShader.GetProperty(107);

	// Token: 0x04004AA0 RID: 19104
	public static UberShaderProperty MouthMap_AtlasSlice = UberShader.GetProperty(108);

	// Token: 0x04004AA1 RID: 19105
	public static UberShaderProperty UseVertexColor = UberShader.GetProperty(109);

	// Token: 0x04004AA2 RID: 19106
	public static UberShaderProperty WaterEffect = UberShader.GetProperty(110);

	// Token: 0x04004AA3 RID: 19107
	public static UberShaderProperty HeightBasedWaterEffect = UberShader.GetProperty(111);

	// Token: 0x04004AA4 RID: 19108
	public static UberShaderProperty UseDayNightLightmap = UberShader.GetProperty(112);

	// Token: 0x04004AA5 RID: 19109
	public static UberShaderProperty UseSpecular = UberShader.GetProperty(113);

	// Token: 0x04004AA6 RID: 19110
	public static UberShaderProperty UseSpecularAlphaChannel = UberShader.GetProperty(114);

	// Token: 0x04004AA7 RID: 19111
	public static UberShaderProperty Smoothness = UberShader.GetProperty(115);

	// Token: 0x04004AA8 RID: 19112
	public static UberShaderProperty UseSpecHighlight = UberShader.GetProperty(116);

	// Token: 0x04004AA9 RID: 19113
	public static UberShaderProperty SpecularDir = UberShader.GetProperty(117);

	// Token: 0x04004AAA RID: 19114
	public static UberShaderProperty SpecularPowerIntensity = UberShader.GetProperty(118);

	// Token: 0x04004AAB RID: 19115
	public static UberShaderProperty SpecularColor = UberShader.GetProperty(119);

	// Token: 0x04004AAC RID: 19116
	public static UberShaderProperty SpecularUseDiffuseColor = UberShader.GetProperty(120);

	// Token: 0x04004AAD RID: 19117
	public static UberShaderProperty EmissionToggle = UberShader.GetProperty(121);

	// Token: 0x04004AAE RID: 19118
	public static UberShaderProperty EmissionColor = UberShader.GetProperty(122);

	// Token: 0x04004AAF RID: 19119
	public static UberShaderProperty EmissionMap = UberShader.GetProperty(123);

	// Token: 0x04004AB0 RID: 19120
	public static UberShaderProperty EmissionMaskByBaseMapAlpha = UberShader.GetProperty(124);

	// Token: 0x04004AB1 RID: 19121
	public static UberShaderProperty EmissionUVScrollSpeed = UberShader.GetProperty(125);

	// Token: 0x04004AB2 RID: 19122
	public static UberShaderProperty EmissionDissolveProgress = UberShader.GetProperty(126);

	// Token: 0x04004AB3 RID: 19123
	public static UberShaderProperty EmissionDissolveAnimation = UberShader.GetProperty(127);

	// Token: 0x04004AB4 RID: 19124
	public static UberShaderProperty EmissionDissolveEdgeSize = UberShader.GetProperty(128);

	// Token: 0x04004AB5 RID: 19125
	public static UberShaderProperty EmissionUseUVWaveWarp = UberShader.GetProperty(129);

	// Token: 0x04004AB6 RID: 19126
	public static UberShaderProperty GreyZoneException = UberShader.GetProperty(130);

	// Token: 0x04004AB7 RID: 19127
	public static UberShaderProperty Cull = UberShader.GetProperty(131);

	// Token: 0x04004AB8 RID: 19128
	public static UberShaderProperty StencilReference = UberShader.GetProperty(132);

	// Token: 0x04004AB9 RID: 19129
	public static UberShaderProperty StencilComparison = UberShader.GetProperty(133);

	// Token: 0x04004ABA RID: 19130
	public static UberShaderProperty StencilPassFront = UberShader.GetProperty(134);

	// Token: 0x04004ABB RID: 19131
	public static UberShaderProperty USE_DEFORM_MAP = UberShader.GetProperty(135);

	// Token: 0x04004ABC RID: 19132
	public static UberShaderProperty DeformMap = UberShader.GetProperty(136);

	// Token: 0x04004ABD RID: 19133
	public static UberShaderProperty DeformMapIntensity = UberShader.GetProperty(137);

	// Token: 0x04004ABE RID: 19134
	public static UberShaderProperty DeformMapMaskByVertColorRAmount = UberShader.GetProperty(138);

	// Token: 0x04004ABF RID: 19135
	public static UberShaderProperty DeformMapScrollSpeed = UberShader.GetProperty(139);

	// Token: 0x04004AC0 RID: 19136
	public static UberShaderProperty DeformMapUV0Influence = UberShader.GetProperty(140);

	// Token: 0x04004AC1 RID: 19137
	public static UberShaderProperty DeformMapObjectSpaceOffsetsU = UberShader.GetProperty(141);

	// Token: 0x04004AC2 RID: 19138
	public static UberShaderProperty DeformMapObjectSpaceOffsetsV = UberShader.GetProperty(142);

	// Token: 0x04004AC3 RID: 19139
	public static UberShaderProperty DeformMapWorldSpaceOffsetsU = UberShader.GetProperty(143);

	// Token: 0x04004AC4 RID: 19140
	public static UberShaderProperty DeformMapWorldSpaceOffsetsV = UberShader.GetProperty(144);

	// Token: 0x04004AC5 RID: 19141
	public static UberShaderProperty RotateOnYAxisBySinTime = UberShader.GetProperty(145);

	// Token: 0x04004AC6 RID: 19142
	public static UberShaderProperty USE_TEX_ARRAY_ATLAS = UberShader.GetProperty(146);

	// Token: 0x04004AC7 RID: 19143
	public static UberShaderProperty BaseMap_Atlas = UberShader.GetProperty(147);

	// Token: 0x04004AC8 RID: 19144
	public static UberShaderProperty BaseMap_AtlasSlice = UberShader.GetProperty(148);

	// Token: 0x04004AC9 RID: 19145
	public static UberShaderProperty EmissionMap_Atlas = UberShader.GetProperty(149);

	// Token: 0x04004ACA RID: 19146
	public static UberShaderProperty EmissionMap_AtlasSlice = UberShader.GetProperty(150);

	// Token: 0x04004ACB RID: 19147
	public static UberShaderProperty DeformMap_Atlas = UberShader.GetProperty(151);

	// Token: 0x04004ACC RID: 19148
	public static UberShaderProperty DeformMap_AtlasSlice = UberShader.GetProperty(152);

	// Token: 0x04004ACD RID: 19149
	public static UberShaderProperty DEBUG_PAWN_DATA = UberShader.GetProperty(153);

	// Token: 0x04004ACE RID: 19150
	public static UberShaderProperty SrcBlend = UberShader.GetProperty(154);

	// Token: 0x04004ACF RID: 19151
	public static UberShaderProperty DstBlend = UberShader.GetProperty(155);

	// Token: 0x04004AD0 RID: 19152
	public static UberShaderProperty SrcBlendAlpha = UberShader.GetProperty(156);

	// Token: 0x04004AD1 RID: 19153
	public static UberShaderProperty DstBlendAlpha = UberShader.GetProperty(157);

	// Token: 0x04004AD2 RID: 19154
	public static UberShaderProperty ZWrite = UberShader.GetProperty(158);

	// Token: 0x04004AD3 RID: 19155
	public static UberShaderProperty AlphaToMask = UberShader.GetProperty(159);

	// Token: 0x04004AD4 RID: 19156
	public static UberShaderProperty Color = UberShader.GetProperty(160);

	// Token: 0x04004AD5 RID: 19157
	public static UberShaderProperty Surface = UberShader.GetProperty(161);

	// Token: 0x04004AD6 RID: 19158
	public static UberShaderProperty Metallic = UberShader.GetProperty(162);

	// Token: 0x04004AD7 RID: 19159
	public static UberShaderProperty SpecColor = UberShader.GetProperty(163);

	// Token: 0x04004AD8 RID: 19160
	public static UberShaderProperty DayNightLightmapArray = UberShader.GetProperty(164);

	// Token: 0x04004AD9 RID: 19161
	public static UberShaderProperty DayNightLightmapArray_AtlasSlice = UberShader.GetProperty(165);

	// Token: 0x04004ADA RID: 19162
	public static UberShaderProperty SingleLightmap = UberShader.GetProperty(166);
}
