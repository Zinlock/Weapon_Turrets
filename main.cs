// main.cs:
// datablocks for turret guns
// functions for shooting stuff, tracking players etc

$Turret_TargetMask = $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::ItemObjectType | $TypeMasks::ProjectileObjectType;
$Turret_WallMask = $TypeMasks::fxBrickObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::StaticObjectType | $TypeMasks::InteriorObjectType | $TypeMasks::TerrainObjectType;

$Turret_LeadDistance = 0.125; // compensate for the bot turning speed

function turret(%pos)
{
	return new AIPlayer(testPlayer)
	{
		datablock = Turret_TribalBaseStand;
		position = %pos;
	};
}

function Armor::turretCanSee(%db, %pl, %target)
{
	%pos = %pl.getHackPosition();
	if(isObject(%target))
	{
		if($tlinedebug)
		{
			drawLine(%pos, %target.getEyePoint(), "0 1 0 0.5", 0.01).schedule(500,delete);
			drawLine(%pos, %target.getHackPosition(), "0 1 0 0.5", 0.01).schedule(500,delete);
			drawLine(%pos, %target.getPosition(), "0 1 0 0.5", 0.01).schedule(500,delete);
		}

		//try the head first
		%ray = containerRayCast(%pos, %target.getHigherPos(), $Turret_WallMask, %pl, %target);
		if(!isObject(%ray))
			return 1;
		else
		{
			//try the chest then
			%ray = containerRayCast(%pos, %target.getCenterPos(), $Turret_WallMask, %pl, %target);
			if(!isObject(%ray))
				return 1;
			else
			{
				//try the feet then
				%ray = containerRayCast(%pos, %target.getPosition(), $Turret_WallMask, %pl, %target);
				if(!isObject(%ray))
					return 1;
			}
		}
	}

	return 0;
}

function Armor::turretCanTrigger(%db, %pl, %target)
{
	if(%target.getDatablock().isTurretArmor)
		return 0;
	
	if(vectorDist(%pl.getHackPosition(), %target.getCenterPos()) > %db.TurretLookRange || !%db.turretCanSee(%pl, %target) || %pl.getDamagePercent() >= 1.0)
		return 0;
	
	if(%target.isCloaked)
		return 0;

	%tt = %pl.triggerTeam;
	
	if(isObject(%pl.sourceClient))
	{
		%dm = minigameCanDamage(%pl.sourceClient, %target);
		if(%dm != 1 && !%tt || %dm != 0 && %tt)
			return 0;
		
		%mg = %pl.sourceClient.minigame;
		if(%mg.isSlayerMinigame && isObject(%pl.sourceClient.slyrTeam))
		{
			if(%target.getType() & $TypeMasks::PlayerObjectType)
				%targ = %target.client;
			else
				%targ = %target.getControllingClient();
			
			if(isObject(%targ))
			{
				%dm = %pl.sourceClient.slyrTeam.isAlliedTeam(%targ.slyrTeam);
				if(%dm && !%tt || !%dm && %tt)
					return 0;
			}
			else if(isObject(%target.spawnBrick))
			{
				%dm = %src.slyrTeam.isAlliedTeam(%mg.teams.getTeamFromName(%target.spawnBrick.getControllingTeam()));
				if(%dm && !%tt || !%dm && %tt)
					return 0;
			}
		}
	}
	else if(isObject(%pl.turretBase.spawnBrick))
	{
		%mg = getMinigameFromObject(%pl.turretBase);
		%mg2 = getMinigameFromObject(%target);

		if(%mg != %mg2)
			return 0;
		
		if(%target.getType() & $TypeMasks::PlayerObjectType)
			%targ = %target.client;
		else
			%targ = %target.getControllingClient();
		
		if(!isObject(%targ))
			return 0;
		
		if(%mg.isSlayerMinigame)
		{
			%teamA = %mg.teams.getTeamFromName(%pl.turretBase.spawnBrick.getControllingTeam());

			if(isObject(%teamA))
			{
				if(isObject(%target.spawnBrick))
				{
					%team = %target.spawnBrick.getControllingTeam();

					if(%team != 0)
						%teamB = %mg.teams.getTeamFromName(%team);
				}
				else
					%teamB = %targ.slyrTeam;
				
				%dm = isObject(%teamB) && %teamA.isAlliedTeam(%teamB);
				if(%dm && !%tt || !%dm && %tt)
					return 0;
			}
		}
		else
		{
			%dm = minigameCanDamage(%pl.turretBase, %target);
			if(%dm != 1 && !%tt || %dm != 0 && %tt)
				return 0;
		}
	}
	else return 0;

	return 1;
}

