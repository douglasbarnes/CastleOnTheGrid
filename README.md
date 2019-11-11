# CastleOnTheGrid
Solution to hackerrank Castle on the Grid problem.

To this problem, I took a breadth first search approach. In this problem, the important detail to notice is that the path does not matter, rather the moves to get that path. After finding an
effective path, you can use the concept of vectors(regardless of movements of the rook) to evaluate solutions in reasonable time. I didn't consider good algorithmic time an important factor here,
this is my first graph theory problem that I have directly solved using graph theory. Because of this, my solution contains a few more features you might find interesting.


The premise of the solution can be modelled quite easily,

![Model](https://i.imgur.com/LjD1uvU.png)

The complex pathing is concentrated at the top. There are multiple paths that could be taken to get to the same point.

![Model](https://i.imgur.com/kURWuGH.png)

The trick is to find the shortest paths to the junction nodes, where the rook could stop and make a meaninful/optimal decision, then consider the paths that branch from there.

![Model](https://i.imgur.com/W6hCPYR.png)

To do this, you iterate every valid move until there is a new node evaluated for the first time(not necessary a junction, any node), queue it, then continue the current path.
When the current path has finished, you get the next item from the queue and evaluate that path. This is because the old paths don't matter, only the fact that it was shown to
be possible to get to that coordinate in X number of moves. After the whole evaluation of the map has finished(every combination of path has been found to each node), the old paths
that were ignored can be treat like a normalised vector to the goal location, as the question asks for the number of moves(magnitude of vector) not the path.
In my solution, I decided to preserve the old paths called "chains". This allows for a display at the end of the shortest path to the goal, not just the shortest number of moves.

![Model](https://i.imgur.com/sU4dCiG.png)

Also, you end up with a nice general solution to a breadth first search problem. Coordinate.GenerateVicinity() provides moves for a rook(and single place horizontal vertical movement in comments), but
can be replaced with any move-producing algorithm to solve a similar problem.

When implementing (hopefully your own) solution to hackerrank, nota bene that the coordinate system used there is strange. It is neither (X,Y), or (Column,Row), rather (Row, Column) like a 2D array.
To get around this, you can swap the names of StartX and StartY and have a sensible (Column,Row) coordinate system. 
