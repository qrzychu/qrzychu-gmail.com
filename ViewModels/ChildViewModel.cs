using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;

namespace RxUI_Sample.ViewModels
{
    public class ChildViewModel : ReactiveObject
    {
        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }
    }
}
