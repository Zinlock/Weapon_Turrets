datablock AudioProfile(Turret_TribalSentryTriggerSound)
{
	fileName = "./wav/base_sentry_trigger.wav";
	description = AudioDefault3D;
	preload = true;
};

datablock AudioProfile(Turret_TribalSentryFireSound)
{
	fileName = "./wav/sentry_fire.wav";
	description = AudioDefault3D;
	preload = true;
};

datablock AudioProfile(Turret_TribalSentryImpactSound)
{
	fileName = "./wav/sentry_impact.wav";
	description = AudioDefault3D;
	preload = true;
};

datablock ParticleData(Turret_TribalSentryExplosionParticle)
{
	dragCoefficient		= 3.5;
	windCoefficient		= 3.5;
	gravityCoefficient	= 0;
	inheritedVelFactor	= -0.25;
	constantAcceleration	= 0.0;
	lifetimeMS		= 500;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 100.0;
	spinRandomMin		= -100.0;
	spinRandomMax		= 100.0;
	useInvAlpha		= false;
	animateTexture		= false;

	textureName		= "base/data/particles/star1";

	colors[0]     = "0 0.2 0.5 0.0";
	colors[1]     = "0.0 0.5 0.9 0.5";
	colors[2]     = "0.025 0.05 0.1 0.2";
	colors[3]     = "0.025 0.05 0.1 0.0";

	sizes[0]	= 0.2;
	sizes[1]	= 1.2;
	sizes[2]	= 2.2;
	sizes[3]	= 4.0;

	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 0.8;
	times[3]	= 1.0;
};

datablock ParticleEmitterData(Turret_TribalSentryExplosionEmitter)
{
	ejectionPeriodMS = 2;
	periodVarianceMS = 0;
	ejectionVelocity = 2;
	velocityVariance = 2.0;
	ejectionOffset   = 0.0;
	thetaMin         = 0;
	thetaMax         = 180;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = "Turret_TribalSentryExplosionParticle";
};

datablock ParticleData(Turret_TribalSentryParticle)
{
	dragCoefficient		= 3.5;
	windCoefficient		= 3.5;
	gravityCoefficient	= 0;
	inheritedVelFactor	= -0.25;
	constantAcceleration	= 0.0;
	lifetimeMS		= 550;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 400.0;
	spinRandomMin		= -400.0;
	spinRandomMax		= 400.0;
	useInvAlpha		= false;
	animateTexture		= false;

	textureName		= "base/data/particles/dot";

	colors[0]     = "1 1 1 0.1";
	colors[1]     = "0.0 0.5 0.9 0.3";
	colors[2]     = "0.025 0.05 0.1 0.1";
	colors[3]     = "0.025 0.05 0.1 0.0";

	sizes[0]	= 0.1;
	sizes[1]	= 0.2;
	sizes[2]	= 0.1;
	sizes[3]	= 0.0;

	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 0.8;
	times[3]	= 1.0;
};

datablock ParticleEmitterData(Turret_TribalSentryEmitter)
{
	ejectionPeriodMS = 2;
	periodVarianceMS = 0;
	ejectionVelocity = 0;
	velocityVariance = 0.0;
	ejectionOffset   = 0.0;
	thetaMin         = 0;
	thetaMax         = 5;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = "Turret_TribalSentryParticle";
};

datablock ExplosionData(Turret_TribalSentryExplosion)
{
	explosionShape = "";
	soundProfile = Turret_TribalSentryImpactSound;

	lifeTimeMS = 350;

	particleEmitter = Turret_TribalSentryExplosionEmitter;
	particleDensity = 10;
	particleRadius = 0.0;

	faceViewer     = true;
	explosionScale = "0.25 0.25 0.25";

	shakeCamera = true;
	camShakeFreq = "1.0 1.0 1.0";
	camShakeAmp = "1.0 3.0 1.0";
	camShakeDuration = 0.5;
	camShakeRadius = 10.0;

	lightStartRadius = 2;
	lightEndRadius = 5;
	lightStartColor = "0 0 1 1";
	lightEndColor = "0 0 0 0";

	damageRadius = 0;
	radiusDamage = 0;

	impulseRadius = 4;
	impulseForce = 150;
};

