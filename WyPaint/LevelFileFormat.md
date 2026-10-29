## Level Files

Level files are just binary files with their own type so that only those files can be used for this program. 
Level files have the following format (From first lien to last):
1. Overall tile count (As an int)
2. Width of the map (As an int)
3. Height of the map (As an int)
4. The "first Color" (As an ARGB int)
5. Number of tiles "in a row" that have this color (As an int)
6. The "second Color" (As an ARGB int)
7. The number of tiles "in a row" that have this color (As an int)

And so on.

**Small note:** What I mean by first "color" is the color of the tile in the top left corner, as this program 
generally "reads" from left to right starting at the top left corner.