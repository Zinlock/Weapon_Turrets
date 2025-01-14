datablock ItemData(Turret_TribalChargeItem : Turret_TribalPulseItem)
{
	shapeFile = "./dts/baseturret_charge.dts";

	uiName = "TB: ELF";
	iconName = "./ico/Charge";

	doColorShift = true;
	colorShiftColor = "0.9 0.9 0.9 1";

	turretImage = Turret_TribalChargeImage;
	turretTitle = "ELF Barrel";
	turretDesc = "Drains enemy energy and health<br>Can't target vehicles";
};

datablock ItemData(Turret_TribalChargeBoxItem)
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

	uiName = "TB: ELF Kit";
	iconName = "./ico/kit";
	doColorShift = true;
	colorShiftColor = Turret_BoxPlaceImage.colorShiftColor;

	image = Turret_BoxPlaceImage;
	canDrop = true;

	isTurretBox = true;
	turretImage = Turret_TribalChargeItem.turretImage;
	turretData = Turret_TribalDeployableStand;
	turretUseHead = true;
	turretTitle = "ELF Turret";
	turretDesc = Turret_TribalChargeItem.turretDesc;
};

datablock ShapeBaseImageData(Turret_TribalChargeImage : Turret_TribalPulseImage)
{
	shapeFile = "./dts/baseturret_charge.dts";
	item = Turret_TribalChargeItem;
	boxItem = Turret_TribalChargeBoxItem;

	doColorShift = true;
	colorShiftColor = "0.9 0.9 0.9 1.0";

	triggerSound = Turret_TribalChargeTriggerSound;
	fireSound = Turret_TribalChargeFireSound;

	triggerTime = 400;
	triggerQuickTime = 200;
	triggerDist = 128;
	triggerWalk = true;
	triggerWalkTime = 0;
	triggerJet = true;
	triggerJetTime = 0;
	triggerGround = false;
	triggerAir = false;
	triggerTeam = false;
	triggerHeal = false;

	elfRange = 128;
	elfAngle = 30;
	elfDrain = 4.0;
	elfDamage = 0.25;

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
	stateTimeoutValue[3] = 0.05;
	stateSequence[3] = "fire";
	stateEjectShell[3] = true;
	
	stateName[4] = "delay";
	stateTransitionOnTimeout[4] = "ready";
	stateTimeoutValue[4] = 0.1;
	stateTransitionOnTriggerDown[4] = "fire";
	stateTransitionOnTriggerUp[4] = "spindown";

	stateName[5] = "spindown";
	stateSequence[5] = "spindown";
	stateTimeoutValue[5] = 0.4;
	stateTransitionOnTimeout[5] = "ready";
};

function Turret_TribalChargeImage::onFire(%img, %obj, %slot) { TW_ELFGunImage::onFire(%img, %obj, %slot); }