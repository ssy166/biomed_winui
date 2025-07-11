<script setup lang="ts">
import { ref } from 'vue';
import axios from 'axios';
import { Chart, registerables } from 'chart.js';
import { Bar } from 'vue-chartjs';

Chart.register(...registerables);

// --- 类型定义 ---
interface HerbCombination {
  herbName: string;
  combinationCount: number;
  combinationRatio: number;
}

// --- 配伍分析状态 ---
const searchHerb = ref<string>('红花');
const analysisResult = ref<HerbCombination[]>([]);
const analysisLoading = ref(false);
const analysisError = ref<string | null>(null);

// --- Chart.js 配置 ---
const chartData = ref({
  labels: [] as string[],
  datasets: [
    {
      label: '配伍次数',
      backgroundColor: 'rgba(176, 209, 131, 0.8)',
      borderColor: '#B0D183',
      borderWidth: 2,
      borderRadius: 8,
      data: [] as number[],
    },
  ],
});

const chartOptions = ref({
  responsive: true,
  maintainAspectRatio: false,
  indexAxis: 'y' as const,
  plugins: {
    legend: { display: false },
    title: {
      display: true,
      text: 'Top 10 配伍药材',
      font: { size: 16, weight: 'bold' },
      color: '#BBC23F',
      padding: 20
    },
  },
  scales: {
    x: {
      beginAtZero: true,
      title: { display: true, text: '配伍次数', font: { weight: 'bold' } },
      grid: { color: 'rgba(0,0,0,0.1)' }
    },
    y: {
      grid: { display: false }
    }
  },
});

// --- API 调用方法 ---
const performAnalysis = async () => {
  if (!searchHerb.value) {
    analysisError.value = '请输入药材名称。';
    return;
  }
  analysisLoading.value = true;
  analysisError.value = null;
  analysisResult.value = [];
  try {
    const response = await axios.get('/api/formula/analysis/herb-combinations', { params: { herbName: searchHerb.value } });
    if (response.data?.code === 20000) {
      analysisResult.value = response.data.data;
      if (analysisResult.value.length === 0) {
        analysisError.value = `未找到关于"${searchHerb.value}"的配伍数据。`;
        chartData.value = { labels: [], datasets: [{ data: [], label: '配伍次数', backgroundColor: 'rgba(176, 209, 131, 0.8)' }] };
      } else {
        const top10 = [...analysisResult.value].slice(0, 10).reverse();
        chartData.value.labels = top10.map(item => item.herbName);
        chartData.value.datasets[0].data = top10.map(item => item.combinationCount);
        chartOptions.value.plugins.title.text = `"${searchHerb.value}" 配伍药材Top 10`;
      }
    } else {
      throw new Error(response.data.msg);
    }
  } catch (err: any) {
    analysisError.value = err.message || '请求失败';
  } finally {
    analysisLoading.value = false;
  }
};
</script>

<template>
  <div class="content-section">
    <v-card class="analysis-card glass-card" elevation="8">
      <v-card-title class="section-title">
        <v-icon class="section-icon">mdi-vector-combine</v-icon>
        配伍分析
      </v-card-title>
      <v-card-text class="analysis-content">
        <v-text-field v-model="searchHerb" label="输入核心药材进行配伍分析" variant="outlined" clearable
          prepend-inner-icon="mdi-leaf" class="herb-input" density="comfortable" @keydown.enter="performAnalysis" />
        <v-btn :loading="analysisLoading" color="#B0D183" variant="flat" size="large" @click="performAnalysis" block
          class="analysis-btn">
          <v-icon class="mr-2">mdi-chart-bar</v-icon>
          开始配伍分析
        </v-btn>
      </v-card-text>
    </v-card>

    <div v-if="analysisLoading" class="loading-section">
      <v-progress-circular indeterminate color="#B0D183" size="60" width="6"></v-progress-circular>
      <p class="loading-text">配伍分析中...</p>
    </div>

    <v-alert v-if="analysisError" type="info" :text="analysisError" variant="tonal" class="error-alert" />

    <v-row v-if="analysisResult.length > 0" class="analysis-results">
      <v-col cols="12" lg="5">
        <v-card class="chart-card glass-card" elevation="8">
          <v-card-title class="chart-title">
            <v-icon class="mr-2">mdi-chart-bar</v-icon>
            配伍频次图表
          </v-card-title>
          <v-card-text class="chart-content">
            <div class="chart-container">
              <Bar :data="chartData" :options="chartOptions" />
            </div>
          </v-card-text>
        </v-card>
      </v-col>
      <v-col cols="12" lg="7">
        <v-card class="table-card glass-card" elevation="8">
          <v-card-title class="table-title">
            <v-icon class="mr-2">mdi-table</v-icon>
            配伍数据详情
          </v-card-title>
          <v-card-text class="table-content">
            <v-table class="analysis-table elegant-table">
              <thead>
                <tr class="table-header">
                  <th class="table-header-cell">配伍药材</th>
                  <th class="table-header-cell">配伍次数</th>
                  <th class="table-header-cell">配伍比例</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="item in analysisResult" :key="item.herbName" class="table-row">
                  <td class="herb-name">{{ item.herbName }}</td>
                  <td class="combination-count">{{ item.combinationCount }}</td>
                  <td class="combination-ratio">{{ (item.combinationRatio * 100).toFixed(0) }}%</td>
                </tr>
              </tbody>
            </v-table>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </div>
</template>

<style scoped>
/* 此处放置该组件特有的样式 */
.content-section {
    display: flex;
    flex-direction: column;
    gap: 1.5rem;
}
.glass-card {
  background: rgba(255, 255, 255, 0.7);
  backdrop-filter: blur(12px) saturate(180%);
  -webkit-backdrop-filter: blur(12px) saturate(180%);
  border: 1px solid rgba(255, 255, 255, 0.5);
  border-radius: 8px !important;
  box-shadow: 0 8px 32px 0 rgba(188, 194, 63, 0.2);
}
.section-title {
  font-size: 1.25rem;
  font-weight: 600;
  color: #BBC23F;
  display: flex;
  align-items: center;
  padding-bottom: 1rem;
}
.section-icon {
  margin-right: 0.75rem;
}
.elegant-table {
  background-color: transparent;
}
:deep(thead) {
  background-color: rgba(176, 209, 131, 0.1);
}
:deep(th) {
  font-weight: bold !important;
  color: #BBC23F !important;
}
:deep(tbody tr:hover) {
  background-color: rgba(176, 209, 131, 0.05) !important;
}
.loading-section {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  padding: 4rem 1rem;
  gap: 1rem;
  text-align: center;
}
.loading-text {
  font-size: 1.1rem;
  font-weight: 500;
  color: #BBC23F;
}
.error-alert {
  font-weight: 500;
}
.analysis-card {
  padding: 1rem;
}
.analysis-btn {
  font-weight: bold;
}
.chart-container {
  height: 450px;
  padding: 1rem;
}
.chart-card,
.table-card {
  height: 100%;
}
</style>