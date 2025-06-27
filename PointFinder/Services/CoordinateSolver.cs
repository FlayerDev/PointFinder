namespace PointFinder.Services;

using MathNet.Numerics.Optimization;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;

using PointFinder.Models;
using System.Collections.ObjectModel;

public record PointEstimate(string Name, double X, double Y);

public static class CoordinateSolver
{
    public static List<PointEstimate> EstimateCoordinates(ObservableCollection<DistanceMeasurement> distances, ObservableCollection<InitialPosition> initialPositions)
    {
        var uniquePoints = distances
            .SelectMany(d => new[] { d.PointA, d.PointB })
            .Distinct()
            .ToList();

        var pointIndices = uniquePoints
            .Select((p, i) => (p, i))
            .ToDictionary(p => p.p, p => p.i);

        // Always fix the first point at (0, 0)
        var fixedPoints = new Dictionary<string, (double X, double Y)>
        {
            [uniquePoints[0]] = (0, 0)
        };

        // Try to fix a second point on x-axis using a known distance
        var ab = distances.FirstOrDefault(d => d.PointA == uniquePoints[0] || d.PointB == uniquePoints[0]);
        if (ab != null)
        {
            var other = ab.PointA == uniquePoints[0] ? ab.PointB : ab.PointA;
            fixedPoints[other] = (ab.Distance, 0);
        }

        // Convert initial positions to dictionary
        var initialGuessDict = initialPositions
            .GroupBy(ip => ip.Point.Trim())
            .ToDictionary(
                g => g.Key,
                g => (X: g.First().XPosition, Y: g.First().YPosition)
            );

        // Variables to optimize: only X and Y for non-fixed points
        var variables = new List<string>();
        foreach (var p in uniquePoints)
        {
            if (!fixedPoints.ContainsKey(p))
            {
                variables.Add(p + "_x");
                variables.Add(p + "_y");
            }
        }

        // Build initial guess vector
        var initial = new double[variables.Count];
        var rand = new Random();
        int idx = 0;

        foreach (var p in uniquePoints)
        {
            if (!fixedPoints.ContainsKey(p))
            {
                if (initialGuessDict.TryGetValue(p, out var guess))
                {
                    initial[idx++] = guess.X;
                    initial[idx++] = guess.Y;
                }
                else
                {
                    initial[idx++] = rand.NextDouble() * 1000 - 500; // Wide spread
                    initial[idx++] = rand.NextDouble() * 1000 - 500;
                }
            }
        }

        // Objective function
        Func<Vector<double>, double> objective = (Vector<double> vars) =>
        {
            var pointCoords = new Dictionary<string, (double X, double Y)>(fixedPoints);
            int i = 0;
            foreach (var p in uniquePoints)
            {
                if (!fixedPoints.ContainsKey(p))
                {
                    pointCoords[p] = (vars[i++], vars[i++]);
                }
            }

            double error = 0;
            foreach (var d in distances)
            {
                var a = pointCoords[d.PointA];
                var b = pointCoords[d.PointB];
                double dx = a.X - b.X;
                double dy = a.Y - b.Y;
                double actual = Math.Sqrt(dx * dx + dy * dy);
                error += Math.Pow(actual - d.Distance, 2);
            }

            return error;
        };

        var solver = new NelderMeadSimplex(1e-4, 1000);
        var result = solver.FindMinimum(
            ObjectiveFunction.Value(objective),
            Vector<double>.Build.DenseOfArray(initial)
        );

        // Rebuild final coordinates
        var coords = new Dictionary<string, (double X, double Y)>(fixedPoints);
        int finalIndex = 0;
        foreach (var p in uniquePoints)
        {
            if (!fixedPoints.ContainsKey(p))
            {
                coords[p] = (
                    result.MinimizingPoint[finalIndex++],
                    result.MinimizingPoint[finalIndex++]
                );
            }
        }

        return coords.Select(kv => new PointEstimate(kv.Key, kv.Value.X, kv.Value.Y)).ToList();
    }
}