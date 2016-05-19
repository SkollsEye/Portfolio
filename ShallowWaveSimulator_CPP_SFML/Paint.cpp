#include <SFML/Graphics.hpp>
#include "Paint.h"

// Private Constructor, unused
Paint::Paint() {}

Paint::~Paint() {}

// Static method PaintWater paints a water surface with ripples using given parameters
// Uses a VertexArray of Quads to draw ripples.
// window: window to draw in
// waterRect: area where water is to be drawn
// quadLength: the side length of the quads
// depth: vector of grid points that determined the transparency of the quads
void Paint::PaintWater(sf::RenderWindow& window, sf::Rect<int>& waterRect, int quadLength, std::vector<float>& depth)
{
	// Create based water plane
	sf::RectangleShape waterPlane(sf::Vector2f(waterRect.width, waterRect.height));
	waterPlane.setPosition(waterRect.left, waterRect.top);
	waterPlane.setFillColor(sf::Color::Blue);

	// Create vertex array, there are 4 vertices for each quad in the grid
	int quadCountWidth = waterRect.width / quadLength;
	int quadCountHeight = waterRect.height / quadLength;
	sf::VertexArray ripples(sf::PrimitiveType::Quads, 4 * quadCountWidth * quadCountHeight);
	
	// Initialize values
	int gridCountWidth = quadCountWidth + 1; // Since grid points bound the quad, there is one more point along the length than there are quads
	int gridCountHeight = quadCountHeight + 1;
	int depthSize = depth.size();
	int alpha = 0;

	// Loop through each grid point (instead of quad) because every vertex on a grid point has the same transparency
	for (int gridPoint = 0; gridPoint < depthSize; gridPoint++)
	{
		// Determine GridPoint's X/Y Coords
		int GPX = gridPoint % gridCountWidth;
		int GPY = gridPoint / gridCountWidth;

		// Find the 4 vertices in the quads that line up with this grid point
		// Direction (top left, etc...) refers to the quad that vertex is responsible for drawing
		int v1 = 4 * (gridPoint - gridCountWidth - GPY) + 2; // Top Left
		int v2 = 4 * (gridPoint - gridCountWidth - GPY + 1) + 3; // Top Right
		int v3 = 4 * (gridPoint - GPY - 1) + 1; // Bottom Left
		int v4 = 4 * (gridPoint - GPY); // Bottom Right

		// Get Color alpha based on depth
		if (depth[gridPoint] < 0) alpha = 0;
		else if (depth[gridPoint] > 1) alpha = 255;
		else alpha = (int)(depth[gridPoint] * 255);

		// Get Grid Point's position in world
		sf::Vector2i gridPointPos(GPX * quadLength + waterRect.left, GPY * quadLength + waterRect.top);
		
		// Top Left Vertex on grid point
		if (GPX > 0 && GPY > 0)
		{
			ripples[v1].color = sf::Color(255, 255, 255, alpha);
			ripples[v1].position.x = gridPointPos.x;
			ripples[v1].position.y = gridPointPos.y;
		}
		
		// Top Right Vertex on grid point
		if (GPX < gridCountWidth - 1 && GPY > 0)
		{
			ripples[v2].color = sf::Color(255, 255, 255, alpha);
			ripples[v2].position.x = gridPointPos.x;
			ripples[v2].position.y = gridPointPos.y;
		}
		
		// Bottom Left Vertex on grid point
		if (GPX > 0 && GPY < gridCountHeight - 1)
		{
			ripples[v3].color = sf::Color(255, 255, 255, alpha);
			ripples[v3].position.x = gridPointPos.x;
			ripples[v3].position.y = gridPointPos.y;
		}
		
		// Bottom Right Vertex on grid point
		if (GPX < gridCountWidth - 1 && GPY < gridCountHeight - 1)
		{
			ripples[v4].color = sf::Color(255, 255, 255, alpha);
			ripples[v4].position.x = gridPointPos.x;
			ripples[v4].position.y = gridPointPos.y;
		}
	}
	
	// Draw water plane first then ripples on top
	window.draw(waterPlane);
	window.draw(ripples);
}