using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Markup;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace RxUI_Sample.ViewModels
{
    public class ParentViewModel : ReactiveObject, IRoutableViewModel
    {
        private SourceList<ChildViewModel> _childViewModels;
        private ReadOnlyObservableCollection<ChildViewModel> _childViewModelsView;

        public ParentViewModel(IScreen screen)
        {
            HostScreen = screen;

            _childViewModels = new SourceList<ChildViewModel>();

            var changeSet = _childViewModels.Connect();

            changeSet
                .AutoRefresh(x => x.IsSelected)
                .Filter(x => x.IsSelected)
                .Transform(x => x.WhenAnyValue(vm => vm.IsSelected).Select(_ => x))
                .DisposeMany()
                .MergeMany(x => x)
                .Subscribe(selectedChildViewModel =>
                {

                });

            changeSet.Bind(out _childViewModelsView)
                .Subscribe();

            AddChild = ReactiveCommand.Create(() => _childViewModels.Add(new ChildViewModel()));

            DeleteChild = ReactiveCommand.Create<ChildViewModel>(vm => _childViewModels.Remove(vm));
        }

        public ReadOnlyObservableCollection<ChildViewModel> ChildViewModels => _childViewModelsView;

        public ReactiveCommand<ChildViewModel, Unit> DeleteChild { get; set; }

        public ReactiveCommand<Unit, Unit> AddChild { get; set; }

        public string UrlPathSegment { get; } = "parent";
        public IScreen HostScreen { get; }
    }
}
