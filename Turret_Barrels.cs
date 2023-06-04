datablock AudioProfile(Turret_TribalBarrelMountSound)
{
	fileName = "./wav/base_barrel_equip.wav";
	description = AudioDefault3D;
	preload = true;
};

datablock AudioProfile(Turret_TribalDeploySound)
{
	fileName = "./wav/turret_deploy.wav";
	description = AudioClose3D;
	preload = true;
};

datablock ShapeBaseImageData(Turret_BoxPlaceImage)
{
	mountPoint = 0;

	armReady = false;

	doColorShift = true;
	colorShiftColor = "0.8 0.25 0.25 1";

	emap = true;

	eyeOffset = "0 1 -0.75";
	offset = "0 0 0.25";

	shapeFile = "./dts/turretbox.dts";
	item = "";

	stateName[0] = "activate";
	stateSequence[0] = "activate";
	stateTimeoutValue[0] = 0.4;
	stateTransitionOnTimeout[0] = "ready";
	stateTransitionOnTriggerDown[0] = "trigger";

	stateName[1] = "ready";
	stateScript[1]  = "onReady";
	stateTransitionOnTriggerDown[1] = "trigger";
	stateTimeoutValue[1] = 0.1;
	stateTransitionOnTimeout[1] = "ready";

	stateName[2] = "trigger";
	stateScript[2] = "onFire";
	stateTransitionOnTriggerUp[2] = "ready";
};

datablock ItemData(Turret_BoxEffectItem)
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

	uiName = "";
	iconName = "";
	doColorShift = true;
	colorShiftColor = Turret_BoxPlaceImage.colorShiftColor;

	image = "";
	canDrop = true;
};

function Turret_BoxEffectItem::onAdd(%db, %itm)
{
	Parent::onAdd(%db, %itm);

	%itm.playThread(0, use);
	%itm.canPickup = false;
}

function Turret_BoxPlaceImage::onAltFire() {}
function Turret_BoxPlaceImage::onAltRelease() {}

function Turret_BoxPlaceImage::onMount(%this, %obj, %slot)
{
	%obj.playThread(1, "armReadyBoth");
}

function Turret_BoxPlaceImage::onReady(%this, %obj, %slot)
{
	if(!%obj.tool[%obj.currTool].isTurretBox)
		return;
	
	if(isObject(%obj.client))
		%obj.client.centerPrint("<color:FD9322><font:impact:24>" @ %obj.tool[%obj.currTool].turretTitle @ "<br><color:FFFFFF><font:arial:16>" @ %obj.tool[%obj.currTool].turretDesc @ "<br>Click to deploy", 1);
}

