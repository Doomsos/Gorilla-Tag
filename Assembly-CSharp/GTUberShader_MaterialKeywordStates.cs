using System;
using UnityEngine;

// Token: 0x02000CE6 RID: 3302
public struct GTUberShader_MaterialKeywordStates
{
	// Token: 0x06005056 RID: 20566 RVA: 0x0019CD98 File Offset: 0x0019AF98
	public GTUberShader_MaterialKeywordStates(Material mat)
	{
		this.material = mat;
		this.STEREO_INSTANCING_ON = mat.IsKeywordEnabled("STEREO_INSTANCING_ON");
		this.UNITY_SINGLE_PASS_STEREO = mat.IsKeywordEnabled("UNITY_SINGLE_PASS_STEREO");
		this.STEREO_MULTIVIEW_ON = mat.IsKeywordEnabled("STEREO_MULTIVIEW_ON");
		this.STEREO_CUBEMAP_RENDER_ON = mat.IsKeywordEnabled("STEREO_CUBEMAP_RENDER_ON");
		this._GLOBAL_ZONE_LIQUID_TYPE__WATER = mat.IsKeywordEnabled("_GLOBAL_ZONE_LIQUID_TYPE__WATER");
		this._GLOBAL_ZONE_LIQUID_TYPE__LAVA = mat.IsKeywordEnabled("_GLOBAL_ZONE_LIQUID_TYPE__LAVA");
		this._ZONE_LIQUID_SHAPE__CYLINDER = mat.IsKeywordEnabled("_ZONE_LIQUID_SHAPE__CYLINDER");
		this._ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX = mat.IsKeywordEnabled("_ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX");
		this._USE_TEXTURE = mat.IsKeywordEnabled("_USE_TEXTURE");
		this.USE_TEXTURE__AS_MASK = mat.IsKeywordEnabled("USE_TEXTURE__AS_MASK");
		this._UV_SOURCE__UV0 = mat.IsKeywordEnabled("_UV_SOURCE__UV0");
		this._UV_SOURCE__WORLD_PLANAR_Y = mat.IsKeywordEnabled("_UV_SOURCE__WORLD_PLANAR_Y");
		this._USE_VERTEX_COLOR = mat.IsKeywordEnabled("_USE_VERTEX_COLOR");
		this._USE_WEATHER_MAP = mat.IsKeywordEnabled("_USE_WEATHER_MAP");
		this._ALPHA_DETAIL_MAP = mat.IsKeywordEnabled("_ALPHA_DETAIL_MAP");
		this._HALF_LAMBERT_TERM = mat.IsKeywordEnabled("_HALF_LAMBERT_TERM");
		this._WATER_EFFECT = mat.IsKeywordEnabled("_WATER_EFFECT");
		this._HEIGHT_BASED_WATER_EFFECT = mat.IsKeywordEnabled("_HEIGHT_BASED_WATER_EFFECT");
		this._WATER_CAUSTICS = mat.IsKeywordEnabled("_WATER_CAUSTICS");
		this._ALPHATEST_ON = mat.IsKeywordEnabled("_ALPHATEST_ON");
		this._MAINTEX_ROTATE = mat.IsKeywordEnabled("_MAINTEX_ROTATE");
		this._UV_WAVE_WARP = mat.IsKeywordEnabled("_UV_WAVE_WARP");
		this._LIQUID_VOLUME = mat.IsKeywordEnabled("_LIQUID_VOLUME");
		this._LIQUID_CONTAINER = mat.IsKeywordEnabled("_LIQUID_CONTAINER");
		this._GT_RIM_LIGHT = mat.IsKeywordEnabled("_GT_RIM_LIGHT");
		this._GT_RIM_LIGHT_FLAT = mat.IsKeywordEnabled("_GT_RIM_LIGHT_FLAT");
		this._GT_RIM_LIGHT_USE_ALPHA = mat.IsKeywordEnabled("_GT_RIM_LIGHT_USE_ALPHA");
		this._SPECULAR_HIGHLIGHT = mat.IsKeywordEnabled("_SPECULAR_HIGHLIGHT");
		this._EMISSION = mat.IsKeywordEnabled("_EMISSION");
		this._EMISSION_USE_UV_WAVE_WARP = mat.IsKeywordEnabled("_EMISSION_USE_UV_WAVE_WARP");
		this._USE_DEFORM_MAP = mat.IsKeywordEnabled("_USE_DEFORM_MAP");
		this._USE_DAY_NIGHT_LIGHTMAP = mat.IsKeywordEnabled("_USE_DAY_NIGHT_LIGHTMAP");
		this._USE_TEX_ARRAY_ATLAS = mat.IsKeywordEnabled("_USE_TEX_ARRAY_ATLAS");
		this._GT_BASE_MAP_ATLAS_SLICE_SOURCE__PROPERTY = mat.IsKeywordEnabled("_GT_BASE_MAP_ATLAS_SLICE_SOURCE__PROPERTY");
		this._GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z = mat.IsKeywordEnabled("_GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z");
		this._CRYSTAL_EFFECT = mat.IsKeywordEnabled("_CRYSTAL_EFFECT");
		this._EYECOMP = mat.IsKeywordEnabled("_EYECOMP");
		this._MOUTHCOMP = mat.IsKeywordEnabled("_MOUTHCOMP");
		this._ALPHA_BLUE_LIVE_ON = mat.IsKeywordEnabled("_ALPHA_BLUE_LIVE_ON");
		this._GRID_EFFECT = mat.IsKeywordEnabled("_GRID_EFFECT");
		this._REFLECTIONS = mat.IsKeywordEnabled("_REFLECTIONS");
		this._REFLECTIONS_BOX_PROJECT = mat.IsKeywordEnabled("_REFLECTIONS_BOX_PROJECT");
		this._REFLECTIONS_MATCAP = mat.IsKeywordEnabled("_REFLECTIONS_MATCAP");
		this._REFLECTIONS_MATCAP_PERSP_AWARE = mat.IsKeywordEnabled("_REFLECTIONS_MATCAP_PERSP_AWARE");
		this._REFLECTIONS_ALBEDO_TINT = mat.IsKeywordEnabled("_REFLECTIONS_ALBEDO_TINT");
		this._REFLECTIONS_USE_NORMAL_TEX = mat.IsKeywordEnabled("_REFLECTIONS_USE_NORMAL_TEX");
		this._VERTEX_ROTATE = mat.IsKeywordEnabled("_VERTEX_ROTATE");
		this._VERTEX_ANIM_FLAP = mat.IsKeywordEnabled("_VERTEX_ANIM_FLAP");
		this._VERTEX_ANIM_WAVE = mat.IsKeywordEnabled("_VERTEX_ANIM_WAVE");
		this._VERTEX_ANIM_WAVE_DEBUG = mat.IsKeywordEnabled("_VERTEX_ANIM_WAVE_DEBUG");
		this._GRADIENT_MAP_ON = mat.IsKeywordEnabled("_GRADIENT_MAP_ON");
		this._PARALLAX = mat.IsKeywordEnabled("_PARALLAX");
		this._PARALLAX_AA = mat.IsKeywordEnabled("_PARALLAX_AA");
		this._PARALLAX_PLANAR = mat.IsKeywordEnabled("_PARALLAX_PLANAR");
		this._MASK_MAP_ON = mat.IsKeywordEnabled("_MASK_MAP_ON");
		this._FX_LAVA_LAMP = mat.IsKeywordEnabled("_FX_LAVA_LAMP");
		this._INNER_GLOW = mat.IsKeywordEnabled("_INNER_GLOW");
		this._STEALTH_EFFECT = mat.IsKeywordEnabled("_STEALTH_EFFECT");
		this._UV_SHIFT = mat.IsKeywordEnabled("_UV_SHIFT");
		this._TEXEL_SNAP_UVS = mat.IsKeywordEnabled("_TEXEL_SNAP_UVS");
		this._UNITY_EDIT_MODE = mat.IsKeywordEnabled("_UNITY_EDIT_MODE");
		this._GT_EDITOR_TIME = mat.IsKeywordEnabled("_GT_EDITOR_TIME");
		this._DEBUG_PAWN_DATA = mat.IsKeywordEnabled("_DEBUG_PAWN_DATA");
		this._COLOR_GRADE_PROTANOMALY = mat.IsKeywordEnabled("_COLOR_GRADE_PROTANOMALY");
		this._COLOR_GRADE_PROTANOPIA = mat.IsKeywordEnabled("_COLOR_GRADE_PROTANOPIA");
		this._COLOR_GRADE_DEUTERANOMALY = mat.IsKeywordEnabled("_COLOR_GRADE_DEUTERANOMALY");
		this._COLOR_GRADE_DEUTERANOPIA = mat.IsKeywordEnabled("_COLOR_GRADE_DEUTERANOPIA");
		this._COLOR_GRADE_TRITANOMALY = mat.IsKeywordEnabled("_COLOR_GRADE_TRITANOMALY");
		this._COLOR_GRADE_TRITANOPIA = mat.IsKeywordEnabled("_COLOR_GRADE_TRITANOPIA");
		this._COLOR_GRADE_ACHROMATOMALY = mat.IsKeywordEnabled("_COLOR_GRADE_ACHROMATOMALY");
		this._COLOR_GRADE_ACHROMATOPSIA = mat.IsKeywordEnabled("_COLOR_GRADE_ACHROMATOPSIA");
		this.LIGHTMAP_ON = mat.IsKeywordEnabled("LIGHTMAP_ON");
		this.DIRLIGHTMAP_COMBINED = mat.IsKeywordEnabled("DIRLIGHTMAP_COMBINED");
		this.INSTANCING_ON = mat.IsKeywordEnabled("INSTANCING_ON");
	}

