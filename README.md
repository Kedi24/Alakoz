Alakoz
===

## Table of Contents

* [Introduction](#introduction)
* [Gameplay](#gameplay)
* [Content](#content)
* [References](#references)

## Introduction <a name ="introduction"></a>
Alakoz is a 2D Roguelike Platformer developed using C# and the MonoGame framework. In this game, players will traverse through levels using a unique set of skills and abilities to reach the end goal. Take caution, however, as hordes of enemies and obstacles stand between you and the finish line. Do you have what it takes to reach the end?

As developers, Alakoz is a passion project and our first step into game development. To become familiar with some of the tools used in game development, we have decided to build the game using C#, MonoGame, .NET, and Tiled as they are common within the industry. All in all, we hope to grow in our knowledge of programming and understanding of game development, while also creating an enjoyable experience for audiences.

## Gameplay <a name ="gameplay"></a>
As characterized by the genre, roguelikes traditionally involve “runs” where you must battle through enemies and maneuver around obstacles to reach the goal. Failure to complete these tasks, however, can be fatal as the player's death at any given moment will end the run. With this in mind, you must use the skills and abilities at your disposal to gain the upper hand.


#### Controls
For basic movement, use the `W` `A` `S` `D` keys to jump, move left, move right, and crouch.

https://github.com/Kedi24/Alakoz/assets/117780105/cb26cce4-1004-49a3-87f1-6e40b6b16910

#### Wallcling
In Alakoz, you can cling to the sides of walls. Clinging to a wall will decrease your falling speed, making it easier to get a better view of any obstacles below you or to time a jump. While airborne, simply hold in the direction of the wall that you would like to cling to. You can exit the cling at any time by pressing in the opposite direction.

https://github.com/Kedi24/Alakoz/assets/117780105/dbd7a75c-ca20-4ae5-a098-bd2cc4335b82

#### Wall jumping
While clinging to a wall, you can also perform a wall jump. To execute a wall jump, you must first cling to a wall as mentioned before. Once on the wall, simply tap the jump button, `W`, to perform the wall jump. If you come into contact with another wall during the wall jump, you will be able to perform more wall jumps in quick succession. Wall jumping is a useful tool to reach places where a normal jump can't reach. 

https://github.com/Kedi24/Alakoz/assets/117780105/caefff46-5f6b-48b1-90ff-f23fe02038fe

#### Dashing
Dashing is a mobility option that gives you a burst of speed in the direction you are facing. To perform a dash, press the `F` key. 
    
https://github.com/Kedi24/Alakoz/assets/117780105/b8d4b653-70fc-4ea5-a77b-2c218f79b652

You can use a dash in both the air and on the ground. Use this to clear gaps or place yourself in advantageous positions.

https://github.com/Kedi24/Alakoz/assets/117780105/4d15795a-8570-4aae-b025-fd9033b165f3

It is also possible to cancel a dash with a jump. To execute this simply tap the jump button at any point during the dash. 

https://github.com/Kedi24/Alakoz/assets/117780105/3fccbb76-15ea-4aff-9a90-06aa3d4fc740

Compared to a normal dash, a canceled dash has a smaller amount of cooldown. Be cautious, however, as a canceled dash does not maintain your forward momentum. 

Using these attributes in tandem with one another can create unique ways to traverse the map.

## Content <a name ="content"></a>

### Currently implemented features

So far, we have successfully implemented the basic traits of a platformer, such as:

* Player Control
* Collision Detection
* Basic Enemy movement
* Basic Levels with obstacles

As well as some general game design features: 
* Game States 
* Camera Control

### For future updates
Here is a list of some of the features we plan to tackle in future updates.
* Complex Enemies
    * Enemies will utilize a path-finding algorithm to chase the player around the map using their own unique abilities.
* Player Attacks and Skill Tree
    * A set of Basic attacks that can be used at any time.
    * A set of classes the player can choose from, such as a Ranged class and Melee class. These classes will change the types of moves the character has.
    * A dense Skill Tree will accommodate the players' choice of class. Each skill will perform its own action distinct from the others. 
* A Save File system

## References <a name="references"></a>
