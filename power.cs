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
	%gens = %grp.generators;
	%cts = %gens.getCount();
	for(%i = 0; %i < %cts; %i++)
	{
		%gen = %gens.getObject(%i);

		if(%gen.powerGroup != %grp)
			%grp.remove(%gen);
	}

	%mems = %grp.members;

	%cts = %mems.getCount();
	for(%i = 0; %i < %cts; %i++)
	{
		%mem = %mems.getObject(%i);

		if(%mem.powerGroup != %grp)
			%grp.remove(%mem);
	}
}

function PowerGroup::add(%grp, %obj)
{
	if(!isObject(%obj))
		return 0;

	if(isFunction(%obj.getClassName(), getDatablock))
		%db = %obj.getDataBlock();

	if(%grp.isMember(%obj) || %grp.isGenerator(%obj))
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

function PowerGroup::isGenerator(%grp, %obj)
{
	if(!isObject(%obj))
		return 0;

	return %grp.generators.isMember(%obj);

	// %gens = %grp.generators;

	// %cts = %gens.getCount();
	// for(%i = 0; %i < %cts; %i++)
	// {
	// 	%gen = %gens.getObject(%idx);
	// 	if(%gen.getId() == %obj.getId())
	// 		return true;
	// }

	// return false;
}

function PowerGroup::isMember(%grp, %obj)
{
	if(!isObject(%obj))
		return 0;
	
	return %grp.members.isMember(%obj);

	// %mems = %grp.members;

	// %cts = %mems.getCount();
	// for(%i = 0; %i < %cts; %i++)
	// {
	// 	%mem = %mems.getObject(%i);
	// 	if(%mem.getId() == %obj.getId())
	// 		return true;
	// }

	// return false;
}

function PowerGroup::isPowered(%grp, %obj)
{
	return %grp.isMember(%obj) && %grp.getPower() > %grp.minPower;
}

function PowerGroup::getPower(%grp)
{
	%power = 0;

	%cts = %grp.generators.getCount();
	for(%i = 0; %i < %cts; %i++)
	{
		%gen = %grp.generators.getObject(%i);
		%db = %gen.getDataBlock();

		if(!%gen.isDisabled && !%gen.isDestroyed && !%gen.isJammed)
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