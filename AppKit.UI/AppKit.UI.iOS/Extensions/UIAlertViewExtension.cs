namespace AdMaiora.AppKit.UI
{
    using System;
    using System.Collections.Generic;
    
    using UIKit;

    public class UIAlertViewBuilder
    {
        private UIAlertView _alert;

        private Dictionary<nint, EventHandler<UIButtonEventArgs>> _handlers;

        public UIAlertViewBuilder(UIAlertView alert)
        {
            _alert = alert;
            _alert.Clicked += (sender, e) =>  _handlers[e.ButtonIndex](sender, e);

            _handlers = new Dictionary<nint, EventHandler<UIButtonEventArgs>>();
        }

        public UIAlertViewBuilder SetTitle(string title)
        {
            _alert.Title = title;
            return this;
        }

        public UIAlertViewBuilder SetMessage(string message)
        {
            _alert.Message = message;
            return this;
        }

        public UIAlertViewBuilder AddButton(string title, EventHandler<UIButtonEventArgs> click)
        {
            nint buttonId = _alert.AddButton(title);
            _handlers.Add(buttonId, click);
            return this;
        }

        public UIAlertView Show()
        {
            _alert.Show();
            return _alert;
        }
    }

    public static class UIAlertViewExtension
    {
        public static UIAlertViewBuilder Builder(this UIAlertView alert)
        {
            return new UIAlertViewBuilder(alert);
        }
    }
}

