#pragma once

#ifndef __PROPERTIES_H__
#define __PROPERTIES_H__

// Forward Declarations
namespace sf
{
	class RenderWindow;
	template<typename T>
	class Rect;
}

/* Properties is a Singleton that defines behavior and constants specific to the project as whole.
 */
class Properties
{
private:
	Properties();
	~Properties();
	static Properties instance;
	int windowWidth, windowHeight;
	// Constants
	const char *title;
	const int margins;
	const int minDimensions;
	const int maxDimensions;
public:
	static Properties *Instance();
	void CreateWindow(sf::RenderWindow& window);
	void GetWaterRect(sf::Rect<int> &rect);
};

#endif