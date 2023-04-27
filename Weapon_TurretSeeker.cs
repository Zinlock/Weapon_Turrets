datablock AudioProfile(Turret_TribalSeekerTriggerSound)
{
	fileName = "./wav/base_seeker_trigger.wav";
	description = AudioDefault3D;
	preload = true;
};

datablock AudioProfile(Turret_TribalSeekerFireSound)
{
	fileName = "./wav/seeker_fire.wav";
	description = AudioDefault3D;
	preload = true;
};

datablock ItemData(Turret_TribalSeekerItem : Turret_TribalPulseItem)
{
	shapeFile = "./dts/baseturret_seeker.dts";

	uiName = "TB: Heat Seeker";
	iconName = "./ico/seeker";

	doColorShift = true;
	colorShiftColor = "0.9 0.9 0.9 1";

	turretImage = Turret_TribalSeekerImage;
	turretTitle = "Heat Seeker Barrel";
	turretDesc = "Slow, but strong<br>Can't target grounded players<br>Deals bonus damage to vehicles";
};

datablock ItemData(Turret_TribalSeekerBoxItem)
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

	uiName = "TB: Seeker Kit";
	iconName = Turret_TribalSeekerItem.iconName;
	doColorShift = true;
	colorShiftColor = Turret_BoxPlaceImage.colorShiftColor;

	image = Turret_BoxPlaceImage;
	canDrop = true;

	isTurretBox = true;
	turretImage = Turret_TribalSeekerItem.turretImage;
	turretData = Turret_TribalDeployableStand;
	turretUseHead = true;
	turretTitle = "Seeker Turret";
	turretDesc = Turret_TribalSeekerItem.turretDesc;
};

datablock ShapeBaseImageData(Turret_TribalSeekerImage : Turret_TribalPulseImage)
{
	shapeFile = "./dts/baseturret_seeker.dts";
	item = Turret_TribalSeekerItem;

	doColorShift = true;
	colorShiftColor = "0.9 0.9 0.9 1.0";

	triggerSound = Turret_TribalSeekerTriggerSound;
	fireSound = Turret_TribalSeekerFireSound;

	triggerTime = 1500;
	triggerQuickTime = 1000;
	triggerDist = 200;
	triggerWalk = false;
	triggerWalkTime = 1000;
	triggerJet = true;
	triggerJetTime = 1500;
	triggerGround = true;
	triggerAir = true;
	triggerTeam = false;
	triggerHeal = false;
	
	projectile = TW_HeatSeekerProjectile;
	projectileSpread = 0;
	projectileCount = 1;
	projectileSpeed = 75;
	projectileTolerance = 1;
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
	stateTimeoutValue[3] = 4.0;
};

function Turret_TribalSeekerImage::onTargetTick(%img, %obj, %slot, %target)
{
	HeatLockOnPrint(-1, %target, 0, 0);
}

function Turret_TribalSeekerImage::onFire(%img, %obj, %slot)
{	
	%shell = Turret_TribalPulseImage::onFire1(%img, %obj, %slot);

	%shell.target = %obj.target;
}