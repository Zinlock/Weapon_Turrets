datablock ItemData(Turret_TribalRepairItem : Turret_TribalPulseItem)
{
	shapeFile = "./dts/baseturret_Repair.dts";

	uiName = "TB: Repair";
	iconName = "./ico/Repair";

	doColorShift = true;
	colorShiftColor = "0.9 0.9 0.9 1";

	turretImage = Turret_TribalRepairImage;
	turretTitle = "Repair Barrel";
	turretDesc = "Heals friendly players and vehicles";
};

datablock ItemData(Turret_TribalRepairBoxItem)
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

	uiName = "TB: Repair Kit";
	iconName = Turret_TribalRepairItem.iconName;
	doColorShift = true;
	colorShiftColor = Turret_BoxPlaceImage.colorShiftColor;

	image = Turret_BoxPlaceImage;
	canDrop = true;

	isTurretBox = true;
	turretImage = Turret_TribalRepairItem.turretImage;
	turretData = Turret_TribalDeployableStand;
	turretUseHead = true;
	turretTitle = "Repair Turret";
	turretDesc = Turret_TribalRepairItem.turretDesc;
};

datablock ShapeBaseImageData(Turret_TribalRepairImage : Turret_TribalPulseImage)
{
	shapeFile = "./dts/baseturret_Repair.dts";
	item = Turret_TribalRepairItem;

	doColorShift = true;
	colorShiftColor = "0.9 0.9 0.9 1.0";

	triggerSound = Turret_TribalRepairTriggerSound;
	fireSound = Turret_TribalRepairFireSound;

	triggerTime = 400;
	triggerQuickTime = 200;
	triggerDist = 12;
	triggerWalk = true;
	triggerWalkTime = 0;
	triggerJet = true;
	triggerJetTime = 0;
	triggerGround = true;
	triggerAir = true;
	triggerTeam = true;
	triggerHeal = true;

	repairRange = 16;
	repairAngle = 45;

	minEnergy = 0.0;
	energyUse = 0.0;
	repairAmt = 0.75;
	repairAmtUtil = 0.75;

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

function Turret_TribalRepairImage::onFire(%img, %obj, %slot) { TW_RepairGunImage::onFire(%img, %obj, %slot); }