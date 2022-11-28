datablock AudioProfile(Turret_ShieldDamagedSound)
{
	fileName = "./wav/base_shield_damage.wav";
	description = AudioClose3D;
	preload = true;
};

datablock AudioProfile(Turret_ShieldDestroyedSound)
{
	fileName = "./wav/base_shield_destroy.wav";
	description = AudioClose3D;
	preload = true;
};

datablock AudioProfile(Turret_ShieldRecoveredSound)
{
	fileName = "./wav/base_shield_recover.wav";
	description = AudioClose3D;
	preload = true;
};

datablock AudioProfile(Turret_BaseIdleSound)
{
	fileName = "./wav/base_turret_idle.wav";
	description = AudioClosestLooping3D;
	preload = true;
};

datablock AudioProfile(Turret_GeneratorIdleSound)
{
	fileName = "./wav/base_generator_idle.wav";
	description = AudioClosestLooping3D;
	preload = true;
};

datablock StaticShapeData(Turret_EnergyShieldShape) { shapeFile = "./dts/energy_shield.dts"; };

datablock ParticleData(Turret_TribalNoPowerParticle)
{
	dragCoefficient			= 0;
	gravityCoefficient		= 0;
	inheritedVelFactor		= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS				= 50;
	lifetimeVarianceMS		= 0;
	textureName				= "./dts/bolt";
	spinSpeed				= 5.0;
	spinRandomMin			= -5.0;
	spinRandomMax			= 5.0;
	colors[0]				= "0.0 0.6 1.0 0.6";
	colors[1]				= "0.0 0.6 1.0 0.0";
	sizes[0]				= 0.9;
	sizes[1]				= 0.8;

	useInvAlpha				= false;
};

datablock ParticleEmitterData(Turret_TribalNoPowerEmitter)
{
	lifeTimeMS			= 100;
	ejectionPeriodMS	= 20;
	periodVarianceMS	= 0;
	ejectionVelocity	= 20;
	velocityVariance	= 4.0;
	ejectionOffset		= 0.5;
	thetaMin			= 0;
	thetaMax			= 180;
	phiReferenceVel		= 0;
	phiVariance			= 360;
	overrideAdvance		= true;
	orientParticles = true;
	orientOnVelocity = false;
	particles			= "Turret_TribalNoPowerParticle";
};

datablock ParticleData(Turret_TribalNoPowerParticle2)
{
	dragCoefficient			= 1;
	gravityCoefficient		= 5;
	inheritedVelFactor		= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS				= 200;
	lifetimeVarianceMS		= 0;
	textureName				= "base/data/particles/dot";
	spinSpeed				= 5.0;
	spinRandomMin			= -5.0;
	spinRandomMax			= 5.0;
	colors[0]				= "1.0 0.6 0.0 0.6";
	colors[1]				= "1.0 0.6 0.0 0.0";
	sizes[0]				= 0.1;
	sizes[1]				= 0.05;

	useInvAlpha				= false;
};

datablock ParticleEmitterData(Turret_TribalNoPowerEmitter2)
{
	lifeTimeMS			= 100;
	ejectionPeriodMS	= 10;
	periodVarianceMS	= 0;
	ejectionVelocity	= 10;
	velocityVariance	= 4.0;
	ejectionOffset		= 0.5;
	thetaMin			= 0;
	thetaMax			= 180;
	phiReferenceVel		= 0;
	phiVariance			= 360;
	overrideAdvance		= true;
	orientParticles = false;
	orientOnVelocity = false;
	particles			= "Turret_TribalNoPowerParticle2";
};

datablock ParticleData(Turret_TribalDisabledParticle)
{
	dragCoefficient			= 5;
	gravityCoefficient		= -1;
	inheritedVelFactor		= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS				= 2000;
	lifetimeVarianceMS		= 600;
	textureName				= "base/data/particles/cloud";
	spinSpeed				= 100.0;
	spinRandomMin			= -100.0;
	spinRandomMax			= 100.0;
	colors[0]				= "0.1 0.1 0.1 0.1";
	colors[1]				= "0.2 0.2 0.2 0.0";
	sizes[0]				= 2;
	sizes[1]				= 4;

	useInvAlpha				= true;
};

datablock ParticleEmitterData(Turret_TribalDisabledEmitter)
{
	lifeTimeMS			= 100;
	ejectionPeriodMS	= 14;
	periodVarianceMS	= 0;
	ejectionVelocity	= 3;
	velocityVariance	= 2.0;
	ejectionOffset		= 0.5;
	thetaMin			= 0;
	thetaMax			= 180;
	phiReferenceVel		= 0;
	phiVariance			= 360;
	overrideAdvance		= true;
	orientParticles = false;
	orientOnVelocity = false;
	particles			= "Turret_TribalDisabledParticle";
};

datablock ParticleData(Turret_TribalDestroyedSmokeParticle)
{
	dragCoefficient		= 2.5;
	windCoefficient		= 3.5;
	gravityCoefficient	= 0;
	inheritedVelFactor	= -0.25;
	constantAcceleration	= 0.0;
	lifetimeMS		= 2500;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 40.0;
	spinRandomMin		= -40.0;
	spinRandomMax		= 40.0;
	useInvAlpha		= true;
	animateTexture		= false;

	textureName		= "base/data/particles/cloud";

	colors[0]     = "0 0 0 0.05";
	colors[1]     = "0 0 0 0.1";
	colors[2]     = "0 0 0 0.05";
	colors[3]     = "0 0 0 0.0";

	sizes[0]	= 5.2;
	sizes[1]	= 7.2;
	sizes[2]	= 5.2;
	sizes[3]	= 3.0;

	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 0.8;
	times[3]	= 1.0;
};

datablock ParticleEmitterData(Turret_TribalDestroyedSmokeEmitter)
{
	ejectionPeriodMS = 5;
	periodVarianceMS = 0;
	ejectionVelocity = 9;
	velocityVariance = 8.0;
	ejectionOffset   = 0.0;
	thetaMin         = 0;
	thetaMax         = 180;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = "Turret_TribalDestroyedSmokeParticle";
};

datablock ExplosionData(Turret_TribalDestroyedExplosion)
{
	explosionShape = "Add-Ons/Weapon_Rocket_Launcher/explosionSphere1.dts";

	lifeTimeMS = 350;

	emitter[0] = Turret_TribalDestroyedSmokeEmitter;

	subExplosion[0] = mine_scrapExplosion;

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

	damageRadius = 0;
	radiusDamage = 0;

	impulseRadius = 10;
	impulseForce = 500;
};

datablock ProjectileData(Turret_TribalDestroyedProjectile)
{
	projectileShapeName = "base/data/shapes/empty.dts";
	directDamage        = 0;
	explosion           = Turret_TribalDestroyedExplosion;

	explodeOnDeath = true;
	explodeOnPlayerImpact = true;

	muzzleVelocity      = 0;
	velInheritFactor    = 0;

	armingDelay         = 0;
	lifetime            = 32;
	fadeDelay           = 32;
};