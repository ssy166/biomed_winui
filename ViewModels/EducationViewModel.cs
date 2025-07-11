using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using biomed.Models;
using biomed.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using System.Linq;
using System.Text;
using System;
using biomed.Views;

namespace biomed.ViewModels
{
    public partial class EducationViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;

        public EducationViewModel()
        {
            _apiClient = App.GetService<ApiClient>();
            Categories = new ObservableCollection<EduCategory>();
            Resources = new ObservableCollection<EduResource>();
            Videos = new ObservableCollection<VideoDto>();
        }

        [ObservableProperty]
        private ObservableCollection<EduCategory> _categories;

        [ObservableProperty]
        private ObservableCollection<EduResource> _resources;

        [ObservableProperty]
        private ObservableCollection<VideoDto> _videos;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsError))]
        private string _errorMessage;

        [ObservableProperty]
        private bool _isBusy;
        
        public bool IsError => !string.IsNullOrEmpty(ErrorMessage);
        
        public async Task LoadInitialDataAsync()
        {
            // Sequentially load categories, then all resources and videos.
            await LoadCategoriesAsync();
            await LoadResourcesAsync();
            await LoadVideosAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            IsBusy = true;
            ErrorMessage = null;
            try
            {
                var categories = await _apiClient.GetEduCategoriesAsync();
                Categories.Clear();
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"加载分类失败: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task LoadResourcesAsync(string categoryId = null)
        {
            IsBusy = true;
            ErrorMessage = null;
            try
            {
                var pagedResult = await _apiClient.GetEduResourcesAsync(categoryId: categoryId);
                Resources.Clear();
                if (pagedResult?.Content != null)
                {
                    foreach (var resource in pagedResult.Content)
                    {
                        Resources.Add(resource);
                    }
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"加载资源失败: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadVideosAsync()
        {
            IsBusy = true;
            ErrorMessage = null;
            try
            {
                var pagedResult = await _apiClient.GetVideosAsync(status: "published");
                Videos.Clear();
                if (pagedResult?.Content != null)
                {
                    foreach (var video in pagedResult.Content)
                    {
                        Videos.Add(video);
                    }
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"加载视频失败: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ShowResourceDetails(EduResource resource)
        {
            if (resource == null) return;

            IsBusy = true;
            ErrorMessage = null;
            try
            {
                var resourceDetail = await _apiClient.GetResourceDetailAsync(resource.Id);
                var detailView = new EduResourceDetailDialog(resourceDetail);

                var dialog = new ContentDialog
                {
                    Title = resourceDetail.Title,
                    Content = detailView,
                    CloseButtonText = "关闭",
                    XamlRoot = App.MainRoot,
                    Width = 600
                };

                dialog.Closing += (s, e) =>
                {
                    if (s.Content is EduResourceDetailDialog view)
                    {
                        view.Cleanup();
                    }
                };

                await dialog.ShowAsync().AsTask();
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"加载资源详情失败: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task PlayVideo(VideoDto video)
        {
            if (video == null) return;

            try
            {
                // 导航到视频播放页面
                var navigationService = App.GetService<INavigationService>();
                navigationService.Navigate(typeof(VideoPlayerPage), video);
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"播放视频失败: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task ShowArticleDetail(EduResource resource)
        {
            if (resource == null) return;

            try
            {
                // 导航到图文详情页面
                var navigationService = App.GetService<INavigationService>();
                navigationService.Navigate(typeof(ArticleDetailPage), resource);
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"查看图文详情失败: {ex.Message}";
            }
        }
    }
} 