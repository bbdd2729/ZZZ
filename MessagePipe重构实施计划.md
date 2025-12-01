# MessagePipe事件系统重构实施计划

## 项目概述
将现有的事件系统从基于委托的EventBus架构迁移到高性能的MessagePipe消息管道系统。

## 当前架构分析

### 现有组件
- **EventBus**: 单例模式的事件总线
- **GameEvent<T>**: 泛型事件容器
- **EventBusAdapter**: IEventBus接口适配器
- **事件定义**: 分散在各个文件中

### 主要问题
1. 性能瓶颈：委托调用开销大
2. 内存分配：每次发布都涉及分配
3. 无异步支持
4. 缺乏消息过滤功能
5. 线程安全问题

## MessagePipe优势
- 零分配消息发布
- 原生异步支持
- 消息过滤和路由
- 高性能内存池
- 依赖注入集成

## 实施步骤

### 第一阶段：依赖包添加
1. 添加MessagePipe核心包
2. 添加MessagePipe.Interprocess用于跨进程通信
3. 添加MessagePack用于消息序列化

### 第二阶段：基础架构创建
1. 创建MessagePipe事件定义
2. 实现MessagePipeEventBusAdapter
3. 配置依赖注入

### 第三阶段：事件迁移
1. 迁移PlayerSwitchCompletedEvent
2. 迁移PlayerSwitchedEvent
3. 迁移PlayerSpawnedEvent
4. 迁移输入事件系统

### 第四阶段：测试和优化
1. 功能测试
2. 性能对比
3. 代码清理

## 详细实施计划

### 1. 添加依赖包
```xml
<package id="MessagePipe" version="1.8.1" manuallyInstalled="true" />
<package id="MessagePipe.Interprocess" version="1.3.1" />
<package id="MessagePack" version="2.5.192" />
```

### 2. 创建基础架构
- MessagePipeEventBusAdapter.cs
- MessagePipeEventDefinitions.cs
- MessagePipeConfiguration.cs

### 3. 迁移事件定义
- 添加MessagePack序列化支持
- 保持向后兼容性
- 优化事件结构

### 4. 更新依赖注入
- 配置MessagePipe服务
- 注册发布者和订阅者
- 设置生命周期管理

### 5. 代码迁移
- 替换Publish调用
- 替换Subscribe调用
- 添加异步支持

## 性能目标
- 事件发布延迟降低50%
- 内存分配减少80%
- 支持10000+事件/秒处理能力

## 风险评估
- 兼容性问题：保持IEventBus接口不变
- 性能回退：保留旧系统作为备选
- 学习成本：提供详细文档

## 时间计划
- 第1-2天：基础架构和依赖添加
- 第3-4天：事件定义迁移
- 第5-6天：代码迁移和测试
- 第7天：性能优化和文档