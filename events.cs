function turretRegisterInputs(%type)
{
	%t = "Self fxDtsBrick" TAB %type @ " Bot" TAB "SourcePlayer Player" TAB "SourceClient GameConnection" TAB "Minigame Minigame";
	registerInputEvent("fxDtsBrick", "on" @ %type @ "Spawn",     "Self fxDtsBrick" TAB %type @ " Bot" TAB "Minigame Minigame");
	registerInputEvent("fxDtsBrick", "on" @ %type @ "Disabled",  %t);
	registerInputEvent("fxDtsBrick", "on" @ %type @ "Destroyed", %t);
	registerInputEvent("fxDtsBrick", "on" @ %type @ "Recovered", %t);
	registerInputEvent("fxDtsBrick", "on" @ %type @ "Repaired",  %t);
}

turretRegisterInputs("Station");

registerOutputEvent("fxDtsBrick", "turretMountImage", "string 200 200" TAB "bool", false);

function fxDtsBrick::turretMountImage(%brk, %name, %force)
{
	if(isObject(%brk.vehicle) && %brk.vehicle.getDataBlock().isTurretArmor)
		%brk.vehicle.turretMountImage(%name, %force);
}

registerOutputEvent("Bot", "turretMountImage", "string 200 200" TAB "bool", false);
registerOutputEvent("Bot", "turretPowerLink", "string 200 200", false);

registerOutputEvent("Bot", "turretTurn", "int -180 180 0", false);

// function Player::turretPowerLink() { }
// function Player::turretMountImage() { }

function AIPlayer::turretPowerLink(%pl, %name)
{
	%db = %pl.getDataBlock();

	if(!%db.isTurretArmor)
		return;
	
	%head = %pl.turretHead;

	if(!isObject(%head))
	{
		if(%db.isTurretHead || %db.isPowerGenerator)
			%head = %pl;
		else
			return;
	}

	if(isObject(%grp = %head.powerGroup))
		%grp.remove(%head);
	
	if(trim(%name) !$= "")
	{
		%grp = getPowerGroup(%name);
		%grp.add(%head);
	}
}

function AIPlayer::turretMountImage(%pl, %name, %force)
{
	%db = %pl.getDataBlock();

	if(!%db.isTurretArmor)
		return;

	%head = %pl.turretHead;

	if(!isObject(%head))
	{
		if(%db.isTurretHead)
			%head = %pl;
		else
			return;
	}

	%img = 0;

	for(%i = 0; %i < datablockGroup.getCount(); %i++)
	{
		%db = datablockGroup.getObject(%i);

		if(%db.isTurretBarrel && (trim(%name) $= trim(%db.turretTitle) || trim(%name) $= trim(%db.uiName)))
		{
			%img = %db.turretImage;
			break;
		}
	}

	if(!isObject(%img))
		return;
	
	if(%force || %head.turretCanMount(%img))
	{
		%head.mountImage(%img, 0);
		%head.triggerTeam = %img.triggerTeam;
		%head.triggerHeal = %img.triggerHeal;
	}
}

function AIPlayer::turretTurn(%pl, %amt)
{
	%rad = mDegToRad(%amt);

	%xform = %pl.getTransform();

	%rot = getWord(%xform, 6) + %rad;

	%pl.setTransform(setWord(%xform, 6, %rot));

	if(isObject(%pad = %pl.vPad))
	{
		%xform = %pad.getTransform();

		%xform = setWord(%xform, 3, "0");
		%xform = setWord(%xform, 4, "0");
		%xform = setWord(%xform, 5, "1");

		%rot = getWord(%xform, 6) + %rad;

		%pad.setTransform(setWord(%xform, 6, %rot));
	}
}

function fxDtsBrick::onTurret(%brk, %pl, %src, %evt)
{
	if(isObject(%src))
		%class = %src.getClassName();

	%db = %pl.getDataBlock();

	%evt = "onStation" @ %evt;

	$inputTarget_Station = %pl;
	$inputTarget_SourcePlayer = (%class $= "Player" ? %src : (%class $= "GameConnection" ? %src.player : %src.sourceObject));
	$inputTarget_SourceClient = %scl = (%class $= "GameConnection" ? %src : %src.client);
	$inputTarget_Minigame = getMinigameFromObject(%pl);
	
	if(%src == %pl)
		%brk.processInputEvent(%evt, %pl);
	else
		%brk.processInputEvent(%evt, %scl);
}