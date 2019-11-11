using System;
using System.Collections.Generic;

namespace CastleOnTheGrid
{    
    class Program
    {
        const int Sides = 3;
        const int GoalX = 0;
        const int GoalY = 2;
        const int StartX = 0;
        const int StartY = 0;
        public class Coordinate
        {            
            public int X;
            public int Y;
            public Coordinate(int x, int y)
            {
                // Hold cartesian coordinates
                X = x;
                Y = y;
            }

            public Coordinate Translate(int r, int u)
            {
                // Translate a coordinate by a vector
                return new Coordinate(X + r, Y + u);
            }

            public List<Coordinate> GenerateVicinity()
            {
                // This function will determine the valid moves. The rest of the program is a general solution for this
                // kind of problem. To change how moves are decided, for example a difference piece, you can create a new
                // algorithm for determining those moves.
                List<Coordinate> Output = new List<Coordinate>();


                // Translation vectors for each direction of rook movement: up, down, left, right. Coordinate
                // struct is used because it already has X and Y fields, but in this case a vector translation.
                Coordinate[] TranslationVectors = new Coordinate[]
                {
                    new Coordinate(1,0),
                    new Coordinate(-1,0),
                    new Coordinate(0,1),
                    new Coordinate(0,-1)
                };
                for (int i = 0; i < 4; i++)
                {
                    // 1 * translation vector
                    int Offset = 1;

                    // Iterate in each direction until moves are no longer valid. If you hit a wall/border, there is no way
                    // a rook could get past that, so the loop can break there. All coordinates in between could be important
                    while (true)
                    {
                        // Translate by the vector, multiplied by the offset. E.g one move will be one in that direction, the
                        // second two in that direction.
                        Coordinate CurrentCoord = Translate(Offset*TranslationVectors[i].X, Offset*TranslationVectors[i].Y);

                        // Check if new coordinate is still valid
                        if (ValidateCoord(CurrentCoord))
                        {
                            // Add the valid move
                            Output.Add(CurrentCoord);                            
                            Offset++;
                        }
                        else
                        {
                            // Break because obstacle reached
                            break;
                        }
                    }                                   
                }
                return Output;
                // For single cell movement between nodes(rook can move in infinte straight lines).
                //return new Coordinate[]
                //{
                //    Translate(1,0),
                //    Translate(-1,0),
                //    Translate(0,1),
                //    Translate(0,-1),
                //};
            }
            public override string? ToString() =>  $"({X},{Y})";            
        }
        public class Node
        {
            public List<Node> Chain = new List<Node>();
            public Coordinate Coord;
            public int Moves = 0;
            public bool Exists = false;
            public Node(Coordinate coord, int dist, bool exists=true)
            {                
                // Create a starting node
                Exists = exists;
                Coord = coord;
                Moves = dist;                
                Chain.Add(this);
            }

            public Node(Node node, int dist, List<Node> chain)
            {
                // Create a node chained onto another node
                Exists = true;
                Coord = node.Coord;
                Moves = dist;
                Chain.AddRange(chain);
                Chain.Add(this);
            }
            public override string? ToString() => Coord.ToString();            
        }
        public class Map
        {
            private Node[,] InnerMap;
            public Map(int sides, Node StartNode)
            {
                InnerMap = new Node[sides, sides];

                // All moves will be relative to this node
                Set(StartNode);
            }
            public Node Get(Coordinate input)
            {
                // Return value if exists, or empty node(solves null checking elsewhere)
                if(InnerMap[input.Y, input.X] != null)
                {
                    return InnerMap[input.Y, input.X];
                }
                else
                {
                    return new Node(input, 0, false);
                }
            }
            public void Set(Node node)
            {
                InnerMap[node.Coord.Y, node.Coord.X] = node;
            }
            public string[] Layout()
            {
                // Create a string representation
                string[] Output = new string[InnerMap.GetLength(0)];

                // Iterate through rows
                for (int i = 0; i < InnerMap.GetLength(0); i++)
                {
                    // Iterate through columns
                    for (int j = 0; j < InnerMap.GetLength(1); j++)
                    {
                        // Add null if blocked or not pathed yet
                        if (InnerMap[i, j] == null)
                        {
                            Output[i] += " X ";
                        }
                        else
                        {
                            Output[i] += $" {InnerMap[i, j].Moves} ";
                        }
                    }
                }
                return Output;
            }

