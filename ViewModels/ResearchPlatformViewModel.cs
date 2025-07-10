using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using biomed.Models;
using biomed.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

namespace biomed.ViewModels
{
    public partial class ResearchPlatformViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _apiClient;
        private readonly IUserStore _userStore;

        private bool _isBusy;
        private string _errorMessage;
        private string _currentView = "Projects";
        private string _searchKeyword;

        public ResearchPlatformViewModel()
        {
            _apiClient = App.Services.GetRequiredService<ApiClient>();
            _userStore = App.Services.GetRequiredService<IUserStore>();

            Projects = new ObservableCollection<ResearchProject>();
            Tasks = new ObservableCollection<ResearchTask>();
            Applications = new ObservableCollection<ResearchApplication>();
            Submissions = new ObservableCollection<ResearchSubmission>();

            LoadProjectsCommand = new RelayCommand(async () => await LoadProjectsAsync());
            LoadTasksCommand = new RelayCommand(async () => await LoadTasksAsync());
            LoadApplicationsCommand = new RelayCommand(async () => await LoadApplicationsAsync());
            LoadSubmissionsCommand = new RelayCommand(async () => await LoadSubmissionsAsync());
            SearchCommand = new RelayCommand(async () => await SearchProjectsAsync());
            SubmitApplicationCommand = new RelayCommand<ResearchProject>(async (project) => await SubmitApplicationAsync(project));
            ViewTaskDetailsCommand = new RelayCommand<ResearchTask>(async (task) => await ViewTaskDetailsAsync(task));
            DownloadSubmissionCommand = new RelayCommand<ResearchSubmission>(async (submission) => await DownloadSubmissionAsync(submission));
            ApproveApplicationCommand = new RelayCommand<ResearchApplication>(async (app) => await ReviewApplicationAsync(app, "approve"));
            RejectApplicationCommand = new RelayCommand<ResearchApplication>(async (app) => await ReviewApplicationAsync(app, "reject"));

            // 不在构造函数中加载数据，而是等页面完全加载后再加载
        }

