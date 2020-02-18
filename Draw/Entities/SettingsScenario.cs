using Draw.ViewModel;
using Newtonsoft.Json;

namespace Draw.Entities
{
    public class SettingsScenario : ViewModelBase
    {
        /* Установления количества цифр */
        [JsonIgnore]
        public int[] countDigits = new int[10];

        public int CountDigit1 { get => countDigits[0]; set { countDigits[0] = value; OnPropertyChanged(); } }
        public int CountDigit2 { get => countDigits[1]; set { countDigits[1] = value; OnPropertyChanged(); } }
        public int CountDigit3 { get => countDigits[2]; set { countDigits[2] = value; OnPropertyChanged(); } }
        public int CountDigit4 { get => countDigits[3]; set { countDigits[3] = value; OnPropertyChanged(); } }
        public int CountDigit5 { get => countDigits[4]; set { countDigits[4] = value; OnPropertyChanged(); } }
        public int CountDigit6 { get => countDigits[5]; set { countDigits[5] = value; OnPropertyChanged(); } }
        public int CountDigit7 { get => countDigits[6]; set { countDigits[6] = value; OnPropertyChanged(); } }
        public int CountDigit8 { get => countDigits[7]; set { countDigits[7] = value; OnPropertyChanged(); } }
        public int CountDigit9 { get => countDigits[8]; set { countDigits[8] = value; OnPropertyChanged(); } }
        public int CountDigit0 { get => countDigits[9]; set { countDigits[9] = value; OnPropertyChanged(); } }

        /* Границы рандомных чисел */

        private (int min, int max) _rotate;
        public int RotateMin { get => _rotate.min; set { _rotate.min = value; OnPropertyChanged(); } }
        public int RotateMax { get => _rotate.max; set { _rotate.max = value; OnPropertyChanged(); } }

        private (int min, int max) _shiftX;
        public int ShiftMinX { get => _shiftX.min; set { _shiftX.min = value; OnPropertyChanged(); } }
        public int ShiftMaxX { get => _shiftX.max; set { _shiftX.max = value; OnPropertyChanged(); } }

        private (int min, int max) _shiftY;
        public int ShiftMinY { get => _shiftY.min; set { _shiftY.min = value; OnPropertyChanged(); } }
        public int ShiftMaxY { get => _shiftY.max; set { _shiftY.max = value; OnPropertyChanged(); } }

        private (double min, double max) _scaleX = (1, 1);
        public double ScaleMinX { get => _scaleX.min; set { _scaleX.min = value; OnPropertyChanged(); } }
        public double ScaleMaxX { get => _scaleX.max; set { _scaleX.max = value; OnPropertyChanged(); } }

        private (double min, double max) _scaleY = (1, 1);
        public double ScaleMinY { get => _scaleY.min; set { _scaleY.min = value; OnPropertyChanged(); } }
        public double ScaleMaxY { get => _scaleY.max; set { _scaleY.max = value; OnPropertyChanged(); } }

        private (int min, int max) _skewX;
        public int SkewMinX { get => _skewX.min; set { _skewX.min = value; OnPropertyChanged(); } }
        public int SkewMaxX { get => _skewX.max; set { _skewX.max = value; OnPropertyChanged(); } }

        private (int min, int max) _skewY;
        public int SkewMinY { get => _skewY.min; set { _skewY.min = value; OnPropertyChanged(); } }
        public int SkewMaxY { get => _skewY.max; set { _skewY.max = value; OnPropertyChanged(); } }
    }
}