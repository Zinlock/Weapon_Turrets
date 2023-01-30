function ShapeBase::getCenterPos(%obj)
{
	if(%obj.getType() & $TypeMasks::PlayerObjectType)
		return %obj.getHackPosition();
	else
		return vectorScale(vectorAdd(%obj.getWorldBoxCenter(), %obj.getPosition()), 0.5);
}

if(!isFunction(ShapeBaseData, onEnergyShieldBreak))
	eval("function ShapeBaseData::onEnergyShieldBreak(%obj, %src) { }");

package T2EnergyShield
{
	function ShapeBase::Damage(%obj, %src, %pos, %dmg, %type)
	{
		%db = %obj.getDataBlock();
		if(%db.energyShield > 0 && %obj.getDamagePercent() < 1.0 && (!%db.isTurretArmor || !%obj.isDisabled && !%obj.isDestroyed && (%obj.isPowered || %obj.turretHead.isPowered)))
		{
			if(%db.energyDelay > 0)
			{
				if(getSimTime() - %obj.lastShieldHitTime < %db.energyDelay * 1000)
					%obj.setEnergyLevel(%obj.lastShieldHitEnergy);
			}

			%shield = mClampF(%db.energyShield, 0, 1);
			%erg = %obj.getEnergyLevel();
			%ndm = %dmg * %shield;

			if(%ndm > %erg)
				%ndm = %erg;
			
			%nrg = %erg - %ndm;

			if(%nrg <= 0 && %erg > 0)
			{
				%db.onEnergyShieldBreak(%obj, %src);
				
				if(isObject(%db.energyBreakSound))
					serverPlay3D(%db.energyBreakSound, %obj.getCenterPos());
			}

			%dmg = %dmg - %ndm;
			%obj.setEnergyLevel(%nrg);
			
			if(%obj.getEnergyLevel() > 0)
			{
				if(isObject(%db.energySound))
					serverPlay3D(%db.energySound, %obj.getCenterPos());

				if(isObject(%db.energyShape))
				{
					%shape = %obj.lastEnergyShape;

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

					%dir = vectorNormalize(vectorSub(%pos, %obj.getCenterPos()));

					%x = getWord(%dir,0) / 2;
					%y = (getWord(%dir,1) + 1) / 2;
					%z = getWord(%dir,2) / 2;

					%shape.setTransform(%obj.getCenterPos() SPC VectorNormalize(%x SPC %y SPC %z) SPC mDegToRad(180));
					%shape.playThread(0, hit);
				}
			}

			%obj.lastShieldHitEnergy = %obj.getEnergyLevel();
			%obj.lastShieldHitTime = getSimTime();
			
			if(%dmg <= 0)
				return;
		}

		return Parent::Damage(%obj, %src, %pos, %dmg, %type);
	}
};
activatePackage(T2EnergyShield);