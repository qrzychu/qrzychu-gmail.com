using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;
using RxUI_Sample.ViewModels;

namespace RxUI_Sample.Views
{
    // this enables the xaml designer - it doesn't like generics
    public class MainViewBase : ReactiveUserControl<MainViewModel> { }

    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : MainViewBase
    {
        public MainView()
        {
            InitializeComponent();

            // this is THE BEST thing in RxUI
            this.WhenActivated(d =>
            {
                // do something when the view is activated
                ViewModel.GetFilters.Execute().Subscribe().DisposeWith(d);

                this.OneWayBind(ViewModel, x => x.Filters, view => view.SidePanel.ItemsSource).DisposeWith(d);
            });
        }
    }
}
