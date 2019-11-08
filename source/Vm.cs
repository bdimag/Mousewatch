using System;
using System.ComponentModel;

namespace Mousewatch
{
    public class Vm : INotifyPropertyChanged, IDisposable
    {
        private readonly Watcher watcher;
        private bool mouse1Down;
        private bool mouse2Down;
        private bool mouse3Down;

        public bool Mouse1Down
        {
            get => mouse1Down;
            set
            {
                mouse1Down = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Mouse1Down)));
            }
        }

        public bool Mouse2Down
        {
            get => mouse2Down;
            set
            {
                mouse2Down = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Mouse2Down)));
            }
        }

        public bool Mouse3Down
        {
            get => mouse3Down;
            set
            {
                mouse3Down = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Mouse3Down)));
            }
        }

        public Vm()
        {
            if (!DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                watcher = new Watcher();
                watcher.MouseStateChanged += Watcher_MouseStateChanged;
            }
        }

        ~Vm()
        {
            Dispose(false);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Watcher_MouseStateChanged(object sender, Watcher.MouseStateChangedEventArgs e)
        {
            switch (e.Button)
            {
                case 1:
                    Mouse1Down = e.State;
                    break;

                case 2:
                    Mouse2Down = e.State;
                    break;

                case 3:
                    Mouse3Down = e.State;
                    break;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                watcher.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}