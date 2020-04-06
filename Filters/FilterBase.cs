using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using ReactiveUI;
using RxUI_Sample.ViewModels;

namespace RxUI_Sample.Filters
{
	public abstract class  IFilter : ReactiveObject
	{
		private Func<ChildViewModel, bool> _predicate;
		protected Subject<Unit> _subject = new Subject<Unit>();

		protected IFilter()
		{
			PredicateChanged = _subject;
		}

		public IObservable<Unit> PredicateChanged { get; }

		public Func<ChildViewModel, bool> Predicate
		{
			get => _predicate;
			protected set => this.RaiseAndSetIfChanged(ref _predicate, value);
		}
	}

	public class TextFilter : IFilter
	{
		private string _text = "";


		public TextFilter()
		{
			this.WhenAnyValue(x => x.Text)
				.Select(_ => new Func<ChildViewModel, bool>(model =>
				{
					if (!string.IsNullOrEmpty(Text))
					{
						return model?.Text?.ToLowerInvariant().Contains(Text.ToLowerInvariant()) ?? false;
					}

					return true;
				}))
				.Subscribe(predicate =>
				{
					Predicate = predicate;
					_subject.OnNext(Unit.Default);
				});
		}

		public string Text
		{
			get => _text;
			set => this.RaiseAndSetIfChanged(ref _text, value ?? "");
		}
	}

	public class IsSelectedFilter : IFilter
	{
		public IsSelectedFilter()
		{
			Predicate = vm => vm?.IsSelected ?? false;

			_subject.OnNext(Unit.Default);
		}
	}

}
