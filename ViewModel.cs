using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Math
{
    public class ViewModel : INotifyPropertyChanged
    {
        List<Formula> formulas = new List<Formula>();

        public ViewModel()
        {
            StartCommand = new StartCommand(this);
        }

        public bool taskCompleted;

        Formula formula;
        public Formula Formula
        {
            get => formula;
            set
            {
                formula = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Formula"));
            }
        }

        int? result;
        public int? Result
        {
            get => result;
            set
            {
                result = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Result"));
            }
        }

        Brush foreground;
        public Brush Foreground
        {
            get => foreground;
            set
            {
                foreground = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Foreground"));
            }
        }

        int total;
        public int Total
        {
            get => total;
            set
            {
                total = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Total"));
            }
        }

        public void OnResultChanged(int result)
        {
            if (formula.Result == result)
            {
                Foreground = new SolidColorBrush(Colors.DarkGreen);
                formula.Passed = true;
            }
            else
            {
                Foreground = new SolidColorBrush(Colors.Red);
                formula.Passed = false;
            }
        }

        public void Next()
        {
            if (formula.Passed)
            {
                if (Current < Total)
                {
                    Current++;
                    Formula = formulas[Current - 1];
                    Result = null;
                }
                else
                {
                    taskCompleted = true;
                    var consume = DateTime.Now - startTime;
                    MessageBox.Show($"练习完成，耗时 {consume}");
                }
            }
        }

        int current;
        public int Current
        {
            get => current;
            set
            {
                current = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Current"));
            }
        }

        public bool Less20 { get; set; }

        public int Generate { get; set; } = 100;

        public event PropertyChangedEventHandler PropertyChanged;

        DateTime startTime;

        public void Start()
        {
            startTime = DateTime.Now;
            taskCompleted = false;
            Total = Generate;
            formulas.Clear();
            var random = new Random();
            for (int i = 0; i < Total; i++)
            {
                Formula f = new Formula();
                if (Less20)
                {
                    f.X1 = random.Next(11, 20);
                    f.X2 = random.Next(11, 20);
                    f.Operator = Operator.Times;
                }
                else
                {
                    f.X1 = random.Next(2, 100);
                    f.X2 = random.Next(2, 100);
                    f.Operator = Operator.Times;
                }
                formulas.Add(f);
            }
            Formula = formulas.First();
            Current = 1;
        }

        public ICommand StartCommand { get; set; }
    }

    public class Formula
    {
        public int X1 { get; set; }

        public int X2 { get; set; }

        public bool Passed { get; set; }

        public Operator Operator { get; set; }

        public int Result
        {
            get
            {
                switch(Operator)
                {
                    case Operator.Add:
                        return X1 + X2;
                    case Operator.Times:
                        return X1 * X2;
                }
                throw new Exception("操作符不支持");
            }
        }

        public override string ToString()
        {
            return $"{X1} {GetOperatorDisplay()} {X2} = ";
        }

        string GetOperatorDisplay()
        {
            switch(Operator)
            {
                case Operator.Add:
                    return "+";
                case Operator.Times:
                    return "x";
            }
            return "_";
        }
    }

    public enum Operator
    {
        Add,
        Times
    }

    public class StartCommand : ICommand
    {
        ViewModel vm;
        public StartCommand(ViewModel vm)
        {
            this.vm = vm;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            vm.Start();
        }
    }
}
