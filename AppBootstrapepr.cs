using ReactiveUI;
using RxUI_Sample.ViewModels;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Splat;

namespace RxUI_Sample
{
    public class AppBootstrapper : ReactiveObject, IScreen
    {
        private readonly RoutingState _router;

        public AppBootstrapper()
        {
            _router = new RoutingState();

            _router.Navigate.Execute(new ParentViewModel(this)).Subscribe();

            RegisterViews(Locator.CurrentMutable);
        }

        private void RegisterViews(IMutableDependencyResolver resolver)
        {
            resolver.RegisterViewsForViewModels(Assembly.GetExecutingAssembly());
        }

        public RoutingState Router => _router;
    }
}
