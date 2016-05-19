#pragma once

#ifndef __PAINT_H__
#define __PAINT_H__

// Forward Declarations
namespace sf
{
	class RenderWindow;
	template <typename T>
	class Rect;
}

/* Paint is a static class used to draw objects to a window given a set of parameters.
 */
class Paint
{
private:
	Paint();
	~Paint();
public:
	static void PaintWater(sf::RenderWindow& window, sf::Rect<int>& waterArea, int quadLength, std::vector<float>& depth);
};

#endif