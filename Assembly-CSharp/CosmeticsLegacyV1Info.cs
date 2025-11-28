using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

// Token: 0x02000290 RID: 656
public static class CosmeticsLegacyV1Info
{
	// Token: 0x060010CE RID: 4302 RVA: 0x000577E0 File Offset: 0x000559E0
	[MethodImpl(256)]
	public static bool TryGetPlayFabId(string unityItemId, string unityDisplayName, string unityOverrideDisplayName, out string playFabId)
	{
		return CosmeticsLegacyV1Info.k_v1DisplayNames_to_playFabIds.TryGetValue(unityItemId, ref playFabId) || CosmeticsLegacyV1Info.k_v1DisplayNames_to_playFabIds.TryGetValue(unityDisplayName, ref playFabId) || CosmeticsLegacyV1Info.k_v1DisplayNames_to_playFabIds.TryGetValue(unityOverrideDisplayName, ref playFabId) || CosmeticsLegacyV1Info.k_special.TryGetValue(unityItemId, ref playFabId) || CosmeticsLegacyV1Info.k_special.TryGetValue(unityDisplayName, ref playFabId) || CosmeticsLegacyV1Info.k_special.TryGetValue(unityOverrideDisplayName, ref playFabId) || CosmeticsLegacyV1Info.k_packs.TryGetValue(unityItemId, ref playFabId) || CosmeticsLegacyV1Info.k_packs.TryGetValue(unityDisplayName, ref playFabId) || CosmeticsLegacyV1Info.k_packs.TryGetValue(unityOverrideDisplayName, ref playFabId) || CosmeticsLegacyV1Info.k_oldPacks.TryGetValue(unityItemId, ref playFabId) || CosmeticsLegacyV1Info.k_oldPacks.TryGetValue(unityDisplayName, ref playFabId) || CosmeticsLegacyV1Info.k_oldPacks.TryGetValue(unityOverrideDisplayName, ref playFabId) || CosmeticsLegacyV1Info.k_unused.TryGetValue(unityItemId, ref playFabId) || CosmeticsLegacyV1Info.k_unused.TryGetValue(unityDisplayName, ref playFabId) || CosmeticsLegacyV1Info.k_unused.TryGetValue(unityOverrideDisplayName, ref playFabId);
	}

	// Token: 0x060010CF RID: 4303 RVA: 0x000578D4 File Offset: 0x00055AD4
	[MethodImpl(256)]
	public static bool TryGetPlayFabId(string unityItemId, out string playFabId, bool logErrors = false)
	{
		return CosmeticsLegacyV1Info.k_v1DisplayNames_to_playFabIds.TryGetValue(unityItemId, ref playFabId) || CosmeticsLegacyV1Info.k_special.TryGetValue(unityItemId, ref playFabId) || CosmeticsLegacyV1Info.k_packs.TryGetValue(unityItemId, ref playFabId) || CosmeticsLegacyV1Info.k_oldPacks.TryGetValue(unityItemId, ref playFabId) || CosmeticsLegacyV1Info.k_unused.TryGetValue(unityItemId, ref playFabId);
	}

	// Token: 0x060010D0 RID: 4304 RVA: 0x0005792A File Offset: 0x00055B2A
	[MethodImpl(256)]
	public static bool TryGetBodyDockAllObjectsIndexes(string playFabId, out int[] bdAllIndexes)
	{
		return CosmeticsLegacyV1Info._k_playFabId_to_bodyDockPositions_allObjects_indexes.TryGetValue(playFabId, ref bdAllIndexes);
	}

