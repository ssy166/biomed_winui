using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System;
using System.Threading.Tasks;
using biomed.ViewModels;
using biomed.Models;
using Microsoft.Extensions.DependencyInjection;

namespace biomed.Views
{
    public sealed partial class ResearchPlatformPage : Page
    {
        public ResearchPlatformViewModel ViewModel { get; }

        public ResearchPlatformPage()
        {
            // 先初始化 ViewModel，然后再调用 InitializeComponent
            ViewModel = App.Services.GetRequiredService<ResearchPlatformViewModel>();
            this.DataContext = ViewModel;
            
            this.InitializeComponent();
            this.Loaded += OnPageLoaded;
        }

        private async void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // 先初始化 ViewModel 数据
                await ViewModel.InitializeAsync();
                
                // 根据用户角色设置默认选项卡
                if (ViewModel.IsTeacher)
                {
                    // Teachers default to the "Applications" tab, which is the 2nd item (index 2).
                    MainPivot.SelectedIndex = 2;
                }
            }
            catch (Exception ex)
            {
                // 显示错误信息
                var dialog = new ContentDialog()
                {
                    Title = "加载失败",
                    Content = $"无法加载研究平台数据：{ex.Message}",
                    CloseButtonText = "确定",
                    XamlRoot = this.XamlRoot
                };
                await dialog.ShowAsync();
            }
        }

        public string GetRoleDescription(bool isTeacher)
        {
            return isTeacher ? "教师管理平台" : "学生研究平台";
        }

        public string GetProjectsTitle(bool isTeacher)
        {
            return isTeacher ? "我的项目" : "可申请项目";
        }

        public string GetTasksTitle(bool isTeacher)
        {
            return "所有任务"; // 统一显示所有任务
        }

        public string GetApplicationsTitle(bool isTeacher)
        {
            return isTeacher ? "待审核申请" : "我的申请";
        }

        public string GetSubmissionsTitle(bool isTeacher)
        {
            return "所有论文"; // 统一显示所有论文
        }
    }
} 