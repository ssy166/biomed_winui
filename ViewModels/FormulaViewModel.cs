using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using biomed.Models;
using biomed.Services;

namespace biomed.ViewModels
{
    public partial class FormulaViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;

        // === 通用属性 ===
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsError))]
        private string _errorMessage;

        [ObservableProperty]
        private bool _isBusy;

        public bool IsError => !string.IsNullOrEmpty(ErrorMessage);

        // === 方剂搜索相关 ===
        [ObservableProperty]
        private ObservableCollection<Formula> _formulas = new();

        [ObservableProperty]
        private string _searchKeyword = "";

        [ObservableProperty]
        private string _sourceFilter = "";

        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private int _totalPages = 1;

        [ObservableProperty]
        private long _totalItems = 0;

        private const int PageSize = 12;

        // === 方剂推荐相关 ===
        [ObservableProperty]
        private ObservableCollection<string> _symptoms = new();

        [ObservableProperty]
        private ObservableCollection<FormulaRecommendation> _recommendations = new();

        [ObservableProperty]
        private string _newSymptom = "";

        [ObservableProperty]
        private bool _isRecommending;

        // 常见症状列表
        public List<string> CommonSymptoms { get; } = new List<string>
        {
            "头痛", "发热", "恶寒", "咳嗽", "咽痛", "无汗",
            "胸痛", "心悸", "失眠", "急躁易怒", "食少", "便溏",
            "腹痛", "腹泻", "便秘", "恶心", "呕吐", "口干",
            "口苦", "眩晕", "耳鸣", "盗汗", "自汗", "畏寒"
        };

        // === 中药配伍分析相关 ===
        [ObservableProperty]
        private string _herbName = "红花";

        [ObservableProperty]
        private ObservableCollection<HerbCombination> _herbCombinations = new();

        [ObservableProperty]
        private bool _isAnalyzing;

        // === 方剂比较相关 ===
        [ObservableProperty]
        private ObservableCollection<Formula> _allFormulasForCompare = new();

        [ObservableProperty]
        private ObservableCollection<Formula> _selectedFormulasForCompare = new();

        [ObservableProperty]
        private FormulaComparison _comparisonResult;

        [ObservableProperty]
        private bool _isComparing;

        public FormulaViewModel()
        {
            _apiClient = App.GetService<ApiClient>();
        }

        // === 方剂搜索功能 ===
        [RelayCommand]
        private async Task SearchFormulasAsync()
        {
            CurrentPage = 1;
            await LoadFormulasAsync();
        }

        [RelayCommand]
        private async Task ResetSearchAsync()
        {
            SearchKeyword = "";
            SourceFilter = "";
            CurrentPage = 1;
            await LoadFormulasAsync();
        }

        [RelayCommand]
        private async Task LoadFormulasAsync()
        {
            IsBusy = true;
            ErrorMessage = null;
            
            try
            {
                var result = await _apiClient.GetFormulasAsync(
                    page: CurrentPage,
                    size: PageSize,
                    keyword: string.IsNullOrWhiteSpace(SearchKeyword) ? null : SearchKeyword,
                    source: string.IsNullOrWhiteSpace(SourceFilter) ? null : SourceFilter
                );

                Formulas.Clear();
                foreach (var formula in result.Records)
                {
                    Formulas.Add(formula);
                }

                TotalItems = result.Total;
                TotalPages = result.Pages;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"加载方剂列表失败: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task NextPageAsync()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                await LoadFormulasAsync();
            }
        }

        [RelayCommand]
        private async Task PreviousPageAsync()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await LoadFormulasAsync();
            }
        }

        // === 方剂推荐功能 ===
        [RelayCommand]
        private void AddSymptom()
        {
            if (!string.IsNullOrWhiteSpace(NewSymptom) && !Symptoms.Contains(NewSymptom))
            {
                Symptoms.Add(NewSymptom);
                NewSymptom = "";
            }
        }

        [RelayCommand]
        private void AddCommonSymptom(string symptom)
        {
            if (!string.IsNullOrWhiteSpace(symptom) && !Symptoms.Contains(symptom))
            {
                Symptoms.Add(symptom);
            }
        }

        [RelayCommand]
        private void RemoveSymptom(string symptom)
        {
            Symptoms.Remove(symptom);
        }

        [RelayCommand]
        private async Task GetRecommendationsAsync()
        {
            if (Symptoms.Count == 0)
            {
                ErrorMessage = "请添加至少一个症状";
                return;
            }

            IsRecommending = true;
            ErrorMessage = null;
            Recommendations.Clear();

            try
            {
                var recommendations = await _apiClient.GetFormulaRecommendationsAsync(Symptoms.ToList());
                
                foreach (var rec in recommendations)
                {
                    Recommendations.Add(rec);
                }

                if (Recommendations.Count == 0)
                {
                    ErrorMessage = "未找到合适的方剂推荐";
                }
                else
                {
                    // 异步加载每个推荐方剂的详情
                    _ = Task.Run(async () => await LoadRecommendationDetailsAsync());
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"获取推荐失败: {ex.Message}";
            }
            finally
            {
                IsRecommending = false;
            }
        }

        private async Task LoadRecommendationDetailsAsync()
        {
            var tasks = Recommendations.Select(async rec =>
            {
                // 在UI线程中设置加载状态
                App.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    rec.IsLoadingDetails = true;
                });

                try
                {
                    var details = await _apiClient.GetFormulaDetailAsync(rec.FormulaId);
                    
                    // 在UI线程中更新结果
                    App.MainWindow.DispatcherQueue.TryEnqueue(() =>
                    {
                        rec.Details = details;
                        rec.IsLoadingDetails = false;
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"加载方剂详情失败 {rec.FormulaId}: {ex.Message}");
                    
                    // 在UI线程中设置加载完成
                    App.MainWindow.DispatcherQueue.TryEnqueue(() =>
                    {
                        rec.IsLoadingDetails = false;
                    });
                }
            });

            await Task.WhenAll(tasks);
        }

        // === 中药配伍分析功能 ===
        [RelayCommand]
        private async Task AnalyzeHerbCombinationsAsync()
        {
            if (string.IsNullOrWhiteSpace(HerbName))
            {
                ErrorMessage = "请输入中药名称";
                return;
            }

            IsAnalyzing = true;
            ErrorMessage = null;
            HerbCombinations.Clear();

            try
            {
                var combinations = await _apiClient.GetHerbCombinationsAsync(HerbName);
                
                foreach (var combination in combinations)
                {
                    HerbCombinations.Add(combination);
                }

                if (HerbCombinations.Count == 0)
                {
                    ErrorMessage = $"未找到关于\"{HerbName}\"的配伍数据";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"配伍分析失败: {ex.Message}";
            }
            finally
            {
                IsAnalyzing = false;
            }
        }

        // === 方剂比较功能 ===
        [RelayCommand]
        private async Task LoadAllFormulasForCompareAsync()
        {
            try
            {
                // 加载所有方剂用于比较选择
                var result = await _apiClient.GetFormulasAsync(page: 1, size: 200);
                AllFormulasForCompare.Clear();
                foreach (var formula in result.Records)
                {
                    AllFormulasForCompare.Add(formula);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"加载方剂列表失败: {ex.Message}";
            }
        }

        [RelayCommand]
        private void AddFormulaToCompare(Formula formula)
        {
            if (formula != null && !SelectedFormulasForCompare.Any(f => f.Id == formula.Id))
            {
                SelectedFormulasForCompare.Add(formula);
            }
        }

        [RelayCommand]
        private void RemoveFormulaFromCompare(Formula formula)
        {
            SelectedFormulasForCompare.Remove(formula);
        }

        [RelayCommand]
        private async Task CompareFormulasAsync()
        {
            if (SelectedFormulasForCompare.Count < 2)
            {
                ErrorMessage = "请选择至少两个方剂进行比较";
                return;
            }

            IsComparing = true;
            ErrorMessage = null;
            ComparisonResult = null;

            try
            {
                var formulaIds = SelectedFormulasForCompare.Select(f => f.Id).ToList();
                ComparisonResult = await _apiClient.CompareFormulasAsync(formulaIds);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"方剂比较失败: {ex.Message}";
            }
            finally
            {
                IsComparing = false;
            }
        }

        // === 初始化方法 ===
        public async Task InitializeAsync()
        {
            await LoadFormulasAsync();
            await LoadAllFormulasForCompareAsync();
        }
    }
} 