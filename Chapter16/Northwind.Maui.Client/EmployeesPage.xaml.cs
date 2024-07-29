using System.Runtime.Versioning;

namespace Northwind.Maui.Client;

public partial class EmployeesPage : ContentPage
{
    public EmployeesPage()
    {
        InitializeComponent();
    }
    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        foreach (Button button in GridCalculator.Children.OfType<Button>())
        {
            button.FontSize = 24;
            button.WidthRequest = 54;
            button.HeightRequest = 54;
            button.Clicked += Button_Clicked;
        }
    }

    private void Button_Clicked(object? sender, EventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        string operationChars = "+-/X=";
        _ = operationChars.Contains(button.Text) ? Output.Text = string.Empty : Output.Text += button.Text;

    }
}