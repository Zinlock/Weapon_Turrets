// ballistics code by Conan
// he saved me from having to learn quadratics :]

// slightly edited version
// * leading ignores gravity for jetting targets
// * separated getProjectileVector and getProjectilePosition
// * edited ground check to use a box search instead of a raycast
// * added optional high arc mode

function getProjectileVector(%target, %projVel, %projGravityMod, %initialPosition, %high)
{
    if (!isObject(%target))
    {
        return %muzzleVector;
    }
    else //need to lead target
    {
        return vectorNormalize(vectorSub(getProjectilePosition(%target, %projVel, %projGravityMod, %initialPosition, %high), %initialPosition));
    }
}

function getProjectilePosition(%target, %projVel, %projGravityMod, %initialPosition, %high)
{
    if (!isObject(%target))
    {
        return %muzzleVector;
    }
    else //need to lead target
    {
        if (isFunction(%target.getClassName(), "getHackPosition")) { %targetPos = %target.getHackPosition(); }
        else { %targetPos = %target.getWorldBoxCenter(); }

        %basePos = %target.getPosition();
        %diff = vectorSub(%targetPos, %basePos);
        %targetVel = %target.getVelocity();
        %muzzleSpeed = %projVel;

        %iterFinalPos = calculateLeadLocation_Iterative(%target, %initialPosition, %basePos, %muzzleSpeed, %targetVel, %projGravityMod, %diff, %high);
        return %iterFinalPos;
    }
}

//support functions
function boxEmpty(%pos, %box, %mask, %e0, %e1, %e2, %e3)
{
	initContainerBoxSearch(%pos, %box, %mask);
	while(isObject(%col = containerSearchNext()))
	{
		if(%col == %e0 || %col == %e1 || %col == %e2 || %col == %e3)
			continue;
		
		return false;
	}

	return true;
}

function calculateLeadLocation_Iterative(%obj, %pos0, %pos1, %speed0, %vel1, %gravity0, %diff, %high)
{
    %projectileGravity = %gravity0 * 9.81;

    %masksR = $Typemasks::StaticObjectType | $TypeMasks::VehicleObjectType;
    %masksB = $TypeMasks::fxBrickObjectType | $Typemasks::PlayerObjectType;

    %downRay = containerRaycast(vectorAdd(%obj.getPosition(), "0 0 0.1"), vectorAdd(%obj.getPosition(), "0 0 -0.1"), %masksR, %obj, %obj.getObjectMount());
		%box = setWord(vectorScale(getWords(%obj.getObjectBox(), 3, 6), 1/2), 2, "0.1");
		%air = boxEmpty(%obj.getPosition(), %box, %masksB, %obj);

    if (isObject(%downRay) || !%air || (%obj.getType() & $TypeMasks::PlayerObjectType) && %obj.isJetting() ||
	      %target.getClassName() $= "FlyingVehicle" || %target.getDatablock().lift > 0)
				 { %gravity = 0; }
    else { %gravity = 9.8; }

    %offset = "0 0 " @ %diff;
    %currTime = vectorDist(%pos0, %pos1) / %speed0;
    %finalPos = vectorAdd(calculateFutureGravityPosition(%pos1, %vel1, %currTime, %gravity), %offset);

    //iteratively test and update the projectile direction used to target the target
    //predicts target future position based on target's velocity and gravity (%gravity)
    // talk("iter: 0 time: " @ mFloatLength(%currTime, 2) @ " ray: " @ %downRay);
    for (%i = 1; %i <= 16; %i++)
    {
        if (%projectileGravity <= 0) //projectile is not affected by gravity
        {
            %nextTime = vectorDist(%finalPos, %pos0) / %speed0;
        }
        else //projectile is affect by gravity
        {
            %zDiff = getWord(%finalPos, 2) - getWord(%pos0, 2);
            %xyDiff = vectorDist(getWords(%finalPos, 0, 1), getWords(%pos0, 0, 1));
            %theta = solveGravityProjectileTheta(%speed0, %xyDiff, %zDiff, %projectileGravity, %high);
            %nextTime = solveGravityProjectileTime(%theta, %xyDiff, %speed0);
        }
        %nextFinalPos = vectorAdd(calculateFutureGravityPosition(%obj, %pos1, %vel1, %nextTime, %gravity), %offset);
        %nextDelta = mAbs(%nextTime - %currTime);

        // talk("iter: " @ %i @ " time: " @ mFloatLength(%nextTime, 2) @ " gravity: " @ %gravity);
        if (%nextDelta < 0.01)
        {
            %currTime = %nextTime;
            %finalPos = %nextFinalPos;
            break;
        }
        else if (%i > 6 && %nextDelta > %lastDelta)
        {
            break;
        }
        %currTime = %nextTime;
        %finalPos = %nextFinalPos;
        %lastDelta = %nextDelta;
    }
    if (%projectileGravity <= 0)
    {
        return %finalPos;
    }
    else
    {
        %zDiff = getWord(%finalPos, 2) - getWord(%pos0, 2);
        %xyDiff = vectorDist(getWords(%finalPos, 0, 1), getWords(%pos0, 0, 1));
        %theta = solveGravityProjectileTheta(%speed0, %xyDiff, %zDiff, %projectileGravity, %high);
        %nextTime = solveGravityProjectileTime(%theta, %xyDiff, %speed0);

        %xyComp = mCos(%theta) * %speed0;
        %zComp = mSin(%theta) * %speed0;
        // fcn(conan).centerprint("gravity: " @ %projectileGravity SPC "theta: " @ mFloatLength(%theta / 3.14159 * 180, 1) SPC "zComp: " @ %zComp SPC "xyComp: " @ %xyComp, 2);
        %xyVec = vectorNormalize(vectorSub(getWords(%finalPos, 0, 1), getWords(%pos0, 0, 1)));
        %finalVec = vectorAdd(vectorScale(%xyVec, %xyComp), "0 0 " @ %zComp);
        return vectorAdd(%pos0, %finalVec);
    }
}

