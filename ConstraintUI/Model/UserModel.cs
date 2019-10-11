using System.ComponentModel;
using System.Runtime.CompilerServices;
using ConstraintUI.Annotations;

namespace ConstraintUI.Model
{
    public class UserModel : INotifyPropertyChanged
    {
        private double _cognitiveLoad;
        private double _cognitiveCapacity;

        public double CognitiveCapacity
        {
            get => _cognitiveCapacity;
            set
            {
                _cognitiveCapacity = value;
                OnPropertyChanged();
            }
        }

        public double CognitiveLoad
        {
            get => _cognitiveLoad;
            set
            {
                _cognitiveLoad = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}