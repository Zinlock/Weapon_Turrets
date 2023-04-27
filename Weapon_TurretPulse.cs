datablock AudioProfile(Turret_TribalPulseTriggerSound)
{
	fileName = "./wav/base_pulse_triggerN.wav";
	description = AudioDefault3D;
	preload = true;
};

datablock AudioProfile(Turret_TribalPulseFireSound)
{
	fileName = "./wav/pulse_fireN.wav";
	description = AudioDefault3D;
	preload = true;
};

datablock AudioProfile(Turret_TribalPulseImpactSound)
{
	fileName = "./wav/pulse_impactN.wav";
	description = AudioDefault3D;
	preload = true;
};

datablock ParticleData(Turret_TribalPulseExplosionParticle)
{
	dragCoefficient		= 3.5;
	windCoefficient		= 3.5;
	gravityCoefficient	= 0;
	inheritedVelFactor	= -0.25;
	constantAcceleration	= 0.0;
	lifetimeMS		= 750;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 100.0;
	spinRandomMin		= -100.0;
	spinRandomMax		= 100.0;
	useInvAlpha		= false;
	animateTexture		= false;

	textureName		= "base/data/particles/star1";

	colors[0]     = "1 1 1 0.5";
	colors[1]     = "0.0 0.5 0.9 0.9";
	colors[2]     = "0.025 0.05 0.1 0.2";
	colors[3]     = "0.025 0.05 0.1 0.0";

	sizes[0]	= 3.2;
	sizes[1]	= 2.2;
	sizes[2]	= 1.2;
	sizes[3]	= 0.0;

	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 0.8;
	times[3]	= 1.0;
};

datablock ParticleEmitterData(Turret_TribalPulseExplosionEmitter)
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
	particles = "Turret_TribalPulseExplosionParticle";
};

datablock ParticleData(Turret_TribalPulseParticle)
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

	sizes[0]	= 0.7;
	sizes[1]	= 0.5;
	sizes[2]	= 0.2;
	sizes[3]	= 0.0;

	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 0.8;
	times[3]	= 1.0;
};

datablock ParticleEmitterData(Turret_TribalPulseEmitter)
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
	particles = "Turret_TribalPulseParticle";
};

datablock ExplosionData(Turret_TribalPulseExplosion)
{
	explosionShape = "Add-Ons/Weapon_Rocket_Launcher/explosionSphere1.dts";
	soundProfile = Turret_TribalPulseImpactSound;

	lifeTimeMS = 350;

	particleEmitter = Turret_TribalPulseExplosionEmitter;
	particleDensity = 100;
	particleRadius = 0.0;

	faceViewer     = true;
	explosionScale = "1 1 1";

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

datablock ProjectileData(Turret_TribalPulseProjectile)
{
	projectileShapeName = "base/data/shapes/empty.dts";
	directDamage        = 10;
	directDamageType = $DamageType::AE;
	radiusDamageType = $DamageType::AE;
	impactImpulse	   = 300;
	verticalImpulse	   = 100;
	explosion           = Turret_TribalPulseExplosion;
	particleEmitter     = Turret_TribalPulseEmitter;

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
	lightRadius = 2.5;
	lightColor  = "0.0 0.0 1.0";

	vehicleDamageMult = 2;

	uiName = "";
};

function Turret_TribalPulseProjectile::damage(%this,%obj,%col,%fade,%pos,%normal)
{
	%damageType = $DamageType::Direct;
	if(%this.DirectDamageType)
		%damageType = %this.DirectDamageType;

	%scale = getWord(%obj.getScale(), 2);
	%directDamage = %this.directDamage * %scale;

	if(%col.getType() & $TypeMasks::PlayerObjectType && !%col.getDataBlock().isTurretArmor)
		%col.damage(%obj, %pos, %directDamage, %damageType);
	else
		%col.damage(%obj, %pos, %directDamage * %this.vehicleDamageMult, %damageType);
}

datablock ItemData(Turret_TribalPulseItem)
{
	category = "TurretBarrel";
	className = "TurretBarrel";

	shapeFile = "./dts/baseturret_pulse.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "TB: Anti Air";
	iconName = "./ico/pulse";
	doColorShift = true;
	colorShiftColor = "0.9 0.9 0.9 1";

	image = Turret_BarrelPlaceImage;
	canDrop = true;

	isTurretBarrel = true;
	turretImage = Turret_TribalPulseImage;
	turretTitle = "Anti Air Barrel";
	turretDesc = "Fast, but weak<br>Can't target grounded players or vehicles<br>Deals bonus damage to flying vehicles";
};

datablock ItemData(Turret_TribalPulseBoxItem)
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

	uiName = "TB: Anti Air Kit";
	iconName = Turret_TribalPulseItem.iconName;
	doColorShift = true;
	colorShiftColor = Turret_BoxPlaceImage.colorShiftColor;

	image = Turret_BoxPlaceImage;
	canDrop = true;

	isTurretBox = true;
	turretImage = Turret_TribalPulseItem.turretImage;
	turretData = Turret_TribalDeployableStand;
	turretUseHead = true;
	turretTitle = "Anti Air Turret";
	turretDesc = Turret_TribalPulseItem.turretDesc;
};

