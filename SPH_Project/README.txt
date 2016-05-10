SPH PROJECT README
BY MARK DIVELBISS [SKOLLSEYE@GMAIL.COM]

Description:
The goal of this project was to perform SPH on water particles and have them flow through a maze-like environment.
A friend, Glen Giffey, and I designed the project to meet the requirements of a Computer Game and Animation class.
The project took approximately two weeks of work, 1-2 hours a day, and is still being improved upon even though the class is over.
Since the project was made in Unity, all code is written using C#.

Files:
1. SPH_Project_Package (Shortcut to Google Drive)
---The unity package that contains all the files necessary to run the project.
---My code samples are taken directly from the package and placed in this directory.
2. README.txt
---This file is intended to help guide and shed light on this aspect of the portfolio.
3. Octree.cs
---Particles in the project were organized spatially using the Octree class I wrote for the project.
---Though this class was designed for the project, the class is multi-purpose and can store any data type.
4. Maze.cs
---The Maze class is a singleton class that creates a grid-based maze once and allows all other classes to statically refer to it.
5. MazeCamera.cs
---The Maze Camera is a simple camera controller designed to smoothly pan over and around the maze.

Controls:
Arrow Keys / WASD - Pan Camera
Left Click - Add to block stack
Right Click - Remove top block on stack

Future Goals:
1. Improve on the SPH algorithm.
---SPH is fairly complicated and requires a lot of time and research to fully understand.
---While the particle behavior is adequate, it's far from perfect and still struggles with numerical instability.
2. Parallel processing on the GPU.
---Currently, there is a low cap on the number of particles the CPU can handle without lag.
---Thus, we wish to improve speed by having threads run particle processes on the GPU.
3. Replace Cubiquity with native implementation.
---Cubiquity is very useful but also large and somewhat uneccessary. We used it because we were familiar with it.
---We wish to replace it with a simple native implementation using primitive game objects for representing the maze.
4. Water Rendering
---Currently, each particle is simply rendered as a blue square, which was simple to make and useful in testing.
---Eventually, we wish to render the water's surface once the SPH algorithm is nailed down.

Credits:

Cubiquity is a free voxel engine created by an independent game development group in the Netherlands called Volumes of Fun.
Visit them at volumesoffun.com

Our smoothing kernels were taken directly from a research paper by M¨uller et al:
M¨uller, M., Charypar, D., Gross, M.: Particle-based fluid
simulation for interactive applications. In: Proc. of Siggraph
Symposium on Computer Animation, pp. 154–159 (2003)