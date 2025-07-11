<script setup lang="ts">
import { ref } from 'vue';
import { Icon } from "@iconify/vue";

// 导入新的子组件
import FormulaSearchPage from './FormulaSearchPage.vue';
import FormulaRecommend from './FormulaRecommend.vue';
import FormulaComparison from './FormulaComparison.vue';
import HerbCombination from './HerbCombination.vue';

// --- 页面状态管理 ---
const activeTab = ref('search');
const tabs = [
  { value: 'search', title: '方剂检索', icon: 'mdi-magnify' },
  {
    value: 'recommend', title: '智能推荐',
    icon: {
      component: Icon,
      props: {
        icon: 'stash:thumb-up-light'
      }
    }
  },
  { value: 'compare', title: '方剂对比', icon: 'mdi-compare-horizontal' },
  { value: 'analysis', title: '配伍分析', icon: 'mdi-vector-combine' }
];
</script>

<template>
  <v-app class="app-container">
    <div class="decorative-bg"></div>

    <v-main>
      <v-card class="header-card elevation-12" flat>
        <div class="header-overlay"></div>
        <v-container>
          <v-row align="center" justify="center" class="header-content">
            <v-col cols="auto">
              <div class="header-icon-container">
                <v-icon size="56" class="header-icon">mdi-medical-bag</v-icon>
                <div class="icon-glow"></div>
              </div>
            </v-col>
            <v-col>
              <div class="header-text">
                <h1 class="main-title">中医方剂智能分析系统</h1>
                <p class="subtitle">传承千年智慧，探索方剂奥秘</p>
                <div class="decorative-line"></div>
              </div>
            </v-col>
          </v-row>
        </v-container>
      </v-card>

      <v-container class="main-container">
        <v-card class="navigation-card elevation-8" flat>
          <!--修改主色调-->
          <v-tabs v-model="activeTab" color="#B0D183" align-tabs="center" class="elegant-tabs" slider-color="#BBC23F">
            <v-tab v-for="tab in tabs" :key="tab.value" :value="tab.value" class="tab-item">
              <template v-if="typeof tab.icon === 'string'">
                <v-icon :icon="tab.icon" class="tab-icon"></v-icon>
              </template>
              <template v-else>
                <component :is="tab.icon.component" v-bind="tab.icon.props" class="tab-icon" />
              </template>
              <span class="tab-text">{{ tab.title }}</span>
            </v-tab>
          </v-tabs>
        </v-card>

        <v-window v-model="activeTab" class="content-window">
          <v-window-item value="search">
            <FormulaSearchPage />
          </v-window-item>
          <v-window-item value="recommend">
            <FormulaRecommend />
          </v-window-item>
          <v-window-item value="compare">
            <FormulaComparison />
          </v-window-item>
          <v-window-item value="analysis">
            <HerbCombination />
          </v-window-item>
        </v-window>
      </v-container>
    </v-main>
  </v-app>
</template>

<style scoped>
/* 此处保留所有通用样式，例如 .app-container, .header-card, .glass-card, .section-title, .elegant-table 等 */
/* --- 全局与布局 --- */
/* 修改主色调 */
.app-container {
  background: linear-gradient(135deg, #f5f7fa 0%, #C1CBAD 100%); /* 使用浅绿作为背景渐变色 */
  font-family: 'Helvetica Neue', Arial, 'Hiragino Sans GB', 'Microsoft YaHei', sans-serif;
}

.v-main {
  padding: 2rem 1rem;
}

.content-window {
  animation: fadeIn 0.8s ease-in-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* --- 精美的头部 --- */
.header-card {
  background: url('https://images.unsplash.com/photo-1558985250-2d89b5355ac2?q=80&w=2070&auto=format&fit=crop') center center/cover;
  border-radius: 10px !important;
  color: white;
  position: relative;
  overflow: hidden;
  margin-bottom: 2rem;
  padding: 2rem 0;
}

/* 修改主色调 */
.header-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  /* 使用深绿 (#BBC23F) 到 主色绿 (#B0D183) 的渐变 */
  background: linear-gradient(90deg, rgba(187, 194, 63, 0.9) 0%, rgba(176, 209, 131, 0.8) 100%);
}

.header-content {
  position: relative;
  z-index: 2;
}

.main-title {
  font-size: 2.5rem;
  font-weight: 700;
  letter-spacing: 1px;
  text-shadow: 0 2px 8px rgba(0, 0, 0, 0.5);
}

.subtitle {
  font-size: 1.1rem;
  opacity: 0.9;
  margin-top: 0.5rem;
  text-shadow: 0 1px 4px rgba(0, 0, 0, 0.4);
}

/* 修改主色调 */
.decorative-line {
  width: 80px;
  height: 4px;
  background-color: #BCA881; /* 改为棕色 */
  border-radius: 2px;
  margin-top: 1rem;
}

/* --- 导航标签 --- */
.navigation-card {
  background: rgba(255, 255, 255, 0.8);
  backdrop-filter: blur(10px);
  border-radius: 16px !important;
  margin-bottom: 1.5rem;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.6);
}

/* 修改主色调 */
.tab-item:hover {
  /* 使用主色调绿色的低透明度作为背景 */
  background-color: rgba(176, 209, 131, 0.1);
}

.tab-item:hover {
  background-color: rgba(63, 81, 181, 0.05);
}

.tab-icon {
  margin-right: 8px;
}
</style>