datablock ShapeBaseImageData(Turret_TribalPulseImage)
{
	mountPoint = 0;
	className = "TurretImage";

	emap = 0;

	shapeFile = "./dts/baseturret_pulse.dts";
	item = Turret_TribalPulseItem;

	isTribalBaseBarrel = true;

	doColorShift = true;
	colorShiftColor = "0.9 0.9 0.9 1.0";

	rotation = eulerToMatrix("-90 0 0");

	triggerSound = Turret_TribalPulseTriggerSound;
	fireSound = Turret_TribalPulseFireSound;

	triggerTime = 750;
	triggerQuickTime = 400;
	triggerDist = 175;
	triggerWalk = false;  // triggers on grounded players
	triggerWalkTime = 1000;  // time before target counts as walking
	triggerJet = true;  // triggers on jetting players
	triggerJetTime = 1000;  // time before target counts as jetting
	triggerGround = false;  // triggers on grounded vehicles
	triggerAir = true;  // triggers on flying vehicles
	triggerCloak = false; // triggers on cloaked objects
	triggerTeam = false; // triggers on friendlies
	triggerHeal = false; // triggers on damaged players/vehicles
	
	projectile = Turret_TribalPulseProjectile;
	projectileSpread = 0.25;
	projectileCount = 1;
	projectileSpeed = 200;
	projectileTolerance = 15;
	projectileArc = false;

	stateName[0] = "activate";
	stateSequence[0] = "activate";
	stateTimeoutValue[0] = 1.5;
	stateTransitionOnTimeout[0] = "ready";
	stateSound[0] = Turret_TribalBarrelMountSound;

	stateName[1] = "ready";
	stateSequence[1] = "root";
	stateTransitionOnTriggerDown[1] = "fire1";

	stateName[2] = "fire1";
	stateScript[2] = "onFire1";
	stateTransitionOnTimeout[2] = "fire2";
	stateTimeoutValue[2] = 0.3;
	stateSequence[2] = "fireA";
	
	stateName[3] = "fire2";
	stateScript[3] = "onFire2";
	stateTransitionOnTimeout[3] = "delay";
	stateTimeoutValue[3] = 0.3;
	stateSequence[3] = "fireB";

	stateName[4] = "delay";
	stateTransitionOnTimeout[4] = "ready";
	stateTimeoutValue[4] = 0.1;
	stateTransitionOnTriggerDown[4] = "fire1";
	stateTransitionOnTriggerUp[4] = "ready";
};

function Turret_TribalPulseImage::onFire1(%img, %obj, %slot)
{
	%src2 = %obj.sourceObject;
	%cli2 = %obj.sourceClient;
	
	%src = %obj.turretBase;
	%cli = %obj.turretBase;

	%vec = %obj.getMuzzleVector(%slot);

	if(mRadToDeg(mAcos(vectorDot(%vec, %obj.aimVector))) < %img.projectileTolerance)
		%vec = %obj.aimVector;

	%shells = ProjectileFire(%img.projectile, %obj.getEyePoint(), %vec, %img.projectileSpread, %img.projectileCount, %slot, %src, %cli, %img.projectileSpeed);

	if(isObject(%cli2))
	{
		for(%i = 0; %i < getWordCount(%shells); %i++)
		{
			%shell = getWord(%shells, %i);
			%shell.schedule(0, sourceHack, %src2, %cli2);
		}
	}

	if(isObject(%img.fireSound))
	{
		%obj.stopAudio(0);
		%obj.playAudio(0, %img.fireSound);
	}

	return %shells;
}

function Turret_TribalPulseImage::onFire2(%img, %obj, %slot) { Turret_TribalPulseImage::onFire1(%img, %obj, %slot); }