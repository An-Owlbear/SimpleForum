﻿using SimpleForum.Common;
using Xamarin.Forms;

namespace SimpleForum.Client
{
    public static class Tools
    {
        public static bool HandleResult<T>(this T sender, Result result) where T : class
        {
            if (result.Success) return true;
            MessagingCenter.Send(sender, "Error", result.Error);
            return false;
        }
    }
}