datablock AudioProfile(Turret_TribalVulcanTriggerSound)
{
	fileName = "./wav/base_vulcan_trigger.wav";
	description = AudioDefault3D;
	preload = true;
};

datablock AudioProfile(Turret_TribalVulcanFireSound)
{
	fileName = "./wav/vulcan_fire.wav";
	description = AudioDefault3D;
	preload = true;
};

datablock AudioProfile(Turret_TribalVulcanImpactSound)
{
	fileName = "./wav/vulcan_impact.wav";
	description = AudioDefault3D;
	preload = true;
};

datablock ParticleData(Turret_TribalVulcanParticle)
{
	dragCoefficient		= 3.5;
	windCoefficient		= 3.5;
	gravityCoefficient	= 0;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 500;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 400.0;
	spinRandomMin		= -400.0;
	spinRandomMax		= 400.0;
	useInvAlpha		= false;
	animateTexture		= false;

	textureName		= "base/data/particles/dot";

	colors[0]     = "1 1 0.35 0.1";
	colors[1]     = "0.8 0.9 0.0 0.3";
	colors[2]     = "0.5 0.1 0.0 0.1";
	colors[3]     = "0.5 0.1 0.0 0.0";

	sizes[0]	= 0.4;
	sizes[1]	= 0.2;
	sizes[2]	= 0.05;
	sizes[3]	= 0.0;

	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 0.8;
	times[3]	= 1.0;
};

datablock ParticleEmitterData(Turret_TribalVulcanEmitter)
{
	ejectionPeriodMS = 1;
	periodVarianceMS = 0;
	ejectionVelocity = 0;
	velocityVariance = 0.0;
	ejectionOffset   = 0.0;
	thetaMin         = 0;
	thetaMax         = 5;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = "Turret_TribalVulcanParticle";
};

datablock ExplosionData(Turret_TribalVulcanExplosion : gunExplosion)
{
	soundProfile = Turret_TribalVulcanImpactSound;
};

datablock ProjectileData(Turret_TribalVulcanProjectile)
{
	projectileShapeName = "base/data/shapes/empty.dts";
	directDamage        = 4;
	directDamageType = $DamageType::AE;
	radiusDamageType = $DamageType::AE;
	impactImpulse	   = 150;
	verticalImpulse	   = 100;
	explosion           = Turret_TribalVulcanExplosion;
	particleEmitter     = Turret_TribalVulcanEmitter;

	brickExplosionRadius = 0;
	brickExplosionImpact = false;          //destroy a brick if we hit it directly?
	brickExplosionForce  = 0;             
	brickExplosionMaxVolume = 0;          //max volume of bricks that we can destroy
	brickExplosionMaxVolumeFloating = 0;  //max volume of bricks that we can destroy if they aren't connected to the ground (should always be >= brickExplosionMaxVolume)

	explodeOnDeath = true;
	explodeOnPlayerImpact = true;

	muzzleVelocity      = 100;
	velInheritFactor    = 0;

	armingDelay         = 35;
	lifetime            = 5000;
	fadeDelay           = 4990;
	bounceElasticity    = 0.5;
	bounceFriction       = 0.20;
	isBallistic         = false;
	gravityMod = 0.0;

	hasLight    = true;
	lightRadius = 5;
	lightColor  = "1 0.4 0";

	uiName = "";
};

datablock ItemData(Turret_TribalVulcanItem : Turret_TribalPulseItem)
{
	shapeFile = "./dts/baseturret_vulcan.dts";

	uiName = "TB: Vulcan";
	iconName = "./ico/vulcan";

	doColorShift = true;
	colorShiftColor = "0.9 0.9 0.9 1";

	turretImage = Turret_TribalVulcanImage;
	turretTitle = "Vulcan Barrel";
	turretDesc = "Fast, but weak<br>Can't target flying vehicles<br>Deals bonus damage to players";
};

datablock ShapeBaseImageData(Turret_TribalVulcanImage : Turret_TribalPulseImage)
{
	shapeFile = "./dts/baseturret_vulcan.dts";

	doColorShift = true;
	colorShiftColor = "0.9 0.9 0.9 1.0";

	triggerSound = Turret_TribalVulcanTriggerSound;
	fireSound = Turret_TribalVulcanFireSound;

	triggerTime = 400;
	triggerQuickTime = 200;
	triggerDist = 125;
	triggerWalk = true;
	triggerWalkTime = 0;
	triggerJet = true;
	triggerJetTime = 0;
	triggerGround = true;
	triggerAir = false;
	triggerTeam = false;
	triggerHeal = false;
	
	projectile = Turret_TribalVulcanProjectile;
	projectileSpread = 1.5;
	projectileCount = 1;
	projectileSpeed = 125;
	projectileTolerance = 10;

	casing = GunShellDebris;
	shellExitDir        = "1.0 -0.3 1.0";
	shellExitOffset     = "0 0 0";
	shellExitVariance   = 15.0;
	shellVelocity       = 7.0;

	stateName[0] = "activate";
	stateSequence[0] = "activate";
	stateTimeoutValue[0] = 1.5;
	stateTransitionOnTimeout[0] = "ready";
	stateSound[0] = Turret_TribalBarrelMountSound;

	stateName[1] = "ready";
	stateSequence[1] = "root";
	stateTransitionOnTriggerDown[1] = "spinup";

	stateName[2] = "spinup";
	stateSequence[2] = "spinup";
	stateTimeoutValue[2] = 0.4;
	stateTransitionOnTimeout[2] = "fire";
	stateTransitionOnTriggerUp[2] = "spindown";

	stateName[3] = "fire";
	stateScript[3] = "onFire";
	stateTransitionOnTimeout[3] = "delay";
	stateTimeoutValue[3] = 0.064;
	stateSequence[3] = "fire";
	stateEjectShell[3] = true;
	
	stateName[4] = "delay";
	stateTransitionOnTimeout[4] = "ready";
	stateTimeoutValue[4] = 0.064;
	stateTransitionOnTriggerDown[4] = "fire";
	stateTransitionOnTriggerUp[4] = "spindown";

	stateName[5] = "spindown";
	stateSequence[5] = "spindown";
	stateTimeoutValue[5] = 0.4;
	stateTransitionOnTimeout[5] = "ready";
};

function Turret_TribalVulcanImage::onFire(%img, %obj, %slot) { Turret_TribalPulseImage::onFire1(%img, %obj, %slot); }