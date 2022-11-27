datablock AudioProfile(Turret_TribalMortarFireSound)
{
	fileName = "./wav/mortar_fire.wav";
	description = AudioDefault3D;
	preload = true;
};

datablock AudioProfile(Turret_TribalMortarImpactSound)
{
	fileName = "./wav/mortar_impact.wav";
	description = AudioDefault3D;
	preload = true;
};

datablock ParticleData(Turret_TribalMortarSmokeParticle)
{
	dragCoefficient		= 3.5;
	windCoefficient		= 3.5;
	gravityCoefficient	= 0;
	inheritedVelFactor	= 0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 2500;
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

datablock ParticleEmitterData(Turret_TribalMortarSmokeEmitter)
{
	ejectionPeriodMS = 2;
	periodVarianceMS = 0;
	ejectionVelocity = 26;
	velocityVariance = 12.0;
	ejectionOffset   = 0.5;
	thetaMin         = 0;
	thetaMax         = 180;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = "Turret_TribalMortarSmokeParticle";
};

datablock ParticleData(Turret_TribalMortarExplosionParticle)
{
	dragCoefficient		= 3.5;
	windCoefficient		= 3.5;
	gravityCoefficient	= 0;
	inheritedVelFactor	= 0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 1000;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 40.0;
	spinRandomMin		= -40.0;
	spinRandomMax		= 40.0;
	useInvAlpha		= false;
	animateTexture		= false;

	textureName		= "base/data/particles/cloud";

	colors[0]     = "1 1 1 0.5";
	colors[1]     = "0.1 0.9 0.0 0.9";
	colors[2]     = "0.1 0.1 0.0 0.2";
	colors[3]     = "0.1 0.1 0.0 0.0";

	sizes[0]	= 4.2;
	sizes[1]	= 6.2;
	sizes[2]	= 4.2;
	sizes[3]	= 2.0;

	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 0.8;
	times[3]	= 1.0;
};

datablock ParticleEmitterData(Turret_TribalMortarExplosionEmitter)
{
	ejectionPeriodMS = 2;
	periodVarianceMS = 0;
	ejectionVelocity = 15;
	velocityVariance = 7.0;
	ejectionOffset   = 0.0;
	thetaMin         = 0;
	thetaMax         = 180;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = "Turret_TribalMortarExplosionParticle";
};

datablock ParticleData(Turret_TribalMortarParticle)
{
	dragCoefficient		= 3.5;
	windCoefficient		= 3.5;
	gravityCoefficient	= -0.25;
	inheritedVelFactor	= 0.2;
	constantAcceleration	= 0.0;
	lifetimeMS		= 2000;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 400.0;
	spinRandomMin		= -400.0;
	spinRandomMax		= 400.0;
	useInvAlpha		= true;
	animateTexture		= false;

	textureName		= "base/data/particles/cloud";

	colors[0]     = "0.3 0.5 0.3 0.5";
	colors[1]     = "0.2 0.7 0.3 0.7";
	colors[2]     = "0.2 0.1 0.1 0.4";
	colors[3]     = "0.0 0.1 0.0 0.0";

	sizes[0]	= 0.5;
	sizes[1]	= 1.2;
	sizes[2]	= 2.5;
	sizes[3]	= 2.0;

	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 0.8;
	times[3]	= 1.0;
};

datablock ParticleEmitterData(Turret_TribalMortarEmitter)
{
	ejectionPeriodMS = 15;
	periodVarianceMS = 0;
	ejectionVelocity = 0;
	velocityVariance = 0.0;
	ejectionOffset   = 0.0;
	thetaMin         = 0;
	thetaMax         = 5;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = "Turret_TribalMortarParticle";
};

datablock ExplosionData(Turret_TribalMortarExplosion)
{
	explosionShape = "Add-Ons/Weapon_Rocket_Launcher/explosionSphere1.dts";
	soundProfile = Turret_TribalMortarImpactSound;

	lifeTimeMS = 350;

	particleEmitter = Turret_TribalMortarExplosionEmitter;
	particleDensity = 100;
	particleRadius = 4.0;

	emitter[0] = Turret_TribalMortarSmokeEmitter;

	faceViewer     = true;
	explosionScale = "1 1 1";

	shakeCamera = true;
	camShakeFreq = "1.0 1.0 1.0";
	camShakeAmp = "1.0 3.0 1.0";
	camShakeDuration = 2.5;
	camShakeRadius = 10.0;

	lightStartRadius = 4;
	lightEndRadius = 12;
	lightStartColor = "0 1 0 1";
	lightEndColor = "0 0 0 0";

	damageRadius = 12;
	radiusDamage = 90;

	impulseRadius = 14;
	impulseForce = 1500;
};

datablock ProjectileData(Turret_TribalMortarProjectile)
{
	projectileShapeName = "./dts/mortar_projectile.dts";
	directDamage        = 0;
	directDamageType = $DamageType::AE;
	radiusDamageType = $DamageType::AE;
	impactImpulse	   = 0;
	verticalImpulse	   = 0;
	explosion           = Turret_TribalMortarExplosion;
	particleEmitter     = Turret_TribalMortarEmitter;

	brickExplosionRadius = 6;
	brickExplosionImpact = false;          //destroy a brick if we hit it directly?
	brickExplosionForce  = 30;             
	brickExplosionMaxVolume = 30;          //max volume of bricks that we can destroy
	brickExplosionMaxVolumeFloating = 60;  //max volume of bricks that we can destroy if they aren't connected to the ground (should always be >= brickExplosionMaxVolume)

	explodeOnDeath = true;
	explodeOnPlayerImpact = false;

	muzzleVelocity      = 100;
	velInheritFactor    = 0;

	armingDelay         = 1000;
	lifetime            = 6000;
	fadeDelay           = 5990;
	bounceElasticity    = 0.1;
	bounceFriction       = 0.5;
	isBallistic         = true;
	gravityMod = 1.0;

	hasLight    = true;
	lightRadius = 6.5;
	lightColor  = "0 1 0";

	uiName = "";
};

datablock ItemData(Turret_TribalMortarItem : Turret_TribalPulseItem)
{
	shapeFile = "./dts/baseturret_mortar.dts";

	uiName = "TB: Mortar";
	iconName = "./ico/mortar";

	doColorShift = true;
	colorShiftColor = "0.9 0.9 0.9 1";

	turretImage = Turret_TribalMortarImage;
	turretTitle = "Mortar Barrel";
	turretDesc = "Slow, but strong<br>Can't target flying vehicles<br>Deals bonus damage to vehicles";
};

datablock ItemData(Turret_TribalMortarBoxItem)
{
	category = "TurretBarrel";
	className = "TurretBarrel";

	shapeFile = "./dts/turretbox.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "TB: Mortar Kit";
	iconName = Turret_TribalMortarItem.iconName;
	doColorShift = true;
	colorShiftColor = Turret_BoxPlaceImage.colorShiftColor;

	image = Turret_BoxPlaceImage;
	canDrop = true;

	isTurretBox = true;
	turretImage = Turret_TribalMortarItem.turretImage;
	turretData = Turret_TribalDeployableStand;
	turretUseHead = true;
	turretTitle = "Mortar Turret";
	turretDesc = Turret_TribalMortarItem.turretDesc;
};

datablock ShapeBaseImageData(Turret_TribalMortarImage : Turret_TribalPulseImage)
{
	shapeFile = "./dts/baseturret_mortar.dts";
	item = Turret_TribalMortarItem;

	doColorShift = true;
	colorShiftColor = "0.9 0.9 0.9 1.0";

	triggerSound = Turret_TribalPlasmaTriggerSound;
	fireSound = Turret_TribalMortarFireSound;

	triggerTime = 1500;
	triggerQuickTime = 650;
	triggerDist = 175;
	triggerWalk = true;
	triggerWalkTime = 0;
	triggerJet = true;
	triggerJetTime = 0;
	triggerGround = true;
	triggerAir = false;
	triggerTeam = false;
	triggerHeal = false;
	
	projectile = Turret_TribalMortarProjectile;
	projectileSpread = 0;
	projectileCount = 1;
	projectileSpeed = 50;
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
	stateTimeoutValue[2] = 1.0;
	stateSequence[2] = "fire";
	
	stateName[3] = "delay";
	stateTransitionOnTimeout[3] = "ready";
	stateTimeoutValue[3] = 1.2;
};

function Turret_TribalMortarImage::onFire(%img, %obj, %slot) { Turret_TribalPulseImage::onFire1(%img, %obj, %slot); }