datablock AudioProfile(Turret_TribalDestroyedSound)
{
	fileName = "./wav/base_turret_destroy.wav";
	description = AudioDefault3D;
	preload = true;
};

datablock PlayerData(Turret_TribalBaseStand : PlayerStandardArmor) // root rootClose open close
{
	paintable = 1;
	defaultScale = "1.8 1.8 1.8";

	isTurretArmor = true;
	isTurretHead = false;
	TurretHeadData = Turret_TribalBaseArms;
	TurretPersistent = true;

	disabledLevel = 0.8;

	powerLostEmitter[0] = Turret_TribalNoPowerEmitter;
	powerLostEmitter[1] = Turret_TribalNoPowerEmitter2;

	disabledEmitter[0] = Turret_TribalDisabledEmitter;
	disabledEmitter[1] = Turret_TribalNoPowerEmitter;
	disabledEmitter[2] = Turret_TribalNoPowerEmitter2;

	destroyedEmitter[0] = Turret_TribalDisabledEmitter;
	destroyedEmitter[1] = Turret_TribalNoPowerEmitter2;
	destroyedExplosion = Turret_TribalDestroyedProjectile;
	destroyedSound = Turret_TribalDestroyedSound;

	renderFirstPerson = false;
	emap = false;

	className = Armor;
	shapeFile = "./dts/baseturret_heavy.dts";

	maxDamage = 200;
	mass = 500000;

	drag = 1;
	density = 5;

	rechargeRate = 12.5 / 31.25;
	maxEnergy = 100;
	energyShield = 1.0;
	energyShape = Turret_EnergyShieldShape;
	energyScale = 1.5;
	energyDelay = 2;
	energySound = Turret_ShieldDamagedSound;
	energyBreakSound = Turret_ShieldDestroyedSound;

	idleSound = Turret_BaseIdleSound;

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

	UIName = "T2: Base Turret";
};

datablock PlayerData(Turret_TribalBaseArms : Turret_TribalBaseStand) // root idle look
{
	isTurretHead = true;
	TurretProjectile = -1;
	TurretLookRange = 300;
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

function Turret_TribalBaseStand::turretOnDisabled(%db, %obj, %src)
{
	Parent::turretOnDisabled(%db, %obj, %src);

	%obj.stopAudio(3);
	
	cancel(%obj.turretHead.fire);
	cancel(%obj.turretHead.idle);	
	cancel(%obj.turretHead.tbi1);
	cancel(%obj.turretHead.tbi2);
	
	%obj.playThread(0, root);
	%obj.playThread(1, root);

	%obj.turretHead.playThread(0, idle);
	%obj.turretHead.playThread(1, root);
}

function Turret_TribalBaseStand::turretOnDestroyed(%db, %obj, %src)
{
	Parent::turretOnDestroyed(%db, %obj, %src);

	cancel(%obj.turretHead.fire);
	cancel(%obj.turretHead.idle);	
	cancel(%obj.turretHead.tbi1);
	cancel(%obj.turretHead.tbi2);

	%obj.playThread(0, rootClose);
	%obj.playThread(1, root);
	%obj.setNodeColor("ALL", "0.15 0.15 0.15 1");

	%obj.turretHead.playThread(0, idle);
	%obj.turretHead.playThread(1, root);
	%obj.turretHead.setNodeColor("ALL", "0.15 0.15 0.15 1");

	%obj.turretHead.lastImage = %obj.turretHead.getMountedImage(0);
	%obj.turretHead.unmountImage(0);
}

function Turret_TribalBaseStand::turretOnRecovered(%db, %obj, %src)
{
	Parent::turretOnRecovered(%db, %obj, %src);
	
	%obj.playThread(0, root);
	%obj.playThread(1, root);
	%obj.setNodeColor("ALL", "1 1 1 1");

	%obj.turretHead.playThread(0, idle);
	%obj.turretHead.playThread(1, root);
	%obj.turretHead.setNodeColor("ALL", "1 1 1 1");

	%obj.turretHead.mountImage(%obj.turretHead.lastImage, 0);
}

function Turret_TribalBaseStand::turretOnRepaired(%db, %obj, %src)
{
	%obj.turretHead.idle = %obj.turretHead.schedule(0, tbIdleReset);

	Parent::turretOnRepaired(%db, %obj, %src);
}

function Turret_TribalBaseArms::turretOnPowerLost(%db, %obj)
{
	%obj.turretBase.stopAudio(3);

	Parent::turretOnPowerLost(%db, %obj);
}

function Turret_TribalBaseArms::turretOnPowerRestored(%db, %obj)
{
	cancel(%obj.fire);
	cancel(%obj.idle);
	cancel(%obj.tbi1);
	cancel(%obj.tbi2);
	%obj.tbIdleReset();

	Parent::turretOnPowerRestored(%db, %obj);
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

			turretImage = %obj.turretImage;

			isTribalBaseTurret = true;
		};

		MissionCleanup.add(%obj.turretHead);

		%obj.isTribalBaseTurret = true;

		%obj.mountObject(%obj.turretHead, 0);

		%obj.turretHead.setControlObject(%obj.turretHead);

		%obj.setNodeColor("ALL", "1 1 1 1");
		%obj.turretHead.setNodeColor("ALL", "1 1 1 1");

		%obj.playAudio(3, %db.idleSound);
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
		{
			%obj.schedule(0, mountObject, %src, 0);
			%src.setControlObject(%src);
		}
	}

	Parent::onDriverLeave(%db, %obj, %src);
}

