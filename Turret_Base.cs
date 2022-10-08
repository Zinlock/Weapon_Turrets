datablock PlayerData(Turret_TribalBaseStand : PlayerStandardArmor) // root rootClose open close
{
	paintable = 1;

	isTurretArmor = true;
	isTurretHead = false;
	TurretHeadData = Turret_TribalBaseArms;
	TurretPersistent = true;

	disabledLevel = 0.8;

	renderFirstPerson = false;
	emap = false;

	className = Armor;
	shapeFile = "./dts/baseturret_heavy.dts";

	maxDamage = 300;
	mass = 500000;

	drag = 1;
	density = 5;

	thirdPersonOnly = 1;
	
	rideable = true;
	canRide = false;

	protectPassengersBurn   = true;
	protectPassengersRadius = true;
	protectPassengersDirect = true;
	
	useCustomPainEffects = true;
	PainHighImage = "";
	PainMidImage  = "";
	PainLowImage  = "";
	painSound     = "";
	deathSound    = "";

	runForce = 20 * 500000;
	maxForwardSpeed = 0;
	maxForwardCrouchSpeed = 0;
	maxBackwardSpeed = 0;
	maxBackwardCrouchSpeed = 0;
	maxSideSpeed = 0;
	maxSideCrouchSpeed = 0;
	jumpForce = 0;

	boundingBox = vectorScale("2.25 2.25 2.25", 4);
	crouchBoundingBox = vectorScale("2.25 2.25 2.25", 4);

	UIName = "Tribal Base Turret";
};

datablock PlayerData(Turret_TribalBaseArms : Turret_TribalBaseStand) // root idle look
{
	isTurretHead = true;
	TurretProjectile = -1;
	TurretLookRange = 128;
	TurretLookTime = 250;
	TurretLookMask = $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType;
	TurretThinkTime = 100;
	TurretDefaultImage = Turret_TribalPulseImage;

	shapeFile = "./dts/baseturret_heavy_arms.dts";

	maxDamage = 1;

	boundingBox = vectorScale("0.5 0.5 0.5", 4);
	crouchBoundingBox = vectorScale("0.5 0.5 0.5", 4);

	UIName = "";
};

function Turret_TribalBaseStand::turretOnDisabled(%db, %obj)
{
	Parent::turretOnDisabled(%db, %obj);

	cancel(%obj.idle);	
	cancel(%obj.tbi1);
	cancel(%obj.tbi2);
	
	%obj.playThread(0, root);
	%obj.playThread(1, root);

	%obj.turretHead.playThread(0, idle);
	%obj.turretHead.playThread(1, root);
}

function Turret_TribalBaseStand::turretOnDestroyed(%db, %obj)
{
	Parent::turretOnDestroyed(%db, %obj);

	cancel(%obj.fire);	
	cancel(%obj.idle);
	cancel(%obj.tbi1);
	cancel(%obj.tbi2);

	%obj.playThread(0, rootClose);
	%obj.playThread(1, root);
	%obj.setNodeColor("ALL", "0.15 0.15 0.15 1");

	%obj.turretHead.playThread(0, idle);
	%obj.turretHead.playThread(1, root);
	%obj.turretHead.setNodeColor("ALL", "0.15 0.15 0.15 1");

	%obj.turretHead.lastImage = %obj.turretHead.getMountedImage(0);
	%obj.turretHead.unmountImage(0);
}

function Turret_TribalBaseStand::turretOnRecovered(%db, %obj)
{
	Parent::turretOnRecovered(%db, %obj);
	
	%obj.playThread(0, root);
	%obj.playThread(1, root);
	%obj.setNodeColor("ALL", "1 1 1 1");

	%obj.turretHead.playThread(0, idle);
	%obj.turretHead.playThread(1, root);
	%obj.turretHead.setNodeColor("ALL", "1 1 1 1");

	%obj.turretHead.mountImage(%obj.turretHead.lastImage, 0);
}

function Turret_TribalBaseStand::turretOnRepaired(%db, %obj)
{
	%obj.turretHead.idle = %obj.turretHead.schedule(0, tbIdleReset);

	Parent::turretOnRepaired(%db, %obj);
}

