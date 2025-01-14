datablock PlayerData(Station_SensorMedium : Turret_TribalBaseStand) // root use
{
	paintable = 1;
	defaultScale = "1 1 1";

	isTurretArmor = true;
	isTurretHead = true;
	TurretHeadData = "";
	TurretPersistent = true;
	TurretLookRange = 0;
	TurretLookMask = 0;
	TurretLookTime = 200;
	TurretThinkTime = 200;

	disabledLevel = 0.8;

	shapeFile = "./dts/station_sensor_medium.dts";

	maxDamage = 250;

	rechargeRate = 5 / 31.25;
	maxEnergy = 100;
	energyShield = 1;
	energyScale = 1.5;
	energyDelay = 2;

	idleSound = Sensor_IdleSound;

	boundingBox = vectorScale("4 4 5", 4);
	crouchBoundingBox = vectorScale("4 4 5", 4);

	sensorRange = 100;

	UIName = "T2: Base Medium Sensor";
};

function Station_SensorMedium::turretOnPowerLost(%db, %obj)
{
	Turret_TribalBaseGenerator::turretOnPowerLost(%db, %obj);
	%obj.playThread(3, root);
}

function Station_SensorMedium::turretOnPowerRestored(%db, %obj)
{
	Turret_TribalBaseGenerator::turretOnPowerRestored(%db, %obj);
	%obj.playThread(3, idle);
}

function Station_SensorMedium::turretOnDisabled(%db, %obj, %src)
{
	Turret_TribalBaseGenerator::turretOnDisabled(%db, %obj, %src);
	%obj.playThread(3, root);
}

function Station_SensorMedium::turretOnDestroyed(%db, %obj, %src)
{
	Turret_TribalBaseGenerator::turretOnDestroyed(%db, %obj, %src);
}

function Station_SensorMedium::turretOnRecovered(%db, %obj, %src)
{
	Turret_TribalBaseGenerator::turretOnRecovered(%db, %obj, %src);
}

function Station_SensorMedium::turretOnRepaired(%db, %obj, %src)
{
	Turret_TribalBaseGenerator::turretOnRepaired(%db, %obj, %src);
	%obj.playThread(3, idle);
}

function Station_SensorMedium::turretOnIdleTick(%db, %pl)
{
	%pl.twSensor();

	Parent::turretOnIdleTick(%db, %pl);
}

function Station_SensorMedium::onAdd(%db, %obj)
{
	if(!isObject(%obj.client))
	{
		%obj.setNodeColor("ALL", "1 1 1 1");		
		%obj.playAudio(3, %db.idleSound);

		%obj.schedule(100, playThread, 3, idle);
	}

	Parent::onAdd(%db, %obj);

	%obj.setShapeName("Base Sensor", 8564862);
	%obj.setShapeNameDistance(32);
	%obj.setShapeNameColor("1 1 1");
}

function Station_SensorMedium::turretCanMount(%db, %pl, %img)
{
	return false;
}