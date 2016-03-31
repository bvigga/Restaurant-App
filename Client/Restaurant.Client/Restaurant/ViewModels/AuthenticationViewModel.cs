﻿using Fusillade;
using ReactiveUI;
using Refit;
using Restaurant.Model;
using Restaurant.Models;
using Restaurant.ReactiveUI;
using Splat;
using System;
using System.Diagnostics;
using System.Net.Http;

namespace Restaurant.ViewModels
{
    public class AuthenticationViewModel : ReactiveObject, INavigatableViewModel
    {
        public ReactiveCommand<UserInfo> Login { get; set; }

        public ReactiveCommand<object> OpenRegester { get; set; }

        public ReactiveCommand<object> OpenLogin { get; set; }


        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { this.RaiseAndSetIfChanged(ref isBusy, value); }
        }

        private string email;

        public string Email
        {
            get { return email; }
            set { this.RaiseAndSetIfChanged(ref email, value); }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set { this.RaiseAndSetIfChanged(ref password, value); }
        }

        private string authenticationStatus;

        public string AuthenticationStatus
        {
            get { return authenticationStatus; }
            set { this.RaiseAndSetIfChanged(ref authenticationStatus, value); }
        }

        public INavigatableScreen NavigationScreen { get; protected set; }

        public string Title => "Authentication";

        public AuthenticationViewModel(INavigatableScreen screen = null)
        {
            NavigationScreen = (screen ?? Locator.Current.GetService<INavigatableScreen>());

            var canLogin = this.WhenAny(x => x.Email, x => x.Password,
                (e, p) => !string.IsNullOrEmpty(e.Value) && !string.IsNullOrEmpty(p.Value));


            Login = ReactiveCommand.CreateAsyncTask(canLogin, async _ =>
             {
                 IsBusy = true;
                 AuthenticationStatus = "started logging...";
                 Debug.WriteLine(Helper.Address);
                 var client = new HttpClient(NetCache.UserInitiated)
                 {
                     BaseAddress = new Uri(Helper.Address)
                 };
                 var api = RestService.For<IRestaurantApi>(client);
                 var token = await api.GetToken(Email, Password);
                 AuthenticationStatus = "started authentication...";
                 Global.AuthenticatedClient = new HttpClient(new AuthenticatedHttpClientHandler(token.access_token))
                 {
                     BaseAddress = new Uri(Helper.Address)
                 };
                 var info = await Global.AuthenticatedApi.GetUserInfo();
                 return info;
             });

            Login.ThrownExceptions.Subscribe(ex =>
            {
                UserError.Throw("Invalid login or password!");
                Debug.WriteLine("Error! - " + ex.Message);
            });


            #region OpenRegester
            OpenRegester = ReactiveCommand.Create();
            OpenRegester.Subscribe(x =>
            {
                var viewModel = Locator.Current.GetService<SignUpViewModel>();
                if (viewModel == null)
                {
                    var regViewModel = new SignUpViewModel(NavigationScreen);
                    Locator.CurrentMutable.RegisterConstant(regViewModel, typeof(SignUpViewModel));
                    NavigationScreen.Navigation.Navigate.Execute(regViewModel);
                }
                else
                {
                    NavigationScreen.Navigation.Navigate.Execute(viewModel);
                }
            });


            OpenLogin = ReactiveCommand.Create();
            OpenLogin.Subscribe(x =>
            {
                var authenViewModel = Locator.Current.GetService<AuthenticationViewModel>();
                NavigationScreen.Navigation.Navigate.Execute(authenViewModel);
            });
            #endregion

        }

        public void NavigateToMainPage(UserInfo user)
        {
            user.Picture = Helper.Address + "/" + user.Picture;
            var mainViewModel = new MainViewModel(user);
            NavigationScreen.Navigation.NavigateToMainPage.Execute(mainViewModel);
        }
    }
}
