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
    public static List<PointEstimate> EstimateCoordinates(ObservableCollection<DistanceMeasurement> distances)
    {
        var uniquePoints = distances.SelectMany(d => new[] { d.PointA, d.PointB }).Distinct().ToList();
        var pointIndices = uniquePoints.Select((p, i) => (p, i)).ToDictionary(p => p.p, p => p.i);

        // Fix A at (0,0) and B at (distance, 0) if available
        var fixedPoints = new Dictionary<string, (double X, double Y)>
        {
            [uniquePoints[0]] = (0, 0)
        };

        if (distances.Any(d => d.PointA == uniquePoints[0] || d.PointB == uniquePoints[0]))
        {
            var firstLink = distances.First(d => d.PointA == uniquePoints[0] || d.PointB == uniquePoints[0]);
            string otherPoint = firstLink.PointA == uniquePoints[0] ? firstLink.PointB : firstLink.PointA;
            fixedPoints[otherPoint] = (firstLink.Distance, 0);
        }

        // Parameters to optimize: only X and Y for non-fixed points
        var variables = new List<string>();
        foreach (var p in uniquePoints)
        {
            if (!fixedPoints.ContainsKey(p))
            {
                variables.Add(p + "_x");
                variables.Add(p + "_y");
            }
        }

        // Initial guess: 0s
        var initial = new double[variables.Count];

        // Define objective function
        Func<Vector<double>, double> objective = (Vector<double> vars) =>
        {
            var pointCoords = new Dictionary<string, (double X, double Y)>(fixedPoints);
            int varIndex = 0;

            foreach (var p in uniquePoints)
            {
                if (!fixedPoints.ContainsKey(p))
                {
                    double x = vars[varIndex++];
                    double y = vars[varIndex++];
                    pointCoords[p] = (x, y);
                }
            }

            double error = 0;
            foreach (var d in distances)
            {
                var pa = pointCoords[d.PointA];
                var pb = pointCoords[d.PointB];
                double dx = pa.X - pb.X;
                double dy = pa.Y - pb.Y;
                double actualDist = Math.Sqrt(dx * dx + dy * dy);
                error += Math.Pow(actualDist - d.Distance, 2);
            }

            return error;
        };

        var solver = new NelderMeadSimplex(1e-5, 10000);
        var result = solver.FindMinimum(ObjectiveFunction.Value(objective), Vector<double>.Build.DenseOfArray(initial));

        // Build final coordinates
        var finalCoords = new Dictionary<string, (double X, double Y)>(fixedPoints);
        int finalIndex = 0;

        foreach (var p in uniquePoints)
        {
            if (!fixedPoints.ContainsKey(p))
            {
                double x = result.MinimizingPoint[finalIndex++];
                double y = result.MinimizingPoint[finalIndex++];
                finalCoords[p] = (x, y);
            }
        }

        return finalCoords.Select(kv => new PointEstimate(kv.Key, kv.Value.X, kv.Value.Y)).ToList();
    }
}