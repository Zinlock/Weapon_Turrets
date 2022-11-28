$Turret_TargetMask = $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::ItemObjectType | $TypeMasks::ProjectileObjectType;
$Turret_WallMask = $TypeMasks::fxBrickObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::StaticObjectType | $TypeMasks::InteriorObjectType | $TypeMasks::TerrainObjectType;

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
			%client.deployedTurret.delete();
	}

	function MinigameSO::removeMember(%mini,%client)
	{
		Parent::removeMember(%mini,%client);

		if(isObject(%client.deployedTurret))
			%client.deployedTurret.delete();
	}
	
	function MinigameSO::reset(%mini,%client)
	{
		Parent::reset(%mini,%client);

		for(%i = 0; %i < %mini.numMembers; %i++)
		{
			%cl = %mini.member[%i];
			if(isObject(%cl.deployedTurret))
				%cl.deployedTurret.delete();
		}
	}

	function GameConnection::onDrop(%cl, %msg)
	{
		if(isObject(%cl.deployedTurret))
			%cl.deployedTurret.delete();

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
				%pl.schedule(200, onTurretIdleTick);
		}
	}

	function Armor::onDisabled(%db, %pl, %state)
	{
		Parent::onDisabled(%db, %pl, %state);

		if(%db.isTurretArmor)
		{
			if(isObject(%pl.turretHead))
				%pl.turretHead.setDamageLevel(%pl.turretHead.getDataBlock().maxDamage);
		}
	}

	function Armor::onTrigger(%db, %pl, %trig, %val)
	{
		if(%trig == 4)
			%pl.jetDown = %val;

		return Parent::onTrigger(%db, %pl, %trig, %val);
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
			if(!%pl.takeSelfDmg && (%src == %pl || %src.sourceObject == %pl))
				return;

			if(%pos $= "")
				%pos = %pl.getHackPosition();
			
			$bloodIgnore[%pos] = true;

			if(%db.energyShield > 0 && !%pl.isDisabled && !%pl.isDestroyed && (%pl.isPowered || %pl.turretHead.isPowered))
			{
				if(%db.energyDelay > 0)
				{
					if(getSimTime() - %pl.lastShieldHitTime < %db.energyDelay * 1000)
						%pl.setEnergyLevel(%pl.lastShieldHitEnergy);
				}

				%shield = mClampF(%db.energyShield, 0, 1);
				%erg = %pl.getEnergyLevel();
				%ndm = %dmg * %shield;

				if(%ndm > %erg)
					%ndm = %erg;
				
				%nrg = %erg - %ndm;

				if(%nrg <= 0 && %erg > 0)
					%db.turretOnShieldBreak(%pl, %src);

				%dmg = %dmg - %ndm;
				%pl.setEnergyLevel(%nrg);
				
				if(%pl.getEnergyLevel() > 0)
				{
					if(isObject(%db.energySound))
						serverPlay3D(%db.energySound, %pl.getHackPosition());

					if(isObject(%db.energyShape))
					{
						%shape = %pl.lastEnergyShape;

						if(!isObject(%shape))
						{
							%shape = new StaticShape() { datablock = %db.energyShape; };
							%shape.cleanup = %shape.schedule(3000, delete);
						}
						else
						{
							cancel(%shape.cleanup);
							%shape.cleanup = %shape.schedule(3000, delete);
						}

						%s = %db.energyScale;

						if(%s <= 0)
							%s = 1;

						%shape.setScale(%s SPC %s SPC %s);

						%dir = vectorNormalize(vectorSub(%pos, %pl.getHackPosition()));

						%x = getWord(%dir,0) / 2;
						%y = (getWord(%dir,1) + 1) / 2;
						%z = getWord(%dir,2) / 2;

						%shape.setTransform(%pl.getHackPosition() SPC VectorNormalize(%x SPC %y SPC %z) SPC mDegToRad(180));
						%shape.playThread(0, hit);
					}
				}
				
				%pl.lastShieldHitEnergy = %pl.getEnergyLevel();
				%pl.lastShieldHitTime = getSimTime();
				
				if(%dmg <= 0)
					return;
			}
		}

		Parent::Damage(%db, %pl, %src, %pos, %dmg, %type);

		if(%db.isTurretArmor)
			%pl.turretDamageCheck(%src);
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