$maxTurretEmitters = 4;

// datablock functions //

function turretIsFriendly(%pl, %target)
{
	if((%target.getType() & $TypeMasks::VehicleObjectType) || isObject(%target.spawnBrick))
	{
		for(%i = 0; %i < %target.getDataBlock().numMountPoints; %i++)
		{
			%obj = %target.getMountNodeObject(%i);
			if(isObject(%obj) && !turretIsFriendly(%pl, %obj))
				return false;
		}

		return true;
	}

	%base = %pl.turretBase;

	if(!isObject(%base))
		%base = %pl;

	if(isObject(%pl.sourceClient))
	{
		if(%pl.sourceClient == %target.client)
			return 1;

		%dm = minigameCanDamage(%pl.sourceClient, %target);
		if(%dm == 0)
			return 1;
		else if(%dm == -1)
			return -1;
		
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
				if(%dm)
					return 1;
			}
			else if(isObject(%target.spawnBrick))
			{
				%dm = %pl.sourceClient.slyrTeam.isAlliedTeam(%mg.teams.getTeamFromName(%target.spawnBrick.getControllingTeam()));
				if(%dm)
					return 1;
			}
		}
	}
	else if(isObject(%base.spawnBrick))
	{
		%mg = getMinigameFromObject(%base);
		%mg2 = getMinigameFromObject(%target);

		if(%mg != %mg2 || !isObject(%mg) || !isObject(%mg2))
			return -1;
		
		if(%target.getType() & $TypeMasks::PlayerObjectType)
			%targ = %target.client;
		else
			%targ = %target.getControllingClient();
		
		if(!isObject(%targ))
			return -1;
		
		if(%mg.isSlayerMinigame)
		{
			%teamA = %mg.teams.getTeamFromName(%base.spawnBrick.getControllingTeam());

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
				if(%dm)
					return 1;
			}
		}
		else
		{
			%dm = minigameCanDamage(%base, %target);
			if(%dm == 0)
				return 1;
		}
	}
	
	return 0;
}

