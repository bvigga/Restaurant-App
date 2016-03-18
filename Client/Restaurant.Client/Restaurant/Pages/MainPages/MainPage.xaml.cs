﻿using ReactiveUI;
using Restaurant.ReactiveUI;
using Restaurant.ViewModels;
using Splat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Restaurant.Pages.MainPages
{
    public partial class MainPage : MainPageXaml
    {
        public MainPage()
        {
            InitializeComponent();
            Master = new MasterPage(ViewModel);
            Detail = new FoodsPage().ToThemedNavigationPage();
            AddDoneButton();
        }
    }

    public class MainPageXaml : BaseMasterDetailPage<MainViewModel>
    {

    }

    public class BaseMasterDetailPage<T> : MainBaseMasterDetailPage, IViewFor<T> where T : class, INavigatableViewModel
    {

        public T ViewModel { get; set; }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }

            set { ViewModel = (T)value; }
        }
        public BaseMasterDetailPage()
        {
            ViewModel = Locator.Current.GetService<T>();
            BindingContext = ViewModel;
        }
    }

    public class MainBaseMasterDetailPage : MasterDetailPage, IColoredPage
    {
        public Color ActionBarTextColor { get; set; }

        public Color ActionBarBackgroundColor { get; set; }

        public Color StatusBarColor { get; set; }

        public Color NavigationBarColor { get; set; }


        public void AddDoneButton(string text = "Done")
        {
            var btnPlus = new ToolbarItem
            {
                Icon = "ic_plus.png"
            };
            var btnMore = new ToolbarItem
            {
                Icon = "ic_more_vert_white"
            };

            this.ToolbarItems.Add(btnPlus);
            this.ToolbarItems.Add(btnMore);
        }
    }
}