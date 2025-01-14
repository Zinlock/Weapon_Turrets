datablock ParticleData(T2StationTrailParticle)
{
	dragCoefficient      = 0;
	windCoefficient     = 0;
	gravityCoefficient   = 0;
	inheritedVelFactor   = 0.0;
	constantAcceleration = 0.0;
	spinRandomMin = -90;
	spinRandomMax = 90;
	lifetimeMS           = 1000;
	lifetimeVarianceMS   = 0;
	textureName          = "base/data/particles/dot";
	useInvAlpha = false;
	
	colors[0]     = "0.0 0.2 0.3 0.1";
	colors[1]     = "0.0 0.0 0.0 0";
	sizes[0]      = 0.5;
	sizes[1]      = 1.0;
};

datablock ParticleEmitterData(T2StationTrailEmitter)
{
	ejectionPeriodMS = 4;
	periodVarianceMS = 0;
	ejectionVelocity = 0;
	velocityVariance = 0.0;
	ejectionOffset   = 0.0;
	thetaMin         = 0;
	thetaMax         = 180;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = true;
	particles = "T2StationTrailParticle";
};

datablock ShapeBaseImageData(T2StationTrail1Image)
{
	shapeFile = "base/data/shapes/empty.dts";
	emap = false;

	mountPoint = 1;
	rotation = "0 0 1 0";

	stateName[0]					= "Ready";
	stateTransitionOnTimeout[0]		= "A";
	stateTimeoutValue[0]			= 0.01;

	stateName[1]					= "A";
	stateTransitionOnTimeout[1]		= "B";
	stateWaitForTimeout[1]			= True;
	stateTimeoutValue[1]			= 10;
	stateEmitter[1]					= T2StationTrailEmitter;
	stateEmitterTime[1]				= 10;

	stateName[2]					= "B";
	stateTransitionOnTimeout[2]		= "A";
	stateWaitForTimeout[2]			= True;
	stateTimeoutValue[2]			= 10;
	stateEmitter[2]					= T2StationTrailEmitter;
	stateEmitterTime[2]				= 10;
};

datablock ShapeBaseImageData(T2StationTrail2Image : T2StationTrail1Image)
{
	mountPoint = 2;
};

datablock ShapeBaseImageData(T2StationTrail3Image : T2StationTrail1Image)
{
	mountPoint = 3;
};

datablock ShapeBaseImageData(T2StationTrail4Image : T2StationTrail1Image)
{
	mountPoint = 4;
};

datablock AudioProfile(Station_DeniedSound)
{
	fileName = "./wav/station_denied.wav";
	description = AudioDefault3D;
	preload = true;
};

datablock AudioProfile(Station_IdleSound)
{
	fileName = "./wav/station_hum.wav";
	description = AudioClosestLooping3D;
	preload = true;
};

datablock AudioProfile(Station_InventoryUseSound)
{
	fileName = "./wav/station_inventory.wav";
	description = AudioDefault3D;
	preload = true;
};

datablock PlayerData(Station_InventoryPad : Turret_TribalBaseStand) // root use
{
	paintable = 1;
	defaultScale = "0.9 0.9 0.8";

	isTurretArmor = true;
	isTurretHead = true;
	TurretHeadData = "";
	TurretPersistent = true;
	TurretLookRange = 0;
	TurretLookMask = 0;
	TurretLookTime = 100;
	TurretThinkTime = 100;

	disabledLevel = 0.8;

	shapeFile = "./dts/station_inventory.dts";

	maxDamage = 150;

	rechargeRate = 10 / 31.25;
	maxEnergy = 100;
	energyShield = 1;
	energyScale = 1.5;
	energyDelay = 2;

	healthRegen = 5;
	energyRegen = 8;

	enterCooldown = 3000;
	minExitTime = 1000;

	useRadius = 3;
	leaveRadius = 2;
	escapeVelocity = 10;

	idleSound = Station_IdleSound;

	boundingBox = vectorScale("3 3 4", 4);
	crouchBoundingBox = vectorScale("3 3 4", 4);

	UIName = "T2: Base Inventory Station";
};

function Station_InventoryPad::turretOnPowerLost(%db, %obj)
{
	%obj.twInvSReset();
	Turret_TribalBaseGenerator::turretOnPowerLost(%db, %obj);
}

function Station_InventoryPad::turretOnPowerRestored(%db, %obj)
{
	Turret_TribalBaseGenerator::turretOnPowerRestored(%db, %obj);
}

function Station_InventoryPad::turretOnDisabled(%db, %obj, %src)
{
	%obj.twInvSReset();
	Turret_TribalBaseGenerator::turretOnDisabled(%db, %obj, %src);
}

function Station_InventoryPad::turretOnDestroyed(%db, %obj, %src)
{
	%obj.twInvSReset();
	Turret_TribalBaseGenerator::turretOnDestroyed(%db, %obj, %src);
}