function Armor::turretOnIdleTick(%db, %pl)
{

}

function Armor::turretOnTargetTick(%db, %pl, %target)
{
	%pl.setAimPointHack(%target.getCenterPos());
}

function Armor::turretOnTargetFound(%db, %pl, %target)
{
	
}

function Armor::turretOnTargetLost(%db, %pl, %target)
{
	
}

function Armor::turretOnDestroyed(%db, %pl)
{
	
}

function Armor::turretOnDisabled(%db, %pl)
{
	cancel(%pl.turretHead.turretIdle);
	cancel(%pl.turretHead.turretTarget);
}

function Armor::turretOnRecovered(%db, %pl)
{
	
}

function Armor::turretOnRepaired(%db, %pl)
{
	if(isObject(%pl.turretHead))
	{
		%pl.turretHead.setDamageLevel(0);
		%pl.turretHead.setControlObject(%pl.turretHead);

		%pl.turretHead.target = "";
		%pl.turretHead.targetFoundTime = "";
		%pl.turretHead.targetLostTime = "";
		%pl.turretHead.schedule(200, onTurretIdleTick);
	}
}

function AIPlayer::onTurretTargetFound(%pl, %target)
{
	%db = %pl.getDatablock();
	cancel(%pl.turretIdle);
	cancel(%pl.turretTarget);

	%pl.targetFoundTime = getSimTime();
	
	%db.turretOnTargetFound(%pl, %target);
	%pl.turretTarget = %pl.schedule(%db.TurretThinkTime, onTurretTargetTick, %target);
}

function AIPlayer::onTurretTargetLost(%pl, %target)
{
	%db = %pl.getDatablock();
	cancel(%pl.turretIdle);
	cancel(%pl.turretTarget);

	%pl.targetLostTime = getSimTime();

	%db.turretOnTargetLost(%pl, %target);
	%pl.turretIdle = %pl.schedule(%db.TurretThinkTime, onTurretIdleTick);
}

function AIPlayer::onTurretTargetTick(%pl, %target)
{
	%db = %pl.getDatablock();
	cancel(%pl.turretIdle);
	cancel(%pl.turretTarget);

	if(isObject(%target) && %db.turretCanTrigger(%pl, %target))
	{
		%db.turretOnTargetTick(%pl, %target);
		%pl.turretTarget = %pl.schedule(%db.TurretThinkTime, onTurretTargetTick, %target);
	}
	else
		%pl.onTurretTargetLost(%target);
}

function AIPlayer::onTurretIdleTick(%pl)
{
	%db = %pl.getDatablock();
	cancel(%pl.turretIdle);
	
	%db.turretOnIdleTick(%pl);

	if(getSimTime() - %pl.lastSightCheck >= %db.TurretLookTime)
	{	
		%pl.lastSightCheck = getSimTime();
		%target = %pl.turretLook(%db.TurretLookRange, %db.TurretLookMask);
		%pl.target = firstWord(%target);

		if(isObject(%target))
			%pl.onTurretTargetFound(firstWord(%target));
	}

	%pl.turretIdle = %pl.schedule(%db.TurretThinkTime, onTurretIdleTick);
}

function AIPlayer::turretRepair(%pl, %amt)
{
	if(isObject(%pl.turretBase))
		return %pl.turretBase.turretRepair(%amt);
	
	%pl.setDamageLevel(%pl.getDamageLevel() - %amt);
	%pl.turretDamageCheck();
}