function calculateFutureGravityPosition(%obj, %pos, %vel, %time, %gravity)
{
    %xy = getWords(%vel, 0, 1);
    %z = getWords(%vel, 2);

    %xyPos = vectorAdd(vectorScale(%xy, %time), %pos);
    %zDelta = (%z * %time) - (%gravity * %time * %time);
    %finalPos = vectorAdd(%xyPos, "0 0 " @ %zDelta);

    %masks = $TypeMasks::fxBrickObjectType | $Typemasks::StaticObjectType | $Typemasks::PlayerObjectType;
    %ray = containerRaycast(vectorAdd(%pos, "0 0 0.01"), %finalPos, %masks, %obj);
    %hit = getWord(%ray, 0);
    %hitloc = getWords(%ray, 1, 3);
    if (isObject(%hit) && (%hit.getClassName() !$= "fxDTSBrick" || %hit.isColliding()))
    {
        %finalPos = %hitloc;
    }
    // echo(%hit SPC " | " @ %hitloc @ " | " @ %finalPos);
    return %finalPos;
}

function solveQuadratic(%a, %b, %c)
{
    %discriminant = (%b * %b) - (4 * %a * %c);
    if (%discriminant < 0)
    {
        return "";
    }
    %s1 = (-1 * %b + mSqrt(%discriminant)) / (2 * %a);
    %s2 = (-1 * %b - mSqrt(%discriminant)) / (2 * %a);
    return %s1 SPC %s2; 
}

//https://www.forrestthewoods.com/blog/solving_ballistic_trajectories/assets/img/07.png
//gravity positive value
//%s = speed, %xy = horizontal distance, %z = vertical distance, %g = gravity
function solveGravityProjectileTheta(%s, %xy, %z, %g, %high)
{
    %discriminant = mPow(%s, 4) - %g * (%g * %xy * %xy + 2 * %s * %s * %z);
    if (%discriminant <= 0)
    {
        return 3.14159265 / 4;
    }
    %a0 = mAtan(((%s * %s) + mSqrt(%discriminant)) / %g / %xy, 1);
    %a1 = mAtan(((%s * %s) - mSqrt(%discriminant)) / %g / %xy, 1);

		if(!%high)
			return %a1 SPC %a0; //return smaller theta first
		else
			return %a0 SPC %a1;
}
function solveGravityProjectileTime(%theta, %xyDiff, %speed)
{
    return %xyDiff / mCos(%theta) / %speed;
}