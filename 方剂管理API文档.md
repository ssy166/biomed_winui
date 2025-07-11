# 方剂管理模块 API 接口文档

本文档详细说明了生物医学信息系统中方剂管理模块的后端API接口。

## 1. 统一响应格式

所有API的响应都遵循统一的结构：

```json
{
  "code": "Integer",
  "msg": "String", 
  "data": "Object | Array | null"
}
```

### 状态码说明

- `20000`: 操作成功
- `20040`: 查询失败
- `20060`: 参数校验失败
- `40100`: 未授权
- `40300`: 权限不足
- `50000`: 系统错误

---

## 2. 方剂管理接口 (`/formula`)

### 2.1 分页查询方剂列表

- **接口地址:** `GET /formula/page`
- **接口描述:** 分页查询方剂列表，支持关键词搜索、分类筛选和来源筛选
- **请求参数:**

| 参数名 | 类型 | 必填 | 默认值 | 说明 |
|--------|------|------|--------|------|
| page | Integer | 否 | 1 | 页码 |
| size | Integer | 否 | 10 | 每页数量 |
| keyword | String | 否 | - | 关键词（匹配方剂名称、别名等） |
| categoryId | Integer | 否 | - | 方剂分类ID |
| source | String | 否 | - | 方剂来源（如：伤寒论、金匮要略等） |

- **成功响应 (`code: 20000`):**
```json
{
  "code": 20000,
  "msg": "操作成功",
  "data": {
    "records": [
      {
        "id": 1,
        "name": "桂枝汤",
        "alias": "桂枝加芍药汤",
        "source": "伤寒论",
        "dynasty": "汉",
        "functionEffect": "解肌发表，调和营卫",
        "mainTreatment": "外感风寒表虚证"
      }
    ],
    "total": 100,
    "size": 10,
    "current": 1,
    "pages": 10
  }
}
```

### 2.2 获取方剂详情

- **接口地址:** `GET /formula/{id}`
- **接口描述:** 根据方剂ID获取详细信息
- **路径参数:**

| 参数名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| id | Long | 是 | 方剂ID |

- **成功响应 (`code: 20000`):**
```json
{
  "code": 20000,
  "msg": "操作成功",
  "data": {
    "id": 1,
    "name": "桂枝汤",
    "alias": "桂枝加芍药汤",
    "source": "伤寒论",
    "dynasty": "汉",
    "author": "张仲景",
    "categoryId": 1,
    "composition": "桂枝三两，芍药三两，甘草二两（炙），生姜三两（切），大枣十二枚（擘）",
    "preparation": "上五味，㕮咀三味，以水七升，微火煮取三升，去滓",
    "usage": "适寒温，服一升。服已须臾，啜热稀粥一升余，以助药力",
    "dosageForm": "汤剂",
    "functionEffect": "解肌发表，调和营卫",
    "mainTreatment": "外感风寒表虚证。发热，汗出，恶风，鼻塞，苔薄白，脉浮缓或浮弱",
    "clinicalApplication": "感冒、流行性感冒、原因不明的低热、产后或病后低热、妊娠呕吐、多汗症等",
    "pharmacologicalAction": "解热、镇痛、抗炎、调节免疫、抗过敏等作用",
    "contraindication": "表实无汗者忌用；温病初起，或风热感冒不宜使用",
    "caution": "服药期间禁食生冷、油腻、辛辣食物",
    "modernResearch": "现代药理研究表明，桂枝汤具有调节体温、增强机体免疫功能等作用",
    "remarks": "桂枝汤为仲景群方之魁，变化极多"
  }
}
```

- **失败响应 (`code: 20040`):**
```json
{
  "code": 20040,
  "msg": "方剂不存在",
  "data": null
}
```

### 2.3 基于症状推荐方剂

- **接口地址:** `POST /formula/recommend`
- **接口描述:** 根据输入的症状分析推荐合适的方剂
- **请求体 (`Content-Type: application/json`):**
```json
{
  "symptoms": ["发热", "恶风", "汗出", "脉浮缓"]
}
```

- **成功响应 (`code: 20000`):**
```json
{
  "code": 20000,
  "msg": "操作成功",
  "data": [
    {
      "formulaId": 1,
      "formulaName": "桂枝汤",
      "score": 0.95,
      "matchedSymptoms": ["发热", "恶风", "汗出"],
      "recommendation": "高度匹配，建议考虑使用桂枝汤治疗表虚感冒"
    },
    {
      "formulaId": 2,
      "formulaName": "桂枝加黄芪汤",
      "score": 0.82,
      "matchedSymptoms": ["发热", "汗出"],
      "recommendation": "部分匹配，适用于表虚汗出较重者"
    }
  ]
}
```

### 2.4 分析中药组合

- **接口地址:** `GET /formula/analysis/herb-combinations`
- **接口描述:** 分析指定中药在方剂中的配伍规律
- **请求参数:**

| 参数名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| herbName | String | 是 | 中药名称 |