	// Token: 0x06005057 RID: 20567 RVA: 0x0019D298 File Offset: 0x0019B498
	public void Refresh()
	{
		Material material = this.material;
		this.STEREO_INSTANCING_ON = material.IsKeywordEnabled("STEREO_INSTANCING_ON");
		this.UNITY_SINGLE_PASS_STEREO = material.IsKeywordEnabled("UNITY_SINGLE_PASS_STEREO");
		this.STEREO_MULTIVIEW_ON = material.IsKeywordEnabled("STEREO_MULTIVIEW_ON");
		this.STEREO_CUBEMAP_RENDER_ON = material.IsKeywordEnabled("STEREO_CUBEMAP_RENDER_ON");
		this._GLOBAL_ZONE_LIQUID_TYPE__WATER = material.IsKeywordEnabled("_GLOBAL_ZONE_LIQUID_TYPE__WATER");
		this._GLOBAL_ZONE_LIQUID_TYPE__LAVA = material.IsKeywordEnabled("_GLOBAL_ZONE_LIQUID_TYPE__LAVA");
		this._ZONE_LIQUID_SHAPE__CYLINDER = material.IsKeywordEnabled("_ZONE_LIQUID_SHAPE__CYLINDER");
		this._ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX = material.IsKeywordEnabled("_ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX");
		this._USE_TEXTURE = material.IsKeywordEnabled("_USE_TEXTURE");
		this.USE_TEXTURE__AS_MASK = material.IsKeywordEnabled("USE_TEXTURE__AS_MASK");
		this._UV_SOURCE__UV0 = material.IsKeywordEnabled("_UV_SOURCE__UV0");
		this._UV_SOURCE__WORLD_PLANAR_Y = material.IsKeywordEnabled("_UV_SOURCE__WORLD_PLANAR_Y");
		this._USE_VERTEX_COLOR = material.IsKeywordEnabled("_USE_VERTEX_COLOR");
		this._USE_WEATHER_MAP = material.IsKeywordEnabled("_USE_WEATHER_MAP");
		this._ALPHA_DETAIL_MAP = material.IsKeywordEnabled("_ALPHA_DETAIL_MAP");
		this._HALF_LAMBERT_TERM = material.IsKeywordEnabled("_HALF_LAMBERT_TERM");
		this._WATER_EFFECT = material.IsKeywordEnabled("_WATER_EFFECT");
		this._HEIGHT_BASED_WATER_EFFECT = material.IsKeywordEnabled("_HEIGHT_BASED_WATER_EFFECT");
		this._WATER_CAUSTICS = material.IsKeywordEnabled("_WATER_CAUSTICS");
		this._ALPHATEST_ON = material.IsKeywordEnabled("_ALPHATEST_ON");
		this._MAINTEX_ROTATE = material.IsKeywordEnabled("_MAINTEX_ROTATE");
		this._UV_WAVE_WARP = material.IsKeywordEnabled("_UV_WAVE_WARP");
		this._LIQUID_VOLUME = material.IsKeywordEnabled("_LIQUID_VOLUME");
		this._LIQUID_CONTAINER = material.IsKeywordEnabled("_LIQUID_CONTAINER");
		this._GT_RIM_LIGHT = material.IsKeywordEnabled("_GT_RIM_LIGHT");
		this._GT_RIM_LIGHT_FLAT = material.IsKeywordEnabled("_GT_RIM_LIGHT_FLAT");
		this._GT_RIM_LIGHT_USE_ALPHA = material.IsKeywordEnabled("_GT_RIM_LIGHT_USE_ALPHA");
		this._SPECULAR_HIGHLIGHT = material.IsKeywordEnabled("_SPECULAR_HIGHLIGHT");
		this._EMISSION = material.IsKeywordEnabled("_EMISSION");
		this._EMISSION_USE_UV_WAVE_WARP = material.IsKeywordEnabled("_EMISSION_USE_UV_WAVE_WARP");
		this._USE_DEFORM_MAP = material.IsKeywordEnabled("_USE_DEFORM_MAP");
		this._USE_DAY_NIGHT_LIGHTMAP = material.IsKeywordEnabled("_USE_DAY_NIGHT_LIGHTMAP");
		this._USE_TEX_ARRAY_ATLAS = material.IsKeywordEnabled("_USE_TEX_ARRAY_ATLAS");
		this._GT_BASE_MAP_ATLAS_SLICE_SOURCE__PROPERTY = material.IsKeywordEnabled("_GT_BASE_MAP_ATLAS_SLICE_SOURCE__PROPERTY");
		this._GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z = material.IsKeywordEnabled("_GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z");
		this._CRYSTAL_EFFECT = material.IsKeywordEnabled("_CRYSTAL_EFFECT");
		this._EYECOMP = material.IsKeywordEnabled("_EYECOMP");
		this._MOUTHCOMP = material.IsKeywordEnabled("_MOUTHCOMP");
		this._ALPHA_BLUE_LIVE_ON = material.IsKeywordEnabled("_ALPHA_BLUE_LIVE_ON");
		this._GRID_EFFECT = material.IsKeywordEnabled("_GRID_EFFECT");
		this._REFLECTIONS = material.IsKeywordEnabled("_REFLECTIONS");
		this._REFLECTIONS_BOX_PROJECT = material.IsKeywordEnabled("_REFLECTIONS_BOX_PROJECT");
		this._REFLECTIONS_MATCAP = material.IsKeywordEnabled("_REFLECTIONS_MATCAP");
		this._REFLECTIONS_MATCAP_PERSP_AWARE = material.IsKeywordEnabled("_REFLECTIONS_MATCAP_PERSP_AWARE");
		this._REFLECTIONS_ALBEDO_TINT = material.IsKeywordEnabled("_REFLECTIONS_ALBEDO_TINT");
		this._REFLECTIONS_USE_NORMAL_TEX = material.IsKeywordEnabled("_REFLECTIONS_USE_NORMAL_TEX");
		this._VERTEX_ROTATE = material.IsKeywordEnabled("_VERTEX_ROTATE");
		this._VERTEX_ANIM_FLAP = material.IsKeywordEnabled("_VERTEX_ANIM_FLAP");
		this._VERTEX_ANIM_WAVE = material.IsKeywordEnabled("_VERTEX_ANIM_WAVE");
		this._VERTEX_ANIM_WAVE_DEBUG = material.IsKeywordEnabled("_VERTEX_ANIM_WAVE_DEBUG");
		this._GRADIENT_MAP_ON = material.IsKeywordEnabled("_GRADIENT_MAP_ON");
		this._PARALLAX = material.IsKeywordEnabled("_PARALLAX");
		this._PARALLAX_AA = material.IsKeywordEnabled("_PARALLAX_AA");
		this._PARALLAX_PLANAR = material.IsKeywordEnabled("_PARALLAX_PLANAR");
		this._MASK_MAP_ON = material.IsKeywordEnabled("_MASK_MAP_ON");
		this._FX_LAVA_LAMP = material.IsKeywordEnabled("_FX_LAVA_LAMP");
		this._INNER_GLOW = material.IsKeywordEnabled("_INNER_GLOW");
		this._STEALTH_EFFECT = material.IsKeywordEnabled("_STEALTH_EFFECT");
		this._UV_SHIFT = material.IsKeywordEnabled("_UV_SHIFT");
		this._TEXEL_SNAP_UVS = material.IsKeywordEnabled("_TEXEL_SNAP_UVS");
		this._UNITY_EDIT_MODE = material.IsKeywordEnabled("_UNITY_EDIT_MODE");
		this._GT_EDITOR_TIME = material.IsKeywordEnabled("_GT_EDITOR_TIME");
		this._DEBUG_PAWN_DATA = material.IsKeywordEnabled("_DEBUG_PAWN_DATA");
		this._COLOR_GRADE_PROTANOMALY = material.IsKeywordEnabled("_COLOR_GRADE_PROTANOMALY");
		this._COLOR_GRADE_PROTANOPIA = material.IsKeywordEnabled("_COLOR_GRADE_PROTANOPIA");
		this._COLOR_GRADE_DEUTERANOMALY = material.IsKeywordEnabled("_COLOR_GRADE_DEUTERANOMALY");
		this._COLOR_GRADE_DEUTERANOPIA = material.IsKeywordEnabled("_COLOR_GRADE_DEUTERANOPIA");
		this._COLOR_GRADE_TRITANOMALY = material.IsKeywordEnabled("_COLOR_GRADE_TRITANOMALY");
		this._COLOR_GRADE_TRITANOPIA = material.IsKeywordEnabled("_COLOR_GRADE_TRITANOPIA");
		this._COLOR_GRADE_ACHROMATOMALY = material.IsKeywordEnabled("_COLOR_GRADE_ACHROMATOMALY");
		this._COLOR_GRADE_ACHROMATOPSIA = material.IsKeywordEnabled("_COLOR_GRADE_ACHROMATOPSIA");
		this.LIGHTMAP_ON = material.IsKeywordEnabled("LIGHTMAP_ON");
		this.DIRLIGHTMAP_COMBINED = material.IsKeywordEnabled("DIRLIGHTMAP_COMBINED");
		this.INSTANCING_ON = material.IsKeywordEnabled("INSTANCING_ON");
	}