function Turret_BoxPlaceImage::onFire(%this, %obj, %slot)
{
	if(!%obj.tool[%obj.currTool].isTurretBox || !isObject(%cl = %obj.client))
		return;

	%vec = %obj.getForwardVector();
	%end = vectorAdd(%obj.getEyePoint(), vectorScale(%vec, 4));
	%ray = containerRayCast(%obj.getEyePoint(), %end, $Turret_WallMask, %obj);

	if(isObject(%ray))
		%pos = vectorAdd(posFromRaycast(%ray), vectorScale(normalFromRaycast(%ray), 0.5));
	else
		%pos = %end;
	
	%end2 = vectorAdd(%pos, vectorSub(%obj.getPosition(), %obj.getEyePoint()));
	%ray2 = containerRayCast(%pos, %end2, $Turret_WallMask, %obj);
	
	if(isObject(%ray2))
		%pos = vectorAdd(posFromRaycast(%ray2), vectorScale(normalFromRaycast(%ray2), 0.5));
	else
		%pos = %end2;
	
	if(isObject(%cl.deployedTurret) && %cl.deployedTurret.getDamagePercent() < 1.0)
		%cl.deployedTurret.turretKill();

	%img = %obj.tool[%obj.currTool].turretImage;
	%db = %obj.tool[%obj.currTool].turretData;
	%head = %obj.tool[%obj.currTool].turretUseHead;

	serverPlay3D(Turret_TribalDeploySound, %pos);

	%ai = new AIPlayer(DeployedTurret)
	{
		datablock = %db;
		position = %pos;

		turretImage = %img;
		
		sourceObject = %obj;
		sourceClient = %cl;
		minigame = %cl.minigame;
	};

	%ai.client = %ai;
	%ai.player = %ai;
	%ai.slyrTeam = %cl.slyrTeam;

	%ai.setTransform(%pos SPC getWords(%obj.getTransform(), 3, 6));

	MissionCleanup.add(%ai);

	%cl.deployedTurret = %ai;

	%obj.weaponCount--;
	%obj.tool[%obj.currTool] = 0;
	messageClient(%cl, 'MsgItemPickup', '', %obj.currTool, 0);
	%obj.unmountImage(%slot);
	%obj.playThread(1, root);

	%p = new Projectile()
	{
		datablock = Turret_TribalDeployedProjectile;
		initialPosition = vectorAdd(%obj.getHackPosition(), vectorScale(%vec, 1));
		initialVelocity = %vec;
	};

	%p.explode();

	%itm = new Item()
	{
		datablock = Turret_BoxEffectItem;
	};

	%itm.setCollisionTimeout(%obj);
	%itm.setTransform(vectorAdd(%obj.getHackPosition(), vectorScale(%vec, 1)) SPC getWords(%obj.getTransform(), 3, 6));
	%itm.setVelocity(vectorScale(%vec, 5));

	%time = 3000;

	%itm.schedule (%time - 1000, "startFade", 1000, 0, 1);

	%color = getWords (%itm.getDataBlock ().colorShiftColor, 0, 2);
	%itm.schedule (%time - 900, setNodeColor, "ALL", %color SPC 0.9);
	%itm.schedule (%time - 800, setNodeColor, "ALL", %color SPC 0.8);
	%itm.schedule (%time - 700, setNodeColor, "ALL", %color SPC 0.7);
	%itm.schedule (%time - 600, setNodeColor, "ALL", %color SPC 0.6);
	%itm.schedule (%time - 500, setNodeColor, "ALL", %color SPC 0.5);
	%itm.schedule (%time - 400, setNodeColor, "ALL", %color SPC 0.4);
	%itm.schedule (%time - 300, setNodeColor, "ALL", %color SPC 0.3);
	%itm.schedule (%time - 200, setNodeColor, "ALL", %color SPC 0.2);
	%itm.schedule (%time - 100, setNodeColor, "ALL", %color SPC 0.1);
	
	%itm.schedule (%time, delete);
}

datablock ShapeBaseImageData(Turret_BarrelPlaceImage)
{
	mountPoint = 0;

	armReady = false;

	emap = 0;

	shapeFile = "base/data/shapes/empty.dts";
	item = "";

	stateName[0] = "activate";
	stateSequence[0] = "activate";
	stateTimeoutValue[0] = 0.1;
	stateTransitionOnTimeout[0] = "ready";

	stateName[1] = "ready";
	stateSequence[1] = "root";
	stateScript[1]  = "onReady";
	stateTransitionOnTriggerDown[1] = "trigger";
	stateTimeoutValue[1] = 0.1;
	stateTransitionOnTimeout[1] = "ready";

	stateName[2] = "trigger";
	stateScript[2] = "onFire";
	stateTransitionOnTriggerUp[2] = "ready";
};

function Turret_BarrelPlaceImage::onAltFire() {}
function Turret_BarrelPlaceImage::onAltRelease() {}

function Turret_BarrelPlaceImage::onReady(%this, %obj, %slot)
{
	if(!%obj.tool[%obj.currTool].isTurretBarrel)
		return;
	
	if(isObject(%obj.client))
		%obj.client.centerPrint("<color:FD9322><font:impact:24>" @ %obj.tool[%obj.currTool].turretTitle @ "<br><color:FFFFFF><font:arial:16>" @ %obj.tool[%obj.currTool].turretDesc @ "<br>Click on a turret to mount", 1);
}

