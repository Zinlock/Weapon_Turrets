datablock AudioProfile(Turret_TribalPlasmaTriggerSound)
{
	fileName = "./wav/base_plasma_trigger.wav";
	description = AudioDefault3D;
	preload = true;
};

datablock AudioProfile(Turret_TribalPlasmaFireSound)
{
	fileName = "./wav/plasma_fire.wav";
	description = AudioDefault3D;
	preload = true;
};

datablock AudioProfile(Turret_TribalPlasmaImpactSound)
{
	fileName = "./wav/plasma_impact.wav";
	description = AudioDefault3D;
	preload = true;
};

datablock ParticleData(Turret_TribalPlasmaSmokeParticle)
{
	dragCoefficient		= 3.5;
	windCoefficient		= 3.5;
	gravityCoefficient	= 0;
	inheritedVelFactor	= -0.25;
	constantAcceleration	= 0.0;
	lifetimeMS		= 1000;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 40.0;
	spinRandomMin		= -40.0;
	spinRandomMax		= 40.0;
	useInvAlpha		= true;
	animateTexture		= false;

	textureName		= "base/data/particles/cloud";

	colors[0]     = "0 0 0 0.1";
	colors[1]     = "0 0 0 0.2";
	colors[2]     = "0 0 0 0.1";
	colors[3]     = "0 0 0 0.0";

	sizes[0]	= 4.2;
	sizes[1]	= 6.2;
	sizes[2]	= 4.2;
	sizes[3]	= 2.0;

	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 0.8;
	times[3]	= 1.0;
};

datablock ParticleEmitterData(Turret_TribalPlasmaSmokeEmitter)
{
	ejectionPeriodMS = 2;
	periodVarianceMS = 0;
	ejectionVelocity = 8;
	velocityVariance = 7.0;
	ejectionOffset   = 0.0;
	thetaMin         = 0;
	thetaMax         = 180;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = "Turret_TribalPlasmaSmokeParticle";
};

datablock ParticleData(Turret_TribalPlasmaExplosionParticle)
{
	dragCoefficient		= 3.5;
	windCoefficient		= 3.5;
	gravityCoefficient	= 0;
	inheritedVelFactor	= -0.25;
	constantAcceleration	= 0.0;
	lifetimeMS		= 700;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 40.0;
	spinRandomMin		= -40.0;
	spinRandomMax		= 40.0;
	useInvAlpha		= false;
	animateTexture		= false;

	textureName		= "base/data/particles/cloud";

	colors[0]     = "1 1 1 0.5";
	colors[1]     = "0.8 0.9 0.0 0.9";
	colors[2]     = "0.5 0.1 0.0 0.2";
	colors[3]     = "0.5 0.1 0.0 0.0";

	sizes[0]	= 4.2;
	sizes[1]	= 6.2;
	sizes[2]	= 4.2;
	sizes[3]	= 2.0;

	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 0.8;
	times[3]	= 1.0;
};

datablock ParticleEmitterData(Turret_TribalPlasmaExplosionEmitter)
{
	ejectionPeriodMS = 2;
	periodVarianceMS = 0;
	ejectionVelocity = 6;
	velocityVariance = 5.0;
	ejectionOffset   = 0.0;
	thetaMin         = 0;
	thetaMax         = 180;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = "Turret_TribalPlasmaExplosionParticle";
};

datablock ParticleData(Turret_TribalPlasmaParticle)
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

	colors[0]     = "1 1 0.35 0.1";
	colors[1]     = "0.8 0.9 0.0 0.3";
	colors[2]     = "0.5 0.1 0.0 0.1";
	colors[3]     = "0.5 0.1 0.0 0.0";

	sizes[0]	= 2.2;
	sizes[1]	= 0.7;
	sizes[2]	= 0.2;
	sizes[3]	= 0.0;

	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 0.8;
	times[3]	= 1.0;
};

datablock ParticleEmitterData(Turret_TribalPlasmaEmitter)
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
	particles = "Turret_TribalPlasmaParticle";
};

