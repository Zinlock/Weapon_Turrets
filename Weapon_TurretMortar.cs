datablock AudioProfile(Turret_TribalMortarFireSound)
{
	fileName = "./wav/mortar_fire.wav";
	description = AudioDefault3D;
	preload = true;
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
	
	projectile = TW_FusionMortarProjectile;
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