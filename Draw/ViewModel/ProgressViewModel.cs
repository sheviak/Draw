namespace Draw.ViewModel
{
    public class ProgressViewModel : ViewModelBase
    {
        private bool _load = false;
        public bool Load { get => _load; set { _load = value; OnPropertyChanged(); } }

        private int _finishTotalNumber;
        public int FinishTotalNumbers { get => _finishTotalNumber; set { _finishTotalNumber = value; OnPropertyChanged(); } }

        private int _totalNumber;
        public int TotalNumber
        {
            get => _totalNumber;
            set
            {
                if (_finishTotalNumber == value)
                {
                    Load = false;
                }
                else
                {
                    _totalNumber = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}