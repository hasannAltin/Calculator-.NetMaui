namespace YeniHesapMakinesi;

public partial class StandardCalculatorPage : ContentPage
{
	public StandardCalculatorPage()
	{
		InitializeComponent();
        BindingContext = new CalculatorViewModel();
    }
}