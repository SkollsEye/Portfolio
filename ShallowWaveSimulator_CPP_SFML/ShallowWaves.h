#pragma once

#ifndef __SHALLOW_WAVES_H__
#define __SHALLOW_WAVES_H__

// Forward Declarations
namespace sf
{
	class RenderWindow;
	class Time;
	template <typename T>
	class Rect;
}
class Input;

/* ShallowWaves runs and displays a shallow wave simulation within a specified area.
 */
class ShallowWaves
{
public:
	ShallowWaves(sf::Rect<int> waterRect);
	~ShallowWaves();
	void Update(Input& input, sf::Time elapsedTime);
	void Draw(sf::RenderWindow& window);
private:
	void RunSimulation();
	int Convert2Dto1D(int x, int y);
	std::vector<float> oldDepth, waveDepth, newDepth;
	bool clickRegistered;
	int waveCellSize;
	int waveLeftX, waveTopY;
	int waveGridWidth, waveGridHeight;
	// Constants
	const float framesPerSecond;
	const int maxQuadCount;
	const float minDepth;
	const float maxDepth;
	const float rate;
	const float dampening;
	const float reduction;
};

#endif