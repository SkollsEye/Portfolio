#include <SFML/Graphics.hpp>
#include "ShallowWaves.h"
#include "Input.h"
#include "Paint.h"

// ShallowWaves Constructor (NO DEFAULT)
// Simulates shallow waves within the given region waterRect (may not use the full region)
// Updates simulation with the given FPS (frames per second)
ShallowWaves::ShallowWaves(sf::Rect<int> waterRect)
	: maxQuadCount(100)
	, minDepth(0)
	, maxDepth(2)
	, rate(0.005)
	, dampening(0.999)
	, reduction(0.000005)
	, framesPerSecond(80)
{
	waveCellSize = waterRect.width > waterRect.height ? (int)ceil(waterRect.width / (double)maxQuadCount) : (int)ceil(waterRect.height / (double)maxQuadCount);
	if (waveCellSize <= 0) waveCellSize = 1;
	waveLeftX = waterRect.left;
	waveTopY = waterRect.top;
	waveGridWidth = waterRect.width / waveCellSize + 1;
	waveGridHeight = waterRect.height / waveCellSize + 1;
	clickRegistered = false;
	oldDepth.resize(waveGridWidth * waveGridHeight, minDepth);
	waveDepth.resize(waveGridWidth * waveGridHeight, minDepth);
	newDepth.resize(waveGridWidth * waveGridHeight, minDepth);
}

ShallowWaves::~ShallowWaves() {}

// Updates Shallow Waves
void ShallowWaves::Update(Input &input, sf::Time elapsedTime)
{
	static sf::Time timeKeeper;
	timeKeeper += elapsedTime;
	sf::Time updateTime = sf::seconds(1 / framesPerSecond);

	// Detect input for ripple (left click)
	if (input.MouseHeld())
	{
		sf::IntRect waterRect(waveLeftX, waveTopY, (waveGridWidth - 1) * waveCellSize, (waveGridHeight - 1) * waveCellSize);

		// Ensure this click hasn't already been seen and that the click is within shallow wave boundaries
		if (!clickRegistered && input.MouseClickCollide(waterRect))
		{
			clickRegistered = true;
			sf::Vector2i loc;
			input.MouseClickLocation(loc);
			waveDepth[Convert2Dto1D((loc.x - waveLeftX) / waveCellSize, (loc.y - waveTopY) / waveCellSize)] = maxDepth;
		}
	}
	else clickRegistered = false; // Mouse is not held down, unregister click

	// Run Depth Simulation based on timer
	if (timeKeeper > updateTime)
	{
		RunSimulation();
		timeKeeper -= updateTime;
	}
}

// Updates depth values using newDepth and oldDepth to simulate ripples
void ShallowWaves::RunSimulation()
{
	// Determine what the new depth for each point on the grid should be
	for (int i = 0; i < waveDepth.size(); i++)
	{
		newDepth[i] = waveDepth[i]
					+ (waveDepth[i] - oldDepth[i]) * dampening
						+ (waveDepth[(i % waveGridWidth - 1 >= 0 ? i - 1 : i)] // inline checking for boundaries (left)
						+ waveDepth[(i % waveGridWidth + 1 < waveGridWidth ? i + 1 : i)] // inline checking for boundaries (right)
						+ waveDepth[(i - waveGridWidth >= 0 ? i - waveGridWidth : i)] // inline checking for boundaries (up)
						+ waveDepth[(i + waveGridWidth < waveDepth.size() ? i + waveGridWidth : i)] // inline checking for boundaries (down)
						- 4 * waveDepth[i] ) * rate
					- reduction;

		// Keep all values within min/max
		if (newDepth[i] < minDepth) newDepth[i] = minDepth;
		else if (newDepth[i] > maxDepth) newDepth[i] = maxDepth;
	}

	// Store current values in old and new values in current
	for (int i = 0; i < waveDepth.size(); i++)
	{
		oldDepth[i] = waveDepth[i];
		waveDepth[i] = newDepth[i];
	}
}

// Returns integer representation of the 2D coordinates X/Y, ensuring return value is a waveDepth index
int ShallowWaves::Convert2Dto1D(int x, int y)
{
	int value = x + y * waveGridWidth;
	return (value >= 0 && value < (int)waveDepth.size() ? value : 0);
}

// Draw Shallow Waves
void ShallowWaves::Draw(sf::RenderWindow& window)
{
	// Draw water with depth waveDepth
	sf::IntRect waterRect(waveLeftX, waveTopY, (waveGridWidth - 1) * waveCellSize, (waveGridHeight - 1) * waveCellSize);
	Paint::PaintWater(window, waterRect, waveCellSize, waveDepth);
}