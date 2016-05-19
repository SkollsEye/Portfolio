#include <SFML/Graphics.hpp>
#include "Input.h"

// Default Constructor
Input::Input()
{
	mouseDownPositionX = 0;
	mouseDownPositionY = 0;
	mouseCurrentPositionX = 0;
	mouseCurrentPositionY = 0;
	mouseHeld = false;
}

Input::~Input() {}

// Used to poll and store events that occur within window.
void Input::LoadEvents(sf::RenderWindow& window)
{
	// Get the current position of the mouse relative to the window.
	sf::Vector2<int> mousePosition = sf::Mouse::getPosition(window);
	mouseCurrentPositionX = mousePosition.x;
	mouseCurrentPositionY = mousePosition.y;

	// Event Checking
	sf::Event event;
	while (window.pollEvent(event))
	{
		switch (event.type)
		{
		case sf::Event::Closed: // Close Window
			window.close();
			break;
		case sf::Event::MouseButtonPressed: // Left Click check, store click location
			if (event.mouseButton.button == sf::Mouse::Button::Left)
			{
				mouseHeld = true;
				mouseDownPositionX = mousePosition.x;
				mouseDownPositionY = mousePosition.y;
			}
			break;
		case sf::Event::MouseButtonReleased: // Left Release check
			if (event.mouseButton.button == sf::Mouse::Button::Left)
				mouseHeld = false;
			break;
		}
	}
}

// Returns true if the mouse is being held down
bool Input::MouseHeld() { return mouseHeld; }

// Returns true if mouse click occured within specified region (relative to the window last used in LoadEvents)
bool Input::MouseClickCollide(sf::Rect<int> &rect) { return rect.contains(sf::Vector2i(mouseDownPositionX, mouseDownPositionY)); }

// Stores the mouse click X/Y position in location (relative to the window last used in LoadEvents)
void Input::MouseClickLocation(sf::Vector2<int> &location)
{
	location.x = mouseDownPositionX;
	location.y = mouseDownPositionY;
}

// Stores the mouse's last known X/Y position in location (relative to the window last used in LoadEvents)
void Input::MouseLocation(sf::Vector2<int> &location)
{
	location.x = mouseCurrentPositionX;
	location.y = mouseCurrentPositionY;
}