MARIO AGAIN README
BY MARK DIVELBISS [SKOLLSEYE@GMAIL.COM]

Description:
During a project course for interactive systems, our team was asked to recreate the first level of Super Mario Bros using Monogame.
I led the team to complete the project, but I wanted to go further. After the class was done, I completely refactored the code and started over.
I focused on readability and scalability within the system and took on some extra challenges to improve the game.

In focusing on readability, I avoided commenting in favor of proper naming schemes and clean code.
In focusing on scalability, I implemented a series of interfaces, most of them based on the State Design Pattern, so that new content could be created quickly and integrated into the existing system.

This game incorporates a Quadtree based collision system, wherein which all objects in the game are stored in the branches of the tree.
By doing so, the collision system could handle a far greater number of collision requests. I also implemented 2D Metaballs as a particle system for some stunning visuals.
The final result is the recreated first level followed by a series of dream-like landscapes in the Mario world.

As this game was made using the XNA framework, it is written primarily in C#.

Files:
1. MarioAgainGame.zip (Shortcut to Google Drive)
---The zip file containing the executable game. Simply unzip the contents and run the executable.
---Controls and credits are in the Game Info screen, accessible through the main menu.
---The code samples in this directory are exactly the same as the ones used in this build of the game.
2. README.txt
---This file is intended to help guide and shed light on this aspect of the portfolio.
3. IObject.cs
---Interface class used to define the basic commonality and functionality between all objects in the game.
4. Star.cs
---An example of an object implementing the IObject class.
---The star is a powerup that gives Mario invulnerability for a short time and exists in the original Super Mario Bros.
5. World.cs
---The world class represents a single level, scene or menu. They all have the same functionality, but are made up of different objects.
---All IObjects in a world are stored in a quadtree. Worlds are generated from XML's using the WorldFactory class.
6. Physics.cs
---This class can be used by an object to perform basic platforming operations.
---This class handles acceleration, friction, gravity, collision querying, collision broadcasting, and movement correction.
7. Quadtree.cs
---This class is used to store IObjects in a Quadtree spatial structure. IObjects are stored in the smallest possible node in the tree.
---This Quadtree is created once per world and is self-organizing. IObjects that fall outside the tree are marked as dead and deleted.