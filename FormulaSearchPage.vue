<script setup lang="ts">
import { ref, onMounted } from 'vue';
import axios from 'axios';
import FormulaCardWrapper from '@/components/common/Pin/FormulaCardWrapper.vue'
// --- 类型定义 ---
interface Formula {
  id: number;
  name: string;
  alias: string | null;
  source: string;
  dynasty: string;
  author: string;
  composition: string;
  preparation: string;
  usage: string;
  dosageForm: string;
  functionEffect: string;
  mainTreatment: string;
}

interface PaginatedFormulas {
  records: Formula[];
  total: number;
}

// --- 方剂检索状态 ---
const formulaList = ref<Formula[]>([]);
const totalItems = ref(0);
const searchLoading = ref(true);
const searchOptions = ref({
  page: 1,
  itemsPerPage: 12,
});
const searchParams = ref({
  keyword: '',
  source: '',
});

// --- API 调用方法 ---
const fetchFormulas = async () => {
  searchLoading.value = true;
  try {
    const params = {
      page: searchOptions.value.page,
      size: searchOptions.value.itemsPerPage,
      keyword: searchParams.value.keyword,
      source: searchParams.value.source
    };
    const response = await axios.get('/api/formula/page', { params });
    if (response.data?.code === 20000) {
      const result = response.data.data as PaginatedFormulas;
      formulaList.value = result.records;
      totalItems.value = result.total;
    } else {
      throw new Error(response.data.msg);
    }
  } catch (err) {
    console.error('获取方剂数据失败:', err);
  } finally {
    searchLoading.value = false;
  }
};

const performSearch = () => {
  searchOptions.value.page = 1;
  fetchFormulas();
};

const resetSearch = () => {
  searchParams.value = { keyword: '', source: '' };
  performSearch();
};

// --- 生命周期 ---
onMounted(() => {
  fetchFormulas();
});
</script>

<template>
  <div class="content-section">
    <v-card class="search-card glass-card" elevation="8">
      <v-card-title class="section-title">
        <v-icon class="section-icon">mdi-magnify</v-icon>
        查找方剂
      </v-card-title>
      <v-card-text class="search-content">
        <v-row align="center" class="search-row">
          <v-col cols="12" md="5">
            <v-text-field v-model="searchParams.keyword" label="搜索方剂名称、主治..." prepend-inner-icon="mdi-magnify"
              variant="outlined" density="comfortable" clearable hide-details class="search-input"
              @keydown.enter="performSearch" />
          </v-col>
          <v-col cols="12" md="4">
            <v-text-field v-model="searchParams.source" label="按出处筛选，如: 伤寒论"
              prepend-inner-icon="mdi-book-open-variant" variant="outlined" density="comfortable" clearable hide-details
              class="search-input" @keydown.enter="performSearch" />
          </v-col>
          <v-col cols="12" md="3" class="text-md-right">
            <div class="button-group">
              <!--修改主色调---->
              <v-btn @click="resetSearch" variant="outlined" color="#BBC23F" class="action-btn reset-btn">
                <v-icon class="mr-1">mdi-refresh</v-icon>
                重置
              </v-btn>
              <v-btn @click="performSearch" color="#B0D183" variant="flat" class="action-btn search-btn">
                <v-icon class="mr-1">mdi-magnify</v-icon>
                搜索
              </v-btn>
            </div>
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>

    <v-row v-if="!searchLoading">
      <v-col v-for="item in formulaList" :key="item.id" cols="12" md="6" lg="4">
        <FormulaCardWrapper :formula="item">
          <v-card class="formula-card glass-card" elevation="4" hover>
            <v-card-title class="formula-title">
              {{ item.name }}
              <div v-if="item.alias" class="formula-alias">({{ item.alias }})</div>
            </v-card-title>
            <v-card-subtitle class="formula-subtitle">
              <v-icon size="x-small" class="mr-1">mdi-book-open-variant</v-icon>
              {{ item.source }} ({{ item.dynasty }})
            </v-card-subtitle>
            <v-divider class="my-2"></v-divider>
            <v-card-text class="formula-content">
              <div class="content-block">
                <div class="content-label">功用</div>
                <p class="content-text">{{ item.functionEffect }}</p>
              </div>
              <div class="content-block mt-3">
                <div class="content-label">主治</div>
                <p class="content-text">{{ item.mainTreatment }}</p>
              </div>
            </v-card-text>
          </v-card>
        </FormulaCardWrapper>
      </v-col>
    </v-row>
    <div v-if="searchLoading" class="loading-container">
      <!--修改主色调-->
      <v-progress-circular indeterminate color="#B0D183" size="50"></v-progress-circular>
    </div>
    <v-pagination v-if="totalItems > searchOptions.itemsPerPage" v-model="searchOptions.page"
      :length="Math.ceil(totalItems / searchOptions.itemsPerPage)" @update:modelValue="fetchFormulas"
      class="mt-4"></v-pagination>
  </div>
</template>

<style scoped>
/* 此处放置该组件特有的样式 */
.content-section {
    display: flex;
    flex-direction: column;
    gap: 1.5rem;
}
/*修改主色调*/
.glass-card {
  background: rgba(255, 255, 255, 0.7);
  backdrop-filter: blur(12px) saturate(180%);
  -webkit-backdrop-filter: blur(12px) saturate(180%);
  border: 1px solid rgba(255, 255, 255, 0.5);
  border-radius: 8px !important;
  transition: all 0.3s ease;
  box-shadow: 0 8px 32px 0 rgba(188, 194, 63, 0.2);
}
.glass-card:hover {
 transform: translateY(-5px);
  box-shadow: 0 12px 40px 0 rgba(188, 194, 63, 0.3);
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
.formula-card {
  display: flex;
  flex-direction: column;
  height: 100%;
}
.formula-title {
  color: #B0D183;
  font-weight: bold;
}
.formula-alias {
  font-size: 0.9rem;
  color: #78909C;
  margin-left: 8px;
}
.formula-subtitle {
  color: #546E7A;
}
.formula-content {
  flex-grow: 1;
}
.content-block .content-label {
  font-weight: 600;
  color: #455A64;
  margin-bottom: 4px;
}
.content-text {
  color: #37474F;
  line-height: 1.6;
}
.loading-container {
    display: flex;
    justify-content: center;
    align-items: center;
    min-height: 300px;
}
.action-btn {
  font-weight: bold;
  letter-spacing: 0.5px;
}
.search-btn {
  box-shadow: 0 2px 5px rgba(176, 209, 131, 0.4);
}
.button-group {
    display: flex;
    gap: 0.5rem;
    justify-content: flex-end;
}
</style>