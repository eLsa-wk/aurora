﻿using System;
using System.Threading.Tasks;
using System.Windows;
using Aurora.Core.Activities;
using Aurora.Core.Workspace;
using Microsoft.Practices.Prism.Commands;
using Newtonsoft.Json.Linq;

namespace Aurora.Sample.Module.Views.TestWorkspace
{
    public class TestWorkspacePresenter : WorkspaceViewPresenter<TestWorkspaceViewModel>
    {
        private readonly IWorkspace workspace;
       
        public TestWorkspacePresenter(ViewActivityInfo info, IWorkspace workspace) : base(info)
        {
            this.workspace = workspace;
        }

        protected override void OnViewModelChanged()
        {
            this.ViewModel.CreateViewCommand = new DelegateCommand(async () => { await this.CreateView(); });
            this.ViewModel.ToggleOrientationCommand = new DelegateCommand(this.ToogleOrientation);
            this.ViewModel.SelectedWindowType = WindowType.Floating;
            this.ViewModel.SelectedViewType = ViewType.Custom;
            this.ViewModel.Title = "Testing";
            this.ViewModel.Width = 800;
            this.ViewModel.Height = 600;

            var input = ViewActivityInfo.ViewData?.ToString();
            this.ViewModel.JsonInput = input ?? "{}";
        }

        private void ToogleOrientation()
        {
           
        }
        private async Task CreateView()
        {
            if (this.ViewModel.SelectedWindowType == WindowType.Floating)
            {

                var data = JObject.Parse(this.ViewModel.JsonInput);
                this.ViewActivityInfo.ViewData = data;
                await this.workspace.CreateFloatingView(GetPresenterType(), this.ViewModel.Title, data, 
                    new Rect(this.ViewModel.Left, this.ViewModel.Top, this.ViewModel.Width, this.ViewModel.Height), false);

            }
            else
            {
                var data = JObject.Parse(this.ViewModel.JsonInput);
                this.ViewActivityInfo.ViewData = data;
                await this.workspace.CreateDockedView(GetPresenterType(), this.ViewModel.Title, data, 
                    this.ViewModel.GroupIdx, this.ViewModel.Order, true);
            
            }
        }

        private Type GetPresenterType()
        {
            if (this.ViewModel.SelectedViewType == ViewType.Custom)
                return typeof(CustomPresenter);
            else if (this.ViewModel.SelectedViewType == ViewType.TestWorkspace)
                return typeof(TestWorkspacePresenter);
            else
                return null;
        }


    }
}