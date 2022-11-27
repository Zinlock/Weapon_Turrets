// datablock functions //

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
	
	if(vectorDist(%pl.getHackPosition(), %target.getCenterPos()) > %db.TurretLookRange || !%db.turretCanSee(%pl, %target) || %pl.getDamagePercent() >= 1.0 || %target.getDamagePercent() >= 1.0)
		return 0;
	
	if(isObject(%img = %pl.getMountedImage(0)) && !%img.canTrigger(%pl, 0, %target))
		return 0;

	%tt = %pl.triggerTeam;
	
	if(isObject(%pl.sourceClient))
	{
		if(%pl.sourceClient == %target.client)
			return 0;

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
				%dm = %pl.sourceClient.slyrTeam.isAlliedTeam(%mg.teams.getTeamFromName(%target.spawnBrick.getControllingTeam()));
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

function Armor::turretOnDestroyed(%db, %pl, %src)
{
	if(isObject(%pl.spawnBrick) && !isObject(%pl.sourceClient))
	{
		if(%db.isPowerGenerator)
			%pl.spawnBrick.onTurret(%pl, %src, "onGeneratorDestroyed");
		else
			%pl.spawnBrick.onTurret(%pl, %src, "onTurretDestroyed");
	}
}

function Armor::turretOnDisabled(%db, %pl, %src)
{
	if(isObject(%pl.turretHead))
	{
		cancel(%pl.turretHead.turretIdle);
		cancel(%pl.turretHead.turretTarget);
		
		%pl.turretHead.setImageTrigger(0,0);
		%pl.turretHead.setImageTrigger(1,0);
		%pl.turretHead.setImageTrigger(2,0);
		%pl.turretHead.setImageTrigger(3,0);
	}
	
	if(isObject(%pl.spawnBrick) && !isObject(%pl.sourceClient))
	{
		if(%db.isPowerGenerator)
			%pl.spawnBrick.onTurret(%pl, %src, "onGeneratorDisabled");
		else
			%pl.spawnBrick.onTurret(%pl, %src, "onTurretDisabled");
	}
}

function Armor::turretOnRecovered(%db, %pl, %src)
{
	if(isObject(%pl.spawnBrick) && !isObject(%pl.sourceClient))
	{
		if(%db.isPowerGenerator)
			%pl.spawnBrick.onTurret(%pl, %src, "onGeneratorRecovered");
		else
			%pl.spawnBrick.onTurret(%pl, %src, "onTurretRecovered");
	}
}

function Armor::turretOnRepaired(%db, %pl, %src)
{
	%pl.setEnergyLevel(0);

	if(isObject(%pl.turretHead))
	{
		%pl.turretHead.setDamageLevel(0);
		%pl.turretHead.setControlObject(%pl.turretHead);

		%pl.turretHead.target = "";
		%pl.turretHead.targetFoundTime = "";
		%pl.turretHead.targetLostTime = "";
		%pl.turretHead.schedule(200, onTurretIdleTick);
	}
	
	if(isObject(%pl.spawnBrick) && !isObject(%pl.sourceClient))
	{
		if(%db.isPowerGenerator)
			%pl.spawnBrick.onTurret(%pl, %src, "onGeneratorRepaired");
		else
			%pl.spawnBrick.onTurret(%pl, %src, "onTurretRepaired");
	}
}

function Armor::turretOnShieldBreak(%db, %pl, %src)
{
	
}

function Armor::turretOnPowerLost(%db, %pl)
{
	
}

function Armor::turretOnNoPowerTick(%db, %pl)
{

}

function Armor::turretOnPowerRestored(%db, %pl)
{
	
}

function Armor::turretOnSpawn(%db, %pl)
{
	if(%db.isPowerGenerator)
		%pl.spawnBrick.onGeneratorSpawn(%pl);
	else
		%pl.spawnBrick.onTurretSpawn(%pl);
}

function Armor::turretCanMount(%db, %pl, %src, %img)
{
	return true;
}

// player functions //

function AIPlayer::turretSpawned(%pl)
{
	return %pl.getDataBlock().turretOnSpawn(%pl);
}

function AIPlayer::turretCanMount(%pl, %src, %img)
{
	return %pl.getDataBlock().turretCanMount(%pl, %src, %img);
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

	if(isObject(%grp = %pl.powerGroup))
	{
		if(!%grp.getPower() && %pl.isPowered)
		{
			%pl.isPowered = false;
			%db.turretOnPowerLost(%pl);
			%pl.onTurretTargetLost();
			return;
		}
	}
	else %pl.isPowered = true;

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

	%skip = false;
	
	if(isObject(%grp = %pl.powerGroup))
	{
		if(!%grp.getPower())
		{
			if(%pl.isPowered)
			{
				%pl.isPowered = false;
				%db.turretOnPowerLost(%pl);
			}
			else %db.turretOnNoPowerTick(%pl);
		}
		else if(!%pl.isPowered)
		{
			%pl.isPowered = true;
			%db.turretOnPowerRestored(%pl);
			%skip = true;
		}
	}
	else %pl.isPowered = true;

	if(%pl.isPowered && !%skip)
	{
		%db.turretOnIdleTick(%pl);

		if(getSimTime() - %pl.lastSightCheck >= %db.TurretLookTime)
		{
			%pl.lastSightCheck = getSimTime();
			%target = %pl.turretLook(%db.TurretLookRange, %db.TurretLookMask);
			%pl.target = firstWord(%target);

			if(isObject(%target))
				%pl.onTurretTargetFound(firstWord(%target));
		}
	}

	%pl.turretIdle = %pl.schedule(%db.TurretThinkTime, onTurretIdleTick);
}

function AIPlayer::turretRepair(%pl, %amt, %src)
{
	if(isObject(%pl.turretBase))
		return %pl.turretBase.turretRepair(%amt, %src);
	
	%pl.setDamageLevel(%pl.getDamageLevel() - %amt);
	%pl.turretDamageCheck(%src);
}

function AIPlayer::turretDamageCheck(%obj, %src)
{
	%db = %obj.getDataBlock();

	if(%obj.getDamagePercent() >= %db.destroyedLevel)
	{
		if(!%obj.isDestroyed)
		{
			%obj.isDisabled = true;
			%obj.isDestroyed = true;
			%db.turretOnDestroyed(%obj, %src);
		}
	}
	else if(%obj.getDamagePercent() >= %db.disabledLevel)
	{
		if(!%obj.isDisabled)
		{
			%obj.isDestroyed = false;
			%obj.isDisabled = true;
			%db.turretOnDisabled(%obj, %src);
		}
		else if(%obj.isDestroyed)
		{
			%obj.isDestroyed = false;
			%obj.isDisabled = true;
			%db.turretOnRecovered(%obj, %src);
		}
	}
	else
	{
		if(%obj.isDestroyed || %obj.isDisabled)
		{
			%obj.isDestroyed = false;
			%obj.isDisabled = false;
			%db.turretOnRepaired(%obj, %src);
		}
	}
}

function AIPlayer::turretKill(%obj)
{
	%obj.takeSelfDmg = true;
	%obj.kill();
	%obj.takeSelfDmg = false;
	%obj.turretDamageCheck(0);
}

// support functions //

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

function Player::isJetting(%pl)
{
	return (%pl.jetDown && %pl.getEnergyLevel() > %pl.getDataBlock().minJetEnergy);
}

function AIPlayer::turretJetLoop(%pl, %targ, %minStart, %minEnd) // this sucks! but it works, so its ok
{
	cancel(%pl.tjl[%targ]);

	if(!isObject(%targ) || %targ.getDamagePercent() >= 1.0 || %pl.isDisabled || !%targ.getDataBlock().canJet)
	{
		%pl.tj[%targ] = false;
		%pl.tjs[%targ] = 0;
		%pl.tje[%targ] = 0;
		return;
	}

	if(%targ.isJetting())
	{
		if(%pl.tj[%targ])
		{
			%pl.tje[%targ] -= getSimTime() - %pl.ltj[%targ];

			if(%pl.tje[%targ] <= 0)
				%pl.tje[%targ] = 0;
		}
		else
		{
			%pl.tjs[%targ] += getSimTime() - %pl.ltj[%targ];

			if(%pl.tjs[%targ] >= %minStart)
			{
				%pl.tj[%targ] = true;
				%pl.tjs[%targ] = 0;
				%pl.tje[%targ] = 0;
			}
		}
	}
	else
	{
		if(%pl.tj[%targ])
		{
			%pl.tje[%targ] += getSimTime() - %pl.ltj[%targ];

			if(%pl.tje[%targ] >= %minEnd)
			{
				%pl.tj[%targ] = false;
				%pl.tje[%targ] = 0;
				%pl.tjs[%targ] = 0;
			}
		}
		else
		{
			%pl.tjs[%targ] -= getSimTime() - %pl.ltj[%targ];

			if(%pl.tjs[%targ] <= 0)
				%pl.tjs[%targ] = 0;
		}
	}

	%pl.ltj[%targ] = getSimTime();
	%pl.tjl[%targ] = %pl.schedule(100, turretJetLoop, %targ, %minStart, %minEnd);
}

function AIPlayer::turretJetting(%pl, %targ)
{
	return %pl.tj[%targ];
}

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

function Projectile::sourceHack(%proj, %src, %cl)
{
	%proj.sourceClient = %cl;
	%proj.client = %cl;
	%proj.sourceObject = %src;
}

if($Version != 21)
{
	eval("function AIPlayer::applyBodyParts() {}");
	eval("function AIPlayer::applyBodyColors() {}");
	eval("function AIPlayer::onDeath() {}");
}