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
        int numberPage = 1;

        readonly ProductEntities dataContext = new ProductEntities();
        public MainWindow()
        {
            InitializeComponent();          
        }

        private int CountProduct () //количество продуктов
        {
            var countPage = from p in dataContext.Product
                            select p.ID;

          return  countPage.Count();
        }
        private void listView_Loaded(object sender, RoutedEventArgs e)
        {
            
            var resultQuery = from p in dataContext.ProductMaterial
                              select new
                              {
                                  id = p.ProductID,
                                  article = p.Product.ArticleNumber,
                                  name = p.Product.Title,
                                  material = p.Material.Title,
                                  cost = p.Material.Cost,
                                  productType = p.Product.ProductType.Title,
                                  image = p.Product.Image,
                                  productionWorkshopNumber = p.Product.ProductionWorkshopNumber,
                                  minCostForAgent = p.Product.MinCostForAgent

                              };


            foreach (var item in resultQuery)
            {
                listProducts.Add(new Product()
                {
                    Title = item.name,
                    ProductTypeTitle = item.productType,
                    ArticleNumber = item.article,
                    CostMaterial = item.cost,
                    TitleMaterial = item.material,
                    ProductionWorkshopNumber = item.productionWorkshopNumber,
                    MinCostForAgent = item.minCostForAgent
                });
            }

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
            pageIndex++;
            listProductsTwenty = listProducts.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            listView.ItemsSource = listProductsTwenty;
            

        }

        private void forwardButton_Click(object sender, RoutedEventArgs e) //след. страница
        {
            int countPage = CountProduct();

            if (numberPage == countPage / pageSize)
            {
                forwardButton.IsEnabled = false;
            }
            else
            {
                backButton.IsEnabled = true;
                numberPage++;
                pageIndex++;
                listProductsTwenty = listProducts.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                listView.ItemsSource = listProductsTwenty;

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
                view.Filter = ProductSeach;
            }
           
        }

        private void backButton_Click(object sender, RoutedEventArgs e) // пред. страница
        {
            int countPage = CountProduct();

            if (numberPage == 1)
            {
                backButton.IsEnabled = false;
            }


            forwardButton.IsEnabled = true;
            numberPage--;
            pageIndex--;
            listProductsTwenty = listProducts.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            listView.ItemsSource = listProductsTwenty;


            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
            view.Filter = ProductSeach;
        }

        private void comboBoxSort(object sender, SelectionChangedEventArgs e) //Сортировка 
        {
            ComboBoxItem comboBox = (ComboBoxItem)sort.SelectedItem;

            string valueComboBoxSort = comboBox.Content.ToString();


            switch (valueComboBoxSort)
            {
                case "Не сортировать":
                    {

                        listProductsTwenty = listProducts.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                        listView.ItemsSource = listProductsTwenty;
                        CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);

                        view.Filter = ProductSeach;

                        break;
                    }


                case "↑ Наименование":
                    {
                
                        listProductsTwenty = listProducts.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                        listView.ItemsSource = listProductsTwenty;
                       
                        CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
                        view.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));
                        view.Filter = ProductSeach;

                        break;
                    }

                case "↓ Наименование":
                    {
                       
                        listProductsTwenty = listProducts.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                        listView.ItemsSource = listProductsTwenty;
                     
                        CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
                        view.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Descending));
                        view.Filter = ProductSeach;
                        break;
                    }

                case "↑ Номер производственного цеха":
                    {
                        listProductsTwenty = listProducts.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                        listView.ItemsSource = listProductsTwenty;

                        CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
                        view.SortDescriptions.Add(new SortDescription("ProductionWorkshopNumber", ListSortDirection.Ascending));
                        view.Filter = ProductSeach;

                        break;
                    }

                case "↓ Номер производственного цеха":
                    {
                        listProductsTwenty = listProducts.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                        listView.ItemsSource = listProductsTwenty;

                        CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
                        view.SortDescriptions.Add(new SortDescription("ProductionWorkshopNumber", ListSortDirection.Descending));
                        view.Filter = ProductSeach;

                        break;
                    }

                case "↑ Минимальная стоимость для агента":
                    {
                        listProductsTwenty = listProducts.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                        listView.ItemsSource = listProductsTwenty;

                        CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
                        view.SortDescriptions.Add(new SortDescription("MinCostForAgent", ListSortDirection.Ascending));
                        view.Filter = ProductSeach;

                        break;
                    }

                case "↓ Минимальная стоимость для агента":
                    {
                        listProductsTwenty = listProducts.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                        listView.ItemsSource = listProductsTwenty;

                        CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
                        view.SortDescriptions.Add(new SortDescription("MinCostForAgent", ListSortDirection.Descending));
                        view.Filter = ProductSeach;

                        break;
                    }
            }

        }

        private bool ProductSeach(object item) // Поиск
        {
            if (String.IsNullOrEmpty(search.Text))
                return true;
            else
                return ((item as Product).Title.IndexOf(search.Text, StringComparison.OrdinalIgnoreCase) >= 0) || ((item as Product).ArticleNumber.IndexOf(search.Text, StringComparison.OrdinalIgnoreCase) >= 0) || ((item as Product).TitleMaterial.IndexOf(search.Text, StringComparison.OrdinalIgnoreCase) >= 0);

        }

        private bool ProductFilter(object item) //Фильтр
        {
            if (String.IsNullOrEmpty(filter.Text))
                return true;
            else
                return ((item as Product).TitleMaterial.IndexOf(filter.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void txtFilterTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) // обновляет listView при изменении TextBox
        {
           
            CollectionViewSource.GetDefaultView(listView.ItemsSource).Refresh();
    
        }

        private void ShowOnlyBargainsFilter(object sender, FilterEventArgs e)
        {
            ListViewItem product = e.Item as ListViewItem;
            if (product != null)
            {
                if (product.Content.ToString() == "Колесо")
                {
                    e.Accepted = true;
                }
                else
                {
                    e.Accepted = false;
                }
            }
        }
        private void comboBoxfilter(object sender, SelectionChangedEventArgs e) // ComboBox фильтра
        {

            ComboBoxItem comboBox = (ComboBoxItem)filter.SelectedItem;

            string valueComboBoxFilter = comboBox.Content.ToString();


            switch (valueComboBoxFilter)
            {
                case "Все типы":
                    {                      
                        listProductsTwenty = listProducts.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                        listView.ItemsSource = listProductsTwenty;
                    
                        break;
                    }

                case "Колесо":
                    {
                        CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);

                        listProductsTwenty = listProducts.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                        listView.ItemsSource = listProductsTwenty;
                        view.Filter += new FilterEventHandler(ShowOnlyBargainsFilter);

                        break;
                    }

                case "Диск":
                    {
                        CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);

                        listProductsTwenty = listProducts.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                        listView.ItemsSource = listProductsTwenty; 
                        view.Filter += new FilterEventHandler(ShowOnlyBargainsFilter);
                 

                        break;
                    }

                case "Запаска":
                    {
                        CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
                   
                        listProductsTwenty = listProducts.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                        listView.ItemsSource = listProductsTwenty;
                        view.Filter += new FilterEventHandler(ShowOnlyBargainsFilter);

                        break;
                    }

            }


        }


       
    }
}
