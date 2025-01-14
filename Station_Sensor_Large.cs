datablock AudioProfile(Sensor_IdleSound)
{
	fileName = "./wav/sensor_hum.wav";
	description = AudioClosestLooping3D;
	preload = true;
};

datablock PlayerData(Station_SensorLarge : Turret_TribalBaseStand) // root use
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

	shapeFile = "./dts/station_sensor_large.dts";

	maxDamage = 250;

	rechargeRate = 5 / 31.25;
	maxEnergy = 100;
	energyShield = 1;
	energyScale = 1.5;
	energyDelay = 2;

	idleSound = Sensor_IdleSound;

	boundingBox = vectorScale("4 4 9.5", 4);
	crouchBoundingBox = vectorScale("4 4 9.5", 4);

	sensorRange = 250;

	UIName = "T2: Base Large Sensor";
};

function Station_SensorLarge::turretOnPowerLost(%db, %obj)
{
	Turret_TribalBaseGenerator::turretOnPowerLost(%db, %obj);
	%obj.playThread(3, root);
}

function Station_SensorLarge::turretOnPowerRestored(%db, %obj)
{
	Turret_TribalBaseGenerator::turretOnPowerRestored(%db, %obj);
	%obj.playThread(3, idle);
}

function Station_SensorLarge::turretOnDisabled(%db, %obj, %src)
{
	Turret_TribalBaseGenerator::turretOnDisabled(%db, %obj, %src);
	%obj.playThread(3, root);
}

function Station_SensorLarge::turretOnDestroyed(%db, %obj, %src)
{
	Turret_TribalBaseGenerator::turretOnDestroyed(%db, %obj, %src);
}

function Station_SensorLarge::turretOnRecovered(%db, %obj, %src)
{
	Turret_TribalBaseGenerator::turretOnRecovered(%db, %obj, %src);
}

function Station_SensorLarge::turretOnRepaired(%db, %obj, %src)
{
	Turret_TribalBaseGenerator::turretOnRepaired(%db, %obj, %src);
	%obj.playThread(3, idle);
}

function AIPlayer::twSensor(%pl)
{
	%db = %pl.getDataBlock();

	if(!%pl.isDisabled && %pl.isPowered)
	{
		%pos = %pl.getHackPosition();
		initContainerRadiusSearch(%pos, %db.sensorRange, $TypeMasks::PlayerObjectType);
		while(isObject(%col = containerSearchNext()))
		{
			if(%col == %pl || %col.getDamagePercent() >= 1.0 || %col.getDataBlock().isTurretArmor)
				continue;

			if(turretIsFriendly(%pl, %col))
				continue;

			%cpos = %col.getHackPosition();

			if(vectorDist(%pl.getPosition(), %col.getPosition()) > %db.sensorRange || isObject(containerRayCast(%pos, %cpos, $Turret_WallMask)))
				continue;

			%col.twSensorPing();
		}
	}
}

function Player::twSensorPing(%pl)
{
	if(%pl.isSensorPinged)
		return;

	cancel(%pl.pingCleanup);

	%pl.isSensorPinged = true;

	if(isObject(%cl = %pl.Client) && isObject(%obj = %cl.STB_Beacon))
		%obj.STB_ScopeToTeam(%cl);

	%pl.pingCleanup = %pl.schedule(1000, twSensorUnPing);
}

function Player::twSensorUnPing(%pl)
{
	if(!%pl.isSensorPinged)
		return;

	cancel(%pl.pingCleanup);

	%pl.isSensorPinged = false;

	if(isObject(%cl = %pl.client) && isObject(%obj = %cl.STB_Beacon))
		%obj.STB_ScopeToTeam(%cl);
}

function Station_SensorLarge::turretOnIdleTick(%db, %pl)
{
	%pl.twSensor();

	Parent::turretOnIdleTick(%db, %pl);
}

function Station_SensorLarge::onAdd(%db, %obj)
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

function Station_SensorLarge::turretCanMount(%db, %pl, %img)
{
	return false;
}

package TWSensorRadar
{
	function GameConnection::MMGCanScope(%cl, %obj)
	{
		%obj.alwaysVisibleTo[%cl] = %obj.isSensorPinged;
		
		return Parent::MMGCanScope(%cl, %obj);
	}

	function GameConnection::STB_IsScopeAlways(%cl)
	{
		if(%cl.player.isSensorPinged)
			return true;
		
		return Parent::STB_IsScopeAlways(%cl);
	}

	function Armor::onRemove(%db, %pl)
	{
		if(isObject(%obj = %pl.client.STB_Beacon))
			%obj.scopeAlways = false;

		return Parent::onRemove(%db, %pl);
	}
};
activatePackage(TWSensorRadar);