ReadMeActual

To get up to speed where I'm at:
- Clone repository locally
- Enable Git large file storage LFS
- In Unity, navigate to assets/scenesSandbox and add the Sandbox scene to the hierarchy
- Hit play

/////////////////////////////////////////////////////////////////////
Before doing any editing, please branch from Main in your git instance.
/////////////////////////////////////////////////////////////////////


I am using Visual Studio Code as my code editor per the recommendation from a Udemy Unity course.

In the Sandbox scene, you should see a platform, a few helicopters, a landing pad, and a sphere.

[]Player: 
You control the PlayerHelicopter child 'PlayerHeli' with a controller. I'm using a DS4. I haven't added support for keyboard because I wanted to focus on PWD from -1 to 1 and not just -1 AND 1
Controls:
Yaw - Left stick left and right
Roll - Right stick left and right
Pitch - Right stick forward and aft
Lift - Left and Right triggers
Machine gun - Right bumper
Rocket Volley - square for DS4, button west
Tracking missile - left bumper

You should be able to change helicopter types simply by deleting PlayerHeli, and replacing it with a helicopter type within assets/prefabs/Helicopters


[]FriendlyAI:
The 3 children of FriendlyAIHelicopters are attempting to navigate to the Tracking Sphere. They are very bad at this.
To investigate their behavior, navigate to Scripts/behaviors/friendlyBehavior. and Scripts/behaviors/MoveToTarget. the moveToTarget behavior is my intended script for this, but it should also depend on the type of rotorcraft, as they handle differently and so should have different PID gains
anyways, in the meantime, mess with the pitch,roll,yaw,lift code and the values of gains for these in the Unity Inspector. The yaw PID is so bad that just using pure linear error correcting is better.

[] Physics system:
Nothing too crazy. Navigate to Assets/Prefabs. ThrustRotor Variant can only apply thrust in line with the origin facing up, positive or negative. Fullrotor Variant can apply thrust and also moments to tilt the rotor.
Whenever thrust is applied to either rotor type, a reaction torque is simulated and applied to the parent helicopter. Instead of calculating that torque reaction, I just set it in the Collective script/inspector so you can fudge it for fun.
The rotors are spinning via a script that rotates them, this has no bearing on the physics system and is for visuals only. 
Rotor size has no bearing on the physics/power either, these maximum values are handled on the helicopter object under the specific flight controller script.


[]Helicopter design and control system:
Theoretically any configuration with any number of rotors is possible, we just have to setup the hierarchy correctly and program a flight controller to interpret input and determine rotor behavior output.

I made 6 helicopter prefabs using the relatively modular rotors setup. Right now I have flight controller programmed for 5 different 'types' of rotorcraft. Find these in Assets/Scripts/flightControllers.
- Conventional: Vertical Main Rotor (Fullrotor Variant) and Horizontal tail rotor (ThrustRotor Variant)
- Coax: 2 vertical Main Rotors (Fullrotor Variants), stacked, and a tail rotor facing aft (ThrustRotor Variant)
- quad: 4 rotors (ThrustRotor Variants)
- sidebyside: 2 vertical Main Rotors (Fullrotor Variants), side by side
- Tandem: 2 vertical Main Rotors (Fullrotor Variants), forward and aft

If you want to make your own using one of these flight control schemes, feel free to take an existing prefab, modify the design, and then add it back into prefabs as an original with a new name.
If you want to make your own design using a flight control scheme that doesn't exist here, you'll need to:
- Make a new flight controller script that inherits from FlightController
- add or subtract GameObject slots in the script and attach rotor prefabs into the inspector to match the number and types of rotors
- Program flight logic for ApplyLift, ApplyPitch, ApplyRoll, ApplyYaw, ApplyThrust
- In Scripts/ControllerInputHandling/PlayerInputController add an else if for your controller type.

[]Weapons:
I plan to do some abstracting so that there is a primary and secondary weapon, and logic to handle whichever weapon type and behavior is slotted into these. For now, it's hard locked to a gun and rocket.