function Turret_TribalBaseArms::onAdd(%db, %obj)
{
	Parent::onAdd(%db, %obj);

	if(isObject(%obj.turretImage))
		%obj.mountImage(%obj.turretImage, 0);
	else if(isObject(%db.TurretDefaultImage))
		%obj.mountImage(%db.TurretDefaultImage, 0);
		
	%obj.idle = %obj.schedule(2000, tbIdleReset);
}

function Turret_TribalBaseArms::turretCanMount(%db, %pl, %img)
{
	if(!%img.isTribalBaseBarrel)
		return false;

	return Parent::turretCanMount(%db, %pl, %img);
}

function Turret_TribalBaseArms::turretCanTrigger(%db, %pl, %target)
{
	return Parent::turretCanTrigger(%db, %pl, %target);
}

function Turret_TribalBaseArms::turretOnTargetFound(%db, %pl, %target)
{
	Parent::turretOnTargetFound(%db, %pl, %target);

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
	Parent::turretOnTargetLost(%db, %pl, %target);

	cancel(%pl.fire);
	%pl.setImageTrigger(0, 0);

	%pl.idle = %pl.schedule(2000, tbIdleReset);
}

function Turret_TribalBaseArms::turretOnTargetTick(%db, %pl, %target)
{
	Parent::turretOnTargetTick(%db, %pl, %target);

	%img = %pl.getMountedImage(0);
	%pos = %img.getAimPoint(%pl, 0, %target); //ProjectilePredict(%pl.getMuzzlePoint(0), %img.projectileSpeed, %target.getCenterPos(), %target.getVelocity(), %img.projectileGravity);

	%pl.aimVector = vectorNormalize(vectorSub(%pos, %pl.getMuzzlePoint(0)));
	%pl.setAimPointHack(%pos);
}

function Turret_TribalBaseArms::tbIdleReset(%db, %pl)
{
	%pl.stopAudio(1);
	%pl.playThread(0, idle);
	%pl.setAimPointHack(vectorAdd(%pl.getEyePoint(), vectorScale(%pl.turretBase.getForwardVector(), 10)));
	%pl.tbi1 = %pl.schedule(650, setTransform, "0 0 0 0 0 1 0");
	%pl.turretBase.playAudio(3, %pl.turretBase.getDataBlock().idleSound);
	%pl.turretBase.playThread(1, close);
	%pl.tbi2 = %pl.turretBase.schedule(650, playThread, 0, rootClose);
}

function AIPlayer::tbIdleReset(%pl)
{
	if(!%pl.isPowered)
		return;
	
	%pl.getDataBlock().tbIdleReset(%pl);
}