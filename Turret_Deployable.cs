datablock PlayerData(Turret_TribalDeployableStand : Turret_TribalBaseStand) // root rootClose open close
{
	TurretHeadData = Turret_TribalDeployableArms;

	energyShield = 0;

	destroyedEmitter[0] = Turret_TribalDisabledEmitter;
	destroyedEmitter[1] = Turret_TribalNoPowerEmitter2;
	destroyedExplosion = Turret_TribalDestroyedProjectile;
	destroyedSound = Turret_TribalDestroyedSound;

	disabledLevel = 1.0;

	shapeFile = "./dts/baseturret_light.dts";

	maxDamage = 200;

	boundingBox = vectorScale("2.25 2.25 2.25", 4);
	crouchBoundingBox = vectorScale("2.25 2.25 2.25", 4);

	UIName = "Tribal Deployable Turret";
};

datablock PlayerData(Turret_TribalDeployableArms : Turret_TribalDeployableStand) // root idle look
{
	isTurretHead = true;
	TurretProjectile = -1;
	TurretLookRange = 300;
	TurretLookTime = 250;
	TurretLookMask = $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType;
	TurretThinkTime = 100;
	TurretDefaultImage = Turret_TribalPulseImage;

	shapeFile = "./dts/baseturret_light_arms.dts";

	maxDamage = 1;

	boundingBox = vectorScale("0.5 0.5 0.5", 4);
	crouchBoundingBox = vectorScale("0.5 0.5 0.5", 4);

	UIName = "";
};

function Turret_TribalDeployableStand::turretOnDisabled(%db, %obj, %src)
{
	Turret_TribalBaseStand::turretOnDisabled(%db, %obj, %src);
}

function Turret_TribalDeployableStand::turretOnDestroyed(%db, %obj, %src)
{
	Turret_TribalBaseStand::turretOnDestroyed(%db, %obj, %src);
}

function Turret_TribalDeployableStand::turretOnRecovered(%db, %obj, %src)
{
	Turret_TribalBaseStand::turretOnRecovered(%db, %obj, %src);
}

function Turret_TribalDeployableStand::turretOnRepaired(%db, %obj, %src)
{
	Turret_TribalBaseStand::turretOnRepaired(%db, %obj, %src);
}

function Turret_TribalDeployableArms::turretOnPowerLost(%db, %obj)
{
	Turret_TribalBaseArms::turretOnPowerLost(%db, %obj);
}

function Turret_TribalDeployableArms::turretOnPowerRestored(%db, %obj)
{
	Turret_TribalBaseArms::turretOnPowerRestored(%db, %obj);
}

function Turret_TribalDeployableStand::onAdd(%db, %obj)
{
	Turret_TribalBaseStand::onAdd(%db, %obj);
}

function Turret_TribalDeployableStand::onRemove(%db, %obj)
{
	Turret_TribalBaseStand::onRemove(%db, %obj);
}

function Turret_TribalDeployableStand::onDriverLeave(%db, %obj, %src)
{
	Turret_TribalBaseStand::onDriverLeave(%db, %obj, %src);
}

function Turret_TribalDeployableArms::onAdd(%db, %obj)
{
	Turret_TribalBaseArms::onAdd(%db, %obj);
}

function Turret_TribalDeployableArms::turretCanMount(%db, %pl, %img)
{
	// return Turret_TribalBaseArms::turretCanMount(%db, %pl, %img);

	return false;
}

function Turret_TribalDeployableArms::turretCanTrigger(%db, %pl, %target)
{
	return Turret_TribalBaseArms::turretCanTrigger(%db, %pl, %target);
}

function Turret_TribalDeployableArms::turretOnTargetFound(%db, %pl, %target)
{
	Turret_TribalBaseArms::turretOnTargetFound(%db, %pl, %target);
}

function Turret_TribalDeployableArms::turretOnTargetLost(%db, %pl, %target)
{
	Turret_TribalBaseArms::turretOnTargetLost(%db, %pl, %target);
}

function Turret_TribalDeployableArms::turretOnTargetTick(%db, %pl, %target)
{
	Turret_TribalBaseArms::turretOnTargetTick(%db, %pl, %target);
}

function Turret_TribalDeployableArms::tbIdleReset(%db, %pl)
{
	%pl.stopAudio(1);
	%pl.setAimPointHack(vectorAdd(%pl.getEyePoint(), vectorScale(%pl.turretBase.getForwardVector(), 10)));
	%pl.tbi1 = %pl.schedule(650, setTransform, "0 0 0 0 0 1 0");
	%pl.turretBase.playThread(1, close);
	%pl.tbi2 = %pl.turretBase.schedule(650, playThread, 0, rootClose);
}