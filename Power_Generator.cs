datablock AudioProfile(Turret_GeneratorDestroyedSound)
{
	fileName = "./wav/base_generator_destroy.wav";
	description = AudioDefault3D;
	preload = true;
};

datablock PlayerData(Turret_TribalBaseGenerator : PlayerStandardArmor)
{
	paintable = 1;

	isTurretArmor = true;
	isTurretHead = true;
	TurretPersistent = true;
	TurretLookRange = 0;
	TurretLookMask = 0;
	TurretLookTime = 200;
	TurretThinkTime = 200;
	
	rechargeRate = 5 / 31.25;
	maxEnergy = 100;
	energyShield = 1.0;
	energyShape = Turret_EnergyShieldShape;
	energyScale = 1.5;
	energyDelay = 2;
	energySound = Turret_ShieldDamagedSound;
	energyBreakSound = Turret_ShieldDestroyedSound;
	
	powerLostEmitter[0] = Turret_TribalNoPowerEmitter;
	powerLostEmitter[1] = Turret_TribalNoPowerEmitter2;

	disabledEmitter[0] = Turret_TribalDisabledEmitter;
	disabledEmitter[1] = Turret_TribalNoPowerEmitter;
	disabledEmitter[2] = Turret_TribalNoPowerEmitter2;

	destroyedEmitter[0] = Turret_TribalDisabledEmitter;
	destroyedEmitter[1] = Turret_TribalNoPowerEmitter2;
	destroyedExplosion = Turret_TribalDestroyedProjectile;
	destroyedSound = Turret_GeneratorDestroyedSound;

	idleSound = Turret_GeneratorIdleSound;

	isPowerGenerator = true;
	generatorPower = 2;

	disabledLevel = 0.8;

	renderFirstPerson = false;
	emap = false;

	className = Armor;
	shapeFile = "./dts/power_generator.dts";

	maxDamage = 200;
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

	boundingBox = vectorScale("2.5 2.5 2.5", 4);
	crouchBoundingBox = vectorScale("2.5 2.5 2.5", 4);

	UIName = "T2: Base Generator";
};

function Turret_TribalBaseGenerator::turretOnPowerLost(%db, %obj)
{
	%obj.stopAudio(3);
	
	Parent::turretOnPowerLost(%db, %obj);
}

function Turret_TribalBaseGenerator::turretOnPowerRestored(%db, %obj)
{
	%obj.playAudio(3, %db.idleSound);
	
	Parent::turretOnPowerRestored(%db, %obj);
}

function Turret_TribalBaseGenerator::turretOnDisabled(%db, %obj, %src)
{
	%obj.stopAudio(3);

	Parent::turretOnDisabled(%db, %obj, %src);
}

function Turret_TribalBaseGenerator::turretOnDestroyed(%db, %obj, %src)
{
	%obj.setNodeColor("ALL", "0.15 0.15 0.15 1");

	Parent::turretOnDestroyed(%db, %obj, %src);
}

function Turret_TribalBaseGenerator::turretOnRecovered(%db, %obj, %src)
{
	%obj.setNodeColor("ALL", "1 1 1 1");

	Parent::turretOnRecovered(%db, %obj, %src);
}

function Turret_TribalBaseGenerator::turretOnRepaired(%db, %obj, %src)
{
	%obj.playAudio(3, %db.idleSound);

	Parent::turretOnRepaired(%db, %obj, %src);
}

function Turret_TribalBaseGenerator::onAdd(%db, %obj)
{
	if(!isObject(%obj.client))
	{
		%obj.isTribalBaseGenerator = true;
		%obj.setNodeColor("ALL", "1 1 1 1");		
		%obj.playAudio(3, %db.idleSound);
	}

	Parent::onAdd(%db, %obj);
}

function Turret_TribalBaseGenerator::turretCanMount(%db, %pl, %img)
{
	return false;
}