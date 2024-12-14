using Florea_Daniel_Lab7.Models;
using System.Diagnostics;
namespace Florea_Daniel_Lab7;

public partial class ListPage : ContentPage
{
    public ListPage()
    {
        InitializeComponent();
    }
    async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        var slist = (ShopList)BindingContext;
        slist.Date = DateTime.UtcNow;
        await App.Database.SaveShopListAsync(slist);
        await Navigation.PopAsync();
    }
    async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        var slist = (ShopList)BindingContext;
        await App.Database.DeleteShopListAsync(slist);
        await Navigation.PopAsync();
    }
    async void OnDeleteItemButtonClicked(object sender, EventArgs e)
    {
        var selectedProduct = listView.SelectedItem as Product;

        if (selectedProduct == null)
        {
            await DisplayAlert("Error", "No product selected. Please select a product to delete.", "OK");
            return;
        }

        var shopList = (ShopList)BindingContext;

        var listProduct = await App.Database.GetListProductByIdsAsync(shopList.ID, selectedProduct.ID);

        if (listProduct == null)
        {
            await DisplayAlert("Error", "The selected product is not associated with the shopping list.", "OK");
            return;
        }

        bool confirm = await DisplayAlert(
            "Delete Product",
            $"Are you sure you want to delete '{selectedProduct.Description}'?",
            "Yes",
            "No");

        if (!confirm)
            return;

        await App.Database.DeleteListProductAsync(listProduct);

        listView.ItemsSource = await App.Database.GetListProductsAsync(shopList.ID);

        listView.SelectedItem = null;
    }
    async void OnChooseButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProductPage((ShopList)
    this.BindingContext)
        {
            BindingContext = new Product()
        });
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var shopl = (ShopList)BindingContext;

        listView.ItemsSource = await App.Database.GetListProductsAsync(shopl.ID);
    }
    void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Product selectedProduct)
        {
            Debug.WriteLine($"Selected Product: {selectedProduct.Description}");
        }
        else
        {
            Debug.WriteLine("Selection cleared or null.");
        }
    }
}