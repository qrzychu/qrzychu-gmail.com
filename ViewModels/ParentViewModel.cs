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
        private string _filterText;
        private ReadOnlyObservableCollection<ChildViewModel> _filteredChildren;
        private ObservableAsPropertyHelper<int> _filterdCount;

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


            // filter is reaplied every time this observable fires
            var filterObservable = this.WhenAnyValue(x => x.FilterText)
                .Throttle(TimeSpan.FromMilliseconds(50), RxApp.MainThreadScheduler)
                .Select(text => new Func<ChildViewModel, bool>(vm =>
                    string.IsNullOrEmpty(text) || (vm.Text?.ToLowerInvariant().Contains(text.ToLowerInvariant()) ?? false) || vm.IsSelected)
                    );


            changeSet
                .AutoRefresh(x => x.Text)
                .AutoRefresh(x => x.IsSelected)
                .Filter(filterObservable)
                .Bind(out _filteredChildren)
                .Subscribe();

            _filterdCount = _filteredChildren.ToObservableChangeSet()
                .ToCollection() // this can be expensive, because it creates a full list of items after each change, but until you have 100+ items you are good
                .Select(x => x.Count)
                .ToProperty(this, x => x.FilteredCount);
                    

            AddChild = ReactiveCommand.Create(() => _childViewModels.Add(new ChildViewModel()));

            DeleteChild = ReactiveCommand.Create<ChildViewModel>(vm =>
            {
                _childViewModels.Remove(vm);
                vm.IsSelected = true;
            });
        }

        public int FilteredCount => _filterdCount?.Value ?? 0;

        public string FilterText
        {
            get => _filterText;
            set => this.RaiseAndSetIfChanged(ref _filterText, value);
        }

        public ReadOnlyObservableCollection<ChildViewModel> FilteredChildren => _filteredChildren;

        public ReadOnlyObservableCollection<ChildViewModel> ChildViewModels => _childViewModelsView;

        public ReactiveCommand<ChildViewModel, Unit> DeleteChild { get; set; }

        public ReactiveCommand<Unit, Unit> AddChild { get; set; }

        public string UrlPathSegment { get; } = "parent";
        public IScreen HostScreen { get; }
    }
}