	// Token: 0x04005F42 RID: 24386
	public Material material;

	// Token: 0x04005F43 RID: 24387
	public bool STEREO_INSTANCING_ON;

	// Token: 0x04005F44 RID: 24388
	public bool UNITY_SINGLE_PASS_STEREO;

	// Token: 0x04005F45 RID: 24389
	public bool STEREO_MULTIVIEW_ON;

	// Token: 0x04005F46 RID: 24390
	public bool STEREO_CUBEMAP_RENDER_ON;

	// Token: 0x04005F47 RID: 24391
	public bool _GLOBAL_ZONE_LIQUID_TYPE__WATER;

	// Token: 0x04005F48 RID: 24392
	public bool _GLOBAL_ZONE_LIQUID_TYPE__LAVA;

	// Token: 0x04005F49 RID: 24393
	public bool _ZONE_LIQUID_SHAPE__CYLINDER;

	// Token: 0x04005F4A RID: 24394
	public bool _ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX;

	// Token: 0x04005F4B RID: 24395
	public bool _USE_TEXTURE;

	// Token: 0x04005F4C RID: 24396
	public bool USE_TEXTURE__AS_MASK;

	// Token: 0x04005F4D RID: 24397
	public bool _UV_SOURCE__UV0;

	// Token: 0x04005F4E RID: 24398
	public bool _UV_SOURCE__WORLD_PLANAR_Y;

