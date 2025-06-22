using System;
using System.Collections.Generic;
using UnityEngine;

public static class GridAStar
{// Класс для представления узла сетки
    public class Node
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int G { get; set; } // Стоимость от начала до текущего узла
        public int H { get; set; } // Эвристическая оценка до конечного узла
        public int F { get { return G + H; } } // Общая оценка F = G + H
        public Node Parent { get; set; } // Родительский узел для восстановления пути

        public Node(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    // Поиск пути с использованием алгоритма A*
    public static List<Node> FindPath(bool[,] grid, Node start, Node end, int maxIterations = 1000)
    {
        int iterations = 0;
        // Инициализация открытого и закрытого списков
        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();
        openList.Add(start);

        while (openList.Count > 0 && iterations < maxIterations)
        {
            // Находим узел с наименьшей F стоимостью
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].F < currentNode.F ||
                    (openList[i].F == currentNode.F && openList[i].H < currentNode.H))
                {
                    currentNode = openList[i];
                }
            }

            // Перемещаем текущий узел из открытого списка в закрытый
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // Если достигли конечного узла, восстанавливаем путь
            if (currentNode.X == end.X && currentNode.Y == end.Y)
            {
                //Debug.Log("Iterations: " + iterations);
                return ReconstructPath(currentNode);
            }

            // Генерируем соседние узлы
            List<Node> neighbors = new List<Node>();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    // Пропускаем текущий узел и диагонали
                    if ((x == 0 && y == 0) || (x != 0 && y != 0))
                        continue;

                    int neighborX = currentNode.X + x;
                    int neighborY = currentNode.Y + y;

                    // Проверяем, что сосед находится в пределах сетки
                    if (neighborX >= 0 && neighborX < grid.GetLength(0) &&
                        neighborY >= 0 && neighborY < grid.GetLength(1))
                    {
                        // Проверяем, что соседняя клетка проходима
                        if (!grid[neighborX, neighborY])
                        {
                            neighbors.Add(new Node(neighborX, neighborY));
                        }
                    }
                }
            }

            // Обрабатываем каждого соседа
            foreach (Node neighbor in neighbors)
            {
                if (closedList.Contains(neighbor))
                    continue;

                // Стоимость перемещения в соседнюю клетку (1 для горизонтали/вертикали)
                int tentativeG = currentNode.G + 1;

                // Если сосед еще не в открытом списке или найден более короткий путь
                if (!openList.Contains(neighbor) || tentativeG < neighbor.G)
                {
                    neighbor.Parent = currentNode;
                    neighbor.G = tentativeG;
                    neighbor.H = CalculateHeuristic(neighbor, end);

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
            iterations++;
        }
        Debug.LogWarning("A* превысил максимальное количество итераций!");
        // Если открытый список пуст, а конечная точка не достигнута - путь не найден
        return null;
    }

    // Эвристическая функция (манхэттенское расстояние)
    private static int CalculateHeuristic(Node a, Node b)
    {
        int dx = Math.Abs(a.X - b.X);
        int dy = Math.Abs(a.Y - b.Y);
        return (dx + dy) + Math.Min(dx, dy) * (1 - 2); // Комбинация манхэттенского и диагонального
    }

    // Восстановление пути от конечного узла к начальному
    private static List<Node> ReconstructPath(Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }

    public static bool[,] ConvertIntArrayToBoolArray(int[,] intArray)
    {
        if (intArray == null)
            throw new ArgumentNullException(nameof(intArray));

        int rows = intArray.GetLength(0);
        int cols = intArray.GetLength(1);

        bool[,] boolArray = new bool[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                boolArray[i, j] = intArray[i, j] != 0;
            }
        }

        return boolArray;
    }
}