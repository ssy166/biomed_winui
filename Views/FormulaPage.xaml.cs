using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using biomed.ViewModels;
using biomed.Models;
using biomed.Services;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace biomed.Views
{
    public sealed partial class FormulaPage : Page
    {
        public FormulaViewModel ViewModel { get; }

        public FormulaPage()
        {
            this.InitializeComponent();
            ViewModel = App.GetService<FormulaViewModel>();
            this.DataContext = ViewModel;
            this.Loaded += FormulaPage_Loaded;
        }

        private async void FormulaPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.InitializeAsync();
        }

        // === 方剂搜索相关事件 ===
        private void SearchKeyword_EnterPressed(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            ViewModel.SearchFormulasCommand.Execute(null);
            args.Handled = true;
        }

        private void SourceFilter_EnterPressed(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            ViewModel.SearchFormulasCommand.Execute(null);
            args.Handled = true;
        }

        private void Formula_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is Formula formula)
            {
                ShowFormulaDetailDialog(formula);
            }
        }

        // === 智能推荐相关事件 ===
        private void NewSymptom_EnterPressed(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            ViewModel.AddSymptomCommand.Execute(null);
            args.Handled = true;
        }

        // === 方剂对比相关事件 ===
        private void FormulaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox?.SelectedItem is Formula selectedFormula)
            {
                ViewModel.AddFormulaToCompareCommand.Execute(selectedFormula);
                comboBox.SelectedItem = null; // 清除选择，允许重复选择
            }
        }

        // === 配伍分析相关事件 ===
        private void HerbName_EnterPressed(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            ViewModel.AnalyzeHerbCombinationsCommand.Execute(null);
            args.Handled = true;
        }

        // === 辅助方法 ===
        private async void ShowFormulaDetailDialog(Formula formula)
        {
            try
            {
                // 获取详细信息
                var apiClient = App.GetService<ApiClient>();
                var detail = await apiClient.GetFormulaDetailAsync(formula.Id);

                // 创建详情对话框
                var dialog = new ContentDialog
                {
                    Title = detail.Name,
                    CloseButtonText = "关闭",
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = this.XamlRoot,
                    Content = CreateFormulaDetailContent(detail)
                };

                _ = await dialog.ShowAsync();
            }
            catch (System.Exception ex)
            {
                var errorDialog = new ContentDialog
                {
                    Title = "错误",
                    Content = $"获取方剂详情失败: {ex.Message}",
                    CloseButtonText = "确定",
                    XamlRoot = this.XamlRoot
                };
                _ = await errorDialog.ShowAsync();
            }
        }

        private ScrollViewer CreateFormulaDetailContent(Formula formula)
        {
            var scrollViewer = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                MaxHeight = 500
            };

            var stackPanel = new StackPanel
            {
                Spacing = 16,
                Padding = new Thickness(24)
            };

            // 基本信息
            if (!string.IsNullOrEmpty(formula.Alias))
            {
                AddDetailSection(stackPanel, "别名", formula.Alias);
            }
            AddDetailSection(stackPanel, "来源", $"{formula.Source} ({formula.Dynasty})");
            if (!string.IsNullOrEmpty(formula.Author))
            {
                AddDetailSection(stackPanel, "作者", formula.Author);
            }

            // 组成和制法
            AddDetailSection(stackPanel, "组成", formula.Composition);
            if (!string.IsNullOrEmpty(formula.Preparation))
            {
                AddDetailSection(stackPanel, "制法", formula.Preparation);
            }
            if (!string.IsNullOrEmpty(formula.Usage))
            {
                AddDetailSection(stackPanel, "用法", formula.Usage);
            }
            if (!string.IsNullOrEmpty(formula.DosageForm))
            {
                AddDetailSection(stackPanel, "剂型", formula.DosageForm);
            }

            // 功效和主治
            AddDetailSection(stackPanel, "功效", formula.FunctionEffect);
            AddDetailSection(stackPanel, "主治", formula.MainTreatment);

            // 临床应用和现代研究
            if (!string.IsNullOrEmpty(formula.ClinicalApplication))
            {
                AddDetailSection(stackPanel, "临床应用", formula.ClinicalApplication);
            }
            if (!string.IsNullOrEmpty(formula.PharmacologicalAction))
            {
                AddDetailSection(stackPanel, "药理作用", formula.PharmacologicalAction);
            }
            if (!string.IsNullOrEmpty(formula.ModernResearch))
            {
                AddDetailSection(stackPanel, "现代研究", formula.ModernResearch);
            }

            // 注意事项
            if (!string.IsNullOrEmpty(formula.Contraindication))
            {
                AddDetailSection(stackPanel, "禁忌", formula.Contraindication);
            }
            if (!string.IsNullOrEmpty(formula.Caution))
            {
                AddDetailSection(stackPanel, "注意事项", formula.Caution);
            }
            if (!string.IsNullOrEmpty(formula.Remarks))
            {
                AddDetailSection(stackPanel, "备注", formula.Remarks);
            }

            scrollViewer.Content = stackPanel;
            return scrollViewer;
        }

        private void AddDetailSection(StackPanel parent, string title, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return;

            var titleBlock = new TextBlock
            {
                Text = title + ":",
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                Style = (Style)Application.Current.Resources["BaseTextBlockStyle"]
            };

            var contentBlock = new TextBlock
            {
                Text = content,
                TextWrapping = TextWrapping.Wrap,
                Style = (Style)Application.Current.Resources["BodyTextBlockStyle"],
                Margin = new Thickness(0, 4, 0, 0)
            };

            parent.Children.Add(titleBlock);
            parent.Children.Add(contentBlock);
        }

        // 处理对比结果的表格显示
        private void UpdateComparisonGrid()
        {
            if (ViewModel.ComparisonResult == null || ComparisonGrid == null)
                return;

            ComparisonGrid.Children.Clear();
            ComparisonGrid.RowDefinitions.Clear();
            ComparisonGrid.ColumnDefinitions.Clear();

            var comparison = ViewModel.ComparisonResult;
            var formulas = comparison.Formulas;
            var points = comparison.ComparisonPoints;

            // 创建列定义
            ComparisonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) }); // 对比项列
            foreach (var formula in formulas)
            {
                ComparisonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            // 创建表头
            ComparisonGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            
            var headerCell = new Border
            {
                Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["AccentFillColorDefaultBrush"],
                Padding = new Thickness(12, 8, 12, 8),
                Child = new TextBlock
                {
                    Text = "对比项",
                    FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                    Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["TextOnAccentFillColorPrimaryBrush"]
                }
            };
            Grid.SetColumn(headerCell, 0);
            Grid.SetRow(headerCell, 0);
            ComparisonGrid.Children.Add(headerCell);

            for (int i = 0; i < formulas.Count; i++)
            {
                var formulaHeaderCell = new Border
                {
                    Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["AccentFillColorDefaultBrush"],
                    Padding = new Thickness(12, 8, 12, 8),
                    Child = new TextBlock
                    {
                        Text = formulas[i].Name,
                        FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                        Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["TextOnAccentFillColorPrimaryBrush"],
                        TextWrapping = TextWrapping.Wrap
                    }
                };
                Grid.SetColumn(formulaHeaderCell, i + 1);
                Grid.SetRow(formulaHeaderCell, 0);
                ComparisonGrid.Children.Add(formulaHeaderCell);
            }

            // 创建数据行
            int rowIndex = 1;
            foreach (var point in points)
            {
                ComparisonGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                // 对比项标题
                var pointCell = new Border
                {
                    Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["CardBackgroundFillColorDefaultBrush"],
                    BorderBrush = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["CardStrokeColorDefaultBrush"],
                    BorderThickness = new Thickness(0, 1, 1, 1),
                    Padding = new Thickness(12, 8, 12, 8),
                    Child = new TextBlock
                    {
                        Text = point.Key,
                        FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
                    }
                };
                Grid.SetColumn(pointCell, 0);
                Grid.SetRow(pointCell, rowIndex);
                ComparisonGrid.Children.Add(pointCell);

                // 对比内容
                for (int i = 0; i < point.Value.Count && i < formulas.Count; i++)
                {
                    var valueCell = new Border
                    {
                        Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["CardBackgroundFillColorDefaultBrush"],
                        BorderBrush = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["CardStrokeColorDefaultBrush"],
                        BorderThickness = new Thickness(0, 1, 1, 1),
                        Padding = new Thickness(12, 8, 12, 8),
                        Child = new TextBlock
                        {
                            Text = point.Value[i] ?? "无",
                            TextWrapping = TextWrapping.Wrap
                        }
                    };
                    Grid.SetColumn(valueCell, i + 1);
                    Grid.SetRow(valueCell, rowIndex);
                    ComparisonGrid.Children.Add(valueCell);
                }

                rowIndex++;
            }
        }

        // 监听对比结果变化
        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.ComparisonResult))
            {
                UpdateComparisonGrid();
            }
        }

        protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        protected override void OnNavigatedFrom(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            base.OnNavigatedFrom(e);
        }
    }
} 