	// Token: 0x04005F4F RID: 24399
	public bool _USE_VERTEX_COLOR;

	// Token: 0x04005F50 RID: 24400
	public bool _USE_WEATHER_MAP;

	// Token: 0x04005F51 RID: 24401
	public bool _ALPHA_DETAIL_MAP;

	// Token: 0x04005F52 RID: 24402
	public bool _HALF_LAMBERT_TERM;

	// Token: 0x04005F53 RID: 24403
	public bool _WATER_EFFECT;

	// Token: 0x04005F54 RID: 24404
	public bool _HEIGHT_BASED_WATER_EFFECT;

	// Token: 0x04005F55 RID: 24405
	public bool _WATER_CAUSTICS;

	// Token: 0x04005F56 RID: 24406
	public bool _ALPHATEST_ON;

	// Token: 0x04005F57 RID: 24407
	public bool _MAINTEX_ROTATE;

	// Token: 0x04005F58 RID: 24408
	public bool _UV_WAVE_WARP;

	// Token: 0x04005F59 RID: 24409
	public bool _LIQUID_VOLUME;

	// Token: 0x04005F5A RID: 24410
	public bool _LIQUID_CONTAINER;

	// Token: 0x04005F5B RID: 24411
	public bool _GT_RIM_LIGHT;