function Armor::turretCanSee(%db, %pl, %target)
{
	%pos = %pl.getCenterPos();
	if(isObject(%target))
	{
		if($tlinedebug)
		{
			drawLine(%pos, %target.getEyePoint(), "0 1 0 0.5", 0.01).schedule(500,delete);
			drawLine(%pos, %target.getCenterPos(), "0 1 0 0.5", 0.01).schedule(500,delete);
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
	
	if(vectorDist(%pl.getCenterPos(), %target.getCenterPos()) > %db.TurretLookRange || !%db.turretCanSee(%pl, %target) || %pl.getDamagePercent() >= 1.0 || %target.getDamagePercent() >= 1.0)
		return 0;
	
	%img = %pl.getMountedImage(0);
	if(!isObject(%img))
		return 0;
	
	%team = turretIsFriendly(%pl, %target);
	if(%team == -1 || %team == 0 && %img.triggerTeam || %team == 1 && !%img.triggerTeam)
		return 0;

	if(!%img.canTrigger(%pl, 0, %target))
		return 0;	

	return 1;
}

function Armor::turretOnIdleTick(%db, %pl)
{

}

function Armor::turretOnTargetTick(%db, %pl, %target)
{
	%pl.getMountedImage(0).onTargetTick(%pl, 0, %target);
}

function Armor::turretOnTargetFound(%db, %pl, %target)
{
	%pl.getMountedImage(0).onTargetFound(%pl, 0, %target);
}

function Armor::turretOnTargetLost(%db, %pl, %target)
{
	%pl.getMountedImage(0).onTargetLost(%pl, 0, %target);
}

function Armor::turretOnDestroyed(%db, %pl, %src)
{
	if(isObject(%pl.spawnBrick) && !isObject(%pl.sourceClient))
		%pl.spawnBrick.onTurret(%pl, %src, "Destroyed");

	if(getSimTime() - %pl.lastExplosionTime > 2000)
	{
		%pl.lastExplosionTime = getSimTime();

		if(isObject(%db.destroyedExplosion))
		{
				%p = new Projectile()
				{
					dataBlock = %db.destroyedExplosion;
					initialPosition = %pl.getCenterPos();
					initialVelocity = "0 0 1";
				};

				%p.explode();
			}

		if(isObject(%db.destroyedSound))
			serverPlay3D(%db.destroyedSound, %pl.getCenterPos());
	}

	for(%i = 0; %i < $maxTurretEmitters; %i++)
	{
		if(isObject(%o = %pl.powerParticle[%i]))
			%o.delete();

		if(isObject(%o = %pl.disabledParticle[%i]))
			%o.delete();

		if(isObject(%db.destroyedEmitter[%i]))
		{
			if(isObject(%pl.destroyedParticle[%i]))
				%pl.destroyedParticle[%i].delete();

			%node = new ParticleEmitterNode()
			{
				datablock = GenericEmitterNode;
				emitter = %db.destroyedEmitter[%i];
				scale = "0 0 0";
				position = %pl.getCenterPos();
			};

			%node.setColor("1.0 1.0 1.0 1.0");
			%pl.destroyedParticle[%i] = %node;
		}
	}

	if(isObject(%pl.wallMount))
		%pl.wallMount.mountObject(%pl, 0);
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

	for(%i = 0; %i < $maxTurretEmitters; %i++)
	{
		if(isObject(%o = %pl.powerParticle[%i]))
			%o.delete();

		if(isObject(%db.disabledEmitter[%i]))
		{
			if(isObject(%pl.disabledParticle[%i]))
				%pl.disabledParticle[%i].delete();

			%node = new ParticleEmitterNode()
			{
				datablock = GenericEmitterNode;
				emitter = %db.disabledEmitter[%i];
				scale = "0 0 0";
				position = %pl.getCenterPos();
			};
			
			%node.setColor("1.0 1.0 1.0 1.0");
			%pl.disabledParticle[%i] = %node;
		}
	}
	
	if(isObject(%pl.spawnBrick) && !isObject(%pl.sourceClient))
		%pl.spawnBrick.onTurret(%pl, %src, "Disabled");
}

function Armor::turretOnRecovered(%db, %pl, %src)
{
	if(isObject(%pl.spawnBrick) && !isObject(%pl.sourceClient))
		%pl.spawnBrick.onTurret(%pl, %src, "Recovered");
	
	for(%i = 0; %i < $maxTurretEmitters; %i++)
	{
		if(isObject(%o = %pl.powerParticle[%i]))
			%o.delete();

		if(isObject(%o = %pl.destroyedParticle[%i]))
			%o.delete();

		if(isObject(%db.disabledEmitter[%i]))
		{
			if(isObject(%pl.disabledParticle[%i]))
				%pl.disabledParticle[%i].delete();

			%node = new ParticleEmitterNode()
			{
				datablock = GenericEmitterNode;
				emitter = %db.disabledEmitter[%i];
				scale = "0 0 0";
				position = %pl.getCenterPos();
			};
			
			%node.setColor("1.0 1.0 1.0 1.0");
			%pl.disabledParticle[%i] = %node;
		}
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

	%head = %pl.turretHead;
	
	if(!isObject(%head))
		%head = %pl;
	
	if(isObject(%pl.spawnBrick) && !isObject(%pl.sourceClient))
		%pl.spawnBrick.onTurret(%pl, %src, "Repaired");
	
	for(%i = 0; %i < $maxTurretEmitters; %i++)
	{
		if(isObject(%o = %pl.powerParticle[%i]))
			%o.delete();

		if(isObject(%o = %pl.destroyedParticle[%i]))
			%o.delete();
		
		if(isObject(%o = %pl.disabledParticle[%i]))
			%o.delete();

		if(!%head.isPowered || %head.isJammed)
		{
			if(isObject(%db.powerLostEmitter[%i]))
			{
				%node = new ParticleEmitterNode()
				{
					datablock = GenericEmitterNode;
					emitter = %db.powerLostEmitter[%i];
					scale = "0 0 0";
					position = %pl.getCenterPos();
				};

				%node.setColor("1.0 1.0 1.0 1.0");
				%pl.powerParticle[%i] = %node;
			}
		}
	}
}

function Armor::turretOnPowerLost(%db, %pl)
{
	%base = %pl.turretBase;

	if(!isObject(%base))
		%base = %pl;
	
	%bdb = %base.getDataBlock();

	if(!%base.isDisabled && !%base.isDestroyed)
	{
		for(%i = 0; %i < $maxTurretEmitters; %i++)
		{
			if(isObject(%bdb.powerLostEmitter[%i]))
			{
				if(isObject(%base.powerParticle[%i]))
					%base.powerParticle[%i].delete();

				%node = new ParticleEmitterNode()
				{
					datablock = GenericEmitterNode;
					emitter = %bdb.powerLostEmitter[%i];
					scale = "0 0 0";
					position = %base.getCenterPos();
				};

				%node.setColor("1.0 1.0 1.0 1.0");
				%base.powerParticle[%i] = %node;
			}
		}
	}
}

function Armor::turretOnNoPowerTick(%db, %pl)
{

}

function Armor::turretOnPowerRestored(%db, %pl)
{
	%base = %pl.turretBase;

	if(!isObject(%base))
		%base = %pl;

	if(!%base.isDisabled && !%base.isDestroyed)
	{
		for(%i = 0; %i < $maxTurretEmitters; %i++)
		{
			if(isObject(%base.powerParticle[%i]))
				%base.powerParticle[%i].delete();
		}
	}
}

function Armor::turretOnSpawn(%db, %pl)
{
	%pl.spawnBrick.onTurret(%pl, %pl, "Spawn");
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

	if(%pl.isJammed || isObject(%grp = %pl.powerGroup))
	{
		if((%pl.isJammed || (!%grp.isGenerator(%pl) && !%grp.getPower())) && %pl.isPowered)
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

	if(%pl.isJammed || isObject(%grp = %pl.powerGroup))
	{
		if(%pl.isJammed || (!%grp.isGenerator(%pl) && !%grp.getPower()))
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
			%pl.wasJammed = false;
			%db.turretOnPowerRestored(%pl);
			%skip = true;
		}
	}
	else
	{
		%pl.isPowered = true;
		if(%pl.wasJammed)
		{
			%db.turretOnPowerRestored(%pl);
			%pl.wasJammed = false;
			%skip = true;
		}
	}

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
			if(!%obj.isDisabled)
				%db.turretOnDisabled(%obj, %src);

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
			if(%obj.isDestroyed)
				%db.turretOnRecovered(%obj, %src);
				
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

function AIPlayer::turretJam(%obj, %time)
{
	if(%obj.getDataBlock().neverJam)
		return;

	if(isObject(%obj.turretHead))
		return %obj.turretHead.turretJam(%time);

	if(%time < getTimeRemaining(%obj.jamReset))
		return;
	
	cancel(%obj.jamReset);

	%obj.isJammed = true;
	%obj.wasJammed = true;
	%obj.jamReset = %obj.schedule(%time, turretUnJam);
}

function AIPlayer::turretUnJam(%obj)
{
	cancel(%obj.jamReset);
	%obj.isJammed = false;
	%obj.wasJammed = true;
}

// support functions //

function Normal2Rotation(%normal)  
{  
	if(getWord(%normal, 2) == 1 || getWord(%normal, 2) == -1)
		%rotAxis = vectorNormalize(vectorCross(%normal, "0 1 0"));
	else
		%rotAxis = vectorNormalize(vectorCross(%normal, "0 0 1"));

	%initialAngle = mACos(vectorDot(%normal, "0 0 1"));
	%rotation = %rotAxis SPC mRadtoDeg(%initialAngle);

	return %rotation;
}

function MatrixInverse(%m) // by Val
{
	%inv_rot = vectorScale(getWords(%m, 3, 5), -1) SPC getWord(%m, 6);
	%inv_pos_mat = MatrixMultiply("0 0 0" SPC %inv_rot, %m);
	
	return vectorScale(getWords(%inv_pos_mat, 0, 2), -1) SPC %inv_rot;
}

function AIPlayer::setAimPointHack(%pl, %point)
{
	if(!%pl.isMounted())
		%pl.setAimLocation(%point);
	else
	{
		%mount = %pl.getObjectMount();
		%pos = %pl.getPosition();

		%xform = MatrixInverse(%pos SPC getWords(%mount.getTransform(), 3, 6));
		%mtx = MatrixMultiply(%xform, %point @ "0 0 1 0");
		%npos = vectorAdd(%pos, %mtx);

		%pl.setAimLocation(%npos);

		if($tlinedebug)
		{
			drawArrow(%pl.getEyePoint(), vectorSub(%point, %pos), "0 1 0 0.5", 1, 1.0).schedule(500,delete);
			drawArrow(%pl.getEyePoint(), vectorSub(%npos,  %pos), "1 0 0 0.5", 1, 1.0).schedule(500,delete);
		}
	}
}

function AIPlayer::turretLook(%pl, %radius, %mask)
{
	%db = %pl.getDatablock();

	%dist = %radius * 2;
	%found = -1;
	%pos = %pl.getCenterPos();
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
		return vectorScale(vectorAdd(%obj.getWorldBoxCenter(), vectorScale(%obj.getPosition(), 3)), 0.25);
		// return %obj.getHackPosition();
	else
		return vectorScale(vectorAdd(%obj.getWorldBoxCenter(), %obj.getPosition()), 0.5);
}

function ShapeBase::getHigherPos(%obj)
{
	if(%obj.getType() & $TypeMasks::PlayerObjectType)
		return vectorScale(vectorAdd(%obj.getWorldBoxCenter(), %obj.getPosition()), 0.5);
		// return %obj.getEyePoint();
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