function Turret_BarrelPlaceImage::onFire(%this, %obj, %slot)
{
	if(!%obj.tool[%obj.currTool].isTurretBarrel)
		return;
	
	%ray = containerRayCast(%obj.getEyePoint(), vectorAdd(%obj.getEyePoint(), vectorScale(%obj.getLookVector(), 5)), $TypeMasks::PlayerObjectType | $Turret_WallMask, %obj);
	if(!isObject(%ray) || %ray.ignoreBarrelMount || %ray.getClassName() !$= "AIPlayer" || !isObject(%head = %ray.turretHead) || %ray.isDestroyed || isObject(%head.target))
		return;
	
	%img = %obj.tool[%obj.currTool].turretImage;

	if(%head.getDataBlock().isTurretHead && %head.getMountedImage(0) != %img.getId() && %head.turretCanMount(%img))
	{
		%head.mountImage(%img, 0);

		%obj.weaponCount--;
		%obj.tool[%obj.currTool] = 0;
		messageClient(%obj.client, 'MsgItemPickup', '', %obj.currTool, 0);
		%obj.unmountImage(%slot);
	}
}

function TurretBarrel::onAdd(%db, %item)
{
	Weapon::onAdd(%db, %item);

	%item.playThread(0, rootClose);
}

function TurretImage::onTargetFound(%img, %obj, %slot, %target)
{
	
}

function TurretImage::onTargetTick(%img, %obj, %slot, %target)
{
	
}

function TurretImage::onTargetLost(%img, %obj, %slot, %target)
{
	
}

function TurretImage::canTrigger(%img, %obj, %slot, %target)
{
	if((%target.isCloaked || %target.isCloaked()) && !%img.triggerCloak)
		return false;
	
	if(%img.triggerHeal && %target.getDamagePercent() <= 0.0)
		return false;

	if(vectorDist(%obj.getMuzzlePoint(%slot), %target.getCenterPos()) > %img.triggerDist)
		return false;
	
	if(!(%target.getType() & $TypeMasks::PlayerObjectType))
	{
		if(!%img.triggerAir && (%target.getClassName() $= "FlyingVehicle" && !%target.getDataBlock().isHoverVehicle || %target.getDatablock().lift > 0))
			return false;
		
		if(!%img.triggerGround && ((%target.getClassName() $= "WheeledVehicle" && %target.getDataBlock().lift <= 0) || %target.getDataBlock().isHoverVehicle))
			return false;
	}
	else
	{
		if(%target.isMounted())
			return false;
		
		if(%img.triggerJet != %img.triggerWalk)
		{
			if(!isEventPending(%obj.tjl[%target]))
				%obj.turretJetLoop(%target, %img.triggerJetTime, %img.triggerWalkTime);
			
			if(!%img.triggerJet && %obj.turretJetting(%target))
				return false;
			
			if(!%img.triggerWalk && !%obj.turretJetting(%target))
				return false;
		}
		else if(!%img.triggerJet)
			return false;
	}

	return true;
}

function TurretImage::getAimPoint(%img, %obj, %slot, %target)
{
	// return ProjectilePredict(%obj.getMuzzlePoint(%slot), %img.projectileSpeed, %target.getCenterPos(), %target.getVelocity(), %img.projectile.gravityMod);

	%grav = %img.projectile.gravityMod;
	%pos = %obj.getMuzzlePoint(%slot);
	%targ = getProjectilePosition(%target, %img.projectileSpeed, %grav, %pos, %img.projectileArc);
	%dist = vectorDist(%pos, %targ);
	%time = %dist / %img.projectileSpeed;

	%div = 8 - 8 * (1 - (vectorDot(%obj.getUpVector(), "0 0 1") + 1 / 2));

	return vectorAdd(%targ, "0 0 " @ ((getWord(%target.getObjectBox(), 5) / %div) * (%time * %grav + (1 - %grav)))); // this is dumb, but it works
}