datablock ProjectileData(Turret_TribalSentryProjectile)
{
	projectileShapeName = "base/data/shapes/empty.dts";
	directDamage        = 8;
	directDamageType = $DamageType::AE;
	radiusDamageType = $DamageType::AE;
	impactImpulse	   = 300;
	verticalImpulse	   = 100;
	explosion           = Turret_TribalSentryExplosion;
	particleEmitter     = Turret_TribalSentryEmitter;

	brickExplosionRadius = 1;
	brickExplosionImpact = false;          //destroy a brick if we hit it directly?
	brickExplosionForce  = 10;             
	brickExplosionMaxVolume = 10;          //max volume of bricks that we can destroy
	brickExplosionMaxVolumeFloating = 10;  //max volume of bricks that we can destroy if they aren't connected to the ground (should always be >= brickExplosionMaxVolume)

	explodeOnDeath = true;
	explodeOnPlayerImpact = false;

	muzzleVelocity      = 100;
	velInheritFactor    = 0;

	armingDelay         = 35;
	lifetime            = 6000;
	fadeDelay           = 5990;
	bounceElasticity    = 0.5;
	bounceFriction       = 0.20;
	isBallistic         = false;
	gravityMod = 0.0;

	hasLight    = true;
	lightRadius = 1.5;
	lightColor  = "0.0 0.0 1.0";

	uiName = "";
};

datablock ShapeBaseImageData(Turret_TribalSentryImage : Turret_TribalPulseImage)
{
	shapeFile = "./dts/baseturret_sentry_barrel.dts";
	item = Turret_TribalSentryItem;

	rotation = eulerToMatrix("-90 0 0");

	doColorShift = true;
	colorShiftColor = "0.9 0.9 0.9 1.0";

	triggerSound = Turret_TribalSentryTriggerSound;
	fireSound = Turret_TribalSentryFireSound;

	triggerTime = 1000;
	triggerQuickTime = 600;
	triggerDist = 64;
	triggerWalk = true;
	triggerWalkTime = 0;
	triggerJet = true;
	triggerJetTime = 0;
	triggerGround = false;
	triggerAir = false;
	triggerTeam = false;
	triggerHeal = false;
	
	projectile = Turret_TribalSentryProjectile;
	projectileSpread = 0;
	projectileCount = 1;
	projectileSpeed = 100;
	projectileTolerance = 15;
	projectileArc = false;

	stateName[0] = "activate";
	stateSequence[0] = "activate";
	stateTimeoutValue[0] = 1.5;
	stateTransitionOnTimeout[0] = "ready";
	stateSound[0] = Turret_TribalBarrelMountSound;

	stateName[1] = "ready";
	stateSequence[1] = "root";
	stateTransitionOnTriggerDown[1] = "fire";

	stateName[2] = "fire";
	stateScript[2] = "onFire";
	stateTransitionOnTimeout[2] = "delay";
	stateTimeoutValue[2] = 0.5;
	stateSequence[2] = "fire";
	
	stateName[4] = "delay";
	stateTransitionOnTimeout[4] = "ready";
	stateTimeoutValue[4] = 0.1;
	stateTransitionOnTriggerDown[4] = "fire";
	stateTransitionOnTriggerUp[4] = "ready";
};

function Turret_TribalSentryImage::onFire(%img, %obj, %slot) { Turret_TribalPulseImage::onFire1(%img, %obj, %slot); }

datablock PlayerData(Turret_TribalSentryStand : Turret_TribalBaseStand)
{
	defaultScale = "1 1 1";
	TurretHeadData = Turret_TribalSentryArms;

	idleSound = "";
	
	rechargeRate = 10 / 31.25;
	maxEnergy = 75;
	energyShield = 1.0;
	energyShape = Turret_EnergyShieldShape;
	energyScale = 1.0;
	energyDelay = 2;
	energySound = Turret_ShieldDamagedSound;
	energyBreakSound = Turret_ShieldDestroyedSound;

	disabledLevel = 1.0;

	shapeFile = "./dts/baseturret_sentry.dts";

	maxDamage = 100;

	boundingBox = vectorScale("1 1 1", 4);
	crouchBoundingBox = vectorScale("1 1 1", 4);

	UIName = "T2: Base Sentry Turret";
};

