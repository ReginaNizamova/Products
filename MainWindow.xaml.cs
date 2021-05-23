using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Products
{

    public partial class Product 
    {
       
        private string productTypeTitle;
        public string ProductTypeTitle
        {
            get { return productTypeTitle; }
            set
            {
                productTypeTitle = value;
                OnPropertyChanged("ProductTypeTitle");
            }
        }

        private string titleMaterial;
        public string TitleMaterial
        {
            get { return titleMaterial; }
            set
            {
                titleMaterial = value;
                OnPropertyChanged("TitleMaterial");
            }
        }

        private decimal costMaterial;
        public decimal CostMaterial
        {
            get { return costMaterial; }
            set
            {
                costMaterial = value;
                OnPropertyChanged("CostMaterial");
            }
        }
        //public int ProductID { get; set; }
        //public int MaterialID { get; set; }
        //public int idProduct { get; set; }
        //public int idMaterial { get; set; }
    }

    public partial class MainWindow : Window
    {

        List<Product> listProducts = new List<Product>();
        List<Product> listProductsTwenty = new List<Product>();
        int pageIndex = -1;
        int pageSize = 20;
        readonly ProductEntities dataContext = new ProductEntities();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void listView_Loaded(object sender, RoutedEventArgs e)
        {
           
            //var resultQuery = from p in dataContext.ProductMaterial
            //                  group p by p.Product.Title into g
            //                  select new
            //                  {
                                 
            //                      article = from p in g select p.Product.ArticleNumber,
            //                      name = g.Key,
            //                      material = from p in g select p.Material.Title,
            //                      cost = from p in g select p.Material.Cost,
            //                      productType = from p in g select p.Product.ProductType.Title,
            //                  };            
            
            var resultQuery = from p in dataContext.ProductMaterial
                              select new
                              {
                                  id = p.ProductID,
                                  article = p.Product.ArticleNumber,
                                  name = p.Product.Title,
                                  material = p.Material.Title,
                                  cost = p.Material.Cost,
                                  productType = p.Product.ProductType.Title,
                                  image = p.Product.Image

                              };


            foreach (var item in resultQuery)
            {
                listProducts.Add(new Product()
                { 
                    Title = item.name,
                    ProductTypeTitle = item.productType,
                    ArticleNumber = item.article,
                    CostMaterial = item.cost,
                    TitleMaterial = item.material  
                });
            }

         
        }

        private void forwardButton_Click(object sender, RoutedEventArgs e)
        {

            pageIndex++;
            listProductsTwenty = listProducts.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            listView.ItemsSource = listProductsTwenty;
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
           
            pageIndex--;
            listProductsTwenty = listProducts.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            listView.ItemsSource = listProductsTwenty;
        }
    }
}