function AIPlayer::turretDamageCheck(%obj)
{
	%db = %obj.getDataBlock();

	if(%obj.getDamagePercent() >= %db.destroyedLevel)
	{
		if(!%obj.isDestroyed)
		{
			%obj.isDisabled = true;
			%obj.isDestroyed = true;
			%db.turretOnDestroyed(%obj);
		}
	}
	else if(%obj.getDamagePercent() >= %db.disabledLevel)
	{
		if(!%obj.isDisabled)
		{
			%obj.isDestroyed = false;
			%obj.isDisabled = true;
			%db.turretOnDisabled(%obj);
		}
		else if(%obj.isDestroyed)
		{
			%obj.isDestroyed = false;
			%obj.isDisabled = true;
			%db.turretOnRecovered(%obj);
		}
	}
	else
	{
		if(%obj.isDestroyed || %obj.isDisabled)
		{
			%obj.isDestroyed = false;
			%obj.isDisabled = false;
			%db.turretOnRepaired(%obj);
		}
	}
}

// IMAGES //

function ProjectileFire(%db, %pos, %vec, %spd, %amt, %srcSlot, %srcObj, %srcCli, %vel)
{	
	%projectile = %db;
	%spread = %spd / 1000;
	%shellcount = %amt;

	if(%vel $= "")
		%vel = %projectile.muzzleVelocity;

	%shells = -1;

	for(%shell=0; %shell<%shellcount; %shell++)
	{
		%velocity = VectorScale(%vec, %vel);
		%x = (getRandom() - 0.5) * 10 * 3.1415926 * %spread;
		%y = (getRandom() - 0.5) * 10 * 3.1415926 * %spread;
		%z = (getRandom() - 0.5) * 10 * 3.1415926 * %spread;
		%mat = MatrixCreateFromEuler(%x @ " " @ %y @ " " @ %z);
		%velocity = MatrixMulVector(%mat, %velocity);

		%p = new Projectile()
		{
			dataBlock = %projectile;
			initialVelocity = %velocity;
			initialPosition = %pos;
			sourceObject = %srcObj;
			sourceSlot = %srcSlot;
			sourceInv = %srcObj.currTool;
			client = %srcCli;
		};
		MissionCleanup.add(%p);

		%shells = %shells TAB %p;
	}

	return removeField(%shells, 0);
}

// OTHER //

function AIPlayer::setAimPointHack(%pl, %pos)
{
	if(!%pl.isMounted())
		%pl.setAimLocation(%pos);
	else
	{
		%mount = %pl.getObjectMount();
		%fwd = %mount.getForwardVector();
		%rgt = getWord(%fwd, 1) SPC (-1 * getWord(%fwd, 0)) SPC getWord(%fwd, 2);
		%dot = vectorDot(%rgt, "0 1 0");
		%rot = mAcos(vectorDot(%fwd, "0 1 0"));
		%right = %dot < 0;

		%vec = vectorNormalize(vectorSub(%pos, %pl.getEyePoint()));
		%dist = vectorDist(%pos, %pl.getEyePoint());

		%bvec = setWord(vectorScale(getWords(%vec, 0, 1), -1), 2, getWord(%vec, 2));

		if(!%right)
			%rvec = getWord(%vec, 1) SPC (getWord(%vec, 0) * -1) SPC getWord(%vec, 2);
		else
			%rvec = getWord(%bvec, 1) SPC (getWord(%bvec, 0) * -1) SPC getWord(%bvec, 2);
		
		if(%rot >= 0 && %rot <= $pi/2) // 0 - 90
		{
			%fAmt = vectorScale(%vec, 1 - (%rot / ($pi/2)));
			%rAmt = vectorScale(%rvec, %rot / ($pi/2));
			
			%nvec = vectorNormalize(vectorAdd(%fAmt, %rAmt));
		}
		else if(%rot >= $pi/2 && %rot <= $pi) // 90 - 180
		{
			%nrot = %rot - $pi/2;

			%fAmt = vectorScale(%rvec, 1 - (%nrot / ($pi/2)));
			%rAmt = vectorScale(%bvec, %nrot / ($pi/2));
			
			%nvec = vectorNormalize(vectorAdd(%fAmt, %rAmt));
		}

		%npos = vectorAdd(%pl.getEyePoint(), vectorScale(vectorNormalize(%nvec), %dist));

		if($tlinedebug)
		{
			drawArrow(%pl.getEyePoint(), %vec, "0 0 1 0.5", 2.5, 0.2).schedule(500,delete);
			
			if(%right)
				drawArrow(%pl.getEyePoint(), %nvec, "1 1 0 0.5", 2.5, 0.2).schedule(500,delete);
			else
				drawArrow(%pl.getEyePoint(), %nvec, "1 0 0 0.5", 2.5, 0.2).schedule(500,delete);
		}
		
		%pl.setAimLocation(%npos);
	}
}