datablock PlayerData(Turret_TribalSentryArms : Turret_TribalSentryStand)
{
	isTurretHead = true;
	TurretProjectile = -1;
	TurretLookRange = 64;
	TurretLookTime = 250;
	TurretLookMask = $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType;
	TurretThinkTime = 100;
	TurretDefaultImage = Turret_TribalSentryImage;

	shapeFile = "./dts/baseturret_sentry_arms.dts";

	maxDamage = 1;

	boundingBox = vectorScale("0.25 0.25 0.25", 4);
	crouchBoundingBox = vectorScale("0.25 0.25 0.25", 4);

	UIName = "";
};

function Turret_TribalSentryStand::turretOnDisabled(%db, %obj, %src)
{
	Turret_TribalBaseStand::turretOnDisabled(%db, %obj, %src);
}

function Turret_TribalSentryStand::turretOnDestroyed(%db, %obj, %src)
{
	Turret_TribalBaseStand::turretOnDestroyed(%db, %obj, %src);
}

function Turret_TribalSentryStand::turretOnRecovered(%db, %obj, %src)
{
	Turret_TribalBaseStand::turretOnRecovered(%db, %obj, %src);
}

function Turret_TribalSentryStand::turretOnRepaired(%db, %obj, %src)
{
	Turret_TribalBaseStand::turretOnRepaired(%db, %obj, %src);
}

function Turret_TribalSentryArms::turretOnPowerLost(%db, %obj)
{
	Turret_TribalBaseArms::turretOnPowerLost(%db, %obj);
}

function Turret_TribalSentryArms::turretOnPowerRestored(%db, %obj)
{
	Turret_TribalBaseArms::turretOnPowerRestored(%db, %obj);
}

function Turret_TribalSentryStand::onAdd(%db, %obj)
{
	Turret_TribalBaseStand::onAdd(%db, %obj);
}

function Turret_TribalSentryStand::onRemove(%db, %obj)
{
	Turret_TribalBaseStand::onRemove(%db, %obj);
}

function Turret_TribalSentryStand::onDriverLeave(%db, %obj, %src)
{
	Turret_TribalBaseStand::onDriverLeave(%db, %obj, %src);
}

function Turret_TribalSentryArms::onAdd(%db, %obj)
{
	Turret_TribalBaseArms::onAdd(%db, %obj);

	%obj.setShapeName("Sentry Turret", 8564862);
	%obj.setShapeNameDistance(32);
	%obj.setShapeNameColor("1 1 1");
}

function Turret_TribalSentryArms::turretCanMount(%db, %pl, %img)
{
	return false;
}

function Turret_TribalSentryArms::turretCanTrigger(%db, %pl, %target)
{
	return Turret_TribalBaseArms::turretCanTrigger(%db, %pl, %target);
}

function Turret_TribalSentryArms::turretOnTargetFound(%db, %pl, %target)
{
	Turret_TribalBaseArms::turretOnTargetFound(%db, %pl, %target);
}

function Turret_TribalSentryArms::turretOnTargetLost(%db, %pl, %target)
{
	Turret_TribalBaseArms::turretOnTargetLost(%db, %pl, %target);
}

function Turret_TribalSentryArms::turretOnTargetTick(%db, %pl, %target)
{
	Turret_TribalBaseArms::turretOnTargetTick(%db, %pl, %target);
}

function Turret_TribalSentryArms::tbIdleReset(%db, %pl)
{
	%pl.stopAudio(1);
	%pl.setAimPointHack(vectorAdd(%pl.getEyePoint(), vectorScale(%pl.turretBase.getUpVector(), -10)));
	%pl.tbi1 = %pl.schedule(650, setTransform, "0 0 0 0 0 1 0");
}