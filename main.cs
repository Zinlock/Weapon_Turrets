// main.cs:
// datablocks for turret guns
// functions for shooting stuff, tracking players etc

$Turret_TargetMask = $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::ItemObjectType | $TypeMasks::ProjectileObjectType;
$Turret_WallMask = $TypeMasks::fxBrickObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::StaticObjectType | $TypeMasks::InteriorObjectType | $TypeMasks::TerrainObjectType;


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

		// if(%db.isTurretArmor)
		// 	%pl.turretDamageCheck();
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

	function Armor::Damage(%db, %pl, %src, %pos, %dmg, %type)
	{
		if(%db.isTurretArmor)
		{
			if(%src == %pl || %src.sourceObject == %pl)
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

				%dmg = %dmg - %ndm;
				%pl.setEnergyLevel(%erg - %ndm);

				if(%erg > %pl.getEnergyLevel() && %erg > (%db.maxEnergy * 0.1))
					%db.turretOnShieldBreak(%pl, %src);
				
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