	// Token: 0x060010D1 RID: 4305 RVA: 0x00057938 File Offset: 0x00055B38
	// Note: this type is marked as 'beforefieldinit'.
	static CosmeticsLegacyV1Info()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Slingshot", "Slingshot");
		CosmeticsLegacyV1Info.k_special = dictionary;
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
		dictionary2.Add("TUXEDO SET", "LSAAO.");
		dictionary2.Add("EXPLORER SET", "LSAAN.");
		dictionary2.Add("SANTA SET 22", "LSAAP.");
		dictionary2.Add("SNOWMAN SET", "LSAAQ.");
		dictionary2.Add("EVIL SANTA SET", "LSAAR.");
		dictionary2.Add("Day 1 Pack", "LSAAP2.");
		dictionary2.Add("DAY 1 PACK", "LSAAP2.");
		dictionary2.Add("LAUNCH BUNDLE", "LSAAP2.");
		dictionary2.Add("LSAAP.2. (1)", "LSAAP2.");
		dictionary2.Add("POLAR BEAR SET", "LSAAT.");
		dictionary2.Add("WIZARD SET", "LSAAV.");
		dictionary2.Add("KNIGHT SET", "LSAAW.");
		dictionary2.Add("BARBARIAN SET", "LSAAX.");
		dictionary2.Add("ORC SET", "LSAAY.");
		dictionary2.Add("LSAAS.", "LSAAS.");
		dictionary2.Add("LSAAU.", "LSAAU.");
		dictionary2.Add("MERFOLK SET", "LSAAZ.");
		dictionary2.Add("SCUBA SET", "LSABA.");
		dictionary2.Add("SAFARI SET", "LSABB.");
		dictionary2.Add("CRYSTAL CAVERNS SET", "LSABC.");
		dictionary2.Add("SPIDER MONKE PACK", "LSABD.");
		dictionary2.Add("HOLIDAY FIR PACK", "LSABE.");
		dictionary2.Add("MAD SCIENTIST PACK", "LSABF.");
		dictionary2.Add("I LAVA YOU PACK", "LSABG.");
		dictionary2.Add("BEEKEEPER PACK", "LSABH.");
		dictionary2.Add("LEAF NINJA PACK", "LSABJ.");
		dictionary2.Add("MONKE MONK PACK", "LSABK.");
		dictionary2.Add("GLAM ROCKER PACK", "LSABL.");
		CosmeticsLegacyV1Info.k_packs = dictionary2;
		Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
		dictionary3.Add("CLOWN SET", "CLOWN SET");
		dictionary3.Add("VAMPIRE SET", "VAMPIRE SET");
		dictionary3.Add("WEREWOLF SET", "WEREWOLF SET");
		dictionary3.Add("STAR PRINCESS SET", "STAR PRINCESS SET");
		dictionary3.Add("SANTA SET", "SANTA SET");
		dictionary3.Add("CARDBOARD ARMOR SET", "CARDBOARD ARMOR SET");
		dictionary3.Add("SPIKED ARMOR SET", "SPIKED ARMOR SET");
		dictionary3.Add("GORILLA ARMOR SET", "GORILLA ARMOR SET");
		dictionary3.Add("SHERIFF SET", "SHERIFF SET");
		dictionary3.Add("ROBOT SET", "ROBOT SET");
		dictionary3.Add("CLOWN 22 SET", "CLOWN 22 SET");
		dictionary3.Add("SUPER HERO SET", "SUPER HERO SET");
		dictionary3.Add("UNICORN PRINCESS SET", "UNICORN PRINCESS SET");
		CosmeticsLegacyV1Info.k_oldPacks = dictionary3;
		Dictionary<string, string> dictionary4 = new Dictionary<string, string>();
		dictionary4.Add("HIGH TECH SLINGSHOT", "HIGH TECH SLINGSHOT");
		dictionary4.Add("THROWABLE SQUISHY EYEBALL", "THROWABLE SQUISHY EYEBALL");
		CosmeticsLegacyV1Info.k_unused = dictionary4;
		Dictionary<string, string> dictionary5 = new Dictionary<string, string>();
		dictionary5.Add("TREE PIN", "LBAAA.");
		dictionary5.Add("BOWTIE", "LBAAB.");
		dictionary5.Add("BASIC SCARF", "LBAAC.");
		dictionary5.Add("ADMINISTRATOR BADGE", "LBAAD.");
		dictionary5.Add("EARLY ACCESS", "LBAAE.");
		dictionary5.Add("CRYSTALS PIN", "LBAAF.");
		dictionary5.Add("CANYON PIN", "LBAAG.");
		dictionary5.Add("CITY PIN", "LBAAH.");
		dictionary5.Add("GORILLA PIN", "LBAAI.");
		dictionary5.Add("NECK SCARF", "LBAAJ.");
		dictionary5.Add("MOD STICK", "LBAAK.");
		dictionary5.Add("CLOWN FRILL", "LBAAL.");
		dictionary5.Add("VAMPIRE COLLAR", "LBAAM.");
		dictionary5.Add("WEREWOLF CLAWS", "LBAAN.");
		dictionary5.Add("STAR PRINCESS WAND", "LBAAO.");
		dictionary5.Add("TURKEY LEG", "LBAAP.");
		dictionary5.Add("TURKEY FINGER PUPPET", "LBAAQ.");
		dictionary5.Add("CANDY CANE", "LBAAR.");
		dictionary5.Add("SPARKLER", "LBAAS.");
		dictionary5.Add("ICICLE", "LBAAT.");
		dictionary5.Add("CHEST HEART", "LBAAU.");
		dictionary5.Add("RED ROSE", "LBAAV.");
		dictionary5.Add("PINK ROSE", "LBAAW.");
		dictionary5.Add("BLACK ROSE", "LBAAX.");
		dictionary5.Add("GOLD ROSE", "LBAAY.");
		dictionary5.Add("GT1 BADGE", "LBAAZ.");
		dictionary5.Add("THUMB PARTYHATS", "LBABA.");
		dictionary5.Add("REGULAR WRENCH", "LBABB.");
		dictionary5.Add("GOLD WRENCH", "LBABC.");
		dictionary5.Add("REGULAR FORK AND KNIFE", "LBABD.");
		dictionary5.Add("GOLD FORK AND KNIFE", "LBABE.");
		dictionary5.Add("FOUR LEAF CLOVER", "LBABF.");
		dictionary5.Add("GOLDEN FOUR LEAF CLOVER", "LBABG.");
		dictionary5.Add("MOUNTAIN PIN", "LBABH.");
		dictionary5.Add("YELLOW RAIN SHAWL", "LBABI.");
		dictionary5.Add("POCKET GORILLA BUN YELLOW", "LBABJ.");
		dictionary5.Add("POCKET GORILLA BUN BLUE", "LBABK.");
		dictionary5.Add("POCKET GORILLA BUN PINK", "LBABL.");
		dictionary5.Add("BONGOS", "LBABM.");
		dictionary5.Add("DRUM SET", "LBABN.");
		dictionary5.Add("SPILLED ICE CREAM", "LBABO.");
		dictionary5.Add("FLAMINGO FLOATIE", "LBABP.");
		dictionary5.Add("PAINTBALL SNOW VEST", "LBABQ.");
		dictionary5.Add("PAINTBALL FOREST VEST", "LBABR.");
		dictionary5.Add("CARDBOARD ARMOR", "LBABS.");
		dictionary5.Add("GORILLA ARMOR", "LBABT.");
		dictionary5.Add("SPIKED ARMOR", "LBABU.");
		dictionary5.Add("CLOWN VEST", "LBABV.");
		dictionary5.Add("ROBOT BODY", "LBABW.");
		dictionary5.Add("SHERIFF VEST", "LBABX.");
		dictionary5.Add("SUPER HERO BODY", "LBABY.");
		dictionary5.Add("UNICORN TUTU", "LBABZ.");
		dictionary5.Add("BIG EYEBROWS", "LFAAA.");
		dictionary5.Add("NOSE RING", "LFAAB.");
		dictionary5.Add("BASIC EARRINGS", "LFAAC.");
		dictionary5.Add("TRIPLE EARRINGS", "LFAAD.");
		dictionary5.Add("EYEBROW STUD", "LFAAE.");
		dictionary5.Add("TRIANGLE SUNGLASSES", "LFAAF.");
		dictionary5.Add("SKULL MASK", "LFAAG.");
		dictionary5.Add("RIGHT EYEPATCH", "LFAAH.");
		dictionary5.Add("LEFT EYEPATCH", "LFAAI.");
		dictionary5.Add("DOUBLE EYEPATCH", "LFAAJ.");
		dictionary5.Add("GOGGLES", "LFAAK.");
		dictionary5.Add("SURGICAL MASK", "LFAAL.");
		dictionary5.Add("TORTOISESHELL SUNGLASSES", "LFAAM.");
		dictionary5.Add("AVIATORS", "LFAAN.");
		dictionary5.Add("ROUND SUNGLASSES", "LFAAO.");
		dictionary5.Add("WITCH NOSE", "LFAAP.");
		dictionary5.Add("MUMMY WRAP", "LFAAQ.");
		dictionary5.Add("CLOWN NOSE", "LFAAR.");
		dictionary5.Add("VAMPIRE FANGS", "LFAAS.");
		dictionary5.Add("WEREWOLF FACE", "LFAAT.");
		dictionary5.Add("STAR PRINCESS GLASSES", "LFAAU.");
		dictionary5.Add("MAPLE LEAF", "LFAAV.");
		dictionary5.Add("FACE SCARF", "LFAAW.");
		dictionary5.Add("SANTA BEARD", "LFAAX.");
		dictionary5.Add("ORNAMENT EARRINGS", "LFAAY.");
		dictionary5.Add("2022 GLASSES", "LFAAZ.");
		dictionary5.Add("NOSE SNOWFLAKE", "LFABA.");
		dictionary5.Add("ROSY CHEEKS", "LFABB.");
		dictionary5.Add("BOXY SUNGLASSES", "LFABC.");
		dictionary5.Add("HEART GLASSES", "LFABD.");
		dictionary5.Add("COOKIE JAR", "LFABE.");
		dictionary5.Add("BITE ONION", "LFABF.");
		dictionary5.Add("EMPEROR NOSE BUTTERFLY", "LFABG.");
		dictionary5.Add("FOREHEAD EGG", "LFABH.");
		dictionary5.Add("LIGHTNING MAKEUP", "LFABI.");
		dictionary5.Add("BLUE SHUTTERS", "LFABJ.");
		dictionary5.Add("BLACK SHUTTERS", "LFABK.");
		dictionary5.Add("GREEN SHUTTERS", "LFABL.");
		dictionary5.Add("RED SHUTTERS", "LFABM.");
		dictionary5.Add("SUNBURN", "LFABN.");
		dictionary5.Add("SUNSCREEN", "LFABO.");
		dictionary5.Add("PAINTBALL FOREST VISOR", "LFABP.");
		dictionary5.Add("PAINTBALL SNOW VISOR", "LFABQ.");
		dictionary5.Add("PAINTBALL GORILLA VISOR", "LFABR.");
		dictionary5.Add("BULGING GOOGLY EYES", "LFABS.");
		dictionary5.Add("CLOWN NOSE 22", "LFABT.");
		dictionary5.Add("SHERIFF MUSTACHE", "LFABU.");
		dictionary5.Add("SLINKY EYES", "LFABV.");
		dictionary5.Add("MOUTH WHEAT", "LFABW.");
		dictionary5.Add("BANANA HAT", "LHAAA.");
		dictionary5.Add("CAT EARS", "LHAAB.");
		dictionary5.Add("PARTY HAT", "LHAAC.");
		dictionary5.Add("USHANKA", "LHAAD.");
		dictionary5.Add("SWEATBAND", "LHAAE.");
		dictionary5.Add("BASEBALL CAP", "LHAAF.");
		dictionary5.Add("GOLDEN HEAD", "LHAAG.");
		dictionary5.Add("FOREHEAD MIRROR", "LHAAH.");
		dictionary5.Add("PINEAPPLE HAT", "LHAAI.");
		dictionary5.Add("WITCH HAT", "LHAAJ.");
		dictionary5.Add("COCONUT", "LHAAK.");
		dictionary5.Add("SUNHAT", "LHAAL.");
		dictionary5.Add("CLOCHE", "LHAAM.");
		dictionary5.Add("COWBOY HAT", "LHAAN.");
		dictionary5.Add("FEZ", "LHAAO.");
		dictionary5.Add("TOP HAT", "LHAAP.");
		dictionary5.Add("BASIC BEANIE", "LHAAQ.");
		dictionary5.Add("WHITE FEDORA", "LHAAR.");
		dictionary5.Add("FLOWER CROWN", "LHAAS.");
		dictionary5.Add("PAPERBAG HAT", "LHAAT.");
		dictionary5.Add("PUMPKIN HAT", "LHAAU.");
		dictionary5.Add("CLOWN WIG", "LHAAV.");
		dictionary5.Add("VAMPIRE WIG", "LHAAW.");
		dictionary5.Add("WEREWOLF EARS", "LHAAX.");
		dictionary5.Add("STAR PRINCESS TIARA", "LHAAY.");
		dictionary5.Add("PIRATE BANDANA", "LHAAZ.");
		dictionary5.Add("SUNNY SUNHAT", "LHABA.");
		dictionary5.Add("CHROME COWBOY HAT", "LHABB.");
		dictionary5.Add("CHEFS HAT", "LHABC.");
		dictionary5.Add("SANTA HAT", "LHABD.");
		dictionary5.Add("SNOWMAN HAT", "LHABE.");
		dictionary5.Add("GIFT HAT", "LHABF.");
		dictionary5.Add("ELF HAT", "LHABG.");
		dictionary5.Add("ORANGE POMPOM HAT", "LHABH.");
		dictionary5.Add("BLUE POMPOM HAT", "LHABI.");
		dictionary5.Add("STRIPE POMPOM HAT", "LHABJ.");
		dictionary5.Add("PATTERN POMPOM HAT", "LHABK.");
		dictionary5.Add("WHITE EARMUFFS", "LHABL.");
		dictionary5.Add("BLACK EARMUFFS", "LHABM.");
		dictionary5.Add("GREEN EARMUFFS", "LHABN.");
		dictionary5.Add("PINK EARMUFFS", "LHABO.");
		dictionary5.Add("HEADPHONES1", "LHABP.");
		dictionary5.Add("BOX OF CHOCOLATES HAT", "LHABQ.");
		dictionary5.Add("HEART POMPOM HAT", "LHABR.");
		dictionary5.Add("PLUNGER HAT", "LHABS.");
		dictionary5.Add("SAUCEPAN HAT", "LHABT.");
		dictionary5.Add("WHITE BUNNY EARS", "LHABU.");
		dictionary5.Add("BROWN BUNNY EARS", "LHABV.");
		dictionary5.Add("LEPRECHAUN HAT", "LHABW.");
		dictionary5.Add("BLUE LILY HAT", "LHABX.");
		dictionary5.Add("PURPLE LILY HAT", "LHABY.");
		dictionary5.Add("YELLOW RAIN HAT", "LHABZ.");
		dictionary5.Add("PAINTED EGG HAT", "LHACA.");
		dictionary5.Add("BLACK LONGHAIR WIG", "LHACB.");
		dictionary5.Add("RED LONGHAIR WIG", "LHACC.");
		dictionary5.Add("ELECTRO HELM", "LHACD.");
		dictionary5.Add("SEAGULL", "LHACE.");
		dictionary5.Add("ROCKIN MOHAWK", "LHACF.");
		dictionary5.Add("SPIKED HELMET", "LHACG.");
		dictionary5.Add("CARDBOARD HELMET", "LHACH.");
		dictionary5.Add("CLOWN CAP", "LHACI.");
		dictionary5.Add("PUMPKIN HEAD HAPPY", "LHACJ.");
		dictionary5.Add("PUMPKIN HEAD SCARY", "LHACK.");
		dictionary5.Add("ROBOT HEAD", "LHACL.");
		dictionary5.Add("SHERIFF HAT", "LHACM.");
		dictionary5.Add("UNICORN CROWN", "LHACN.");
		dictionary5.Add("SUPER HERO HEADBAND", "LHACO.");
		dictionary5.Add("PIE HAT", "LHACP.");
		dictionary5.Add("SCARECROW HAT", "LHACQ.");
		dictionary5.Add("CHERRY BLOSSOM BRANCH", "LMAAA.");
		dictionary5.Add("CHERRY BLOSSOM BRANCH ROSE GOLD", "LMAAB.");
		dictionary5.Add("YELLOW HAND BOOTS", "LMAAC.");
		dictionary5.Add("CLOUD HAND BOOTS", "LMAAD.");
		dictionary5.Add("GOLDEN HAND BOOTS", "LMAAE.");
		dictionary5.Add("BLACK UMBRELLA", "LMAAF.");
		dictionary5.Add("COLORFUL UMBRELLA", "LMAAG.");
		dictionary5.Add("GOLDEN UMBRELLA", "LMAAH.");
		dictionary5.Add("ACOUSTIC GUITAR", "LMAAI.");
		dictionary5.Add("GOLDEN ACOUSTIC GUITAR", "LMAAJ.");
		dictionary5.Add("ELECTRIC GUITAR", "LMAAK.");
		dictionary5.Add("GOLDEN ELECTRIC GUITAR", "LMAAL.");
		dictionary5.Add("BUBBLER", "LMAAM.");
		dictionary5.Add("POPSICLE", "LMAAN.");
		dictionary5.Add("RUBBER DUCK", "LMAAO.");
		dictionary5.Add("STAR BALLOON", "LMAAP.");
		dictionary5.Add("STAR BALLON", "LMAAP.");
		dictionary5.Add("STICKABLE TAR.GET", "LMAAQ.");
		dictionary5.Add("STICKABLE TARGET", "LMAAQ.");
		dictionary5.Add("DIAMOND BALLOON", "LMAAR.");
		dictionary5.Add("CHOCOLATE DONUT BALLOON", "LMAAS.");
		dictionary5.Add("HEART BALLOON", "LMAAT.");
		dictionary5.Add("FINGER FLAG", "LMAAU.");
		dictionary5.Add("HIGH TECH S.LINGSHOT", "LMAAV.");
		dictionary5.Add("UNICORN STAFF", "LMAAW.");
		dictionary5.Add("GHOST BALLOON", "LMAAX.");
		dictionary5.Add("GIANT CANDY BAR", "LMAAY.");
		dictionary5.Add("CANDY BAR FUN SIZE", "LMAAZ.");
		dictionary5.Add("SPIDER WEB UMBRELLA", "LMABA.");
		dictionary5.Add("DEADSHOT", "LMABB.");
		dictionary5.Add("YORICK", "LMABC.");
		dictionary5.Add("PINK DONUT BALLOON", "LMABD.");
		dictionary5.Add("TURKEY TOY", "LMABE.");
		dictionary5.Add("CRANBERRY CAN", "LMABF.");
		dictionary5.Add("FRYING PAN", "LMABG.");
		dictionary5.Add("BALLOON TURKEY", "LMABH.");
		dictionary5.Add("CANDY APPLE", "LMABI.");
		dictionary5.Add("CARAMEL APPLE", "LMABJ.");
		dictionary5.Add("PIE SLICE", "LMABK.");
		dictionary5.Add("LADLE", "LMABL.");
		dictionary5.Add("TURKEY LEG 22", "LMABM.");
		dictionary5.Add("CORN ON THE COB", "LMABN.");
		dictionary5.Add("FINGER OLIVES", "LMABO.");
		CosmeticsLegacyV1Info.k_v1DisplayNames_to_playFabIds = dictionary5;
		Dictionary<string, int[]> dictionary6 = new Dictionary<string, int[]>();
		dictionary6.Add("LMAAC.", new int[]
		{
			default(int),
			1
		});
		dictionary6.Add("LMAAD.", new int[]
		{
			2,
			3
		});
		dictionary6.Add("LMAAE.", new int[]
		{
			4,
			5
		});
		dictionary6.Add("LMAAK.", new int[]
		{
			6,
			7
		});
		dictionary6.Add("LMAAL.", new int[]
		{
			8,
			9
		});
		dictionary6.Add("LMAAF.", new int[]
		{
			10
		});
		dictionary6.Add("LBABE.", new int[]
		{
			11,
			12
		});
		dictionary6.Add("LBABD.", new int[]
		{
			13,
			14
		});
		dictionary6.Add("LMAAG.", new int[]
		{
			15
		});
		dictionary6.Add("LMAAH.", new int[]
		{
			16
		});
		dictionary6.Add("LMAAO.", new int[]
		{
			17
		});
		dictionary6.Add("LBABB.", new int[]
		{
			18
		});
		dictionary6.Add("LBAAP.", new int[]
		{
			19
		});
		dictionary6.Add("LBAAS.", new int[]
		{
			20
		});
		dictionary6.Add("LBAAT.", new int[]
		{
			21
		});
		dictionary6.Add("LBAAY.", new int[]
		{
			22
		});
		dictionary6.Add("LBABC.", new int[]
		{
			23
		});
		dictionary6.Add("LBAAW.", new int[]
		{
			24
		});
		dictionary6.Add("LBAAV.", new int[]
		{
			25
		});
		dictionary6.Add("LBABF.", new int[]
		{
			26
		});
		dictionary6.Add("LBAAX.", new int[]
		{
			27
		});
		dictionary6.Add("LBAAK.", new int[]
		{
			28
		});
		dictionary6.Add("LBABG.", new int[]
		{
			29
		});
		dictionary6.Add("LBAAO.", new int[]
		{
			30
		});
		dictionary6.Add("LMAAA.", new int[]
		{
			31
		});
		dictionary6.Add("LMAAB.", new int[]
		{
			32
		});
		dictionary6.Add("LMAAM.", new int[]
		{
			33
		});
		dictionary6.Add("LMAAN.", new int[]
		{
			34
		});
		dictionary6.Add("LBAAR.", new int[]
		{
			35
		});
		dictionary6.Add("LMAAQ.", new int[]
		{
			36
		});
		dictionary6.Add("LMAAP.", new int[]
		{
			37
		});
		dictionary6.Add("LMAAR.", new int[]
		{
			38
		});
		dictionary6.Add("LMAAS.", new int[]
		{
			39
		});
		dictionary6.Add("LMABD.", new int[]
		{
			40
		});
		dictionary6.Add("LMAAT.", new int[]
		{
			41
		});
		dictionary6.Add("LMABA.", new int[]
		{
			42
		});
		dictionary6.Add("LMAAW.", new int[]
		{
			43
		});
		dictionary6.Add("LMAAX.", new int[]
		{
			44
		});
		dictionary6.Add("LMAAY.", new int[]
		{
			45
		});
		dictionary6.Add("LMAAZ.", new int[]
		{
			46
		});
		dictionary6.Add("LMABC.", new int[]
		{
			47
		});
		dictionary6.Add("LMABE.", new int[]
		{
			48
		});
		dictionary6.Add("LMABF.", new int[]
		{
			49
		});
		dictionary6.Add("LMABI.", new int[]
		{
			50
		});
		dictionary6.Add("LMABJ.", new int[]
		{
			51
		});
		dictionary6.Add("LMABH.", new int[]
		{
			52
		});
		dictionary6.Add("LMABG.", new int[]
		{
			53
		});
		dictionary6.Add("LMABL.", new int[]
		{
			54
		});
		dictionary6.Add("LMABM.", new int[]
		{
			55
		});
		dictionary6.Add("LMABK.", new int[]
		{
			56
		});
		dictionary6.Add("LMABN.", new int[]
		{
			57
		});
		dictionary6.Add("LMABS.", new int[]
		{
			58
		});
		dictionary6.Add("LMABR.", new int[]
		{
			59
		});
		dictionary6.Add("LMABT.", new int[]
		{
			60
		});
		dictionary6.Add("LMABP.", new int[]
		{
			61
		});
		dictionary6.Add("LMABQ.", new int[]
		{
			62
		});
		dictionary6.Add("LMABU.", new int[]
		{
			63
		});
		dictionary6.Add("LMABW.", new int[]
		{
			64
		});
		dictionary6.Add("LMABX.", new int[]
		{
			65
		});
		dictionary6.Add("LMACB.", new int[]
		{
			66
		});
		dictionary6.Add("LMACC.", new int[]
		{
			67
		});
		dictionary6.Add("LMACD.", new int[]
		{
			68
		});
		dictionary6.Add("LMACI.", new int[]
		{
			69
		});
		dictionary6.Add("LMACJ.", new int[]
		{
			70
		});
		dictionary6.Add("LMACL.", new int[]
		{
			71
		});
		dictionary6.Add("LMACR.", new int[]
		{
			72
		});
		dictionary6.Add("LMACQ.", new int[]
		{
			73
		});
		dictionary6.Add("LMACS.", new int[]
		{
			74
		});
		dictionary6.Add("LMACP.", new int[]
		{
			75
		});
		dictionary6.Add("LMACT.", new int[]
		{
			76
		});
		dictionary6.Add("LMACV.", new int[]
		{
			77
		});
		dictionary6.Add("LMACW.", new int[]
		{
			78
		});
		dictionary6.Add("LMACY.", new int[]
		{
			79
		});
		dictionary6.Add("LMADA.", new int[]
		{
			80
		});
		dictionary6.Add("LMADB.", new int[]
		{
			81
		});
		dictionary6.Add("LMADD.", new int[]
		{
			82
		});
		dictionary6.Add("LMADE.", new int[]
		{
			83
		});
		dictionary6.Add("LMADH.", new int[]
		{
			84
		});
		dictionary6.Add("LMADJ.", new int[]
		{
			85
		});
		dictionary6.Add("LMADK.", new int[]
		{
			86
		});
		dictionary6.Add("LMADL.", new int[]
		{
			87
		});
		dictionary6.Add("LMADM.", new int[]
		{
			88
		});
		dictionary6.Add("LMADN.", new int[]
		{
			89
		});
		dictionary6.Add("LMADQ.", new int[]
		{
			90
		});
		dictionary6.Add("LMADR.", new int[]
		{
			91
		});
		dictionary6.Add("LMADS.", new int[]
		{
			92
		});
		dictionary6.Add("LMADV.", new int[]
		{
			93
		});
		dictionary6.Add("LMADW.", new int[]
		{
			94
		});
		dictionary6.Add("LMADX.", new int[]
		{
			95
		});
		dictionary6.Add("LMADZ.", new int[]
		{
			96
		});
		dictionary6.Add("LMAEA.", new int[]
		{
			97
		});
		dictionary6.Add("LMAEB.", new int[]
		{
			98
		});
		dictionary6.Add("LMAEC.", new int[]
		{
			99
		});
		dictionary6.Add("LMAED.", new int[]
		{
			100
		});
		dictionary6.Add("LMAEF.", new int[]
		{
			101
		});
		dictionary6.Add("LMAEG.", new int[]
		{
			102
		});
		dictionary6.Add("LMAEH.", new int[]
		{
			103
		});
		dictionary6.Add("LMADY.", new int[]
		{
			104
		});
		dictionary6.Add("LMAEK.", new int[]
		{
			105
		});
		dictionary6.Add("LMAEL.", new int[]
		{
			106
		});
		dictionary6.Add("LMAEM.", new int[]
		{
			107
		});
		dictionary6.Add("LMAEN.", new int[]
		{
			108
		});
		dictionary6.Add("LMAEP.", new int[]
		{
			109
		});
		dictionary6.Add("LMAEQ.", new int[]
		{
			110
		});
		dictionary6.Add("LMAES.", new int[]
		{
			111
		});
		dictionary6.Add("LMAEU.", new int[]
		{
			112
		});
		dictionary6.Add("LMAER.", new int[]
		{
			113
		});
		dictionary6.Add("LMAET.", new int[]
		{
			114
		});
		dictionary6.Add("LMAFH.", new int[]
		{
			115
		});
		dictionary6.Add("LMAFA.", new int[]
		{
			116
		});
		dictionary6.Add("LMAFB.", new int[]
		{
			117
		});
		dictionary6.Add("LMAFC.", new int[]
		{
			118
		});
		dictionary6.Add("LMAFD.", new int[]
		{
			119
		});
		dictionary6.Add("LMAFE.", new int[]
		{
			120
		});
		dictionary6.Add("LMAFF.", new int[]
		{
			121
		});
		dictionary6.Add("LMAFI.", new int[]
		{
			122
		});
		dictionary6.Add("LMAFG.", new int[]
		{
			123
		});
		dictionary6.Add("LMAFJ.", new int[]
		{
			124
		});
		dictionary6.Add("LMAFL.", new int[]
		{
			125
		});
		dictionary6.Add("LMAFM.", new int[]
		{
			126
		});
		dictionary6.Add("LMAFO.", new int[]
		{
			127
		});
		dictionary6.Add("LMAFP.", new int[]
		{
			128
		});
		dictionary6.Add("LMAFR.", new int[]
		{
			129
		});
		dictionary6.Add("LMAFS.", new int[]
		{
			130
		});
		dictionary6.Add("LMAFQ.", new int[]
		{
			131
		});
		dictionary6.Add("LMAFT.", new int[]
		{
			132
		});
		dictionary6.Add("LMAFU.", new int[]
		{
			133
		});
		dictionary6.Add("LMAFV.", new int[]
		{
			134
		});
		dictionary6.Add("LMAFW.", new int[]
		{
			135
		});
		dictionary6.Add("LMAFZ.", new int[]
		{
			136
		});
		dictionary6.Add("LMAGA.", new int[]
		{
			137
		});
		dictionary6.Add("LMAGC.", new int[]
		{
			138
		});
		dictionary6.Add("LMAGB.", new int[]
		{
			139
		});
		dictionary6.Add("LMAGF.", new int[]
		{
			140
		});
		dictionary6.Add("LMAGG.", new int[]
		{
			141
		});
		dictionary6.Add("LMAGI.", new int[]
		{
			142
		});
		dictionary6.Add("LMAGK.", new int[]
		{
			143
		});
		dictionary6.Add("LMAGL.", new int[]
		{
			144
		});
		dictionary6.Add("LMAGN.", new int[]
		{
			145
		});
		dictionary6.Add("LMAGO.", new int[]
		{
			146
		});
		dictionary6.Add("LMAGQ.", new int[]
		{
			147
		});
		dictionary6.Add("LMAGZ.", new int[]
		{
			148
		});
		dictionary6.Add("LMAGS.", new int[]
		{
			149
		});
		dictionary6.Add("LMAGV.", new int[]
		{
			150
		});
		dictionary6.Add("LMAGW.", new int[]
		{
			151
		});
		dictionary6.Add("LMAGY.", new int[]
		{
			152
		});
		dictionary6.Add("LMAHA.", new int[]
		{
			153
		});
		dictionary6.Add("LMAHB.", new int[]
		{
			154
		});
		dictionary6.Add("LMAHD.", new int[]
		{
			155
		});
		dictionary6.Add("LMAHE.", new int[]
		{
			156
		});
		dictionary6.Add("LMAHF.", new int[]
		{
			157
		});
		dictionary6.Add("LMAHG.", new int[]
		{
			158
		});
		dictionary6.Add("LMAHI.", new int[]
		{
			159
		});
		dictionary6.Add("LMAHJ.", new int[]
		{
			160
		});
		dictionary6.Add("LMAHK.", new int[]
		{
			161
		});
		dictionary6.Add("LMAHO.", new int[]
		{
			162
		});
		dictionary6.Add("LMAHM.", new int[]
		{
			163
		});
		dictionary6.Add("LMAHN.", new int[]
		{
			164
		});
		dictionary6.Add("LMAHP.", new int[]
		{
			165
		});
		dictionary6.Add("LMAHS.", new int[]
		{
			166
		});
		dictionary6.Add("LMAHT.", new int[]
		{
			167
		});
		dictionary6.Add("LMAHU.", new int[]
		{
			168
		});
		dictionary6.Add("LMAHV.", new int[]
		{
			169
		});
		dictionary6.Add("LMAHZ.", new int[]
		{
			170
		});
		dictionary6.Add("LMAIA.", new int[]
		{
			171
		});
		dictionary6.Add("LMAHW.", new int[]
		{
			172
		});
		dictionary6.Add("LMAHY.", new int[]
		{
			173
		});
		dictionary6.Add("LMAHX.", new int[]
		{
			174
		});
		dictionary6.Add("LMAII.", new int[]
		{
			175
		});
		dictionary6.Add("LMAIH.", new int[]
		{
			176
		});
		dictionary6.Add("LMAIJ.", new int[]
		{
			177
		});
		dictionary6.Add("LMAIK.", new int[]
		{
			178
		});
		dictionary6.Add("LMAIL.", new int[]
		{
			179
		});
		dictionary6.Add("LMAIN.", new int[]
		{
			180
		});
		dictionary6.Add("LMAIQ.", new int[]
		{
			181
		});
		dictionary6.Add("LMAIS.", new int[]
		{
			182
		});
		dictionary6.Add("LMAIT.", new int[]
		{
			183
		});
		dictionary6.Add("LMAIU.", new int[]
		{
			184
		});
		dictionary6.Add("LMAIX.", new int[]
		{
			185
		});
		dictionary6.Add("LMAIW.", new int[]
		{
			186
		});
		dictionary6.Add("LMAIV.", new int[]
		{
			187
		});
		dictionary6.Add("LMAIY.", new int[]
		{
			188
		});
		dictionary6.Add("LMAIZ.", new int[]
		{
			189
		});
		dictionary6.Add("LMAJA.", new int[]
		{
			190
		});
		dictionary6.Add("LMAJB.", new int[]
		{
			191
		});
		dictionary6.Add("LMAJC.", new int[]
		{
			192
		});
		dictionary6.Add("LMAJD.", new int[]
		{
			193
		});
		dictionary6.Add("LMAJE.", new int[]
		{
			194
		});
		dictionary6.Add("LMAJF.", new int[]
		{
			195
		});
		dictionary6.Add("LMAJH.", new int[]
		{
			196
		});
		dictionary6.Add("LMAJI.", new int[]
		{
			197
		});
		dictionary6.Add("LMAJJ.", new int[]
		{
			198
		});
		dictionary6.Add("LMAJN.", new int[]
		{
			199
		});
		dictionary6.Add("LMAJK.", new int[]
		{
			200
		});
		dictionary6.Add("LMAJL.", new int[]
		{
			201
		});
		dictionary6.Add("LMAJM.", new int[]
		{
			202
		});
		dictionary6.Add("LMAJS.", new int[]
		{
			203
		});
		dictionary6.Add("LMAJT.", new int[]
		{
			204
		});
		dictionary6.Add("LMAJU.", new int[]
		{
			205
		});
		dictionary6.Add("LMAJW.", new int[]
		{
			206
		});
		dictionary6.Add("LMAJX.", new int[]
		{
			207
		});
		dictionary6.Add("LMAJZ.", new int[]
		{
			208
		});
		dictionary6.Add("LMAKA.", new int[]
		{
			209
		});
		dictionary6.Add("LMAKB.", new int[]
		{
			210
		});
		dictionary6.Add("LMAJV.", new int[]
		{
			211
		});
		dictionary6.Add("Slingshot", new int[]
		{
			212
		});
		dictionary6.Add("HIGH TECH SLINGSHOT", new int[]
		{
			213
		});
		dictionary6.Add("LMABB.", new int[]
		{
			214
		});
		dictionary6.Add("LMABV.", new int[]
		{
			215
		});
		dictionary6.Add("LMACU.", new int[]
		{
			216
		});
		dictionary6.Add("LMADC.", new int[]
		{
			217
		});
		dictionary6.Add("LMADU.", new int[]
		{
			218
		});
		dictionary6.Add("LMAGJ.", new int[]
		{
			219
		});
		dictionary6.Add("LMAGR.", new int[]
		{
			220
		});
		dictionary6.Add("LMAIG.", new int[]
		{
			221
		});
		dictionary6.Add("LMAJQ.", new int[]
		{
			222
		});
		dictionary6.Add("LMAJP.", new int[]
		{
			223
		});
		CosmeticsLegacyV1Info._k_playFabId_to_bodyDockPositions_allObjects_indexes = dictionary6;
	}

	// Token: 0x04001504 RID: 5380
	public const int k_bodyDockPositions_allObjects_length = 224;

	// Token: 0x04001505 RID: 5381
	private static readonly Dictionary<string, string> k_special;

	// Token: 0x04001506 RID: 5382
	private static readonly Dictionary<string, string> k_packs;

	// Token: 0x04001507 RID: 5383
	private static readonly Dictionary<string, string> k_oldPacks;

	// Token: 0x04001508 RID: 5384
	private static readonly Dictionary<string, string> k_unused;

	// Token: 0x04001509 RID: 5385
	private static readonly Dictionary<string, string> k_v1DisplayNames_to_playFabIds;

	// Token: 0x0400150A RID: 5386
	private static readonly Dictionary<string, int[]> _k_playFabId_to_bodyDockPositions_allObjects_indexes;
}
