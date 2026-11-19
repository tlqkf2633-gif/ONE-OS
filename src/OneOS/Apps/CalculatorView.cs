using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace OneOS.Apps;

public static class CalculatorView
{
    public static FrameworkElement Create()
    {
        var page = ViewFactoryStyles.Page("Calculator", "Basic decimal arithmetic.");
        var display = new TextBox
        {
            Text = "0", FontSize = 24, Padding = new Thickness(10), Margin = new Thickness(0, 0, 0, 10),
            HorizontalContentAlignment = HorizontalAlignment.Right,
            Background = ViewFactoryStyles.CardBrush, Foreground = ViewFactoryStyles.ForegroundBrush
        };
        display.SetResourceReference(Control.BackgroundProperty, "InputBrush");
        display.SetResourceReference(Control.ForegroundProperty, "TextBrush");
        var status = ViewFactoryStyles.Status();
        decimal? accumulator = null;
        string? operation = null;
        bool replaceDisplay = true;

        void Show(decimal value) => display.Text = value.ToString(CultureInfo.CurrentCulture);
        decimal ReadValue() => decimal.Parse(display.Text, NumberStyles.Number, CultureInfo.CurrentCulture);
        void InputDigit(string digit)
        {
            if (replaceDisplay || display.Text == "0") display.Text = digit;
            else display.Text += digit;
            replaceDisplay = false;
            status.Text = "";
        }
        void SetOperation(string op)
        {
            try
            {
                var value = ReadValue();
                if (accumulator.HasValue && operation is not null && !replaceDisplay)
                    value = Calculate(accumulator.Value, value, operation);
                accumulator = value;
                operation = op;
                Show(value);
                replaceDisplay = true;
                status.Text = "";
            }
            catch (Exception ex) { status.Text = $"Calculation error: {ex.Message}"; accumulator = null; operation = null; }
        }
        void EqualsResult()
        {
            if (!accumulator.HasValue || operation is null) return;
            try { Show(Calculate(accumulator.Value, ReadValue(), operation)); status.Text = ""; }
            catch (Exception ex) { status.Text = $"Calculation error: {ex.Message}"; }
            accumulator = null; operation = null; replaceDisplay = true;
        }
        var grid = new UniformGrid { Columns = 4, Margin = new Thickness(0, 0, 0, 6) };
        foreach (var label in new[] { "7", "8", "9", "/", "4", "5", "6", "*", "1", "2", "3", "-", "C", "0", "=", "+" })
        {
            var button = ViewFactoryStyles.Button(label, (_, _) =>
            {
                if (label.Length == 1 && char.IsDigit(label[0])) InputDigit(label);
                else if (label is "+" or "-" or "*" or "/") SetOperation(label);
                else if (label == "=") EqualsResult();
                else { Show(0); accumulator = null; operation = null; replaceDisplay = true; status.Text = ""; }
            });
            button.Margin = new Thickness(2); button.MinHeight = 44; grid.Children.Add(button);
        }
        page.Children.Add(display); page.Children.Add(grid); page.Children.Add(status);
        return page;
    }

    private static decimal Calculate(decimal left, decimal right, string operation) => operation switch
    {
        "+" => left + right,
        "-" => left - right,
        "*" => left * right,
        "/" => right == 0 ? throw new DivideByZeroException("Cannot divide by zero.") : left / right,
        _ => throw new InvalidOperationException("Unsupported operation.")
    };
}