            public string[] ShowPath(Node node)
            {
                // Show the shortest path to a node denoted with $s
                string[] Output = new string[InnerMap.GetLength(0)];
                for (int i = 0; i < InnerMap.GetLength(0); i++)
                {
                    for (int j = 0; j < InnerMap.GetLength(1); j++)
                    {
                        if (InnerMap[i, j] == null)
                        {
                            Output[i] += " X ";
                        }
                        else
                        {
                            bool Found = false;
                            foreach (Node PathNode in node.Chain)
                            {
                                if (PathNode.Coord.X == j && PathNode.Coord.Y == i)
                                {
                                    Output[i] += $" $ ";
                                    Found = true;
                                    break;
                                }
                            }
                            if (!Found)
                            {
                                Output[i] += "   ";
                            }
                        }                       
                        
                        
                    }
                }
                return Output;
            }
        }

        static string[] Grid = new string[]
        {
            ".X.",
            ".X.",
            "...",
        };
        static void Main(string[] args)
        {
            // BFS queue
            Queue<Node> SearchQueue = new Queue<Node>();
                       
            Node StartNode = new Node(new Coordinate(StartX, StartY), 0);
            SearchQueue.Enqueue(StartNode);
            Map NodeMap = new Map(Sides, StartNode);

            // Continue until all paths have been evaluated
            while (SearchQueue.Count > 0)
            {
                // Get next node to evaluate
                Node CurrentPosition = SearchQueue.Dequeue();

                // Generate possible moves in vicinity
                List<Coordinate> PossibleMoves = CurrentPosition.Coord.GenerateVicinity();
                for (int i = 0; i < PossibleMoves.Count; i++)
                {
                    // Get the node that already exists at that coordinate
                    Node ExistingNode = NodeMap.Get(PossibleMoves[i]);

                    // If the node was evaluated from a shorter path, leave it as it is, otherwise erase the old path
                    // and replace with the new shorter path. This is similar to a Dijkstra's shortest path approach,
                    // google that if unsure.
                    if (ExistingNode.Moves > CurrentPosition.Moves || !ExistingNode.Exists)
                    {
                        // Add extra move on and the chain of the current node. Replace the coordinates of the existing node
                        Node PotentialNode = new Node(ExistingNode, CurrentPosition.Moves + 1, CurrentPosition.Chain);

                        if (!ExistingNode.Exists)
                        {
                            // If this node has not already been evaluated to lead to new paths, queue it to check out later
                            // Google Breadth first search.
                            SearchQueue.Enqueue(PotentialNode);
                        }

                        // Overwrite the old node as new one is shorter
                        NodeMap.Set(PotentialNode);

                        // Output to screen
                        Console.WriteLine(string.Join('\n', NodeMap.Layout()));
                        Console.WriteLine("");
                    }                    
                }                
            }

            // Get information of goal node
            Node GoalNode = NodeMap.Get(new Coordinate(GoalX, GoalY));

            // If it exists, a path was found
            if (GoalNode.Exists)
            {
                Console.WriteLine(GoalNode.Moves);

                // Output the vector path to the goal
                Console.WriteLine(string.Join('\n', NodeMap.ShowPath(GoalNode)));
            }
            else
            {
                Console.WriteLine("Could not find path!");
            }

            // Full view of all shortest paths
            Console.WriteLine("Result:\n" + string.Join('\n', NodeMap.Layout()));

        }
        public static bool ValidateCoord(Coordinate c)
        {
            // True if in bounds of Sides, or if not an X.
            return c.X >= 0 && c.Y >= 0 && c.X < Sides && c.Y < Sides && Grid[c.Y][c.X] != 'X';            
        }
    }
}
