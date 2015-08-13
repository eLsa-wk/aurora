﻿using System.Windows;
using Aurora.Core.Activities;

namespace Aurora.Core
{
    public class ActiveView<TPresenter> : ActiveView where TPresenter : IPresenter
    {
        public ActiveView(TPresenter presenter, IViewModel viewModel, FrameworkElement view, IActivity activity = null) 
            : base(presenter, viewModel, view, activity)
        {
        }

        public new TPresenter Presenter => (TPresenter)base.Presenter;
    }

    public class ActiveView
    {
        public ActiveView(IPresenter presenter, IViewModel viewModel, FrameworkElement view, IActivity activity = null)
        {
            Presenter = presenter;
            ViewModel = viewModel;
            View = view;
            Activity = activity;
        }

        public IPresenter Presenter { get; }
        public IViewModel ViewModel { get; }
        public FrameworkElement View { get; }
        public IActivity Activity { get; }
    }
}