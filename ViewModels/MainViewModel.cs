using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxUI_Sample.ViewModels
{
    public class MainViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly ReadOnlyObservableCollection<BaseFilterViewModel> _filters;
        private readonly IScreen _screen;
        private SourceList<BaseFilterViewModel> _filtersSourceList;

        public MainViewModel(IScreen screen)
        {
            _screen = screen;

            _filtersSourceList = new SourceList<BaseFilterViewModel>();

            _filtersSourceList.Connect() // setup the view
                .ObserveOn(RxApp.MainThreadScheduler) // specify scheduler, possible to replace for tests
                .Bind(out _filters)
                .Subscribe();

            specialFilter = new BoolFilterViewModel();
            GetFilters = ReactiveCommand.CreateFromTask(async () =>
            {
                await Task.Delay(1000);
                return new List<BaseFilterViewModel>
                {
                    specialFilter,
                    new BoolFilterViewModel(),
                    new TextFilterViewModel{},
                    new BoolFilterViewModel(),
                    new TextFilterViewModel{},
                    new BoolFilterViewModel(),
                    new TextFilterViewModel{},
                };
            });

            GetFilters.Subscribe(filters =>
            {
                // this runs on main thread, SourceList is thread safe though
                _filtersSourceList.Clear();
                _filtersSourceList.AddRange(filters);
            });

            GetFilters.ThrownExceptions.Subscribe(ex =>
            {
                // handle exceptions
            });

            //GoToOther = screen.Router.Navigate.Execute(new OtherViewModel(_screen));
            GoToOther = ReactiveCommand.CreateFromObservable(
                () => screen.Router.Navigate.Execute(new OtherViewModel(_screen)), 
                specialFilter.WhenAnyValue(x => x.SomeFilter)
                );

            this.WhenAnyValue(x => x.SelectedFilter)
                .Select(_ => Unit.Default) // type must match command parameter type
                .InvokeCommand(ReactiveCommand.Create(() => Unit.Default)); // do something whenever user selects a filter
        }

        public ReadOnlyObservableCollection<BaseFilterViewModel> Filters => _filters;

        public string UrlPathSegment => "main";
        public IScreen HostScreen => _screen;

        private BoolFilterViewModel specialFilter;
        private BaseFilterViewModel _selectedFilter;

        public BaseFilterViewModel SelectedFilter
        {
            get => _selectedFilter;
            set => this.RaiseAndSetIfChanged(ref _selectedFilter, value);
        }

        public ReactiveCommand<Unit, List<BaseFilterViewModel>> GetFilters { get; }
        public IObservable<IRoutableViewModel> GoToOther { get; }
    }
}
