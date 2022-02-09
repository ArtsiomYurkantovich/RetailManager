using System;
using Caliburn.Micro;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMDesktopUI.Library.Api;
using RMDesktopUI.Library.Models;
using RMDesktopUI.Library.Helpers;
using AutoMapper;
using RMWPFUserInterfece.Models;
using System.Collections.Generic;
using System.Dynamic;
using System.Windows;

namespace RMWPFUserInterfece.ViewModels
{
    public class SalesViewModel : Screen
    {
        IProductEndPoint _productEndPoint;
        IConfigHelper _configHelper;
        ISaleEndPoint _saleEndPoint;
        IMapper _mapper;
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _manager;

        public SalesViewModel(IProductEndPoint productEndPoint, IConfigHelper configHelper, ISaleEndPoint saleEndPoint, IMapper mapper, StatusInfoViewModel status, IWindowManager manager)
        {
            _productEndPoint = productEndPoint;
            _configHelper = configHelper;
            _saleEndPoint = saleEndPoint;
            _mapper = mapper;
            _status = status;
            _manager = manager;
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadProducts();

            }
            catch (Exception ex)
            {
                dynamic settings = new ExpandoObject();
                settings.WindowsStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "System Eror";
                if (ex.Message == "Unauthorized")
                {
                    _status.UpdateMessage("Unauthorized Access", "You do not have permission to interact with the Sales Form.");
                    _manager.ShowDialog(_status, null, null);
                }
                else
                {
                    _status.UpdateMessage("Fatal Exeption", ex.Message);
                    _manager.ShowDialog(_status, null, null);
                }
                TryClose();

            }
        }

        private async Task LoadProducts()
        {
            var productList = await _productEndPoint.GetAll();
            var products = _mapper.Map<List<ProductDisplayModel>>(productList);
            Products = new BindingList<ProductDisplayModel>(products);
        }

        public async Task ResetSalesViewModel()
        {
            Cart = new BindingList<CartItemDisplayModel>();
            await LoadProducts();
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
        }

        private BindingList<ProductDisplayModel> _products;

        public BindingList<ProductDisplayModel> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }

        public ProductDisplayModel _selectedProduct { get; set; }
        public ProductDisplayModel SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);

            }
        }

        public CartItemDisplayModel _selectedCartItem { get; set; }
        public CartItemDisplayModel SelectedCartItem
        {
            get { return _selectedCartItem; }
            set
            {
                _selectedCartItem = value;
                NotifyOfPropertyChange(() => SelectedCartItem);
                NotifyOfPropertyChange(() => CanRemoveFromCart);

            }
        }

        private int _itemQuantity = 1;

        public int ItemQuantity
        {
            get { return _itemQuantity; }
            set
            {
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
                NotifyOfPropertyChange(() => CanAddToCart);


            }
        }

        private BindingList<CartItemDisplayModel> _cart = new BindingList<CartItemDisplayModel>();

        public BindingList<CartItemDisplayModel> Cart
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

                if (ItemQuantity > 0 && SelectedProduct?.QuantityInStock >= ItemQuantity)
                {
                    output = true;
                }

                return output;
            }
        }

        public string SubTotal
        {
            get
            {
                return CalculateSubTotal().ToString("C");
            }
        }

        private decimal CalculateSubTotal()
        {
            decimal subTotal = 0;
            foreach (var item in Cart)
            {
                subTotal += item.Product.RetailPrice * item.QuantityInCart;
            }
            return subTotal;
        }

        public string Tax
        {
            get
            {

                return CalculateTotalTax().ToString("C");
            }
        }

        private decimal CalculateTotalTax()
        {
            decimal taxAmount = 0;
            decimal taxRate = _configHelper.GetTaxRate() / 100;

            taxAmount = Cart
                .Where(x => x.Product.IsTaxable)
                .Sum(x => x.Product.RetailPrice * x.QuantityInCart * taxRate);

            //foreach (var item in Cart)
            //{
            //    if (item.Product.IsTaxable)
            //    {
            //        taxAmount = (item.Product.RetailPrice * item.QuantityInCart * taxRate);
            //    }
            //}
            return taxAmount;
        }

        public string Total
        {
            get
            {
                decimal total = CalculateSubTotal() + CalculateTotalTax();
                return total.ToString("C");

            }
        }

        public void AddToCart()
        {
            CartItemDisplayModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);
            if (existingItem != null)
            {
                existingItem.QuantityInCart += ItemQuantity;
            }
            else
            {
                CartItemDisplayModel item = new CartItemDisplayModel
                {
                    Product = SelectedProduct,
                    QuantityInCart = ItemQuantity
                };
                Cart.Add(item);
            }

            SelectedProduct.QuantityInStock -= ItemQuantity;
            ItemQuantity = 1;
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);

        }

        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;

                if (SelectedCartItem != null && SelectedCartItem?.QuantityInCart > 0)
                {
                    output = true;
                }

                return output;
            }
        }

        public void RemoveFromCart()
        {
            SelectedCartItem.Product.QuantityInStock += 1;

            if (SelectedCartItem.QuantityInCart > 1)
            {
                SelectedCartItem.QuantityInCart -= 1;
            }
            else
            {
                Cart.Remove(SelectedCartItem);
            }

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
            NotifyOfPropertyChange(() => CanAddToCart);



        }
        public bool CanCheckOut
        {
            get
            {
                bool output = false;

                if (Cart.Count > 0)
                {
                    output = true;
                }

                return output;
            }
        }
        public async Task CheckOut()
        {
            SaleModel sale = new SaleModel();

            foreach (var item in Cart)
            {
                sale.SaleDetails.Add(new SaleDetailModel
                {
                    ProductId = item.Product.Id,
                    Quantity = item.QuantityInCart
                });
            }

            await _saleEndPoint.PostSale(sale);
            await ResetSalesViewModel();
        }

    }
}
