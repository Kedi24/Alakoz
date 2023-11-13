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
VISUAL

#### Walljumping
You can also perform walljump while airborne and near a wall. To execute a wall jump, hold into the direction of the wall you would like to jump off of, then press the jump button, `W`.
VISUAL

#### Dashing
Dashing is a mobility option that gives you a burst of speed in the direction you are facing. To perform a dash, press the `F` key. 
    
    VISUAL

You can use a dash in both the air and on the ground. Use this to clear gaps or place yourself in advantageous positions.
    
    VISUAL

It is also possible to cancel a dash with a jump. To execute this simply tap the jump button at any point during the dash. 
    
    VISUAL

Compared to a normal dash, a canceled dash has a smaller amount of cooldown. Be cautious, however, as a canceled dash does not maintain your forward momentum. 

Using these attributes in tandem with one another can create unique ways to traverse the map.
    
    VISUAL

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