        // 添加初始化方法，在页面加载时调用
        public async Task InitializeAsync()
        {
            try
            {
                // 检查用户是否已登录
                if (!_userStore.IsLoggedIn)
                {
                    ErrorMessage = "请先登录以访问研究平台";
                    return;
                }
                
                await LoadProjectsAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"初始化失败: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public string CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public string SearchKeyword
        {
            get => _searchKeyword;
            set => SetProperty(ref _searchKeyword, value);
        }

        public bool IsTeacher 
        { 
            get 
            {
                try 
                {
                    return _userStore?.CurrentUser?.Role == 2;
                }
                catch 
                {
                    return false;
                }
            }
        }
        
        public bool IsStudent 
        { 
            get 
            {
                try 
                {
                    return _userStore?.CurrentUser?.Role == 1;
                }
                catch 
                {
                    return false;
                }
            }
        }

        public ObservableCollection<ResearchProject> Projects { get; }
        public ObservableCollection<ResearchTask> Tasks { get; }
        public ObservableCollection<ResearchApplication> Applications { get; }
        public ObservableCollection<ResearchSubmission> Submissions { get; }

        public ICommand LoadProjectsCommand { get; }
        public ICommand LoadTasksCommand { get; }
        public ICommand LoadApplicationsCommand { get; }
        public ICommand LoadSubmissionsCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand SubmitApplicationCommand { get; }
        public ICommand ViewTaskDetailsCommand { get; }
        public ICommand DownloadSubmissionCommand { get; }
        public ICommand ApproveApplicationCommand { get; }
        public ICommand RejectApplicationCommand { get; }

        private async Task LoadProjectsAsync()
        {
            try
            {
                IsBusy = true;
                ErrorMessage = null;
                Projects.Clear();

                // 检查登录状态
                if (!_userStore.IsLoggedIn)
                {
                    ErrorMessage = "请先登录";
                    return;
                }

                PagedResult<ResearchProject> result;
                if (IsTeacher)
                {
                    result = await _apiClient.GetTeacherProjectsAsync();
                }
                else
                {
                    result = await _apiClient.GetAvailableProjectsAsync();
                }

                if (result?.Records != null)
                {
                    foreach (var project in result.Records)
                    {
                        Projects.Add(project);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"加载项目失败: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadTasksAsync()
        {
            try
            {
                IsBusy = true;
                ErrorMessage = null;
                Tasks.Clear();

                if (!_userStore.IsLoggedIn)
                {
                    ErrorMessage = "请先登录";
                    return;
                }

                // 获取所有任务，不区分教师和学生
                var result = await _apiClient.GetAllTasksAsync();

                if (result?.Records != null)
                {
                    foreach (var task in result.Records)
                    {
                        Tasks.Add(task);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"加载任务失败: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadApplicationsAsync()
        {
            try
            {
                IsBusy = true;
                ErrorMessage = null;
                Applications.Clear();

                if (IsTeacher)
                {
                    var result = await _apiClient.GetPendingApplicationsAsync();
                    foreach (var application in result.Records)
                    {
                        Applications.Add(application);
                    }
                }
                else
                {
                    var applications = await _apiClient.GetMyApplicationsAsync();
                    foreach (var application in applications)
                    {
                        Applications.Add(application);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"加载申请失败: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadSubmissionsAsync()
        {
            try
            {
                IsBusy = true;
                ErrorMessage = null;
                Submissions.Clear();

                // 获取所有论文提交，不区分教师和学生
                var result = await _apiClient.GetAllSubmissionsAsync();
                foreach (var submission in result.Records)
                {
                    Submissions.Add(submission);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"加载论文失败: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SearchProjectsAsync()
        {
            try
            {
                IsBusy = true;
                ErrorMessage = null;
                Projects.Clear();

                PagedResult<ResearchProject> result;
                if (IsTeacher)
                {
                    result = await _apiClient.GetTeacherProjectsAsync(keyword: SearchKeyword);
                }
                else
                {
                    result = await _apiClient.GetAvailableProjectsAsync(keyword: SearchKeyword);
                }

                foreach (var project in result.Records)
                {
                    Projects.Add(project);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"搜索失败: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ReviewApplicationAsync(ResearchApplication application, string action)
        {
            if (application == null) return;

            var actionText = action == "approve" ? "通过" : "拒绝";
            var commentTextBox = new TextBox
            {
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
                MaxHeight = 200,
                PlaceholderText = $"请填写审核意见（可选）"
            };

            var dialog = new ContentDialog
            {
                Title = $"审核申请 - {actionText}",
                Content = commentTextBox,
                PrimaryButtonText = "确认",
                CloseButtonText = "取消"
            };
            
            var mainWindow = App.Services.GetRequiredService<MainWindow>();
            dialog.XamlRoot = mainWindow.Content.XamlRoot;

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    IsBusy = true;
                    ErrorMessage = null;
                    
                    var reviewRequest = new ReviewRequestDto
                    {
                        ApplicationId = application.Id,
                        Action = action,
                        ReviewComment = commentTextBox.Text
                    };

                    await _apiClient.ReviewApplicationAsync(reviewRequest);
                    
                    ErrorMessage = "审核操作成功！";
                    await LoadApplicationsAsync(); // 刷新列表
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"审核失败: {ex.Message}";
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async Task SubmitApplicationAsync(ResearchProject project)
        {
            if (project == null) 
            {
                ErrorMessage = "未选择项目";
                return;
            }

            var reasonTextBox = new TextBox
            {
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
                MaxHeight = 200,
                PlaceholderText = "请在此处填写您的申报理由..."
            };

            var dialog = new ContentDialog
            {
                Title = $"申报项目: {project.ProjectName}",
                Content = reasonTextBox,
                PrimaryButtonText = "确认提交",
                CloseButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary
            };

            var mainWindow = App.Services.GetRequiredService<MainWindow>();
            dialog.XamlRoot = mainWindow.Content.XamlRoot;

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var applicationReason = reasonTextBox.Text;
                if (string.IsNullOrWhiteSpace(applicationReason))
                {
                    ErrorMessage = "申报理由不能为空。";
                    return;
                }

                try
                {
                    IsBusy = true;
                    ErrorMessage = null;
                    System.Diagnostics.Debug.WriteLine($"🚀 开始提交申请，项目ID: {project.Id}，理由: {applicationReason}");

                    var applicationId = await _apiClient.SubmitApplicationAsync(project.Id, applicationReason);
                    
                    System.Diagnostics.Debug.WriteLine($"✅ 申请提交成功，申请ID: {applicationId}");
                    ErrorMessage = $"申请提交成功！申请ID: {applicationId}";
                    
                    // Refresh applications
                    await LoadApplicationsAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ 申请提交失败: {ex.Message}");
                    ErrorMessage = $"提交申请失败: {ex.Message}";
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async Task ViewTaskDetailsAsync(ResearchTask task)
        {
            if (task == null) return;
            
            var message = $"任务详情\n\n" +
                         $"任务标题: {task.Title}\n" +
                         $"所属项目: {task.ProjectName}\n" +
                         $"任务状态: {task.Status}\n" +
                         $"优先级: {task.Priority}\n" +
                         $"截止日期: {task.Deadline}\n" +
                         $"当前进度: {task.Progress}%\n\n" +
                         $"任务描述: \n{task.Description}\n\n" +
                         $"要求: \n{task.Requirements}";

            var dialog = new ContentDialog()
            {
                Title = "任务详情",
                Content = new ScrollViewer()
                {
                    Content = new TextBlock()
                    {
                        Text = message,
                        TextWrapping = TextWrapping.Wrap,
                        IsTextSelectionEnabled = true
                    },
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    MaxHeight = 500
                },
                CloseButtonText = "关闭"
            };

            var mainWindow = App.Services.GetRequiredService<MainWindow>();
            dialog.XamlRoot = mainWindow.Content.XamlRoot;

            await dialog.ShowAsync();
        }

        private async Task DownloadSubmissionAsync(ResearchSubmission submission)
        {
            if (submission == null) return;

            // TODO: 实现文件下载逻辑
            // 目前，后端API文档中没有提供文件下载接口。
            // 这里我们只显示一个提示。
            
            var dialog = new ContentDialog()
            {
                Title = "功能待实现",
                Content = "文件下载功能正在开发中，敬请期待。",
                CloseButtonText = "好的"
            };

            var mainWindow = App.Services.GetRequiredService<MainWindow>();
            dialog.XamlRoot = mainWindow.Content.XamlRoot;

            await dialog.ShowAsync();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Func<Task> execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public async void Execute(object parameter)
        {
            await _execute();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Func<T, Task> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Func<T, Task> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            try
            {
                if (parameter is T typedParameter)
                {
                    return _canExecute?.Invoke(typedParameter) ?? true;
                }
                return _canExecute == null; // 如果没有CanExecute函数且参数类型不匹配，则允许执行
            }
            catch
            {
                return false;
            }
        }

        public async void Execute(object parameter)
        {
            try
            {
                if (parameter is T typedParameter)
                {
                    await _execute(typedParameter);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"命令执行异常: {ex.Message}");
                // 可以在这里添加全局错误处理
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
} 