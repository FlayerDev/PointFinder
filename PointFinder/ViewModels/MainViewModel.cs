using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PointFinder.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

using PointFinder.Services;
using System.Collections.Generic;

namespace PointFinder.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string resultText;
    [ObservableProperty]
    private string newPointA;
    [ObservableProperty]
    private string newPointB;
    [ObservableProperty]
    private double newDistance;


    [ObservableProperty]
    private string newPoint;
    [ObservableProperty]
    private double newXPosition;
    [ObservableProperty]
    private double newYPosition;

    public ICommand AddMeasurementCommand { get; }
    public ICommand AddLocationCommand { get; }
    public ICommand CalculateCommand { get; }

    public MainViewModel()
    {
        AddMeasurementCommand = new RelayCommand(AddMeasurement);
        CalculateCommand = new RelayCommand(Calculate);
        AddLocationCommand = new RelayCommand(AddLocation);
        NewPointA = string.Empty;
        NewPointB = string.Empty;
        NewDistance = 0.0;
        NewPoint = string.Empty;
        NewXPosition = 0.0;
        NewYPosition = 0.0;
        resultText = "Awaiting for measurements...";
    }

    private void AddMeasurement()
    {
        Measurements.Add(new DistanceMeasurement(NewPointA, NewPointB, NewDistance));
        NewPointA = string.Empty;
        NewPointB = string.Empty;
        NewDistance = 0.0;
    }

    private void AddLocation()
    {
        Locations.Add(new InitialPosition(NewPoint, NewXPosition, NewYPosition));
        NewPoint = string.Empty;
        NewXPosition = 0.0;
        NewYPosition = 0.0;
    }

    private void Calculate()
    {
        ResultText = "Calculating coordinates...\n";
        List<PointEstimate> pointEstimates = CoordinateSolver.EstimateCoordinates(Measurements, Locations);
        ResultText = $"Found {pointEstimates.Count} points:\n\n";
        foreach (var estimate in pointEstimates)
        {
            ResultText += $"{estimate.Name}: ({estimate.X:0.000}, {estimate.Y:0.000})\n";
        }
    }

    public ObservableCollection<DistanceMeasurement> Measurements { get; } = new ObservableCollection<DistanceMeasurement>
    {
        new DistanceMeasurement("A", "B", 373.5),
        new DistanceMeasurement("B", "C", 476.3),
        new DistanceMeasurement("C", "D", 373.6),
        new DistanceMeasurement("C", "A", 293),
        new DistanceMeasurement("B", "D", 293.9)
    };
    public ObservableCollection<InitialPosition> Locations { get; } = new ObservableCollection<InitialPosition>
    {
        new InitialPosition("A", 0, 0),
        new InitialPosition("B", 1, 0),
        new InitialPosition("C", 0, 0),
        new InitialPosition("A", 0, 0),
    };
}
