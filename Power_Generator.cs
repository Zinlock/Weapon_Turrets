datablock PlayerData(Turret_TribalBaseGenerator : PlayerStandardArmor)
{
	paintable = 1;

	isTurretArmor = true;
	isTurretHead = false;
	TurretPersistent = true;
	TurretLookRange = 0;
	TurretLookTime = 0;
	TurretLookMask = 0;
	TurretThinkTime = 0;

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

	UIName = "Tribal Base Generator";
};

function Turret_TribalBaseGenerator::turretOnDisabled(%db, %obj, %src)
{
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
	Parent::turretOnRepaired(%db, %obj, %src);
}

function Turret_TribalBaseGenerator::onAdd(%db, %obj)
{
	if(!isObject(%obj.client))
	{
		%obj.isTribalBaseGenerator = true;
		%obj.setNodeColor("ALL", "1 1 1 1");
		%obj.isBot = true;
	}

	Parent::onAdd(%db, %obj);
}

function Turret_TribalBaseGenerator::turretCanMount(%db, %pl, %img)
{
	return false;
}