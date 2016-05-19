#pragma once

#ifndef __INPUT_H__
#define __INPUT_H__

// Forward Declarations
namespace sf
{
	class RenderWindow;
	template<typename T>
	class Rect;
	template<typename T>
	class Vector2;
}

/* Input class is used to abstract input from the user into functions.
 * Currently handles closing the window, mouse movement, and left clicking.
 */
class Input
{
private:
	bool mouseHeld;
	int mouseDownPositionX, mouseDownPositionY;
	int mouseCurrentPositionX, mouseCurrentPositionY;
public:
	Input();
	~Input();
	void LoadEvents(sf::RenderWindow &window);
	bool MouseHeld();
	bool MouseClickCollide(sf::Rect<int> &rect);
	void MouseClickLocation(sf::Vector2<int> &location);
	void MouseLocation(sf::Vector2<int> &location);
};

#endif