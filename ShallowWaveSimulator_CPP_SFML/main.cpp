#include <SFML/Graphics.hpp>
#include "ShallowWaves.h"
#include "Input.h"
#include "Properties.h"

int main()
{
	// Setup window, clock, input, and waves
	sf::RenderWindow window;
	Properties::Instance()->CreateWindow(window);
	sf::Clock clock;
	Input input;
	sf::Rect<int> waterRect;
	Properties::Instance()->GetWaterRect(waterRect);
	ShallowWaves waves(waterRect);

	while (window.isOpen()) // Game Loop, terminate if window is closed
	{
		// Events
		input.LoadEvents(window);

		// Logic
		waves.Update(input, clock.restart());
		
		// Render
		window.clear();
		waves.Draw(window);
		window.display();
	}

	return 0;
}