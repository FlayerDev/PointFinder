using CommunityToolkit.Mvvm.ComponentModel;

namespace PointFinder.Models;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class DistanceMeasurement : ObservableObject
{
    [ObservableProperty]
    private string pointA;

    [ObservableProperty]
    private string pointB;

    [ObservableProperty]
    private double distance;

    public DistanceMeasurement(string pointA, string pointB, double distance)
    {
        this.pointA = pointA;
        this.pointB = pointB;
        this.distance = distance;
    }
}