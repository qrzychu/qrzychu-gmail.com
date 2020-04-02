using System;
using System.Collections.Generic;
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
    public class ParentViewBase : ReactiveUserControl<ParentViewModel> { }

    /// <summary>
    /// Interaction logic for ParentView.xaml
    /// </summary>
    public partial class ParentView : ParentViewBase
    {
        public ParentView()
        {
            InitializeComponent();
        }
    }
}
