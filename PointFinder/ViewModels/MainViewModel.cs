using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PointFinder.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PointFinder.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string newPointA;
    [ObservableProperty]
    private string newPointB;
    [ObservableProperty]
    private double newDistance;

    public ICommand AddMeasurementCommand { get; }

    public MainViewModel()
    {
        AddMeasurementCommand = new RelayCommand(AddMeasurement);
        NewPointA = string.Empty;
        NewPointB = string.Empty;
    }

    private void AddMeasurement()
    {
        // implement logic
    }

    public ObservableCollection<DistanceMeasurement> Measurements { get; } = new ObservableCollection<DistanceMeasurement>
    {
        new DistanceMeasurement("A", "B", 12.5),
        new DistanceMeasurement("B", "C", 7.3),
        new DistanceMeasurement("C", "D", 5.8)
    };
}
