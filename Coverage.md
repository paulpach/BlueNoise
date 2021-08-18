# Square Coverage

If we place samples in a grid,  we can easily make sure that all parts of the map are under a square object.

However that is not desireable, squares should be placed in seemingly random places with random space between them.  

So just measuring how much percentage of the map is covered by one object is not the best measurement to tell if an algorithm is good.

I am also _not_ trying to increase density per se.  The amount of samples per cells are not measured,  however,  good algorithms tend to have higher density.

Instead,  the ideal algoritm will fill the space in such way that no new sample can be placed without violating the minimum distance requirements.

I consider an algorithm to be maximal, if there is no position x,y whose nearest sample is farther than the minimum distance. Pach4 is fully maximal.  It will completely fill up the space with no room for any more samples.  

Pach2, Pach3 and other future algorithms are not be maximal.  To measure how good the algorithm fills the space,  I calculate how many more additional samples I can place without violating the constraints. 

For example,  In a 30x30 grid, Pach2 was able to find 480 samples. I can place 38 additional samples without violating the constraints,  thus Pach2 found `480 / (38 + 480) = 92%` of the samples.

Yet another way to look at this coverage is to place squares of size `2n * 2n`.  If I cover the entire plane, then the algorithm is maximal.