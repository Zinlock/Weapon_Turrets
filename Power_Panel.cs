datablock PlayerData(Turret_TribalBasePanel : Turret_TribalBaseGenerator)
{
	destroyedExplosion = Turret_TribalDestroyedProjectile;
	destroyedSound = Turret_GeneratorDestroyedSound;

	idleSound = Turret_GeneratorIdleSound;

	generatorPower = 1;

	shapeFile = "./dts/power_panel.dts";

	maxDamage = 150;

	boundingBox = vectorScale("2.5 2.5 2.2", 4);
	crouchBoundingBox = vectorScale("2.5 2.5 2.2", 4);

	UIName = "T2: Base Solar Panel";
};

function Turret_TribalBasePanel::turretOnPowerLost(%db, %obj)
{
	Turret_TribalBaseGenerator::turretOnPowerLost(%db, %obj);
}

function Turret_TribalBasePanel::turretOnPowerRestored(%db, %obj)
{
	Turret_TribalBaseGenerator::turretOnPowerRestored(%db, %obj);
}

function Turret_TribalBasePanel::turretOnDisabled(%db, %obj, %src)
{
	Turret_TribalBaseGenerator::turretOnDisabled(%db, %obj, %src);
}

function Turret_TribalBasePanel::turretOnDestroyed(%db, %obj, %src)
{
	Turret_TribalBaseGenerator::turretOnDestroyed(%db, %obj, %src);
}

function Turret_TribalBasePanel::turretOnRecovered(%db, %obj, %src)
{
	Turret_TribalBaseGenerator::turretOnRecovered(%db, %obj, %src);
}

function Turret_TribalBasePanel::turretOnRepaired(%db, %obj, %src)
{
	Turret_TribalBaseGenerator::turretOnRepaired(%db, %obj, %src);
}

function Turret_TribalBasePanel::onAdd(%db, %obj)
{
	Turret_TribalBaseGenerator::onAdd(%db, %obj);
}

function Turret_TribalBasePanel::turretCanMount(%db, %pl, %img)
{
	return Turret_TribalBaseGenerator::turretCanMount(%db, %pl, %img);
}