datablock ExplosionData(Turret_TribalPlasmaExplosion)
{
	explosionShape = "Add-Ons/Weapon_Rocket_Launcher/explosionSphere1.dts";
	soundProfile = Turret_TribalPlasmaImpactSound;

	lifeTimeMS = 350;

	particleEmitter = Turret_TribalPlasmaExplosionEmitter;
	particleDensity = 100;
	particleRadius = 0.0;

	emitter[0] = Turret_TribalPlasmaSmokeEmitter;

	faceViewer     = true;
	explosionScale = "1 1 1";

	shakeCamera = true;
	camShakeFreq = "1.0 1.0 1.0";
	camShakeAmp = "1.0 3.0 1.0";
	camShakeDuration = 2.5;
	camShakeRadius = 10.0;

	lightStartRadius = 4;
	lightEndRadius = 12;
	lightStartColor = "1 1 0 1";
	lightEndColor = "0 0 0 0";

	damageRadius = 10;
	radiusDamage = 35;

	impulseRadius = 10;
	impulseForce = 500;
};

datablock ProjectileData(Turret_TribalPlasmaProjectile)
{
	projectileShapeName = "base/data/shapes/empty.dts";
	directDamage        = 10;
	directDamageType = $DamageType::AE;
	radiusDamageType = $DamageType::AE;
	impactImpulse	   = 300;
	verticalImpulse	   = 100;
	explosion           = Turret_TribalPlasmaExplosion;
	particleEmitter     = Turret_TribalPlasmaEmitter;

	brickExplosionRadius = 6;
	brickExplosionImpact = true;          //destroy a brick if we hit it directly?
	brickExplosionForce  = 30;             
	brickExplosionMaxVolume = 30;          //max volume of bricks that we can destroy
	brickExplosionMaxVolumeFloating = 60;  //max volume of bricks that we can destroy if they aren't connected to the ground (should always be >= brickExplosionMaxVolume)

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
	lightRadius = 4.5;
	lightColor  = "1 1 0";

	uiName = "";
};

datablock ItemData(Turret_TribalPlasmaItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./dts/baseturret_plasma.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "TB: Plasma";
	iconName = "./ico/plasma";
	doColorShift = true;
	colorShiftColor = "1 1 1 1";

	image = Turret_BarrelPlaceImage;
	canDrop = true;

	isTurretBarrel = true;
	turretImage = Turret_TribalPlasmaImage;
	turretTitle = "Plasma Barrel";
};

datablock ShapeBaseImageData(Turret_TribalPlasmaImage)
{
	mountPoint = 0;

	emap = 0;

	shapeFile = "./dts/baseturret_plasma.dts";
	item = "";

	doColorShift = true;
	colorShiftColor = "0.9 0.9 0.9 1.0";

	rotation = eulerToMatrix("-90 0 0");

	triggerSound = Turret_TribalPlasmaTriggerSound;
	fireSound = Turret_TribalPlasmaFireSound;

	triggerTime = 750;
	triggerTeam = false;
	triggerHeal = false;
	
	projectile = Turret_TribalPlasmaProjectile;
	projectileSpread = 0;
	projectileCount = 1;
	projectileSpeed = 100;

	stateName[0] = "activate";
	stateSequence[0] = "activate";
	stateTimeoutValue[0] = 1.5;
	stateTransitionOnTimeout[0] = "ready";

	stateName[1] = "ready";
	stateSequence[1] = "root";
	stateTransitionOnTriggerDown[1] = "fire";

	stateName[2] = "fire";
	stateScript[2] = "onFire";
	stateTransitionOnTimeout[2] = "delay";
	stateTimeoutValue[2] = 1.25;
	stateSequence[2] = "fire";
	
	stateName[4] = "delay";
	stateTransitionOnTimeout[4] = "ready";
	stateTimeoutValue[4] = 0.1;
	stateTransitionOnTriggerDown[4] = "fire";
	stateTransitionOnTriggerUp[4] = "ready";
};

function Turret_TribalPlasmaImage::onFire(%img, %obj, %slot)
{
	%src = %obj.sourceObject;
	%cli = %obj.sourceClient;

	if(!isObject(%cli))
	{
		%src = %obj.turretBase;
		%cli = %obj.turretBase;
	}

	ProjectileFire(%img.projectile, %obj.getMuzzlePoint(%slot), %obj.getMuzzleVector(%slot), %img.projectileSpread, %img.projectileCount, %slot, %src, %cli, %img.projectileSpeed);

	if(isObject(%img.fireSound))
	{
		%obj.stopAudio(0);
		%obj.playAudio(0, %img.fireSound);
	}
}