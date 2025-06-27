namespace PointFinder.Models;

using CommunityToolkit.Mvvm.ComponentModel;


public partial class InitialPosition : ObservableObject
{
    [ObservableProperty]
    private string point;

    [ObservableProperty]
    private double xPosition;

    [ObservableProperty]
    private double yPosition;

    public InitialPosition(string Point, double X, double Y)
    {
        this.point = Point;
        this.xPosition = X;
        this.yPosition = Y;
    }
}
