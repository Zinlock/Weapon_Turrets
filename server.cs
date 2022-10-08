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
exec("./main.cs");

exec("./Turret_Base.cs");
exec("./Turret_Barrels.cs");
exec("./Weapon_TurretPulse.cs");
exec("./Weapon_TurretPlasma.cs");
exec("./Weapon_TurretMortar.cs");
exec("./Weapon_TurretVulcan.cs");