- **成功响应 (`code: 20000`):**
```json
{
  "code": 20000,
  "msg": "操作成功",
  "data": [
    {
      "herbName": "芍药",
      "combinationCount": 156,
      "combinationRatio": 0.85
    },
    {
      "herbName": "甘草",
      "combinationCount": 134,
      "combinationRatio": 0.73
    },
    {
      "herbName": "生姜",
      "combinationCount": 98,
      "combinationRatio": 0.53
    }
  ]
}
```

### 2.5 方剂比较

- **接口地址:** `POST /formula/compare`
- **接口描述:** 比较多个方剂的异同点
- **请求体 (`Content-Type: application/json`):**
```json
[1, 2, 3]
```

- **成功响应 (`code: 20000`):**
```json
{
  "code": 20000,
  "msg": "操作成功",
  "data": {
    "formulas": [
      {
        "id": 1,
        "name": "桂枝汤",
        "composition": "桂枝三两，芍药三两，甘草二两（炙），生姜三两（切），大枣十二枚（擘）",
        "functionEffect": "解肌发表，调和营卫"
      },
      {
        "id": 2,
        "name": "桂枝加黄芪汤",
        "composition": "桂枝汤加黄芪一两半",
        "functionEffect": "解肌发表，益气固表"
      }
    ],
    "comparisonPoints": {
      "共同药物": ["桂枝", "芍药", "甘草", "生姜", "大枣"],
      "不同药物": ["黄芪（桂枝加黄芪汤特有）"],
      "功效差异": ["桂枝汤：调和营卫", "桂枝加黄芪汤：增加益气固表"],
      "适应症差异": ["桂枝汤：表虚感冒", "桂枝加黄芪汤：表虚汗出较重"]
    }
  }
}
```

---

## 3. 数据模型说明

### 3.1 FormulaVO（方剂列表项）

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | Long | 方剂ID |
| name | String | 方剂名称 |
| alias | String | 别名 |
| source | String | 来源典籍 |
| dynasty | String | 朝代 |
| functionEffect | String | 功效 |
| mainTreatment | String | 主治 |

### 3.2 FormulaDetailVO（方剂详情）

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | Long | 方剂ID |
| name | String | 方剂名称 |
| alias | String | 别名 |
| source | String | 来源典籍 |
| dynasty | String | 朝代 |
| author | String | 作者 |
| categoryId | Integer | 分类ID |
| composition | String | 组成 |
| preparation | String | 制法 |
| usage | String | 用法 |
| dosageForm | String | 剂型 |
| functionEffect | String | 功效 |
| mainTreatment | String | 主治 |
| clinicalApplication | String | 临床应用 |
| pharmacologicalAction | String | 药理作用 |
| contraindication | String | 禁忌 |
| caution | String | 注意事项 |
| modernResearch | String | 现代研究 |
| remarks | String | 备注 |

### 3.3 FormulaRecommendVO（方剂推荐）

| 字段名 | 类型 | 说明 |
|--------|------|------|
| formulaId | Long | 方剂ID |
| formulaName | String | 方剂名称 |
| score | Double | 匹配度评分（0-1） |
| matchedSymptoms | List<String> | 匹配的症状 |
| recommendation | String | 推荐理由 |

### 3.4 HerbCombinationVO（中药组合）

| 字段名 | 类型 | 说明 |
|--------|------|------|
| herbName | String | 中药名称 |
| combinationCount | Long | 配伍次数 |
| combinationRatio | Double | 配伍比例 |

### 3.5 FormulaComparisonVO（方剂比较）

| 字段名 | 类型 | 说明 |
|--------|------|------|
| formulas | List<FormulaDetailVO> | 比较的方剂列表 |
| comparisonPoints | Map<String, List<String>> | 比较要点 |

---

## 4. 错误处理

### 常见错误响应

- **参数验证失败 (`code: 20060`)**
```json
{
  "code": 20060,
  "msg": "请求参数不能为空",
  "data": null
}
```

- **资源不存在 (`code: 20040`)**
```json
{
  "code": 20040,
  "msg": "方剂不存在",
  "data": null
}
```

- **系统错误 (`code: 50000`)**
```json
{
  "code": 50000,
  "msg": "系统内部错误",
  "data": null
}
```

---

## 5. 注意事项

1. 所有请求需要在Header中包含认证信息
2. 分页查询最大每页数量限制为100条
3. 方剂推荐功能基于症状匹配算法，仅供参考
4. 中药组合分析数据来源于历史方剂统计
5. 方剂比较最多支持同时比较5个方剂

---

## 6. 使用示例

### 6.1 查询包含"桂枝"的方剂

```bash
GET /formula/page?keyword=桂枝&page=1&size=10
```

### 6.2 获取指定方剂详情

```bash
GET /formula/1
```

### 6.3 基于症状推荐方剂

```bash
POST /formula/recommend
Content-Type: application/json

{
  "symptoms": ["发热", "恶风", "汗出"]
}
```

### 6.4 分析桂枝的配伍规律

```bash
GET /formula/analysis/herb-combinations?herbName=桂枝
```

### 6.5 比较两个方剂

```bash
POST /formula/compare
Content-Type: application/json

[1, 2]
``` 