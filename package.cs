package TurretPackMain
{
	function ae_calculateDamagePosition(%pl, %pos)
	{
		if(isObject(%pl))
		{
			%db = %pl.getDataBlock();

			if(%db.isTurretArmor)
				return "upbody";
		}

		return Parent::ae_calculateDamagePosition(%pl, %pos);
	}

	function MinigameSO::addMember(%mini,%client)
	{
		Parent::addMember(%mini,%client);

		if(isObject(%client.deployedTurret))
			%client.deployedTurret.turretKill();
	}

	function MinigameSO::removeMember(%mini,%client)
	{
		Parent::removeMember(%mini,%client);

		if(isObject(%client.deployedTurret))
			%client.deployedTurret.turretKill();
	}
	
	function MinigameSO::reset(%mini,%client)
	{
		Parent::reset(%mini,%client);

		for(%i = 0; %i < %mini.numMembers; %i++)
		{
			%cl = %mini.member[%i];
			if(isObject(%cl.deployedTurret))
				%cl.deployedTurret.turretKill();
		}
	}

	function GameConnection::onDrop(%cl, %msg)
	{
		if(isObject(%cl.deployedTurret))
			%cl.deployedTurret.turretKill();

		Parent::onDrop(%cl, %msg);
	}

	function AIPlayer::Pickup(%pl, %itm, %x)
	{
		if(%pl.getDataBlock().isTurretArmor)
			return;
		
		return Parent::Pickup(%pl, %itm, %x);
	}

	function Armor::onRemove(%db, %pl)
	{
		if(%db.isTurretArmor)
		{
			if(isObject(%pl.wallMount))
				%pl.wallMount.delete();

			for(%i = 0; %i < $maxTurretEmitters; %i++)
			{
				if(isObject(%o = %pl.powerParticle[%i]))
					%o.delete();

				if(isObject(%o = %pl.destroyedParticle[%i]))
					%o.delete();
				
				if(isObject(%o = %pl.disabledParticle[%i]))
					%o.delete();
			}
		}

		Parent::onRemove(%db, %pl);
	}

	function Armor::onAdd(%db, %pl)
	{
		Parent::onAdd(%db, %pl);

		if(isObject(%pl))
		{
			if(%db.isTurretHead)
			{
				%pl.isPowered = true;
				%pl.schedule(200, onTurretIdleTick);

				if(!isObject(GlobalTurretSet))
					new SimSet(GlobalTurretSet);

				GlobalTurretSet.add(%pl);
			}
			
			if(%db.defaultScale !$= "")
				%pl.schedule(0, setScale, %db.defaultScale);

			if(!%db.isTurretArmor)
			{
				if(!isObject(TurretTargetSet))
					new SimSet(TurretTargetSet);

				TurretTargetSet.add(%pl);
			}
		}
	}

	function Armor::onDisabled(%db, %pl, %state)
	{
		if(%db.isTurretArmor)
		{
			%pl.lastMount = %pl.getObjectMount();
			%pl.lastMountNode = %pl.getMountNode();
		}

		Parent::onDisabled(%db, %pl, %state);

		if(%db.isTurretArmor)
		{
			if(isObject(%pl.turretHead))
				%pl.turretHead.setDamageLevel(%pl.turretHead.getDataBlock().maxDamage);
			
			if(isObject(%pl.lastMount))
			{
				%pl.lastMount.mountObject(%pl, %pl.lastMountNode);
				%pl.lastMount = -1;
			}
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

	function AIPlayer::zap(%pl)
	{
		%db = %pl.getDataBlock();
		if(%db.isTurretArmor)
		{
			%time = %pl.zapTicks * 250;
			%pl.turretJam(%time);
		}
		else
			Parent::zap(%pl);
	}

	function Armor::Damage(%db, %pl, %src, %pos, %dmg, %type)
	{
		if(%db.isTurretArmor)
		{
			if(!%pl.takeSelfDmg && !isObject(%pl.getControllingClient()))
			{
				if((%src == %pl || %src.sourceObject == %pl) || (isObject(%src) && %src.getDataBlock().isTurretArmor) || (isObject(%src.sourceObject) && %src.sourceObject.getDataBlock().isTurretArmor))
					return;
			}

			%dmg *= getWord(%pl.getScale(), 2);

			if(%pos $= "")
				%pos = %pl.getHackPosition();
			
			$bloodIgnore[%pos] = true;
		}

		%p = Parent::Damage(%db, %pl, %src, %pos, %dmg, %type);
		
		if(%db.isTurretArmor)
			%pl.turretDamageCheck(%src);
		
		return %p;
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
		if(%db.isTurretArmor)
			return;
		
		Parent::playDeathCry(%pl);
	}
	
	function AIPlayer::playDeathAnimation(%pl)
	{
		%db = %pl.getDataBlock();
		if(%db.isTurretArmor)
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
	
	function WheeledVehicleData::onAdd(%db, %obj)
	{
		Parent::onAdd(%db, %obj);

		if(isObject(%obj))
		{
			if(!isObject(TurretTargetSet))
				new SimSet(TurretTargetSet);

			TurretTargetSet.add(%obj);
		}
	}
	
	function FlyingVehicleData::onAdd(%db, %obj)
	{
		Parent::onAdd(%db, %obj);

		if(isObject(%obj))
		{
			if(!isObject(TurretTargetSet))
				new SimSet(TurretTargetSet);

			TurretTargetSet.add(%obj);
		}
	}

	function WheeledVehicleData::onCollision(%this, %obj, %col, %vec, %speed)
	{
		if(%col.getDataBlock().isTurretArmor)
			return;
		
		return Parent::onCollision(%this, %obj, %col, %vec, %speed);
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
		
		if(isObject(%veh = %obj.Vehicle) && (%db = %veh.getDataBlock()).isTurretArmor && !%veh.spawned)
		{
			%veh.spawned = true;
			%veh.turretSpawned();
		}
	}
};
activatePackage(TurretPackMain);

package playerJettingPackage
{
	function Armor::onTrigger(%db, %pl, %trig, %val)
	{
		if(%trig == 4)
		{
			%pl.jetDown = %val;
			if(%val) %pl.jetDownTime = getSimTime();
			else %pl.jetUpTime = getSimTime();
		}

		return Parent::onTrigger(%db, %pl, %trig, %val);
	}
};
activatePackage(playerJettingPackage);