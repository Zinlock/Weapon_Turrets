datablock DebrisData(mine_scrapCogDebris)
{
	shapeFile = "./dts/debrisCog.dts";
	lifetime = 6.0;
	spinSpeed			= 600.0;
	minSpinSpeed = -1200.0;
	maxSpinSpeed = 1200.0;
	elasticity = 0.5;
	friction = 0.2;
	numBounces = 3;
	staticOnMaxBounce = true;
	snapOnMaxBounce = false;
	fade = true;

	gravModifier = 1;
};

datablock ExplosionData(mine_scrapCogExplosion)
{
	debris = mine_scrapCogDebris;
	debrisNum = 3;
	debrisNumVariance = 3;
	debrisPhiMin = 0;
	debrisPhiMax = 360;
	debrisThetaMin = 0;
	debrisThetaMax = 180;
	debrisVelocity = 10;
	debrisVelocityVariance = 4;
};

datablock DebrisData(mine_scrapNutDebris : mine_scrapCogDebris) { shapeFile = "./dts/debrisNut.dts"; };

datablock ExplosionData(mine_scrapNutExplosion : mine_scrapCogExplosion)
{
	debris = mine_scrapNutDebris;
	debrisNum = 6;
};

datablock DebrisData(mine_scrapScrewDebris : mine_scrapCogDebris) { shapeFile = "./dts/debrisScrew.dts"; };

datablock ExplosionData(mine_scrapScrewExplosion : mine_scrapCogExplosion)
{
	debris = mine_scrapScrewDebris;
	debrisNum = 6;

	subExplosion[0] = mine_scrapCogExplosion;
};

datablock ParticleData(mine_scrapDustParticle)
{
	dragCoefficient      = 1;
	gravityCoefficient   = 0.4;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS           = 2200;
	lifetimeVarianceMS   = 100;
	textureName          = "base/data/particles/cloud";
	spinSpeed			= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	colors[0]			= "0.8 0.8 0.6 0.3";
	colors[1]			= "0.8 0.8 0.6 0.0";
	sizes[0]			= 6.25;
	sizes[1]			= 8.25;

	useInvAlpha = true;
};

datablock ParticleEmitterData(mine_scrapDustEmitter)
{
	ejectionPeriodMS	= 1;
	periodVarianceMS	= 0;
	ejectionVelocity	= 4;
	velocityVariance	= 1.0;
	ejectionOffset  	= 0.0;
	thetaMin			= 89;
	thetaMax			= 90;
	phiReferenceVel		= 0;
	phiVariance			= 360;
	overrideAdvance		= false;
	particles			= "mine_scrapDustParticle";
};

datablock ParticleData(mine_scrapChunksParticle)
{
	dragCoefficient			= 1;
	gravityCoefficient		= 6;
	inheritedVelFactor		= 0.2;
	constantAcceleration	= 0.0;
	lifetimeMS				= 700;
	lifetimeVarianceMS		= 100;
	textureName				= "base/data/particles/chunk";
	spinSpeed				= 790.0;
	spinRandomMin			= -790.0;
	spinRandomMax			= 790.0;
	colors[0]				= "0.4 0.2 0.0 0.6";
	colors[1]				= "0.4 0.2 0.0 0.0";
	sizes[0]				= 0.7;
	sizes[1]				= 0.6;

	useInvAlpha				= false;
};

datablock ParticleEmitterData(mine_scrapChunksEmitter)
{
	lifeTimeMS			= 100;
	ejectionPeriodMS	= 6;
	periodVarianceMS	= 0;
	ejectionVelocity	= 12;
	velocityVariance	= 12.0;
	ejectionOffset		= 1.0;
	thetaMin			= 0;
	thetaMax			= 180;
	phiReferenceVel		= 0;
	phiVariance			= 360;
	overrideAdvance		= false;
	particles			= "mine_scrapChunksParticle";
};

datablock ExplosionData(mine_scrapExplosion)
{
	lifeTimeMS = 150;
	explosionScale = "1 1 1";

	subExplosion[0] = mine_scrapScrewExplosion;
	subExplosion[1] = mine_scrapNutExplosion;

	particleEmitter = mine_scrapDustEmitter;
	particleDensity = 15;
	particleRadius = 0.6;

	emitter[0] = mine_scrapChunksEmitter;
	emitter[1] = grenade_concExplosionDebris2Emitter;

	impulseRadius = 0;
	impulseForce = 0;

	damageRadius = 0;
	radiusDamage = 0;
};