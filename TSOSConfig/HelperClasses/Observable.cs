using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Allows you to obtain the method or property name of the caller.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class CallerMemberNameAttribute : Attribute { }
}

namespace TSOSConfig
{
    public abstract class Observable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected void OnInvokePropertyChanged(string name)
        {
            try
            {
                Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
                }));
            }
            catch
            {
                // ignored
            }
        }

        private readonly Dictionary<string, object> _fields = new Dictionary<string, object>();

        protected void Set<T>(T value, [CallerMemberName] string name = null)
        {
            if (name == null)
            {
                return;
            }

            if (_fields.ContainsKey(name))
            {
                var old = this._fields[name];
                if (Equals(old, value))
                {
                    return;
                }

                _fields[name] = value;
            }
            else
            {
                if (Equals(value, default(T)))
                {
                    return;
                }
                _fields.Add(name, value);
            }
            this.OnPropertyChanged(name);
        }

        protected T Get<T>([CallerMemberName] string name = null)
        {
            if (name == null || !_fields.ContainsKey(name))
            {
                return default(T);
            }

            return (T)_fields[name];
        }
    }
}
