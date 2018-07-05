using System;
using System.Collections.Generic;
using System.Text;

using Xamarin;
using Xamarin.Forms;
using Xamarin.Forms.Platform;
using Xamarin.Forms.Xaml.Internals;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Internals;

namespace iOSFix
{

    public class Behav : Xamarin.Forms.Behavior<Entry>
    {
        public int MaxLenght { get; set; }
        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.TextChanged += OnTextChanged;
        }
        protected override void OnDetachingFrom(Entry bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.TextChanged += OnTextChanged;
        }

        void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = (Entry)sender;
                string EntryTxt = entry.Text.ToUpper();
            if (entry.Text.Length > this.MaxLenght)
            {
                EntryTxt = EntryTxt.Remove(EntryTxt.Length - 1);
            }
                entry.Text = EntryTxt;
        }
    }
    public class Trans
    {
        public string Tipo { get; set; }
        public int Valor { get; set; }
    }
}