function Station_InventoryPad::turretOnRecovered(%db, %obj, %src)
{
	Turret_TribalBaseGenerator::turretOnRecovered(%db, %obj, %src);
}

function Station_InventoryPad::turretOnRepaired(%db, %obj, %src)
{
	Turret_TribalBaseGenerator::turretOnRepaired(%db, %obj, %src);
}

function AIPlayer::twInvSEffect(%obj)
{
	serverPlay3D(Station_InventoryUseSound, %obj.getHackPosition());
	%obj.playThread(0, use);

	for(%i = 0; %i < 4; %i++)
	{
		%obj.mountImage(T2StationTrail @ %i + 1 @ Image, %i);
		cancel(%obj.s[%i]);
		%obj.s[%i] = %obj.schedule(1200, unMountImage, %i);
	}
}

function AIPlayer::twInvSUse(%obj, %pl)
{
	%obj.stationInUse = true;
	%obj.stationUser = %pl;

	%pl.usingStation = %obj;
	%pl.stationEnterTime = getSimTime();

	%pl.setTransform(vectorAdd(%obj.getPosition(), "0 0 0.1"));
	%pl.setVelocity("0 0 0");

	if(isObject(%cl = %pl.Client) && $LOSetCount !$= "")
		%cl.LOApplyLoadout(true, true);
}

function AIPlayer::twInvSReset(%obj)
{
	if(%obj.stationInUse)
	{
		if(isObject(%pl = %obj.stationUser) && %pl.getDamagePercent() < 1.0)
		{
			%pl.usingStation = -1;
			%pl.stationLeaveTime = getSimTime();
		}

		%obj.stationUser = -1;
		%obj.stationInUse = false;
	}
}

function Station_InventoryPad::turretOnIdleTick(%db, %pl)
{
	if(!%pl.isDisabled && %pl.isPowered)
	{
		if(!%pl.stationInUse)
		{
			initContainerRadiusSearch(%pl.getHackPosition(), %db.useRadius, $TypeMasks::PlayerObjectType);
			while(isObject(%col = containerSearchNext()))
			{
				if(%col == %pl)
					continue;

				if(%col.getDamagePercent() >= 1.0)
					continue;

				if(%col.isMounted())
					continue;

				if(%col.getDataBlock().isTurretArmor)
					continue;

				if(isObject(%col.usingStation) || getSimTime() - %col.stationLeaveTime < %db.enterCooldown)
					continue;

				if(!%pl.getDataBlock().turretCanSee(%pl, %col))
					continue;

				if(vectorLen(%col.getVelocity()) > %db.escapeVelocity)
					continue;

				if(vectorDist(%pl.getPosition(), %col.getPosition()) > %db.useRadius)
					continue;

				if(!turretIsFriendly(%pl, %col))
				{
					if(isObject(%col.client) && getSimTime() - %col.stationWarnTime > 3000)
					{
						%col.stationWarnTime = getSimTime();
						%col.client.play2D(Station_DeniedSound);
						%col.client.centerPrint("Your team can not use this inventory station.", 2);
					}
					continue;
				}

				%pl.twInvSUse(%col);
				%pl.twInvSEffect();
				break;
			}
		}
		else if(!isObject(%pl.stationUser) || %pl.stationUser.getDamagePercent() >= 1.0)
			%pl.twInvSReset();
		else
		{
			%user = %pl.stationUser;

			if(getSimTime() - %user.stationEnterTime >= %db.minExitTime || vectorLen(%user.getVelocity()) > %db.escapeVelocity)
			{
				%dist = vectorDist(%pl.getPosition(), %user.getPosition());

				if(%dist >= %db.leaveRadius)
				{
					%pl.twInvSReset();
					return;
				}
			}
			else
			{
				%user.setTransform(%pl.getPosition());
				%user.setVelocity("0 0 0");
			}

			if(isObject(%user.getMountedImage(0)))
			{
				%user.unMountImage(0);
				%user.schedule(50, unMountImage, 0);
				commandToClient(%user.Client, 'setScrollMode', -1);
			}

			%user.setDamageLevel(%user.getDamageLevel() - %db.healthRegen);
			%user.setEnergyLevel(%user.getEnergyLevel() + %db.energyRegen);
		}
	}

	Parent::turretOnIdleTick(%db, %pl);
}

function Station_InventoryPad::onAdd(%db, %obj)
{
	if(!isObject(%obj.client))
	{
		%obj.setNodeColor("ALL", "1 1 1 1");		
		%obj.playAudio(3, %db.idleSound);
	}

	Parent::onAdd(%db, %obj);

	%obj.setShapeName("Inventory Station", 8564862);
	%obj.setShapeNameDistance(32);
	%obj.setShapeNameColor("1 1 1");
}

function Station_InventoryPad::turretCanMount(%db, %pl, %img)
{
	return false;
}