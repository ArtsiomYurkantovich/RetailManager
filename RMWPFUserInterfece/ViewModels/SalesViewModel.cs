using System;
using Caliburn.Micro;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMDesktopUI.Library.Api;
using RMDesktopUI.Library.Models;

namespace RMWPFUserInterfece.ViewModels
{
    public class SalesViewModel : Screen
    {
        IProductEndPoint _productEndPoint;
        public SalesViewModel(IProductEndPoint productEndPoint)
        {
            _productEndPoint = productEndPoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadProducts();
        }
        private async Task LoadProducts()
        {
            var productList = await _productEndPoint.GetAll();
            Products = new BindingList<ProductModel>(productList);
        }
        private BindingList<ProductModel> _products;

        public BindingList<ProductModel> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }

        private int _itemQuantity;

        public int ItemQuantity
        {
            get { return _itemQuantity; }
            set
            {
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
            }
        }

        private BindingList<ProductModel> _cart;

        public BindingList<ProductModel> Cart
        {
            get { return _cart; }
            set
            {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }

        public bool CanAddToCart
        {
            get
            {
                bool output = false;

                //Make sure something is selected

                return output;
            }
        }

        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;

                //Make sure something is selected

                return output;
            }
        }
        public bool CanCheckOut
        {
            get
            {
                bool output = false;

                //Make sure there is something in the cart

                return output;
            }
        }
        public string SubTotal
        {
            get
            {
                //TODO calculation subtotal
                return "$0.00";
            }
        }

        public string Total
        {
            get
            {
                //TODO calculation total
                return "$0.00";
            }
        }

        public string Tax
        {
            get
            {
                //TODO calculation tax
                return "$0.00";
            }
        }
        public void AddToCart()
        {

        }

        public void RemoveFromCart()
        {

        }

        public void CheckOut()
        {

        }

    }
}
