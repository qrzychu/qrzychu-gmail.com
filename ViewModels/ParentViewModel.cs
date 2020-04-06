using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Windows.Markup;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using RxUI_Sample.Filters;

namespace RxUI_Sample.ViewModels
{
    public class ParentViewModel : ReactiveObject, IRoutableViewModel
    {
        private SourceList<ChildViewModel> _childViewModels;
        private ReadOnlyObservableCollection<ChildViewModel> _childViewModelsView;
        private string _filterText;
        private ReadOnlyObservableCollection<ChildViewModel> _filteredChildren;
        private ObservableAsPropertyHelper<int> _filterdCount;
        private SourceList<IFilter> _filters;
        private ReadOnlyObservableCollection<IFilter> _allFilters;
        private IObservable<Unit> _filtersChanged;

        public ParentViewModel(IScreen screen)
        {
            HostScreen = screen;

            _childViewModels = new SourceList<ChildViewModel>();

            _childViewModels.AddRange(new []
            {
                new ChildViewModel { Text = "abc"},
                new ChildViewModel { Text = "123"},
                new ChildViewModel { Text = "abc123"},
                new ChildViewModel { IsSelected = true},
            });

            var changeSet = _childViewModels.Connect();

            changeSet
                .AutoRefresh(x => x.IsSelected)
//                .Filter(x => x.IsSelected)
                .Transform(x => x.WhenAnyValue(vm => vm.IsSelected).Select(_ => x))
                .DisposeMany()
                .MergeMany(x => x)
                .Subscribe(selectedChildViewModel =>
                {

                });

            changeSet.Bind(out _childViewModelsView)
                .Subscribe();


            _filters = new SourceList<IFilter>();

            _filters.Connect()
	            .Bind(out _allFilters)
	            .Subscribe();

	        _filtersChanged = _filters.Connect()
		        .Transform(x => x.PredicateChanged)
		        .DisposeMany()
		        .MergeMany(x => x)
		        .Merge(_filters.CountChanged.Select(x => Unit.Default))
				.Do(unit => { });

            changeSet
                .FilterOnObservable(ApplyFilters)
                .ObserveOnDispatcher()
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

            AddTextFilter = ReactiveCommand.Create(() => _filters.Add(new TextFilter()));
            AddIsSelectedFilter = ReactiveCommand.Create(() => _filters.Add(new IsSelectedFilter()));

            RemoveFilter = ReactiveCommand.Create<IFilter>(filter => _filters.Remove(filter));
        }

        private IObservable<bool> ApplyFilters(ChildViewModel vm)
        {
	        return _filtersChanged.Merge(vm.WhenAnyPropertyChanged().Select(_ => Unit.Default))
		        .Select(_ => _allFilters.All(f => f.Predicate?.Invoke(vm) ?? false));
        }

        public ReactiveCommand<IFilter, Unit> RemoveFilter { get; set; }

        public ReactiveCommand<Unit, Unit> AddIsSelectedFilter { get; set; }

        public ReactiveCommand<Unit, Unit> AddTextFilter { get; set; }

        public int FilteredCount => _filterdCount?.Value ?? 0;

        public string FilterText
        {
            get => _filterText;
            set => this.RaiseAndSetIfChanged(ref _filterText, value);
        }

        public ReadOnlyObservableCollection<ChildViewModel> FilteredChildren => _filteredChildren;

        public ReadOnlyObservableCollection<ChildViewModel> ChildViewModels => _childViewModelsView;
        public ReadOnlyObservableCollection<IFilter> AllFilters => _allFilters;

        public ReactiveCommand<ChildViewModel, Unit> DeleteChild { get; set; }

        public ReactiveCommand<Unit, Unit> AddChild { get; set; }

        public string UrlPathSegment { get; } = "parent";
        public IScreen HostScreen { get; }
    }
}
