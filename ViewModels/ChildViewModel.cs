using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;

namespace RxUI_Sample.ViewModels
{
    public class ChildViewModel : ReactiveObject
    {
        private bool _isSelected;
        private string _text;

        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }

        public string Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }
    }
}
