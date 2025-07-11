<script setup lang="ts">
import { ref, onMounted } from 'vue';
import axios from 'axios';

// --- 类型定义 ---
interface Formula {
  id: number;
  name: string;
}

interface ComparisonData {
  formulas: Formula[];
  comparisonPoints: Record<string, string[]>;
}

// --- 方剂对比状态 ---
const allFormulas = ref<{ id: number; name: string }[]>([]);
const selectedFormulaIds = ref<number[]>([]);
const comparisonData = ref<ComparisonData | null>(null);
const compareLoading = ref(false);
const compareError = ref<string | null>(null);

// --- API 调用方法 ---
const fetchAllFormulasForCompare = async () => {
  try {
    const response = await axios.get('/api/formula/page', { params: { page: 1, size: 200 } });
    if (response.data?.code === 20000) {
      allFormulas.value = response.data.data.records.map((f: Formula) => ({ id: f.id, name: f.name }));
    }
  } catch (err) {
    console.error("获取所有方剂列表失败:", err);
  }
};

const performComparison = async () => {
  if (selectedFormulaIds.value.length < 2) {
    compareError.value = '请至少选择两个方剂。';
    return;
  }
  compareLoading.value = true;
  compareError.value = null;
  comparisonData.value = null;
  try {
    const response = await axios.post('/api/formula/compare', selectedFormulaIds.value);
    if (response.data?.code === 20000) {
      comparisonData.value = response.data.data;
    } else {
      throw new Error(response.data.msg);
    }
  } catch (err: any) {
    compareError.value = err.message || '请求失败';
  } finally {
    compareLoading.value = false;
  }
};

onMounted(() => {
  fetchAllFormulasForCompare();
});
</script>

<template>
  <div class="content-section">
    <v-card class="compare-card glass-card" elevation="8">
      <v-card-title class="section-title">
        <v-icon class="section-icon">mdi-compare-horizontal</v-icon>
        方剂对比
      </v-card-title>
      <v-card-text class="compare-content">
        <v-select v-model="selectedFormulaIds" :items="allFormulas" item-title="name" item-value="id"
          label="选择两个或更多方剂进行对比" variant="outlined" multiple chips clearable prepend-inner-icon="mdi-pill"
          class="formula-select" density="comfortable" />
        <v-btn :loading="compareLoading" :disabled="selectedFormulaIds.length < 2" color="#B0D183" variant="flat"
          size="large" @click="performComparison" block class="compare-btn">
          <v-icon class="mr-2">mdi-compare</v-icon>
          开始对比分析
        </v-btn>
      </v-card-text>
    </v-card>

    <div v-if="compareLoading" class="loading-section">
      <v-progress-circular indeterminate color="#B0D183" size="60" width="6"></v-progress-circular>
      <p class="loading-text">对比分析中...</p>
    </div>

    <v-alert v-if="compareError" type="error" :text="compareError" variant="tonal" class="error-alert" />

    <v-card v-if="comparisonData" class="comparison-result glass-card" elevation="8">
      <v-card-title class="comparison-title">
        <v-icon class="mr-2">mdi-table</v-icon>
        对比结果
      </v-card-title>
      <v-card-text class="comparison-content">
        <div class="table-responsive-wrapper">
          <v-table class="comparison-table elegant-table">
            <thead>
              <tr class="table-header">
                <th class="comparison-label">对比项</th>
                <th v-for="formula in comparisonData.formulas" :key="formula.id" class="formula-header">
                  {{ formula.name }}
                </th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(values, key) in comparisonData.comparisonPoints" :key="key" class="comparison-row">
                <td class="comparison-key">{{ key }}</td>
                <td v-for="(value, index) in values" :key="index" class="comparison-value">
                  {{ value || '无' }}
                </td>
              </tr>
            </tbody>
          </v-table>
        </div>
      </v-card-text>
    </v-card>
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
.compare-card {
  padding: 1rem;
}
.compare-btn {
  font-weight: bold;
}
.table-responsive-wrapper {
  overflow-x: auto;
  width: 100%;
}
.comparison-table {
  width: 100%;
  table-layout: fixed;
}
:deep(.comparison-table th),
:deep(.comparison-table td) {
  white-space: nowrap;
  padding: 16px 20px !important;
}
:deep(.comparison-table .comparison-label) {
  width: 140px;
}
:deep(.comparison-table .formula-header) {
  min-width: 220px;
}
</style>