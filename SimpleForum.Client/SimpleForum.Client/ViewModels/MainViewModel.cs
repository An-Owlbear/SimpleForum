using System.Collections.Generic;
using SimpleForum.Client.Services;

namespace SimpleForum.Client.ViewModels
{
    public class MainViewModel
    {
        public AccountService AccountService { get; set; } = new AccountService();
    }
}