function AIPlayer::turretLook(%pl, %radius, %mask)
{
	%db = %pl.getDatablock();

	%dist = %radius * 2;
	%found = -1;
	%pos = %pl.getHackPosition();
	initContainerRadiusSearch(%pos, %radius, %mask);
	while(isObject(%col = containerSearchNext()))
	{
		if(%col == %pl || %col == %pl.turretHead || %col == %pl.turretBase) continue;

		if(!isObject(%pl.sourceClient) || minigameCanDamage(%pl.sourceClient, %col) == 1)
		{
			%cpos = %col.getCenterPos();
			
			if($tlinedebug)
				drawLine(%pos, %cpos, "0 1 0 0.5", 0.01).schedule(500,delete);
			
			if(!%db.turretCanTrigger(%pl, %col))
				continue;
			
			if((%d2 = vectorDist(%pos, %cpos)) < %dist)
			{
				%found = %col;
				%dist = %d2;
			}
		}
	}

	if(isObject(%found))
		return %found SPC %dist;
	else
		return -1;
}

function Player::getLookVector(%pl)
{
	%fvec = %pl.getForwardVector();
	%fX = getWord(%fvec,0);
	%fY = getWord(%fvec,1);

	%evec = %pl.getEyeVector();
	%eX = getWord(%evec,0);
	%eY = getWord(%evec,1);
	%eZ = getWord(%evec,2);

	%eXY = mSqrt(%eX*%eX+%eY*%eY);

	%aimVec = %fX*%eXY SPC %fY*%eXY SPC %eZ;
	return %aimVec;
}

function ShapeBase::getCenterPos(%obj)
{
	if(%obj.getType() & $TypeMasks::PlayerObjectType)
		return %obj.getHackPosition();
	else
		return vectorScale(vectorAdd(%obj.getWorldBoxCenter(), %obj.getPosition()), 0.5);
}

function ShapeBase::getHigherPos(%obj)
{
	if(%obj.getType() & $TypeMasks::PlayerObjectType)
		return %obj.getEyePoint();
	else
		return %obj.getWorldBoxCenter();
}

function ProjectilePredict(%posP, %velP, %posT, %velT) // this is a little silly, but at least it does work somewhat
{
	if(vectorLen(%velP) <= 0 || vectorLen(%velT) <= 0)
		return %posT;
	
	// %dvel = vectorDist(%velP, %velT);
	// %dpos = vectorDist(%posP, %posT);

	// %npos = vectorAdd(%posT, vectorScale(%velT, %dpos / %dvel));

	// %npos = vectorAdd(%npos, vectorScale(%velT, $Turret_LeadDistance));
	
	// for(%i = 0; %i < 10; %i++)
	// 	v = shot vector, calculated to hit p exactly
	// 	t = distance / vectorLen(getWords(v, 0, 1)); //time it takes for the projectile to fly to p
	// 	p = player position + player.velocity * t - ("0 0 " @ 2 * 9.8 * t); //calculating new p given the time
		
	for(%i = 0; %i < 10; %i++)
		v = shot vector, calculated to hit p exactly
		t = distance / vectorLen(getWords(v, 0, 1)); //time it takes for the projectile to fly to p
		p = player position + player.velocity * t - ("0 0 " @ 2 * 9.8 * t); //calculating new p given the time

	if($tlinedebug)
	{
		drawLine(%posP, %posT, "0 1 0 0.5", 0.1).schedule(500, delete);
		drawLine(%posT, %npos, "0 1 0 0.5", 0.1).schedule(500, delete);
		drawLine(%posP, %npos, "1 0 0 0.5", 0.1).schedule(500, delete);
	}
	
	return %npos;
}

