using System;

// Token: 0x02000100 RID: 256
public enum SIUpgradeType
{
	// Token: 0x0400082F RID: 2095
	InvalidNode = -2,
	// Token: 0x04000830 RID: 2096
	Initialize,
	// Token: 0x04000831 RID: 2097
	Thruster_Unlock,
	// Token: 0x04000832 RID: 2098
	Thruster_Jet,
	// Token: 0x04000833 RID: 2099
	Thruster_Prop,
	// Token: 0x04000834 RID: 2100
	Thruster_Jet_Duration,
	// Token: 0x04000835 RID: 2101
	Thruster_Jet_Accel,
	// Token: 0x04000836 RID: 2102
	Thruster_Prop_Duration,
	// Token: 0x04000837 RID: 2103
	Thruster_Prop_Speed,
	// Token: 0x04000838 RID: 2104
	Thruster_Jet_Tag,
	// Token: 0x04000839 RID: 2105
	Thruster_Prop_Knockback,
	// Token: 0x0400083A RID: 2106
	Thruster_Fuel_Grounding,
	// Token: 0x0400083B RID: 2107
	Thruster_Throttle_Control,
	// Token: 0x0400083C RID: 2108
	Stilt_Unlock = 100,
	// Token: 0x0400083D RID: 2109
	Stilt_Tag_Tip,
	// Token: 0x0400083E RID: 2110
	Stilt_Retractable,
	// Token: 0x0400083F RID: 2111
	Stilt_Adjustable_Length,
	// Token: 0x04000840 RID: 2112
	Stilt_Retract_Speed,
	// Token: 0x04000841 RID: 2113
	Stilt_Max_Length,
	// Token: 0x04000842 RID: 2114
	Stilt_Stun_Tip,
	// Token: 0x04000843 RID: 2115
	Stilt_Muscle_Fusion,
	// Token: 0x04000844 RID: 2116
	Stilt_Short,
	// Token: 0x04000845 RID: 2117
	Stilt_Long,
	// Token: 0x04000846 RID: 2118
	Stilt_Motorized,
	// Token: 0x04000847 RID: 2119
	Stilt_Motorized_Triple,
	// Token: 0x04000848 RID: 2120
	Stilt_Turkey_Coma,
	// Token: 0x04000849 RID: 2121
	Grenade_Concussion_Unlock = 200,
	// Token: 0x0400084A RID: 2122
	Grenade_Antigravity_Unlock,
	// Token: 0x0400084B RID: 2123
	Grenade_Concussion_Stun,
	// Token: 0x0400084C RID: 2124
	Grenade_Concussion_Radius,
	// Token: 0x0400084D RID: 2125
	Grenade_Antigravity_Persists,
	// Token: 0x0400084E RID: 2126
	Grenade_Antigravity_Cooldown,
	// Token: 0x0400084F RID: 2127
	Grenade_Concussion_Self_Boost,
	// Token: 0x04000850 RID: 2128
	Grenade_Concussion_Overcharge,
	// Token: 0x04000851 RID: 2129
	Grenade_Antigravity_Pro_Gravity,
	// Token: 0x04000852 RID: 2130
	Grenade_Concussion_Impact_Accelerant,
	// Token: 0x04000853 RID: 2131
	Grenade_Antigravity_Gravity_Bomb,
	// Token: 0x04000854 RID: 2132
	Grenade_Antigravity_Black_Hole,
	// Token: 0x04000855 RID: 2133
	Grenade_Holster_Unlock,
	// Token: 0x04000856 RID: 2134
	Grenade_Stun_Unlock,
	// Token: 0x04000857 RID: 2135
	Grenade_Puller_Unlock,
	// Token: 0x04000858 RID: 2136
	Grenade_Disrupter_Unlock,
	// Token: 0x04000859 RID: 2137
	Dash_Slash_Unlock = 300,
	// Token: 0x0400085A RID: 2138
	Dash_Yoyo_Unlock,
	// Token: 0x0400085B RID: 2139
	Dash_Slash_Speed,
	// Token: 0x0400085C RID: 2140
	Dash_Slash_Cooldown,
	// Token: 0x0400085D RID: 2141
	Dash_Yoyo_Range,
	// Token: 0x0400085E RID: 2142
	Dash_Yoyo_Speed,
	// Token: 0x0400085F RID: 2143
	Dash_Unused_306,
	// Token: 0x04000860 RID: 2144
	Dash_Unused_307,
	// Token: 0x04000861 RID: 2145
	Dash_Yoyo_Cooldown,
	// Token: 0x04000862 RID: 2146
	Dash_Yoyo_Dynamic,
	// Token: 0x04000863 RID: 2147
	Dash_Unused_310,
	// Token: 0x04000864 RID: 2148
	Dash_Yoyo_Stun,
	// Token: 0x04000865 RID: 2149
	Dash_Yoyo_Tag,
	// Token: 0x04000866 RID: 2150
	Dash_Unused_313,
	// Token: 0x04000867 RID: 2151
	Dash_Unused_314,
	// Token: 0x04000868 RID: 2152
	Platform_Unlock = 400,
	// Token: 0x04000869 RID: 2153
	Platform_Cooldown,
	// Token: 0x0400086A RID: 2154
	Platform_Duration,
	// Token: 0x0400086B RID: 2155
	Platform_Capacity,
	// Token: 0x0400086C RID: 2156
	Platform_SpeedBoost,
	// Token: 0x0400086D RID: 2157
	Tapteleport_Unlock = 500,
	// Token: 0x0400086E RID: 2158
	Tapteleport_Zone,
	// Token: 0x0400086F RID: 2159
	Tapteleport_Stealth,
	// Token: 0x04000870 RID: 2160
	Tapteleport_Portal_Selection,
	// Token: 0x04000871 RID: 2161
	Tapteleport_Keep_Velocity,
	// Token: 0x04000872 RID: 2162
	Tapteleport_Infinite_Use,
	// Token: 0x04000873 RID: 2163
	Tentacle_Unlock = 600,
	// Token: 0x04000874 RID: 2164
	AirGrab_Unlock = 700,
	// Token: 0x04000875 RID: 2165
	LaserZipline_Unlock,
	// Token: 0x04000876 RID: 2166
	Wing_Unlock,
	// Token: 0x04000877 RID: 2167
	Unused_800 = 800,
	// Token: 0x04000878 RID: 2168
	SlipMitt_Unlock = 900,
	// Token: 0x04000879 RID: 2169
	Blaster_Standard_Unlock = 1000,
	// Token: 0x0400087A RID: 2170
	Blaster_Charge_Unlock,
	// Token: 0x0400087B RID: 2171
	Blaster_Lobber_Unlock,
	// Token: 0x0400087C RID: 2172
	Blaster_PumpDart_Unlock,
	// Token: 0x0400087D RID: 2173
	Blaster_MegaCharge_Unlock,
	// Token: 0x0400087E RID: 2174
	Blaster_LongBlaster_Unlock
}
