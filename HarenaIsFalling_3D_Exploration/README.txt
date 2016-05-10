HARENA IS FALLING README
BY MARK DIVELBISS [SKOLLSEYE@GMAIL.COM]

Description:
I was part of a small game development group for the final capstone course of my college degree.
The game is based off of a crumbling island concept I designed and pitched to the team, which was revised to suit our skill sets and individual interests.
This game was created in Unity using the Cubiquity Voxel Engine to simulate a crumbling island.

Harena is falling; the world is ending.
The player appears in the Welcome Center of a floating paradise dubbed Harena, meaning sand.
As the player explores, the island proves to be unstable and dangerous, and the player quickly realizes they might be the last human alive on earth.
The destruction of humanity and the world appears to be related to something called the "Black Box," some great unknown technology.

As this game was made in Unity, it is written primarily in C#.
The game is currently not publicly available but CODE SAMPLES have been included in this directory from the game.

Files:
1. README.txt
---This file is intended to help guide and shed light on this aspect of the portfolio.
2. GameManager.cs
---This class is the bread and butter of the entire game. It controls scene transitions, all pause screens, and other essential functions such as notifications and music.
---This class is a Singleton created when the game starts.
3. GameProgress.cs
---Game Progress stores a single instance of the player's progress, which can be statically accessed and updated using specific values or broad skipping methods.
---Works essentially like a save file, but the game does not include a save system as it is so short. Thus, it also behaves as a Singleton.
4. InfoLoader.cs
---Loads in letter data from an XML in the Resources folder and handles any exceptions that may occur.
5. Controller.cs
---A simple class for abstracting user intention away from user input.