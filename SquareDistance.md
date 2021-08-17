# Square distance

I define the square distance as how big a square I can fit between 2 points.

The formula is simple, given 2 points (x1,y1) and (x2,y2),  the square distance is:

`d = Max(|x2-x1|, |y2-y1|)`

This is useful because I can use it to determine if 2 squares touch.  For example,  if there is a square centered at (x1,y1) and another square centered at (x2,y2),  and the squares are of size n,  then the squares touch if and only if `d < n`

Note this is different from the Euclidean distance, which is the distance between two points in a straight line.

It is also different from the Manhattan distance, which measures how far a taxi would travel between two points in roads in a city block.
