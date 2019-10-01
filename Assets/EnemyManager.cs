// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;

// namespace Voidheart {
//     // handles all AI behavior for enemies
//     public class EnemyManager : SerializedSingleton<EnemyManager> {
//         public VBoard board;
//         private static VDijkstraPathfinding _pathfinder = new VDijkstraPathfinding();

//         // Dictionary containing scores for each cell for a given board state
//         private Dictionary<VCell, int> cellScores;

//         private void Start() {
//             TurnManagerSystem.Instance.OnEnemyPhaseEnter += StartEnemyTurn;
//             TurnManagerSystem.Instance.OnEnemyResolutionPhaseEnter += EndEnemyTurn;
//         }

//         /// <summary>
//         /// This is the main AI function. Starts by calculating a score for
//         /// each cell by distance from heroes. 
//         /// </summary>
//         public void StartEnemyTurn() {
//             List<VEnemyUnit> enemies = board.GetAllEnemies();

//             MakeCellScores();

//             foreach (VEnemyUnit enemy in enemies) {
//                 var availableDestinations = GetAvailableDestinations(board.Cells, enemy, 3); // TODO: replace 3 with actual movement cost

//                 // moveScores contains the score of each valid move
//                 var moveScores = new Dictionary<VCell, int>();
//                 foreach (VCell cell in availableDestinations) {
//                     moveScores.Add(cell, cellScores[cell]);
//                 }
//                 var sortedMoveScore = moveScores.OrderByDescending(x => x.Value);

//                 // destination cell is where the unit is going to move to, defaulting to its original position
//                 VCell destinationCell = board.GetCell(enemy.Coordinate);
//                 if (sortedMoveScore.Count() == 1) {
//                     destinationCell = sortedMoveScore.First().Key;
//                 } else if (sortedMoveScore.Count() > 1) {
//                     // randomly choose between the top two choices if there are more than 1
//                     destinationCell = sortedMoveScore.ElementAt(Random.Range(0, 2)).Key;
//                 }
//                 board.MoveUnit(enemy, destinationCell.Coordinate);

//                 foreach (VCell target in destinationCell.GetNeighbours(board.Cells)) {
//                     BoardMarkingSingleton.Instance.CreateHighlightsWithTags(new List<Coord> {
//                         target.Coordinate
//                     }, new List<string> {
//                         "EnemyAttack"
//                     }, BoardMarkingSingleton.Instance.redColor);
//                 }
//             }

//             cellScores.Clear();
//             TurnManagerSystem.Instance.EndPhase(TurnPhase.ENEMY);
//         }

//         /// <summary>
//         /// Undo highlights and resolve attacks
//         /// </summary>
//         public void EndEnemyTurn() {
//             BoardMarkingSingleton.Instance.Remove(board.Cells.Select(cell => cell.Coordinate), new List<string> {
//                 "EnemyAttack"
//             });
//             List<VEnemyUnit> enemies = board.GetAllEnemies();
//             foreach (VEnemyUnit enemy in enemies) {
//                 foreach (VCell target in board.GetCell(enemy.Coordinate).GetNeighbours(board.Cells)) {

//                     board.DealDamageToCoord(target.Coordinate, 5); // TODO: use actual attack patterns and damage numbers
//                 }
//             }
//             TurnManagerSystem.Instance.EndPhase(TurnPhase.RESOLUTION);
//         }

//         /// <summary>
//         /// Calculates the score for a given board state, based on each cell's distance from Orion and Ophelia
//         /// </summary>
//         void MakeCellScores() {
//             cellScores = new Dictionary<VCell, int>();
//             foreach (VCell cell in board.Cells) {
//                 cellScores.Add(cell, 0);
//             }
//             VCell opheliaCell = board.GetCell(board.GetUnitByCardType(CardType.OPHELIA).Coordinate);
//             RippleScores(opheliaCell, 5);
//             VCell orionCell = board.GetCell(board.GetUnitByCardType(CardType.ORION).Coordinate);
//             RippleScores(orionCell, 5);
//         }

//         /// <summary>
//         /// Recursive function that ripples a decreasing scores across the board, only updating if the current score is higher
//         /// </summary>
//         void RippleScores(VCell cell, int score) {
//             cellScores[cell] = score;
//             if (score <= 0) return;
//             foreach (VCell neighbor in cell.GetNeighbours(board.Cells)) {
//                 if (cellScores[neighbor] < score - 1) {
//                     RippleScores(neighbor, score - 1);
//                 }
//             }
//         }

//         /// <summary>
//         /// Method returns all cells that the unit is capable of moving to.
//         /// </summary>
//         public virtual HashSet<VCell> GetAvailableDestinations(List<VCell> cells, VEnemyUnit enemy, int movementPoint) {
//             enemy.cachedPaths = new Dictionary<VCell, List<VCell>>();

//             var edges = GetGraphEdges(cells);
//             var paths = _pathfinder.findAllPaths(edges, board.GetCell(enemy.Coordinate));
//             foreach (var key in paths.Keys) {
//                 if (!key.IsTraversable || key.IsOccupied) continue;
//                 var path = paths[key];

//                 var pathCost = path.Sum(c => c.MovementCost);
//                 if (pathCost <= movementPoint) {
//                     enemy.cachedPaths.Add(key, path);
//                 }
//             }
//             return new HashSet<VCell>(enemy.cachedPaths.Keys);
//         }

//         /// <summary>
//         /// Method returns graph representation of cell grid for pathfinding.
//         /// </summary>
//         protected virtual Dictionary<VCell, Dictionary<VCell, int>> GetGraphEdges(List<VCell> cells) {
//             Dictionary<VCell, Dictionary<VCell, int>> ret = new Dictionary<VCell, Dictionary<VCell, int>>();
//             foreach (var cell in cells) {
//                 if (cell.IsTraversable) {
//                     ret[cell] = new Dictionary<VCell, int>();
//                     foreach (var neighbour in cell.GetNeighbours(cells).FindAll(c => c.IsTraversable)) {
//                         ret[cell][neighbour] = neighbour.MovementCost; // cost of moving to that cell
//                     }
//                 }
//             }
//             return ret;
//         }
//     }
// }