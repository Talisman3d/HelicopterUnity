ReadMeActual

To get up to speed where I'm at:
- Clone repository locally
- Enable Git large file storage LFS
- In Unity, navigate to assets/scenesSandbox and add the Sandbox scene to the hierarchy
- Hit play

Before doing any editing, please branch from Main in your git instance.
I am using Visual Studio Code as my code editor per the recommendation from a Udemy Unity course.

In the Sandbox scene, you should see a platform, a few helicopters, a landing pad, and a sphere.

Player: 
You control the PlayerHelicopter child 'PlayerHeli' with a controller. I'm using a DS4. I haven't added support for keyboard because I wanted to focus on PWD from -1 to 1 and not just -1 AND 1
Controls:
Yaw - Left stick left and right
Roll - Right stick left and right
Pitch - Right stick forward and aft
Lift - Left and Right triggers
Machine gun - Right bumper
Rocket Volley - square for DS4, button west

You should be able to change helicopter types simply by deleting PlayerHeli, and replacing it with a helicopter type within assets/prefabs/Helicopters


FriendlyAI:
The 3 children of FriendlyAIHelicopters are attempting to navigate to the Tracking Sphere. They are very bad at this.
To investigate their behavior, navigate to Scripts/behaviors/friendlyBehavior. Couple of things to note here:
- friendlyBehavior will eventually just flip between behaviors, and the logic to move to target will be abstracted, so the player or enemy can use it too
- the moveToTarget behavior is my intended script for this, but it should also depend on the type of rotorcraft, as they handle differently and so should have different PID gains

anyways, in the meantime, mess with the pitch,roll,yaw,lift code and the values of gains for these in the Unity Inspector. The yaw PID is so bad that just using pure linear error correcting is better.

Weapons:
I plan to do some abstracting so that there is a primary and secondary weapon, and logic to handle whichever weapon type and behavior is slotted into these. For now, it's hard locked to a gun and rocket.