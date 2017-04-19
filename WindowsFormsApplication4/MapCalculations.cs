﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication4
{
	class MapCalculations
	{
		private List<HexagonButton> _queue = new List<HexagonButton>();
		private List<HexagonButton> _pathsToEdge = new List<HexagonButton>();
		private Random rnd = new Random();

		public void calculateRoutes(HexagonButton[,] hexMap, HexagonButton startingHex)
		{
			resetAllButtons(hexMap);
			_queue.Add(startingHex);

			while(_queue.Any())
			{
				HexagonButton currentHex = _queue.First();
				_queue.Remove(_queue.First());
				currentHex.Visited = true;
				if (currentHex.IsEdgeTile == false)
				{
					foreach (HexagonButton hex in currentHex.neighbourList)
					{
						if (hex.Visited == false && hex.Passable == true)
						{
							hex.parent = currentHex;
							_queue.Add(hex);
							hex.Visited = true;
						}
					}
				} else
				{
					_pathsToEdge.Add(currentHex);
				}
			}
			List<HexagonButton> shortestRoutes = findShortestRoutes(_pathsToEdge);
			List<HexagonButton> shortestRouteByRand = chooseRouteByRand(shortestRoutes);

			Console.WriteLine($"The route starts with: ({startingHex.XCoordinate}, {startingHex.YCoordinate})");
			foreach (HexagonButton hex in shortestRouteByRand)
			{
				Console.WriteLine($"The route goes by: ({hex.XCoordinate}, {hex.YCoordinate})");
			}
		}

		private List<HexagonButton> findShortestRoutes(List<HexagonButton> edgeHexList)
		{
			var shortestRoutes = new List<HexagonButton>();
			foreach (HexagonButton hex in edgeHexList)
			{
				hex.CostToStart = checkParent(0, hex);

				if (shortestRoutes.Count == 0)
					shortestRoutes.Add(hex);
				else if (shortestRoutes.First().CostToStart > hex.CostToStart)
				{
					shortestRoutes.Clear();
					shortestRoutes.Add(hex);
				}
				else if (shortestRoutes.First().CostToStart == hex.CostToStart)
					shortestRoutes.Add(hex);
			}
			return shortestRoutes;
		}

		private int checkParent(int count, HexagonButton hexToCheck)
		{
			if(hexToCheck.parent == null)
			{
				return count;
			} else
			{
				return checkParent(count + 1, hexToCheck.parent);
			}
		}

		private List<HexagonButton> chooseRouteByRand(List<HexagonButton> shortestRoutes)
		{
			var shortestRouteByRand = new List<HexagonButton>();
			int routeToChoose = rnd.Next(0, shortestRoutes.Count);

			HexagonButton edgeHex = shortestRoutes.ElementAt(routeToChoose);
			HexagonButton currentHex = edgeHex;

			do
			{
				shortestRouteByRand.Add(currentHex);
				currentHex.BackColor = System.Drawing.Color.FromArgb(50, 205, 50);
				currentHex = currentHex.parent;
			} while (currentHex.parent != null);

			shortestRouteByRand.Reverse();
			
			return shortestRouteByRand;
		}

		private void resetAllButtons(HexagonButton[,] hexMap)
		{
			foreach (HexagonButton hex in hexMap)
			{
				hex.Visited = false;
				hex.parent = null;
				if (hex.BackColor == System.Drawing.Color.FromArgb(50, 205, 50))
				{
					hex.BackColor = System.Drawing.Color.LightGray;
				}
			}
		}
	}
}
