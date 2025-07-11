using Microsoft.UI.Xaml.Controls;
using biomed.ViewModels;
using Microsoft.UI.Xaml;
using biomed.Models;
using System.Linq;

namespace biomed.Views
{
    public sealed partial class EducationPage : Page
    {
        public EducationViewModel ViewModel { get; }

        public EducationPage()
        {
            this.InitializeComponent();
            ViewModel = App.GetService<EducationViewModel>();
            this.Loaded += EducationPage_Loaded;
        }

        private async void EducationPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadInitialDataAsync();
        }

        private async void CategoriesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.FirstOrDefault() is EduCategory selectedCategory)
            {
                await ViewModel.LoadResourcesAsync(selectedCategory.Id.ToString());
            }
        }

        private void ResourcesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is EduResource clickedResource)
            {
                ViewModel.ShowResourceDetailsCommand.Execute(clickedResource);
            }
        }

        private void VideosGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is VideoDto clickedVideo)
            {
                ViewModel.PlayVideoCommand.Execute(clickedVideo);
            }
        }
    }
} 