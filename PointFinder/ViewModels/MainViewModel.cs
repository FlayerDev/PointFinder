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

    public ICommand AddMeasurementCommand { get; }
    public ICommand CalculateCommand { get; }

    public MainViewModel()
    {
        AddMeasurementCommand = new RelayCommand(AddMeasurement);
        CalculateCommand = new RelayCommand(Calculate);
        NewPointA = string.Empty;
        NewPointB = string.Empty;
        resultText = "Awaiting for measurements...";
    }

    private void AddMeasurement()
    {
        Measurements.Add(new DistanceMeasurement(NewPointA, NewPointB, NewDistance));
        NewPointA = string.Empty;
        NewPointB = string.Empty;
        NewDistance = 0.0;
    }

    private void Calculate()
    {
        ResultText = "Calculating coordinates...\n";
        List<PointEstimate> pointEstimates = CoordinateSolver.EstimateCoordinates(Measurements);
        ResultText = $"Found {pointEstimates.Count} points:\n\n";
        foreach (var estimate in pointEstimates)
        {
            ResultText += $"{estimate.Name}: ({estimate.X:0.000}, {estimate.Y:0.000})\n";
        }
    }

    public ObservableCollection<DistanceMeasurement> Measurements { get; } = new ObservableCollection<DistanceMeasurement>
    {
        new DistanceMeasurement("A", "B", 12.5),
        new DistanceMeasurement("B", "C", 7.3),
        new DistanceMeasurement("C", "D", 5.8)
    };
}
