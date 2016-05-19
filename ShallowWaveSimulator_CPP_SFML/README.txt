SHALLOW WAVE SIMULATOR README
BY MARK DIVELBISS [SKOLLSEYE@GMAIL.COM]

Description:
The goal of this project was to display ability to code in C++ using the SFML library.
The project uses a simple shallow wave simulation algorithm to allow the user
to click the blue area and create ripples as though it were water.

Files:
1. ShallowWaveSimulator.zip
---Zipped executable containing the simulator along with some DLLs from SFML's library.
---My code samples are the source code for the executable and placed in this directory.
2. README.txt
---This file is intended to help guide and shed light on this aspect of the portfolio.
3. main.cpp
---Main C++ file where program begins execution. Controls setup, game loop, and teardown.
4. Properties.h
---Properties header file.
5. Properties.cpp
---Singleton class used to store useful constants and methods.
---Handles the creation of the window and determined water area.
6. Input.h
---Input header file.
7. Input.cpp
---The input class abstracts user input to the program so that it is easier to manage.
8. ShallowWaves.h
---ShallowWaves header file.
9. ShallowWaves.cpp
---ShallowWaves class handles the creation, updating, and drawing of a shallow wave simulation.
---Pass the constructor the specified area you wish to run the simulation in relative to any window.
10. Paint.h
---Paint header file.
11. Paint.cpp
---Static class with private constructor.
---Houses methods for drawing objects to the window given a set of parameters.

Controls:
Left Click - Create Ripple