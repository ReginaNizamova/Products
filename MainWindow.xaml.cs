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
using System.Drawing;
using Microsoft.WindowsAPICodePack.Shell;

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

        private byte byteImage;
        public byte ByteImage
        {
            get { return byteImage; }
            set
            {
                byteImage = value;
                OnPropertyChanged("ByteImage");
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
        readonly int pageSize = 20;
        int numberPage = 1;

        readonly ProductEntities dataContext = new ProductEntities();
        public MainWindow()
        {
            InitializeComponent();
            string filePath = @"C:\Users\User\Desktop\Product\Resource\products\tire_0.jpg";
            var image = GetImage(filePath);
            var byteArray = ByteArray(image);
            var q = ToBitmapSource(byteArray);
            qq.Source =  q;

        }

        private int CountProduct () //количество продуктов
        {
            var countPage = from p in dataContext.Product
                            select p.ID;

          return  countPage.Count();
        }
        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            string filePath = @"C:\Users\User\Desktop\Product\Resource\products\tire_0.jpg";
            var image = GetImage(filePath);
            var byteArray = ByteArray(image);
            var q = ToBitmapSource(byteArray);
            qq.Source = q;


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
                            minCostForAgent = p.Product.MinCostForAgent,
                            

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
                    MinCostForAgent = item.minCostForAgent,

                });
            }

            //CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
            pageIndex++;
            listProductsTwenty = listProducts.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            //listView.ItemsSource = listProductsTwenty;
        }


        public static BitmapSource GetImage (string filePath)
        {
            using (var file = ShellFile.FromFilePath (filePath))
            {
                file.Thumbnail.FormatOption = ShellThumbnailFormatOption.IconOnly;
                return file.Thumbnail.SmallBitmapSource;
            }
        }

        public static byte [] ByteArray (BitmapSource bitmapSource)
        {
            using (var memory = new MemoryStream())
            {
                var encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(memory);
                return memory.GetBuffer();
            }
        }

        public static BitmapSource ToBitmapSource (byte [] byteArray)
        {
            using (var memory = new MemoryStream (byteArray))
            {
                var image = new BitmapImage();
                memory.Seek(0, SeekOrigin.Begin);
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.StreamSource = memory;
                image.EndInit();
                image.Freeze();
                return image;
            }
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e) //след. страница
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
                //listView.ItemsSource = listProductsTwenty;

                //CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
                //view.Filter = ProductSeach;
            }
           
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) // пред. страница
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
            //listView.ItemsSource = listProductsTwenty;


            //CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
            //view.Filter = ProductSeach;
        }

        private void ComboBoxSort(object sender, SelectionChangedEventArgs e) //Сортировка 
        {
            ComboBoxItem comboBox = (ComboBoxItem)sort.SelectedItem;

            string valueComboBoxSort = comboBox.Content.ToString();


            switch (valueComboBoxSort)
            {
                case "Не сортировать":
                    {

                        listProductsTwenty = listProducts.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                        //listView.ItemsSource = listProductsTwenty;
                        //CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);

                        //view.Filter = ProductSeach;

                        break;
                    }


                case "↑ Наименование":
                    {
                
                        listProductsTwenty = listProducts.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                        //listView.ItemsSource = listProductsTwenty;
                       
                        //CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
                        //view.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));
                        //view.Filter = ProductSeach;

                        break;
                    }

                case "↓ Наименование":
                    {
                       
                        listProductsTwenty = listProducts.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                        //listView.ItemsSource = listProductsTwenty;
                     
                        //CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
                        //view.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Descending));
                        //view.Filter = ProductSeach;
                        break;
                    }

                case "↑ Номер производственного цеха":
                    {
                        listProductsTwenty = listProducts.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                        //listView.ItemsSource = listProductsTwenty;

                        //CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
                        //view.SortDescriptions.Add(new SortDescription("ProductionWorkshopNumber", ListSortDirection.Ascending));
                        //view.Filter = ProductSeach;

                        break;
                    }

                case "↓ Номер производственного цеха":
                    {
                        listProductsTwenty = listProducts.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                        //listView.ItemsSource = listProductsTwenty;

                        //CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
                        //view.SortDescriptions.Add(new SortDescription("ProductionWorkshopNumber", ListSortDirection.Descending));
                        //view.Filter = ProductSeach;

                        break;
                    }

                case "↑ Минимальная стоимость для агента":
                    {
                        listProductsTwenty = listProducts.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                        //listView.ItemsSource = listProductsTwenty;

                        //CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
                        //view.SortDescriptions.Add(new SortDescription("MinCostForAgent", ListSortDirection.Ascending));
                        //view.Filter = ProductSeach;

                        break;
                    }

                case "↓ Минимальная стоимость для агента":
                    {
                        listProductsTwenty = listProducts.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                        //listView.ItemsSource = listProductsTwenty;

                        //CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
                        //view.SortDescriptions.Add(new SortDescription("MinCostForAgent", ListSortDirection.Descending));
                        //view.Filter = ProductSeach;

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

        private void FilterTextChanged(object sender, TextChangedEventArgs e) // обновляет listView при изменении TextBox
        {

            //CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
            //view.Filter = ProductSeach;
            //CollectionViewSource.GetDefaultView(listView.ItemsSource).Refresh();
    
        }

        private void ComboBoxfilter(object sender, SelectionChangedEventArgs e) // ComboBox фильтра
        {
            
            ProductEntities dataContext = new ProductEntities();
            ComboBoxItem comboBox = (ComboBoxItem)filter.SelectedItem;
            ComboBoxItem selectedItem = new ComboBoxItem();

            //if (Convert.ToString(selectedItem.Content) == "Все типы")
            //{
            //    listProductsTwenty = listProducts.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            //    listView.ItemsSource = listProductsTwenty;
            //}

            //else
            //{
            //    listProductsTwenty = listProducts.Where(p => p.ProductType.Title == Convert.ToString(selectedItem.Content)).ToList();
            //    listView.ItemsSource = listProductsTwenty;
            //}
        }

    }
}
