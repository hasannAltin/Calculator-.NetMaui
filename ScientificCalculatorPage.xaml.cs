namespace YeniHesapMakinesi;

public partial class ScientificCalculatorPage : ContentPage
{
	public ScientificCalculatorPage()
	{
		InitializeComponent();
        BindingContext = new CalculatorViewModel(true);
    }
}