package TurretPackMain
{
	function Armor::onAdd(%db, %pl)
	{
		Parent::onAdd(%db, %pl);

		if(isObject(%pl))
		{
			if(%db.isTurretHead)
				%pl.schedule(200, onTurretIdleTick);
		}
	}

	function Armor::onDamage(%db, %pl, %dmg)
	{
		Parent::onDamage(%db, %pl, %dmg);

		if(%db.isTurretArmor)
			%pl.turretDamageCheck();
	}

	function Armor::onDisabled(%db, %pl, %state)
	{
		Parent::onDisabled(%db, %pl, %state);

		if(%db.isTurretArmor)
		{
			if(isObject(%pl.turretHead))
				%pl.turretHead.setDamageLevel(%pl.turretHead.getDataBlock().maxDamage);
			
			cancel(%pl.turretIdle);
			cancel(%pl.turretTarget);

			%pl.setImageTrigger(0,0);
			%pl.setImageTrigger(1,0);
			%pl.setImageTrigger(2,0);
			%pl.setImageTrigger(3,0);
		}
	}

	function AIPlayer::AEDumpAmmo(%pl)
	{
		%db = %pl.getDataBlock();
		if(%db.isTurretArmor)
			return;
		
		Parent::AEDumpAmmo(%pl);
	}

	function AIPlayer::RBloodSimulate(%pl, %p, %v, %d, %s, %k)
	{
		%db = %pl.getDataBlock();
		if(%db.isTurretArmor)
			return;

		Parent::RBloodSimulate(%pl, %p, %v, %d, %s, %k);
	}

	function Armor::Damage(%db, %pl, %src, %pos, %dmg, %type)
	{
		if(%db.isTurretArmor)
		{
			if(%pos $= "")
				%pos = %pl.getHackPosition();
			
			$bloodIgnore[%pos] = true;
		}

		Parent::Damage(%db, %pl, %src, %pos, %dmg, %type);
	}

	function createBloodSplatterExplosion(%pos, %vel, %scale)
	{
		if($bloodIgnore[%pos])
		{
			$bloodIgnore[%pos] = "";
			return;
		}

		Parent::createBloodSplatterExplosion(%pos, %vel, %scale);
	}

	function AIPlayer::startDrippingBlood(%pl, %len)
	{
		%db = %pl.getDataBlock();
		if(%db.isTurretArmor)
			return;
		
		Parent::startDrippingBlood(%pl, %len);
	}

	function AIPlayer::doSplatterBlood(%pl, %amt, %pos)
	{
		%db = %pl.getDataBlock();
		if(%db.isTurretArmor)
			return;
		
		Parent::doSplatterBlood(%pl, %amt, %pos);
	}

	function AIPlayer::playDeathCry(%pl)
	{
		%db = %pl.getDataBlock();
		if(%db.isTurretArmor && %db.TurretPersistent && (isObject(%pl.spawnBrick) || isObject(%pl.getObjectMount())))
			return;
		
		Parent::playDeathCry(%pl);
	}
	
	function AIPlayer::playDeathAnimation(%pl)
	{
		%db = %pl.getDataBlock();
		if(%db.isTurretArmor && %db.TurretPersistent && (isObject(%pl.spawnBrick) || isObject(%pl.getObjectMount())))
			return;
		
		Parent::playDeathAnimation(%pl);
	}

	function AIPlayer::RemoveBody(%pl)
	{
		%db = %pl.getDataBlock();
		if(%db.isTurretArmor && %db.TurretPersistent && (isObject(%pl.spawnBrick) || isObject(%pl.getObjectMount())))
			return;
		
		Parent::RemoveBody(%pl);
	}

	function fxDtsBrick::spawnVehicle(%obj, %time)
	{
		%stop = 0;
		if(isObject(%veh = %obj.Vehicle) && (%db = %veh.getDataBlock()).TurretPersistent)
		{
			if(%time > 0)
			{
				%stop = 1;
			}
			else if(%veh.getDamagePercent() >= 1.0)
				%veh.delete();
		}

		Parent::spawnVehicle(%obj, %time);

		if(%stop && isEventPending(%obj.spawnVehicleSchedule))
			cancel(%obj.spawnVehicleSchedule);
	}
};
activatePackage(TurretPackMain);