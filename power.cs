function getPowerGroup(%name)
{
	%name = getSafeVariableName(%name);

	if(isObject($PowerGroup_[%name]))
		return $PowerGroup_[%name];
	else
		return newPowerGroup(%name);
}

function newPowerGroup(%name)
{
	%name = getSafeVariableName(%name);
	
	%grp = new ScriptObject(PowerGroup)
	{
		minPower = 1;
		groupName = %name;
	};

	%grp.generators = new SimSet();
	%grp.members = new SimSet();

	MissionCleanup.add(%grp);
	MissionCleanup.add(%grp.generators);
	MissionCleanup.add(%grp.members);

	$PowerGroup_[%name] = %grp;

	if(!isObject(PowerGroupSet))
	{
		%set = new SimSet(PowerGroupSet);
		MissionCleanup.add(%set);
	}

	PowerGroupSet.add(%grp);

	return %grp;
}

function PowerGroup::validate(%grp)
{
	%cts = %grp.getGenerators();
	for(%i = 0; %i < %cts; %i++)
	{
		%gen = %grp.getGenerator(%i);
		
		if(%gen.powerGroup != %grp)
			%grp.remove(%gen);
	}

	%cts = %grp.getMembers();
	for(%i = 0; %i < %cts; %i++)
	{
		%mem = %grp.getMember(%i);
		
		if(%mem.powerGroup != %grp)
			%grp.remove(%mem);
	}
}

function PowerGroup::add(%grp, %obj)
{
	if(!isObject(%obj))
		return 0;

	%db = %obj.getDataBlock();

	if(!%db.isTurretArmor || %grp.isMember(%obj) || %grp.isGenerator(%obj))
		return 0;
	
	if(isObject(%obj.powerGroup))
	{
		error("PowerGroup::add() - object " @ %obj @ " already belongs to group " @ %obj.powerGroup);
		return 0;
	}
	
	if(%db.isPowerGenerator)
	{
		%grp.generators.add(%obj);
		%obj.powerGroup = %grp;
		%grp.validate();
		return 1;
	}
	else
	{
		%grp.members.add(%obj);
		%obj.powerGroup = %grp;
		%grp.validate();
		return 1;
	}
}

function PowerGroup::remove(%grp, %obj)
{
	if(!isObject(%obj))
		return 0;

	%db = %obj.getDataBlock();

	if(!%db.isTurretArmor)
		return 0;
	
	if(%grp.isMember(%obj))
	{
		%grp.members.remove(%obj);
		%obj.powerGroup = "";
		return 1;
	}
	else if(%grp.isGenerator(%obj))
	{
		%grp.generators.remove(%obj);
		%obj.powerGroup = "";
		return 1;
	}
	else
		return 0;
}

function PowerGroup::getGenerators(%grp)
{
	return %grp.generators.getCount();
}

function PowerGroup::getGenerator(%grp, %idx)
{
	if(%idx >= %grp.generators.getCount())
	{
		error("PowerGroup::getGenerator() - index out of range (" @ %idx @ " > " @ %grp.generators.getCount() @ ")");
		return -1;
	}

	return %grp.generators.getObject(%idx);
}

function PowerGroup::isGenerator(%grp, %obj)
{
	if(!isObject(%obj))
		return 0;
	
	%cts = %grp.getGenerators();
	for(%i = 0; %i < %cts; %i++)
	{
		%gen = %grp.getGenerator(%i);
		if(%gen.getId() == %obj.getId())
			return true;
	}

	return false;
}

function PowerGroup::getMembers(%grp)
{
	return %grp.members.getCount();
}

function PowerGroup::getMember(%grp, %idx)
{
	if(%idx >= %grp.members.getCount())
	{
		error("PowerGroup::getMember() - index out of range (" @ %idx @ " > " @ %grp.members.getCount() @ ")");
		return -1;
	}

	return %grp.members.getObject(%idx);
}

function PowerGroup::isMember(%grp, %obj)
{
	if(!isObject(%obj))
		return 0;
	
	%cts = %grp.getMembers();
	for(%i = 0; %i < %cts; %i++)
	{
		%mem = %grp.getMember(%i);
		if(%mem.getId() == %obj.getId())
			return true;
	}

	return false;
}

function PowerGroup::isPowered(%grp, %obj)
{
	return %grp.isMember(%obj) && %grp.getPower() > %grp.minPower;
}

function PowerGroup::getPower(%grp)
{
	%power = 0;

	%cts = %grp.getGenerators();
	for(%i = 0; %i < %cts; %i++)
	{
		%gen = %grp.getGenerator(%i);
		%db = %gen.getDataBlock();

		if(!%gen.isDisabled && !%gen.isDestroyed)
		{
			if(%db.generatorPower > 0)
				%power += %db.generatorPower;
			else
				%power ++;
		}
	}

	return %power;
}

function PowerGroup::onRemove(%grp)
{
	if(isObject(%grp.generators))
		%grp.generators.delete();
	
	if(isObject(%grp.members))
		%grp.members.delete();
}