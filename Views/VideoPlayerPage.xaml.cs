using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using biomed.Models;
using biomed.Services;
using System;
using Windows.Media.Core;

namespace biomed.Views
{
    public sealed partial class VideoPlayerPage : Page
    {
        public VideoDto Video { get; private set; }

        public VideoPlayerPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            try
            {
                if (e.Parameter is VideoDto video && video != null)
                {
                    Video = video;
                    LoadVideo();
                }
                else
                {
                    ShowError("无法加载视频信息：参数无效");
                }
            }
            catch (Exception ex)
            {
                ShowError($"初始化视频播放器失败: {ex.Message}");
            }
        }

        private void LoadVideo()
        {
            try
            {
                // 检查Video对象是否为null
                if (Video == null)
                {
                    ShowError("视频对象为空");
                    return;
                }

                // 检查视频URL是否有效
                if (string.IsNullOrWhiteSpace(Video.VideoUrl))
                {
                    ShowError("视频地址无效或为空");
                    return;
                }

                // 验证URL格式
                if (!Uri.TryCreate(Video.VideoUrl, UriKind.Absolute, out Uri videoUri))
                {
                    ShowError($"视频地址格式无效: {Video.VideoUrl}");
                    return;
                }

                // 创建媒体源
                var mediaSource = MediaSource.CreateFromUri(videoUri);
                
                // 设置媒体源到播放器
                if (VideoPlayer != null)
                {
                    VideoPlayer.Source = mediaSource;
                }
                else
                {
                    ShowError("视频播放器控件未初始化");
                }
            }
            catch (Exception ex)
            {
                ShowError($"加载视频失败: {ex.Message}");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var navigationService = App.GetService<INavigationService>();
                navigationService.NavigateTo(typeof(EducationPage));
            }
            catch (Exception ex)
            {
                ShowError($"返回教学平台失败: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            try
            {
                if (ErrorInfoBar != null)
                {
                    ErrorInfoBar.Message = message;
                    ErrorInfoBar.IsOpen = true;
                }
            }
            catch
            {
                // 如果错误信息栏也有问题，则忽略
            }
        }

        public string FormatDate(DateTime dateTime)
        {
            try
            {
                return dateTime.ToString("yyyy-MM-dd");
            }
            catch
            {
                return "未知日期";
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            try
            {
                // 清理资源
                if (VideoPlayer != null)
                {
                    VideoPlayer.Source = null;
                }
            }
            catch
            {
                // 忽略清理时的错误
            }
            
            base.OnNavigatedFrom(e);
        }
    }
} 