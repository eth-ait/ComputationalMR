using System.Windows;
using ConstraintUI.Controller;
using ConstraintUI.Model;
using ConstraintUI.Optimization;
using ConstraintUI.View;

namespace ConstraintUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ViewModel _viewModel;
        private OptimizationModel _optimizationModel;
        private readonly AppModel _appModel;

        private readonly AppController _appController;

        public MainWindow()
        {
            _optimizationModel = new OptimizationModel();
            _appModel = new AppModel(_optimizationModel);

            _viewModel = new ViewModel(_appModel);

            _appController = new AppController(_appModel, _viewModel);
            _appController.InitFakeViewModel(this);
            _appController.InitModels();

            DataContext = _viewModel;

            InitializeComponent();
        }

        private void OnOptimizeClick(object sender, RoutedEventArgs e)
        {
            _appController.StartOptimization();
        }

        private void OnResetClick(object sender, RoutedEventArgs e)
        {
            _appController.ResetSolution();
        }

        private void OnParameterSliderChanged(object sender, RoutedEventArgs routedEventArgs)
        {
            if (_viewModel.AutoOptimizeEnabled)
                _appController.StartOptimization();
        }

        private void OnSelectedSolutionSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _appController.SelectSolution((int)e.NewValue);
            _appController.UpdateModelForCurrentSolution();
        }
    }
}
