using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;

namespace RxUI_Sample.ViewModels
{
    public class OtherViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly IScreen _screen;

        public ReactiveCommand<Unit, Unit> GoBack { get; }

        public OtherViewModel(IScreen screen)
        {
            _screen = screen;

            GoBack = screen.Router.NavigateBack;
        }

        public string UrlPathSegment => "other";

        public IScreen HostScreen => _screen;
    }
}