	// Token: 0x04005F5C RID: 24412
	public bool _GT_RIM_LIGHT_FLAT;

	// Token: 0x04005F5D RID: 24413
	public bool _GT_RIM_LIGHT_USE_ALPHA;

	// Token: 0x04005F5E RID: 24414
	public bool _SPECULAR_HIGHLIGHT;

	// Token: 0x04005F5F RID: 24415
	public bool _EMISSION;

	// Token: 0x04005F60 RID: 24416
	public bool _EMISSION_USE_UV_WAVE_WARP;

	// Token: 0x04005F61 RID: 24417
	public bool _USE_DEFORM_MAP;

	// Token: 0x04005F62 RID: 24418
	public bool _USE_DAY_NIGHT_LIGHTMAP;

	// Token: 0x04005F63 RID: 24419
	public bool _USE_TEX_ARRAY_ATLAS;

	// Token: 0x04005F64 RID: 24420
	public bool _GT_BASE_MAP_ATLAS_SLICE_SOURCE__PROPERTY;

	// Token: 0x04005F65 RID: 24421
	public bool _GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z;

	// Token: 0x04005F66 RID: 24422
	public bool _CRYSTAL_EFFECT;

	// Token: 0x04005F67 RID: 24423
	public bool _EYECOMP;

	// Token: 0x04005F68 RID: 24424
	public bool _MOUTHCOMP;

