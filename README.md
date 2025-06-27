
# Point Finder

A tool for converting real-life distances between points into coordinates.

Getting real-world measurements into CAD software isn’t always straightforward.
Suppose you want to locate point A, but you only know its distances from two reference points, B and C. You might run into ambiguity: the circles defined by those distances can intersect at two locations.

In practice, measurement errors make things worse — the circles might not even intersect properly.

PointFinder solves this by adjusting the shape, bending it just enough so that all measured distances are as close to accurate as possible. It then calculates and outputs the estimated coordinates of each point.
