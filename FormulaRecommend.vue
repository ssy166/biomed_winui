<script setup lang="ts">
import { ref } from 'vue';
import axios from 'axios';
import { Icon } from "@iconify/vue";

// --- 类型定义 ---
interface Formula {
  composition: string;
  usage: string;
}

interface FormulaRecommendVO {
  formulaId: number;
  formulaName: string;
  score: number;
  recommendation: string;
  details?: Formula;
  loadingDetails?: boolean;
}

// --- 智能推荐状态 ---
const symptoms = ref<string[]>(['头痛', '恶寒']);
const recommendations = ref<FormulaRecommendVO[]>([]);
const recommendLoading = ref(false);
const recommendError = ref<string | null>(null);

const commonSymptoms = [
  '头痛', '发热', '恶寒', '咳嗽', '咽痛', '无汗',
  '胸痛', '心悸', '失眠', '急躁易怒', '食少', '便溏',
  '腹痛', '腹泻', '便秘', '恶心', '呕吐', '口干'
];

// --- API 调用方法 ---
const getRecommendations = async () => {
  if (symptoms.value.length === 0) {
    recommendError.value = '请输入至少一个症状。';
    return;
  }
  recommendLoading.value = true;
  recommendError.value = null;
  recommendations.value = [];
  try {
    const response = await axios.post('/api/formula/recommend', { symptoms: symptoms.value });
    if (response.data?.code === 20000) {
      recommendations.value = response.data.data.map((rec: any) => ({ ...rec, loadingDetails: true }));
      if (recommendations.value.length === 0) {
        recommendError.value = '未找到合适的方剂。';
      } else {
        fetchAllFormulaDetails();
      }
    } else {
      throw new Error(response.data.msg);
    }
  } catch (err: any) {
    recommendError.value = err.message || '请求失败';
  } finally {
    recommendLoading.value = false;
  }
};

const fetchAllFormulaDetails = async () => {
  const detailPromises = recommendations.value.map(rec =>
    axios.get(`/api/formula/${rec.formulaId}`)
      .then(res => {
        const targetRec = recommendations.value.find(r => r.formulaId === res.data.data.id);
        if (targetRec) {
          targetRec.details = res.data.data;
        }
      })
      .catch(err => console.error(`获取ID ${rec.formulaId} 详情失败:`, err))
      .finally(() => {
        const targetRec = recommendations.value.find(r => r.formulaId === rec.formulaId);
        if (targetRec) {
          targetRec.loadingDetails = false;
        }
      })
  );
  await Promise.all(detailPromises);
};

// --- 工具函数 ---
const getScoreColor = (score: number) => {
   if (score > 0.7) return '#B0D183'; // 高匹配度使用主色调绿色
  if (score > 0.5) return '#BBC23F'; // 中等匹配度使用深绿色
  return '#BCA881'; // 低匹配度使用棕色
};
</script>

<template>
  <div class="content-section">
    <v-card class="recommend-card glass-card" elevation="8">
      <v-card-title class="section-title">
        <component :is="Icon" icon="stash:thumb-up-light" class="section-icon" />
        智能推荐
      </v-card-title>
      <v-card-text class="recommend-content">
        <v-combobox v-model="symptoms" :items="commonSymptoms" label="输入症状后按回车键确认，可输入多个" variant="outlined" multiple
          chips clearable prepend-inner-icon="mdi-clipboard-text-search-outline" class="symptoms-input"
          density="comfortable" />
        <v-btn :loading="recommendLoading" color="#B0D183" variant="flat" size="large" @click="getRecommendations" block class="recommend-btn">
          <v-icon class="mr-2">mdi-brain</v-icon>
          获取方剂推荐
        </v-btn>
      </v-card-text>
    </v-card>

    <div v-if="recommendLoading" class="loading-section">
      <v-progress-circular indeterminate color="#B0D183" size="60" width="6"></v-progress-circular>
      <p class="loading-text">AI辨证分析中...</p>
    </div>

    <v-alert v-if="recommendError" type="warning" :text="recommendError" variant="tonal" class="error-alert" />

    <v-row v-if="recommendations.length > 0" class="recommendations-grid">
      <v-col v-for="rec in recommendations" :key="rec.formulaId" cols="12" lg="6">
        <v-card class="recommendation-card glass-card" elevation="6">
          <v-card-title class="recommendation-header">
            <div class="recommendation-name">{{ rec.formulaName }}</div>
            <v-chip :color="getScoreColor(rec.score)" label class="score-chip" size="small">
              匹配度: {{ (rec.score * 100).toFixed(1) }}%
            </v-chip>
          </v-card-title>
          <v-card-text class="recommendation-content">
            <v-progress-linear :model-value="rec.score * 100" :color="getScoreColor(rec.score)" height="8" rounded
              class="score-progress" />
            <div v-if="rec.loadingDetails" class="loading-details">
              <v-progress-circular indeterminate size="20" width="2" color="#B0D183"></v-progress-circular>
              <span class="loading-text-small">加载详情...</span>
            </div>
            <div v-if="rec.details && !rec.loadingDetails" class="formula-details">
              <div class="detail-item">
                <strong>组成:</strong> {{ rec.details.composition }}
              </div>
              <div class="detail-item">
                <strong>用法:</strong> {{ rec.details.usage }}
              </div>
            </div>
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
.recommend-card {
  padding: 1rem;
}
.recommend-btn {
  font-weight: bold;
}
.recommendation-card {
  transition: all 0.3s ease;
}
.recommendation-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.recommendation-name {
  font-weight: 600;
  color: #B0D183; /* 改为主色调绿色 */
}
.score-chip {
  font-weight: 600;
}
.score-progress {
  margin-bottom: 1rem;
}
.formula-details {
  font-size: 0.9rem;
  line-height: 1.6;
}
.detail-item {
  margin-bottom: 0.5rem;
}
.loading-details {
  display: flex;
  align-items: center;
  font-size: 0.9rem;
  color: #555;
  padding: 1rem 0;
}
.loading-text-small {
  margin-left: 0.5rem;
}
</style>