	// Token: 0x04005F69 RID: 24425
	public bool _ALPHA_BLUE_LIVE_ON;

	// Token: 0x04005F6A RID: 24426
	public bool _GRID_EFFECT;

	// Token: 0x04005F6B RID: 24427
	public bool _REFLECTIONS;

	// Token: 0x04005F6C RID: 24428
	public bool _REFLECTIONS_BOX_PROJECT;

	// Token: 0x04005F6D RID: 24429
	public bool _REFLECTIONS_MATCAP;

	// Token: 0x04005F6E RID: 24430
	public bool _REFLECTIONS_MATCAP_PERSP_AWARE;

	// Token: 0x04005F6F RID: 24431
	public bool _REFLECTIONS_ALBEDO_TINT;

	// Token: 0x04005F70 RID: 24432
	public bool _REFLECTIONS_USE_NORMAL_TEX;

	// Token: 0x04005F71 RID: 24433
	public bool _VERTEX_ROTATE;

	// Token: 0x04005F72 RID: 24434
	public bool _VERTEX_ANIM_FLAP;

	// Token: 0x04005F73 RID: 24435
	public bool _VERTEX_ANIM_WAVE;

	// Token: 0x04005F74 RID: 24436
	public bool _VERTEX_ANIM_WAVE_DEBUG;

	// Token: 0x04005F75 RID: 24437
	public bool _GRADIENT_MAP_ON;

	// Token: 0x04005F76 RID: 24438
	public bool _PARALLAX;

	// Token: 0x04005F77 RID: 24439
	public bool _PARALLAX_AA;

	// Token: 0x04005F78 RID: 24440
	public bool _PARALLAX_PLANAR;

	// Token: 0x04005F79 RID: 24441
	public bool _MASK_MAP_ON;

	// Token: 0x04005F7A RID: 24442
	public bool _FX_LAVA_LAMP;

	// Token: 0x04005F7B RID: 24443
	public bool _INNER_GLOW;

	// Token: 0x04005F7C RID: 24444
	public bool _STEALTH_EFFECT;

	// Token: 0x04005F7D RID: 24445
	public bool _UV_SHIFT;

	// Token: 0x04005F7E RID: 24446
	public bool _TEXEL_SNAP_UVS;

	// Token: 0x04005F7F RID: 24447
	public bool _UNITY_EDIT_MODE;

	// Token: 0x04005F80 RID: 24448
	public bool _GT_EDITOR_TIME;

	// Token: 0x04005F81 RID: 24449
	public bool _DEBUG_PAWN_DATA;

	// Token: 0x04005F82 RID: 24450
	public bool _COLOR_GRADE_PROTANOMALY;

	// Token: 0x04005F83 RID: 24451
	public bool _COLOR_GRADE_PROTANOPIA;

	// Token: 0x04005F84 RID: 24452
	public bool _COLOR_GRADE_DEUTERANOMALY;

	// Token: 0x04005F85 RID: 24453
	public bool _COLOR_GRADE_DEUTERANOPIA;

	// Token: 0x04005F86 RID: 24454
	public bool _COLOR_GRADE_TRITANOMALY;

	// Token: 0x04005F87 RID: 24455
	public bool _COLOR_GRADE_TRITANOPIA;

	// Token: 0x04005F88 RID: 24456
	public bool _COLOR_GRADE_ACHROMATOMALY;

	// Token: 0x04005F89 RID: 24457
	public bool _COLOR_GRADE_ACHROMATOPSIA;

	// Token: 0x04005F8A RID: 24458
	public bool LIGHTMAP_ON;

	// Token: 0x04005F8B RID: 24459
	public bool DIRLIGHTMAP_COMBINED;

	// Token: 0x04005F8C RID: 24460
	public bool INSTANCING_ON;
}
