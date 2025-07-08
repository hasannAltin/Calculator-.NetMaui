using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace YeniHesapMakinesi
{
    internal class CalculatorViewModel : INotifyPropertyChanged
    {
        private string _displayValue = "0";
        private string _currentInput = string.Empty;
        private double _currentValue = 0;
        private double _memoryValue = 0;
        private string _pendingOperation;
        private bool _isScientificMode;
        private bool _isDegreeMode = true;
        private bool _isSecondFunction = false;

        public string DisplayValue
        {
            get => _displayValue;
            set
            {
                _displayValue = value;
                OnPropertyChanged();
            }
        }

        public ICommand NumberCommand { get; }
        public ICommand DecimalCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand ClearEntryCommand { get; }
        public ICommand BackspaceCommand { get; }
        public ICommand ToggleSignCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand SubtractCommand { get; }
        public ICommand MultiplyCommand { get; }
        public ICommand DivideCommand { get; }
        public ICommand EqualsCommand { get; }
        public ICommand PercentageCommand { get; }
        public ICommand SquareRootCommand { get; }
        public ICommand SquareCommand { get; }
        public ICommand ReciprocalCommand { get; }

        public ICommand MemoryClearCommand { get; }
        public ICommand MemoryRecallCommand { get; }
        public ICommand MemoryAddCommand { get; }
        public ICommand MemorySubtractCommand { get; }
        public ICommand MemoryStoreCommand { get; }
        public ICommand MemoryViewCommand { get; }

        public ICommand SinCommand { get; }
        public ICommand CosCommand { get; }
        public ICommand TanCommand { get; }
        public ICommand PiCommand { get; }
        public ICommand ECommand { get; }
        public ICommand AbsoluteCommand { get; }
        public ICommand ExpCommand { get; }
        public ICommand ModCommand { get; }
        public ICommand CubeCommand { get; }
        public ICommand PowerOfTenCommand { get; }
        public ICommand LogCommand { get; }
        public ICommand NaturalLogCommand { get; }
        public ICommand FactorialCommand { get; }
        public ICommand OpenParenthesisCommand { get; }
        public ICommand CloseParenthesisCommand { get; }
        public ICommand PowerCommand { get; }
        public ICommand ScientificNotationCommand { get; }
        public ICommand RandomCommand { get; }
        public ICommand HyperbolicCommand { get; }
        public ICommand ToggleAngleModeCommand { get; }
        public ICommand ToggleNotationCommand { get; }
        public ICommand SecondFunctionCommand { get; }

        public CalculatorViewModel(bool isScientificMode = false)
        {
            _isScientificMode = isScientificMode;

            NumberCommand = new Command<string>(AppendNumber);
            DecimalCommand = new Command(AddDecimal);
            ClearCommand = new Command(ClearAll);
            ClearEntryCommand = new Command(ClearEntry);
            BackspaceCommand = new Command(Backspace);
            ToggleSignCommand = new Command(ToggleSign);
            AddCommand = new Command(() => SetOperation("+"));
            SubtractCommand = new Command(() => SetOperation("-"));
            MultiplyCommand = new Command(() => SetOperation("×"));
            DivideCommand = new Command(() => SetOperation("÷"));
            EqualsCommand = new Command(Calculate);
            PercentageCommand = new Command(CalculatePercentage);
            SquareRootCommand = new Command(CalculateSquareRoot);
            SquareCommand = new Command(CalculateSquare);
            ReciprocalCommand = new Command(CalculateReciprocal);

            MemoryClearCommand = new Command(MemoryClear);
            MemoryRecallCommand = new Command(MemoryRecall);
            MemoryAddCommand = new Command(MemoryAdd);
            MemorySubtractCommand = new Command(MemorySubtract);
            MemoryStoreCommand = new Command(MemoryStore);
            MemoryViewCommand = new Command(MemoryView);

            SinCommand = new Command(() => CalculateTrigFunction(Math.Sin));
            CosCommand = new Command(() => CalculateTrigFunction(Math.Cos));
            TanCommand = new Command(() => CalculateTrigFunction(Math.Tan));
            PiCommand = new Command(() => DisplayValue = Math.PI.ToString());
            ECommand = new Command(() => DisplayValue = Math.E.ToString());
            AbsoluteCommand = new Command(() => DisplayValue = Math.Abs(GetCurrentValue()).ToString());
            ExpCommand = new Command(CalculateExponential);
            ModCommand = new Command(() => SetOperation("mod"));
            CubeCommand = new Command(CalculateCube);
            PowerOfTenCommand = new Command(CalculatePowerOfTen);
            LogCommand = new Command(CalculateLog);
            NaturalLogCommand = new Command(CalculateNaturalLog);
            FactorialCommand = new Command(CalculateFactorial);
            OpenParenthesisCommand = new Command(() => AppendToInput("("));
            CloseParenthesisCommand = new Command(() => AppendToInput(")"));
            PowerCommand = new Command(() => SetOperation("^"));
            ScientificNotationCommand = new Command(ToggleScientificNotation);
            RandomCommand = new Command(GenerateRandomNumber);
            HyperbolicCommand = new Command(ToggleHyperbolic);
            ToggleAngleModeCommand = new Command(ToggleAngleMode);
            ToggleNotationCommand = new Command(ToggleNotation);
            SecondFunctionCommand = new Command(ToggleSecondFunction);
        }

        private void AppendNumber(string number)
        {
            if (_currentInput == "0" && number != "0")
                _currentInput = number;
            else
                _currentInput += number;

            DisplayValue = _currentInput;
        }

        private void AddDecimal()
        {
            if (!_currentInput.Contains("."))
            {
                _currentInput += _currentInput == string.Empty ? "0." : ".";
                DisplayValue = _currentInput;
            }
        }

        private void ClearAll()
        {
            _currentInput = string.Empty;
            _currentValue = 0;
            _pendingOperation = null;
            DisplayValue = "0";
        }

        private void ClearEntry()
        {
            _currentInput = string.Empty;
            DisplayValue = "0";
        }

        private void Backspace()
        {
            if (_currentInput.Length > 0)
            {
                _currentInput = _currentInput.Substring(0, _currentInput.Length - 1);
                DisplayValue = _currentInput.Length > 0 ? _currentInput : "0";
            }
        }

        private void ToggleSign()
        {
            if (_currentInput.StartsWith("-"))
                _currentInput = _currentInput.Substring(1);
            else if (!string.IsNullOrEmpty(_currentInput) && _currentInput != "0")
                _currentInput = "-" + _currentInput;

            DisplayValue = _currentInput;
        }

        private void SetOperation(string operation)
        {
            if (!string.IsNullOrEmpty(_currentInput))
            {
                _currentValue = GetCurrentValue();
                _currentInput = string.Empty;
            }
            _pendingOperation = operation;
        }

        private void Calculate()
        {
            if (_pendingOperation == null || string.IsNullOrEmpty(_currentInput))
                return;

            double secondValue = GetCurrentValue();
            double result = 0;

            switch (_pendingOperation)
            {
                case "+":
                    result = _currentValue + secondValue;
                    break;
                case "-":
                    result = _currentValue - secondValue;
                    break;
                case "×":
                    result = _currentValue * secondValue;
                    break;
                case "÷":
                    if (secondValue == 0)
                    {
                        DisplayValue = "Sıfıra bölme hatası";
                        return;
                    }
                    result = _currentValue / secondValue;
                    break;
                case "mod":
                    result = _currentValue % secondValue;
                    break;
                case "^":
                    result = Math.Pow(_currentValue, secondValue);
                    break;
            }

            DisplayValue = result.ToString();
            _currentValue = result;
            _currentInput = string.Empty;
            _pendingOperation = null;
        }

        private double GetCurrentValue()
        {
            if (string.IsNullOrEmpty(_currentInput))
                return 0;

            return double.Parse(_currentInput);
        }

        private void CalculatePercentage()
        {
            if (!string.IsNullOrEmpty(_currentInput))
            {
                double value = GetCurrentValue();
                DisplayValue = (value / 100).ToString();
                _currentInput = DisplayValue;
            }
        }

        private void CalculateSquareRoot()
        {
            if (!string.IsNullOrEmpty(_currentInput))
            {
                double value = GetCurrentValue();
                if (value < 0)
                {
                    DisplayValue = "Geçersiz giriş";
                    return;
                }
                DisplayValue = Math.Sqrt(value).ToString();
                _currentInput = DisplayValue;
            }
        }

        private void CalculateSquare()
        {
            if (!string.IsNullOrEmpty(_currentInput))
            {
                double value = GetCurrentValue();
                DisplayValue = (value * value).ToString();
                _currentInput = DisplayValue;
            }
        }

        private void CalculateReciprocal()
        {
            if (!string.IsNullOrEmpty(_currentInput))
            {
                double value = GetCurrentValue();
                if (value == 0)
                {
                    DisplayValue = "Sıfıra bölme hatası";
                    return;
                }
                DisplayValue = (1 / value).ToString();
                _currentInput = DisplayValue;
            }
        }


        private void MemoryClear()
        {
            _memoryValue = 0;
        }

        private void MemoryRecall()
        {
            DisplayValue = _memoryValue.ToString();
            _currentInput = DisplayValue;
        }

        private void MemoryAdd()
        {
            _memoryValue += GetCurrentValue();
            _currentInput = string.Empty;
        }

        private void MemorySubtract()
        {
            _memoryValue -= GetCurrentValue();
            _currentInput = string.Empty;
        }

        private void MemoryStore()
        {
            _memoryValue = GetCurrentValue();
            _currentInput = string.Empty;
        }

        private void MemoryView()
        {
            DisplayValue = _memoryValue.ToString();
        }


        private void CalculateTrigFunction(Func<double, double> trigFunction)
        {
            if (!string.IsNullOrEmpty(_currentInput))
            {
                double value = GetCurrentValue();
                if (!_isDegreeMode)
                {
                    value = value * Math.PI / 180;
                }
                DisplayValue = trigFunction(value).ToString();
                _currentInput = DisplayValue;
            }
        }

        private void CalculateExponential()
        {
            if (!string.IsNullOrEmpty(_currentInput))
            {
                double value = GetCurrentValue();
                DisplayValue = Math.Exp(value).ToString();
                _currentInput = DisplayValue;
            }
        }

        private void CalculateCube()
        {
            if (!string.IsNullOrEmpty(_currentInput))
            {
                double value = GetCurrentValue();
                DisplayValue = Math.Pow(value, 3).ToString();
                _currentInput = DisplayValue;
            }
        }

        private void CalculatePowerOfTen()
        {
            if (!string.IsNullOrEmpty(_currentInput))
            {
                double value = GetCurrentValue();
                DisplayValue = Math.Pow(10, value).ToString();
                _currentInput = DisplayValue;
            }
        }

        private void CalculateLog()
        {
            if (!string.IsNullOrEmpty(_currentInput))
            {
                double value = GetCurrentValue();
                if (value <= 0)
                {
                    DisplayValue = "Geçersiz giriş";
                    return;
                }
                DisplayValue = Math.Log10(value).ToString();
                _currentInput = DisplayValue;
            }
        }

        private void CalculateNaturalLog()
        {
            if (!string.IsNullOrEmpty(_currentInput))
            {
                double value = GetCurrentValue();
                if (value <= 0)
                {
                    DisplayValue = "Geçersiz giriş";
                    return;
                }
                DisplayValue = Math.Log(value).ToString();
                _currentInput = DisplayValue;
            }
        }

        private void CalculateFactorial()
        {
            if (!string.IsNullOrEmpty(_currentInput))
            {
                int value = (int)GetCurrentValue();
                if (value < 0)
                {
                    DisplayValue = "Geçersiz giriş";
                    return;
                }

                long result = 1;
                for (int i = 2; i <= value; i++)
                {
                    result *= i;
                }

                DisplayValue = result.ToString();
                _currentInput = DisplayValue;
            }
        }

        private void ToggleScientificNotation()
        {
            if (!string.IsNullOrEmpty(_currentInput))
            {
                double value = GetCurrentValue();
                DisplayValue = value.ToString("E6");
                _currentInput = DisplayValue;
            }
        }

        private void GenerateRandomNumber()
        {
            Random random = new Random();
            double value = random.NextDouble();
            DisplayValue = value.ToString();
            _currentInput = DisplayValue;
        }

        private void ToggleHyperbolic()
        {

        }

        private void ToggleAngleMode()
        {
            _isDegreeMode = !_isDegreeMode;
        }

        private void ToggleNotation()
        {

        }

        private void ToggleSecondFunction()
        {
            _isSecondFunction = !_isSecondFunction;

        }

        private void AppendToInput(string text)
        {
            _currentInput += text;
            DisplayValue = _currentInput;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}