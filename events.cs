$t = "Self fxDtsBrick" TAB "Turret Player" TAB "SourcePlayer Player" TAB "SourceClient GameConnection" TAB "Minigame Minigame";
registerInputEvent("fxDtsBrick", "onTurretSpawn",     "Self fxDtsBrick" TAB "Turret Player" TAB "Minigame Minigame");
registerInputEvent("fxDtsBrick", "onTurretDisabled",  $t);
registerInputEvent("fxDtsBrick", "onTurretDestroyed", $t);
registerInputEvent("fxDtsBrick", "onTurretRecovered", $t);
registerInputEvent("fxDtsBrick", "onTurretRepaired",  $t);

$t = "Self fxDtsBrick" TAB "Generator Player" TAB "SourcePlayer Player" TAB "SourceClient GameConnection" TAB "Minigame Minigame";
registerInputEvent("fxDtsBrick", "onGeneratorSpawn",     "Self fxDtsBrick" TAB "Generator Player" TAB "Minigame Minigame");
registerInputEvent("fxDtsBrick", "onGeneratorDisabled",  $t);
registerInputEvent("fxDtsBrick", "onGeneratorDestroyed", $t);
registerInputEvent("fxDtsBrick", "onGeneratorRecovered", $t);
registerInputEvent("fxDtsBrick", "onGeneratorRepaired",  $t);

registerOutputEvent("fxDtsBrick", "turretMountImage", "string 200 200" TAB "bool", false);

function fxDtsBrick::turretMountImage(%brk, %name, %force)
{
	if(isObject(%brk.vehicle) && %brk.vehicle.getDataBlock().isTurretArmor)
		%brk.vehicle.turretMountImage(%name, %force);
}

registerOutputEvent("Player", "turretMountImage", "string 200 200" TAB "bool", false);
registerOutputEvent("Player", "turretPowerLink", "string 200 200", false);

function Player::turretPowerLink() { }
function Player::turretMountImage() { }

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

	talk(%name);
	talk(%head SPC %head.getMountedImage(0).getName());

	for(%i = 0; %i < datablockGroup.getCount(); %i++)
	{
		%db = datablockGroup.getObject(%i);

		if(%db.isTurretBarrel && (trim(%name) $= trim(%db.turretTitle) || trim(%name) $= trim(%db.uiName)))
		{
			%img = %db.turretImage;
			talk(%img.getName());
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

function fxDtsBrick::onTurretSpawn(%brk, %pl)
{
	$inputTarget_Turret = %pl;
	$inputTarget_Minigame = getMinigameFromObject(%pl);
	
	%brk.processInputEvent("onTurretSpawn", %pl);
}

function fxDtsBrick::onGeneratorSpawn(%brk, %pl)
{
	$inputTarget_Generator = %pl;
	$inputTarget_Minigame = getMinigameFromObject(%pl);
	
	%brk.processInputEvent("onGeneratorSpawn", %pl);
}

function fxDtsBrick::onTurret(%brk, %pl, %src, %evt)
{
	if(isObject(%src))
		%class = %src.getClassName();

	$inputTarget_Turret = %pl;
	$inputTarget_Generator = %pl;
	$inputTarget_SourcePlayer = (%class $= "Player" ? %src : (%class $= "GameConnection" ? %src.player : %src.sourceObject));
	$inputTarget_SourceClient = %scl = (%class $= "GameConnection" ? %src : %src.client);
	$inputTarget_Minigame = getMinigameFromObject(%pl);
	
	%brk.processInputEvent(%evt, %scl);
}