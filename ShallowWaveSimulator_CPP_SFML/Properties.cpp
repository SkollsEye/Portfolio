#include <SFML/Graphics.hpp>
#include "Properties.h"

// Singleton instance created when program runs
Properties Properties::instance = Properties();

// Default Constructor, initializes constants so that they can be changed without impacting the header
Properties::Properties()
	: title("Shallow Wave Simulator")
	, margins(0)
	, minDimensions(100)
	, maxDimensions(500)
{}

Properties::~Properties() {}

// Retrieve singleton (never null)
Properties *Properties::Instance() { return &instance; }

// Creates a Close style window using constants
void Properties::CreateWindow(sf::RenderWindow& window)
{
	// Determine appropriate dimensions (typically maxDimensions x maxDimensions unless small screen)
	sf::VideoMode mode = sf::VideoMode::getDesktopMode();
	windowWidth = mode.width;
	windowHeight = mode.height;

	// Ensure minimum size requirement is met
	if (windowWidth < minDimensions) windowWidth = minDimensions;
	else if (windowWidth > maxDimensions) windowWidth = maxDimensions;
	if (windowHeight < minDimensions) windowHeight = minDimensions;
	else if (windowHeight > maxDimensions) windowHeight = maxDimensions;

	// Create Window
	window.create(sf::VideoMode(windowWidth, windowHeight), title, sf::Style::Close);
}

// Determines appropriate area to run Shallow Waves using constants
void Properties::GetWaterRect(sf::Rect<int> &rect)
{
	rect.left = margins;
	rect.top = margins;
	rect.width = windowWidth - 2 * margins;
	rect.height = windowHeight - 2 * margins;
}