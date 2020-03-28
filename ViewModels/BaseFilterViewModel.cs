using ReactiveUI;

namespace RxUI_Sample.ViewModels
{
    public abstract class BaseFilterViewModel : ReactiveObject
    {
        public abstract string Name { get; }
    }

    public class BoolFilterViewModel : BaseFilterViewModel
    {
        private bool _someFilter;

        public override string Name => "Bool";
        public bool SomeFilter
        {
            get => _someFilter;
            set => this.RaiseAndSetIfChanged(ref _someFilter, value);
        }
    }

    public class TextFilterViewModel : BaseFilterViewModel
    {
        private string _filterText = "";

        public override string Name => "Text";

        public string FilterText
        {
            get => _filterText;
            set => this.RaiseAndSetIfChanged(ref _filterText, value);
        }
    }
}