function Turret_TribalBaseStand::onAdd(%db, %obj)
{
	if(!isObject(%obj.client))
	{
		%obj.turretHead = new AIPlayer(th)
		{
			datablock = %db.turretHeadData;
			position = %obj.getPosition();
			turretBase = %obj;
			sourceClient = %obj.sourceClient;
			sourceObject = %obj.sourceObject;
			triggerTeam = false;
			triggerHeal = false;
			isTribalBaseTurret = true;
		};

		%obj.mountObject(%obj.turretHead, 0);

		%obj.turretHead.setControlObject(%obj.turretHead);

		%obj.setNodeColor("ALL", "1 1 1 1");
		%obj.turretHead.setNodeColor("ALL", "1 1 1 1");

		%obj.isBot = true;
	}

	Parent::onAdd(%db, %obj);
}

function Turret_TribalBaseStand::onRemove(%db, %obj)
{
	if(isObject(%obj.turretHead))
		%obj.turretHead.delete();

	Parent::onRemove(%db, %obj);
}

function Turret_TribalBaseStand::onDriverLeave(%db, %obj, %src)
{
	if(%src == %obj.turretHead)
	{
		if(isObject(%obj) && isObject(%src))
			%obj.schedule(0, mountObject, %src, 0);
	}

	Parent::onDriverLeave(%db, %obj, %src);
}

function Turret_TribalBaseArms::onAdd(%db, %obj)
{
	Parent::onAdd(%db, %obj);

	if(isObject(%db.TurretDefaultImage))
		%obj.mountImage(%db.TurretDefaultImage, 0);
		
	%obj.idle = %obj.schedule(2000, tbIdleReset);
}

function Turret_TribalBaseStand::turretCanMount(%db, %pl, %img)
{
	if(!%img.isTribalBaseBarrel)
		return false;

	return Parent::turretCanMount(%db, %pl, %img);
}

function Turret_TribalBaseStand::turretCanTrigger(%db, %pl, %target)
{
	%r = Parent::turretCanTrigger(%db, %pl, %target);

	if(%r)
	{
		if(%pl.triggerHeal)
			return %target.getDamagePercent() > 0.0;
	}
	
	return %r;
}

function Turret_TribalBaseArms::turretOnTargetFound(%db, %pl, %target)
{
	if(isEventPending(%pl.idle))
	{
		cancel(%pl.idle);
		
		%img = %pl.getMountedImage(0);
		%pl.fire = %pl.schedule(%img.triggerQuickTime, setImageTrigger, 0, 1);
	}
	else
	{
		cancel(%pl.tbi1);
		cancel(%pl.tbi2);

		%img = %pl.getMountedImage(0);
		%pl.playAudio(1, %img.triggerSound);
		%pl.turretBase.playThread(1, open);
		%pl.turretBase.schedule(500, playThread, 0, root);
		%pl.playThread(0, root);
		%pl.fire = %pl.schedule(%img.triggerTime, setImageTrigger, 0, 1);
	}
}

function Turret_TribalBaseArms::turretOnTargetLost(%db, %pl, %target)
{
	cancel(%pl.fire);
	%pl.setImageTrigger(0, 0);

	%pl.idle = %pl.schedule(2000, tbIdleReset);
}

function Turret_TribalBaseArms::turretOnTargetTick(%db, %pl, %target)
{
	%img = %pl.getMountedImage(0);
	%pos = %img.getAimPoint(%pl, 0, %target); //ProjectilePredict(%pl.getMuzzlePoint(0), %img.projectileSpeed, %target.getCenterPos(), %target.getVelocity(), %img.projectileGravity);

	%pl.aimVector = vectorNormalize(vectorSub(%pos, %pl.getMuzzlePoint(0)));
	%pl.setAimPointHack(%pos);
}

function AIPlayer::tbIdleReset(%pl)
{
	%pl.stopAudio(1);
	%pl.playThread(0, idle);
	%pl.setAimPointHack(vectorAdd(%pl.getEyePoint(), vectorScale(%pl.turretBase.getForwardVector(), 10)));
	%pl.tbi1 = %pl.schedule(650, setTransform, "0 0 0 0 0 1 0");
	%pl.turretBase.playThread(1, close);
	%pl.tbi2 = %pl.turretBase.schedule(650, playThread, 0, rootClose);
}