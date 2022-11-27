if(!$AddOnLoaded__Weapon_Gun && forceRequiredAddon("Weapon_Gun") == $Error::AddOn_NotFound)
{
	error("Weapon_Turrets Error: Required Add-On Weapon_Gun not found!");
	return;
}

function tt(%path)
{
	if(%path $= "")
		exec("./server.cs");
	else
		exec("./" @ %path @ ".cs");
}

exec("Add-Ons/Support_ShapelinesV2/server.cs");

exec("./ballistics.cs");
exec("./package.cs");
exec("./functions.cs");
exec("./events.cs");
exec("./power.cs");

exec("./Turret_Base.cs");
exec("./Turret_Deployable.cs");
exec("./Turret_Barrels.cs");
exec("./Weapon_TurretPulse.cs");
exec("./Weapon_TurretPlasma.cs");
exec("./Weapon_TurretMortar.cs"); // [?] move to extras
exec("./Weapon_TurretVulcan.cs");

exec("./Power_Generator.cs");
// exec("./Power_Panel.cs"); // todo

if($AddOn__Weapon_TribalWar && isFile("Add-Ons/Weapon_TribalWar/server.cs")) // extra barrels that require the guns
{
	forceRequiredAddon("Weapon_TribalWar");

	// exec("./Weapon_TurretCluster.cs"); // todo
	// exec("./Weapon_TurretRepair.cs"); // todo
	// exec("./Weapon_TurretCharge.cs"); // todo
	// exec("./Weapon_TurretSeeker.cs"); // todo
}