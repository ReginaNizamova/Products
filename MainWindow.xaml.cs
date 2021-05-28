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
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace Products
{
    
    public partial class Product
    {
      
        private string productTypeTitle;  
        [JsonIgnore]
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
        [JsonIgnore]
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
        [JsonIgnore]
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

        private int CountProduct() //количество продуктов
        {
            var countPage = from p in dataContext.Product
                            select p.ID;

            return countPage.Count();
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
                                  image = p.Product.ImageByte,
                                  productionWorkshopNumber = p.Product.ProductionWorkshopNumber,
                                  minCostForAgent = p.Product.MinCostForAgent
                                  
                             };


            foreach (var item in resultQuery)
            {
                listProducts.Add(new Product()
                {
                    ID = item.id,
                    Title = item.name,
                    ProductTypeTitle = item.productType,
                    ArticleNumber = item.article,
                    CostMaterial = item.cost,
                    TitleMaterial = item.material,
                    ProductionWorkshopNumber = item.productionWorkshopNumber,
                    MinCostForAgent = item.minCostForAgent,          
           
                });
            }

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
            pageIndex++;
            listProductsTwenty = listProducts.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            listView.ItemsSource = listProductsTwenty;
            Serialized();
            Deserialized();
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
                listView.ItemsSource = listProductsTwenty;

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
                view.Filter = ProductSeach;
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
            listView.ItemsSource = listProductsTwenty;


            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
            view.Filter = ProductSeach;
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

        private void FilterTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) // обновляет listView при изменении TextBox
        {

            CollectionViewSource.GetDefaultView(listView.ItemsSource).Refresh();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
            
            view.Filter = ProductSeach;
        }

        private void ComboBoxfilter(object sender, SelectionChangedEventArgs e) // Фильтр
        {
            ComboBoxItem comboBox = (ComboBoxItem)filter.SelectedItem;

            if (Convert.ToString(comboBox.Content) == "Все типы")
            {
                listProductsTwenty = listProducts.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                listView.ItemsSource = listProductsTwenty;
            }
            else
            {
                listProductsTwenty = listProducts.Where(p => p.ProductTypeTitle == Convert.ToString(comboBox.Content)).ToList();
                listView.ItemsSource = listProductsTwenty;
            }
        }


        //private byte[] Serialized()
        //{
        //    string path = @"D:\bin\Student\Nizamova\Product\Resource\products/tire_0.jpg";
        //    using (System.Drawing.Image image = System.Drawing.Image.FromFile(path))
        //    {
        //        using (MemoryStream memory = new MemoryStream())
        //        {
        //            image.Save(memory, image.RawFormat);
        //            byte[] imageBytes = memory.ToArray();
        //            return imageBytes;
        //        }
        //    }
        //}

        private void Serialized()
        {
            string[] path = new string[64];
            byte[] imageBytes;
            Bitmap bitmap;


            for (int i = 0; i < 64; i++)
            {
                path[i] = @"D:\bin\Student\Nizamova\Product\Resource\products/tire_" + i + ".jpg";

                using (System.Drawing.Image image = System.Drawing.Image.FromFile(path[i]))
                {
                    using (MemoryStream memory = new MemoryStream())
                    {
                        image.Save(memory, image.RawFormat);
                        imageBytes = memory.ToArray();
                        bitmap = new Bitmap(image);
                    }
                }

                try
                {
                    Product product = new Product()
                    {
                        ImageByte = imageBytes
                    };

                    dataContext.Product.Add(product);
                    dataContext.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1}",
                                                    validationError.PropertyName,
                                                    validationError.ErrorMessage);
                        }
                    }
                }

            }
        }


        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        public void Deserialized ()
        {
            //byte[] byteArray = Serialized();
            //Bitmap bitmap = (Bitmap)((new ImageConverter()).ConvertFrom(byteArray));
            //q.Source = BitmapToImageSource(bitmap);
        }
    }
}

