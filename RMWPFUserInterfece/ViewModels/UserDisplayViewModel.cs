using Caliburn.Micro;
using RMDesktopUI.Library.Api;
using RMDesktopUI.Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RMWPFUserInterfece.ViewModels
{
    public class UserDisplayViewModel : Screen
    {
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _manager;
        private readonly IUserEndPoint _userEndPoint;
        BindingList<UserModel> _users; 
        public BindingList<UserModel> Users
        {
            get
            {
                return _users;
            }
            set
            {
                _users = value;
                NotifyOfPropertyChange(() => Users);
            }
        }
        public UserDisplayViewModel(StatusInfoViewModel status, IWindowManager manager, IUserEndPoint userEndPoint)
        {
            _status = status;
            _manager = manager;
            _userEndPoint = userEndPoint;
        }
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadUsers();

            }
            catch (Exception ex)
            {
                dynamic settings = new ExpandoObject();
                settings.WindowsStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "System Eror";
                if (ex.Message == "Unauthorized")
                {
                    _status.UpdateMessage("Unauthorized Access", "You do not have permission to interact with the Sales Form.");
                    _manager.ShowDialog(_status, null, null);
                }
                else
                {
                    _status.UpdateMessage("Fatal Exeption", ex.Message);
                    _manager.ShowDialog(_status, null, null);
                }
                TryClose();

            }
        }

        private async Task LoadUsers()
        {
            var userList = await _userEndPoint.GetAll();
            Users = new BindingList<UserModel>(userList);
        }
    }
}
